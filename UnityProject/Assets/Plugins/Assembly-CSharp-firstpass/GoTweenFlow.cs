// Rewritten from libil2cpp.so 1.0.605 (GoTweenFlow @ 0x016d4448..0x016d47ec, Ghidra vaddr).
public class GoTweenFlow : AbstractGoTweenCollection
{
	public GoTweenFlow setId(int id)
	{
		base.id = id;
		return this;
	}

	public GoTweenFlow setIterations(int iterations)
	{
		base.iterations = iterations;
		return this;
	}

	public GoTweenFlow setIterations(int iterations, GoLoopType loopType)
	{
		base.loopType = loopType;
		base.iterations = iterations;
		return this;
	}

	public GoTweenFlow setUpdateType(GoUpdateType updateType)
	{
		base.updateType = updateType;
		return this;
	}

	private void insert(TweenFlowItem item)
	{
		if (item.tween != null && !item.tween.isValid())
		{
			return;
		}
		if (float.IsInfinity(item.duration))
		{
			FLog.UnityLog("adding a Tween with infinite iterations to a TweenFlow is not permitted");
			return;
		}
		if (item.tween != null)
		{
			Go.removeTween(item.tween);
		}
		_tweenFlows.Add(item);
		_tweenFlows.Sort((TweenFlowItem x, TweenFlowItem y) => x.startTime.CompareTo(y.startTime));
		base.duration = UnityEngine.Mathf.Max(item.startTime + item.duration, base.duration);
		base.totalDuration = base.duration * (float)base.iterations;
	}

	public GoTweenFlow insert(float startTime, AbstractGoTween tween)
	{
		TweenFlowItem item = new TweenFlowItem(startTime, tween);
		insert(item);
		return this;
	}
}
