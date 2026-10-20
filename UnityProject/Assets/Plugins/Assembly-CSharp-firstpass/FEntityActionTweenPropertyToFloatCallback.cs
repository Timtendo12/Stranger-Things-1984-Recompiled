using System;

// Rewritten from libil2cpp.so 1.0.605 (FEntityActionTweenPropertyToFloatCallback @ 0x164e02c..0x164e354, Ghidra vaddr).
public class FEntityActionTweenPropertyToFloatCallback : FEntityActionTweenProperty<float>
{
	protected Func<float> mTweenCallback;

	public FEntityActionTweenPropertyToFloatCallback(float duration, string propertyName, Func<float> tweenCallback, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, tweenCallback(), easeType)
	{
		mTweenCallback = tweenCallback;
	}

	public FEntityActionTweenPropertyToFloatCallback(float duration, string propertyName, float initialValue, Func<float> tweenCallback, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, initialValue, tweenCallback(), easeType)
	{
		mTweenCallback = tweenCallback;
	}

	public FEntityActionTweenPropertyToFloatCallback(float duration, string propertyName, float initialValue, Func<float> tweenCallback, Func<float, float, float, float, float> easeFunc)
		: base(duration, propertyName, initialValue, tweenCallback(), easeFunc)
	{
		mTweenCallback = tweenCallback;
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		mTweenValue = mTweenCallback();
		float value = mEaseFunction((float)mElapsed, mInitialValue, mTweenValue - mInitialValue, (float)mDuration);
		mPropertySetter(value);
		return mElapsed < mDuration - FEntityAction.DeltaTimeThreshhold;
	}
}
