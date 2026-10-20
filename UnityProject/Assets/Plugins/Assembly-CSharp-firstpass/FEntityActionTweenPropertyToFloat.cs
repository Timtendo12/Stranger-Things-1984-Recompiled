using System;

// Rewritten from libil2cpp.so 1.0.605 (FEntityActionTweenPropertyToFloat, Ghidra vaddr, 0x164dd90..0x1650000 region).
public class FEntityActionTweenPropertyToFloat : FEntityActionTweenProperty<float>
{
	public FEntityActionTweenPropertyToFloat(float duration, string propertyName, float tweenValue, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, tweenValue, easeType)
	{
	}

	public FEntityActionTweenPropertyToFloat(float duration, string propertyName, float initialValue, float tweenValue, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, initialValue, tweenValue, easeType)
	{
	}

	public FEntityActionTweenPropertyToFloat(float duration, string propertyName, float initialValue, float tweenValue, Func<float, float, float, float, float> easeFunc)
		: base(duration, propertyName, initialValue, tweenValue, easeFunc)
	{
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		float value = mEaseFunction((float)mElapsed, mInitialValue, mTweenValue - mInitialValue, (float)mDuration);
		mPropertySetter(value);
		return mElapsed < mDuration - FEntityAction.DeltaTimeThreshhold;
	}
}
