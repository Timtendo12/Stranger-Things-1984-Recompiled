// Rewritten from libil2cpp.so 1.0.605 (FEntityActionSequence @ 0x164ccd0..0x164d0fc, Ghidra vaddr).
public class FEntityActionSequence : FEntityActionContainer
{
	protected int mCurrentIndex;

	public FEntityActionSequence()
	{
	}

	public FEntityActionSequence(params FEntityAction[] actions)
		: base(actions)
	{
	}

	public override void OnSetTarget()
	{
		base.OnSetTarget();
		mDuration = Fix64.Zero;
		for (int i = 0; i < mChildren.Count; i++)
		{
			if (mChildren[i].IndeterminateLength)
			{
				mDuration = FEntityAction.Indeterminate;
			}
			else if (!IndeterminateLength)
			{
				mDuration += mChildren[i].Duration;
			}
		}
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		if (mCurrentIndex < mChildren.Count)
		{
			Fix64 remainder = mChildren[mCurrentIndex].Update(deltaTime);
			if (mChildren[mCurrentIndex].State == FEntityActionState.Stopped)
			{
				mCurrentIndex++;
				if (mCurrentIndex >= mChildren.Count)
				{
					return false;
				}
				mChildren[mCurrentIndex].Start();
				mChildren[mCurrentIndex].Update(remainder);
			}
			return true;
		}
		return false;
	}

	public override void OnStart()
	{
		mCurrentIndex = 0;
		if (mChildren.Count >= 1)
		{
			mChildren[0].Start();
		}
	}

	public override void OnStop()
	{
		for (int i = mCurrentIndex; i < mChildren.Count; i++)
		{
			if (mChildren[i].State == FEntityActionState.NotStarted)
			{
				mChildren[i].Start();
				mChildren[i].Update(Fix64.Zero);
			}
		}
	}
}
