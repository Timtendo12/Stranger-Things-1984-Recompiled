using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageGameModeChoice @ 0x17f5d54..0x17f7f10, Ghidra vaddr).
public class FPageGameModeChoice : FUIPage
{
	public static readonly Vector2 cSize = new Vector2(600f, 730f);

	public static readonly Vector2 cTextButtonSize = new Vector2(500f, 256f);

	protected FUICollider mCollider;

	protected FNode mDialog;

	protected FSlicedSprite mBackground;

	protected FLabel mTitleText;

	protected List<FGameModeNode> mSaveButtons = new List<FGameModeNode>();

	protected FUIScrollListVertical mButtonStack;

	protected FGameModeNode mClassicButton;

	protected FLabel mResetText;

	public FPageGameModeChoice(string name)
		: base(name)
	{
		base.Modal = true;
		base.DarkenLower = true;
		base.TransitionStyle = TransitionStyles.Scale;
		mCollider = new FUICollider("Pause", false);
		mCollider.AddSignalPress(HandleScreenCollider);
		mCollider.SetParent(this);
		mDialog = new FNode("DialogGameModeChoice");
		mDialog.SetParent(this);
		mBackground = new FSlicedSprite(Name + ".Background", "Dialog003.png");
		mBackground.SetParent(mDialog);
		mTitleText = new FLabel(Name + ".Title", "DefaultStyle", FLoc.GetString("SelectGameMode.Title"), 64f, true, true, false);
		mTitleText.Color = new Color(1f, 0f, 0f, 1f);
		mTitleText.SetParent(mBackground);
		mButtonStack = new FUIScrollListVertical(Name + ".SaveSlotButtons", new Vector2(cTextButtonSize.x, cSize.y - 300f), cTextButtonSize.y, false);
		mButtonStack.ClipToExtents = true;
		mButtonStack.SetParent(mBackground);
		mResetText = new FLabel(Name + ".Reset", "DefaultStyle", FLoc.GetString("SelectGameMode.Reset"), 32f, true, true, false);
		mResetText.Color = Color.grey;
		mResetText.SetParent(mBackground);
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		mCollider.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		mDialog.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		mBackground.DockInside(mDialog, InnerDock.Center, cSize.x, cSize.y, 0f, 0f);
		mTitleText.DockInside(mBackground, InnerDock.Top, cSize.x - 48f, 64f, 0f, -24f);
		mButtonStack.DockOutside(mTitleText, OuterDock.Bottom, cSize.x - 48f, cSize.y - 72f - 64f - 48f, 0f, -24f);
		mButtonStack.RemoveItems(false);
		foreach (FGameModeNode saveButton in mSaveButtons)
		{
			mButtonStack.AddItem(saveButton);
		}
		mButtonStack.LayoutListContents();
		mButtonStack.ScrollToTop();
		mResetText.DockInside(mBackground, InnerDock.Bottom, cSize.x - 32f, 48f, 0f, 16f);
		MarkDirtyRecursive();
	}

	public override void TransitionOut()
	{
		TransitionOut(mDialog);
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		UpdateSlots();
		LayoutPage();
		TransitionIn(mDialog);
	}

	protected void UpdateSlots()
	{
		foreach (FGameModeNode saveButton in mSaveButtons)
		{
			FNode.Cleanup(saveButton);
		}
		mSaveButtons.Clear();
		int slot = 0;
		foreach (FPlayerState state in FApp.PlayerStats.SavedPlayerStates)
		{
			string title = ((state.SaveType == 0) ? FLoc.GetString("NormalButton") : FLoc.GetString("ClassicButton"));
			FGameModeNode node = new FGameModeNode(Name + ".SaveButton_" + slot.ToString(), title);
			node.Initialize(slot, cTextButtonSize, Vector2.zero, "Button002.png", "Button003.png", HandleSaveButton);
			FApp.PlayerStats.SwitchSaveSlot(slot);
			node.SetSaveSlotInfo(state, (FPlayerStats.SaveTypes)state.SaveType);
			if (state.SaveType == 0)
			{
				mSaveButtons.Insert(0, node);
			}
			else
			{
				mSaveButtons.Add(node);
			}
			slot++;
		}
		if (!FApp.PlayerStats.SwitchToFirstSaveSlotOfType(0))
		{
			FGameModeNode node2 = new FGameModeNode(Name + ".NormalButton", FLoc.GetString("NormalButton"));
			node2.Name = slot.ToString();
			node2.Initialize(slot, cTextButtonSize, Vector2.zero, "Button002.png", "Button003.png", HandleNormalButton);
			node2.SetSaveSlotInfo(null, FPlayerStats.SaveTypes.NORMAL);
			mSaveButtons.Insert(0, node2);
			slot++;
		}
		if (!FApp.PlayerStats.SwitchToFirstSaveSlotOfType(1))
		{
			FGameModeNode node3 = new FGameModeNode(Name + ".ClassicButton", FLoc.GetString("ClassicButton"));
			node3.Name = slot.ToString();
			node3.Initialize(slot, cTextButtonSize, Vector2.zero, "Button002.png", "Button003.png", HandleClassicButton);
			node3.SetSaveSlotInfo(null, FPlayerStats.SaveTypes.CLASSIC);
			mSaveButtons.Add(node3);
		}
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
	}

	public override bool HandleInput(float deltaTime)
	{
		bool escape = Input.GetKeyDown(KeyCode.Escape);
		if (escape)
		{
			Exit();
		}
		return escape;
	}

	protected void HandleSaveButton(int buttonIndex, FTouch touch)
	{
		if (buttonIndex >= 0 && buttonIndex < mSaveButtons.Count)
		{
			if (FApp.PlayerStats.SwitchSaveSlot(buttonIndex))
			{
				LoadGame();
			}
			else
			{
				FLog.Warning("Could not switch to save slot index: " + buttonIndex.ToString());
			}
		}
		else
		{
			FLog.Warning("Save button index out of range: " + buttonIndex.ToString());
		}
	}

	protected void HandleNormalButton(int buttonIndex, FTouch touch)
	{
		SwitchToSaveOfType(0);
		LoadGame();
	}

	protected void HandleClassicButton(int buttonIndex, FTouch touch)
	{
		SwitchToSaveOfType(1);
		LoadGame();
	}

	protected void SwitchToSaveOfType(int saveType)
	{
		if (!FApp.PlayerStats.SwitchToFirstSaveSlotOfType(saveType))
		{
			FApp.PlayerStats.AddSaveSlot(saveType);
			FApp.PlayerStats.SwitchToFirstSaveSlotOfType(saveType);
		}
	}

	protected void LoadGame()
	{
		string world = (FCoreUtil.Valid(FApp.PlayerStats.CurrentWorld) ? FApp.PlayerStats.CurrentWorld : FApp.WorldManager.DefaultWorldName);
		FApp.HUD.LoadLevel(world, null, null, false);
		Exit();
	}

	protected void Exit()
	{
		FApp.HUD.PopPage(this);
	}

	public void HandleScreenCollider(FUICollider collider, FTouch touch)
	{
		FApp.HUD.PopPage(this);
	}
}
