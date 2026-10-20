using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (Vector2TweenProperty @ 0x016d36c4..0x016d5a70, Ghidra vaddr).
public class Vector2TweenProperty : AbstractTweenProperty, IGenericProperty
{
	private Action<Vector2> _setter;

	protected Vector2 _originalEndValue;

	protected Vector2 _startValue;

	protected Vector2 _endValue;

	protected Vector2 _diffValue;

	public string propertyName { get; private set; }

	public Vector2TweenProperty(string propertyName, Vector2 endValue, bool isRelative = false)
		: base(isRelative)
	{
		this.propertyName = propertyName;
		_originalEndValue = endValue;
	}

	public override bool validateTarget(object target)
	{
		_setter = GoTweenUtils.setterForProperty<Action<Vector2>>(target, propertyName);
		return _setter != null;
	}

	public override void prepareForUse()
	{
		Func<Vector2> func = GoTweenUtils.getterForProperty<Func<Vector2>>(_ownerTween.target, propertyName);
		_endValue = _originalEndValue;
		if (_ownerTween.isFrom)
		{
			_startValue = _endValue;
			_endValue = func();
		}
		else
		{
			_startValue = func();
		}
		if (_isRelative && !_ownerTween.isFrom)
		{
			_diffValue = _endValue;
		}
		else
		{
			_diffValue = _endValue - _startValue;
		}
	}

	public override void tick(float totalElapsedTime)
	{
		float value = _easeFunction(totalElapsedTime, 0f, 1f, _ownerTween.duration);
		_setter(GoTweenUtils.unclampedVector2Lerp(_startValue, _diffValue, value));
	}
}
