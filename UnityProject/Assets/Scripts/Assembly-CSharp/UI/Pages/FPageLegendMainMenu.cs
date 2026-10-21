using System;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageLegendMainMenu @ 0x17ff6a8..0x180396c, Ghidra vaddr).
public class FPageLegendMainMenu : FUIPage
{
	protected const float cTimeBetweenAlphaSwitch = 0.35f;

	protected FNode mUIContainer;

	protected FUICollider mBGCollider;

	protected FSprite mBGBackdrop;

	protected FSprite mBG;

	protected FSprite mBGStars;

	protected FLabel mCopyrightInfo;

	protected FUIButton mPlayButton;

	protected FUIButton mNewGameButton;

	protected FSprite mWatchOnText;

	protected FUIButton mOptionsButton;

	protected float mLastVisSwitch;

	protected FNodeStack mStrangerStack;

	protected FNodeStack mThingsStack;

	protected FSprite[] mStrangerSprites;

	protected FSprite[] mThingsSprites;

	protected FSprite mLeftBar;

	protected FSprite mRightBar;

	protected FSprite mTopBar;

	protected FLabel mSubtitle;

	protected bool mIntroFinished;

	public FPageLegendMainMenu(string name)
		: base(name)
	{
		mUIContainer = new FNode("UIContainer");
		mUIContainer.SetParent(this);
		mUIContainer.ExpandToFill(this, 0f, 0f);
		mBGCollider = new FUICollider(Name + ".BGCollider", true);
		mBGCollider.ExpandToFill(this, 0f, 0f);
		mBGCollider.SetParent(mUIContainer);
		mBGCollider.AddSignalPress(HandlePress);
		mBGBackdrop = new FSprite("BlackSquare.png");
		mBGBackdrop.SetParent(mUIContainer);
		mBGBackdrop.ExpandToFill(mUIContainer, 0f, 0f);
		mBG = new FSprite("AppLoadHD.png");
		mBG.SetParent(mUIContainer);
		mBGStars = new FSprite("StaticStars.png");
		mBGStars.SetParent(mUIContainer);
		mStrangerStack = new FNodeStack(Name + ".StrangerStack", FNodeStack.Modes.CenteredX, 0f);
		mStrangerStack.SetParent(mUIContainer);
		mStrangerSprites = new FSprite[8];
		for (int i = 0; i < mStrangerSprites.Length; i++)
		{
			mStrangerSprites[i] = new FSprite(Name + ".Stranger" + i.ToString(), string.Format("Letter{0:D2}.png", i + 1));
		}
		mThingsSprites = new FSprite[6];
		for (int j = 0; j < mThingsSprites.Length; j++)
		{
			mThingsSprites[j] = new FSprite(Name + ".Things" + j.ToString(), string.Format("Letter{0:D2}.png", j + 9));
		}
		mThingsStack = new FNodeStack(Name + ".ThingsStack", FNodeStack.Modes.CenteredX, 0f);
		mThingsStack.SetParent(mUIContainer);
		mNewGameButton = new FUIButton(Name + ".NewGameButton");
		mNewGameButton.SetParent(mUIContainer);
		mNewGameButton.SetText(FLoc.GetString("StartButton"), "DefaultStyle", 48f, 14f);
		mNewGameButton.AddHandleRelease(HandleNewGameButton);
		mOptionsButton = new FUIButton(Name + ".OptionsButton");
		mOptionsButton.SetParent(mUIContainer);
		mOptionsButton.SetBackground("ButtonMenu.png");
		mOptionsButton.AddHandleRelease(HandleOptionsButton);
		mCopyrightInfo = new FLabel(Name + ".CopyrightInfo", "DefaultStyle", FLoc.GetString("CopyrightInfo"), 24f, true, true, false);
		mCopyrightInfo.Justify(Sides.CenterX, Sides.CenterY);
		mCopyrightInfo.Color = FUI.Colors.LegendRed;
		mCopyrightInfo.SetParent(mUIContainer);
		mSubtitle = new FLabel(Name + ".Subtitle", "DefaultStyle", FLoc.GetString("TheGame"), 32f, true, true, false);
		mSubtitle.Justify(Sides.CenterX, Sides.CenterY);
		mSubtitle.Color = FUI.Colors.LegendRed;
		mSubtitle.SetParent(mUIContainer);
		mLeftBar = new FSprite(Name + ".LeftBar", "SideBar.png");
		mLeftBar.SetParent(mUIContainer);
		mRightBar = new FSprite(Name + ".RightBar", "SideBar.png");
		mRightBar.SetParent(mUIContainer);
		mTopBar = new FSprite(Name + ".TopBar", "TopBar.png");
		mTopBar.SetParent(mUIContainer);
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		float scale = Futile.Screen.Height / 1080f;
		mBG.DockInside(mUIContainer, InnerDock.Center, scale * 1920f, scale * 1080f, 0f, 0f);
		mBGStars.DockInside(mBG, InnerDock.Top, 844f, 388f, 0f, 0f);
		mStrangerStack.DockInside(mUIContainer, InnerDock.Top, 1000f, 200f, 0f, -96f);
		StackStrangerSprites();
		mThingsStack.DockOutside(mStrangerStack, OuterDock.Bottom, 1000f, 200f, 0f, 110f);
		StackThingsSprites();
		mLeftBar.DockInside(mThingsStack, InnerDock.Left, 92f, 12f, 200f, 8f);
		mRightBar.DockInside(mThingsStack, InnerDock.Right, 92f, 12f, -200f, 8f);
		mTopBar.DockOutside(mStrangerStack, OuterDock.Top, 592f, 12f, 0f, -50f);
		mSubtitle.DockOutside(mThingsStack, OuterDock.Bottom, 1000f, 40f, 0f, 50f);
		mNewGameButton.DockInside(mUIContainer, InnerDock.Center, 1000f, 128f, 0f, -170f);
		mCopyrightInfo.DockInside(mUIContainer, InnerDock.Bottom, Futile.Screen.Width, 72f, 0f, 16f);
		mOptionsButton.DockInside(mUIContainer, InnerDock.TopRight, 128f, 128f, -16f, -16f);
	}

	protected void StackStrangerSprites()
	{
		mStrangerStack.ClearStack();
		mStrangerStack.StackChild(mStrangerSprites[0], new Vector2(92f, 104f), new Vector2(0f, -14f));
		mStrangerStack.StackChild(mStrangerSprites[1], new Vector2(64f, 76f), new Vector2(-12f, 0f));
		mStrangerStack.StackChild(mStrangerSprites[2], new Vector2(84f, 76f), new Vector2(0f, 0f));
		mStrangerStack.StackChild(mStrangerSprites[3], new Vector2(72f, 76f), new Vector2(-12f, 0f));
		mStrangerStack.StackChild(mStrangerSprites[4], new Vector2(76f, 76f), new Vector2(-4f, 0f));
		mStrangerStack.StackChild(mStrangerSprites[5], new Vector2(80f, 76f), new Vector2(0f, 0f));
		mStrangerStack.StackChild(mStrangerSprites[6], new Vector2(68f, 76f), new Vector2(0f, 0f));
		mStrangerStack.StackChild(mStrangerSprites[7], new Vector2(108f, 104f), new Vector2(-6f, -14f));
	}

	protected void StackThingsSprites()
	{
		mThingsStack.ClearStack();
		mThingsStack.StackChild(mThingsSprites[0], new Vector2(64f, 76f), new Vector2(0f, 0f));
		mThingsStack.StackChild(mThingsSprites[1], new Vector2(80f, 76f), new Vector2(0f, 0f));
		mThingsStack.StackChild(mThingsSprites[2], new Vector2(36f, 76f), new Vector2(4f, 0f));
		mThingsStack.StackChild(mThingsSprites[3], new Vector2(72f, 76f), new Vector2(4f, 0f));
		mThingsStack.StackChild(mThingsSprites[4], new Vector2(76f, 76f), new Vector2(0f, 0f));
		mThingsStack.StackChild(mThingsSprites[5], new Vector2(68f, 76f), new Vector2(0f, 0f));
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		if (FApp.SoundManager != null)
		{
			FApp.SoundManager.PlayMusic("MenuTheme", 0.25f, 0.25f, true, 0f);
		}
		mNewGameButton.Visible = false;
		mLastVisSwitch = Time.time;
		if (FApp.PlayerStats != null && FApp.PlayerStats.HadOutOfDateSaves)
		{
			FApp.PlayerStats.HadOutOfDateSaves = false;
			FApp.HUD.OpenOkDialog(FLoc.GetString("SaveOutOfDate.Title"), FLoc.GetString("SaveOutOfDate.Details"), null, null, null, TransitionStyles.Right, FPageOkDialog.AndroidBackButtonType.Button0);
		}
		FinalizeIntroAnim();
		mStrangerStack.SetVisibleRecursive(false);
		mThingsStack.SetVisibleRecursive(false);
		mSubtitle.SetVisibleRecursive(false);
		mNewGameButton.SetVisibleRecursive(false);
		mOptionsButton.SetVisibleRecursive(false);
		mCopyrightInfo.SetVisibleRecursive(false);
		mLeftBar.SetVisibleRecursive(false);
		mRightBar.SetVisibleRecursive(false);
		mTopBar.SetVisibleRecursive(false);
		mBGCollider.SetParent(mUIContainer);
		FGameUtil.ExecuteNextFrame(DoIntroAnim);
		if (FApp.PlayerData != null)
		{
			if (FApp.PlayerData.HadCorruptedSaveData)
			{
				FApp.PlayerData.HadCorruptedSaveData = false;
			}
			if (!FApp.PlayerData.HasCheckedForLostSaves)
			{
				Silent_CheckForLostSaves();
			}
			else
			{
				FLog.Log("Player has previously checked for lost saves, skipping step.", true);
			}
		}
	}

	protected void DoIntroAnim()
	{
		mIntroFinished = false;
		mStrangerStack.SetVisibleRecursive(true);
		mThingsStack.SetVisibleRecursive(true);
		for (int i = 0; i < mStrangerSprites.Length; i++)
		{
			FlyInSprite(mStrangerSprites[i]);
		}
		for (int j = 0; j < mThingsSprites.Length; j++)
		{
			FlyInSprite(mThingsSprites[j]);
		}
		AddAction(new FEntityActionSequence(new FEntityAction[2]
		{
			new FEntityActionDelay(4f),
			new FEntityActionCallMethod(DoSecondaryIntroAnim)
		}));
	}

	private void FlyInSprite(FSprite sprite)
	{
		float offsetX = Vector2.zero.x;
		float offsetY = Vector2.zero.y;
		bool vertical = !FRandom.Generic.RandBool();
		bool negative = FRandom.Generic.RandBool();
		float distance = FRandom.Generic.RangeFloat(800f, 1500f);
		float delta = (negative ? (0f - distance) : distance);
		if (vertical)
		{
			offsetY += delta;
		}
		else
		{
			offsetX += delta;
		}
		sprite.x = sprite.x + offsetX;
		sprite.y = sprite.y + offsetY;
		sprite.AddAction(new FEntityActionSpawn(new FEntityAction[2]
		{
			new FEntityActionTweenPropertyByFloat(4f, "x", 0f - offsetX, GoEaseType.CircOut),
			new FEntityActionTweenPropertyByFloat(4f, "y", 0f - offsetY, GoEaseType.CircOut)
		}));
	}

	protected void DoSecondaryIntroAnim()
	{
		mSubtitle.SetVisibleRecursive(true);
		mLeftBar.SetVisibleRecursive(true);
		mRightBar.SetVisibleRecursive(true);
		mTopBar.SetVisibleRecursive(true);
		mLeftBar.ScaleX = 0f;
		mRightBar.ScaleX = 0f;
		mTopBar.ScaleX = 0f;
		FEntityAction[] leftActions = new FEntityAction[2];
		leftActions[0] = new FEntityActionTweenPropertyToFloat(1f, "ScaleX", 0f, 1f, GoEaseType.CircOut);
		float leftX = mLeftBar.x;
		float leftWidth = mLeftBar.Width;
		leftActions[1] = new FEntityActionTweenPropertyToFloat(1f, "x", leftX + leftWidth * 0.5f, mLeftBar.x, GoEaseType.CircOut);
		mLeftBar.AddAction(new FEntityActionSpawn(leftActions));
		mTopBar.AddAction(new FEntityActionTweenPropertyToFloat(1f, "ScaleX", 0f, 1f, GoEaseType.CircOut));
		FEntityAction[] rightActions = new FEntityAction[2];
		rightActions[0] = new FEntityActionTweenPropertyToFloat(1f, "ScaleX", 0f, 1f, GoEaseType.CircOut);
		float rightX = mRightBar.x;
		float rightWidth = mRightBar.Width;
		rightActions[1] = new FEntityActionTweenPropertyToFloat(1f, "x", rightX + rightWidth * -0.5f, mRightBar.x, GoEaseType.CircOut);
		mRightBar.AddAction(new FEntityActionSpawn(rightActions));
		mSubtitle.ScaleY = 0f;
		FEntityAction[] subtitleActions = new FEntityAction[2];
		subtitleActions[0] = new FEntityActionTweenPropertyToFloat(1f, "ScaleY", 0f, 1f, GoEaseType.CircOut);
		float subtitleY = mSubtitle.y;
		float subtitleHeight = mSubtitle.Height;
		subtitleActions[1] = new FEntityActionTweenPropertyToFloat(1f, "y", subtitleY + subtitleHeight * 0.5f, mSubtitle.y, GoEaseType.CircOut);
		mSubtitle.AddAction(new FEntityActionSpawn(subtitleActions));
		AddAction(new FEntityActionSequence(new FEntityAction[2]
		{
			new FEntityActionDelay(1f),
			new FEntityActionCallMethod(FinalizeIntroAnim)
		}));
	}

	protected void FinalizeIntroAnim()
	{
		mIntroFinished = true;
		RemoveAllActions();
		for (int i = 0; i < mStrangerSprites.Length; i++)
		{
			mStrangerSprites[i].RemoveAllActions();
			mStrangerSprites[i].ClearUserPosition();
		}
		for (int j = 0; j < mThingsSprites.Length; j++)
		{
			mThingsSprites[j].RemoveAllActions();
			mThingsSprites[j].ClearUserPosition();
		}
		mSubtitle.RemoveAllActions();
		mTopBar.RemoveAllActions();
		mRightBar.RemoveAllActions();
		mLeftBar.RemoveAllActions();
		mSubtitle.ClearUserPosition();
		mRightBar.ClearUserPosition();
		mLeftBar.ClearUserPosition();
		mSubtitle.ScaleY = 1f;
		mTopBar.ScaleX = 1f;
		mRightBar.ScaleX = 1f;
		mLeftBar.ScaleX = 1f;
		mSubtitle.SetVisibleRecursive(true);
		mNewGameButton.SetVisibleRecursive(true);
		mOptionsButton.SetVisibleRecursive(true);
		mCopyrightInfo.SetVisibleRecursive(true);
		mLeftBar.SetVisibleRecursive(true);
		mRightBar.SetVisibleRecursive(true);
		mTopBar.SetVisibleRecursive(true);
		mBGCollider.Unparent();
		LayoutPage();
		MarkDirtyRecursive();
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		if (FApp.SoundManager != null)
		{
			FApp.SoundManager.FadeOutMusic(0.1f);
		}
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
		if (0.35f < Time.time - mLastVisSwitch)
		{
			// PC port: blink the label, not the button, so the button stays clickable / keyboard-focusable while blinked off
			mNewGameButton.Label.Visible = !mNewGameButton.Label.Visible;
			mLastVisSwitch = Time.time;
		}
	}

	// PC port: Enter on the title screen starts the game instead of opening the options.
	public override FUICollider PCDefaultFocus => mNewGameButton.Collider;

	protected void HandlePress(FUICollider coll, FTouch touch)
	{
		if (!mIntroFinished)
		{
			FinalizeIntroAnim();
		}
	}

	protected void TryClearIntro()
	{
		if (!mIntroFinished)
		{
			FinalizeIntroAnim();
		}
	}

	protected void HandleNewGameButton(FUIButton button, FTouch touch)
	{
		if (!mIntroFinished)
		{
			FinalizeIntroAnim();
		}
		FApp.SoundManager.PlaySoundSet("UISuccess");
		FApp.HUD.PushPage(FApp.HUD.PageGameModeChoice);
	}

	protected void HandleOptionsButton(FUIButton button, FTouch touch)
	{
		if (!mIntroFinished)
		{
			FinalizeIntroAnim();
		}
		FApp.SoundManager.PlaySoundSet("UISuccess");
		FApp.HUD.PushPage(FApp.HUD.PagePregameOptions);
	}

	public void ShowMenuButton(bool show)
	{
		if (!show)
		{
			if (mOptionsButton.OnStage)
			{
				mOptionsButton.Unparent();
			}
		}
		else if (!mOptionsButton.OnStage)
		{
			mOptionsButton.SetParent(mUIContainer);
		}
	}

	protected async void Silent_CheckForLostSaves()
	{
		if (FApp.PlayerData.HasLoadedCloudSaveData)
		{
			return;
		}
		FLog.Warning("SAVEDATALOGGING: Player has not checked for lost saves yet. Checking and adding to current playerstats");
		FApp.PlayerData.HasCheckedForLostSaves = true;
		FPlayerData[] datas = await FPlayerData.DebugLoadAllSavedPlayerData(false);
		if (datas == null)
		{
			return;
		}
		for (int i = 0; i < datas.Length; i++)
		{
			FPlayerData data = datas[i];
			if (data.LoadedXmlDoc == null)
			{
				continue;
			}
			FPlayerStats stats = new FPlayerStats();
			XmlElement root = data.LoadedXmlDoc.DocumentElement;
			stats.Deserialize(root.GetChildValue("SavedState", string.Empty));
			if (stats.SwitchToFirstSaveSlotOfType(0))
			{
				FApp.PlayerStats.SavedPlayerStates.Add(stats.CurrentPlayerState);
			}
			if (stats.SwitchToFirstSaveSlotOfType(1))
			{
				FApp.PlayerStats.SavedPlayerStates.Add(stats.CurrentPlayerState);
			}
			string unlocks = root.GetChildValue("PermanentUnlocks", string.Empty);
			if (unlocks.Valid())
			{
				string[] names = unlocks.Split('|');
				for (int j = 0; j < names.Length; j++)
				{
					FApp.PlayerData.AddPermanentUnlock(names[j]);
				}
			}
		}
	}

	public async void CheckForLostSaves()
	{
		if (FApp.PlayerData.HasLoadedCloudSaveData)
		{
			return;
		}
		FPlayerData[] totalDatas = await FPlayerData.DebugLoadAllSavedPlayerData(true);
		if (totalDatas == null || totalDatas.Length <= 1)
		{
			return;
		}
		int savesWithProgress = 0;
		for (int i = 0; i < totalDatas.Length; i++)
		{
			FPlayerData data = totalDatas[i];
			if (data.LoadedXmlDoc == null)
			{
				continue;
			}
			FPlayerStats stats = new FPlayerStats();
			stats.Deserialize(data.LoadedXmlDoc.DocumentElement.GetChildValue("SavedState", string.Empty));
			bool hasProgress = stats.SwitchToFirstSaveSlotOfType(0) && stats.GetPercentComplete() > 0f;
			if (!stats.SwitchToFirstSaveSlotOfType(1))
			{
				if (hasProgress)
				{
					savesWithProgress++;
				}
				continue;
			}
			if (stats.GetPercentComplete() > 0f)
			{
				hasProgress = true;
			}
			if (hasProgress)
			{
				savesWithProgress++;
			}
		}
		if (savesWithProgress > 1)
		{
			Action openRecovery = delegate
			{
				FApp.HUD.PushPage(FApp.HUD.PageSaveRecovery);
				FApp.HUD.PageSaveRecovery.UpdateSaves(totalDatas);
			};
			FApp.HUD.OpenYesNoDialog(FLoc.GetString("SaveRecovery.Prompt.Title"), FLoc.GetString("SaveRecovery.Prompt.Desc"), FLoc.GetString("YesButton"), openRecovery, FLoc.GetString("NoButton"), null, openRecovery, null, TransitionStyles.Right, FPageOkDialog.AndroidBackButtonType.Button1);
		}
	}
}
