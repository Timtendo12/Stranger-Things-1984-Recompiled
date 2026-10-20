using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (AbstractGoTweenCollection @ 0x016cc734..0x016cd74c, Ghidra vaddr).
public class AbstractGoTweenCollection : AbstractGoTween
{
	protected class TweenFlowItem
	{
		public float startTime;

		public float duration;

		public AbstractGoTween tween;

		public TweenFlowItem(float startTime, AbstractGoTween tween)
		{
			this.tween = tween;
			this.startTime = startTime;
			duration = tween.totalDuration;
		}

		public TweenFlowItem(float startTime, float duration)
		{
			this.startTime = startTime;
			this.duration = duration;
		}
	}

	protected List<TweenFlowItem> _tweenFlows = new List<TweenFlowItem>();

	public AbstractGoTweenCollection()
	{
		autoRemoveOnComplete = true;
		base.timeScale = 1f;
		base.iterations = 1;
		base.state = GoTweenState.Paused;
		setAutoID();
		Go.addTween(this);
	}

	public List<GoTween> tweensWithTarget(object target)
	{
		List<GoTween> list = new List<GoTween>();
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween == null)
			{
				continue;
			}
			if (tweenFlow.tween is GoTween goTween)
			{
				if (goTween.target == target)
				{
					list.Add(goTween);
				}
			}
			else if (tweenFlow.tween is AbstractGoTweenCollection abstractGoTweenCollection)
			{
				List<GoTween> list2 = abstractGoTweenCollection.tweensWithTarget(target);
				if (list2.Count > 0)
				{
					list.AddRange(list2);
				}
			}
		}
		return list;
	}

	public override bool removeTweenProperty(AbstractTweenProperty property)
	{
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween != null && tweenFlow.tween.removeTweenProperty(property))
			{
				return true;
			}
		}
		return false;
	}

	public override bool containsTweenProperty(AbstractTweenProperty property)
	{
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween != null && tweenFlow.tween.containsTweenProperty(property))
			{
				return true;
			}
		}
		return false;
	}

	public override List<AbstractTweenProperty> allTweenProperties()
	{
		List<AbstractTweenProperty> list = new List<AbstractTweenProperty>();
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween != null)
			{
				list.AddRange(tweenFlow.tween.allTweenProperties());
			}
		}
		return list;
	}

	public override bool isValid()
	{
		return true;
	}

	public override bool update(float deltaTime)
	{
		base.update(deltaTime);
		float num = (_isLoopingBackOnPingPong ? (base.duration - _elapsedTime) : _elapsedTime);
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween != null && tweenFlow.startTime < num)
			{
				tweenFlow.tween.goTo(num - tweenFlow.startTime);
			}
		}
		bool flag = base.state == GoTweenState.Complete;
		if (flag && !_didComplete)
		{
			foreach (TweenFlowItem tweenFlow2 in _tweenFlows)
			{
				if (tweenFlow2.tween != null && tweenFlow2.startTime < num)
				{
					tweenFlow2.tween.onCompleteIfNotComplete();
				}
			}
			onComplete();
		}
		return flag;
	}

	public override void rewind()
	{
		_completedIterations = 0;
		base.state = GoTweenState.Paused;
		_elapsedTime = 0f;
		_totalElapsedTime = 0f;
		_isLoopingBackOnPingPong = false;
	}

	public override void complete()
	{
		if (base.iterations < 0)
		{
			return;
		}
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
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween != null)
			{
				tweenFlow.tween.goTo(tweenFlow.tween.totalDuration);
			}
		}
	}
}
