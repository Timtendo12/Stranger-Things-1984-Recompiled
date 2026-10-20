using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageDungeonComplete @ 0x17f373c..0x17f5d54, Ghidra vaddr).
public class FPageDungeonComplete : FUIPage
{
	protected const bool cShowShareButtonsInFinalBuild = true;

	protected FUICollider mCollider;

	protected FNode mDialog;

	protected FUIButton mCloseButton;

	protected FSprite mHUDCover;

	protected Vector2 mLastTouchPos = Vector2.zero;

	protected FLabelDropshadow mDungeonLabel;

	protected FLabelDropshadow mPercentCompleteLabel;

	protected FLabelDropshadow mTimeSpentLabel;

	protected FUIButton mShareButton;

	protected FSprite mHealthSprite;

	protected FLabelDropshadow mHealthLabel;

	protected FSprite mCurrencySprite;

	protected FUIPinballText mCurrencyLabel;

	protected FSprite mKeyCardSprite;

	protected FLabelDropshadow mKeyCardLabel;

	public string OverrideDungeonName;

	public FPageDungeonComplete(string name)
		: base(name)
	{
		base.Modal = true;
		base.DarkenLower = true;
		base.TransitionStyle = TransitionStyles.Fade;
		mCollider = new FUICollider(Name + ".BGCollider", false);
		mCollider.SetParent(this);
		mDialog = new FNode(Name + ".Dialog");
		mDialog.SetParent(this);
		mHUDCover = new FSprite(Name + ".HUDCover", "BlackSquare.png");
		mHUDCover.SetParent(mDialog);
		mCloseButton = new FUIButton(Name + ".CloseButton");
		mCloseButton.SetBackground("ContinueButtonBG.png");
		mCloseButton.SetForegroundColor(new Color(1f, 1f, 1f, 1f));
		mCloseButton.SetText(FLoc.GetString("ContinueButton"), "DefaultStyle", 24f, 0f);
		mCloseButton.AddHandleTap(HandleCancelButton);
		mCloseButton.SetParent(mDialog);
		mDungeonLabel = new FLabelDropshadow(Name + ".DungeonNameLabel", "DefaultStyle", "0", 48f, false, true, false);
		mDungeonLabel.Wrap(true);
		mDungeonLabel.SetTextColor(FUI.Colors.Red, FUI.Colors.Black);
		mDungeonLabel.SetParent(mDialog);
		mPercentCompleteLabel = new FLabelDropshadow(Name + ".PercentCompleteLabel", "DefaultStyle", "0", 24f, false, true, false);
		mPercentCompleteLabel.Wrap(true);
		mPercentCompleteLabel.SetTextColor(FUI.Colors.White, FUI.Colors.Black);
		mPercentCompleteLabel.SetParent(mHUDCover);
		mTimeSpentLabel = new FLabelDropshadow(Name + ".TimeSpentLabel", "DefaultStyle", "0", 24f, false, true, false);
		mTimeSpentLabel.Wrap(true);
		mTimeSpentLabel.SetTextColor(FUI.Colors.White, FUI.Colors.Black);
		mTimeSpentLabel.SetParent(mHUDCover);
		mShareButton = new FUIButton(Name + ".FacebookButton");
		mShareButton.SetBackground("NativeShareButton.png");
		mShareButton.AddHandleRelease(HandleShareButton);
		mHealthSprite = new FSprite(Name + ".HealthSprite", "HUDHeartFull.png");
		mHealthSprite.SetParent(mHUDCover);
		mHealthLabel = new FLabelDropshadow(Name + ".HealthLabel", "DefaultStyle", "0", 32f, false, true, false);
		mHealthLabel.Justify(FNode.Sides.CenterX, FNode.Sides.CenterY);
		mHealthLabel.SetParent(mHUDCover);
		mCurrencySprite = new FSprite(Name + ".CurrencySprite", "HUDCoin.png");
		mCurrencySprite.SetParent(mHUDCover);
		mKeyCardSprite = new FSprite(Name + ".KeyCardSprite", "HUDKeyCard.png");
		mKeyCardSprite.SetParent(mHUDCover);
		mCurrencyLabel = new FUIPinballText(Name + ".CurrencyLabel", "DefaultStyle", "0", 32f, false);
		mCurrencyLabel.Justify(FNode.Sides.Left, FNode.Sides.CenterY);
		mCurrencyLabel.SetParent(mHUDCover);
		mKeyCardLabel = new FLabelDropshadow(Name + ".KeyCardLabel", "DefaultStyle", "0", 32f, false, true, false);
		mKeyCardLabel.Justify(FNode.Sides.Left, FNode.Sides.CenterY);
		mKeyCardLabel.SetParent(mHUDCover);
		mShareButton.SetParent(mDialog);
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		mCollider.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		mDialog.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		Vector2 hud = FApp.HUD.PageLegendWorld.HUDSize;
		float screenWidth = Futile.Screen.Width;
		float dungeonLabelY;
		if (!Futile.Screen.IsWidescreen)
		{
			mHUDCover.DockInside(mDialog, InnerDock.Top, hud.x, hud.y, 0f, 0f);
			float spriteSize = 72f * hud.y / 100f;
			float labelWidth = 120f * hud.y / 100f;
			float margin = hud.y * 8f / 100f;
			mCurrencySprite.DockInside(mHUDCover, InnerDock.Left, spriteSize, spriteSize, margin, 0f);
			mCurrencyLabel.DockOutside(mCurrencySprite, OuterDock.Right, labelWidth, spriteSize, margin, margin * -0.75f);
			mKeyCardSprite.DockOutside(mCurrencyLabel, OuterDock.Right, spriteSize, spriteSize, margin, margin * 0.75f);
			mKeyCardLabel.DockOutside(mKeyCardSprite, OuterDock.Right, labelWidth, spriteSize, margin, margin * -0.75f);
			mHealthSprite.DockOutside(mKeyCardLabel, OuterDock.Right, 96f, 96f, margin * 4f, margin * 0.75f);
			mHealthLabel.DockInside(mHealthSprite, InnerDock.Center, 96f, 96f, 2f, -4f);
			mCloseButton.DockInside(mDialog, InnerDock.Bottom, 288f, 96f, 0f, 64f);
			mTimeSpentLabel.DockOutside(mHealthSprite, OuterDock.Right, 256f, 96f, margin + margin, 0f);
			mPercentCompleteLabel.DockOutside(mTimeSpentLabel, OuterDock.Right, 256f, 96f, margin + margin, 0f);
			dungeonLabelY = -32f - hud.y;
		}
		else
		{
			mHUDCover.DockInside(mDialog, InnerDock.Right, 256f, hud.y, 0f, 0f);
			mCurrencySprite.DockInside(mHUDCover, InnerDock.TopLeft, 64f, 64f, 8f, -8f);
			mCurrencyLabel.DockOutside(mCurrencySprite, OuterDock.Right, 168f, 64f, 8f, -6f);
			mKeyCardSprite.DockOutside(mCurrencySprite, OuterDock.BottomAlignLeft, 64f, 64f, 0f, -8f);
			mKeyCardLabel.DockOutside(mKeyCardSprite, OuterDock.Right, 168f, 64f, 8f, -6f);
			mHealthSprite.DockOutside(mKeyCardLabel, OuterDock.Bottom, 96f, 96f, -48f, -4f);
			mHealthLabel.DockInside(mHealthSprite, InnerDock.Center, 96f, 96f, 0f, -4f);
			mCloseButton.DockInside(mDialog, InnerDock.Bottom, 288f, 96f, -128f, 64f);
			mTimeSpentLabel.DockOutside(mKeyCardSprite, OuterDock.BottomAlignLeft, 256f, 96f, -16f, -132f);
			mPercentCompleteLabel.DockOutside(mTimeSpentLabel, OuterDock.BottomAlignLeft, 256f, 96f, 2f, -4f);
			screenWidth -= 256f;
			dungeonLabelY = -32f;
		}
		mDungeonLabel.DockInside(mDialog, InnerDock.TopLeft, screenWidth, 128f, 0f, dungeonLabelY);
		mShareButton.DockOutside(mCloseButton, OuterDock.Left, 96f, 96f, -16f, 0f);
		MarkDirtyRecursive();
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		string dungeonName = OverrideDungeonName.Valid() ? OverrideDungeonName : FLoc.GetString("Dungeon_" + FApp.World.ProtoWorld.Name, false);
		if (!dungeonName.Valid())
		{
			Close();
			return;
		}
		OverrideDungeonName = null;
		TransitionIn(mDialog);
		FApp.World.Pause(true, true);
		SyncUIBits(dungeonName);
		mPercentCompleteLabel.SetVisibleRecursive(false);
		mTimeSpentLabel.SetVisibleRecursive(false);
		mDungeonLabel.SetVisibleRecursive(false);
		mShareButton.SetVisibleRecursive(false);
		mPercentCompleteLabel.StopAllActions();
		mTimeSpentLabel.StopAllActions();
		mDungeonLabel.StopAllActions();
		mShareButton.StopAllActions();
		mDungeonLabel.AddAction(CreateRevealSequence(0.25f, mDungeonLabel));
		mTimeSpentLabel.AddAction(CreateRevealSequence(0.75f, mTimeSpentLabel));
		mPercentCompleteLabel.AddAction(CreateRevealSequence(1.25f, mPercentCompleteLabel));
		mShareButton.Enable(true);
		mShareButton.AddAction(CreateRevealSequence(1.5f, mShareButton));
		FSocialManager.ShareCompleteAction += ShareComplete;
	}

	// Port helper replacing the <HandleAddedToStage>b__19_0..7 lambda pairs: delay, show, scale 5 -> 1 (CircIn), punch.
	private FEntityActionSequence CreateRevealSequence(float delay, FNode node)
	{
		FEntityActionSequence sequence = new FEntityActionSequence();
		sequence.AddAction(new FEntityActionDelay(delay));
		sequence.AddAction(new FEntityActionCallMethod(delegate
		{
			node.SetVisibleRecursive(true);
		}));
		sequence.AddAction(new FEntityActionTweenPropertyToFloat(0.25f, "Scale", 5f, 1f, GoEaseType.CircIn));
		sequence.AddAction(new FEntityActionCallMethod(delegate
		{
			FApp.HUD.CreateAndAddPunchPositionAction(node, 10f, 0.3f, 3);
		}));
		return sequence;
	}

	protected void Close()
	{
		FApp.HUD.PopPage(this);
		FApp.SoundManager.PlaySoundSet("UI.Inventory.Close");
	}

	protected void SyncUIBits(string dungeonName)
	{
		if (FApp.PlayerStats != null)
		{
			SyncHearts(FApp.PlayerStats.Health, FApp.PlayerStats.MaxHealth);
			SyncCurrency(FApp.PlayerStats.Coins);
			SyncKeyCards(FApp.PlayerStats.KeyCards);
			mPercentCompleteLabel.Text = FLoc.Format("PercentCompleteFormat", FApp.PlayerStats.GetPercentComplete());
			mTimeSpentLabel.Text = FLoc.Format("TimeSpentFormat", FLoc.GetFormattedTime(FApp.PlayerStats.TotalGameTime));
			mDungeonLabel.Text = dungeonName;
		}
	}

	protected void SyncHearts(int health, int maxHealth)
	{
		mHealthLabel.Text = health.ToString();
		if (health < maxHealth)
		{
			mHealthSprite.SetSprite((health > 0) ? "HUDHeartHalf.png" : "HUDHeartEmpty.png");
		}
		else
		{
			mHealthSprite.SetSprite("HUDHeartFull.png");
		}
	}

	protected void SyncCurrency(int currency)
	{
		mCurrencyLabel.SetNumber(currency, 1f, false, string.Empty);
	}

	protected void SyncKeyCards(int keyCards)
	{
		mKeyCardLabel.Text = keyCards.ToString();
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		TransitionOut(mDialog);
		FApp.World.UnPause();
		FSocialManager.ShareCompleteAction -= ShareComplete;
	}

	protected void HandleCancelButton(FUIButton button, FTouch touch)
	{
		FApp.HUD.PopPage(this);
		FApp.SoundManager.PlaySoundSet("UI.Inventory.Close");
	}

	protected void HandleShareButton(FUIButton button, FTouch touch)
	{
		button.Enable(false);
		button.TurnOnSpinner();
		FSocialManager.ShareToOS();
		FAnalytics.SendCustomDesignEvent("Social_Share");
	}

	public void ShareComplete(bool success)
	{
		mShareButton.TurnOffSpinner();
		mShareButton.Enable(true);
	}
}
