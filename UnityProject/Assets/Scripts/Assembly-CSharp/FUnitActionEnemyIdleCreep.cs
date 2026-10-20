// Rewritten from libil2cpp.so 1.0.605 (FUnitActionEnemyIdleCreep @ 0x17298e8..0x1729b5c, Ghidra vaddr).
public class FUnitActionEnemyIdleCreep : FUnitActionEnemyIdleBase
{
	protected float mPatrolPointSwitchTime;

	protected bool mReverse;

	public FUnitActionEnemyIdleCreep(int newID, FUnit owner, bool reverse)
		: base(newID, owner)
	{
		mReverse = reverse;
		mPatrolPointSwitchTime = mOwner.World.Gametime;
		if (mMyAI.IsOnPatrolPoint())
		{
			mPatrolPointSwitchTime += FRandom.Generic.RangeFloat(mOwner.ProtoUnit.MinPatrolPointPause, mOwner.ProtoUnit.MaxPatrolPointPause);
		}
	}

	public override bool Evaluate()
	{
		if (mState == States.Finished)
		{
			return true;
		}
		if (mOwner.World != null && mPatrolPointSwitchTime <= mOwner.World.Gametime)
		{
			int index = mMyAI.FindNextPatrolPointIndex(false, mReverse);
			if (index >= 0)
			{
				mMyAI.CurrentPatrolPointIndex = index;
				mMyAI.DoMoveToPoint(mMyAI.PatrolPoints[index], 0, false, false, true, string.Empty, -1f);
				return true;
			}
		}
		return false;
	}

	public override bool CollidedWithUnit(FUnit other)
	{
		return false;
	}
}
