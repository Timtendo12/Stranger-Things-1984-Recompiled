using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (Vector3PathTweenProperty @ 0x016d3b10..0x016d5d40, Ghidra vaddr).
public class Vector3PathTweenProperty : AbstractTweenProperty, IGenericProperty
{
	private Action<Vector3> _setter;

	private GoSpline _path;

	private Vector3 _startValue;

	public string propertyName { get; private set; }

	public Vector3PathTweenProperty(string propertyName, GoSpline path, bool isRelative = false)
		: base(isRelative)
	{
		this.propertyName = propertyName;
		_path = path;
	}

	public override bool validateTarget(object target)
	{
		_setter = GoTweenUtils.setterForProperty<Action<Vector3>>(target, propertyName);
		return _setter != null;
	}

	public override void prepareForUse()
	{
		if (_ownerTween.isFrom)
		{
			_path.reverseNodes();
		}
		else
		{
			_path.unreverseNodes();
		}
		_path.buildPath();
		if (_ownerTween.isFrom)
		{
			_startValue = _path.getLastNode();
		}
		else
		{
			_startValue = GoTweenUtils.getterForProperty<Func<Vector3>>(_ownerTween.target, propertyName)();
		}
	}

	public override void tick(float totalElapsedTime)
	{
		float t = _easeFunction(totalElapsedTime, 0f, 1f, _ownerTween.duration);
		Vector3 vector = _path.getPointOnPath(t);
		if (_isRelative)
		{
			vector += _startValue;
		}
		_setter(vector);
	}
}
