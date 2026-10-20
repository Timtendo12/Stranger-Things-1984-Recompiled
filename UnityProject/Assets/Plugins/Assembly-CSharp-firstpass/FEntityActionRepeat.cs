// Rewritten from libil2cpp.so 1.0.605 (FEntityActionRepeat @ 0x164cad8..0x164ccd0, Ghidra vaddr).
public class FEntityActionRepeat : FEntityActionSingleContainer
{
	protected int mRepeatCount;

	protected int mIteration;

	public int RepeatCount => mRepeatCount;

	public int Iteration => mIteration;

	public FEntityActionRepeat(int repeatCount, FEntityAction child)
		: base(child)
	{
		mRepeatCount = repeatCount;
	}

	public override void OnSetTarget()
	{
		mChild.SetTarget(mTarget);
		if (mRepeatCount < 1)
		{
			mDuration = FEntityAction.Infinite;
		}
		else if (mChild.IndeterminateLength)
		{
			mDuration = FEntityAction.Indeterminate;
		}
		else
		{
			mDuration = mRepeatCount * mChild.Duration;
		}
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		Fix64 remainder = mChild.Update(deltaTime);
		if (mChild.State == FEntityActionState.Stopped)
		{
			mIteration++;
			if (mRepeatCount >= 0 && mRepeatCount <= mIteration)
			{
				return false;
			}
			mChild.Start();
			mChild.Update(remainder);
		}
		return true;
	}

	public override void OnStart()
	{
		mIteration = 0;
		mChild.Start();
	}
}
