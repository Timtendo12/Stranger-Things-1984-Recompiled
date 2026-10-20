using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FEntityActionTweenPropertyByColor, Ghidra vaddr, 0x164dd90..0x1650000 region).
// Note: shipped ByColor.OnUpdate computes (tween - initial) exactly like ToColor.
public class FEntityActionTweenPropertyByColor : FEntityActionTweenProperty<Color>
{
	public FEntityActionTweenPropertyByColor(float duration, string propertyName, Color tweenValue, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, tweenValue, easeType)
	{
	}

	public FEntityActionTweenPropertyByColor(float duration, string propertyName, Color initialValue, Color tweenValue, GoEaseType easeType = GoEaseType.Linear)
		: base(duration, propertyName, initialValue, tweenValue, easeType)
	{
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		mPropertySetter(GoTweenUtils.unclampedColorLerp(mInitialValue, mTweenValue - mInitialValue, EasedT));
		return mElapsed < mDuration - FEntityAction.DeltaTimeThreshhold;
	}
}
