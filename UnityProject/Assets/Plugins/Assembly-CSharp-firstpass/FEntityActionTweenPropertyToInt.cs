using System;

// Rewritten from libil2cpp.so 1.0.605 (FEntityActionTweenPropertyToInt, Ghidra vaddr, 0x164dd90..0x1650000 region).
public class FEntityActionTweenPropertyToInt : FEntityActionTweenProperty<int>
{
	public FEntityActionTweenPropertyToInt(float duration, string propertyName, int tweenValue, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, tweenValue, easeType)
	{
	}

	public FEntityActionTweenPropertyToInt(float duration, string propertyName, int initialValue, int tweenValue, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, initialValue, tweenValue, easeType)
	{
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		float value = mEaseFunction((float)mElapsed, (float)mInitialValue, (float)(mTweenValue - mInitialValue), (float)mDuration);
		mPropertySetter((int)value);
		return mElapsed < mDuration - FEntityAction.DeltaTimeThreshhold;
	}
}
