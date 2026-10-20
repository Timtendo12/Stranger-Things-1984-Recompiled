using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (MaterialColorTweenProperty @ 0x016d3458..0x016d6fac, Ghidra vaddr).
public class MaterialColorTweenProperty : AbstractColorTweenProperty
{
	private string _materialColorName;

	public MaterialColorTweenProperty(Color endValue, GoMaterialColorType colorName = GoMaterialColorType.Color, bool isRelative = false)
		: base(endValue, isRelative)
	{
		_materialColorName = "_" + colorName.ToString();
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (base.Equals(obj))
		{
			return _materialColorName == ((MaterialColorTweenProperty)obj)._materialColorName;
		}
		return false;
	}

	public override void prepareForUse()
	{
		_endValue = _originalEndValue;
		if (_ownerTween.isFrom)
		{
			_startValue = _endValue;
			_endValue = _target.GetColor(_materialColorName);
		}
		else
		{
			_startValue = _target.GetColor(_materialColorName);
		}
		base.prepareForUse();
	}

	public override void tick(float totalElapsedTime)
	{
		float value = _easeFunction(totalElapsedTime, 0f, 1f, _ownerTween.duration);
		_target.SetColor(_materialColorName, GoTweenUtils.unclampedColorLerp(_startValue, _diffValue, value));
	}
}
