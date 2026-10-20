using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FEntityActionTweenPropertyToVector2, Ghidra vaddr, 0x164dd90..0x1650000 region).
public class FEntityActionTweenPropertyToVector2 : FEntityActionTweenProperty<Vector2>
{
	public FEntityActionTweenPropertyToVector2(float duration, string propertyName, Vector2 tweenValue, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, tweenValue, easeType)
	{
	}

	public FEntityActionTweenPropertyToVector2(float duration, string propertyName, Vector2 initialValue, Vector2 tweenValue, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, initialValue, tweenValue, easeType)
	{
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		mPropertySetter(GoTweenUtils.unclampedVector2Lerp(mInitialValue, mTweenValue - mInitialValue, EasedT));
		return mElapsed < mDuration - FEntityAction.DeltaTimeThreshhold;
	}
}
