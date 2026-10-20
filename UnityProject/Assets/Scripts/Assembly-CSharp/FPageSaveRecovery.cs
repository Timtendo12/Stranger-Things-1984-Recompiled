using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageSaveRecovery @ 0x180ed38..0x1810138, Ghidra vaddr).
public class FPageSaveRecovery : FUIPage
{
	public static readonly Vector2 cSize = new Vector2(600f, 700f);

	public static readonly Vector2 cTextButtonSize = new Vector2(500f, 128f);

	protected FUICollider mCollider;

	protected FNode mDialog;

	protected FSlicedSprite mBackground;

	protected FLabel mTitleText;

	protected List<FSaveRecoveryNode> mButtons = new List<FSaveRecoveryNode>();

	protected FLabel mResetText;

	protected FNodeStack mButtonStack;

	public FPageSaveRecovery(string name)
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
		mTitleText = new FLabel(Name + ".Title", "DefaultStyle", FLoc.GetString("SaveRecovery.Title"), 64f, true, true, false);
		mTitleText.Color = new Color(1f, 0f, 0f, 1f);
		mTitleText.SetParent(mBackground);
		mButtonStack = new FNodeStack(Name + ".SaveButtons", FNodeStack.Modes.FromTop, 16f);
		mButtonStack.SetParent(mBackground);
		mResetText = new FLabel(Name + ".Reset", "DefaultStyle", FLoc.GetString("SaveRecovery.Warning"), 32f, true, true, false);
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
		mResetText.DockInside(mBackground, InnerDock.Bottom, cSize.x - 32f, 48f, 0f, 16f);
		mButtonStack.DockOutside(mTitleText, OuterDock.Bottom, cSize.x - 48f, cSize.y - 72f - 64f, 0f, -24f);
		mButtonStack.ClearStack();
		foreach (FSaveRecoveryNode button in mButtons)
		{
			mButtonStack.StackChild(button, cTextButtonSize);
		}
		MarkDirtyRecursive();
	}

	public override void TransitionOut()
	{
		TransitionOut(mDialog);
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		LayoutPage();
		TransitionIn(mDialog);
	}

	public void UpdateSaves(FPlayerData[] datas)
	{
		if (datas == null || datas.Length < 2)
		{
			Exit();
			return;
		}
		int index = 1;
		foreach (FSaveRecoveryNode button in mButtons)
		{
			FNode.Cleanup(button);
		}
		mButtons.Clear();
		for (int i = 0; i < datas.Length; i++)
		{
			FPlayerData data = datas[i];
			if (data.LoadedXmlDoc != null)
			{
				FPlayerStats stats = new FPlayerStats();
				stats.Deserialize(data.LoadedXmlDoc.DocumentElement.GetChildValue("SavedState", string.Empty));
				FSaveRecoveryNode node = new FSaveRecoveryNode(Name + ".Button." + data.Filename, index.ToString());
				node.SetBackground("Button002.png");
				node.AddHandleRelease(HandleSaveButton);
				node.SetParent(mButtonStack);
				node.SetSaveInfo(stats);
				mButtons.Add(node);
				index++;
			}
		}
		LayoutPage();
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

	protected void HandleSaveButton(FUIButton button, FTouch touch)
	{
		FSaveRecoveryNode node = button as FSaveRecoveryNode;
		if (node != null)
		{
			FApp.PlayerStats = node.PlayerStats;
			Exit();
		}
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
