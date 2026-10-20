using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (ShakeTweenProperty @ 0x016d3548..0x016d8560, Ghidra vaddr).
public class ShakeTweenProperty : AbstractTweenProperty
{
	private Transform _target;

	private Vector3 _shakeMagnitude;

	private Vector3 _startPosition;

	private Vector3 _startScale;

	private Vector3 _startEulers;

	private GoShakeType _shakeType;

	private int _frameCount;

	private int _frameMod;

	private bool _useLocalProperties;

	public bool useLocalProperties => _useLocalProperties;

	public ShakeTweenProperty(Vector3 shakeMagnitude, GoShakeType shakeType, int frameMod = 1, bool useLocalProperties = false)
		: base(true)
	{
		_shakeMagnitude = shakeMagnitude;
		_shakeType = shakeType;
		_frameMod = frameMod;
		_useLocalProperties = useLocalProperties;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (base.Equals(obj))
		{
			return _shakeType == ((ShakeTweenProperty)obj)._shakeType;
		}
		return false;
	}

	public override bool validateTarget(object target)
	{
		return target is Transform;
	}

	public override void prepareForUse()
	{
		_target = _ownerTween.target as Transform;
		_frameCount = 0;
		if ((_shakeType & GoShakeType.Position) != 0)
		{
			_startPosition = (_useLocalProperties ? _target.localPosition : _target.position);
		}
		if ((_shakeType & GoShakeType.Eulers) != 0)
		{
			// Shipped code always reads world eulerAngles here, even with useLocalProperties.
			_startEulers = _target.eulerAngles;
		}
		if ((_shakeType & GoShakeType.Scale) != 0)
		{
			_startScale = _target.localScale;
		}
	}

	private Vector3 randomDiminishingTarget(float falloffValue)
	{
		return new Vector3(Random.Range(0f - _shakeMagnitude.x, _shakeMagnitude.x), Random.Range(0f - _shakeMagnitude.y, _shakeMagnitude.y), Random.Range(0f - _shakeMagnitude.z, _shakeMagnitude.z)) * falloffValue;
	}

	public override void tick(float totalElapsedTime)
	{
		if (_frameMod > 1)
		{
			_frameCount++;
			if (_frameCount % _frameMod != 0)
			{
				return;
			}
		}
		float falloffValue = 1f - _easeFunction(totalElapsedTime, 0f, 1f, _ownerTween.duration);
		if ((_shakeType & GoShakeType.Position) != 0)
		{
			Vector3 vector = _startPosition + randomDiminishingTarget(falloffValue);
			if (_useLocalProperties)
			{
				_target.localPosition = vector;
			}
			else
			{
				_target.position = vector;
			}
		}
		if ((_shakeType & GoShakeType.Eulers) != 0)
		{
			Vector3 vector2 = _startEulers + randomDiminishingTarget(falloffValue);
			if (_useLocalProperties)
			{
				_target.localEulerAngles = vector2;
			}
			else
			{
				_target.eulerAngles = vector2;
			}
		}
		if ((_shakeType & GoShakeType.Scale) != 0)
		{
			_target.localScale = _startScale + randomDiminishingTarget(falloffValue);
		}
	}
}
