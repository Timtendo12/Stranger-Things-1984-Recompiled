using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIBossGuardBrain @ 0x1734500..0x17370c4 + iterators up to 0x1738cc0, Ghidra vaddr).
public class FUnitAIBossGuardBrain : FUnitAIBase
{
	protected enum BossGuardState
	{
		IDLE = 0,
		ROTATION = 1,
		CHARGE = 2,
		CHARGE_WINDUP = 3,
		FINAL_BATTLE = 4
	}

	protected BossGuardState mState;

	protected float mTimer;

	protected List<FUnitAIBossGuard> mGuardUnits;

	protected List<FPoint2> mPathPoints;

	protected int mStartingHealth;

	protected bool mEncounterStarted;

	protected bool mFinalBattleStarted;

	protected int mBossStage;

	protected bool mAttackedThisCharge;

	protected List<FAnimationController> mAnimsToUpdate = new List<FAnimationController>();

	public FUnitAIBossGuardBrain(FUnit owner)
		: base(owner)
	{
	}

	protected override void Init()
	{
		if (mOwner.Room.State != FRoom.RoomStates.Current)
		{
			return;
		}
		mInitialized = true;
		mGuardUnits = (from g in mOwner.World.GetUnitsByName("BossGuard")
			orderby (g.AI as FUnitAIBossGuard).Index
			select g.AI as FUnitAIBossGuard).ToList();
		for (int i = 0; i < mGuardUnits.Count; i++)
		{
			FUnitAIBossGuard guard = mGuardUnits[i];
			mOwner.SendMessage(guard.Owner, "RegisterBrain", "");
			guard.Owner.AnimController.Animator.SpriteRenderer.enabled = false;
			mOwner.World.SpawnEffect("BossGuardReveal", guard.Owner, Vector2.zero, 200);
			mStartingHealth += guard.Owner.HP;
		}
		if (mPathPoints == null)
		{
			mPathPoints = mGuardUnits.Select((FUnitAIBossGuard g) => g.Owner.TilePosition).ToList();
		}
		FGameUtil.DelayExecute(1f, Enter);
		mOwner.World.InputEnabled = false;
	}

	protected void Enter()
	{
		mOwner.World.InputEnabled = false;
		StartEncounter();
	}

	protected void StartEncounter()
	{
		FGameUtil.StartCoroutine(DoStartEncounter());
	}

	protected IEnumerator DoStartEncounter()
	{
		mOwner.World.ForceMovePlayerToPoint(mOwner.TilePosition, true, null);
		for (int index = 0; index < 3; index++)
		{
			FApp.SoundManager.PlaySoundSet("Bush_Break");
			yield return new WaitForSeconds(1f);
		}
		for (int index = 3; index < 6; index++)
		{
			yield return new WaitForSeconds(1f);
		}
		yield return new WaitForSeconds(0.5f);
		for (int i = 0; i < mGuardUnits.Count; i++)
		{
			FUnitAIBossGuard guard = mGuardUnits[i];
			guard.Owner.AnimController.Animator.SpriteRenderer.enabled = true;
			TurnUnitInward(guard.Owner);
		}
		FApp.SoundManager.PlaySoundSet("Bush_Break");
		yield return new WaitForSeconds(2f);
		mOwner.World.ZoomOut();
		mOwner.World.BeginBossEncounter(mOwner);
		FApp.HUD.PageLegendWorld.SetBossHealth(mStartingHealth, mStartingHealth);
		yield return new WaitForSeconds(1f);
		mOwner.World.InputEnabled = true;
		mEncounterStarted = true;
	}

	protected void StartLeave()
	{
		mOwner.World.StartCoroutine(DoLeave());
	}

	protected IEnumerator DoLeave()
	{
		mOwner.World.InputEnabled = false;
		FUnitAIBossGuard remainingGuard = mGuardUnits[0];
		FAnimationController helicopterAnim = mOwner.World.SpawnDiscreteAnimation("BossGuardHelicopter", 200, "Play");
		if (helicopterAnim == null)
		{
			yield break;
		}
		FApp.HUD.PageLegendWorld.ShowBossHealth(false);
		mAnimsToUpdate.Add(helicopterAnim);
		Transform helicopterTransform = helicopterAnim.GO.transform;
		helicopterTransform.position = mOwner.Position + Vector3.up * 7f;
		Go.to(helicopterTransform, 1f, new GoTweenConfig().position(mOwner.Position + Vector3.up * 4f).setEaseType(GoEaseType.CircOut));
		FApp.SoundManager.FadeOutMusic(0.5f);
		FApp.SoundManager.PlaySoundSet("ForestBossHelicopter");
		yield return new WaitForSeconds(1f);
		FAnimationController ropeAnim = mOwner.World.SpawnDiscreteAnimation("BossGuardRope", -10, "Drop");
		if (ropeAnim == null)
		{
			yield break;
		}
		mAnimsToUpdate.Add(ropeAnim);
		ropeAnim.EndCurrentLoop();
		Transform ropeTransform = ropeAnim.GO.transform;
		ropeTransform.position = helicopterTransform.position;
		Go.to(ropeTransform, 1f, new GoTweenConfig().position(mOwner.Position).setEaseType(GoEaseType.BounceOut));
		yield return new WaitForSeconds(0.5f);
		remainingGuard.Owner.ShowEmote("Surprise", false);
		yield return new WaitForSeconds(0.5f);
		remainingGuard.Owner.FacePosition(FWorld.Player.TilePosition);
		remainingGuard.Owner.PlayAnim("Attack", false, null, false);
		yield return new WaitForSeconds(0.1f);
		FUnit player = FWorld.Player;
		player.Push(player.Back(), 8);
		yield return new WaitForSeconds(0.5f);
		remainingGuard.Owner.SetObstructs(false);
		remainingGuard.DoMoveToPoint(mOwner.TilePosition, 0, false, false, false, "Walk", -1f, null, delegate
		{
			Vector3 pos = mOwner.Position + Vector3.down;
			remainingGuard.Owner.FacePosition(new Vector2(pos.x, pos.y));
		}, null);
		yield return new WaitForSeconds(2f);
		Go.to(ropeTransform, 2f, new GoTweenConfig().position(mOwner.Position + Vector3.up * 3f));
		Go.to(remainingGuard.Owner.GO.transform, 2f, new GoTweenConfig().position(mOwner.Position + Vector3.up * 3f));
		Transform guardShadow = remainingGuard.Owner.GO.transform.Find("Shadow");
		if (guardShadow != null)
		{
			guardShadow.parent = null;
			Go.to(guardShadow, 1f, new GoTweenConfig().scale(0f).onComplete(delegate
			{
				UnityEngine.Object.Destroy(guardShadow.gameObject);
			}));
		}
		yield return new WaitForSeconds(2f);
		int bearID = -1;
		if (mOwner.World.SpawnUnitAtPosition("Bear", mOwner.TilePosition + new FPoint2(-6, 2), out bearID, true) != FRoom.SpawnUnitResult.Success)
		{
			yield break;
		}
		FUnit bear = mOwner.World.GetUnitByID(bearID);
		if (bear == null)
		{
			yield break;
		}
		bear.SetObstructs(false);
		Transform bearShadow = bear.GO.transform.Find("Shadow");
		if (bearShadow != null)
		{
			bearShadow.gameObject.SetActive(false);
		}
		bear.SetFacing(FAnimEntry.AnimFacing.RIGHT);
		bear.PlayAnim("Jump");
		Go.to(bear.GO.transform, 0.5f, new GoTweenConfig().position(remainingGuard.Owner.Position + Vector3.left * 0.5f));
		yield return new WaitForSeconds(0.5f);
		remainingGuard.Owner.PlayAnim("Hit", true, null, false);
		remainingGuard.Owner.GO.transform.parent = bear.GO.transform;
		Go.to(bear.GO.transform, 0.5f, new GoTweenConfig().position(mOwner.Position + Vector3.right * 3f));
		yield return new WaitForSeconds(0.5f);
		if (bearShadow != null)
		{
			bearShadow.gameObject.SetActive(true);
		}
		remainingGuard.Owner.SetVisible(false);
		bear.PlayAnim("Drag");
		bear.AI.DoMoveToPoint(mOwner.TilePosition + new FPoint2(8, 0), 0, false, false, false, "Drag", 2f);
		Go.to(ropeTransform, 0.5f, new GoTweenConfig().position(helicopterTransform.position).setEaseType(GoEaseType.BackIn));
		yield return new WaitForSeconds(0.5f);
		Go.to(ropeTransform, 0.5f, new GoTweenConfig().position(helicopterTransform.position + Vector3.up + Vector3.up).setEaseType(GoEaseType.CircIn));
		Go.to(helicopterTransform, 0.5f, new GoTweenConfig().position(helicopterTransform.position + Vector3.up + Vector3.up).setEaseType(GoEaseType.CircIn));
		yield return new WaitForSeconds(2f);
		mOwner.World.EndBossEncounter(true);
		mOwner.World.RemoveUnit(remainingGuard.Owner);
		yield return new WaitForSeconds(1f);
		mOwner.World.RemoveUnit(bear);
		UnityEngine.Object.Destroy(helicopterTransform.gameObject);
		UnityEngine.Object.Destroy(ropeTransform.gameObject);
		FinishLeave();
	}

	protected void FinishLeave()
	{
		mOwner.Dying = true;
		mOwner.DoLoot();
		mOwner.World.RemoveUnit(mOwner);
		mOwner.World.InputEnabled = true;
		FWorld.Player.EnableInput(true);
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		foreach (FAnimationController item in mAnimsToUpdate)
		{
			item.Update(deltaTime);
		}
		if (!mEncounterStarted)
		{
			return;
		}
		switch (mState)
		{
		case BossGuardState.IDLE:
			if (mTimer > 2f)
			{
				if (mBossStage == 0)
				{
					DoRotation();
					mState = BossGuardState.ROTATION;
					mTimer = 0f;
				}
				else if (AllGuardsReady())
				{
					mState = BossGuardState.CHARGE_WINDUP;
					mTimer = 0f;
				}
			}
			break;
		case BossGuardState.ROTATION:
			for (int i = 0; i < mGuardUnits.Count; i++)
			{
				TurnUnitTowardsPlayer(mGuardUnits[i].Owner);
			}
			if (AllGuardsReady())
			{
				DoWindup();
				mState = BossGuardState.CHARGE_WINDUP;
				mTimer = 0f;
			}
			break;
		case BossGuardState.CHARGE:
			if (!mAttackedThisCharge && AllGuardsReady())
			{
				for (int j = 0; j < mGuardUnits.Count; j++)
				{
					FUnitAIBossGuard guard = mGuardUnits[j];
					TurnUnitInward(guard.Owner);
					guard.AddAction(new FUnitActionAttack(mOwner.World.GetNextActionID(), guard.Owner, FWorld.Player, false, "Attack"));
				}
				mAttackedThisCharge = true;
			}
			if (mTimer > 4f)
			{
				if (mBossStage != 0)
				{
					mState = BossGuardState.CHARGE_WINDUP;
					mTimer = 0f;
					for (int k = 0; k < mGuardUnits.Count; k++)
					{
						TurnUnitInward(mGuardUnits[k].Owner);
					}
				}
				else
				{
					BackToPosition();
					mState = BossGuardState.IDLE;
					mTimer = 0f;
				}
			}
			break;
		case BossGuardState.CHARGE_WINDUP:
			if (mTimer > 2f)
			{
				mAttackedThisCharge = false;
				DoCharge();
				mState = BossGuardState.CHARGE;
				mTimer = 0f;
			}
			break;
		case BossGuardState.FINAL_BATTLE:
			if (mTimer > 2f && !mFinalBattleStarted && mGuardUnits.Count >= 1)
			{
				int index = FRandom.Generic.RangeInt(0, mGuardUnits.Count - 1);
				mGuardUnits[index].ChargePlayer();
				mFinalBattleStarted = true;
			}
			break;
		}
		mTimer += deltaTime;
	}

	protected void SetState(BossGuardState state)
	{
		switch (state)
		{
		case BossGuardState.IDLE:
			if (mState == BossGuardState.CHARGE)
			{
				BackToPosition();
			}
			break;
		case BossGuardState.ROTATION:
			if (mState == BossGuardState.IDLE)
			{
				DoRotation();
			}
			break;
		case BossGuardState.CHARGE:
			if (mState == BossGuardState.CHARGE_WINDUP)
			{
				mAttackedThisCharge = false;
				DoCharge();
			}
			break;
		case BossGuardState.CHARGE_WINDUP:
			if (mState == BossGuardState.ROTATION)
			{
				DoWindup();
			}
			break;
		}
		mState = state;
		mTimer = 0f;
	}

	protected bool AllGuardsReady()
	{
		return mGuardUnits.All((FUnitAIBossGuard g) => g.IsReady);
	}

	protected void TurnUnitInward(FUnit unit)
	{
		unit.FacePosition(mOwner.Position);
	}

	protected void TurnUnitTowardsPlayer(FUnit unit)
	{
		unit.FacePosition(FWorld.Player.Position);
	}

	public void BackToPosition()
	{
		for (int i = 0; i < mGuardUnits.Count; i++)
		{
			FUnitAIBossGuard guard = mGuardUnits[i];
			guard.StopActions();
			guard.Owner.SetObstructs(false);
			FPoint2 target = mPathPoints[guard.Index];
			guard.DoMoveToPoint(target, 0, false, false, true, "Walk", -1f, new List<int> { FWorld.Player.ID }, delegate(FUnit unit)
			{
				unit.SetObstructs(true);
				TurnUnitTowardsPlayer(unit);
			}, null);
		}
	}

	protected void DoCharge()
	{
		for (int i = 0; i < mGuardUnits.Count; i++)
		{
			FUnitAIBossGuard guard = mGuardUnits[i];
			guard.StopActions();
			TurnUnitInward(guard.Owner);
			guard.Owner.SetObstructs(true);
			FPoint2 offset = FPoint2.zero;
			FPoint2 target = mOwner.TilePosition;
			if (mBossStage == 1)
			{
				int dx = mOwner.TilePosition.x - guard.Owner.TilePosition.x;
				int sign = -4;
				if (dx != 0 && guard.Owner.TilePosition.x <= mOwner.TilePosition.x)
				{
					sign = 4;
				}
				offset = new FPoint2(dx + sign, offset.y);
				target = guard.Owner.TilePosition + offset;
			}
			else if (mBossStage == 0)
			{
				Vector3 diff = guard.Owner.Position - mOwner.Position;
				float dist = diff.magnitude;
				Vector2 dir = ((!(dist <= 1E-05f)) ? new Vector2(diff.x / dist, diff.y / dist) : Vector2.zero);
				offset = new FPoint2(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y));
				target = mOwner.TilePosition + offset;
			}
			guard.DoMoveToPoint(target, 0, false, false, false, "Charge", 12f);
		}
	}

	protected void DoWindup()
	{
		for (int i = 0; i < mGuardUnits.Count; i++)
		{
			FUnitAIBossGuard guard = mGuardUnits[i];
			guard.StopActions();
			if (mBossStage == 1)
			{
				TurnUnitInward(guard.Owner);
			}
			else
			{
				TurnUnitTowardsPlayer(guard.Owner);
			}
			guard.Owner.SetObstructs(true);
			guard.Owner.PlayAnim("ChargeWindUp");
		}
	}

	protected void DoRotation()
	{
		for (int i = 0; i < mGuardUnits.Count; i++)
		{
			FUnitAIBossGuard guard = mGuardUnits[i];
			List<FPoint2> list = new List<FPoint2>();
			int k = 0;
			for (int p = 0; p < mPathPoints.Count; p++)
			{
				FPoint2 point = mPathPoints[p];
				int num = ((p == guard.Index) ? p : k);
				list.Insert(p - num, point);
				k = num;
			}
			list.Add(mPathPoints[guard.Index]);
			FPath fPath = new FPath();
			fPath.AddWaypoints(list);
			guard.StopActions();
			guard.DoMoveToPoint(null, 0, fPath, false, false, string.Empty, false, delegate
			{
				if (mState == BossGuardState.ROTATION)
				{
					DoWindup();
				}
				mState = BossGuardState.CHARGE_WINDUP;
			}, null);
			if (guard.GetCurrentAction() is FUnitActionMove fUnitActionMove)
			{
				fUnitActionMove.SetAnimation = false;
			}
		}
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (m.SenderID < 0)
		{
			return false;
		}
		FUnit unit = mOwner.World.GetUnitByID(m.SenderID);
		if (unit == null)
		{
			return false;
		}
		string message = m.Message;
		if (message == "HitPlayer")
		{
			if (mBossStage == 1)
			{
				FWorld.Player.OnAttacked(mOwner, 1, unit.Forward(), 1, true);
				return true;
			}
			Vector3 pushDir = mOwner.Position - FWorld.Player.Position;
			int knockback = ((!(FWorld.Player.TilePosition == mOwner.TilePosition)) ? 1 : 0);
			FWorld.Player.OnAttacked(mOwner, 1, new Vector2(pushDir.x, pushDir.y), knockback, true);
			if (mState == BossGuardState.CHARGE)
			{
				BackToPosition();
				mState = BossGuardState.IDLE;
				mTimer = 0f;
			}
			return true;
		}
		if (message != "ImDed")
		{
			if (message == "GuardAttacked")
			{
				int totalHP = 0;
				foreach (FUnitAIBossGuard guardUnit in mGuardUnits)
				{
					totalHP += guardUnit.Owner.HP;
				}
				FApp.HUD.PageLegendWorld.SetBossHealth(totalHP, mStartingHealth);
				FApp.HUD.PageLegendWorld.FlashBossHealth();
				if (mGuardUnits.Count != 1 || totalHP != 1)
				{
					return true;
				}
				NextBossStage();
				return true;
			}
			if (message != "ChargeComplete")
			{
				return false;
			}
		}
		else
		{
			FUnitAIBossGuard deadGuard = unit.AI as FUnitAIBossGuard;
			if (mGuardUnits.Contains(deadGuard))
			{
				mGuardUnits.Remove(deadGuard);
			}
			int count = mGuardUnits.Count;
			if (count == 0)
			{
				NextBossStage();
				return true;
			}
			if (count > 2)
			{
				if (count > 4)
				{
					return true;
				}
				if (mBossStage != 0)
				{
					return true;
				}
				NextBossStage();
				return true;
			}
			if (mBossStage == 1)
			{
				NextBossStage();
				return true;
			}
			if (count == 1)
			{
				FUnitAIBossGuard last = mGuardUnits[0];
				if (!last.FinalForm)
				{
					CompleteFinalStageMove(last.Owner);
				}
				last.Invincible = true;
			}
		}
		FUnitAIBossGuard next;
		if (mGuardUnits.Count == 1)
		{
			next = mGuardUnits[0];
		}
		else
		{
			next = mGuardUnits.FirstOrDefault((FUnitAIBossGuard u) => u.Owner != unit);
			if (next == null)
			{
				return true;
			}
		}
		next.ChargePlayer();
		return true;
	}

	protected void NextBossStage()
	{
		int stage = mBossStage;
		mTimer = 0f;
		mBossStage = stage + 1;
		switch (stage)
		{
		case 2:
			FWorld.Player.EnableInput(false);
			StartLeave();
			break;
		case 1:
			mEncounterStarted = false;
			mOwner.World.ForceMovePlayerToPoint(mOwner.TilePosition, false, null);
			FWorld.Player.EnableInput(false);
			for (int i = 0; i < mGuardUnits.Count; i++)
			{
				FUnitAIBossGuard guard = mGuardUnits[i];
				guard.StopActions();
				guard.Owner.SetObstructs(false);
				FPoint2 target = mOwner.TilePosition + new FPoint2((i == 0) ? 4 : (-4), 0);
				guard.DoMoveToPoint(target, 0, false, false, false, "Walk", -1f, null, delegate(FUnit unit)
				{
					FGameUtil.ExecuteNextFrame(delegate
					{
						CompleteFinalStageMove(unit);
					});
				}, null);
			}
			break;
		case 0:
		{
			mEncounterStarted = false;
			FPoint2 playerTarget = mOwner.TilePosition + new FPoint2(0, -2);
			mOwner.World.ForceMovePlayerToPoint(playerTarget, false, delegate
			{
				FWorld.Player.EnableInput(true);
			});
			FWorld.Player.EnableInput(false);
			for (int j = 1; j - 1 < mGuardUnits.Count; j++)
			{
				FUnitAIBossGuard guard2 = mGuardUnits[j - 1];
				guard2.StopActions();
				guard2.Owner.SetObstructs(false);
				FPoint2 target2 = mPathPoints[j];
				guard2.DoMoveToPoint(target2, 0, false, false, false, "Walk", -1f, null, delegate(FUnit unit)
				{
					unit.SetObstructs(true);
					TurnUnitTowardsPlayer(unit);
					mEncounterStarted = true;
				}, null);
			}
			break;
		}
		}
	}

	protected void CompleteFinalStageMove(FUnit unit)
	{
		unit.SetObstructs(true);
		TurnUnitTowardsPlayer(unit);
		if (AllGuardsReady())
		{
			mState = BossGuardState.FINAL_BATTLE;
			mEncounterStarted = true;
			FWorld.Player.EnableInput(true);
			for (int i = 0; i < mGuardUnits.Count; i++)
			{
				mGuardUnits[i].GoToFinalForm();
			}
		}
	}
}
