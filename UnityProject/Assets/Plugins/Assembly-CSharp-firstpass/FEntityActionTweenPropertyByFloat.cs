using System;

// Rewritten from libil2cpp.so 1.0.605 (FEntityActionTweenPropertyByFloat, Ghidra vaddr, 0x164dd90..0x1650000 region).
public class FEntityActionTweenPropertyByFloat : FEntityActionTweenProperty<float>
{
	public FEntityActionTweenPropertyByFloat(float duration, string propertyName, float tweenValue, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, tweenValue, easeType)
	{
	}

	public FEntityActionTweenPropertyByFloat(float duration, string propertyName, float initialValue, float tweenValue, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, initialValue, tweenValue, easeType)
	{
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		float value = mEaseFunction((float)mElapsed, mInitialValue, mTweenValue, (float)mDuration);
		mPropertySetter(value);
		return mElapsed < mDuration - FEntityAction.DeltaTimeThreshhold;
	}
}
