using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (Vector3ZTweenProperty @ 0x016d3f34..0x016d64d4, Ghidra vaddr).
public class Vector3ZTweenProperty : Vector3XTweenProperty
{
	public Vector3ZTweenProperty(string propertyName, float endValue, bool isRelative = false)
		: base(propertyName, endValue, isRelative)
	{
	}

	public override void prepareForUse()
	{
		_getter = GoTweenUtils.getterForProperty<System.Func<Vector3>>(_ownerTween.target, propertyName);
		_endValue = _originalEndValue;
		if (_ownerTween.isFrom)
		{
			_startValue = _endValue;
			_endValue = _getter().z;
		}
		else
		{
			_startValue = _getter().z;
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
		Vector3 obj = _getter();
		obj.z = _easeFunction(totalElapsedTime, _startValue, _diffValue, _ownerTween.duration);
		_setter(obj);
	}
}
