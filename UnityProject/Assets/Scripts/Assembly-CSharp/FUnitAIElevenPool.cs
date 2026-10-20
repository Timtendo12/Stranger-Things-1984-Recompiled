using System.Collections;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIElevenPool @ 0x1745154..0x1745eb0, Ghidra vaddr).
public class FUnitAIElevenPool : FUnitAIBase
{
	public FUnitAIElevenPool(FUnit owner)
		: base(owner)
	{
	}

	protected override void Init()
	{
		base.Init();
		mOwner.AddToTile(mOwner.TilePosition + new FPoint2(1, 0));
		mOwner.AddToTile(mOwner.TilePosition + new FPoint2(1, -1));
		mOwner.AddToTile(mOwner.TilePosition + new FPoint2(0, -1));
	}

	public void DoOpen(FUnit triggeredBy, bool force = false)
	{
		string leadsTo = mOwner.StartData.GetObjectProperty<string>("LeadsTo");
		if (!leadsTo.Valid())
		{
			return;
		}
		string[] parts = leadsTo.Split('.');
		if (parts.Length < 2)
		{
			FLog.Warning("Eleven Pool has LeadsTo property of: " + leadsTo + " which is not in the correct format. Please contact your service provider.");
			return;
		}
		FPlayerStartParams startParams = new FPlayerStartParams();
		startParams.EntryPoint = parts[1];
		startParams.PlaySound = "Will_Pipe_Out";
		FGameUtil.StartCoroutine(DoJump(parts[0], startParams));
	}

	protected IEnumerator DoJump(string mapName, FPlayerStartParams startParams)
	{
		FWorld.Player.AI.StopActions();
		FWorld.Player.EnableInput(false);
		FWorld.Player.PlayAnim("Jump_Intro", false);
		FWorld.Player.QueueAnim("Jump_Loop");
		System.Collections.Generic.List<Vector3> nodes = new System.Collections.Generic.List<Vector3>();
		Vector3 start = FWorld.Player.Position;
		Vector3 ownerPos = mOwner.Position;
		Vector3 end = new Vector3(ownerPos.x + 0.5f, ownerPos.y - 0.5f, start.z);
		nodes.Add(start);
		nodes.Add(new Vector3(start.x + (end.x - start.x) * 0.5f + Vector3.up.x * 2f, start.y + (end.y - start.y) * 0.5f + Vector3.up.y * 2f, start.z + (end.z - start.z) * 0.5f + Vector3.up.z * 2f));
		nodes.Add(end);
		GoSpline spline = new GoSpline(nodes);
		Go.to(FWorld.Player.GO.transform, 0.5f, new GoTweenConfig().positionPath(spline).setUpdateType(GoUpdateType.FWorldUpdate));
		yield return new WaitForSeconds(0.5f);
		FWorld.Player.SetIdle("Jump_End_Loop", true, true);
		FWorld.Player.PlayAnim("Jump_End", false);
		yield return new WaitForSeconds(FWorld.Player.GetCurrentAnimLength());
		FWorld.Player.SetVisible(false);
		mOwner.World.TryWorldTransition(mapName, null, startParams);
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (base.ProcessMessage(m))
		{
			return true;
		}
		if (m.Message == "Interact")
		{
			bool canOpen = mOwner.ProtoUnit.CheckInteractRequirements();
			if (!canOpen)
			{
				string currentCharacter = FApp.PlayerStats.CurrentCharacter;
				string requiredCharacter = mOwner.ProtoUnit.GetInteractRequirementsRequiredCharacter();
				if (!string.IsNullOrEmpty(requiredCharacter) && requiredCharacter != FApp.PlayerStats.CurrentCharacter && (FApp.PlayerStats.HasCollectible(requiredCharacter) || requiredCharacter == "Hopper"))
				{
					mOwner.World.SetPlayerCharacter(requiredCharacter);
					if (mOwner.ProtoUnit.CheckInteractRequirements())
					{
						canOpen = true;
					}
					else
					{
						mOwner.World.SetPlayerCharacter(currentCharacter);
					}
				}
			}
			if (canOpen)
			{
				if (m.SenderID >= 0)
				{
					FUnit sender = mOwner.World.GetUnitByID(m.SenderID);
					if (sender != null && sender.IsPlayer)
					{
						DoOpen(sender);
					}
				}
				return true;
			}
		}
		return false;
	}
}
