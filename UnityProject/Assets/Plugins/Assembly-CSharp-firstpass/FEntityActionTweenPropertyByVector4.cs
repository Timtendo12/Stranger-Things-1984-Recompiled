using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FEntityActionTweenPropertyByVector4, Ghidra vaddr, 0x164dd90..0x1650000 region).
public class FEntityActionTweenPropertyByVector4 : FEntityActionTweenProperty<Vector4>
{
	public FEntityActionTweenPropertyByVector4(float duration, string propertyName, Vector4 tweenValue, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, tweenValue, easeType)
	{
	}

	public FEntityActionTweenPropertyByVector4(float duration, string propertyName, Vector4 initialValue, Vector4 tweenValue, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, initialValue, tweenValue, easeType)
	{
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		mPropertySetter(GoTweenUtils.unclampedVector4Lerp(mInitialValue, mTweenValue, EasedT));
		return mElapsed < mDuration - FEntityAction.DeltaTimeThreshhold;
	}
}
