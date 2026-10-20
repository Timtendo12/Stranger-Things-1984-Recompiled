using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIBikeRamp @ 0x1730728..0x1731470, Ghidra vaddr).
public class FUnitAIBikeRamp : FUnitAIBase
{
	protected string mLeadsTo = string.Empty;

	public FUnitAIBikeRamp(FUnit owner)
		: base(owner)
	{
		mLeadsTo = mOwner.StartData.GetObjectProperty<string>("LeadsTo");
	}

	protected void StartJump()
	{
		if (!mLeadsTo.Valid())
		{
			return;
		}
		FUnit target = mOwner.World.GetUnitByName(mLeadsTo);
		if (target == null)
		{
			return;
		}
		FUnit player = FWorld.Player;
		if (player.ProtoUnit.Name != "MikeOnABike")
		{
			FProtoUnit bikeProto = FApp.ProtoUnitManager.GetProtoUnit("MikeOnABike");
			if (bikeProto != null)
			{
				FWorld.Player.ChangeProto(bikeProto);
			}
			FApp.HUD.PageLegendWorld.SyncSpecialWeaponButton();
		}
		FPoint2 approach = mOwner.TilePosition + mOwner.BackFP2();
		FPoint2 targetPoint = target.TilePosition + target.BackFP2();
		FWorld.Player.EnableInput(false);
		FWorld.Player.AI.DoMoveToPoint(approach, 0, false, true, true, "Bike", -1f, null, delegate
		{
			DoJump(targetPoint);
		});
	}

	protected void DoJump(FPoint2 targetPoint)
	{
		FWorld.Player.SetObstructs(false);
		Vector3 start = FWorld.Player.Position;
		Vector2 end = targetPoint.ToVector2();
		List<Vector3> nodes = new List<Vector3>();
		nodes.Add(start);
		nodes.Add(new Vector3(start.x + (end.x - start.x) * 0.5f + Vector3.up.x * 2f, start.y + (end.y - start.y) * 0.5f + Vector3.up.y * 2f, start.z + (0f - start.z) * 0.5f + Vector3.up.z * 2f));
		nodes.Add(new Vector3(end.x, end.y, 0f));
		GoSpline spline = new GoSpline(nodes);
		FWorld.Player.PlayAnim("BikeJump");
		Go.to(FWorld.Player.GO.transform, 0.5f, new GoTweenConfig().positionPath(spline).setUpdateType(GoUpdateType.FWorldUpdate).onComplete(delegate
		{
			FinishJump();
		}));
		Go.to(FWorld.Player.GO.transform, 0.5f, new GoTweenConfig().localEulerAngles(new Vector3(0f, 0f, 360f), true).setUpdateType(GoUpdateType.FWorldUpdate));
	}

	protected void FinishJump()
	{
		FWorld.Player.SyncTilePosition();
		FWorld.Player.SetObstructs(true);
		FWorld.Player.EnableInput(true);
		FWorld.Player.SetIdle();
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (m.Message == "Interact")
		{
			if (!mOwner.ProtoUnit.CheckInteractRequirements() && !FWorld.Player.TrySwitchToRequiredCharacter(mOwner.ProtoUnit))
			{
				return false;
			}
			StartJump();
			return true;
		}
		return false;
	}
}
