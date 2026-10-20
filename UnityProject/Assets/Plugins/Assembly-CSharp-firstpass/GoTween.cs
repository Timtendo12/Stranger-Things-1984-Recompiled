using System;
using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (GoTween @ 0x016d0c34..0x016d2800, Ghidra vaddr).
public class GoTween : AbstractGoTween
{
	private float _elapsedDelay;

	private bool _delayComplete;

	private List<AbstractTweenProperty> _tweenPropertyList = new List<AbstractTweenProperty>();

	private GoEaseType _easeType;

	public object target { get; private set; }

	public float delay { get; private set; }

	public bool isFrom { get; private set; }

	public GoEaseType easeType
	{
		get
		{
			return _easeType;
		}
		set
		{
			_easeType = value;
			foreach (AbstractTweenProperty tweenProperty in _tweenPropertyList)
			{
				// Shipped code writes tweenProperty._easeFunction directly; setEaseType is equivalent.
				tweenProperty.setEaseType(value);
			}
		}
	}

	public GoTween(object target, float duration, GoTweenConfig config, Action<AbstractGoTween> onComplete = null)
	{
		this.target = target;
		autoRemoveOnComplete = true;
		base.duration = duration;
		id = config.id;
		base.iterations = config.iterations;
		_easeType = config.easeType;
		delay = config.delay;
		base.updateType = config.propertyUpdateType;
		base.loopType = config.loopType;
		isFrom = config.isFrom;
		_onComplete = config.onCompleteHandler;
		base.timeScale = config.timeScale;
		_onStart = config.onStartHandler;
		if (config.isPaused)
		{
			base.state = GoTweenState.Paused;
		}
		if (onComplete != null)
		{
			_onComplete = onComplete;
		}
		List<AbstractTweenProperty> tweenProperties = config.tweenProperties;
		for (int i = 0; i < tweenProperties.Count; i++)
		{
			AbstractTweenProperty abstractTweenProperty = tweenProperties[i];
			if (abstractTweenProperty.isInitialized)
			{
				abstractTweenProperty = abstractTweenProperty.clone();
			}
			addTweenProperty(abstractTweenProperty);
		}
		if (base.iterations < 0)
		{
			base.totalDuration = float.PositiveInfinity;
		}
		else
		{
			base.totalDuration = (float)base.iterations * duration;
		}
	}

	public override bool update(float deltaTime)
	{
		if (Go.validateTargetObjectsEachTick && (target == null || target.Equals(null)))
		{
			FLog.Warning("ID " + id.ToString() + ", target validation failed. destroying the tween to avoid errors");
			autoRemoveOnComplete = true;
			return true;
		}
		if (!_delayComplete && _elapsedDelay < delay)
		{
			if (base.timeScale != 0f)
			{
				_elapsedDelay += deltaTime / base.timeScale;
			}
			if (_elapsedDelay >= delay)
			{
				_delayComplete = true;
			}
			return false;
		}
		base.update(deltaTime);
		float totalElapsedTime = (_isLoopingBackOnPingPong ? (base.duration - _elapsedTime) : _elapsedTime);
		for (int i = 0; i < _tweenPropertyList.Count; i++)
		{
			_tweenPropertyList[i].tick(totalElapsedTime);
		}
		if (base.state == GoTweenState.Complete)
		{
			if (!_didComplete)
			{
				onComplete();
			}
			return true;
		}
		return false;
	}

	public override bool isValid()
	{
		return target != null;
	}

	public void addTweenProperty(AbstractTweenProperty tweenProp)
	{
		if (tweenProp.validateTarget(target))
		{
			if (_tweenPropertyList.Contains(tweenProp))
			{
				FLog.UnityLog("not adding tween property because one already exists: " + tweenProp.ToString());
				return;
			}
			_tweenPropertyList.Add(tweenProp);
			tweenProp.init(this);
		}
		else
		{
			FLog.UnityLog("tween failed to validate target: " + tweenProp.ToString());
		}
	}

	public override bool removeTweenProperty(AbstractTweenProperty property)
	{
		if (_tweenPropertyList.Contains(property))
		{
			_tweenPropertyList.Remove(property);
			return true;
		}
		return false;
	}

	public override bool containsTweenProperty(AbstractTweenProperty property)
	{
		return _tweenPropertyList.Contains(property);
	}

	public void clearTweenProperties()
	{
		_tweenPropertyList.Clear();
	}

	public override List<AbstractTweenProperty> allTweenProperties()
	{
		return _tweenPropertyList;
	}

	protected override void onStart()
	{
		base.onStart();
		for (int i = 0; i < _tweenPropertyList.Count; i++)
		{
			_tweenPropertyList[i].prepareForUse();
		}
	}

	public override void destroy()
	{
		base.destroy();
		_tweenPropertyList.Clear();
		target = null;
	}

	public override void goTo(float time)
	{
		_delayComplete = true;
		_elapsedDelay = delay;
		base.goTo(time);
	}

	public override void rewind()
	{
		rewind(true);
	}

	public void rewind(bool skipDelay)
	{
		_elapsedTime = 0f;
		_totalElapsedTime = 0f;
		base.state = GoTweenState.Paused;
		// Shipped code loads offset 0x18 (duration), not delay, here; harmless since _delayComplete is then true.
		_elapsedDelay = (skipDelay ? base.duration : 0f);
		_delayComplete = skipDelay;
		_isLoopingBackOnPingPong = false;
		_completedIterations = 0;
	}

	public override void complete()
	{
		if (base.iterations >= 0)
		{
			if (base.isReversed)
			{
				_elapsedTime = 0f;
				_totalElapsedTime = 0f;
				_completedIterations = 0;
			}
			else
			{
				_elapsedTime = base.duration;
				_totalElapsedTime = base.totalDuration;
				_completedIterations = base.iterations;
			}
			base.state = GoTweenState.Running;
			_delayComplete = true;
		}
	}
}
