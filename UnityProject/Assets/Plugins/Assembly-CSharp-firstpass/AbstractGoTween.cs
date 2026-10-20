using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (AbstractGoTween @ 0x016cc094..0x016cc6ec, Ghidra vaddr).
public abstract class AbstractGoTween
{
	public int id;

	public static int mNextID;

	protected bool _didStart;

	protected bool _didComplete;

	protected float _elapsedTime;

	protected float _totalElapsedTime;

	protected bool _isLoopingBackOnPingPong;

	protected int _completedIterations;

	protected Action<AbstractGoTween> _onStart;

	protected Action<AbstractGoTween> _onComplete;

	public GoTweenState state { get; protected set; }

	public float duration { get; protected set; }

	public float totalDuration { get; protected set; }

	public float timeScale { get; set; }

	public GoUpdateType updateType { get; protected set; }

	public GoLoopType loopType { get; protected set; }

	public int iterations { get; protected set; }

	public bool autoRemoveOnComplete { get; set; }

	public bool isReversed { get; protected set; }

	public float totalElapsedTime => _totalElapsedTime;

	public bool isLoopoingBackOnPingPong => _isLoopingBackOnPingPong;

	public int completedIterations => _completedIterations;

	public void setOnStartHandler(Action<AbstractGoTween> onStart)
	{
		_onStart = onStart;
	}

	public void setOnCompleteHandler(Action<AbstractGoTween> onComplete)
	{
		_onComplete = onComplete;
	}

	protected virtual void onStart()
	{
		_didStart = true;
		if (_onStart != null)
		{
			_onStart(this);
		}
	}

	protected virtual void onComplete()
	{
		_didComplete = true;
		if (_onComplete != null)
		{
			_onComplete(this);
		}
	}

	public virtual void onCompleteIfNotComplete()
	{
		if (!_didComplete)
		{
			onComplete();
		}
	}

	public virtual bool update(float deltaTime)
	{
		if (!_didStart)
		{
			onStart();
		}
		_isLoopingBackOnPingPong = false;
		if (isReversed)
		{
			deltaTime = 0f - deltaTime;
		}
		float num = _totalElapsedTime + deltaTime;
		_totalElapsedTime = (num >= 0f) ? Mathf.Min(num, totalDuration) : 0f;
		_completedIterations = (int)(_totalElapsedTime / duration);
		if (loopType == GoLoopType.PingPong)
		{
			if (iterations < 0)
			{
				if (_completedIterations % 2 != 0)
				{
					_isLoopingBackOnPingPong = true;
				}
			}
			else if (iterations != 0)
			{
				if (_completedIterations < iterations)
				{
					if (_completedIterations % 2 != 0)
					{
						_isLoopingBackOnPingPong = true;
					}
				}
				else if (_completedIterations % 2 == 0)
				{
					_isLoopingBackOnPingPong = true;
				}
			}
		}
		if (iterations > 0 && _completedIterations >= iterations)
		{
			_elapsedTime = duration;
			if (!isReversed)
			{
				state = GoTweenState.Complete;
				return false;
			}
		}
		else
		{
			_elapsedTime = (_totalElapsedTime >= duration) ? (_totalElapsedTime % duration) : _totalElapsedTime;
			if (!isReversed)
			{
				return false;
			}
		}
		if (_totalElapsedTime > 0f)
		{
			return false;
		}
		state = GoTweenState.Complete;
		return false;
	}

	public abstract bool isValid();

	public abstract bool removeTweenProperty(AbstractTweenProperty property);

	public abstract bool containsTweenProperty(AbstractTweenProperty property);

	public abstract List<AbstractTweenProperty> allTweenProperties();

	public virtual void destroy()
	{
		state = GoTweenState.Destroyed;
		Go.removeTween(this);
	}

	public void pause()
	{
		state = GoTweenState.Paused;
	}

	public void play()
	{
		state = GoTweenState.Running;
	}

	public void playForward()
	{
		isReversed = false;
		state = GoTweenState.Running;
	}

	public void playBackwards()
	{
		state = GoTweenState.Running;
		isReversed = true;
	}

	public abstract void rewind();

	public void restart(bool skipDelay = true)
	{
		_didComplete = false;
		rewind();
		state = GoTweenState.Running;
	}

	public void reverse()
	{
		isReversed = !isReversed;
	}

	public virtual void complete()
	{
		if (iterations >= 0)
		{
			if (isReversed)
			{
				_elapsedTime = 0f;
				_totalElapsedTime = 0f;
				_completedIterations = 0;
			}
			else
			{
				_elapsedTime = duration;
				_totalElapsedTime = totalDuration;
				_completedIterations = iterations;
			}
			state = GoTweenState.Running;
		}
	}

	public virtual void goTo(float time)
	{
		time = (time >= 0f) ? Mathf.Min(time, totalDuration) : 0f;
		_elapsedTime = time;
		_totalElapsedTime = time;
		if (iterations != 0)
		{
			_completedIterations = (int)(_totalElapsedTime / duration);
			if (loopType == GoLoopType.PingPong)
			{
				_isLoopingBackOnPingPong = (_completedIterations & 1) != 0;
			}
			if (iterations < 0 || _completedIterations <= iterations)
			{
				_elapsedTime = _totalElapsedTime % duration;
			}
		}
		update(0f);
	}

	public void goToAndPlay(float time)
	{
		state = GoTweenState.Running;
		goTo(time);
	}

	public IEnumerator waitForCompletion()
	{
		while (state != GoTweenState.Complete && state != GoTweenState.Destroyed)
		{
			yield return null;
		}
	}

	public void setAutoID()
	{
		id = mNextID++;
	}
}
