using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (PositionPathTweenProperty @ 0x016d2f94..0x016d74c4, Ghidra vaddr).
public class PositionPathTweenProperty : AbstractTweenProperty
{
	protected bool _useLocalPosition;

	protected Transform _target;

	protected Vector3 _startValue;

	private GoSpline _path;

	private GoLookAtType _lookAtType;

	private Transform _lookTarget;

	private GoSmoothedQuaternion _smoothedRotation;

	public bool useLocalPosition => _useLocalPosition;

	public PositionPathTweenProperty(GoSpline path, bool isRelative = false, bool useLocalPosition = false, GoLookAtType lookAtType = GoLookAtType.None, Transform lookTarget = null)
		: base(isRelative)
	{
		_path = path;
		_lookTarget = lookTarget;
		_useLocalPosition = useLocalPosition;
		_lookAtType = lookAtType;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (base.Equals(obj))
		{
			return _useLocalPosition == ((PositionPathTweenProperty)obj)._useLocalPosition;
		}
		if (obj is PositionTweenProperty)
		{
			return _useLocalPosition == ((PositionTweenProperty)obj).useLocalPosition;
		}
		return false;
	}

	public override void prepareForUse()
	{
		_target = _ownerTween.target as Transform;
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
			_startValue = (_useLocalPosition ? _target.localPosition : _target.position);
		}
		if (_lookAtType == GoLookAtType.TargetTransform && _lookTarget == null)
		{
			_lookAtType = GoLookAtType.None;
		}
		_smoothedRotation = new GoSmoothedQuaternion(_target.rotation);
	}

	public override void tick(float totalElapsedTime)
	{
		float t = _easeFunction(totalElapsedTime, 0f, 1f, _ownerTween.duration);
		Vector3 vector = _path.getPointOnPath(t);
		if (_isRelative)
		{
			vector += _startValue;
		}
		if (_lookAtType == GoLookAtType.TargetTransform)
		{
			_target.LookAt(_lookTarget, Vector3.up);
		}
		else if (_lookAtType == GoLookAtType.NextPathNode)
		{
			if (vector == _target.position)
			{
				_smoothedRotation.smoothValue = Quaternion.identity;
			}
			else
			{
				_smoothedRotation.smoothValue = Quaternion.LookRotation(vector - _target.position);
			}
			_target.rotation = _smoothedRotation.smoothValue;
		}
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
