using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FEntityActionTweenPropertyToVector3, Ghidra vaddr, 0x164dd90..0x1650000 region).
public class FEntityActionTweenPropertyToVector3 : FEntityActionTweenProperty<Vector3>
{
	public FEntityActionTweenPropertyToVector3(float duration, string propertyName, Vector3 tweenValue, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, tweenValue, easeType)
	{
	}

	public FEntityActionTweenPropertyToVector3(float duration, string propertyName, Vector3 initialValue, Vector3 tweenValue, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, initialValue, tweenValue, easeType)
	{
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		mPropertySetter(GoTweenUtils.unclampedVector3Lerp(mInitialValue, mTweenValue - mInitialValue, EasedT));
		return mElapsed < mDuration - FEntityAction.DeltaTimeThreshhold;
	}
}
