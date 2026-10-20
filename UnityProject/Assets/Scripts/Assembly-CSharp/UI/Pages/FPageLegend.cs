using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageLegend @ 0x17f0ef0..0x17ff6ac, Ghidra vaddr).
public class FPageLegend : FUIPage
{
	protected Vector2 mDialogSize;

	protected FUICollider mDialogCollider;

	protected FNode mDialog;

	protected float mBorderMargin = 32f;

	protected Vector2 mBackgroundSize;

	protected FSlicedSprite mBackground;

	protected FUICollider mBackgroundCollider;

	protected FSprite mBackgroundBorderLeft;

	protected FSprite mBackgroundBorderRight;

	protected FSprite mBackgroundBorderTop;

	protected FSprite mBackgroundBorderBottom;

	protected Vector2 mTitleSize;

	protected FLabel mTitle;

	protected FSprite mTitleLine;

	protected Vector2 mContentSize;

	protected Vector2 mCloseButtonSize;

	protected FUIButton mCloseButton;

	public FPageLegend(string name)
		: base(name)
	{
		mDialogCollider = new FUICollider(Name + ".DialogCollider", false);
		mDialogCollider.SetParent(this);
		mDialogCollider.AddSignalPress(HandleDialogCollider);
		mDialog = new FNode(Name + ".Dialog");
		mDialog.SetParent(this);
		mBackground = new FSlicedSprite(Name + ".Background", "Dialog002.png", -1f);
		mBackground.SetParent(mDialog);
		mBackgroundCollider = new FUICollider(Name + ".BackgroundCollider", false);
		mBackgroundCollider.SetParent(mBackground);
		mBackgroundCollider.ExpandToFill(mBackground, 0f, 0f);
		mBackgroundBorderLeft = new FSprite(Name + ".BackgroundBorderLeft", "DialogBorderOverlayV002.png");
		mBackgroundBorderLeft.SetParent(mBackground);
		mBackgroundBorderRight = new FSprite(Name + ".BackgroundBorderRight", "DialogBorderOverlayV002.png");
		mBackgroundBorderRight.SetParent(mBackground);
		mBackgroundBorderTop = new FSprite(Name + ".BackgroundBorderTop", "DialogBorderOverlayH002.png");
		mBackgroundBorderTop.SetParent(mBackground);
		mBackgroundBorderBottom = new FSprite(Name + ".BackgroundBorderBottom", "DialogBorderOverlayH002.png");
		mBackgroundBorderBottom.SetParent(mBackground);
		mTitle = new FLabel(Name + ".Title", "DefaultStyle", "[Title]", 32f, true, true, false);
		mTitle.SetParent(mBackground);
		mTitle.Justify(Sides.CenterX, Sides.CenterY);
		mTitleLine = new FSprite(Name + ".TitleLine", "TitleLine.png");
		mTitleLine.SetParent(mBackground);
		mCloseButton = new FUIButton(Name + ".CloseButton");
		mCloseButton.SetBackground("Button004.png");
		mCloseButton.SetForegroundColor(new Color(1f, 0f, 0f, 1f));
		mCloseButton.SetText(FLoc.GetString("CloseButton"), "DefaultStyle", 24f, 0f);
		mCloseButton.AddHandleRelease(HandleCloseButton);
		mCloseButton.SetParent(mBackground);
		TransitionStyle = TransitionStyles.Scale;
		DarkenLower = true;
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		mDialogSize = new Vector2(Futile.Screen.Width, Futile.Screen.Height);
		mDialogCollider.DockInside(this, InnerDock.Center, mDialogSize.x, mDialogSize.y, 0f, 0f);
		mDialog.DockInside(mDialogCollider, InnerDock.Center, mDialogSize.x, mDialogSize.y, 0f, 0f);
		mBackgroundSize = new Vector2(Futile.Screen.Width * 0.9f, Futile.Screen.Height * 0.8f);
		mBackground.DockInside(mDialog, InnerDock.Center, mBackgroundSize.x, mBackgroundSize.y, 0f, 0f);
		mBackgroundBorderLeft.DockInside(mBackground, InnerDock.Left, 15f, 256f, 0f, 0f);
		mBackgroundBorderRight.DockInside(mBackground, InnerDock.Right, 15f, 256f, 0f, 0f);
		mBackgroundBorderTop.DockInside(mBackground, InnerDock.Top, 512f, 15f, 0f, 0f);
		mBackgroundBorderBottom.DockInside(mBackground, InnerDock.Bottom, 512f, 15f, 0f, 0f);
		mTitleSize.x = mBackgroundSize.x - (mBorderMargin + mBorderMargin);
		mTitleSize.y = 48f;
		mTitle.DockInside(mBackground, InnerDock.Top, mTitleSize.x, 48f, 0f, 0f - mBorderMargin);
		mTitleLine.DockOutside(mTitle, OuterDock.Bottom, 768f, 3f, 0f, 0f);
		mContentSize.x = mTitleSize.x;
		mContentSize.y = mBackgroundSize.y - mBorderMargin * 3f - mTitleSize.y;
		mCloseButtonSize = new Vector2(192f, 64f);
		mCloseButton.DockInside(mBackground, InnerDock.BottomRight, 192f, 64f, 0f - mBorderMargin, mBorderMargin);
		MarkDirtyRecursive();
	}

	protected void HandleDialogCollider(FUICollider collider, FTouch touch)
	{
		ClosePage();
	}

	protected virtual void ClosePage(string soundset = "UIButtonClose")
	{
		FApp.SoundManager.PlaySoundSet(soundset);
		FApp.HUD.PopPage(this);
	}

	protected void HandleCloseButton(FUIButton button, FTouch touch)
	{
		ClosePage();
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		if (FApp.World != null)
		{
			FApp.World.Pause(true, true);
		}
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		if (FApp.World != null)
		{
			FApp.World.UnPause();
		}
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
	}
}
