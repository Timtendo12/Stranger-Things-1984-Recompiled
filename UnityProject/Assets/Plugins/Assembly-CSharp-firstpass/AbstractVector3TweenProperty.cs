using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (AbstractVector3TweenProperty @ 0x016d510c..0x016d5258, Ghidra vaddr).
public abstract class AbstractVector3TweenProperty : AbstractTweenProperty
{
	protected Transform _target;

	protected Vector3 _originalEndValue;

	protected Vector3 _startValue;

	protected Vector3 _endValue;

	protected Vector3 _diffValue;

	public AbstractVector3TweenProperty()
		: base(false)
	{
	}

	public AbstractVector3TweenProperty(Vector3 endValue, bool isRelative = false)
		: base(isRelative)
	{
		_originalEndValue = endValue;
	}

	public override bool validateTarget(object target)
	{
		return target is Transform;
	}

	public override void prepareForUse()
	{
		if (_isRelative && !_ownerTween.isFrom)
		{
			_diffValue = _endValue;
		}
		else
		{
			_diffValue = _endValue - _startValue;
		}
	}

	public void resetWithNewEndValue(Vector3 endValue)
	{
		_originalEndValue = endValue;
		prepareForUse();
	}
}
