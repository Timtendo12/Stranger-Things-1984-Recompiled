using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FEntityActionTweenPropertyByVector3, Ghidra vaddr, 0x164dd90..0x1650000 region).
public class FEntityActionTweenPropertyByVector3 : FEntityActionTweenProperty<Vector3>
{
	public FEntityActionTweenPropertyByVector3(float duration, string propertyName, Vector3 tweenValue, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, tweenValue, easeType)
	{
	}

	public FEntityActionTweenPropertyByVector3(float duration, string propertyName, Vector3 initialValue, Vector3 tweenValue, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, initialValue, tweenValue, easeType)
	{
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		mPropertySetter(GoTweenUtils.unclampedVector3Lerp(mInitialValue, mTweenValue, EasedT));
		return mElapsed < mDuration - FEntityAction.DeltaTimeThreshhold;
	}
}
