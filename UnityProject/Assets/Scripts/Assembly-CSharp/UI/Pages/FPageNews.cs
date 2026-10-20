using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageNews @ 0x17a84d4..0x17a9760, Ghidra vaddr).
public class FPageNews : FUIPage
{
	public enum Modes
	{
		News = 0,
		BuildNotes = 1
	}

	protected static readonly Vector2 cScrollListSize = new Vector2(660f, 480f);

	protected static readonly Vector2 cScrollListItemSize = new Vector2(cScrollListSize.x - FUIScrollbar.cDefaultButtonSizeMobile.x, 200f);

	protected FUICollider mCollider;

	protected FNode mDialog;

	protected FSlicedSprite mBackground;

	protected FSlicedSprite mBackgroundBorder;

	protected FLabel mTitleLabel;

	protected FUIButton mCloseButton;

	protected FUIScrollListVertical mScrollList;

	public Modes Mode { get; set; }

	public FPageNews()
		: base("News")
	{
		Mode = Modes.BuildNotes;
		base.Modal = true;
		base.SepiaUnder = true;
		mCollider = new FUICollider("NewsCollider", false);
		mCollider.SetParent(this);
		mDialog = new FNode("NewsDialog");
		mDialog.SetParent(this);
		mBackground = new FSlicedSprite("UIBox001.png");
		mBackground.SetParent(mDialog);
		mBackgroundBorder = new FSlicedSprite("UIBorder001.png");
		mBackgroundBorder.SetParent(mDialog);
		mTitleLabel = new FLabel("DefaultStyle", FLoc.GetString("NewsDialog.Title"), 72f, true, true, false);
		mTitleLabel.SetParent(mDialog);
		mCloseButton = new FUIButton(base.Name + "CloseButton");
		mCloseButton.SetParent(mDialog);
		mCloseButton.SetBackground("ButtonClose.png");
		mCloseButton.AddHandleRelease(HandleCloseButton);
		mScrollList = new FUIScrollListVertical("ScoreScrollList", cScrollListSize, cScrollListItemSize.y, true);
		mScrollList.SetParent(mDialog);
		Mode = Modes.BuildNotes;
		AddNewsItemsToList();
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		Vector2 size = Futile.Screen.Size;
		mCollider.DockInside(this, InnerDock.Center, size);
		mDialog.DockInside(this, InnerDock.Center, 740f, 640f, 0f, 0f);
		mBackground.DockInside(mDialog, InnerDock.Center, 740f, 640f, 0f, 0f);
		mBackgroundBorder.DockInside(mDialog, InnerDock.Center, 740f, 640f, 0f, 0f);
		mTitleLabel.DockInside(mDialog, InnerDock.Top, 430f, 72f, 0f, -32f);
		mTitleLabel.Color = FUI.Colors.UITitle;
		mCloseButton.DockInside(mDialog, InnerDock.TopLeft, FUI.Buttons.BackButtonSize, FUI.Buttons.BackButtonOffset);
		mScrollList.DockInside(mDialog, InnerDock.Bottom, cScrollListSize, new Vector2(0f, 32f));
		for (int i = 0; i < mScrollList.NumItems; i++)
		{
			FNewsListItem fNewsListItem = mScrollList.GetItem(i) as FNewsListItem;
			if (fNewsListItem != null)
			{
				fNewsListItem.RefreshItemSize();
			}
		}
		mScrollList.LayoutListContents();
		MarkDirtyRecursive();
		mScrollList.SetScrollPosition(mScrollList.GetMinScrollX(), mScrollList.GetMinScrollY());
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		LayoutPage();
		TransitionIn(mDialog);
	}

	protected void HandleCloseButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButtonClose");
		TransitionOut(mDialog);
	}

	protected void CloseDialog()
	{
		FApp.SoundManager.PlaySoundSet("UIButtonClose");
		TransitionOut(mDialog);
	}

	protected void AddNewsItemsToList()
	{
		mScrollList.RemoveItems();
		if (Mode == Modes.News)
		{
			mTitleLabel.Text = FLoc.GetString("NewsDialog.Title");
			for (int i = 0; i < FNewsManager.GetNumNewsItems(); i++)
			{
				FNewsItem newsItem = FNewsManager.GetNewsItem(i);
				if (newsItem != null)
				{
					mScrollList.AddItem(new FNewsListItem(mScrollList, cScrollListItemSize.x, newsItem.Title, newsItem.Description, newsItem.URL));
				}
			}
			FNewsManager.GetMaxNewsItemID();
		}
		else
		{
			mTitleLabel.Text = FLoc.GetString("BuildNotesDialog.Title");
			mScrollList.AddItem(new FNewsListItem(mScrollList, cScrollListItemSize.x, FLoc.GetString("BuildNotesDialog.FoundersBuildNotesTitle"), FLoc.GetString("BuildNotesDialog.FoundersBuildNotesText"), null));
			for (int j = 0; j < FNewsManager.BuildNotes.Count; j++)
			{
				FBuildNotes fBuildNotes = FNewsManager.BuildNotes[j];
				if (fBuildNotes != null)
				{
					string title = ((!(fBuildNotes.DateTime != DateTime.MinValue)) ? FLoc.Format("BuildNotesDialog.Build", fBuildNotes.CoreBuildNumber) : FLoc.Format("BuildNotesDialog.BuildWithDate", fBuildNotes.CoreBuildNumber, fBuildNotes.DateTime));
					mScrollList.AddItem(new FNewsListItem(mScrollList, cScrollListItemSize.x, title, fBuildNotes.Text, null));
				}
			}
			int highestBuildNumber = FNewsManager.GetHighestBuildNumber();
			if (highestBuildNumber > FApp.PlayerData.HighestBuildNumberSeen)
			{
				FApp.PlayerData.HighestBuildNumberSeen = highestBuildNumber;
				FApp.PlayerData.Save();
			}
		}
		mScrollList.LayoutListContents();
		MarkDirtyRecursive();
		mScrollList.SetScrollPosition(mScrollList.GetMinScrollX(), mScrollList.GetMinScrollY());
	}

	public override bool HandleInput(float deltaTime)
	{
		bool keyDown = Input.GetKeyDown(KeyCode.Escape);
		if (keyDown)
		{
			CloseDialog();
		}
		else
		{
			bool isAltDown = FApp.HUD.IsAltDown;
		}
		return keyDown;
	}
}
