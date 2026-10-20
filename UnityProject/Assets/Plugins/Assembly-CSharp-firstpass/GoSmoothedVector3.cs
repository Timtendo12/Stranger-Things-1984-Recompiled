using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoSmoothedVector3 @ 0x016d68c8..0x016d6b24, Ghidra vaddr).
public struct GoSmoothedVector3
{
	public GoSmoothingType smoothingType;

	public float duration;

	private Vector3 _currentValue;

	private Vector3 _target;

	private Vector3 _start;

	private float _startTime;

	public Vector3 smoothValue
	{
		get
		{
			float t = (Time.time - _startTime) / duration;
			switch (smoothingType)
			{
			case GoSmoothingType.Lerp:
				_currentValue = Vector3.Lerp(_start, _target, t);
				break;
			case GoSmoothingType.Slerp:
				_currentValue = Vector3.Slerp(_start, _target, t);
				break;
			}
			return _currentValue;
		}
		private set
		{
			_start = smoothValue;
			_startTime = Time.time;
			_target = value;
		}
	}

	public float x
	{
		get
		{
			return _currentValue.x;
		}
		set
		{
			smoothValue = new Vector3(value, _target.y, _target.z);
		}
	}

	public float y
	{
		get
		{
			return _currentValue.y;
		}
		set
		{
			smoothValue = new Vector3(_target.x, value, _target.z);
		}
	}

	public float z
	{
		get
		{
			return _currentValue.z;
		}
		set
		{
			smoothValue = new Vector3(_target.x, _target.y, value);
		}
	}

	public GoSmoothedVector3(Vector3 vector)
	{
		_currentValue = vector;
		_start = vector;
		_target = vector;
		_startTime = Time.time;
		smoothingType = GoSmoothingType.Lerp;
		duration = 0.2f;
	}

	public static implicit operator GoSmoothedVector3(Vector3 v)
	{
		return new GoSmoothedVector3(v);
	}
}
