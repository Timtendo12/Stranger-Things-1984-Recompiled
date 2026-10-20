using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (Vector3TweenProperty @ 0x016d3834..0x016d5f84, Ghidra vaddr).
public class Vector3TweenProperty : AbstractVector3TweenProperty, IGenericProperty
{
	private Action<Vector3> _setter;

	public string propertyName { get; private set; }

	public Vector3TweenProperty(string propertyName, Vector3 endValue, bool isRelative = false)
		: base(endValue, isRelative)
	{
		this.propertyName = propertyName;
	}

	public override bool validateTarget(object target)
	{
		_setter = GoTweenUtils.setterForProperty<Action<Vector3>>(target, propertyName);
		return _setter != null;
	}

	public override void prepareForUse()
	{
		Func<Vector3> func = GoTweenUtils.getterForProperty<Func<Vector3>>(_ownerTween.target, propertyName);
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
		base.prepareForUse();
	}

	public override void tick(float totalElapsedTime)
	{
		float value = _easeFunction(totalElapsedTime, 0f, 1f, _ownerTween.duration);
		_setter(GoTweenUtils.unclampedVector3Lerp(_startValue, _diffValue, value));
	}
}
