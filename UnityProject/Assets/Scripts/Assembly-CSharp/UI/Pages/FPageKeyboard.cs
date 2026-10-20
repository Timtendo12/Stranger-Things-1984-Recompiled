using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageKeyboard @ 0x17a6078..0x17a6e14, Ghidra vaddr).
// PC port: when the platform has no on-screen keyboard (TouchScreenKeyboard.isSupported == false) OpenKeyboard is a no-op,
// the hardware keyboard drives the focused FTextEntry directly.
public class FPageKeyboard : FUIPage
{
	protected const float cEntryTextPointSize = 48f;

	protected const float cInstructionTextPointSize = 24f;

	protected static readonly Vector2 cDialogSize = new Vector2(600f, 200f);

	protected static readonly Vector2 cDialogOffset = new Vector2(0f, 40f);

	protected static readonly Vector2 cEntryTextSize = new Vector2(560f, 60f);

	protected static readonly Vector2 cEntryTextOffset = new Vector2(0f, -20f);

	protected static readonly Vector2 cInstructionTextSize = new Vector2(560f, 36f);

	protected static readonly Vector2 cInstructionTextOffset = new Vector2(0f, 20f);

	protected FUICollider mScreenCollider;

	protected FUICollider mCollider;

	protected FNode mDialog;

	protected FSlicedSprite mBackground;

	protected FLabel mEntryText;

	protected FLabel mInstructionText;

	protected Action<bool, string> OnKeyboardFinish;

	protected string mDefaultText;

	protected string mPlaceholderText;

	protected FKeyboardOptions mKeyboardOptions;

	protected TouchScreenKeyboard mKeyboard;

	public FPageKeyboard()
		: base("PageKeyboard")
	{
		mDefaultText = string.Empty;
		mPlaceholderText = string.Empty;
		base.TransitionDialogInEaseType = GoEaseType.ExpoOut;
		base.TransitionDialogInTime = 0.5f;
		base.TransitionFadeInTime = 0.25f;
		base.TransitionStyle = TransitionStyles.Bottom;
		base.TransitionDialogOutEaseType = GoEaseType.SineIn;
		base.TransitionDialogOutTime = 0.1f;
		base.TransitionFadeOutTime = 0.1f;
		mScreenCollider = new FUICollider("ScreenCollider", false);
		mScreenCollider.AddSignalRelease(HandleScreenColliderRelease, 40f);
		mScreenCollider.SetParent(this);
		mCollider = new FUICollider("DialogCollider", false);
		mCollider.AddSignalRelease(HandleColliderRelease, 40f);
		mCollider.SetParent(this);
		mDialog = new FNode("DialogRoot");
		mDialog.SetParent(this);
		mBackground = new FSlicedSprite("Background", "UIBox001.png");
		mBackground.SetParent(mDialog);
		mEntryText = new FLabel("EntryText", "DefaultStyle", string.Empty, 48f, true, true, false);
		mEntryText.SetParent(mBackground);
		mInstructionText = new FLabel("InstructionText", "DefaultStyle", string.Empty, 24f, true, true, false);
		mInstructionText.SetParent(mBackground);
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		mScreenCollider.DockInside(this, InnerDock.BottomLeft, Futile.Screen.Size);
		mCollider.DockInside(this, InnerDock.Bottom, cDialogSize, cDialogOffset);
		mDialog.DockInside(this, InnerDock.Bottom, Futile.Screen.Size);
		mBackground.DockInside(mDialog, InnerDock.Bottom, cDialogSize, cDialogOffset);
		mEntryText.DockInside(mBackground, InnerDock.Top, cEntryTextSize, cEntryTextOffset);
		mInstructionText.DockInside(mBackground, InnerDock.Bottom, cInstructionTextSize, cInstructionTextOffset);
		MarkDirtyRecursive();
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
		if (mKeyboard != null)
		{
			if (mKeyboard.wasCanceled)
			{
				FinishKeyboard(false);
			}
			else if (mKeyboard.done)
			{
				FinishKeyboard(true);
			}
			else
			{
				mEntryText.Text = mKeyboard.text.StripEmoji();
			}
		}
	}

	protected void FinishKeyboard(bool didFinish)
	{
		string text = string.Empty;
		if (mKeyboard != null)
		{
			text = mKeyboard.text;
			mKeyboard.active = false;
			mKeyboard = null;
		}
		if (OnKeyboardFinish != null)
		{
			OnKeyboardFinish(didFinish, text.StripEmoji());
		}
		TransitionOut();
	}

	public void OpenKeyboard(Action<bool, string> onFinishCallback, FKeyboardOptions options, string text = "", string placeholderText = "")
	{
		if (!TouchScreenKeyboard.isSupported)
		{
			// PC port: hardware keyboard, no on-screen keyboard page
			return;
		}
		OnKeyboardFinish = onFinishCallback;
		mDefaultText = text;
		mPlaceholderText = placeholderText;
		mKeyboardOptions = options;
		FApp.HUD.PushPage(FApp.HUD.PageKeyboard, true, false);
	}

	public void DisableCollider()
	{
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		mKeyboard = TouchScreenKeyboard.Open(mDefaultText, mKeyboardOptions.KeyboardType, mKeyboardOptions.Autocorrect, mKeyboardOptions.Multiline, mKeyboardOptions.Secure, false, mPlaceholderText);
		LayoutPage();
		TransitionIn(mDialog);
		FApp.SoundManager.PlaySoundSet("UI.CardMenu.Open");
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		if (mKeyboard != null)
		{
			mKeyboard.active = false;
		}
		mKeyboard = null;
		OnKeyboardFinish = null;
		mDefaultText = string.Empty;
		mPlaceholderText = string.Empty;
		FApp.SoundManager.PlaySoundSet("UI.CardMenu.Close");
	}

	public override void TransitionOut()
	{
		TransitionOut(mDialog);
	}

	public void VoidCloseKeyboard()
	{
		FinishKeyboard(false);
	}

	public override bool HandleBackKey()
	{
		TransitionOut();
		return true;
	}

	protected void HandleScreenColliderRelease(FUICollider collider, FTouch touch)
	{
		DoClose();
	}

	protected void HandleColliderRelease(FUICollider collider, FTouch touch)
	{
	}

	public void DoClose()
	{
		mDialog.AddAction(new FEntityActionSequence(new FEntityAction[1]
		{
			new FEntityActionCallMethod(VoidCloseKeyboard, true)
		}));
	}
}
