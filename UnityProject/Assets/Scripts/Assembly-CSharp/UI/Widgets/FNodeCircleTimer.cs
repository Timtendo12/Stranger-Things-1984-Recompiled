using System;
using UnityEngine;

public class FNodeCircleTimer : FNode
{
	protected FSprite mBackground;

	protected FRadialWipeSprite mForeground;

	protected FLabel mText;

	protected Fix64 mDuration;

	protected Func<Fix64> mGetRemaining;

	protected bool mShowTimerSprite;

	public FNodeCircleTimer(string name, Vector2 size, string backgroundSprite, string foregroundSprite, string font, float pointSize, bool showTimerText)
	{
	}

	public override void HandleAddedToStage()
	{
	}

	public override void HandleRemovedFromStage()
	{
	}

	protected override void HandleUpdate()
	{
	}

	public bool Setup(Fix64 duration, Func<Fix64> getRemaining)
	{
		return false;
	}

	protected Fix64 GetRemaining()
	{
		return default;
	}
}
