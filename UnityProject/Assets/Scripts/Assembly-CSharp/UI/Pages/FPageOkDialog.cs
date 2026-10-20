using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageOkDialog @ 0x17a9748..0x17ac0d0, Ghidra vaddr).
public class FPageOkDialog : FUIPage
{
	public enum AndroidBackButtonType
	{
		None = 0,
		Button0 = 1,
		Button1 = 2
	}

	protected const float cTipOffset = 40f;

	protected static readonly Vector2 cDimensions = new Vector2(828f, 544f);

	protected static readonly Vector2 cBodyDimensions = new Vector2(748f, 248f);

	protected static readonly Vector2 cBodyWithCharacterDimensions = new Vector2(590f, 248f);

	protected bool mFinalized;

	protected FUICollider mCollider;

	protected FNode mDialog;

	protected FSlicedSprite mBackground;

	protected FSlicedSprite mBackgroundBorder;

	protected FUIButton mButton0;

	[CompilerGenerated]
	private Action m_mButton0Press;

	protected FUIButton mButton1;

	[CompilerGenerated]
	private Action m_mButton1Press;

	[CompilerGenerated]
	private Action m_mReturnPress;

	[CompilerGenerated]
	private Action m_mEscapePress;

	protected FLabel mTitleLabel;

	protected FLabel mBodyLabel;

	protected FSprite mSpinner;

	protected FNode mVisual;

	protected FUIButton mBugButton;

	protected FUIButton mOptionButton;

	public bool PlaySounds { get; set; }

	public AndroidBackButtonType AndroidBackButton { get; set; }

	protected event Action mButton0Press
	{
		add
		{
			m_mButton0Press = (Action)Delegate.Combine(m_mButton0Press, value);
		}
		remove
		{
			m_mButton0Press = (Action)Delegate.Remove(m_mButton0Press, value);
		}
	}

	public event Action Button0Press
	{
		add
		{
			mButton0Press -= value;
			mButton0Press += value;
		}
		remove
		{
			mButton0Press -= value;
		}
	}

	protected event Action mButton1Press
	{
		add
		{
			m_mButton1Press = (Action)Delegate.Combine(m_mButton1Press, value);
		}
		remove
		{
			m_mButton1Press = (Action)Delegate.Remove(m_mButton1Press, value);
		}
	}

	public event Action Button1Press
	{
		add
		{
			mButton1Press -= value;
			mButton1Press += value;
		}
		remove
		{
			mButton1Press -= value;
		}
	}

	protected event Action mReturnPress
	{
		add
		{
			m_mReturnPress = (Action)Delegate.Combine(m_mReturnPress, value);
		}
		remove
		{
			m_mReturnPress = (Action)Delegate.Remove(m_mReturnPress, value);
		}
	}

	protected event Action mEscapePress
	{
		add
		{
			m_mEscapePress = (Action)Delegate.Combine(m_mEscapePress, value);
		}
		remove
		{
			m_mEscapePress = (Action)Delegate.Remove(m_mEscapePress, value);
		}
	}

	public FPageOkDialog(string name)
		: base(name)
	{
		AndroidBackButton = AndroidBackButtonType.None;
		base.Modal = true;
		base.DarkenLower = true;
		PlaySounds = true;
	}

	public void SetupBackground(string background, string backgroundBorder, Vector2 size, string titleText, Vector2 titleTextSize, Vector2 titleTextOffset, string bodyText, Vector2 bodyTextSize, Vector2 bodyTextOffset, string buttonText, string buttonBackground, Vector2 buttonSize, Vector2 buttonOffset, Action buttonPress, Action returnPress, Action escapePress)
	{
		mCollider = new FUICollider("OkDialog", false);
		mCollider.SetParent(this);
		mCollider.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		mDialog = new FNode("DialogOk");
		mDialog.SetParent(this);
		mDialog.DockInside(this, InnerDock.Center, size.x, size.y, 0f, 0f);
		mBackground = new FSlicedSprite(background);
		mBackground.SetParent(mDialog);
		mBackground.DockInside(mDialog, InnerDock.Center, size.x, size.y, 0f, 0f);
		if (!string.IsNullOrEmpty(backgroundBorder))
		{
			mBackgroundBorder = new FSlicedSprite(Name + ".BackgroundBorder", backgroundBorder);
			mBackgroundBorder.SetParent(mDialog);
			mBackgroundBorder.DockInside(mDialog, InnerDock.Center, size.x, size.y, 0f, 0f);
		}
		if (!string.IsNullOrEmpty(titleText))
		{
			mTitleLabel = new FLabel("DefaultStyle", titleText, 64f, true, true, false);
			mTitleLabel.SetParent(mDialog);
			mTitleLabel.Color = new Color(1f, 0f, 0f, 1f);
			mTitleLabel.DockInside(mDialog, InnerDock.Top, titleTextSize.x, titleTextSize.y, titleTextOffset.x, titleTextOffset.y);
		}
		if (!string.IsNullOrEmpty(bodyText))
		{
			mBodyLabel = new FLabel("OKDialogBody", "DefaultStyle", bodyText, 48f, true, true, false);
			mBodyLabel.SetParent(mDialog);
			mBodyLabel.Color = new Color(1f, 0f, 0f, 1f);
			mBodyLabel.ClipAndWrap(true);
			mBodyLabel.DockInside(mDialog, InnerDock.Bottom, bodyTextSize.x, bodyTextSize.y, bodyTextOffset.x, bodyTextOffset.y);
		}
		mButton0 = new FUIButton("Button0");
		mButton0.SetParent(mDialog);
		mButton0.SetBackground(buttonBackground);
		mButton0.SetForegroundColor(FUI.Colors.UITitle);
		mButton0.SetText(buttonText, "DefaultStyle", 32f, 14f);
		mButton0.AddHandleRelease(HandleButton0Release);
		mButton0.DockInside(mDialog, InnerDock.Bottom, buttonSize.x, buttonSize.y, buttonOffset.x, buttonOffset.y);
		if (buttonPress != null)
		{
			mButton0Press += buttonPress;
		}
		m_mReturnPress = returnPress;
		m_mEscapePress = escapePress;
		mBugButton = new FUIButton("OkDialogBugButton");
		mBugButton.SetBackground("UIButtonBug.png");
		mBugButton.AddHandleRelease(HandleBugButton);
		mOptionButton = new FUIButton("OkDialogOptionButton");
		mOptionButton.SetBackground("UIButtonOptions.png");
		mOptionButton.AddHandleRelease(HandleOptionButton);
	}

	public void SetupText(string titleText, string bodyText, bool useMarkup, float bodyPtSize)
	{
		mCollider = new FUICollider("OkDialog", false);
		mCollider.SetParent(this);
		mCollider.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		mDialog = new FNode("DialogOk");
		mDialog.SetParent(this);
		mDialog.DockInside(this, InnerDock.Center, cDimensions.x, cDimensions.y, 0f, 0f);
		mBackground = new FSlicedSprite("Dialog002.png");
		mBackground.SetParent(mDialog);
		mBackground.DockInside(mDialog, InnerDock.Center, cDimensions.x, cDimensions.y, 0f, 0f);
		mTitleLabel = new FLabel("DefaultStyle", titleText, 64f, true, useMarkup, false);
		mTitleLabel.SetParent(mDialog);
		mTitleLabel.Color = new Color(1f, 0f, 0f, 1f);
		mTitleLabel.DockInside(mDialog, InnerDock.Top, cDimensions.x - 128f, 96f, 0f, -32f);
		mBodyLabel = new FLabel("OKDialogBody", "DefaultStyle", bodyText, bodyPtSize, true, useMarkup, false);
		mBodyLabel.SetParent(mDialog);
		mBodyLabel.Color = new Color(1f, 0f, 0f, 1f);
		mBodyLabel.ClipAndWrap(true);
		mBodyLabel.DockOutside(mTitleLabel, OuterDock.Bottom, cBodyDimensions.x, cBodyDimensions.y, 0f, 0f);
		mBugButton = new FUIButton("OkDialogBugButton");
		mBugButton.SetBackground("UIButtonBug.png");
		mBugButton.AddHandleRelease(HandleBugButton);
		mOptionButton = new FUIButton("OkDialogOptionButton");
		mOptionButton.SetBackground("UIButtonOptions.png");
		mOptionButton.AddHandleRelease(HandleOptionButton);
	}

	public void SetupModel(FVisualDefinition vd, string animation = "Idle")
	{
		FAssert.That(vd != null, "SetupModel called with null FVisualDefinition!");
		mVisual = vd.AllocateNode();
		FAssert.That(mVisual != null, "No visual create for itemtype: " + vd.GOTreeName);
		mVisual.SetParent(mDialog);
		// TODO(port): the shipped code picks the size by a type test on mVisual (type not recoverable from the decompile); FGameObjectNode assumed [left: model dialogs use 3D visual definitions (FGOTree), unreachable in the 2D data]
		float width;
		float height;
		float xOffset;
		if (mVisual is FGameObjectNode)
		{
			width = 205f;
			height = 205f;
			xOffset = 32f;
		}
		else
		{
			width = 160f;
			height = 330f;
			xOffset = 44f;
		}
		mVisual.DockInside(mDialog, InnerDock.Left, width, height, xOffset, 0f);
		mTitleLabel.DockCustom(mDialog, HorizSide.Right, -128f - cBodyWithCharacterDimensions.x, mDialog, HorizSide.Right, -36f, mDialog, VertSide.Bottom, cBodyDimensions.y, mDialog, VertSide.Bottom, 0f);
		mBodyLabel.DockCustom(mDialog, HorizSide.Right, -36f - cBodyWithCharacterDimensions.x, mDialog, HorizSide.Right, -36f, mTitleLabel, VertSide.Bottom, 0f, mTitleLabel, VertSide.Bottom, 0f - cBodyWithCharacterDimensions.y);
	}

	public void SetupSprite(string mainSprite, string backSprite, string foreSprite, Vector2 spriteSize)
	{
		mVisual = new FNode();
		mVisual.SetParent(mDialog);
		mVisual.DockInside(mDialog, InnerDock.TopLeft, spriteSize.x, spriteSize.y, 48f, -48f);
		if (!string.IsNullOrEmpty(backSprite))
		{
			FSprite back = new FSprite(backSprite);
			back.SetParent(mVisual);
			back.DockInside(mVisual, InnerDock.Center, spriteSize.x, spriteSize.y, 0f, 0f);
		}
		if (!string.IsNullOrEmpty(mainSprite))
		{
			FSprite main = new FSprite(mainSprite);
			main.SetParent(mVisual);
			main.DockInside(mVisual, InnerDock.Center, spriteSize.x, spriteSize.y, 0f, 0f);
		}
		if (!string.IsNullOrEmpty(foreSprite))
		{
			FSprite fore = new FSprite(foreSprite);
			fore.SetParent(mVisual);
			fore.DockInside(mVisual, InnerDock.Center, spriteSize.x, spriteSize.y, 0f, 0f);
		}
		mTitleLabel.DockCustom(mDialog, HorizSide.Right, -128f - cBodyWithCharacterDimensions.x, mDialog, HorizSide.Right, -36f, mDialog, VertSide.Bottom, cBodyDimensions.y, mDialog, VertSide.Bottom, 0f);
		mBodyLabel.DockCustom(mDialog, HorizSide.Right, -36f - cBodyWithCharacterDimensions.x, mDialog, HorizSide.Right, -36f, mTitleLabel, VertSide.Bottom, 0f, mTitleLabel, VertSide.Bottom, 0f - cBodyWithCharacterDimensions.y);
	}

	public void SetupSingleButton(string buttonText, float width, Action press, Action returnPress, Action escapePress)
	{
		FAssert.That(mDialog != null, "SetupSingleButton called with null mDialog!");
		mButton0 = new FUIButton("Button0");
		mButton0.SetParent(mDialog);
		mButton0.SetBackground("BlackSquare.png");
		mButton0.SetText(buttonText, "DefaultStyle", 32f, 0f);
		mButton0.AddHandleRelease(HandleButton0Release);
		mButton0.DockInside(mDialog, InnerDock.Bottom, 200f, 96f, 0f, 48f);
		if (press != null)
		{
			mButton0Press += press;
		}
		m_mReturnPress = returnPress;
		m_mEscapePress = escapePress;
	}

	public void SetupDoubleButton(string buttonText0, Action press0, string buttonText1, Action press1, Action returnPress, Action escapePress)
	{
		FAssert.That(mDialog != null, "SetupSingleButton called with null mDialog!");
		mButton0 = new FUIButton("Button0");
		mButton0.SetParent(mDialog);
		mButton0.SetBackground("BlackSquare.png");
		mButton0.SetText(buttonText0, "DefaultStyle", 32f, 0f);
		mButton0.AddHandleRelease(HandleButton0Release);
		mButton0.DockInside(mDialog, InnerDock.Bottom, 200f, 96f, 148f, 48f);
		if (press0 != null)
		{
			mButton0Press += press0;
		}
		mButton1 = new FUIButton("Button0");
		mButton1.SetParent(mDialog);
		mButton1.SetBackground("BlackSquare.png");
		mButton1.SetText(buttonText1, "DefaultStyle", 32f, 0f);
		mButton1.AddHandleRelease(HandleButton1Release);
		mButton1.DockInside(mDialog, InnerDock.Bottom, 200f, 96f, -148f, 48f);
		if (press1 != null)
		{
			mButton1Press += press1;
		}
		m_mReturnPress = returnPress;
		m_mEscapePress = escapePress;
	}

	public void SetupBugAndOptionButton()
	{
		if (mBugButton != null)
		{
			mBugButton.SetParent(mDialog);
			mBugButton.DockInside(mDialog, InnerDock.TopRight, 72f, 72f, -16f, -16f);
		}
		if (mOptionButton != null)
		{
			mOptionButton.SetParent(mDialog);
			mOptionButton.DockInside(mDialog, InnerDock.TopRight, 72f, 72f, -104f, -16f);
		}
	}

	protected void HandleBugButton(FUIButton button, FTouch touch)
	{
		FPluginRelay.ReportBug();
	}

	protected void HandleOptionButton(FUIButton button, FTouch touch)
	{
	}

	public void SetupDefaultKeyActions()
	{
		m_mReturnPress = m_mButton0Press;
		m_mEscapePress = m_mButton1Press;
	}

	public void SetupKeyActions(Action returnPress, Action escapePress)
	{
		FAssert.That(mDialog != null, "SetupKeyActions called with null mDialog!");
		m_mReturnPress = returnPress;
		m_mEscapePress = escapePress;
	}

	protected void HandleScreenPress(FUICollider collider, FTouch touch)
	{
		if (mButton0 != null && mButton0.OnStage)
		{
			mButton0.Flash();
		}
	}

	protected void Flash()
	{
		if (mButton0 != null && mButton0.OnStage)
		{
			mButton0.Flash();
		}
	}

	public void SetupFinalize()
	{
		MarkDirtyRecursive();
		mFinalized = true;
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		FAssert.That(mFinalized, Name + " OkDialog is not finalized before being added to stage.");
		TransitionIn(mDialog);
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
		if (Input.GetKeyDown(KeyCode.Return))
		{
			HandleReturnKeyPress();
		}
		else if (Input.GetKeyDown(KeyCode.Escape))
		{
			HandleEscapeKeyPress();
		}
	}

	public void Close()
	{
		TransitionOut(mDialog);
	}

	protected void HandleButton0Release(FUIButton button, FTouch touch)
	{
		if (PlaySounds)
		{
			FApp.SoundManager.PlaySoundSet("UIButton");
		}
		if (m_mButton0Press != null)
		{
			m_mButton0Press();
		}
		TransitionOut(mDialog);
	}

	public void DoButton0Action()
	{
		if (m_mButton0Press != null)
		{
			m_mButton0Press();
		}
		TransitionOut(mDialog);
	}

	protected void HandleReturnKeyPress()
	{
		if (m_mReturnPress != null)
		{
			if (PlaySounds)
			{
				FApp.SoundManager.PlaySoundSet("UIButton");
			}
			m_mReturnPress();
			TransitionOut(mDialog);
		}
	}

	protected void HandleButton1Release(FUIButton button, FTouch touch)
	{
		if (PlaySounds)
		{
			FApp.SoundManager.PlaySoundSet("UIButton");
		}
		if (m_mButton1Press != null)
		{
			m_mButton1Press();
		}
		TransitionOut(mDialog);
	}

	public void DoButton1Action()
	{
		if (m_mButton1Press != null)
		{
			m_mButton1Press();
		}
		TransitionOut(mDialog);
	}

	protected void HandleEscapeKeyPress()
	{
		if (m_mEscapePress != null)
		{
			if (PlaySounds)
			{
				FApp.SoundManager.PlaySoundSet("UIButton");
			}
			m_mEscapePress();
			TransitionOut(mDialog);
		}
	}

	public void SetButton0Text(string text)
	{
		mButton0.SetText(text);
	}

	public void SetButton1Background(string background)
	{
		if (mButton1 != null)
		{
			mButton1.SetBackground(background);
		}
	}

	public void SetBodyText(string text)
	{
		mBodyLabel.Text = text;
	}

	public void SetTitleText(string text)
	{
		mTitleLabel.Text = text;
	}

	public void TurnOnSpinner()
	{
		if (mSpinner == null)
		{
			mSpinner = new FSprite("Spinner.png");
			mSpinner.SetParent(mDialog);
			mSpinner.Color = FUI.Colors.UIEmphasisDark;
			int yOffset = ((mButton0 != null && mButton0.Visible) ? 192 : 128);
			mSpinner.DockCustom(mDialog, HorizSide.CenterX, -32f, mDialog, HorizSide.CenterX, 32f, mDialog, VertSide.Bottom, (float)(yOffset | 0x20), mDialog, VertSide.Bottom, (float)(yOffset - 32));
			FEntityActionTweenPropertyByFloat tween = new FEntityActionTweenPropertyByFloat(1.5f, "Rotation", 360f);
			mSpinner.AddAction(new FEntityActionRepeat(-1, tween));
		}
	}

	public void TurnOffSpinner()
	{
		FNode.Cleanup(mSpinner);
		mSpinner = null;
	}

	public void SetButton0Visible(bool visible)
	{
		if (mButton0 != null)
		{
			mButton0.Visible = visible;
			mButton0.Enable(visible);
		}
	}
}
