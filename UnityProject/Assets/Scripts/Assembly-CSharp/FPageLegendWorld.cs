using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageLegendWorld @ 0x1803974..0x180ba78, Ghidra vaddr).
public class FPageLegendWorld : FUIPage
{
	public enum DialoguePlacement
	{
		TOP = 0,
		BOTTOM = 1
	}

	protected const float cPinchZoomMultiplier = 0.02f;

	public const float cDefaultTouchHoldInterval = 0.2f;

	public const float cBikeRaceTouchHoldInterval = 0f;

	public static float TouchHoldInterval = 0.2f;

	protected bool mLetterBoxMode;

	protected FNode mUIContainer;

	protected Vector2 mWorldSize = Vector2.zero;

	protected FUICollider mWorldCollider;

	protected Vector2 mHUDSize = Vector2.zero;

	protected FNode mHUDContainer;

	protected FSprite mHUDBackground;

	protected FUIButton mMenuButton;

	protected FSprite mHealthSprite;

	protected FLabelDropshadow mHealthLabel;

	protected FSprite mCurrencySprite;

	protected FUIPinballText mCurrencyLabel;

	protected FSprite mKeyCardSprite;

	protected FLabelDropshadow mKeyCardLabel;

	protected bool mDialogueOnLeft = true;

	protected FSlicedSprite mDialogueBG;

	protected FLabel mDialogueLabel;

	protected FLabel mDialogueSpeakerLabel;

	protected FSprite mDialogueSpeakerImage;

	protected FUIButton mDialogueDoneButton;

	protected Coroutine mDialogueTypeRoutine;

	protected bool mDialogueOnScreen;

	protected FDialogueLine[] mDialogueLines;

	protected int mDialogueIndex;

	protected FUnit mDialogueTarget;

	protected Vector2 mDialogueBGSize = Vector2.zero;

	protected string mCurrentCharacter = string.Empty;

	protected FUIButton mChangeCharacterButton;

	protected FUIButton mCharacterWeapon;

	protected FLabelDropshadow mCharacterAmmo;

	protected FUIButton mCycleCharacterButton;

	protected FSprite mCycleCharacterArrow;

	protected FLabelDropshadow mAreaNameLabel;

	protected FUIButton mInventoryButton;

	protected FUIButton mMapButton;

	protected FSprite mWorldDimmer;

	protected Dictionary<int, FTouch> mTouches = new Dictionary<int, FTouch>();

	protected bool mPinchZoomMode;

	protected float mInitialPinchDist;

	protected float mCurrPinchDist;

	protected FLoopingSoundHelper mLoopingSoundHelper;

	protected bool mTouchDown;

	protected Vector2 mLastTouchPos = Vector2.zero;

	protected float mLastTouchUpdate;

	protected bool mIsHolding;

	protected bool mCharacterSwitchingEnabled = true;

	protected FXmasLightsNode mXmasNode;

	protected FLabelDropshadow mBossName;

	protected FHealthBar mBossHealthBar;

	protected List<int> mRevealedQuestMarkers = new List<int>();

	protected bool mQuestMarkersSynced;

	private Action m_mDialogueCompleteAction;

	protected FLabel mDebugGameSpeedLabel;

	public Vector2 HUDSize => mHUDSize;

	public bool DialogueOnScreen => mDialogueOnScreen;

	protected event Action mDialogueCompleteAction
	{
		add
		{
			m_mDialogueCompleteAction = (Action)Delegate.Combine(m_mDialogueCompleteAction, value);
		}
		remove
		{
			m_mDialogueCompleteAction = (Action)Delegate.Remove(m_mDialogueCompleteAction, value);
		}
	}

	public event Action DialogueCompleteAction
	{
		add
		{
			mDialogueCompleteAction -= value;
			mDialogueCompleteAction += value;
		}
		remove
		{
			mDialogueCompleteAction -= value;
		}
	}

	static FPageLegendWorld()
	{
		TouchHoldInterval = 0.2f;
	}

	public FPageLegendWorld(string name)
		: base(name)
	{
		mUIContainer = new FNode("UIContainer");
		mUIContainer.SetParent(this);
		mUIContainer.ExpandToFill(this, 0f, 0f);
		mHUDContainer = new FNode("HUDContainer");
		mHUDContainer.SetParent(mUIContainer);
		mWorldCollider = new FUICollider(Name + ".WorldCollider", true);
		mWorldCollider.SetParent(mUIContainer);
		mWorldCollider.AddSignalRelease(HandlePressRelease, 40f);
		mWorldCollider.AddSignalPress(HandlePress);
		mWorldCollider.AddSignalMove(HandlePressMove);
		mWorldCollider.AddSignalCancel(HandlePressCancel);
		mWorldCollider.AddSignalAdditionalTouch(HandleAdditionalPress);
		mWorldDimmer = new FSprite(Name + ".WorldDimmer", "BlackSquare.png");
		mHUDBackground = new FSprite(Name + ".HUDBackground", "BlackSquare.png");
		mHUDBackground.SetParent(mHUDContainer);
		mHUDBackground.ExpandToFill(mHUDContainer, 0f, 0f);
		mMenuButton = new FUIButton(Name + ".MenuButton");
		mMenuButton.SetBackground("ButtonMenu.png");
		mMenuButton.SetParent(mHUDContainer);
		mMenuButton.AddHandleRelease(HandleMenuButton);
		mCurrencySprite = new FSprite(Name + ".CurrencySprite", "HUDCoin.png");
		mCurrencySprite.SetParent(mHUDContainer);
		mKeyCardSprite = new FSprite(Name + ".KeyCardSprite", "HUDKeyCard.png");
		mKeyCardSprite.SetParent(mHUDContainer);
		mInventoryButton = new FUIButton(Name + ".InventoryButton");
		mInventoryButton.SetParent(mHUDContainer);
		mInventoryButton.SetBackground("ButtonInventory.png");
		mInventoryButton.AddHandleRelease(HandleInventoryButton);
		mMapButton = new FUIButton(Name + ".MapButton");
		mMapButton.SetParent(mHUDContainer);
		mMapButton.SetBackground("ButtonMap.png");
		mMapButton.AddHandleRelease(HandleMapButton);
		mCycleCharacterButton = new FUIButton(Name + ".CycleCharButton");
		mCycleCharacterButton.SetParent(mHUDContainer);
		mCycleCharacterButton.SetBackground("CharacterSwap_button.png");
		mCycleCharacterButton.AddHandleRelease(HandlePlayerCycleButton);
		mCycleCharacterArrow = new FSprite(Name + ".CycleCharArrow", "CharacterSwap_Arrow.png");
		mCycleCharacterArrow.SetParent(mHUDContainer);
		mChangeCharacterButton = new FUIButton(Name + ".ChangeCharButton");
		mChangeCharacterButton.SetParent(mHUDContainer);
		mChangeCharacterButton.SetBackground("Button001.png");
		mChangeCharacterButton.AddHandleRelease(HandleChangeCharacterButton);
		mCharacterWeapon = new FUIButton(Name + ".CharacterWeapon");
		mCharacterWeapon.AddHandleTap(HandleCharacterWeaponButton);
		mCharacterWeapon.SetParent(mHUDContainer);
		mCharacterAmmo = new FLabelDropshadow(Name + ".CharacterAmmo", "DefaultStyle", "0", 24f, false, true, false);
		mCharacterAmmo.Justify(Sides.Right, Sides.Bottom);
		mCharacterAmmo.SetParent(mHUDContainer);
		mHealthSprite = new FSprite(Name + ".HealthSprite", "HUDHeartFull.png");
		mHealthSprite.SetParent(mHUDContainer);
		mHealthLabel = new FLabelDropshadow(Name + ".HealthLabel", "DefaultStyle", "0", 32f, false, true, false);
		mHealthLabel.Justify(Sides.CenterX, Sides.CenterY);
		mHealthLabel.SetParent(mHUDContainer);
		mCurrencyLabel = new FUIPinballText(Name + ".CurrencyLabel", "DefaultStyle", "0", 32f, false);
		mCurrencyLabel.Justify(Sides.Left, Sides.CenterY);
		mCurrencyLabel.SetParent(mHUDContainer);
		mKeyCardLabel = new FLabelDropshadow(Name + ".KeyCardLabel", "DefaultStyle", "0", 32f, false, true, false);
		mKeyCardLabel.Justify(Sides.Left, Sides.CenterY);
		mKeyCardLabel.SetParent(mHUDContainer);
		mAreaNameLabel = new FLabelDropshadow(Name + ".AreaName", "DefaultStyle", "0", 24f, false, true, false);
		mAreaNameLabel.Justify(Sides.CenterX, Sides.CenterY);
		mAreaNameLabel.SetParent(mUIContainer);
		mDialogueBG = new FSlicedSprite("Dialog002.png");
		mDialogueSpeakerLabel = new FLabel(Name + ".DialogSpeakerLabel", "DefaultStyle", string.Empty, 32f, true, true, false);
		mDialogueSpeakerLabel.Justify(Sides.CenterX, Sides.CenterY);
		mDialogueSpeakerLabel.Wrap = true;
		mDialogueSpeakerLabel.Color = new Color(1f, 0f, 0f, 1f);
		mDialogueSpeakerLabel.SetParent(mDialogueBG);
		mDialogueSpeakerImage = new FSprite("BlackSquare.png");
		mDialogueSpeakerImage.SetParent(mDialogueBG);
		mDialogueLabel = new FLabel(Name + ".DialogLabel", "DefaultStyle", string.Empty, 32f, true, true, false);
		mDialogueLabel.Justify(Sides.Left, Sides.Top);
		mDialogueLabel.Wrap = true;
		mDialogueLabel.SetParent(mDialogueBG);
		mDialogueDoneButton = new FUIButton(Name + ".DialogDoneButton");
		mDialogueDoneButton.SetText(string.Empty, "DefaultStyle", 32f, 14f);
		mDialogueDoneButton.AddHandleRelease(DialogueDoneButton);
		mLoopingSoundHelper = new FLoopingSoundHelper();
		mDebugGameSpeedLabel = new FLabel(Name + ".DebugGameSpeedLabel", "DefaultStyle", string.Empty, 24f, true, true, false);
		mBossHealthBar = new FHealthBar(Name + ".BossHealth");
		mBossName = new FLabelDropshadow(Name + ".BossName", "DefaultStyle", string.Empty, 32f, true, true, false);
		mXmasNode = new FXmasLightsNode(Name + ".XmasLightsNode");
		mWorldCollider.MoveToBottom();
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		if (Futile.Screen.IsWidescreen)
		{
			LayoutPageWidescreen();
		}
		else
		{
			LayoutPageNormal();
		}
	}

	protected void LayoutPageWidescreen()
	{
		float hudWidth;
		float hudHeight;
		if (!(FApp.World != null))
		{
			hudWidth = 214f;
			hudHeight = Futile.Screen.Height;
		}
		else
		{
			hudWidth = Futile.Screen.Width;
			hudHeight = Futile.Screen.Height;
			hudWidth -= hudWidth * FApp.World.CameraFactor.x;
		}
		mHUDSize = new Vector2(hudWidth, hudHeight);
		mHUDContainer.DockInside(mUIContainer, InnerDock.Right, hudWidth, hudHeight, 0f, 0f);
		mWorldSize = new Vector2(Futile.Screen.Width - mHUDSize.x, mHUDSize.y);
		mWorldCollider.DockInside(mUIContainer, InnerDock.Left, mWorldSize.x, mWorldSize.y, 0f, 0f);
		mWorldDimmer.DockInside(mUIContainer, InnerDock.Left, mWorldSize.x, mWorldSize.y, 0f, 0f);
		mMenuButton.DockInside(mHUDContainer, InnerDock.TopLeft, 96f, 96f, 104f, -8f);
		mCurrencySprite.DockInside(mHUDContainer, InnerDock.TopLeft, 64f, 64f, 16f, -114f);
		float labelWidth = mHUDSize.x + -8f + -8f + -8f + -64f;
		mCurrencyLabel.DockOutside(mCurrencySprite, OuterDock.Right, labelWidth, 64f, 16f, -6f);
		mKeyCardSprite.DockOutside(mCurrencySprite, OuterDock.BottomAlignLeft, 64f, 64f, 0f, -8f);
		mKeyCardLabel.DockOutside(mKeyCardSprite, OuterDock.Right, labelWidth, 64f, 16f, -6f);
		mMapButton.DockOutside(mKeyCardLabel, OuterDock.BottomAlignLeft, 96f, 96f, 8f, -72f);
		mInventoryButton.DockOutside(mMapButton, OuterDock.Left, 96f, 96f, -4f, 0f);
		mChangeCharacterButton.DockOutside(mMapButton, OuterDock.Bottom, 96f, 96f, 0f, -120f);
		mCycleCharacterButton.DockOutside(mChangeCharacterButton, OuterDock.Top, 80f, 80f, 0f, 24f);
		mCycleCharacterArrow.Rotation = 0f;
		mCycleCharacterArrow.DockOutside(mCycleCharacterButton, OuterDock.Bottom, 80f, 80f, 0f, 6f);
		mCharacterWeapon.DockOutside(mChangeCharacterButton, OuterDock.Left, 96f, 96f, -8f, 0f);
		mCharacterAmmo.DockInside(mCharacterWeapon, InnerDock.Center, 96f, 96f, 0f, 0f);
		mHealthSprite.DockOutside(mChangeCharacterButton, OuterDock.Bottom, 96f, 96f, 0f, -4f);
		mHealthLabel.DockInside(mHealthSprite, InnerDock.Center, 96f, 96f, 2f, -4f);
		mDialogueBGSize = new Vector2(Futile.Screen.Width - mHUDSize.x + -100f, 400f);
		mDialogueBG.DockOutside(mHUDContainer, OuterDock.LeftAlignTop, mDialogueBGSize.x, mDialogueBGSize.y, -32f, -32f);
		LayoutDialogueBits();
		mDebugGameSpeedLabel.DockOutside(mHUDContainer, OuterDock.LeftAlignTop, 400f, 128f, 0f, 0f);
		mBossHealthBar.DockInside(mUIContainer, InnerDock.Top, 512f, 64f, mHUDSize.x * -0.5f, -8f);
		mBossName.DockInside(mBossHealthBar, InnerDock.Center, 448f, 64f, 0f, 0f);
		mXmasNode.DockOutside(mHUDContainer, OuterDock.LeftAlignTop, 64f, 256f, -8f, -8f);
		mAreaNameLabel.DockInside(mUIContainer, InnerDock.Bottom, 256f, 64f, mHUDSize.x * -0.5f, 32f);
	}

	protected void LayoutPageNormal()
	{
		float hudWidth;
		float hudHeight;
		if (!(FApp.World != null))
		{
			hudWidth = Futile.Screen.Width;
			hudHeight = 100f;
		}
		else
		{
			hudWidth = Futile.Screen.Width;
			hudHeight = Futile.Screen.Height;
			hudHeight -= hudHeight * FApp.World.CameraFactor.y;
		}
		mHUDSize = new Vector2(hudWidth, hudHeight);
		mHUDContainer.DockInside(mUIContainer, InnerDock.Top, hudWidth, hudHeight, 0f, 0f);
		mWorldSize = new Vector2(Futile.Screen.Width, Futile.Screen.Height - mHUDSize.y);
		mWorldCollider.DockInside(mUIContainer, InnerDock.Bottom, mWorldSize.x, mWorldSize.y, 0f, 0f);
		mWorldDimmer.DockInside(mUIContainer, InnerDock.Bottom, mWorldSize.x, mWorldSize.y, 0f, 0f);
		float margin = mHUDSize.y * 8f / 100f;
		float buttonSize = mHUDSize.y * 96f / 100f;
		mMenuButton.DockInside(mHUDContainer, InnerDock.Right, buttonSize, buttonSize, 0f - margin, 0f);
		mMapButton.DockOutside(mMenuButton, OuterDock.Left, buttonSize, buttonSize, 0f - margin, 0f);
		mInventoryButton.DockOutside(mMapButton, OuterDock.Left, buttonSize, buttonSize, 0f - margin, 0f);
		float iconSize = 72f * mHUDSize.y / 100f;
		float labelWidth = 120f * mHUDSize.y / 100f;
		mCurrencySprite.DockInside(mHUDContainer, InnerDock.Left, iconSize, iconSize, margin, 0f);
		mCurrencyLabel.DockOutside(mCurrencySprite, OuterDock.Right, labelWidth, iconSize, margin, margin * -0.75f);
		mKeyCardSprite.DockOutside(mCurrencyLabel, OuterDock.Right, iconSize, iconSize, margin, margin * 0.75f);
		mKeyCardLabel.DockOutside(mKeyCardSprite, OuterDock.Right, labelWidth, iconSize, margin, margin * -0.75f);
		mHealthSprite.DockOutside(mKeyCardLabel, OuterDock.Right, buttonSize, buttonSize, margin * 4f, margin * 0.75f);
		mHealthLabel.DockInside(mHealthSprite, InnerDock.Center, buttonSize, buttonSize, 2f, -4f);
		mCharacterWeapon.DockOutside(mHealthSprite, OuterDock.Right, buttonSize, buttonSize, margin, 0f);
		mChangeCharacterButton.DockOutside(mCharacterWeapon, OuterDock.Right, buttonSize, buttonSize, margin, 0f);
		float cycleSize = buttonSize + -16f;
		mCycleCharacterButton.DockOutside(mChangeCharacterButton, OuterDock.Right, cycleSize, cycleSize, 24f, 0f);
		mCycleCharacterArrow.Rotation = 90f;
		mCycleCharacterArrow.DockOutside(mCycleCharacterButton, OuterDock.Left, cycleSize, cycleSize, 6f, 0f);
		mCharacterAmmo.DockInside(mCharacterWeapon, InnerDock.Center, buttonSize, buttonSize, 0f, 0f);
		mDialogueBGSize = new Vector2(Futile.Screen.Width + -100f, 400f);
		mDialogueBG.DockOutside(mHUDContainer, OuterDock.Bottom, mDialogueBGSize.x, mDialogueBGSize.y, 0f, -32f);
		LayoutDialogueBits();
		mDebugGameSpeedLabel.DockOutside(mHUDContainer, OuterDock.BottomAlignRight, 400f, 128f, 0f, -8f);
		mBossHealthBar.DockInside(mUIContainer, InnerDock.Top, 512f, 64f, 0f, -8f - mHUDSize.y);
		mBossName.DockInside(mBossHealthBar, InnerDock.Center, 448f, 64f, 0f, 0f);
		mXmasNode.DockOutside(mHUDContainer, OuterDock.BottomAlignRight, 256f, 64f, -8f, -8f);
		mAreaNameLabel.DockInside(mUIContainer, InnerDock.Bottom, 256f, 64f, 0f, 0f);
	}

	protected void LayoutDialogueBits()
	{
		FDialogueLine line = null;
		int speakerFlag = 0;
		bool hasSpeaker = false;
		if (mDialogueOnScreen && mDialogueIndex >= 0 && mDialogueIndex < mDialogueLines.Length)
		{
			line = mDialogueLines[mDialogueIndex];
			speakerFlag = line.Speaker.Length;
			if (mDialogueIndex != 0 && line.Speaker != mDialogueLines[mDialogueIndex - 1].Speaker)
			{
				mDialogueOnLeft = !mDialogueOnLeft;
			}
			hasSpeaker = speakerFlag >= 1;
		}
		Sides speakerJustifyX;
		if (!hasSpeaker)
		{
			mDialogueSpeakerImage.Unparent();
			mDialogueSpeakerLabel.Unparent();
			mDialogueLabel.DockInside(mDialogueBG, InnerDock.Bottom, mDialogueBGSize.x + -32f, mDialogueBGSize.y + -128f, 0f, 96f);
			speakerJustifyX = Sides.CenterX;
			mDialogueLabel.Justify(speakerJustifyX, Sides.CenterY);
		}
		else
		{
			mDialogueSpeakerImage.SetParent(mDialogueBG);
			mDialogueSpeakerLabel.SetParent(mDialogueBG);
			float imageSize = mDialogueBGSize.y * 0.5f;
			mDialogueLabel.Justify(FLoc.IsRightToLeft ? Sides.Right : Sides.Left, Sides.Top);
			if (!mDialogueOnLeft)
			{
				mDialogueSpeakerImage.DockInside(mDialogueBG, InnerDock.TopRight, imageSize, imageSize, -16f, -16f);
				mDialogueLabel.DockInside(mDialogueBG, InnerDock.BottomLeft, mDialogueBGSize.x - (imageSize + 64f), mDialogueBGSize.y + -192f, FLoc.IsRightToLeft ? 16f : 32f, 96f);
				mDialogueSpeakerLabel.DockInside(mDialogueBG, InnerDock.TopLeft, mDialogueBGSize.x + -32f - imageSize, 64f, 0f, -16f);
				speakerJustifyX = Sides.Right;
			}
			else
			{
				mDialogueSpeakerImage.DockInside(mDialogueBG, InnerDock.TopLeft, imageSize, imageSize, 16f, -16f);
				mDialogueLabel.DockInside(mDialogueBG, InnerDock.BottomRight, mDialogueBGSize.x - (imageSize + 64f), mDialogueBGSize.y + -192f, FLoc.IsRightToLeft ? -32f : -16f, 96f);
				mDialogueSpeakerLabel.DockInside(mDialogueBG, InnerDock.TopRight, mDialogueBGSize.x + -32f - imageSize, 64f, 0f, -16f);
				speakerJustifyX = Sides.Left;
			}
			mDialogueSpeakerLabel.Justify(speakerJustifyX, Sides.CenterY);
		}
		mDialogueDoneButton.DockInside(mDialogueBG, InnerDock.BottomRight, 300f, 96f, 0f, 0f);
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		if (FApp.PlayerStats != null)
		{
			FApp.PlayerStats.StatsUpdatedAction += UpdatePlayerInfo;
			UpdatePlayerInfo(FApp.PlayerStats.CurrentPlayerState);
		}
		mHealthLabel.MoveToTop();
		ShowBossHealth(false);
	}

	public void UpdatePlayerInfo(FPlayerState stats)
	{
		SyncHearts(stats.Health, stats.MaxHealth);
		SyncCurrency(stats.Coins);
		SyncKeyCards(stats.KeyCards);
		OutfitHandler.HandleOutfitAdding(stats.Collectibles);
		SyncAvailableCharacters();
		SyncCurrentCharacter(stats.CurrentCharacter);
		SyncXmasLights();
		SyncQuestMarkerReveal();
	}

	public void ShowBossHealth(bool show)
	{
		if (!show)
		{
			if (mBossHealthBar.HasParent)
			{
				mBossHealthBar.Unparent();
				mBossName.Unparent();
			}
		}
		else
		{
			if (mBossHealthBar.HasParent)
			{
				return;
			}
			mBossHealthBar.RemoveAllActions();
			mBossName.RemoveAllActions();
			mBossHealthBar.SetParent(mUIContainer);
			mBossName.SetParent(mUIContainer);
			mBossName.MoveToBottom();
			mBossHealthBar.MoveToBottom();
			mBossName.SetVisibleRecursive(false);
			mBossHealthBar.SetVisibleRecursive(false);
			FGameUtil.ExecuteNextFrame(delegate
			{
				mBossName.SetVisibleRecursive(true);
				mBossHealthBar.SetVisibleRecursive(true);
			});
			float barY = mBossHealthBar.y;
			mBossHealthBar.y = mBossHealthBar.y + 100f;
			mBossHealthBar.AddAction(new FEntityActionTweenPropertyToFloat(0.5f, "y", barY, GoEaseType.QuadOut));
			float nameY = mBossName.y;
			mBossName.y = mBossName.y + 100f;
			mBossName.AddAction(new FEntityActionTweenPropertyToFloat(0.5f, "y", nameY, GoEaseType.QuadOut));
		}
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		if (FApp.PlayerStats != null)
		{
			FApp.PlayerStats.StatsUpdatedAction -= UpdatePlayerInfo;
		}
		mLoopingSoundHelper.StopAllLoopingSounds();
	}

	public void ResetQuestMarkers()
	{
		mQuestMarkersSynced = false;
		mRevealedQuestMarkers.Clear();
		mMapButton.ClearFlash();
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
		if (mTouchDown && TouchHoldInterval < Time.time - mLastTouchUpdate)
		{
			if (FApp.World != null)
			{
				FApp.World.HandleWorldHold(new Vector3(mLastTouchPos.x, mLastTouchPos.y, 0f));
			}
			mIsHolding = true;
			mLastTouchUpdate = Time.time;
		}
	}

	public override bool HandleInput(float deltaTime)
	{
		bool escape = Input.GetKeyDown(KeyCode.Escape);
		// PC port: only open the pause menu when no other page (inventory, map, ...) is on top; PCInput handles Escape there.
		if (escape && FApp.HUD.TopPage == this)
		{
			ShowPauseMenu();
		}
		return escape;
	}

	// PC port (new): keyboard / gamepad entry points that trigger the same handlers as the HUD buttons.
	public bool PCAdvanceDialogue()
	{
		if (!mDialogueOnScreen)
		{
			return false;
		}
		AdvanceDialogue();
		return true;
	}

	public void PCOpenMap()
	{
		HandleMapButton(mMapButton, default(FTouch));
	}

	public void PCOpenInventory()
	{
		HandleInventoryButton(mInventoryButton, default(FTouch));
	}

	public void PCCycleCharacter()
	{
		HandlePlayerCycleButton(mCycleCharacterButton, default(FTouch));
	}

	public void PCSpecialAbility()
	{
		HandleCharacterWeaponButton(mCharacterWeapon, default(FTouch));
	}

	// index is the 0-based position among the unlocked player characters.
	public void PCSelectCharacter(int index)
	{
		if (!MenuButtonsEnabled() || !mCharacterSwitchingEnabled || FApp.World == null || FWorld.Player == null)
		{
			return;
		}
		FProtoUnit[] characters = FApp.ProtoUnitManager.GetProtoUnitsWithTag("PlayerCharacter");
		int n = 0;
		for (int i = 0; characters != null && i < characters.Length; i++)
		{
			if (!characters[i].CheckUnlockRequirements())
			{
				continue;
			}
			if (n++ == index)
			{
				if (characters[i].Name != FApp.PlayerStats.CurrentCharacter)
				{
					FApp.SoundManager.PlaySoundSet("UIButton");
					FApp.World.SetPlayerCharacter(characters[i], false);
				}
				return;
			}
		}
	}

	public void ShowPauseMenu()
	{
		if (!FApp.HUD.PagePause.OnStage)
		{
			FApp.SoundManager.PlaySoundSet("UI.Inventory.Open");
			FApp.World.Pause(true, true);
			FApp.HUD.PushPage(FApp.HUD.PagePause, true, false);
		}
	}

	public void RoomChanged()
	{
		string areaName = FApp.World.CurrentRoom.ProtoRoom.AreaName;
		if (areaName.Valid())
		{
			areaName = FLoc.GetString(areaName, true);
		}
		if (areaName != mAreaNameLabel.Text)
		{
			mAreaNameLabel.SetParent(mUIContainer);
			mAreaNameLabel.MoveToBottom();
			mAreaNameLabel.Text = areaName;
			mAreaNameLabel.RemoveAllActions();
			FEntityActionSequence sequence = new FEntityActionSequence();
			sequence.AddAction(new FEntityActionTweenPropertyToFloat(0.5f, "Alpha", 0f, 1f));
			sequence.AddAction(new FEntityActionDelay(3f));
			sequence.AddAction(new FEntityActionTweenPropertyToFloat(0.5f, "Alpha", 1f, 0f));
			sequence.AddAction(new FEntityActionCallMethod(mAreaNameLabel.Unparent));
			mAreaNameLabel.AddAction(sequence);
		}
	}

	protected void SyncTimeScaleDisplay()
	{
		if (Time.timeScale == 1f)
		{
			mDebugGameSpeedLabel.Unparent();
			return;
		}
		mDebugGameSpeedLabel.SetParent(mHUDContainer);
		mDebugGameSpeedLabel.Text = FLoc.Format("TimeScaleFormat", Time.timeScale);
	}

	protected void SyncHearts(int health, int maxHealth)
	{
		mHealthLabel.Text = health.ToString();
		string sprite;
		if (health < maxHealth)
		{
			sprite = ((health > 0) ? "HUDHeartHalf.png" : "HUDHeartEmpty.png");
		}
		else
		{
			sprite = "HUDHeartFull.png";
		}
		mHealthSprite.SetSprite(sprite);
	}

	protected void SyncCurrency(int currency)
	{
		mCurrencyLabel.SetNumber(currency, 1f, false, string.Empty);
	}

	protected void SyncKeyCards(int keyCards)
	{
		mKeyCardLabel.Text = keyCards.ToString();
	}

	protected void SyncCurrentCharacter(string character)
	{
		FProtoUnit unit = FApp.ProtoUnitManager.GetProtoUnit(character);
		if (unit == null)
		{
			return;
		}
		mCurrentCharacter = character;
		mChangeCharacterButton.SetForeground(character + "_Icon_Head.png");
		if (!FApp.PlayerStats.LastCharacter.Valid())
		{
			mCycleCharacterButton.SetForeground("BlackSquare.png");
			mCycleCharacterArrow.Unparent();
		}
		else
		{
			mCycleCharacterButton.SetForeground(FApp.PlayerStats.LastCharacter + "_Icon_Head.png");
			mCycleCharacterArrow.SetParent(mHUDContainer);
		}
		FProtoWeapon weapon = unit.GetProtoWeapon();
		if (weapon == null)
		{
			mCharacterWeapon.Unparent();
			mCharacterAmmo.Unparent();
		}
		else
		{
			mCharacterWeapon.SetParent(mHUDContainer);
			string icon = weapon.IconOverride;
			if (!icon.Valid())
			{
				icon = character + "_Icon_Weapon.png";
			}
			mCharacterWeapon.SetForeground(icon);
			if (unit.AlternateWeapons == null)
			{
				mCharacterAmmo.Unparent();
			}
			else
			{
				FProtoWeapon ammoWeapon = unit.AlternateWeapons.Select((string w) => FApp.ProtoWeaponManager.GetProtoWeapon(w)).FirstOrDefault((FProtoWeapon w) => w.HasAmmoResource);
				if (ammoWeapon == null || !ammoWeapon.HasAmmoResource)
				{
					mCharacterAmmo.Unparent();
				}
				else
				{
					mCharacterAmmo.SetParent(mHUDContainer);
					int ammo = FApp.PlayerStats.GetAmmoAmount(ammoWeapon.AmmoResource);
					mCharacterAmmo.Text = ammo.ToString();
				}
			}
		}
		if (character == "Dustin" || character == "Mike")
		{
			mCharacterWeapon.Enable(true, true);
		}
		else
		{
			mCharacterWeapon.Disable(false, true);
		}
		SyncSpecialWeaponButton();
	}

	protected void SyncAvailableCharacters()
	{
		if (!(FApp.HUD == null) && FApp.HUD.PageCharacterPicker != null)
		{
			FApp.HUD.PageCharacterPicker.InitCharacterList();
		}
	}

	protected void SyncXmasLights()
	{
		ShowXmasLights(FApp.PlayerStats.HasCollectible(699));
	}

	protected void SyncQuestMarkerReveal()
	{
		bool flash = false;
		foreach (int questId in FApp.PlayerStats.CurrentQuests)
		{
			FProtoQuest quest = FApp.ProtoQuestManager.GetProtoQuestByDBID(questId);
			if (quest == null || quest.Reminders == null)
			{
				continue;
			}
			foreach (QuestReminder reminder in quest.Reminders)
			{
				if (!reminder.MapMarker.Valid() || !reminder.MapMarkerRevealed)
				{
					continue;
				}
				FMapMarker marker = FApp.MapMarkerManager.GetMapMarker(reminder.MapMarker);
				if (marker != null && marker.FlashMapButton)
				{
					FProtoWorld protoWorld = FApp.WorldManager.GetProtoWorld(marker.Map);
					if (protoWorld != null && !(protoWorld.MapPack != FApp.World.ProtoWorld.MapPack) && !mRevealedQuestMarkers.Contains(questId))
					{
						mRevealedQuestMarkers.Add(questId);
						flash = mQuestMarkersSynced | flash;
					}
				}
			}
		}
		if (flash)
		{
			mMapButton.Flash();
		}
		mQuestMarkersSynced = true;
	}

	public void SyncSpecialWeaponButton()
	{
		string background = string.Empty;
		string character = FApp.PlayerStats.CurrentCharacter;
		if (character == "Dustin")
		{
			background = ((FWorld.Player.ProtoUnit.Weapon == "PuddinPop") ? "Button003.png" : "Button002.png");
		}
		else if (character == "Mike")
		{
			background = ((FWorld.Player.ProtoUnit.Name == "MikeOnABike") ? "Button003.png" : "Button002.png");
		}
		mCharacterWeapon.SetBackground(background);
	}

	public void ResetSpecialWeaponButton()
	{
		string character = FApp.PlayerStats.CurrentCharacter;
		if (character == "Dustin")
		{
			if (FWorld.Player.ProtoUnit.Weapon == "PuddinPop")
			{
				DoSpecialWeaponButton(true);
			}
		}
		else if (character == "Mike" && (FWorld.Player.ProtoUnit.Name == "Mike" || FWorld.Player.ProtoUnit.Name == "MikeOnABike"))
		{
			SyncSpecialWeaponButton();
		}
	}

	protected void HandlePlayerCycleButton(FUIButton button, FTouch touch)
	{
		if (MenuButtonsEnabled() && mCharacterSwitchingEnabled)
		{
			mCharacterWeapon.ClearFlash();
			FApp.SoundManager.PlaySoundSet("UIButton");
			if (FApp.World != null)
			{
				FApp.World.CyclePlayerCharacter();
			}
		}
	}

	protected void HandleMenuButton(FUIButton button, FTouch touch)
	{
		if (MenuButtonsEnabled())
		{
			button.ClearFlash();
			ShowPauseMenu();
		}
	}

	public bool MenuButtonsEnabled()
	{
		if (FApp.World != null && FApp.World.InputEnabled)
		{
			return !mDialogueOnScreen;
		}
		return false;
	}

	public void HidePauseMenu()
	{
		if (FApp.HUD.PagePause.OnStage)
		{
			FApp.SoundManager.PlaySoundSet("UI.Inventory.Close");
			FApp.World.UnPause();
			FApp.HUD.PagePause.TransitionOut();
		}
	}

	public void EnableCharacterSwitching(bool enable)
	{
		mCharacterSwitchingEnabled = enable;
		mCycleCharacterButton.Enable(enable, true);
		mChangeCharacterButton.Enable(enable, true);
		mCycleCharacterArrow.Color = (enable ? new Color(1f, 1f, 1f, 1f) : Color.grey);
	}

	protected void HandleChangeCharacterButton(FUIButton button, FTouch touch)
	{
		if (MenuButtonsEnabled() && mCharacterSwitchingEnabled)
		{
			button.ClearFlash();
			mCharacterWeapon.ClearFlash();
			FApp.HUD.PushPage(FApp.HUD.PageCharacterPicker, true, false);
		}
	}

	protected void HandleCharacterWeaponButton(FUIButton button, FTouch touch)
	{
		button.ClearFlash();
		DoSpecialWeaponButton();
	}

	protected void DoSpecialWeaponButton(bool force = false)
	{
		if (!force && !(MenuButtonsEnabled() && mCharacterSwitchingEnabled))
		{
			return;
		}
		if (FApp.PlayerStats == null)
		{
			return;
		}
		string character = FApp.PlayerStats.CurrentCharacter;
		if (character == "Dustin")
		{
			if (FApp.PlayerStats.Pudding < 1)
			{
				FWorld.Player.World.SpawnFlyOffText(FLoc.GetString("NoAmmo"), FWorld.Player.Position, new Vector2(32f, 32f));
			}
			else
			{
				FProtoUnit protoUnit = FWorld.Player.ProtoUnit;
				protoUnit.SetWeapon((protoUnit.Weapon == "PuddinPop") ? "BasicMelee" : "PuddinPop");
				FWorld.Player.SyncWeaponWithProto();
			}
			SyncCurrentCharacter(FApp.PlayerStats.CurrentCharacter);
		}
		else if (character == "Mike")
		{
			string newProto = ((FWorld.Player.ProtoUnit.Name == "Mike") ? "MikeOnABike" : "Mike");
			FProtoUnit proto = FApp.ProtoUnitManager.GetProtoUnit(newProto);
			if (proto != null)
			{
				FWorld.Player.ChangeProto(proto);
			}
		}
		SyncSpecialWeaponButton();
	}

	protected void HandleInventoryButton(FUIButton button, FTouch touch)
	{
		button.ClearFlash();
		if (MenuButtonsEnabled())
		{
			FApp.HUD.PushPage(FApp.HUD.PageInventory, true, false);
		}
	}

	protected void HandleMapButton(FUIButton button, FTouch touch)
	{
		button.ClearFlash();
		if (MenuButtonsEnabled())
		{
			FApp.SoundManager.PlaySoundSet("UI.Inventory.Open");
			FApp.HUD.PushPage(FApp.HUD.PageWorldMap, true, false);
		}
	}

	public void FlashButton(string name)
	{
		FUIButton button;
		switch (name)
		{
		case "CharacterButton":
			button = mChangeCharacterButton;
			break;
		case "WeaponButton":
			button = mCharacterWeapon;
			break;
		case "MapButton":
			button = mMapButton;
			break;
		case "OptionsButton":
			button = mMenuButton;
			break;
		case "InventoryButton":
			button = mInventoryButton;
			break;
		default:
			return;
		}
		button.Flash();
	}

	public void HandlePress(FUICollider collider, FTouch touch)
	{
		if (mDialogueOnScreen)
		{
			AdvanceDialogue();
			return;
		}
		mTouchDown = true;
		mLastTouchPos = touch.RawPosition;
		mLastTouchUpdate = Time.time;
		mIsHolding = false;
		if (FApp.World != null)
		{
			FApp.World.HandleWorldTap(new Vector3(mLastTouchPos.x, mLastTouchPos.y, 0f), false);
		}
	}

	public void HandleAdditionalPress(FUICollider collider, FTouch touch)
	{
	}

	public void HandlePressMove(FUICollider collider, FTouch touch)
	{
		mLastTouchPos = touch.RawPosition;
	}

	public void HandlePressCancel(FUICollider collider, FTouch touch)
	{
		if (FApp.World != null)
		{
			FApp.World.HandleWorldTap(new Vector3(mLastTouchPos.x, mLastTouchPos.y, 0f), false);
		}
		mTouchDown = false;
		mIsHolding = false;
	}

	public void HandlePressRelease(FUICollider collider, FTouch touch)
	{
		if (mIsHolding && FApp.World != null)
		{
			FApp.World.HandleWorldTap(new Vector3(mLastTouchPos.x, mLastTouchPos.y, 0f), true);
		}
		mIsHolding = false;
		mTouchDown = false;
	}

	protected void AddTouch(FTouch touch)
	{
		if (!mTouches.ContainsKey(touch.FingerId))
		{
			mTouches[touch.FingerId] = touch;
		}
		if (mTouches.Count > 1 && !mPinchZoomMode)
		{
			BeginPinchZoomMode();
		}
	}

	protected void RemoveTouch(FTouch touch)
	{
		if (mTouches.ContainsKey(touch.FingerId))
		{
			mTouches.Remove(touch.FingerId);
		}
		if (mPinchZoomMode)
		{
			ClearPinchZoomMode();
		}
	}

	protected void BeginPinchZoomMode()
	{
		if (mTouches.Count >= 2)
		{
			Vector2 first = mTouches[0].RawPosition;
			Vector2 second = mTouches[1].RawPosition;
			mInitialPinchDist = Mathf.Sqrt((first.x - second.x) * (first.x - second.x) + (first.y - second.y) * (first.y - second.y));
			UpdatePinchDist();
			mPinchZoomMode = true;
		}
	}

	protected void ClearPinchZoomMode()
	{
		mTouches.Clear();
		mPinchZoomMode = false;
	}

	protected void UpdatePinchDist()
	{
		if (mTouches.Count >= 2)
		{
			Vector2 first = mTouches[0].RawPosition;
			Vector2 second = mTouches[1].RawPosition;
			float dx = first.x - second.x;
			float dy = first.y - second.y;
			mCurrPinchDist = Mathf.Sqrt(dx * dx + dy * dy);
		}
	}

	public void DimWorld(float alpha, float duration = 0.5f)
	{
		mWorldDimmer.RemoveAllActions();
		if (alpha <= Mathf.Epsilon)
		{
			if (!mWorldDimmer.OnStage)
			{
				return;
			}
			if (duration < Mathf.Epsilon)
			{
				mWorldDimmer.Unparent();
				return;
			}
			FEntityAction[] actions = new FEntityAction[2]
			{
				new FEntityActionTweenPropertyToFloat(duration, "Alpha", 0f),
				new FEntityActionCallMethod(delegate
				{
					mWorldDimmer.Unparent();
				})
			};
			mWorldDimmer.AddAction(new FEntityActionSequence(actions));
			return;
		}
		if (!mWorldDimmer.OnStage)
		{
			mWorldDimmer.SetParent(mUIContainer);
		}
		if (Mathf.Epsilon < mWorldDimmer.Alpha)
		{
			mWorldDimmer.Alpha = 0f;
		}
		if (Mathf.Epsilon <= duration)
		{
			mWorldDimmer.AddAction(new FEntityActionTweenPropertyToFloat(duration, "Alpha", alpha));
		}
		else
		{
			mWorldDimmer.Alpha = alpha;
		}
	}

	public void SetLowHP(bool flashHearts, bool playSound)
	{
		mHealthLabel.RemoveAllActions();
		if (!flashHearts)
		{
			mHealthLabel.MoveToTop();
		}
		else
		{
			FEntityActionSequence sequence = new FEntityActionSequence();
			sequence.AddAction(new FEntityActionCallMethod(delegate
			{
				mHealthLabel.MoveToBottom();
			}));
			sequence.AddAction(new FEntityActionDelay(0.2f));
			sequence.AddAction(new FEntityActionCallMethod(delegate
			{
				mHealthLabel.MoveToTop();
			}));
			sequence.AddAction(new FEntityActionDelay(0.2f));
			mHealthLabel.AddAction(new FEntityActionRepeat(-1, sequence));
		}
		if (!playSound)
		{
			mLoopingSoundHelper.StopLoopingSound("LowHealthLoop");
		}
		else if (!mLoopingSoundHelper.IsPlaying("LowHealthLoop"))
		{
			mLoopingSoundHelper.PlayLoopingSound("LowHealthLoop");
		}
	}

	public void SetBossHealth(int current, int max)
	{
		mBossHealthBar.SetPercentage((float)current / (float)max, false, false);
	}

	public void FlashBossHealth()
	{
		mBossHealthBar.RemoveAllActions();
		mBossHealthBar.FlashForeground(0.2f);
	}

	public void SetBossName(string name)
	{
		mBossName.Text = name;
	}

	public void ShowXmasLights(bool show)
	{
		if (!show)
		{
			if (mXmasNode.HasParent)
			{
				mXmasNode.Unparent();
			}
		}
		else if (!mXmasNode.HasParent)
		{
			mXmasNode.SetParent(mHUDContainer);
		}
	}

	public void BounceHealth()
	{
		mHealthSprite.RemoveAllActions();
		mHealthSprite.Scale = 1f;
		FApp.HUD.CreateAndAddPunchScaleAction(mHealthSprite, 0.25f, 1f, 1);
	}

	public void DisplayDialogue(FDialogueLine[] lines, FUnit unit, DialoguePlacement placement)
	{
		mDialogueTarget = unit;
		mDialogueBG.SetParent(mHUDContainer);
		mDialogueBG.RemoveAllActions();
		if (placement == DialoguePlacement.BOTTOM)
		{
			if (!Futile.Screen.IsWidescreen)
			{
				mDialogueBG.DockOutside(mHUDContainer, OuterDock.Bottom, mDialogueBGSize.x, mDialogueBGSize.y, 0f, mDialogueBGSize.y - (Futile.Screen.Size.y - mHUDSize.y) + 32f);
			}
			else
			{
				mDialogueBG.DockOutside(mHUDContainer, OuterDock.LeftAlignBottom, mDialogueBGSize.x, mDialogueBGSize.y, -32f, 32f);
			}
		}
		else if (placement == DialoguePlacement.TOP)
		{
			if (!Futile.Screen.IsWidescreen)
			{
				mDialogueBG.DockOutside(mHUDContainer, OuterDock.Bottom, mDialogueBGSize.x, mDialogueBGSize.y, 0f, -32f);
			}
			else
			{
				mDialogueBG.DockOutside(mHUDContainer, OuterDock.LeftAlignTop, mDialogueBGSize.x, mDialogueBGSize.y, -32f, -32f);
			}
		}
		mDialogueBG.ScaleX = 0f;
		mDialogueBG.AddAction(new FEntityActionSpawn(new FEntityActionTweenPropertyToFloat(0.25f, "ScaleX", 0f, 1f)));
		mDialogueOnScreen = true;
		mDialogueLines = lines;
		mDialogueIndex = 0;
		mDialogueOnLeft = true;
		StartDialogueLine();
		FApp.World.Pause(true, false);
		FApp.SoundManager.PlaySoundSet("UI.Dialogue.Open");
	}

	protected void StartDialogueLine()
	{
		mDialogueDoneButton.Unparent();
		FDialogueLine line = mDialogueLines[mDialogueIndex];
		mDialogueSpeakerLabel.Text = line.LocSpeaker;
		string speaker;
		if (!(mDialogueSpeakerLabel.Text == FLoc.GetString("Characters.Current")))
		{
			speaker = mDialogueLines[mDialogueIndex].SpeakerName;
		}
		else
		{
			speaker = FApp.PlayerStats.CurrentCharacter;
		}
		string headSprite = speaker + "_Icon_Head.png";
		if (Futile.AtlasManager.GetElementWithName(headSprite, true) == null)
		{
			headSprite = "Generic_Icon_Head.png";
		}
		mDialogueSpeakerImage.SetSprite(headSprite);
		string locLine = mDialogueLines[mDialogueIndex].LocLine;
		FNode.Cleanup(mDialogueLabel);
		mDialogueLabel = new FLabel(Name + ".DialogLabel", "DefaultStyle", string.Empty, 32f, true, true, false);
		mDialogueLabel.Justify(Sides.Left, Sides.Top);
		mDialogueLabel.Wrap = true;
		mDialogueLabel.SetParent(mDialogueBG);
		LayoutPage();
		FDialogueLine current = mDialogueLines[mDialogueIndex];
		if (current.RunTagsFirst)
		{
			current.EvaluateTags(mDialogueTarget);
		}
		mDialogueTypeRoutine = FGameUtil.StartCoroutine(TypeDialogue());
		if (!locLine.Valid())
		{
			AdvanceDialogue();
		}
	}

	protected IEnumerator TypeDialogue()
	{
		yield return 0;
		mDialogueLabel.Text = mDialogueLines[mDialogueIndex].LocLine;
		mDialogueLabel.UpdateLocalPosition();
		mDialogueLabel.UpdateJustification();
		mDialogueLabel.SelfResizeExtents();
		mDialogueLabel.CreateTextQuads();
		if (mDialogueLabel.LetterQuadLines == null)
		{
			FinalizeDialogue();
			yield break;
		}
		mLoopingSoundHelper.PlayLoopingSound("UITextTyping");
		float charInterval = 0.02f;
		mDialogueLabel.MaxQuads = 0;
		yield return new WaitForSeconds(0.2f);
		int i = 0;
		while (true)
		{
			if (mDialogueLabel.TotalQuads <= i)
			{
				AdvanceDialogue();
				yield break;
			}
			float time = 0f;
			while (time < charInterval)
			{
				time += Time.deltaTime;
				yield return 0;
			}
			mDialogueLabel.MaxQuads = mDialogueLabel.MaxQuads + 1;
			i++;
		}
	}

	protected void DialogueDoneButton(FUIButton button, FTouch touch)
	{
		mLoopingSoundHelper.StopLoopingSound("UITextTyping");
		mDialogueIndex++;
		if (mDialogueIndex < mDialogueLines.Length)
		{
			StartDialogueLine();
		}
		else
		{
			HideDialogue();
		}
	}

	protected void AdvanceDialogue()
	{
		if (mDialogueTypeRoutine != null)
		{
			mLoopingSoundHelper.StopLoopingSound("UITextTyping");
			mDialogueBG.RemoveAllActions();
			mDialogueBG.ScaleX = 1f;
			FGameUtil.StopCoroutine(mDialogueTypeRoutine);
			mDialogueTypeRoutine = null;
			FinalizeDialogue();
		}
		else
		{
			DialogueDoneButton(null, default(FTouch));
		}
	}

	protected void FinalizeDialogue()
	{
		FDialogueLine line = mDialogueLines[mDialogueIndex];
		mDialogueLabel.Text = line.LocLine;
		mDialogueLabel.ClearMaxQuads();
		mDialogueDoneButton.SetParent(mDialogueBG);
		FDialogueLine current = mDialogueLines[mDialogueIndex];
		if (!current.RunTagsFirst)
		{
			current.EvaluateTags(mDialogueTarget);
		}
		string key = ((mDialogueIndex < mDialogueLines.Length - 1) ? "DialogNext" : "DialogDone");
		mDialogueDoneButton.SetText(FLoc.GetString(key));
	}

	protected void HideDialogue()
	{
		mLoopingSoundHelper.StopLoopingSound("UITextTyping");
		FEntityActionSpawn spawn = new FEntityActionSpawn(new FEntityActionTweenPropertyToFloat(0.25f, "ScaleX", 1f, 0f));
		FEntityAction[] actions = new FEntityAction[2]
		{
			spawn,
			new FEntityActionCallMethod(mDialogueBG.Unparent)
		};
		mDialogueBG.AddAction(new FEntityActionSequence(actions));
		mDialogueDoneButton.Unparent();
		mDialogueOnScreen = false;
		if (FApp.World.InputEnabled)
		{
			FApp.World.UnPause();
		}
		FApp.SoundManager.PlaySoundSet("UI.Dialogue.Close");
		if (m_mDialogueCompleteAction != null)
		{
			m_mDialogueCompleteAction();
			m_mDialogueCompleteAction = null;
		}
	}
}
