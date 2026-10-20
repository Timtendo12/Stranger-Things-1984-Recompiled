using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPagePause @ 0x17ac0d0..0x17ad940, Ghidra vaddr).
public class FPagePause : FUIPage
{
	public static readonly Vector2 cSize = new Vector2(600f, 700f);

	public static readonly Vector2 cTextButtonSize = new Vector2(500f, 72f);

	protected FUICollider mCollider;

	protected FNode mDialog;

	protected FSlicedSprite mBackground;

	protected FLabel mTitleText;

	protected FNodeStack mButtonStack;

	protected FUIButton mSoundFXButton;

	protected FUIButton mMusicButton;

	protected FUIButton mQuitButton;

	protected FUIButton mConsoleButton;

	protected FUIButton mBugButton;

	public FPagePause(string name)
		: base(name)
	{
		base.Modal = true;
		base.DarkenLower = true;
		base.TransitionStyle = TransitionStyles.Scale;
		mCollider = new FUICollider("Pause", false);
		mCollider.AddSignalPress(HandleScreenCollider);
		mCollider.SetParent(this);
		mDialog = new FNode("DialogPause");
		mDialog.SetParent(this);
		mBackground = new FSlicedSprite(Name + ".Background", "Dialog003.png");
		mBackground.SetParent(mDialog);
		mTitleText = new FLabel(Name + ".Title", "DefaultStyle", FLoc.GetString("Paused"), 64f, true, true, false);
		mTitleText.Color = new Color(1f, 0f, 0f, 1f);
		mTitleText.SetParent(mBackground);
		mButtonStack = new FNodeStack(Name + ".PauseButtons", FNodeStack.Modes.CenteredY, 16f);
		mButtonStack.SetParent(mBackground);
		mQuitButton = new FUIButton(Name + ".QuitButton");
		mQuitButton.SetText(FLoc.GetString("QuitGameButton"), "DefaultStyle", 32f, 0f);
		mQuitButton.AddHandleRelease(HandleQuitButton);
		mSoundFXButton = new FUIButton(Name + ".SoundFXButton");
		mSoundFXButton.SetText(FLoc.GetString("SoundFXButtonOn"), "DefaultStyle", 32f, 0f);
		mSoundFXButton.AddHandleRelease(HandleSoundFXButton);
		mMusicButton = new FUIButton(Name + ".MusicButton");
		mMusicButton.SetText(FLoc.GetString("MusicButtonOn"), "DefaultStyle", 32f, 0f);
		mMusicButton.AddHandleRelease(HandleMusicButton);
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		mCollider.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		mDialog.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		mBackground.DockInside(mDialog, InnerDock.Center, cSize.x, cSize.y, 0f, 0f);
		mTitleText.DockInside(mBackground, InnerDock.Top, cSize.x - 48f, 64f, 0f, -24f);
		mButtonStack.DockInside(mBackground, InnerDock.Bottom, cSize.x - 48f, cSize.y - 72f - 64f, 0f, 24f);
		mButtonStack.ClearStack();
		mButtonStack.StackChild(mQuitButton, cTextButtonSize);
		mButtonStack.StackChild(mSoundFXButton, cTextButtonSize);
		mButtonStack.StackChild(mMusicButton, cTextButtonSize);
		MarkDirtyRecursive();
	}

	public override void TransitionOut()
	{
		TransitionOut(mDialog);
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		Go.killAllAbstractTweensWithTarget(mDialog);
		TransitionIn(mDialog);
		SyncButtonText();
	}

	protected void SyncButtonText()
	{
		if (mSoundFXButton != null)
		{
			mSoundFXButton.SetText(FLoc.GetString((FApp.SoundManager.EffectVolume <= 0f) ? "SoundFXButtonOff" : "SoundFXButtonOn"));
		}
		if (mMusicButton != null)
		{
			mMusicButton.SetText(FLoc.GetString((FApp.SoundManager.MusicVolume <= 0f) ? "MusicButtonOff" : "MusicButtonOn"));
		}
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		Go.killAllAbstractTweensWithTarget(mDialog);
	}

	public override bool HandleInput(float deltaTime)
	{
		bool escape = Input.GetKeyDown(KeyCode.Escape);
		if (escape && FApp.HUD.PageLegendWorld != null)
		{
			FApp.HUD.PageLegendWorld.HidePauseMenu();
		}
		return escape;
	}

	protected void HandleResumeButton(FUIButton button, FTouch touch)
	{
		if (FApp.HUD.PageLegendWorld != null)
		{
			FApp.HUD.PageLegendWorld.HidePauseMenu();
		}
	}

	protected void Resume()
	{
		if (FApp.HUD.PageLegendWorld != null)
		{
			FApp.HUD.PageLegendWorld.HidePauseMenu();
		}
	}

	protected void HandleQuitButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		FApp.HUD.OpenYesNoDialog(FLoc.GetString("Pause.QuitGame.Title"), FLoc.GetString("Pause.QuitGame.Details"), FLoc.GetString("YesButton"), delegate
		{
			ConfirmMainMenu(0);
		}, FLoc.GetString("NoButton"), null, delegate
		{
			ConfirmMainMenu(0);
		}, null, FUIPage.TransitionStyles.Right, FPageOkDialog.AndroidBackButtonType.Button1);
	}

	protected void ConfirmMainMenu(int foo)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		if (!(FApp.World == null))
		{
			FApp.PlayerStats.Reset();
			FApp.DestroyWorld(true);
			TransitionOut(mDialog);
		}
	}

	protected void HandleSoundFXButton(FUIButton button, FTouch touch)
	{
		float volume = ((FApp.SoundManager.EffectVolume <= 0f) ? FPlayerData.DefaultEffectVolume : 0f);
		FApp.SoundManager.EffectVolume = volume;
		FApp.PlayerData.EffectVolume = volume;
		FApp.SoundManager.PlaySoundSet("UIButton");
		SyncButtonText();
	}

	protected void HandleMusicButton(FUIButton button, FTouch touch)
	{
		float volume = ((FApp.SoundManager.MusicVolume <= 0f) ? FPlayerData.DefaultMusicVolume : 0f);
		FApp.SoundManager.MusicVolume = volume;
		FApp.PlayerData.MusicVolume = volume;
		FApp.SoundManager.PlaySoundSet("UIButton");
		SyncButtonText();
	}

	protected void HandleConsoleButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		// debug console does not exist on PC
		if (FApp.HUD.PageDebug != null)
		{
			FApp.HUD.PageDebug.ToggleConsole();
		}
	}

	protected void HandleBugButton(FUIButton button, FTouch touch)
	{
		FPluginRelay.ReportBug();
	}

	public void HandleScreenCollider(FUICollider collider, FTouch touch)
	{
		if (FApp.HUD.PageLegendWorld != null)
		{
			FApp.HUD.PageLegendWorld.HidePauseMenu();
		}
	}

	public void ScreenShotOn()
	{
		if (base.OnStage)
		{
			mDialog.Visible = false;
		}
	}

	public void ScreenShotOff()
	{
		if (base.OnStage)
		{
			mDialog.Visible = true;
		}
	}
}
