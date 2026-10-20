using System;

// Rewritten from libil2cpp.so 1.0.605 (FEntityActionTweenProperty<T> @ 0x1f66104..0x1f66590, Ghidra vaddr).
public abstract class FEntityActionTweenProperty<T> : FEntityActionTweenPropertyBase
{
	protected Action<T> mPropertySetter;

	protected T mTweenValue;

	protected T mInitialValue;

	protected bool mGetInitialValueOnStart = true;

	protected Func<float, float, float, float, float> mEaseFunction = GoEaseLinear.EaseNone;

	public Action<T> PropertySetter => mPropertySetter;

	public T TweenValue => mTweenValue;

	public T InitialValue => mInitialValue;

	protected bool GetInitialValueOnStart => mGetInitialValueOnStart;

	public Func<float, float, float, float, float> EaseFunction
	{
		get
		{
			return mEaseFunction;
		}
		set
		{
			mEaseFunction = value;
		}
	}

	public float EasedT => mEaseFunction((float)mElapsed, 0f, 1f, (float)mDuration);

	public static float TestEaseOut(float t, float b, float c, float d, Func<float, float, float, float, float> ease1, Func<float, float, float, float, float> ease2, float cutover)
	{
		float cutoverTime = d * cutover;
		float cutoverChange = c * cutover;
		if (t >= cutoverTime)
		{
			return ease2(t - cutoverTime, b + cutoverChange, c - cutoverChange, d - cutoverTime);
		}
		return ease1(t, b, cutoverChange, cutoverTime);
	}

	public FEntityActionTweenProperty(float duration, string propertyName, T tweenValue, Func<float, float, float, float, float> easeFunc)
	{
		mPropertyName = propertyName;
		mTweenValue = tweenValue;
		mDuration = (Fix64)duration;
		mEaseFunction = easeFunc;
	}

	public FEntityActionTweenProperty(float duration, string propertyName, T tweenValue, GoEaseType easeType)
		: this(duration, propertyName, tweenValue, GoTweenUtils.easeFunctionForType(easeType))
	{
	}

	public FEntityActionTweenProperty(float duration, string propertyName, T initialValue, T tweenValue, Func<float, float, float, float, float> easeFunc)
		: this(duration, propertyName, tweenValue, easeFunc)
	{
		mInitialValue = initialValue;
		mGetInitialValueOnStart = false;
	}

	public FEntityActionTweenProperty(float duration, string propertyName, T initialValue, T tweenValue, GoEaseType easeType)
		: this(duration, propertyName, tweenValue, GoTweenUtils.easeFunctionForType(easeType))
	{
		mInitialValue = initialValue;
		mGetInitialValueOnStart = false;
	}

	public override void OnSetTarget()
	{
		base.OnSetTarget();
		mPropertySetter = GoTweenUtils.setterForProperty<Action<T>>(mTarget, mPropertyName);
	}

	public override void OnStart()
	{
		base.OnStart();
		if (mGetInitialValueOnStart)
		{
			Func<T> getter = GoTweenUtils.getterForProperty<Func<T>>(mTarget, mPropertyName);
			mInitialValue = ((getter != null) ? getter() : default(T));
		}
	}
}
