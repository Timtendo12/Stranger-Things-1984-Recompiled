using System;
using System.Collections;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageLoad @ 0x17a6e14..0x17a7d88, Ghidra vaddr).
public class FPageLoad : FUIPage
{
	protected FPageOkDialog mWaitDialog;

	protected FUICollider mBackgroundCollider;

	protected FNode mDialog;

	protected FSprite mBackground;

	protected FLabel mTitleLabel;

	protected FLabel mBodyLabel;

	protected FLabel mTipLabel;

	protected FSprite mSpinner;

	protected FSlicedSprite mInnerBackground;

	protected string mTitle = string.Empty;

	protected string mBody = string.Empty;

	protected FUIButton mCancelButton;

	protected Action mCancelButtonAction;

	public IEnumerator CoroutineToWaitFor;

	public Action OnFullyVisible { get; set; }

	public string Title
	{
		get
		{
			return mTitle;
		}
		set
		{
			mTitle = value;
			if (mTitleLabel != null)
			{
				mTitleLabel.Text = mTitle;
			}
		}
	}

	public string Body
	{
		get
		{
			return mBody;
		}
		set
		{
			mBody = value;
			if (mBodyLabel != null)
			{
				mBodyLabel.Text = mBody;
			}
		}
	}

	public FPageLoad(string name)
		: base(name)
	{
		SetDefaultTransitionInRight();
		base.Modal = true;
		mBackgroundCollider = new FUICollider("PageLoadCollider", false);
		mBackgroundCollider.SetParent(this);
		mDialog = new FNode("PageLoadDialog");
		mDialog.SetParent(this);
		mBackground = new FSprite("Backgrounds/AppLoad");
		mBackground.SetParent(mDialog);
		mTitleLabel = new FLabel("DefaultStyle", mTitle, 72f, true, true, false);
		mTitleLabel.Color = FUI.Colors.UITitle;
		mTitleLabel.Justify(Sides.CenterX, Sides.CenterY);
		mTitleLabel.SetParent(mDialog);
		mSpinner = new FSprite("Spinner.png");
		mSpinner.Color = FColorNames.Get("UIEmphasis");
		mSpinner.SetParent(mDialog);
		mTipLabel = new FLabel("DefaultStyle", string.Empty, 36f, true, true, false);
		mTipLabel.Color = FUI.Colors.UIEmphasis;
		mTipLabel.SetParent(mDialog);
		mBodyLabel = new FLabel("DefaultStyle", mBody, 96f, true, true, false);
		mBodyLabel.Color = FUI.Colors.UITitle;
		mBodyLabel.Justify(Sides.CenterX, Sides.CenterY);
		mBodyLabel.Wrap = true;
		mBodyLabel.SetParent(mBackground);
		mCancelButton = new FUIButton("Cancel");
		mCancelButton.SetBackground("ButtonClose.png");
		mCancelButton.SetForegroundColor(FColorNames.Get("UITitle"));
		mCancelButton.SetText(FLoc.GetString("CancelButton"), "DefaultStyle", 48f, 14f);
		mCancelButton.SetParent(mBackground);
		mCancelButton.AddHandleRelease(HandleCancelButtonRelease);
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		mBackgroundCollider.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		mDialog.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		float height = Futile.Screen.Height;
		mBackground.DockInside(mDialog, InnerDock.Center, height * 16f / 9f, height, 0f, 0f);
		mTitleLabel.DockInside(mDialog, InnerDock.Top, FUI.Screens.LetterboxSize);
		mSpinner.DockInside(mDialog, InnerDock.BottomRight, 64f, 64f, -10f, 0f);
		mSpinner.AddAction(new FEntityActionRepeat(-1, new FEntityActionTweenPropertyByFloat(1.5f, "Rotation", 360f)));
		mTipLabel.DockInside(mDialog, InnerDock.Bottom, Futile.Screen.Width, 150f, 0f, 0f);
		mBodyLabel.DockInside(mBackground, InnerDock.Center, Futile.Screen.Width * 0.75f, FUI.Screens.ScreenHeightMinusLetterboxes, 0f, -32f);
		mCancelButton.DockInside(mBackground, InnerDock.Center, 200f, 96f, 0f, -200f);
		UpdateCancelButton();
	}

	protected void UpdateCancelButton()
	{
		if (mCancelButtonAction == null)
		{
			mCancelButton.Unparent();
		}
		else
		{
			mCancelButton.SetParent(mBackground);
		}
		mCancelButton.MarkDirtyRecursive();
	}

	public void TurnOffCancelButton()
	{
		mCancelButtonAction = null;
		UpdateCancelButton();
	}

	public void TurnOnCancelButton(Action cancelAction)
	{
		mCancelButtonAction = cancelAction;
		UpdateCancelButton();
	}

	protected void HandleCancelButtonRelease(FUIButton button, FTouch touch)
	{
		if (mCancelButtonAction != null)
		{
			mCancelButtonAction();
		}
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		RemoveAllActions();
		SetupWaitForNextTip();
		TransitionIn(mDialog);
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		if (mWaitDialog != null)
		{
			mWaitDialog = null;
		}
	}

	protected override void OnCompleteTransitionIn(AbstractGoTween tween)
	{
		base.OnCompleteTransitionIn(tween);
		if (OnFullyVisible != null)
		{
			OnFullyVisible();
		}
		if (CoroutineToWaitFor != null)
		{
			WaitForCoroutine(CoroutineToWaitFor);
		}
	}

	protected void SetupWaitForNextTip()
	{
		FTip tip = FTipManager.GetRandomTip();
		if (tip != null)
		{
			mTipLabel.Text = tip.Details;
		}
		FEntityAction[] actions = new FEntityAction[2]
		{
			new FEntityActionDelay(10f),
			new FEntityActionCallMethod(SetupWaitForNextTip)
		};
		AddAction(new FEntityActionSequence(actions));
	}

	public void WaitForCoroutine(IEnumerator coroutineToStart)
	{
		Coroutine coroutine = StartCoroutine(coroutineToStart);
		FGameUtil.StartCoroutine(DoWaitForCoroutine(coroutine));
	}

	protected IEnumerator DoWaitForCoroutine(Coroutine coroutine)
	{
		yield return coroutine;
		CoroutineToWaitFor = null;
		FApp.HUD.PopPage(this);
	}
}
