using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (PositionTweenProperty @ 0x016d2e3c..0x016d77c8, Ghidra vaddr).
public class PositionTweenProperty : AbstractVector3TweenProperty
{
	protected bool _useLocalPosition;

	public bool useLocalPosition => _useLocalPosition;

	public PositionTweenProperty(Vector3 endValue, bool isRelative = false, bool useLocalPosition = false)
		: base(endValue, isRelative)
	{
		_useLocalPosition = useLocalPosition;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (base.Equals(obj))
		{
			return _useLocalPosition == ((PositionTweenProperty)obj)._useLocalPosition;
		}
		if (obj is PositionPathTweenProperty)
		{
			return _useLocalPosition == ((PositionPathTweenProperty)obj).useLocalPosition;
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
			_endValue = (_useLocalPosition ? _target.localPosition : _target.position);
		}
		else
		{
			_startValue = (_useLocalPosition ? _target.localPosition : _target.position);
		}
		base.prepareForUse();
	}

	public override void tick(float totalElapsedTime)
	{
		float num = _easeFunction(totalElapsedTime, 0f, 1f, _ownerTween.duration);
		Vector3 vector = new Vector3(_startValue.x + _diffValue.x * num, _startValue.y + _diffValue.y * num, _startValue.z + num * _diffValue.z);
		if (_useLocalPosition)
		{
			_target.localPosition = vector;
		}
		else
		{
			_target.position = vector;
		}
	}
}
