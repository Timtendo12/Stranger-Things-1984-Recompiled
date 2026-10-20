using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageBugReport @ 0x1798cac..0x179b1a0, Ghidra vaddr).
// PC port: submitting goes through FPluginRelay.ReportBug (local-only on PC).
public class FPageBugReport : FUIPage
{
	public static readonly float cMargin = 24f;

	public static readonly float cTitleBarHeight = 96f;

	public static readonly float cNameNodeHeight = 96f;

	public static readonly float cSubmitButtonHeight = 96f;

	protected FUICollider mCollider;

	protected FNode mDialog;

	protected FSlicedSprite mBackground;

	protected FNode mTitleBar;

	protected FUIButton mCloseButton;

	protected FLabel mTitleLabel;

	protected FNodeStack mContents;

	protected FNode mNameNode;

	protected FLabel mNameLabel;

	protected FSlicedSprite mNameTextBackground;

	protected FTextEntry mNameTextEntry;

	protected FNode mDetailsNode;

	protected FLabel mDetailsLabel;

	protected FSlicedSprite mDetailsTextBackground;

	protected FTextEntry mDetailsTextEntry;

	protected FUIButton mSubmitButton;

	protected List<FNode> mTabNodes = new List<FNode>();

	protected FNode mFocusNode;

	public FPageBugReport(string name)
		: base(name)
	{
		base.Modal = true;
		base.TransitionStyle = TransitionStyles.Scale;
		mCollider = new FUICollider(Name + ".Collider", false);
		mCollider.SetParent(this);
		mDialog = new FNode(Name + ".Dialog");
		mDialog.SetParent(this);
		mBackground = new FSlicedSprite(Name + ".Background", "BlackSquare.png");
		mBackground.SetParent(mDialog);
		mTitleBar = new FNode(Name + ".TitleBar");
		mTitleBar.SetParent(mDialog);
		mCloseButton = new FUIButton(Name + ".TitleBar.CloseButton");
		mCloseButton.SetParent(mTitleBar);
		mCloseButton.SetBackground("ButtonClose.png");
		mCloseButton.FocusBackgroundSprite = "ButtonClose.png";
		mCloseButton.NonFocusBackgroundSprite = "ButtonClose.png";
		mCloseButton.WantsFocus = true;
		mCloseButton.AddHandleRelease(HandleCloseButton);
		mCloseButton.AddHandleTab(HandleTab);
		mCloseButton.AddHandleEscape(HandleEscape);
		mCloseButton.AddHandleEnter(HandleEnter);
		mTitleLabel = new FLabel(Name + ".TitleBar.TitleLabel", "DefaultStyle", string.Empty, 64f, true, true, false);
		mTitleLabel.SetParent(mTitleBar);
		mTitleLabel.Text = FLoc.GetString("BugReport.Title");
		mTitleLabel.Color = FUI.Colors.UITitle;
		mContents = new FNodeStack(Name + ".Contents", FNodeStack.Modes.CenteredY, cMargin);
		mContents.SetParent(mDialog);
		mNameNode = new FNode(Name + ".NameNode");
		mNameLabel = new FLabel(Name + ".NameNode.NameLabel", "DefaultStyle", string.Empty, 48f, true, true, false);
		mNameLabel.SetParent(mNameNode);
		mNameLabel.Justify(FNode.Sides.Left, FNode.Sides.CenterY);
		mNameLabel.Text = FLoc.GetString("BugReport.Name");
		mNameLabel.Color = FUI.Colors.UITitle;
		mNameTextBackground = new FSlicedSprite(Name + ".NameNode.NameTextBackground", "WhiteSquare.png");
		mNameTextBackground.Color = FColorNames.Get("DarkGray");
		mNameTextBackground.SetParent(mNameNode);
		mNameTextEntry = new FTextEntry(Name + ".NameNode.TextEntry", "DefaultStyle", 48f, HorizSide.Left, VertSide.CenterY, FPlayerData.cBugReportNameMaxLength, false, true);
		mNameTextEntry.SetParent(mNameNode);
		mNameTextEntry.AddHandleEnter(HandleEnter);
		mNameTextEntry.AddHandleTab(HandleTab);
		mNameTextEntry.AddHandleEscape(HandleEscape);
		mNameTextEntry.AddHandleWantFocus(HandleChildWantFocus);
		mDetailsNode = new FNode(Name + ".DetailsNode");
		mDetailsLabel = new FLabel(Name + ".DetailsNode.DetailsLabel", "DefaultStyle", string.Empty, 48f, true, true, false);
		mDetailsLabel.Justify(FNode.Sides.Left, FNode.Sides.CenterY);
		mDetailsLabel.SetParent(mDetailsNode);
		mDetailsLabel.Text = FLoc.GetString("BugReport.Details");
		mDetailsLabel.Color = FUI.Colors.UITitle;
		mDetailsTextBackground = new FSlicedSprite(Name + ".DetailsNode.DetailsTextBackground", "WhiteSquare.png");
		mDetailsTextBackground.Color = FColorNames.Get("DarkGray");
		mDetailsTextBackground.SetParent(mDetailsNode);
		mDetailsTextEntry = new FTextEntry(Name + ".DetailsNode.TextEntry", "DefaultStyle", 48f, HorizSide.Left, VertSide.CenterY, 512, false, true);
		mDetailsTextEntry.SetParent(mDetailsNode);
		mDetailsTextEntry.AddHandleEnter(HandleEnter);
		mDetailsTextEntry.AddHandleTab(HandleTab);
		mDetailsTextEntry.AddHandleEscape(HandleEscape);
		mDetailsTextEntry.AddHandleWantFocus(HandleChildWantFocus);
		mSubmitButton = FUI.Buttons.AllocateStandardButton("SubmitButton", HandleSubmitButton, HandleTab, HandleEscape, HandleEnter, true);
		mSubmitButton.SetParent(mDialog);
		mTabNodes.Add(mCloseButton);
		mTabNodes.Add(mNameTextEntry);
		mTabNodes.Add(mDetailsTextEntry);
		mTabNodes.Add(mSubmitButton);
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		mCollider.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		mDialog.DockInside(this, InnerDock.Center, 1200f, 800f, 0f, 0f);
		mBackground.DockInside(mDialog, InnerDock.Center, 1200f, 800f, 0f, 0f);
		float innerWidth = 1200f - (cMargin + cMargin);
		mTitleBar.DockInside(mDialog, InnerDock.Top, innerWidth, cTitleBarHeight, 0f, 0f - cMargin);
		mCloseButton.DockInside(mTitleBar, InnerDock.Left, cTitleBarHeight, cTitleBarHeight, Vector2.zero);
		mTitleLabel.DockInside(mTitleBar, InnerDock.Right, innerWidth - cTitleBarHeight - cMargin, cTitleBarHeight, Vector2.zero);
		float contentsHeight = 800f - cMargin - cTitleBarHeight - cMargin - cMargin;
		// TODO(port): the contents stack's y offset is lost in the decompile (0x1799ebc); assumed to be the margin [left: the bug report page is constructed but never pushed on PC]
		mContents.DockInside(mDialog, InnerDock.Bottom, innerWidth, contentsHeight, 0f, cMargin);
		mContents.ClearStack();
		mContents.StackChild(mNameNode, new Vector2(innerWidth, cNameNodeHeight));
		float labelWidth = innerWidth * 0.2f;
		mNameLabel.DockInside(mNameNode, InnerDock.Left, labelWidth, cNameNodeHeight, Vector2.zero);
		float entryWidth = innerWidth - labelWidth;
		mNameTextBackground.DockInside(mNameNode, InnerDock.Right, entryWidth, cNameNodeHeight, Vector2.zero);
		mNameTextEntry.DockInside(mNameNode, InnerDock.Right, entryWidth, cNameNodeHeight, Vector2.zero);
		float detailsHeight = contentsHeight - cNameNodeHeight - cMargin - cSubmitButtonHeight - cMargin;
		mContents.StackChild(mDetailsNode, new Vector2(innerWidth, detailsHeight));
		mDetailsLabel.DockInside(mDetailsNode, InnerDock.TopLeft, innerWidth, cNameNodeHeight, Vector2.zero);
		float detailsEntryHeight = detailsHeight - cNameNodeHeight;
		mDetailsTextBackground.DockInside(mDetailsNode, InnerDock.Bottom, innerWidth, detailsEntryHeight, Vector2.zero);
		mDetailsTextEntry.DockInside(mDetailsNode, InnerDock.Bottom, innerWidth, detailsEntryHeight, Vector2.zero);
		mContents.StackChild(mSubmitButton, new Vector2(cSubmitButtonHeight * 3f, cSubmitButtonHeight));
		MarkDirtyRecursive();
	}

	public bool IsTextEntryOn()
	{
		return base.OnStage;
	}

	public override void TransitionOut()
	{
		TransitionOut(mDialog);
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		TransitionIn(mDialog);
		mFocusNode = null;
		if (mNameTextEntry != null && mDetailsTextEntry != null)
		{
			mNameTextEntry.SetText(FApp.PlayerData.BugReportName);
			mDetailsTextEntry.SetText(string.Empty);
		}
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
		if (mFocusNode == null)
		{
			SetInitialFocus();
		}
		if (mNameTextEntry != null && mDetailsTextEntry != null && mSubmitButton != null)
		{
			if (!string.IsNullOrEmpty(mNameTextEntry.Text) && !string.IsNullOrEmpty(mDetailsTextEntry.Text))
			{
				mSubmitButton.Enable(true);
			}
			else
			{
				mSubmitButton.Disable(true, false);
			}
		}
	}

	protected void HandleCloseButton(FUIButton button, FTouch touch)
	{
		CloseDialog();
	}

	protected void CloseDialog()
	{
		FApp.SoundManager.PlaySoundSet("UIButtonClose");
		TransitionOut(mDialog);
	}

	protected void HandleSubmitButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		SubmitBug();
		CloseDialog();
	}

	protected void SubmitBug()
	{
		if (mNameTextEntry != null && mDetailsTextEntry != null && !string.IsNullOrEmpty(mNameTextEntry.Text))
		{
			if (string.IsNullOrEmpty(mDetailsTextEntry.Text))
			{
				return;
			}
			SaveBugReporterName();
			FPluginRelay.ReportBug(mNameTextEntry.Text, mDetailsTextEntry.Text);
		}
	}

	protected void SetInitialFocus()
	{
		if (mNameTextEntry != null && mDetailsTextEntry != null)
		{
			SetFocus(string.IsNullOrEmpty(FApp.PlayerData.BugReportName) ? mNameTextEntry : mDetailsTextEntry);
		}
	}

	protected void SetFocus(FNode node)
	{
		if (mFocusNode != null)
		{
			Futile.Instance.RemoveFocusNode(mFocusNode);
		}
		mFocusNode = node;
		if (mFocusNode != null)
		{
			Futile.Instance.AddFocusNode(mFocusNode);
		}
	}

	protected int GetTabIndex(FNode node)
	{
		for (int i = 0; i < mTabNodes.Count; i++)
		{
			if (mTabNodes[i] == node)
			{
				return i;
			}
		}
		return -1;
	}

	public void HandleTab(FNode node, string text, bool forward)
	{
		int tabIndex = GetTabIndex(mFocusNode);
		if (tabIndex < 0)
		{
			SetInitialFocus();
			return;
		}
		int current = tabIndex;
		int step = forward ? 1 : -1;
		while (true)
		{
			int next = current + step;
			if (next == tabIndex)
			{
				return;
			}
			current = ((next >= 0) ? next : (mTabNodes.Count - 1));
			if (mTabNodes.Count <= next)
			{
				current = 0;
			}
			if (mTabNodes[current].CanGainFocus())
			{
				break;
			}
		}
		SetFocus(mTabNodes[current]);
	}

	public void HandleChildWantFocus(FNode childNode)
	{
		if (childNode != null && mTabNodes.Contains(childNode))
		{
			SetFocus(childNode);
		}
	}

	public void HandleEscape(FNode node, string text)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		if (node == mNameTextEntry)
		{
			mNameTextEntry.SetText(FApp.PlayerData.BugReportName);
		}
		else if (node == mDetailsTextEntry)
		{
			mDetailsTextEntry.SetText(string.Empty);
		}
		CloseDialog();
	}

	protected void HandleEnter(FNode node, string text)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		if (node != mNameTextEntry)
		{
			if (node != mCloseButton)
			{
				if (node != mSubmitButton)
				{
					return;
				}
				SubmitBug();
			}
			CloseDialog();
		}
		else if (!string.IsNullOrEmpty(text))
		{
			SaveBugReporterName();
		}
		else
		{
			mNameTextEntry.SetText(FApp.PlayerData.BugReportName);
		}
	}

	protected void SaveBugReporterName()
	{
		if (mNameTextEntry != null && !(mNameTextEntry.Text == FApp.PlayerData.BugReportName))
		{
			FApp.PlayerData.BugReportName = mNameTextEntry.Text;
			FApp.PlayerData.Save();
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
