using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitActionWork @ 0x172ba24..0x172c2a0, Ghidra vaddr).
public class FUnitActionWork : FUnitActionBase
{
	public enum States
	{
		None = 0,
		Work = 1,
		Fail = 2,
		Success = 3
	}

	protected States mState;

	protected FUnit mTargetUnit;

	public States State => mState;

	public override int TargetID
	{
		get
		{
			if (mTargetUnit != null)
			{
				return mTargetUnit.ID;
			}
			return -1;
		}
	}

	public FUnitActionWork(int newID, FUnit owner, FUnit targetUnit)
		: base(newID, owner)
	{
		mTargetUnit = targetUnit;
		mState = States.None;
	}

	public override bool GetIsFailed()
	{
		return mState == States.Fail;
	}

	public override bool GetIsSuccessful()
	{
		return mState == States.Success;
	}

	public override bool GetIsFinished()
	{
		return mState == States.Fail || mState == States.Success;
	}

	public override bool Start()
	{
		if (mTargetUnit != null && mOwner != null && mState == States.None)
		{
			mState = States.Work;
			return true;
		}
		return false;
	}

	public void SetState(States newState)
	{
		mState = newState;
	}

	public override void Update(float deltaTime)
	{
		if (IsFinished)
		{
			return;
		}
		if (mState == States.None)
		{
			Start();
			if (mState == States.None)
			{
				mState = States.Fail;
				return;
			}
		}
		DoWork();
	}

	public virtual void DoWork()
	{
		Vector3 targetPos = mTargetUnit.GO.transform.position;
		Vector3 myPos = mOwner.GO.transform.position;
		mOwner.SetFacing(FUnit.GetFacingFromDir(new Vector2(targetPos.x - myPos.x, targetPos.y - myPos.y)));
		mState = States.Success;
	}
}
