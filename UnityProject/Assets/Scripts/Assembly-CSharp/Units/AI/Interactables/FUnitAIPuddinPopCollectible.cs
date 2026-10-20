using System.Collections.Generic;
using System.Linq;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIPuddinPopCollectible @ 0x174e070..0x174ed30, Ghidra vaddr).
public class FUnitAIPuddinPopCollectible : FUnitAICollectible
{
	protected const float cTimeBetweenCalls = 0.25f;

	protected const float cExpirationTime = 8f;

	protected FUnit mCalledUnit;

	protected float mLastCallAttempt;

	protected List<FUnit> mCallTargets = new List<FUnit>();

	protected float mExpirationTime;

	public FUnitAIPuddinPopCollectible(FUnit owner)
		: base(owner)
	{
		mOnlyPlayerCollectable = false;
		mExpirationTime = mOwner.World.Gametime + 8f;
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		if (!mCollected)
		{
			if (mCalledUnit == null)
			{
				if (mOwner.World.Gametime - mLastCallAttempt > 0.25f)
				{
					TryCallUnit();
					mLastCallAttempt = mOwner.World.Gametime;
				}
			}
			else if (mCalledUnit.AI.CurrentTargetTile != mOwner.TilePosition)
			{
				ReleaseCalledUnit();
			}
			if (mOwner.World.Gametime < mExpirationTime)
			{
				return;
			}
		}
		else if (mCalledUnit != null && mCalledUnit.AliveAndWell)
		{
			return;
		}
		ReleaseCalledUnit();
		mOwner.World.RemoveUnit(mOwner);
	}

	protected void ReleaseCalledUnit()
	{
		if (mCalledUnit != null)
		{
			FUnitAIEnemy enemyAI = mCalledUnit.AI as FUnitAIEnemy;
			if (enemyAI != null && !enemyAI.PayingAttention)
			{
				enemyAI.PayingAttention = true;
			}
			mCalledUnit = null;
		}
	}

	protected void TryCallUnit()
	{
		UnityEngine.Vector3 pos = mOwner.Position;
		mOwner.World.GetUnitsByCircleCast(new UnityEngine.Vector2(pos.x, pos.y), 10f, mCallTargets);
		IEnumerable<FUnit> ordered = mCallTargets.OrderBy((FUnit u) =>
		{
			UnityEngine.Vector3 a = mOwner.Position;
			UnityEngine.Vector3 b = u.Position;
			return UnityEngine.Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
		});
		foreach (FUnit candidate in ordered)
		{
			FUnitAIEnemy enemyAI = candidate.AI as FUnitAIEnemy;
			if (enemyAI != null && !candidate.ProtoUnit.ImmuneToStun && enemyAI.PayingAttention && candidate.CanSee(mOwner, false))
			{
				enemyAI.StopActions();
				enemyAI.DoMoveToPoint(mOwner.TilePosition, 0, false, true, true, string.Empty, -1f, null, null, delegate
				{
					enemyAI.PayingAttention = true;
				});
				enemyAI.PayingAttention = false;
				mCalledUnit = candidate;
				break;
			}
		}
	}

	public override void DoCollect(FUnit other)
	{
		if (other == null)
		{
			return;
		}
		if (mCalledUnit != null && mCalledUnit != other)
		{
			ReleaseCalledUnit();
		}
		if (other == FWorld.Player)
		{
			base.DoCollect(other);
			mCalledUnit = FWorld.Player;
			return;
		}
		FUnitAIEnemy enemyAI = other.AI as FUnitAIEnemy;
		if (enemyAI == null)
		{
			base.DoCollect(other);
			return;
		}
		other.ShowEmote("Love", false);
		if (!other.ProtoUnit.ImmuneToStun)
		{
			if (enemyAI.CurrentTargetTile != mOwner.TilePosition)
			{
				enemyAI.StopActions();
			}
			enemyAI.PayingAttention = false;
			enemyAI.AddAction(new FUnitActionWait(mOwner.World.GetNextActionID(), other, 10f));
			enemyAI.AddAction(new FUnitActionCallMethod(mOwner.World.GetNextActionID(), other, delegate
			{
				enemyAI.PayingAttention = true;
			}));
		}
		CompleteCollect();
	}
}
