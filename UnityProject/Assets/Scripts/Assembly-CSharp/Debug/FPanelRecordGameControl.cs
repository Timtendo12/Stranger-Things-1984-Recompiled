using UnityEngine;

public class FPanelRecordGameControl : FNode
{
	public const float cTextPointSize = 24f;

	public const float cMargin = 16f;

	public const float cButtonSize = 72f;

	public static readonly Vector2 cSize;

	public static readonly Vector2 cTitleSize;

	public static readonly Vector2 cStatusSize;

	protected FSlicedSprite mBackground;

	protected FLabel mRecordGameTitle;

	protected FLabel mRecordGameStatus;

	protected FUIButton mStopButton;

	protected FUIButton mFastForwardButton;

	protected FUIButton mRestartButton;

	protected FNodeStack mButtonStack;

	public override void HandleAddedToStage()
	{
	}

	public override void HandleRemovedFromStage()
	{
	}

	protected override void HandleUpdate()
	{
	}

	protected void HandleButtonStop(FUIButton button, FTouch touch)
	{
	}

	protected void HandleButtonFastForward(FUIButton button, FTouch touch)
	{
	}

	protected void HandleButtonRestart(FUIButton button, FTouch touch)
	{
	}
}
