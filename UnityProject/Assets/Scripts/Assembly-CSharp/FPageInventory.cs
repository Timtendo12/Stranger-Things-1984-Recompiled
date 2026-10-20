using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageInventory @ 0x17fa528..0x17ff2dc, Ghidra vaddr).
public class FPageInventory : FPageLegend
{
	public enum Modes
	{
		None = 0,
		Inventory = 1,
		Hearts = 2,
		Gnomes = 3,
		Waffles = 4
	}

	protected Modes mMode = Modes.Inventory;

	protected FNode mNodeInventory;

	protected FPageInventoryContainer mNodeInventoryTools;

	protected FPageInventoryContainer mNodeInventoryAmmo;

	protected FPageInventoryContainer mNodeInventoryUpgrades;

	protected FPageInventoryContainer mNodeInventoryQuestItems;

	protected FLabel mNodeInventoryDetails;

	protected string mSelectedInventoryItemDetails = string.Empty;

	protected FNode mNodeHearts;

	protected FLabel mNodeHeartsDetails;

	protected FUIScrollListGrid mNodeHeartsGrid;

	protected bool mNodeHeartsGridFilled;

	protected List<FUIHeartPieceItem> mNodeHeartsGridItems = new List<FUIHeartPieceItem>();

	protected FNode mNodeGnomes;

	protected FLabel mNodeGnomesDetails;

	protected FUIScrollListGrid mNodeGnomesGrid;

	protected bool mNodeGnomesGridFilled;

	protected List<FUIGnomeItem> mNodeGnomesGridItems = new List<FUIGnomeItem>();

	protected int mSelectedGnomeIndex = -1;

	protected FNode mNodeWaffles;

	protected FLabel mNodeWafflesDetails;

	protected FUIScrollListGrid mNodeWafflesGrid;

	protected bool mNodeWafflesGridFilled;

	protected List<FUIWaffleItem> mNodeWafflesGridItems = new List<FUIWaffleItem>();

	protected int mSelectedWaffleIndex = -1;

	protected FUIScrollListGrid mNodeOutfitGrid;

	protected bool mNodeOutfitGridFilled;

	protected List<FUIOutfitItem> mNodeOutfitGridItems = new List<FUIOutfitItem>();

	protected int mSelectedOutfitmIndex = -1;

	private FUIButton mInventoryButton;

	private FUIButton mGnomesButton;

	private FUIButton mHeartsButton;

	private FUIButton mWafflesButton;

	public FPageInventory(string name)
		: base(name)
	{
		mInventoryButton = CreateTabButton(".InventoryButton", "Inventory.Inventory.Title", HandleInventoryButton);
		mGnomesButton = CreateTabButton(".GnomesButton", "Inventory.Gnomes.Title", HandleGnomesButton);
		mHeartsButton = CreateTabButton(".HeartsButton", "Inventory.Hearts.Title", HandleHeartsButton);
		mWafflesButton = CreateTabButton(".WafflesButton", "Inventory.Waffles.Title", HandleWafflesButton);
		mNodeInventory = new FNode(Name + ".NodeInventory");
		mNodeInventory.SetParent(mBackground);
		mNodeInventoryTools = new FPageInventoryContainer(Name + ".NodeInventoryTools", "Inventory.Inventory.Tools.Title");
		mNodeInventoryTools.SetParent(mNodeInventory);
		mNodeInventoryAmmo = new FPageInventoryContainer(Name + ".NodeInventoryAmmo", "Inventory.Inventory.Ammo.Title");
		mNodeInventoryAmmo.SetParent(mNodeInventory);
		mNodeInventoryUpgrades = new FPageInventoryContainer(Name + ".NodeInventoryUpgrades", "Inventory.Inventory.Upgrades.Title");
		mNodeInventoryUpgrades.SetParent(mNodeInventory);
		mNodeInventoryQuestItems = new FPageInventoryContainer(Name + ".NodeInventoryQuestItems", "Inventory.Inventory.QuestItems.Title");
		mNodeInventoryQuestItems.SetParent(mNodeInventory);
		mNodeInventoryDetails = new FLabel(Name + ".NodeInventoryDetails", "DefaultStyle", string.Empty, 24f, true, true, false);
		mNodeInventoryDetails.Wrap = true;
		mNodeInventoryDetails.SetParent(mNodeInventory);
		mNodeHearts = new FNode(Name + ".NodeHearts");
		mNodeHearts.SetParent(mBackground);
		mNodeHeartsDetails = new FLabel(Name + ".NodeHeartsDetails", "DefaultStyle", FLoc.GetString("Inventory.Hearts.Details"), 24f, true, true, false);
		mNodeHeartsDetails.Wrap = true;
		mNodeHeartsDetails.SetParent(mNodeHearts);
		mNodeHeartsGrid = new FUIScrollListGrid(Name + ".NodeHeartsGrid", Vector2.zero, Vector2.zero, false);
		mNodeHeartsGrid.GridMargin = new Vector2(16f, 0f);
		mNodeHeartsGrid.SetParent(mNodeHearts);
		mNodeGnomes = new FNode(Name + ".NodeGnomes");
		mNodeGnomes.SetParent(mBackground);
		mNodeGnomesDetails = new FLabel(Name + ".NodeGnomesDetails", "DefaultStyle", FLoc.GetString("Inventory.Gnomes.Details"), 24f, true, true, false);
		mNodeGnomesDetails.Wrap = true;
		mNodeGnomesDetails.SetParent(mNodeGnomes);
		mNodeGnomesGrid = new FUIScrollListGrid(Name + ".NodeGnomesGrid", Vector2.zero, Vector2.zero, false);
		mNodeGnomesGrid.SetParent(mNodeGnomes);
		mNodeWaffles = new FNode(Name + ".NodeWaffles");
		mNodeWaffles.SetParent(mBackground);
		mNodeWafflesDetails = new FLabel(Name + ".NodeWafflesDetails", "DefaultStyle", FLoc.GetString("Inventory.Waffles.Details"), 24f, true, true, false);
		mNodeWafflesDetails.Wrap = true;
		mNodeWafflesDetails.SetParent(mNodeWaffles);
		mNodeWafflesGrid = new FUIScrollListGrid(Name + ".NodeWafflesGrid", Vector2.zero, Vector2.zero, false);
		mNodeWafflesGrid.SetParent(mNodeWaffles);
		mNodeOutfitGrid = new FUIScrollListGrid(Name + ".NodeOutfitGrid", Vector2.zero, Vector2.zero, false);
		mNodeOutfitGrid.SetParent(mNodeWaffles);
	}

	private FUIButton CreateTabButton(string suffix, string titleLoc, System.Action<FUIButton, FTouch> handler)
	{
		FUIButton button = new FUIButton(Name + suffix);
		button.SetBackground("UITab.png");
		button.SetForegroundColor(new Color(1f, 0f, 0f, 1f));
		button.SetText(FLoc.GetString(titleLoc), "DefaultStyle", 18f, 0f);
		button.AddHandleTap(handler);
		button.SetParent(this);
		button.MoveToBottom();
		return button;
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		float q = mBackgroundSize.x * 0.25f;
		float w = q - 16f;
		mInventoryButton.DockOutside(mBackground, OuterDock.Top, w, 64f, q * -0.5f - q + 3f, -15f);
		mHeartsButton.DockOutside(mBackground, OuterDock.Top, w, 64f, q * -0.5f + 1f, -15f);
		mGnomesButton.DockOutside(mBackground, OuterDock.Top, w, 64f, q * 0.5f - 1f, -15f);
		mWafflesButton.DockOutside(mBackground, OuterDock.Top, w, 64f, q + q * 0.5f - 3f, -15f);
		mInventoryButton.MoveToBottom();
		mInventoryButton.SetForegroundColor(Color.grey);
		mGnomesButton.MoveToBottom();
		mGnomesButton.SetForegroundColor(Color.grey);
		mHeartsButton.MoveToBottom();
		mHeartsButton.SetForegroundColor(Color.grey);
		mWafflesButton.MoveToBottom();
		mWafflesButton.SetForegroundColor(Color.grey);
		mDialogCollider.MoveToBottom();
		FUIButton active;
		switch (mMode)
		{
		case Modes.Inventory:
			LayoutPageInventory();
			active = mInventoryButton;
			break;
		case Modes.Hearts:
			LayoutPageHearts();
			active = mHeartsButton;
			break;
		case Modes.Gnomes:
			LayoutPageGnomes();
			active = mGnomesButton;
			break;
		case Modes.Waffles:
			LayoutPageWaffles();
			active = mWafflesButton;
			break;
		default:
			return;
		}
		active.MoveToTop();
		active.SetForegroundColor(new Color(1f, 0f, 0f, 1f));
	}

	protected void LayoutPageInventory()
	{
		mTitle.Text = FLoc.GetString("Inventory.Inventory.Title");
		mNodeInventory.SetParent(mBackground);
		mNodeHearts.Unparent();
		mNodeGnomes.Unparent();
		mNodeWaffles.Unparent();
		mNodeInventory.DockInside(mBackground, InnerDock.Bottom, mContentSize.x, mContentSize.y, 0f, mBorderMargin);
		float contentW = mContentSize.x;
		float margin = mBorderMargin;
		float toolsW = contentW * 0.25f * 3f - margin;
		float ammoW = contentW - toolsW - margin;
		List<FUIScrollListItem> items = new List<FUIScrollListItem>();
		int index = 0;
		foreach (string collectibleName in FApp.PlayerStats.Collectibles)
		{
			FProtoCollectible collectible = FApp.ProtoCollectibleManager.GetProtoCollectible(collectibleName);
			if (collectible != null && collectible.Type == FProtoCollectible.Types.Tool)
			{
				items.Add(new FUIInventoryItem(index, collectibleName + "_ToolIcon.png", FLoc.GetString("Tool." + collectibleName + ".Label"), FLoc.GetString("Tool." + collectibleName + ".Details"), SelectInventoryItem));
				index++;
			}
		}
		mNodeInventoryTools.DockInside(mNodeInventory, InnerDock.TopLeft, toolsW, 96f, 0f, -18f);
		mNodeInventoryTools.Layout(new Vector2(toolsW, 96f), new Vector2(toolsW - 16f, 80f), new Vector2(128f, 36f), new Vector2(80f, 80f), items);
		items = new List<FUIScrollListItem>();
		FProtoUnit[] playerProtos = FApp.ProtoUnitManager.GetProtoUnitsWithTag("PlayerCharacter");
		if (playerProtos != null)
		{
			index = 0;
			for (int i = 0; i < playerProtos.Length; i++)
			{
				FProtoUnit proto = playerProtos[i];
				FProtoWeapon weapon = FApp.ProtoWeaponManager.GetProtoWeapon(proto.Weapon);
				if (weapon == null)
				{
					continue;
				}
				if (!weapon.HasAmmoResource)
				{
					if (proto.AlternateWeapons == null)
					{
						continue;
					}
					weapon = proto.AlternateWeapons.Select((string w) => FApp.ProtoWeaponManager.GetProtoWeapon(w)).FirstOrDefault((FProtoWeapon w) => w.HasAmmoResource);
					if (weapon == null)
					{
						continue;
					}
				}
				int ammo = FApp.PlayerStats.GetAmmoAmount(weapon.AmmoResource);
				if (!proto.CheckUnlockRequirements() || ammo < 0)
				{
					items.Add(new FUIInventoryItem(index, "InventoryItemUnknown.png", string.Empty, string.Empty, SelectInventoryItem));
				}
				else
				{
					items.Add(new FUIInventoryItem(index, proto.Name + "_Icon_Weapon.png", ammo.ToString(), FLoc.GetString(proto.Name + ".Ammo.Details"), SelectInventoryItem));
				}
				index++;
			}
		}
		mNodeInventoryAmmo.DockInside(mNodeInventory, InnerDock.TopRight, ammoW, 96f, 0f, -18f);
		mNodeInventoryAmmo.Layout(new Vector2(ammoW, 96f), new Vector2(ammoW - 16f, 80f), new Vector2(110f, 36f), new Vector2(80f, 80f), items);
		items = new List<FUIScrollListItem>();
		List<FProtoCollectible> upgrades = FApp.PlayerStats.Collectibles.Select((string c) => FApp.ProtoCollectibleManager.GetProtoCollectible(c)).Where((FProtoCollectible col) => col.Type == FProtoCollectible.Types.CharacterUpgrade).ToList();
		index = 0;
		foreach (FProtoCollectible upgrade in upgrades)
		{
			items.Add(new FUIInventoryItem(index, upgrade.Name + "_Icon.png", string.Empty, FLoc.GetString("Upgrade." + upgrade.Name + ".Details"), SelectInventoryItem));
			index++;
		}
		mNodeInventoryUpgrades.DockOutside(mNodeInventoryTools, OuterDock.BottomAlignLeft, contentW, 96f, 0f, 0f - mBorderMargin);
		float gridW = contentW - 16f;
		mNodeInventoryUpgrades.Layout(new Vector2(contentW, 96f), new Vector2(gridW, 80f), new Vector2(160f, 36f), new Vector2(80f, 80f), items);
		items = new List<FUIScrollListItem>();
		List<FProtoCollectible> questCollectibles = FApp.PlayerStats.Collectibles.Select((string c) => FApp.ProtoCollectibleManager.GetProtoCollectible(c)).Where((FProtoCollectible col) => col.Type == FProtoCollectible.Types.QuestItem).ToList();
		int maxItems = (int)(gridW / 80f);
		index = 0;
		foreach (FProtoCollectible questCollectible in questCollectibles)
		{
			if (index >= maxItems)
			{
				break;
			}
			FProtoQuest quest = FApp.ProtoQuestManager.GetProtoQuestRequiresItemDBID(questCollectible.DBID);
			if (quest != null && FApp.PlayerStats.CanCompleteQuest(quest.DBID, false) && quest.Icon.Valid())
			{
				FUIInventoryQuestItem questItem = new FUIInventoryQuestItem(index, quest.DBID, quest.Icon, string.Empty, quest.Details, SelectInventoryItem);
				items.Add(questItem);
				index++;
			}
		}
		mNodeInventoryQuestItems.DockOutside(mNodeInventoryUpgrades, OuterDock.BottomAlignLeft, contentW, 96f, 0f, 0f - mBorderMargin);
		mNodeInventoryQuestItems.Layout(new Vector2(contentW, 96f), new Vector2(gridW, 80f), new Vector2(200f, 36f), new Vector2(80f, 80f), items);
		UpdateQuestItems();
		mNodeInventoryDetails.DockInside(mNodeInventory, InnerDock.BottomLeft, mContentSize.x - mCloseButtonSize.x - mBorderMargin, 96f, 0f, -16f);
		mNodeInventoryDetails.Text = mSelectedInventoryItemDetails;
	}

	protected void UpdateQuestItems()
	{
		foreach (FUIScrollListItem item in mNodeInventoryQuestItems.GridItems)
		{
			if (item is FUIInventoryQuestItem questItem)
			{
				questItem.UpdateStatus(FApp.PlayerStats.CanCompleteQuest(questItem.DBID, true));
			}
		}
	}

	public void SelectInventoryItem(string details)
	{
		mNodeInventoryDetails.Text = details;
	}

	protected void LayoutPageHearts()
	{
		mTitle.Text = FLoc.GetString("Inventory.Hearts.Title");
		mNodeHearts.SetParent(mBackground);
		mNodeInventory.Unparent();
		mNodeGnomes.Unparent();
		mNodeWaffles.Unparent();
		mNodeHearts.DockInside(mBackground, InnerDock.Bottom, mContentSize.x, mContentSize.y, 0f, mBorderMargin);
		int total = FApp.ProtoCollectibleManager.GetNumberProtoCollectibleOfType(FProtoCollectible.Types.HeartPiece);
		int have = FApp.PlayerStats.GetNumberHeartPieceCollectibles();
		mNodeHeartsDetails.Text = FLoc.Format("Inventory.Hearts.Details", have, total);
		mNodeHeartsDetails.DockInside(mNodeHearts, InnerDock.BottomLeft, mContentSize.x - mCloseButtonSize.x - mBorderMargin, 96f, 0f, 0f);
		float gridW = mContentSize.x;
		float gridH = mContentSize.y - 96f;
		mNodeHeartsGrid.DockInside(mNodeHearts, InnerDock.Top, gridW, gridH, 0f, 0f);
		if (mNodeHeartsGridFilled)
		{
			for (int i = 0; i < total; i++)
			{
				mNodeHeartsGridItems[i].UpdateStatus(FApp.PlayerStats.HasCollectible(i + 100));
			}
			return;
		}
		mNodeHeartsGrid.RemoveItems();
		mNodeHeartsGrid.UpdateSizes(new Vector2(gridW, gridH), new Vector2(96f, 96f));
		for (int j = 0; j < total; j++)
		{
			FUIHeartPieceItem item = new FUIHeartPieceItem(j, FApp.PlayerStats.HasCollectible(j + 100));
			mNodeHeartsGrid.AddItem(item);
			mNodeHeartsGridItems.Add(item);
		}
		mNodeHeartsGrid.LayoutListContents();
		mNodeHeartsGridFilled = true;
	}

	protected void LayoutPageGnomes()
	{
		mTitle.Text = FLoc.GetString("Inventory.Gnomes.Title");
		mNodeGnomes.SetParent(mBackground);
		mNodeInventory.Unparent();
		mNodeHearts.Unparent();
		mNodeWaffles.Unparent();
		mNodeGnomes.DockInside(mBackground, InnerDock.Bottom, mContentSize.x, mContentSize.y, 0f, mBorderMargin);
		int total = FApp.ProtoCollectibleManager.GetNumberProtoCollectibleOfType(FProtoCollectible.Types.Gnome);
		mNodeGnomesDetails.DockInside(mNodeGnomes, InnerDock.BottomLeft, mContentSize.x - mCloseButtonSize.x - mBorderMargin, 96f, 0f, 0f);
		float gridH = mContentSize.y - 96f;
		mNodeGnomesGrid.DockInside(mNodeGnomes, InnerDock.Top, 576f, gridH, 0f, 0f);
		if (!mNodeGnomesGridFilled)
		{
			mNodeGnomesGrid.RemoveItems();
			mNodeGnomesGrid.UpdateSizes(new Vector2(576f, gridH), new Vector2(96f, 96f));
			for (int i = 0; i < total; i++)
			{
				FUIGnomeItem item = new FUIGnomeItem(i, false, false, SelectGnomeIndex);
				mNodeGnomesGrid.AddItem(item);
				mNodeGnomesGridItems.Add(item);
			}
			mNodeGnomesGrid.LayoutListContents();
			mNodeGnomesGridFilled = true;
		}
		UpdateGnomeStatus();
	}

	protected void UpdateGnomeStatus()
	{
		if (mSelectedGnomeIndex < 0)
		{
			mNodeGnomesDetails.Text = FLoc.GetString("Inventory.Gnomes.Details");
		}
		int total = FApp.ProtoCollectibleManager.GetNumberProtoCollectibleOfType(FProtoCollectible.Types.Gnome);
		for (int i = 0; i < total; i++)
		{
			bool collected = FApp.PlayerStats.HasCollectible(i);
			if (i == mSelectedGnomeIndex)
			{
				FProtoCollectible collectible = FApp.ProtoCollectibleManager.GetProtoCollectibleByDBID(i);
				if (collectible != null)
				{
					mNodeGnomesDetails.Text = (FLoc.IsRightToLeft ? (collectible.Details + " :" + collectible.VisibleName) : (collectible.VisibleName + ": " + collectible.Details));
				}
			}
			mNodeGnomesGridItems[i].UpdateStatus(collected, i == mSelectedGnomeIndex);
		}
	}

	public void SelectGnomeIndex(int index)
	{
		mSelectedGnomeIndex = index;
		UpdateGnomeStatus();
	}

	protected void LayoutPageWaffles()
	{
		mTitle.Text = FLoc.GetString("Inventory.Waffles.Title");
		mNodeWaffles.SetParent(mBackground);
		mNodeInventory.Unparent();
		mNodeHearts.Unparent();
		mNodeGnomes.Unparent();
		mNodeWaffles.DockInside(mBackground, InnerDock.Bottom, mContentSize.x, mContentSize.y, 0f, mBorderMargin);
		int total = FApp.ProtoCollectibleManager.GetNumberProtoCollectibleOfType(FProtoCollectible.Types.Eggo);
		mNodeWafflesDetails.DockInside(mNodeWaffles, InnerDock.BottomLeft, mContentSize.x - mCloseButtonSize.x - mBorderMargin, 96f, 0f, 0f);
		mSelectedWaffleIndex = -1;
		float gridH = mContentSize.y - 96f;
		float gridW = mContentSize.x * 0.5f - mBorderMargin;
		mNodeWafflesGrid.DockInside(mNodeWaffles, InnerDock.TopLeft, gridW, gridH, 0f, 0f);
		if (!mNodeWafflesGridFilled)
		{
			mNodeWafflesGrid.RemoveItems();
			mNodeWafflesGrid.UpdateSizes(new Vector2(gridW, gridH), new Vector2(96f, 96f));
			for (int i = 0; i < total; i++)
			{
				FUIWaffleItem item = new FUIWaffleItem(i, false, false, SelectWaffleIndex);
				mNodeWafflesGrid.AddItem(item);
				mNodeWafflesGridItems.Add(item);
			}
			mNodeWafflesGrid.LayoutListContents();
			mNodeWafflesGridFilled = true;
		}
		UpdateWaffleStatus();
		int outfitTotal = FApp.ProtoCollectibleManager.GetNumberProtoCollectibleOfType(FProtoCollectible.Types.Outfit);
		mSelectedOutfitmIndex = -1;
		mNodeOutfitGrid.DockInside(mNodeWaffles, InnerDock.TopRight, gridW, gridH, 0f, 0f);
		if (!mNodeOutfitGridFilled)
		{
			mNodeOutfitGrid.RemoveItems();
			mNodeOutfitGrid.UpdateSizes(new Vector2(gridW, gridH), new Vector2(96f, 96f));
			for (int j = 0; j < outfitTotal; j++)
			{
				FUIOutfitItem outfitItem = new FUIOutfitItem(j, false, false, SelectOutfitIndex);
				mNodeOutfitGrid.AddItem(outfitItem);
				mNodeOutfitGridItems.Add(outfitItem);
			}
			mNodeOutfitGrid.LayoutListContents();
			mNodeOutfitGridFilled = true;
		}
		UpdateOutfitStatus();
	}

	protected void UpdateWaffleStatus()
	{
		if (mSelectedWaffleIndex < 0)
		{
			mNodeWafflesDetails.Text = FLoc.GetString("Inventory.Waffles.Details");
		}
		int total = FApp.ProtoCollectibleManager.GetNumberProtoCollectibleOfType(FProtoCollectible.Types.Eggo);
		for (int i = 0; i < total; i++)
		{
			bool collected = FApp.PlayerStats.HasCollectible(i + 200);
			if (i == mSelectedWaffleIndex)
			{
				FProtoCollectible collectible = FApp.ProtoCollectibleManager.GetProtoCollectibleByDBID(i + 200);
				if (collectible != null)
				{
					mNodeWafflesDetails.Text = (FLoc.IsRightToLeft ? (collectible.Details + " :" + collectible.VisibleName) : (collectible.VisibleName + ": " + collectible.Details));
				}
			}
			mNodeWafflesGridItems[i].UpdateStatus(collected, i == mSelectedWaffleIndex);
		}
	}

	protected void UpdateOutfitStatus()
	{
		if (mSelectedOutfitmIndex < 0)
		{
			mNodeWafflesDetails.Text = FLoc.GetString("Inventory.Waffles.Details");
		}
		int total = FApp.ProtoCollectibleManager.GetNumberProtoCollectibleOfType(FProtoCollectible.Types.Outfit);
		for (int i = 0; i < total; i++)
		{
			bool collected = FApp.PlayerStats.HasCollectible(i + 300);
			FProtoCollectible collectible = FApp.ProtoCollectibleManager.GetProtoCollectibleByDBID(i + 300);
			if (i == mSelectedOutfitmIndex)
			{
				mNodeWafflesDetails.Text = (FLoc.IsRightToLeft ? (collectible.Details + " :" + collectible.VisibleName) : (collectible.VisibleName + ": " + collectible.Details));
			}
			string characterName = OutfitHandler.GetCharacterNameFromOutfitName(collectible.Name);
			mNodeOutfitGridItems[i].UpdateStatus(collected, i == mSelectedOutfitmIndex, characterName);
		}
	}

	public void SelectWaffleIndex(int index)
	{
		mSelectedOutfitmIndex = -1;
		UpdateOutfitStatus();
		mSelectedWaffleIndex = index;
		UpdateWaffleStatus();
	}

	public void SelectOutfitIndex(int index)
	{
		mSelectedWaffleIndex = -1;
		UpdateWaffleStatus();
		mSelectedOutfitmIndex = index;
		UpdateOutfitStatus();
	}

	public override void HandleAddedToStage()
	{
		mMode = Modes.Inventory;
		mSelectedInventoryItemDetails = string.Empty;
		mSelectedGnomeIndex = -1;
		mSelectedWaffleIndex = -1;
		base.HandleAddedToStage();
		FApp.SoundManager.PlaySoundSet("UI.Inventory.Open");
		LayoutPage();
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		FApp.SoundManager.PlaySoundSet("UI.Inventory.Close");
	}

	protected void HandleInventoryButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		mMode = Modes.Inventory;
		LayoutPage();
	}

	protected void HandleGnomesButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		mMode = Modes.Gnomes;
		LayoutPage();
	}

	protected void HandleHeartsButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		mMode = Modes.Hearts;
		LayoutPage();
	}

	protected void HandleWafflesButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		mMode = Modes.Waffles;
		LayoutPage();
	}

	protected void HandleNextButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		mMode = (((int)mMode < 4) ? (mMode + 1) : Modes.Inventory);
		LayoutPage();
	}

	protected void HandlePrevButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		int prev = (int)mMode - 1;
		if (prev == 0 || (int)mMode < 1)
		{
			prev = 4;
		}
		mMode = (Modes)prev;
		LayoutPage();
	}
}
