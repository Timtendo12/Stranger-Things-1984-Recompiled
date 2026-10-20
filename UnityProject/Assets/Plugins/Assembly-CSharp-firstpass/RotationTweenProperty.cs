using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (RotationTweenProperty @ 0x016d32f0..0x016d7c08, Ghidra vaddr).
public class RotationTweenProperty : AbstractVector3TweenProperty
{
	private bool _useLocalRotation;

	public bool useLocalRotation => _useLocalRotation;

	public RotationTweenProperty(Vector3 endValue, bool isRelative = false, bool useLocalRotation = false)
		: base(endValue, isRelative)
	{
		_useLocalRotation = useLocalRotation;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (base.Equals(obj))
		{
			return _useLocalRotation == ((RotationTweenProperty)obj)._useLocalRotation;
		}
		if (obj is EulerAnglesTweenProperty)
		{
			return _useLocalRotation == ((EulerAnglesTweenProperty)obj).useLocalEulers;
		}
		return false;
	}

	private static float wrapDiff(float value)
	{
		value -= (float)(int)(value / 360f) * 360f;
		value = Mathf.Clamp(value, 0f, 360f);
		if (value > 180f)
		{
			value -= 360f;
		}
		return value;
	}

	public override void prepareForUse()
	{
		_target = _ownerTween.target as Transform;
		_endValue = _originalEndValue;
		if (_ownerTween.isFrom)
		{
			_startValue = _endValue;
			_endValue = (_useLocalRotation ? _target.localRotation.eulerAngles : _target.rotation.eulerAngles);
		}
		else
		{
			_startValue = (_useLocalRotation ? _target.localRotation.eulerAngles : _target.rotation.eulerAngles);
		}
		if (_isRelative && !_ownerTween.isFrom)
		{
			_diffValue = _startValue + _endValue;
		}
		else
		{
			Vector3 vector = _endValue - _startValue;
			_diffValue = new Vector3(wrapDiff(vector.x), wrapDiff(vector.y), wrapDiff(vector.z));
		}
	}

	public override void tick(float totalElapsedTime)
	{
		float num = _easeFunction(totalElapsedTime, 0f, 1f, _ownerTween.duration);
		Quaternion quaternion = Quaternion.Euler(new Vector3(_startValue.x + _diffValue.x * num, _startValue.y + _diffValue.y * num, _startValue.z + num * _diffValue.z));
		// Shipped code has these inverted relative to prepareForUse (useLocalRotation -> world rotation).
		if (_useLocalRotation)
		{
			_target.rotation = quaternion;
		}
		else
		{
			_target.localRotation = quaternion;
		}
	}
}
