using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageAbout @ 0x179796c..0x1798c1c, Ghidra vaddr).
public class FPageAbout : FUIPage
{
	protected static readonly Vector2 cDialogSize = new Vector2(860f, 640f);

	protected static readonly Vector2 cTitleLabelSize = new Vector2(860f, 72f);

	protected static readonly Vector2 cTitleLabelOffset = new Vector2(0f, -32f);

	protected static readonly Vector2 cScrollListSize = new Vector2(800f, 480f);

	protected static readonly Vector2 cScrollListOffset = new Vector2(0f, 32f);

	protected static readonly Vector2 cScrollListItemSize = new Vector2(cScrollListSize.x - FUIScrollbar.cDefaultButtonSizeMobile.x, 200f);

	protected FUICollider mCollider;

	protected FNode mDialog;

	protected FSlicedSprite mBackground;

	protected FUIButton mCloseButton;

	protected FUIButton mLinkedPageButton;

	protected FUIScrollListVertical mScrollList;

	protected string mCategory = string.Empty;

	public FPageAbout(string name)
		: base(name)
	{
		mCategory = "About";
		base.Modal = true;
		base.DarkenLower = true;
		mCollider = new FUICollider("AboutCollider", false);
		mCollider.SetParent(this);
		mDialog = new FNode("AboutDialog");
		mDialog.SetParent(this);
		mBackground = new FSlicedSprite("Dialog003.png");
		mBackground.SetParent(mDialog);
		mBackground.ExpandToFill(mDialog, 0f, 0f);
		mCloseButton = new FUIButton(Name + ".CloseButton");
		mCloseButton.SetParent(mDialog);
		mCloseButton.SetBackground("ButtonClose.png");
		mCloseButton.AddHandleRelease(HandleCloseButton);
		mLinkedPageButton = new FUIButton(Name + ".LinkedPageButton");
		mLinkedPageButton.SetParent(mDialog);
		mLinkedPageButton.SetBackground("Button004.png");
		mLinkedPageButton.SetForegroundColor(new Color(1f, 0f, 0f, 1f));
		mLinkedPageButton.SetText(FLoc.GetString("About.LinkedPage.PageLegal"), "DefaultStyle", 24f, 14f);
		mLinkedPageButton.AddHandleRelease(HandleLinkedPageButton);
		mScrollList = new FUIScrollListVertical("AboutScrollList", cScrollListSize, cScrollListItemSize.y, true);
		mScrollList.SetParent(mDialog);
		base.TransitionStyle = TransitionStyles.Fade;
		AddAboutItemsToList();
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		mCollider.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		mDialog.DockInside(this, InnerDock.Center, cDialogSize.x, cDialogSize.y, 0f, 0f);
		mCloseButton.DockInside(mDialog, InnerDock.TopLeft, FUI.Buttons.BackButtonSize, FUI.Buttons.BackButtonOffset);
		mScrollList.DockInside(mDialog, InnerDock.Bottom, cScrollListSize, cScrollListOffset);
		mLinkedPageButton.DockInside(mDialog, InnerDock.TopRight, 300f, 72f, 0f - FUI.Buttons.BackButtonOffset.x, FUI.Buttons.BackButtonOffset.y);
		for (int i = 0; i < mScrollList.NumItems; i++)
		{
			FAboutListItem item = mScrollList.GetItem(i) as FAboutListItem;
			if (item != null)
			{
				item.RefreshItemSize();
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

	protected void HandleLinkedPageButton(FUIButton button, FTouch touch)
	{
		Application.OpenURL("http://bonusxp.com/sttg-softwarelicenses/");
	}

	protected void HandlePrivacyPolicyButton(FUIButton button, FTouch touch)
	{
		Application.OpenURL("http://bonusxp.com/privacy-policy/");
	}

	protected void CloseDialog()
	{
		FApp.SoundManager.PlaySoundSet("UIButtonClose");
		TransitionOut(mDialog);
	}

	protected void AddAboutItemsToList()
	{
		mScrollList.RemoveItems(true);
		XmlElement root = FUtil.LoadXmlAsset("Data/TextNuggets");
		// port guard: the shipped code dereferences root unconditionally
		foreach (XmlNode node in (root != null) ? (System.Collections.IEnumerable)root.ChildNodes : new XmlNode[0])
		{
			if (node.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			XmlElement element = node as XmlElement;
			string category = element.GetAttrib("Category");
			if (!string.IsNullOrEmpty(category) && string.Compare(category, mCategory, true) == 0)
			{
				string title = element.GetAttrib("Title");
				string locTitle = element.GetAttrib("LocTitle");
				if (!title.Valid() && locTitle.Valid())
				{
					title = FLoc.GetString(locTitle);
				}
				string text = element.GetAttrib("Text");
				string locText = element.GetAttrib("LocText");
				if (!text.Valid() && locText.Valid())
				{
					text = FLoc.GetString(locText);
				}
				if (!string.IsNullOrEmpty(text))
				{
					text = Regex.Unescape(text);
				}
				FAboutListItem item = new FAboutListItem(cScrollListItemSize, title, text);
				mScrollList.AddItem(item);
			}
		}
		mScrollList.LayoutListContents();
		MarkDirtyRecursive();
		mScrollList.SetScrollPosition(mScrollList.GetMinScrollX(), mScrollList.GetMinScrollY());
	}

	public override bool HandleInput(float deltaTime)
	{
		bool escape = Input.GetKeyDown(KeyCode.Escape);
		if (escape)
		{
			CloseDialog();
		}
		else
		{
			bool altDown = FApp.HUD.IsAltDown;
		}
		return escape;
	}
}
