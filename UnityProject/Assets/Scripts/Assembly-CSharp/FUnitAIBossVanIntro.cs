using System.Collections;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIBossVanIntro @ 0x1742dc0..0x1743e40, Ghidra vaddr).
public class FUnitAIBossVanIntro : FUnitAIBase
{
	public int AnimIndex { get; set; }

	public FUnitAIBossVanIntro(FUnit owner)
		: base(owner)
	{
		AnimIndex = 1;
	}

	protected override void Init()
	{
		base.Init();
		mOwner.World.StartCoroutine(DoAnimation());
	}

	protected IEnumerator DoAnimation()
	{
		FProtoUnit mike = FApp.ProtoUnitManager.GetProtoUnit("Mike");
		if (mike != null)
		{
			mOwner.World.SetPlayerCharacter(mike, true);
		}
		FWorld.Player.EnableInput(false);
		mOwner.World.InputEnabled = false;
		Transform vanVis = mOwner.GO.transform.Find("Vis");
		if (vanVis != null)
		{
			vanVis.localPosition = vanVis.localPosition + Vector3.left * 3f;
		}
		FWorld.Player.AI.DoMoveToPoint(new FPoint2(FWorld.Player.TilePosition.x, mOwner.TilePosition.y), 0, false, false, false, string.Empty, -1f);
		yield return new WaitForSeconds(2f);
		mOwner.World.TryShowDialogue(mOwner);
		while (FApp.HUD.PageLegendWorld.DialogueOnScreen)
		{
			yield return 0;
		}
		mOwner.World.UnPause();
		yield return new WaitForSeconds(0.5f);
		if (vanVis != null)
		{
			Go.to(vanVis, 0.5f, new GoTweenConfig().localPosition(Vector3.zero).setEaseType(GoEaseType.CircOut).setUpdateType(GoUpdateType.FWorldUpdate));
		}
		FApp.SoundManager.PlaySoundSet("BossVan_Rev");
		yield return new WaitForSeconds(0.2f);
		FApp.SoundManager.PlaySoundSet("BossVan_Horn");
		yield return new WaitForSeconds(0.3f);
		mOwner.AnimController.Animator.SetAnimSpeed(0f);
		FWorld.Player.ShowEmote("Surprise", false);
		FWorld.Player.PlayAnim("Hit", false, null, false);
		yield return new WaitForSeconds(1f);
		mOwner.World.TryShowDialogue(mOwner);
		while (FApp.HUD.PageLegendWorld.DialogueOnScreen)
		{
			yield return 0;
		}
		mOwner.World.UnPause();
		FProtoUnit bike = FApp.ProtoUnitManager.GetProtoUnit("MikeOnABike");
		if (bike != null)
		{
			mOwner.World.SetPlayerCharacter(bike, true);
		}
		yield return new WaitForSeconds(0.5f);
		FPoint2 endPoint = new FPoint2(FWorld.Player.TilePosition.x + 9, FWorld.Player.TilePosition.y);
		FWorld.Player.AI.DoMoveToPoint(endPoint, 0, false, false, false, string.Empty, 4f);
		yield return new WaitForSeconds(1f);
		mOwner.AnimController.Animator.SetAnimSpeed(1f);
		FApp.SoundManager.PlaySoundSet("BossVan_Rev");
		DoMoveToPoint(endPoint, 0, false, false, false, string.Empty, -1f);
		yield return new WaitForSeconds(3f);
		if (!mOwner.World.ProtoWorld.LoadData.ContainsKey("LeadsTo"))
		{
			FApp.DestroyWorld(false);
			FGameUtil.StartCoroutine(FWorld.Init(FApp.PlayerStats.CurrentWorld, new FPlayerStartParams()));
		}
		else
		{
			mOwner.World.TryWorldTransition(mOwner.World.ProtoWorld.LoadData["LeadsTo"], null);
		}
	}

	public override string GetAnimName(string anim)
	{
		if (anim == "Walk")
		{
			anim = "Idle";
		}
		else if (anim != "Transition" && anim != "Idle" && anim != "Swerve" && anim != "Stun" && anim != "Hit")
		{
			return anim;
		}
		return anim + "0" + AnimIndex.ToString();
	}
}
