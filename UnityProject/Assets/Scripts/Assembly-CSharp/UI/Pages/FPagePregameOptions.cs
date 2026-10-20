using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPagePregameOptions @ 0x180d188..0x180ed38, Ghidra vaddr).
public class FPagePregameOptions : FUIPage
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

	protected FUIButton mAboutButton;

	protected FUIButton mLanguageButton;

	protected FUIButton mConsoleButton;

	protected FUIButton mResetSaveButton;

	protected FSprite mPGSIcon_Leaderboards;

	protected FSprite mPGSIcon_Achievements;

	protected const int maxButtonsOnPage = 6;

	public FPagePregameOptions(string name)
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
		mTitleText = new FLabel(Name + ".Title", "DefaultStyle", FLoc.GetString("Options"), 64f, true, true, false);
		mTitleText.Color = new Color(1f, 0f, 0f, 1f);
		mTitleText.SetParent(mBackground);
		mButtonStack = new FNodeStack(Name + ".PauseButtons", FNodeStack.Modes.CenteredY, 16f);
		mButtonStack.SetParent(mBackground);
		mAboutButton = new FUIButton(Name + ".AboutButton");
		mAboutButton.SetText(FLoc.GetString("AboutButton"), "DefaultStyle", 32f, 0f);
		mAboutButton.AddHandleRelease(HandleAboutButton);
		mLanguageButton = new FUIButton(Name + ".LanguageButton");
		mLanguageButton.SetText(LanguageLabel(), "DefaultStyle", 32f, 0f);
		mLanguageButton.AddHandleRelease(HandleLanguageButton);
		mSoundFXButton = new FUIButton(Name + ".SoundFXButton");
		mSoundFXButton.SetText(FLoc.GetString("SoundFXButtonOn"), "DefaultStyle", 32f, 0f);
		mSoundFXButton.AddHandleRelease(HandleSoundFXButton);
		mMusicButton = new FUIButton(Name + ".MusicButton");
		mMusicButton.SetText(FLoc.GetString("MusicButtonOn"), "DefaultStyle", 32f, 0f);
		mMusicButton.AddHandleRelease(HandleMusicButton);
		mResetSaveButton = new FUIButton(Name + ".ResetSaveButton");
		mResetSaveButton.SetText(FLoc.GetString("ResetSaveButton"), "DefaultStyle", 32f, 0f);
		mResetSaveButton.Color = FColorNames.Get("LegendRed");
		mResetSaveButton.AddHandleRelease(HandleResetSaveButton);
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
		mButtonStack.StackChild(mAboutButton, cTextButtonSize);
		mButtonStack.StackChild(mLanguageButton, cTextButtonSize);
		mButtonStack.StackChild(mSoundFXButton, cTextButtonSize);
		mButtonStack.StackChild(mMusicButton, cTextButtonSize);
		mButtonStack.StackChild(mResetSaveButton, cTextButtonSize);
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

	protected static string LanguageLabel()
	{
		string label = FLoc.GetString("LanguageButton");
		if (string.IsNullOrEmpty(label) || label == "LanguageButton")
		{
			label = "Language";
		}
		return label + ": " + FApp.LanguageDisplayName(FApp.PreferredLanguage);
	}

	protected void HandleLanguageButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		FPageStringPicker picker = FApp.HUD.StartStringPicker();
		picker.AddRandomChoice = false;
		string title = FLoc.GetString("LanguageButton");
		picker.TitleText = (string.IsNullOrEmpty(title) || title == "LanguageButton") ? "Language" : title;
		picker.EntrySize = new Vector2(picker.EntrySize.x, 96f);
		string current = FApp.PreferredLanguage;
		picker.CurrentChoice = current;
		foreach (string code in FApp.SupportedLanguages)
		{
			picker.AddChoice(code, FApp.LanguageDisplayName(code), string.Empty, string.Empty);
		}
		picker.AddChoiceChangedEvent(delegate(string code)
		{
			if (code != current)
			{
				// let the picker finish its close transition before the HUD is rebuilt
				FGameUtil.DelayExecute(0.35f, delegate
				{
					FApp.SetPreferredLanguage(code, true);
				});
			}
		});
		FApp.HUD.FinishStringPicker(picker);
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

	protected void HandleAboutButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		FApp.HUD.PushPage(FApp.HUD.PageAbout, true, false);
	}

	protected void HandleResetSaveButton(FUIButton button, FTouch touch)
	{
		if (FApp.PlayerStats == null || FApp.PlayerData == null)
		{
			return;
		}
		if (!(FApp.World != null))
		{
			FApp.HUD.OpenYesNoDialog(FLoc.GetString("ResetSave.Title"), FLoc.GetString("ResetSave.Details"), FLoc.GetString("YesButton"), ConfirmResetSave, FLoc.GetString("NoButton"), null, ConfirmResetSave, null, FUIPage.TransitionStyles.Right, FPageOkDialog.AndroidBackButtonType.Button1);
		}
		else
		{
			FApp.SoundManager.PlaySoundSet("UIFail");
			FConsoleCore.Error("Reset Save cannot be done while you are in a game. Quit to the Main Menu.");
		}
	}

	protected void ConfirmResetSave()
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		FApp.PlayerStats.FullWipePlayerState();
		FApp.PlayerData.Save(); // PC port: persist the wipe (incl. the PlayerPrefs backup) so a restart cannot restore the old save
		if (FApp.HUD.PageLegendMainMenu != null && FApp.HUD.PageLegendMainMenu.OnStage)
		{
			FApp.HUD.PageLegendMainMenu.CheckForLostSaves();
		}
	}

	public void HandleScreenCollider(FUICollider collider, FTouch touch)
	{
		TransitionOut(mDialog);
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
