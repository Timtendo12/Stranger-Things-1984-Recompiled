using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageDebug @ 0x179e5bc..0x179ed90, Ghidra vaddr).
// Developer-only page (FPS display + console); the shipped HUD never creates the debug stage on PC.
public class FPageDebug : FUIPage
{
	protected const float cUpdateInterval = 0.5f;

	protected static readonly Vector2 cFpsDisplaySize = new Vector2(200f, 64f);

	protected static readonly Vector2 cFpsDisplayOffset = new Vector2(0f, 32f);

	protected float mAccum;

	protected int mFrames;

	protected float mTimeLeft;

	protected float mLastTime;

	protected FNode mFPSDisplay;

	protected FLabel mFPSText;

	protected FSlicedSprite mFPSBackground;

	protected FPanelConsole mConsole;

	public FPanelConsole Console => mConsole;

	public FPageDebug(string name)
		: base(name)
	{
		mFPSDisplay = new FNode();
		mFPSDisplay.SetParent(this);
		mFPSBackground = new FSlicedSprite("BlackSquare.png");
		mFPSBackground.SetParent(mFPSDisplay);
		mFPSText = new FLabel("FixedWidthStroke", "FPS <>", 32f, true, false, false);
		mFPSText.SetParent(mFPSDisplay);
		mConsole = new FPanelConsole();
		mConsole.SetParent(this);
		mConsole.DockInside(this, InnerDock.Top, mConsole.Size.x, mConsole.Size.y, 0f, 0f);
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		mFPSDisplay.DockInside(this, InnerDock.BottomRight, cFpsDisplaySize, cFpsDisplayOffset);
		mFPSBackground.Color = new Color(1f, 1f, 1f, 0.5f);
		mFPSBackground.DockInside(mFPSDisplay, InnerDock.Center, cFpsDisplaySize.x, cFpsDisplaySize.y, 0f, 0f);
		mFPSText.Justify(FNode.Sides.Right, FNode.Sides.CenterY);
		mFPSText.DockInside(mFPSDisplay, InnerDock.Center, cFpsDisplaySize.x, cFpsDisplaySize.y, 0f, 0f);
		FFPSGraph.PageDebugInit(this);
		mConsole.SetParent(this);
		MarkDirtyRecursive();
		mConsole.Unparent();
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		mTimeLeft = 0.5f;
		mLastTime = Time.realtimeSinceStartup;
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
		UpdateFPS();
	}

	protected void UpdateFPS()
	{
		float delta = Time.realtimeSinceStartup - mLastTime;
		mLastTime = Time.realtimeSinceStartup;
		mTimeLeft -= delta;
		mFrames++;
		mAccum += 1f / delta;
		if (0f < mTimeLeft)
		{
			return;
		}
		float fps = mAccum / (float)mFrames;
		float memMB = (float)FMemDebug.GetMemUsage() / 1048576f;
		string text = ((!(memMB > 0f)) ? string.Format("\n{0:F2}", fps) : string.Format("\n{0:F2} ({1:F1}MB)", fps, memMB));
		if (mFPSText.OnStage)
		{
			mFPSText.Text = string.Empty + text;
		}
		// TODO(port): the colour thresholds (30 / 10 fps) are reconstructed from a mangled branch (0x179e96c) [left: debug overlay colours]
		if (fps >= 30f)
		{
			mFPSText.Color = new Color(0f, 1f, 0f, 1f);
		}
		else if (fps >= 10f)
		{
			mFPSText.Color = new Color(1f, 0.9215686f, 0.015686275f, 1f);
		}
		else
		{
			mFPSText.Color = new Color(1f, 0f, 0f, 1f);
		}
		mAccum = 0f;
		mTimeLeft = 0.5f;
	}

	public void ToggleConsole()
	{
		if (mConsole.HasParent)
		{
			mConsole.Unparent();
		}
		else
		{
			mConsole.SetParent(this);
		}
	}

	public void ShowFPS(bool v)
	{
		if (!v)
		{
			mFPSDisplay.Unparent();
			return;
		}
		mFPSDisplay.SetParent(this);
		mFPSDisplay.DockInside(this, InnerDock.BottomRight, cFpsDisplaySize, cFpsDisplayOffset);
		mFPSDisplay.MarkDirtyRecursive();
	}

	protected void HandleBugButton(FUIButton button, FTouch touch)
	{
		FPluginRelay.ReportBug();
	}

	public override bool HandleInput(float deltaTime)
	{
		return false;
	}
}
