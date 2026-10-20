using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIEnemy @ 0x1740f70..0x17470dc, Ghidra vaddr).
public class FUnitAIEnemy : FUnitAIBase
{
	public enum EnemyState
	{
		IDLE = 0,
		AGGROD = 1,
		LEASHING = 2
	}

	public EnemyState State;

	protected bool mPayingAttention = true;

	protected bool mShouldAggro = true;

	protected bool mMustFaceTargetToAggro;

	public bool CanLeash = true;

	public bool Aggrod => State == EnemyState.AGGROD;

	public bool PayingAttention
	{
		get
		{
			return mPayingAttention;
		}
		set
		{
			SetPayingAttention(value);
		}
	}

	public FUnitAIEnemy(FUnit owner)
		: base(owner)
	{
		State = EnemyState.IDLE;
		mPayingAttention = true;
	}

	protected override void Init()
	{
		base.Init();
		if (mOwner.IsTag("AutoAggro"))
		{
			SetState(EnemyState.AGGROD);
		}
	}

	public override void Update(float deltaTime)
	{
		if (mSpawnPoint == FPoint2.invalid)
		{
			mSpawnPoint = mOwner.TilePosition;
		}
		if (!mPayingAttention)
		{
			if (State == EnemyState.AGGROD)
			{
				mPayingAttention = true;
			}
		}
		else if (State == EnemyState.IDLE || State == EnemyState.LEASHING)
		{
			if (mShouldAggro && FWorld.Player != null && mOwner.IsOnScreen)
			{
				bool hasMove = GetHasMoveAction();
				bool hasIdle = GetHasIdleAction();
				if (hasMove || hasIdle)
				{
					bool singleAxis;
					float range;
					if (mOwner.Weapon == null)
					{
						singleAxis = false;
						range = 1.2f;
					}
					else
					{
						singleAxis = mOwner.Weapon.SingleAxis;
						range = (float)mOwner.Weapon.Range + 0.2f;
					}
					bool aggro = false;
					if (mOwner.DistanceToUnitEdge(FWorld.Player, singleAxis) <= range)
					{
						aggro = mOwner.CanSee(FWorld.Player, false) && State != EnemyState.LEASHING;
					}
					bool shouldAggro = aggro;
					if (mMustFaceTargetToAggro)
					{
						Vector2 forward = mOwner.Forward();
						Vector3 playerPos = FWorld.Player.Position;
						Vector3 myPos = mOwner.Position;
						shouldAggro = aggro && forward.x * (playerPos.x - myPos.x) + forward.y * (playerPos.y - myPos.y) >= 0.75f;
					}
					if (shouldAggro)
					{
						SetState(EnemyState.AGGROD);
						if (hasMove)
						{
							StopActions();
						}
					}
				}
			}
		}
		else if (State == EnemyState.AGGROD)
		{
			if (mPatrolPoints.Count < 1)
			{
				if (mSpawnPoint.ManhattanDistanceTo(mOwner.TilePosition) >= mOwner.ProtoUnit.LeashDistance)
				{
					Leash();
				}
			}
			else if (mPatrolPoints.All((FPoint2 p) => p.ManhattanDistanceTo(mOwner.TilePosition) >= mOwner.ProtoUnit.LeashDistance))
			{
				Leash();
			}
		}
		if (mActions.Count == 0)
		{
			AddIdleAction(deltaTime);
		}
		base.Update(deltaTime);
	}

	protected void SetPayingAttention(bool payingAttention)
	{
		mPayingAttention = payingAttention;
		string message = (payingAttention ? "UnitOnTileUnStunned" : "UnitOnTileStunned");
		FTile tile = mOwner.World.GetTile(mOwner.TilePosition);
		if (tile != null)
		{
			tile.SendMessageToAllUnits(message, mOwner.ID);
		}
	}

	public void Leash()
	{
		if (CanLeash)
		{
			bool hasMove = GetHasMoveAction();
			bool hasIdle = GetHasIdleAction();
			if ((hasMove || hasIdle) && InputEnabled)
			{
				StopActions();
			}
			State = EnemyState.LEASHING;
			MoveToNextWaypoint();
		}
	}

	public bool MoveToNextWaypoint()
	{
		int index = FindNextPatrolPointIndex();
		FPoint2 target;
		if (index >= 0)
		{
			CurrentPatrolPointIndex = index;
			target = mPatrolPoints[index];
		}
		else
		{
			target = mSpawnPoint;
		}
		bool valid = target != FPoint2.invalid;
		if (valid)
		{
			List<int> ignore = new List<int>(1);
			ignore.Add(FWorld.Player.ID);
			DoMoveToPoint(target, 0, false, true, true, string.Empty, -1f, ignore);
		}
		return valid;
	}

	protected virtual void AddIdleAction(float deltaTime)
	{
		AddAction(new FUnitActionEnemyIdleBase(mOwner.World.GetNextActionID(), mOwner));
	}

	public override bool CollidedWithUnit(FUnit other)
	{
		if (other == null)
		{
			return false;
		}
		if (State != EnemyState.AGGROD && other.IsPlayer)
		{
			SetState(EnemyState.AGGROD);
		}
		return base.CollidedWithUnit(other);
	}

	public void SetState(EnemyState state)
	{
		if (state == EnemyState.AGGROD && State != EnemyState.AGGROD)
		{
			mOwner.ShowEmote("Surprise", false);
		}
		State = state;
	}
}
