using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FGameModeNode @ 0x17f7188..0x17f7f90, Ghidra vaddr).
public class FGameModeNode : FUIScrollListItemButton
{
	protected FNodeStack mCharactersUnlocked;

	protected FLabel mTitleLabel;

	protected FLabel mOtherInfoLabel;

	public FGameModeNode(string name, string titleText)
		: base(name)
	{
		mCharactersUnlocked = new FNodeStack(Name + ".CharactersUnlocked", FNodeStack.Modes.CenteredX, -4f);
		mCharactersUnlocked.SetParent(this);
		mTitleLabel = new FLabel(Name + ".Title", "DefaultStyle", titleText, 64f);
		mTitleLabel.Justify(FNode.Sides.CenterX, FNode.Sides.Top);
		mTitleLabel.SetParent(this);
		mTitleLabel.Wrap = true;
		mOtherInfoLabel = new FLabel(Name + ".OtherInfo", "DefaultStyle", string.Empty, 24f);
		mOtherInfoLabel.Justify(FNode.Sides.CenterX, FNode.Sides.Bottom);
		mOtherInfoLabel.SetParent(this);
		mOtherInfoLabel.Wrap = true;
	}

	public override void SelfResizeExtents()
	{
		base.SelfResizeExtents();
		float width = base.ItemSize.x - 32f;
		mTitleLabel.DockInside(this, InnerDock.Top, width, 64f, 0f, -16f);
		mCharactersUnlocked.DockOutside(mTitleLabel, OuterDock.Bottom, width, 96f);
	}

	public void SetSaveSlotInfo(FPlayerState currentSave, FPlayerStats.SaveTypes type)
	{
		mCharactersUnlocked.ClearStack(cleanup: true);
		if (currentSave == null)
		{
			string text = FLoc.GetString((type != FPlayerStats.SaveTypes.NORMAL) ? "SelectGameMode.Classic.Details" : "SelectGameMode.Normal.Details");
			mOtherInfoLabel.Text = text;
			mOtherInfoLabel.DockInside(this, InnerDock.Bottom, base.ItemSize.x - 32f, base.ItemSize.y - 64f - 32f, 0f, 16f);
			return;
		}
		mOtherInfoLabel.DockInside(this, InnerDock.Bottom, base.ItemSize.x - 32f, base.ItemSize.y - 96f - 64f - 32f, 0f, 16f);
		FProtoUnit[] protoUnitsWithTag = FApp.ProtoUnitManager.GetProtoUnitsWithTag("PlayerCharacter");
		foreach (FProtoUnit fProtoUnit in protoUnitsWithTag)
		{
			if (fProtoUnit.CheckUnlockRequirements())
			{
				FSprite node = new FSprite(Name + ".CharSprite." + fProtoUnit.Name, fProtoUnit.Name + "_Icon_Full.png");
				mCharactersUnlocked.StackChild(node, new Vector2(64f, 64f));
			}
		}
		string text2 = FLoc.Format("TimeSpentFormatSingleLine", FLoc.GetFormattedTime(currentSave.TotalGameTime));
		// separator = literal slot 0x32085a8, the same one FUtil.BalancedWordWrap inserts as its line break: "\n"
		text2 = text2 + "\n" + currentSave.GetPercentCompleteString(singleLine: true);
		mOtherInfoLabel.Text = text2;
		mTitleLabel.MoveToTop();
		mCharactersUnlocked.MoveToTop();
		mOtherInfoLabel.MoveToTop();
	}
}
