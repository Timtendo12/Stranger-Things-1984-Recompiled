using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (ScaleTweenProperty @ 0x016d300c..0x016d80f4, Ghidra vaddr).
public class ScaleTweenProperty : AbstractVector3TweenProperty
{
	public ScaleTweenProperty(Vector3 endValue, bool isRelative = false)
		: base(endValue, isRelative)
	{
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (base.Equals(obj))
		{
			return true;
		}
		return obj.GetType() == typeof(ScalePathTweenProperty);
	}

	public override void prepareForUse()
	{
		_target = _ownerTween.target as Transform;
		_endValue = _originalEndValue;
		if (_ownerTween.isFrom)
		{
			_startValue = _endValue;
			_endValue = _target.localScale;
		}
		else
		{
			_startValue = _target.localScale;
		}
		base.prepareForUse();
	}

	public override void tick(float totalElapsedTime)
	{
		float num = _easeFunction(totalElapsedTime, 0f, 1f, _ownerTween.duration);
		_target.localScale = new Vector3(_startValue.x + _diffValue.x * num, _startValue.y + _diffValue.y * num, _startValue.z + num * _diffValue.z);
	}
}
