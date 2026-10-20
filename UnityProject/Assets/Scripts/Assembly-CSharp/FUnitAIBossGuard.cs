using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIBossGuard @ 0x17330c4..0x17344e4, Ghidra vaddr).
public class FUnitAIBossGuard : FUnitAIBase
{
	public enum GuardState
	{
		NONE = 0,
		CHARGE = 1,
		DEFEND = 2
	}

	public bool FinalForm;

	public bool Invincible;

	protected FUnit mBrain;

	protected int mShieldHealth;

	protected bool mCheckForPlayer;

	protected int mLastHP;

	protected float mTimeTilDefend;

	protected float mTimeTilCharge;

	protected Vector2 mChargePos;

	public int Index { get; protected set; }

	public bool IsReady => mActions.Count == 0;

	public GuardState State { get; set; }

	public FUnitAIBossGuard(FUnit owner)
		: base(owner)
	{
		mShieldHealth = 2;
		mCheckForPlayer = true;
		Index = owner.StartData.GetObjectProperty<int>("GuardIndex");
		mShieldHealth = 1;
		State = GuardState.NONE;
		mLastHP = owner.HP;
	}

	protected override void Init()
	{
		mInitialized = true;
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		if (mOwner.HP <= 1)
		{
			return;
		}
		if (State == GuardState.CHARGE)
		{
			if (mTimeTilDefend > 0f)
			{
				mTimeTilDefend -= deltaTime;
				if (mTimeTilDefend <= 0f)
				{
					StartDefend();
				}
			}
			else if (mTimeTilCharge > 0f)
			{
				mTimeTilCharge -= deltaTime;
				if (mTimeTilCharge <= 0f)
				{
					DoCharge();
				}
			}
		}
		else if (State == GuardState.DEFEND)
		{
			mOwner.FacePosition(FWorld.Player.Position);
		}
	}

	public override bool CollidedWithUnit(FUnit other)
	{
		if (other.IsPlayer)
		{
			mOwner.SendMessage(mBrain, "HitPlayer", "");
		}
		return false;
	}

	public void ChargePlayer()
	{
		StopActions();
		if (mOwner.AliveAndWell)
		{
			State = GuardState.CHARGE;
			mOwner.FacePosition(FWorld.Player.Position);
			mOwner.PlayAnim("ChargeWindUp");
			mTimeTilCharge = 2f;
			mOwner.ShowEmote("Anger", false);
			mChargePos = FWorld.Player.Position;
		}
	}

	protected void DoCharge()
	{
		StopActions();
		FUnitActionMove fUnitActionMove = new FUnitActionMove(mOwner.World.GetNextActionID(), mOwner);
		fUnitActionMove.SetTargetPosition(mChargePos.ToFPoint2(), 0f, false);
		fUnitActionMove.IsAttackRush = true;
		fUnitActionMove.MoveSuccessAction = (Action<FUnit>)Delegate.Combine(fUnitActionMove.MoveSuccessAction, (Action<FUnit>)delegate
		{
			ChargeComplete();
		});
		fUnitActionMove.MoveFailAction = (Action<FUnit>)Delegate.Combine(fUnitActionMove.MoveFailAction, (Action<FUnit>)delegate
		{
			ChargeComplete();
		});
		AddAction(fUnitActionMove);
	}

	protected void ChargeComplete()
	{
		FGameUtil.ExecuteNextFrame(delegate
		{
			AddAction(new FUnitActionAttack(mOwner.World.GetNextActionID(), mOwner, FWorld.Player, false, "Attack"));
		});
		mTimeTilDefend = 1f;
	}

	public void StartDefend()
	{
		State = GuardState.DEFEND;
		mOwner.SendMessage(mBrain, "ChargeComplete", "");
	}

	public void GoToFinalForm()
	{
		mOwner.SetObstructs(true);
		FinalForm = true;
		BounceAwayUnit("BossGuardHelmet");
		mOwner.PlayAnim("Transition");
		mOwner.ShowEmote("Anger", false);
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (m.SenderID >= 0)
		{
			FUnit unitByID = mOwner.World.GetUnitByID(m.SenderID);
			if (unitByID != null)
			{
				string message = m.Message;
				if (message == "Attacked")
				{
					int shield = mShieldHealth;
					if (shield >= 1)
					{
						int remaining = mShieldHealth;
						if (FApp.PlayerStats.CurrentCharacter == "Nancy")
						{
							remaining--;
							mShieldHealth = remaining;
						}
						if (remaining <= 0)
						{
							mOwner.World.SpawnEffect("PoofEffect", mOwner.Position, 200);
							mOwner.PlayAnim("Hit", false);
							BounceAwayUnit("BossGuardShield");
						}
					}
					FGameUtil.ExecuteAtEndOfFrame(CheckHP);
					return shield > 0;
				}
				if (message == "RegisterBrain")
				{
					mBrain = unitByID;
				}
			}
		}
		return false;
	}

	protected void CheckHP()
	{
		if (mLastHP == mOwner.HP)
		{
			mOwner.World.SpawnFlyOffText(FLoc.GetString("Blocked"), mOwner.Position, new Vector2(0f, 50f));
			return;
		}
		mLastHP = mOwner.HP;
		mOwner.SendMessage(mBrain, "GuardAttacked", "");
		mOwner.PlayAnim("Hit", false);
		if (mOwner.HP < 2)
		{
			mOwner.SendMessage(mBrain, "ImDed", "");
			StopActions();
			if (!Invincible)
			{
				mOwner.Die();
			}
		}
	}

	protected void BounceAwayUnit(string unitName)
	{
		int newID = -1;
		if (mOwner.World.SpawnUnitAtPosition(unitName, mOwner.TilePosition, out newID) != FRoom.SpawnUnitResult.Success || newID < 0)
		{
			return;
		}
		FUnit unit = mOwner.World.GetUnitByID(newID);
		if (unit == null)
		{
			return;
		}
		Transform transform = unit.GO.transform;
		transform.position = mOwner.Position + Vector3.up * 0.5f;
		float x = (FRandom.Generic.RandBool() ? (-2f) : 2f);
		Go.to(unit.GO.transform, 1f, new GoTweenConfig().vector3XProp("localPosition", x, true).setEaseType(GoEaseType.QuadOut));
		Go.to(unit.GO.transform, 1f, new GoTweenConfig().vector3YProp("position", -0.5f, true).setEaseType(GoEaseType.BounceOut).onComplete(delegate
		{
			PoofAwayUnit(unit);
		}));
	}

	protected void PoofAwayUnit(FUnit unit)
	{
		unit.SyncTilePosition();
		mOwner.World.SpawnEffect("PoofEffect", unit.Position, 200);
		unit.Die();
	}

	public override string GetAnimName(string anim)
	{
		int index = 1;
		if (mShieldHealth < 1)
		{
			index = 2;
		}
		if (FinalForm)
		{
			index += 2;
		}
		switch (anim)
		{
		case "Walk":
		case "ChargeWindUp":
		case "Idle":
		case "Attack":
		case "Hit":
		case "Charge":
		case "Transition":
			return anim + "0" + index;
		default:
			return anim;
		}
	}

}
