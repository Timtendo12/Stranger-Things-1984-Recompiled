using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageCredits @ 0x179d400..0x179e4bc, Ghidra vaddr).
public class FPageCredits : FUIPage
{
	protected static readonly Vector2 cDialogSize = new Vector2(740f, 640f);

	protected static readonly Vector2 cScrollListSize = new Vector2(660f, 480f);

	protected static readonly Vector2 cScrollListOffset = new Vector2(0f, 32f);

	protected static readonly Vector2 cScrollListItemSize = new Vector2(cScrollListSize.x - FUIScrollbar.cDefaultButtonSizeMobile.x, 200f);

	protected static readonly Vector2 cTitleLabelSize = new Vector2(740f, 72f);

	protected static readonly Vector2 cTitleLabelOffset = new Vector2(0f, -32f);

	protected FUICollider mCollider;

	protected FNode mDialog;

	protected FSlicedSprite mBackground;

	protected FSlicedSprite mBackgroundBorder;

	protected FLabel mTitleLabel;

	protected FUIButton mCloseButton;

	protected FUIScrollListVertical mScrollList;

	public FPageCredits(string name)
		: base(name)
	{
		base.Modal = true;
		base.SepiaUnder = true;
		mCollider = new FUICollider("CreditsCollider", false);
		mCollider.SetParent(this);
		mDialog = new FNode("CreditsDialog");
		mDialog.SetParent(this);
		mBackground = new FSlicedSprite("UIBox001.png");
		mBackground.SetParent(mDialog);
		mBackground.ExpandToFill(mDialog, 0f, 0f);
		mBackgroundBorder = new FSlicedSprite("UIBorder001.png");
		mBackgroundBorder.SetParent(mDialog);
		mBackgroundBorder.ExpandToFill(mDialog, 0f, 0f);
		mTitleLabel = new FLabel("DefaultStyle", FLoc.GetString("Credits.Title"), 72f, true, true, false);
		mTitleLabel.Color = FUI.Colors.UITitle;
		mTitleLabel.Justify(Sides.CenterX, Sides.CenterY);
		mTitleLabel.Text = FLoc.GetString("Credits.Title");
		mTitleLabel.SetParent(mDialog);
		mCloseButton = new FUIButton(base.Name + "CloseButton");
		mCloseButton.SetParent(mDialog);
		mCloseButton.SetBackground("ButtonClose.png");
		mCloseButton.AddHandleRelease(HandleCloseButton);
		mScrollList = new FUIScrollListVertical("CreditsScrollList", cScrollListSize, cScrollListItemSize.y, true);
		mScrollList.SetParent(mDialog);
		base.TransitionStyle = TransitionStyles.Fade;
		AddCreditsItemsToList();
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		mCollider.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		mDialog.DockInside(this, InnerDock.Center, cDialogSize);
		mTitleLabel.DockInside(mDialog, InnerDock.Top, cTitleLabelSize, cTitleLabelOffset);
		mCloseButton.DockInside(mDialog, InnerDock.TopLeft, FUI.Buttons.BackButtonSize, FUI.Buttons.BackButtonOffset);
		mScrollList.DockInside(mDialog, InnerDock.Bottom, cScrollListSize, cScrollListOffset);
		for (int i = 0; i < mScrollList.NumItems; i++)
		{
			FCreditsListItem fCreditsListItem = mScrollList.GetItem(i) as FCreditsListItem;
			if (fCreditsListItem != null)
			{
				fCreditsListItem.RefreshItemSize();
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

	private void AddCreditsItemsToList()
	{
		mScrollList.RemoveItems(true);
		XmlElement xmlElement = FUtil.LoadXmlAsset("Data/TextNuggets");
		if (xmlElement != null)
		{
			foreach (XmlNode childNode in xmlElement.ChildNodes)
			{
				if (childNode.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				XmlElement xmlElement2 = childNode as XmlElement;
				string attrib = xmlElement2.GetAttrib("Category");
				if (!string.IsNullOrEmpty(attrib) && string.Compare(attrib, "Credit", true) == 0)
				{
					string text = xmlElement2.GetAttrib("Title");
					string attrib2 = xmlElement2.GetAttrib("LocTitle");
					if (!text.Valid() && attrib2.Valid())
					{
						text = FLoc.GetString(attrib2);
					}
					string text2 = xmlElement2.GetAttrib("Text");
					string attrib3 = xmlElement2.GetAttrib("LocText");
					if (!text2.Valid() && attrib3.Valid())
					{
						text2 = FLoc.GetString(attrib3);
					}
					if (!string.IsNullOrEmpty(text2))
					{
						text2 = Regex.Unescape(text2);
					}
					mScrollList.AddItem(new FCreditsListItem(cScrollListItemSize, text, text2));
				}
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
