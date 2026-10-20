using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIBossGunman @ 0x1738d18..0x173b1a8 + iterator to 0x173b408, Ghidra vaddr).
public class FUnitAIBossGunman : FUnitAIEnemy
{
	protected enum GunmanState
	{
		IDLE = 0,
		SEARCHING = 1,
		FIRING = 2,
		WINDUP = 3
	}

	protected const float cFireAngleInterval = 40f;

	protected const float cStartFireAngle = 40f;

	protected int mBossStage;

	protected bool mEncounterStarted;

	protected int mLastHP;

	protected FUnit mChosenBlocker;

	protected float mThrowGrenadeTime;

	protected GunmanState mState;

	protected int mFireCounter;

	public FUnitAIBossGunman(FUnit owner)
		: base(owner)
	{
		mShouldAggro = false;
		mLastHP = owner.HP;
	}

	protected override void Init()
	{
		if (mOwner.Room.State != FRoom.RoomStates.Current)
		{
			return;
		}
		base.Init();
		FGameUtil.DelayExecute(1f, Enter);
		mOwner.World.InputEnabled = false;
		mOwner.SetFacing(FAnimEntry.AnimFacing.DOWN);
		mOwner.PlayAnim("Spawn_Idle");
	}

	protected void Enter()
	{
		mOwner.World.InputEnabled = false;
		mOwner.World.ZoomInToPosition(mOwner.Position);
		StartEncounter();
	}

	protected void StartLeave()
	{
		mOwner.SetSortingOrder(100);
		FinishLeave();
	}

	protected void FinishLeave()
	{
		mOwner.World.ZoomOut();
		mOwner.World.SpawnEffect("DeathPoofEffect", mOwner.Position, 200);
		mOwner.DoLoot();
		mOwner.World.RemoveUnit(mOwner);
		mOwner.World.InputEnabled = true;
	}

	protected void StartEncounter()
	{
		FGameUtil.StartCoroutine(DoStartEncounter());
	}

	protected IEnumerator DoStartEncounter()
	{
		FApp.SoundManager.PlaySoundSet("Elevator");
		yield return new WaitForSeconds(0.2f);
		mOwner.PlayAnim("Spawn", false);
		yield return new WaitForSeconds(2f);
		mOwner.World.ZoomOut();
		mOwner.World.BeginBossEncounter(mOwner);
		yield return new WaitForSeconds(1f);
		mOwner.World.InputEnabled = true;
		mEncounterStarted = true;
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
	}

	protected override void AddIdleAction(float deltaTime)
	{
		if (!mEncounterStarted || mState == GunmanState.WINDUP)
		{
			return;
		}
		Vector3 diff = FWorld.Player.Position - mOwner.Position;
		float dist = diff.magnitude;
		Vector2 dir = ((!(dist <= 1E-05f)) ? new Vector2(diff.x / dist, diff.y / dist) : Vector2.zero);
		float wait;
		if (Vector2.Dot(dir, mOwner.Forward()) <= 0.6f)
		{
			mOwner.PlayAnim("Idle");
			if (mState == GunmanState.FIRING)
			{
				wait = 1f;
			}
			else
			{
				mOwner.FacePosition(FWorld.Player.Position);
				wait = 2f;
			}
			mState = GunmanState.SEARCHING;
		}
		else
		{
			mOwner.PlayAnim("Attack");
			mState = GunmanState.FIRING;
			wait = 6f - (float)mBossStage;
		}
		AddAction(new FUnitActionWait(mOwner.World.GetNextActionID(), mOwner, wait));
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		string message = m.Message;
		if (message == "Attacked")
		{
			if (!mEncounterStarted)
			{
				return true;
			}
			FGameUtil.ExecuteAtEndOfFrame(CheckHP);
		}
		else if (message == "Fire")
		{
			Fire();
		}
		else if (message == "SpawnGrenade")
		{
			SpawnGrenade();
		}
		else if (message == "Push")
		{
			FUnit player = FWorld.Player;
			player.Push(player.Back(), 3);
			FGameUtil.DelayExecute(1f, mOwner.World.ZoomOut);
		}
		else if (message == "ResetFireCounter")
		{
			mFireCounter = 0;
		}
		return false;
	}

	protected void Fire()
	{
		float angle = 40f - (float)mFireCounter * 40f;
		Vector3 pos = mOwner.Position;
		Vector2 dir = Quaternion.Euler(0f, 0f, angle) * (Vector3)mOwner.Forward();
		FPoint2 target = new FPoint2(Mathf.RoundToInt(pos.x + dir.x * 10f), Mathf.RoundToInt(pos.y + dir.y * 10f));
		mOwner.Attack(null, target);
		mFireCounter++;
	}

	protected void DestroyBlocker()
	{
		FUnit[] blockers = mOwner.World.GetUnitsByName("HazmatGunnerBlocker");
		if (blockers != null)
		{
			if (blockers.Length != 0)
			{
				List<FUnit> candidates = blockers.Where((FUnit b) => b.Obstructs).ToList();
				if (!candidates.Any())
				{
					return;
				}
				mChosenBlocker = candidates.OrderBy((FUnit b) => b.TilePosition.DistanceTo(FWorld.Player.TilePosition)).First();
			}
			if (mChosenBlocker == null)
			{
				return;
			}
			mOwner.FacePosition(mChosenBlocker.Position);
			mOwner.PlayAnim("WindUp");
			AddAction(new FUnitActionWait(mOwner.World.GetNextActionID(), mOwner, 1f));
			AddAction(new FUnitActionCallMethod(mOwner.World.GetNextActionID(), mOwner, delegate
			{
				mOwner.World.InputEnabled = true;
			}));
			AddAction(new FUnitActionCallMethod(mOwner.World.GetNextActionID(), mOwner, ThrowGrenade));
		}
	}

	protected void ThrowGrenade()
	{
		mOwner.PlayAnim("Throw", false);
		AddAction(new FUnitActionWait(mOwner.World.GetNextActionID(), mOwner, 1f));
		AddAction(new FUnitActionCallMethod(mOwner.World.GetNextActionID(), mOwner, ResumeCombat));
	}

	protected void SpawnGrenade()
	{
		if (mChosenBlocker == null)
		{
			return;
		}
		int newID = -1;
		if (mOwner.World.SpawnUnitAtPosition("BossSewerGrenade", mOwner.TilePosition, out newID) != FRoom.SpawnUnitResult.Success || newID < 0)
		{
			return;
		}
		FUnit grenade = mOwner.World.GetUnitByID(newID);
		if (grenade != null)
		{
			grenade.SetSortingOrder(110);
			List<Vector3> nodes = new List<Vector3>();
			nodes.Add(mOwner.Position);
			nodes.Add(mOwner.Position + (mChosenBlocker.Position - mOwner.Position) * 0.5f + Vector3.up * 3f);
			nodes.Add(mChosenBlocker.Position);
			GoSpline path = new GoSpline(nodes);
			Go.to(grenade.GO.transform, 0.5f, new GoTweenConfig().positionPath(path).onComplete(delegate
			{
				PrimeGrenade(grenade);
			}).setUpdateType(GoUpdateType.FWorldUpdate));
		}
	}

	protected void ResumeCombat()
	{
		mEncounterStarted = true;
		mOwner.FacePosition(FWorld.Player.Position);
		mState = GunmanState.SEARCHING;
	}

	protected void PrimeGrenade(FUnit grenade)
	{
		grenade.ShowCountdown(true);
		grenade.PlayAnim("Landed");
		FGameUtil.DelayExecute(1.1f, delegate
		{
			FinishDestroyBlocker(grenade);
		});
	}

	protected void FinishDestroyBlocker(FUnit grenade)
	{
		if (mChosenBlocker == null)
		{
			return;
		}
		FUnit indicator = mOwner.World.GetUnitByName("GrenadeIndicator");
		if (indicator != null)
		{
			mOwner.World.RemoveUnit(indicator);
		}
		mOwner.World.RemoveUnit(grenade);
		mChosenBlocker.SetIdle("Idle_Destroyed", true, true);
		mChosenBlocker.PlayAnim("Hit", false);
		mChosenBlocker.SetObstructs(false);
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				FPoint2 fPoint = mChosenBlocker.TilePosition + new FPoint2(j, i);
				if (FWorld.Player.TilePosition == fPoint)
				{
					FWorld.Player.OnAttacked(mOwner, 1, Vector2.zero, 0, false);
				}
				if (j != 0 || i != 0)
				{
					mOwner.World.SpawnEffect("SmallExplosion", fPoint.ToVector2(), 200);
				}
			}
		}
		mChosenBlocker = null;
	}

	protected void CheckHP()
	{
		if (mLastHP == mOwner.HP)
		{
			mOwner.World.SpawnFlyOffText(FLoc.GetString("Deflect"), mOwner.Position, new Vector2(0f, 50f));
		}
		else
		{
			FApp.HUD.PageLegendWorld.SetBossHealth(mOwner.HP - 1, mOwner.ProtoUnit.Health - 1);
			FApp.HUD.PageLegendWorld.FlashBossHealth();
			mLastHP = mOwner.HP;
			mOwner.PlayAnim("Hit", false);
		}
		int hP = mOwner.HP;
		if (hP > 16)
		{
			return;
		}
		int bossStage = mBossStage;
		if (bossStage != 0)
		{
			if (hP > 11)
			{
				return;
			}
			if (bossStage != 1)
			{
				if (hP > 6)
				{
					return;
				}
				if (bossStage == 2)
				{
					NextBossStage();
				}
				if (mOwner.HP > 1 || mBossStage != 3)
				{
					return;
				}
			}
		}
		NextBossStage();
	}

	protected void NextBossStage()
	{
		mOwner.World.InputEnabled = false;
		mEncounterStarted = false;
		StopActions();
		mOwner.PlayAnim("Idle");
		mOwner.World.ZoomInToPosition(mOwner.Position);
		mBossStage++;
		mOwner.FacePosition(FWorld.Player.Position);
		if (mBossStage >= 1 && mBossStage <= 3)
		{
			mOwner.PlayAnim("Push", false);
			FGameUtil.DelayExecute(mOwner.GetCurrentAnimLength(), DestroyBlocker);
			mState = GunmanState.WINDUP;
		}
		else if (mBossStage == 4)
		{
			mOwner.RemoveAnimationLayers();
			mOwner.World.EndBossEncounter(true);
			mOwner.PlayAnim("Death");
			FGameUtil.DelayExecute(3f, StartLeave);
		}
	}

	public override string GetAnimName(string anim)
	{
		int num = mBossStage;
		if (num < 2)
		{
			num = 1;
		}
		switch (anim)
		{
		case "Throw":
		case "Walk":
		case "Idle":
		case "Enrage":
		case "Hit":
		case "WindUp":
		case "Attack":
			return anim + "0" + num;
		default:
			return anim;
		}
	}
}
