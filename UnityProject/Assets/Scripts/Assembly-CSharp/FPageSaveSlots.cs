using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageSaveSlots @ 0x18101f0..0x18120d0, Ghidra vaddr).
public class FPageSaveSlots : FUIPage
{
	public static readonly Vector2 cSize = new Vector2(600f, 700f);

	public static readonly Vector2 cTextButtonSize = new Vector2(500f, 116f);

	protected FUICollider mCollider;

	protected FNode mDialog;

	protected FSlicedSprite mBackground;

	protected FLabel mTitleText;

	protected FUIScrollListVertical mButtonlist;

	protected FUIScrollListItemButton[] mSaveSlots;

	protected FUIScrollListItemButton mNewSaveButton;

	public FPageSaveSlots(string name)
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
		mTitleText = new FLabel(Name + ".Title", "DefaultStyle", FLoc.GetString("SelectSave"), 64f, true, true, false);
		mTitleText.Color = new Color(1f, 0f, 0f, 1f);
		mTitleText.SetParent(mBackground);
		mButtonlist = new FUIScrollListVertical(Name + ".SaveSlotButtons", new Vector2(cTextButtonSize.x, cSize.y), cTextButtonSize.y, false);
		mButtonlist.ClipToExtents = true;
		mButtonlist.SetParent(mBackground);
		mNewSaveButton = new FUIScrollListItemButton(Name + ".NewSaveButton");
		mNewSaveButton.Initialize(9999, cTextButtonSize, Vector2.zero, "Button002.png", "Button003.png", HandleNewSaveButton);
		mNewSaveButton.Button.SetText(FLoc.GetString("NewSaveSlotButton"), "DefaultStyle", 32f, 0f);
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		mCollider.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		mDialog.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		mBackground.DockInside(mDialog, InnerDock.Center, cSize.x, cSize.y, 0f, 0f);
		mTitleText.DockInside(mBackground, InnerDock.Top, cSize.x - 48f, 64f, 0f, -24f);
		mButtonlist.DockInside(mBackground, InnerDock.Bottom, cSize.x - 48f, cSize.y - 72f - 64f, 0f, 24f);
		mButtonlist.RemoveItems(false);
		if (mSaveSlots == null)
		{
			PopSaveSlots();
		}
		// port guard: the shipped code dereferences mSaveSlots unconditionally (PlayerStats exists when the HUD is built there)
		for (int i = 0; mSaveSlots != null && i < mSaveSlots.Length; i++)
		{
			mButtonlist.AddItem(mSaveSlots[i]);
		}
		mButtonlist.AddItem(mNewSaveButton);
		mButtonlist.LayoutListContents();
		mButtonlist.ScrollToTop();
		MarkDirtyRecursive();
	}

	public override void TransitionOut()
	{
		TransitionOut(mDialog);
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		PopSaveSlots();
		LayoutPage();
		UpdateSaveSlots();
		TransitionIn(mDialog);
		// original calls FApp.HUD.PageDebug.ToggleConsole() here; the debug page does not exist on PC
		if (FApp.HUD.PageDebug != null)
		{
			FApp.HUD.PageDebug.ToggleConsole();
		}
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
	}

	protected void PopSaveSlots()
	{
		if (FApp.PlayerStats == null)
		{
			return;
		}
		if (mSaveSlots != null)
		{
			for (int i = 0; i < mSaveSlots.Length; i++)
			{
				FNode.Cleanup(mSaveSlots[i]);
			}
		}
		System.Collections.Generic.List<FPlayerState> states = FApp.PlayerStats.SavedPlayerStates;
		mSaveSlots = new FUIScrollListItemButton[states.Count];
		for (int j = 0; j < mSaveSlots.Length; j++)
		{
			FPlayerState state = states[j];
			mSaveSlots[j] = new FUIScrollListItemButton(Name + ".SaveSlot_" + j.ToString());
			mSaveSlots[j].Initialize(j, cTextButtonSize, Vector2.zero, "Button002.png", "Button003.png", HandleSaveSlotButton);
			// separators: the decompile labels are 1 char (0x32122b8, "%") and 2 chars (0x320a740, ", "), matching the pieces below (not independently readable from the binary)
			string text = string.Concat(new string[14]
			{
				(state.SaveType == 0) ? "Normal" : "Classic",
				"\n<image name='HUDHeartFull.png'/>x",
				state.MaxHealth.ToString(),
				" <image name='HUDCoin.png'/>x",
				state.Coins.ToString(),
				" <image name='HUDKeyCard.png'/>x",
				state.KeyCards.ToString(),
				" v",
				state.SaveVersion.ToString(),
				", ",
				FLoc.GetFormattedTime(state.TotalGameTime),
				", ",
				state.GetPercentComplete().ToString(),
				"%"
			});
			mSaveSlots[j].Button.SetText(text, "DefaultStyle", 24f, 0f);
			FUIButton deleteButton = new FUIButton("Delete_Button");
			deleteButton.Name = j.ToString();
			deleteButton.SetBackground("ButtonClose.png");
			deleteButton.AddHandleTap(HandleDeleteSave);
			deleteButton.SetParent(mSaveSlots[j]);
			deleteButton.DockInside(mSaveSlots[j], InnerDock.Right, 64f, 64f, -8f, 0f);
			FUIButton copyButton = new FUIButton("Copy_Button");
			copyButton.Name = j.ToString();
			copyButton.SetBackground("UIButtonOptions.png");
			copyButton.AddHandleTap(HandleCopySave);
			copyButton.SetParent(mSaveSlots[j]);
			copyButton.DockInside(mSaveSlots[j], InnerDock.Left, 64f, 64f, 8f, 0f);
		}
	}

	protected void HandleDeleteSave(FUIButton button, FTouch touch)
	{
		int index = -1;
		if (int.TryParse(button.Name, out index))
		{
			FApp.HUD.OpenYesNoDialog(FLoc.GetString("ResetSave.Title"), FLoc.GetString("ResetSave.Details"), FLoc.GetString("YesButton"), delegate
			{
				ActuallyDeleteSave(index);
			}, FLoc.GetString("NoButton"), null, delegate
			{
				ActuallyDeleteSave(index);
			}, null, FUIPage.TransitionStyles.Right, FPageOkDialog.AndroidBackButtonType.Button1);
		}
	}

	protected void HandleCopySave(FUIButton button, FTouch touch)
	{
		int result = -1;
		if (int.TryParse(button.Name, out result))
		{
			FApp.PlayerStats.DuplicateSave(result);
			PopSaveSlots();
			LayoutPage();
			UpdateSaveSlots();
		}
	}

	protected void ActuallyDeleteSave(int index)
	{
		FApp.PlayerStats.DeleteSaveSlot(index);
		FApp.PlayerData.Save(); // PC port: persist the deletion
		PopSaveSlots();
		LayoutPage();
		UpdateSaveSlots();
	}

	protected void UpdateSaveSlots()
	{
		for (int i = 0; i < mSaveSlots.Length; i++)
		{
			mSaveSlots[i].Button.SetBackground((i == FApp.PlayerStats.CurrentSaveSlot) ? "Button003.png" : "Button002.png");
		}
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

	protected void HandleNewSaveButton(int index, FTouch touch)
	{
		FApp.HUD.OpenYesNoDialog(FLoc.GetString("SaveType.Title"), FLoc.GetString("SaveType.Details"), FLoc.GetString("NormalButton"), delegate
		{
			CreateNewSaveOfType(0);
		}, FLoc.GetString("ClassicButton"), delegate
		{
			CreateNewSaveOfType(1);
		}, null, null, FUIPage.TransitionStyles.Right, FPageOkDialog.AndroidBackButtonType.Button1);
	}

	protected void CreateNewSaveOfType(int saveType)
	{
		FApp.PlayerStats.AddSaveSlot(saveType);
		PopSaveSlots();
		LayoutPage();
		UpdateSaveSlots();
	}

	protected void HandleSaveSlotButton(int index, FTouch touch)
	{
		if (index >= 0)
		{
			if (FApp.PlayerStats.CurrentSaveSlot == index)
			{
				Exit();
				return;
			}
			FApp.PlayerStats.SwitchSaveSlot(index);
			UpdateSaveSlots();
		}
	}

	protected void HandleResumeButton(FUIButton button, FTouch touch)
	{
		FApp.HUD.PopPage(this);
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
