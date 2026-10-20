using System;
using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoTweenConfig @ 0x016cadcc..0x016d4448, Ghidra vaddr).
public class GoTweenConfig
{
	private List<AbstractTweenProperty> _tweenProperties = new List<AbstractTweenProperty>();

	public int id = -1;

	public float delay;

	public int iterations = 1;

	public int timeScale = 1;

	public GoLoopType loopType = Go.defaultLoopType;

	public GoEaseType easeType = Go.defaultEaseType;

	public bool isPaused;

	public GoUpdateType propertyUpdateType = Go.defaultUpdateType;

	public bool isFrom;

	public Action<AbstractGoTween> onCompleteHandler;

	public Action<AbstractGoTween> onStartHandler;

	public List<AbstractTweenProperty> tweenProperties => _tweenProperties;

	public GoTweenConfig position(Vector3 endValue, bool isRelative = false)
	{
		PositionTweenProperty item = new PositionTweenProperty(endValue, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig localPosition(Vector3 endValue, bool isRelative = false)
	{
		PositionTweenProperty item = new PositionTweenProperty(endValue, isRelative, true);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig positionPath(GoSpline path, bool isRelative = false, GoLookAtType lookAtType = GoLookAtType.None, Transform lookTarget = null)
	{
		PositionPathTweenProperty item = new PositionPathTweenProperty(path, isRelative, false, lookAtType, lookTarget);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig scale(float endValue, bool isRelative = false)
	{
		return scale(new Vector3(endValue, endValue, endValue), isRelative);
	}

	public GoTweenConfig scale(Vector3 endValue, bool isRelative = false)
	{
		ScaleTweenProperty item = new ScaleTweenProperty(endValue, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig scalePath(GoSpline path, bool isRelative = false)
	{
		ScalePathTweenProperty item = new ScalePathTweenProperty(path, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig eulerAngles(Vector3 endValue, bool isRelative = false)
	{
		EulerAnglesTweenProperty item = new EulerAnglesTweenProperty(endValue, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig localEulerAngles(Vector3 endValue, bool isRelative = false)
	{
		// Shipped code passes useLocalEulers = false here too (field 0x60 stored as 0).
		EulerAnglesTweenProperty item = new EulerAnglesTweenProperty(endValue, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig rotation(Vector3 endValue, bool isRelative = false)
	{
		RotationTweenProperty item = new RotationTweenProperty(endValue, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig localRotation(Vector3 endValue, bool isRelative = false)
	{
		RotationTweenProperty item = new RotationTweenProperty(endValue, isRelative, true);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig materialColor(Color endValue, GoMaterialColorType colorType = GoMaterialColorType.Color, bool isRelative = false)
	{
		MaterialColorTweenProperty item = new MaterialColorTweenProperty(endValue, colorType, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig shake(Vector3 shakeMagnitude, GoShakeType shakeType = GoShakeType.Position, int frameMod = 1, bool useLocalProperties = false)
	{
		ShakeTweenProperty item = new ShakeTweenProperty(shakeMagnitude, shakeType, frameMod, useLocalProperties);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig vector2Prop(string propertyName, Vector2 endValue, bool isRelative = false)
	{
		Vector2TweenProperty item = new Vector2TweenProperty(propertyName, endValue, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig vector3Prop(string propertyName, Vector3 endValue, bool isRelative = false)
	{
		Vector3TweenProperty item = new Vector3TweenProperty(propertyName, endValue, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig vector4Prop(string propertyName, Vector4 endValue, bool isRelative = false)
	{
		Vector4TweenProperty item = new Vector4TweenProperty(propertyName, endValue, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig vector3PathProp(string propertyName, GoSpline path, bool isRelative = false)
	{
		Vector3PathTweenProperty item = new Vector3PathTweenProperty(propertyName, path, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig vector3XProp(string propertyName, float endValue, bool isRelative = false)
	{
		Vector3XTweenProperty item = new Vector3XTweenProperty(propertyName, endValue, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig vector3YProp(string propertyName, float endValue, bool isRelative = false)
	{
		Vector3YTweenProperty item = new Vector3YTweenProperty(propertyName, endValue, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig vector3ZProp(string propertyName, float endValue, bool isRelative = false)
	{
		Vector3ZTweenProperty item = new Vector3ZTweenProperty(propertyName, endValue, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig colorProp(string propertyName, Color endValue, bool isRelative = false)
	{
		ColorTweenProperty item = new ColorTweenProperty(propertyName, endValue, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig intProp(string propertyName, int endValue, bool isRelative = false)
	{
		IntTweenProperty item = new IntTweenProperty(propertyName, endValue, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig floatProp(string propertyName, float endValue, bool isRelative = false)
	{
		FloatTweenProperty item = new FloatTweenProperty(propertyName, endValue, isRelative);
		_tweenProperties.Add(item);
		return this;
	}

	public GoTweenConfig addTweenProperty(AbstractTweenProperty tweenProp)
	{
		_tweenProperties.Add(tweenProp);
		return this;
	}

	public GoTweenConfig clearProperties()
	{
		_tweenProperties.Clear();
		return this;
	}

	public GoTweenConfig setDelay(float seconds)
	{
		delay = seconds;
		return this;
	}

	public GoTweenConfig setIterations(int iterations)
	{
		this.iterations = iterations;
		return this;
	}

	public GoTweenConfig setIterations(int iterations, GoLoopType loopType)
	{
		this.iterations = iterations;
		this.loopType = loopType;
		return this;
	}

	public GoTweenConfig setTimeScale(int timeScale)
	{
		this.timeScale = timeScale;
		return this;
	}

	public GoTweenConfig setEaseType(GoEaseType easeType)
	{
		this.easeType = easeType;
		return this;
	}

	public GoTweenConfig startPaused()
	{
		isPaused = true;
		return this;
	}

	public GoTweenConfig setUpdateType(GoUpdateType setUpdateType)
	{
		propertyUpdateType = setUpdateType;
		return this;
	}

	public GoTweenConfig setIsFrom()
	{
		isFrom = true;
		return this;
	}

	public GoTweenConfig onComplete(Action<AbstractGoTween> onComplete)
	{
		onCompleteHandler = onComplete;
		return this;
	}

	public GoTweenConfig onStart(Action<AbstractGoTween> onStart)
	{
		onStartHandler = onStart;
		return this;
	}

	public GoTweenConfig setId(int id)
	{
		this.id = id;
		return this;
	}
}
