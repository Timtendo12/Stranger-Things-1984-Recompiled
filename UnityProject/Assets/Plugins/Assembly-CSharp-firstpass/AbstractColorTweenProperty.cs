using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (AbstractColorTweenProperty @ 0x016d47ec..0x016d4bd8, Ghidra vaddr).
public abstract class AbstractColorTweenProperty : AbstractTweenProperty
{
	protected Material _target;

	protected Color _originalEndValue;

	protected Color _startValue;

	protected Color _endValue;

	protected Color _diffValue;

	public AbstractColorTweenProperty(Color endValue, bool isRelative)
		: base(isRelative)
	{
		_originalEndValue = endValue;
	}

	public override bool validateTarget(object target)
	{
		return target is Material || target is GameObject || target is Transform || target is Renderer;
	}

	public override void init(GoTween owner)
	{
		object target = owner.target;
		if (target is Material)
		{
			_target = (Material)target;
		}
		else if (target is GameObject)
		{
			_target = ((GameObject)target).GetComponent<Renderer>().material;
		}
		else if (target is Transform)
		{
			_target = ((Transform)target).GetComponent<Renderer>().material;
		}
		else if (target is Renderer)
		{
			_target = ((Renderer)target).material;
		}
		base.init(owner);
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
}
