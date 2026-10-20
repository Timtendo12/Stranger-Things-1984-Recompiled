using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (EulerAnglesTweenProperty @ 0x016d318c..0x016d6e18, Ghidra vaddr).
public class EulerAnglesTweenProperty : AbstractVector3TweenProperty
{
	private bool _useLocalEulers;

	public bool useLocalEulers => _useLocalEulers;

	public EulerAnglesTweenProperty(Vector3 endValue, bool isRelative = false, bool useLocalEulers = false)
		: base(endValue, isRelative)
	{
		_useLocalEulers = useLocalEulers;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (base.Equals(obj))
		{
			return _useLocalEulers == ((EulerAnglesTweenProperty)obj)._useLocalEulers;
		}
		if (obj is RotationTweenProperty)
		{
			return _useLocalEulers == ((RotationTweenProperty)obj).useLocalRotation;
		}
		return false;
	}

	public override void prepareForUse()
	{
		_target = _ownerTween.target as Transform;
		_endValue = _originalEndValue;
		if (_ownerTween.isFrom)
		{
			_startValue = _endValue;
			_endValue = (_useLocalEulers ? _target.localEulerAngles : _target.eulerAngles);
		}
		else
		{
			_startValue = (_useLocalEulers ? _target.localEulerAngles : _target.eulerAngles);
		}
		base.prepareForUse();
	}

	public override void tick(float totalElapsedTime)
	{
		float num = _easeFunction(totalElapsedTime, 0f, 1f, _ownerTween.duration);
		Vector3 vector = new Vector3(_startValue.x + _diffValue.x * num, _startValue.y + _diffValue.y * num, _startValue.z + num * _diffValue.z);
		if (_useLocalEulers)
		{
			_target.localEulerAngles = vector;
		}
		else
		{
			_target.eulerAngles = vector;
		}
	}
}
