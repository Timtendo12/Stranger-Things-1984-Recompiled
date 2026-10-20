using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageStringPicker @ 0x17aeb6c..0x17b02ec, Ghidra vaddr).
// Not reachable from the shipped game content (FHUD.StartStringPicker has no callers).
public class FPageStringPicker : FUIPage
{
	protected FUICollider mBackground;

	protected Vector2 mDialogSize = Vector2.zero;

	protected FNode mDialog;

	protected FSlicedSprite mDialogBackground;

	protected FSlicedSprite mDialogBackgroundBorder;

	protected FUIScrollListVertical mEntryList;

	protected List<FStringPickerEntry> mEntries = new List<FStringPickerEntry>();

	protected FLabel mTitle;

	protected FUIButton mBackButton;

	protected string mTitleText = FLoc.GetString("LevelPicker.TitlePregame");

	protected string mDefaultChoice = "Random";

	protected bool mAddRandomChoice = true;

	protected string mCurrentChoice = string.Empty;

	protected List<FPageStringPickerString> mChoices = new List<FPageStringPickerString>();

	[CompilerGenerated]
	private Action<string> m_mChoiceChangedEvent;

	protected static Vector2 mScrollSize = new Vector2(640f, 544f);

	protected Vector2 mEntrySize = new Vector2(mScrollSize.x - FUIScrollbar.cDefaultButtonSizeMobile.x, 160f);

	protected Vector2 mEntryMargin = new Vector2(12f, 8f);

	public Vector2 EntrySize
	{
		get
		{
			return mEntrySize;
		}
		set
		{
			mEntrySize = value;
		}
	}

	public Vector2 EntryMargin
	{
		get
		{
			return mEntryMargin;
		}
		set
		{
			mEntryMargin = value;
		}
	}

	public string TitleText
	{
		get
		{
			return mTitleText;
		}
		set
		{
			mTitleText = value;
			if (mTitle != null)
			{
				mTitle.Text = value;
			}
		}
	}

	public bool AddRandomChoice
	{
		get
		{
			return mAddRandomChoice;
		}
		set
		{
			mAddRandomChoice = value;
		}
	}

	public string DefaultChoice
	{
		get
		{
			return mDefaultChoice;
		}
		set
		{
			mDefaultChoice = value;
		}
	}

	public string CurrentChoice
	{
		get
		{
			return mCurrentChoice;
		}
		set
		{
			mCurrentChoice = value;
		}
	}

	protected event Action<string> mChoiceChangedEvent
	{
		add
		{
			m_mChoiceChangedEvent = (Action<string>)Delegate.Combine(m_mChoiceChangedEvent, value);
		}
		remove
		{
			m_mChoiceChangedEvent = (Action<string>)Delegate.Remove(m_mChoiceChangedEvent, value);
		}
	}

	public FPageStringPicker(string name)
		: base(name)
	{
		base.TransitionDialogInEaseType = GoEaseType.BackOut;
		base.TransitionDialogInTime = 0.25f;
		base.TransitionFadeInTime = 0.1f;
		base.TransitionDialogOutEaseType = GoEaseType.QuadIn;
		base.TransitionDialogOutTime = 0.15f;
		base.TransitionFadeOutTime = 0.1f;
		base.TransitionStyle = TransitionStyles.ScalePosition;
		base.Modal = true;
		base.SepiaUnder = true;
		mBackground = new FUICollider("StringPickerBackground", false);
		mBackground.SetParent(this);
		mDialog = new FNode("StringPickerDialog");
		mDialog.SetParent(this);
		mDialogBackground = new FSlicedSprite("UIBox001.png");
		mDialogBackground.SetParent(mDialog);
		mDialogBackgroundBorder = new FSlicedSprite("UIBorder001.png");
		mDialogBackgroundBorder.SetParent(mDialog);
		mTitle = new FLabel("DefaultStyle", mTitleText, 72f, true, true, false);
		mTitle.SetParent(mDialog);
		mEntryList = new FUIScrollListVertical("StringPickerEntryList", new Vector2(mScrollSize.x, mScrollSize.y), mEntrySize.y, true);
		mEntryList.SetParent(mDialog);
		mBackButton = new FUIButton("BackButton");
		mBackButton.SetParent(mDialog);
		mBackButton.AddHandleRelease(HandleBackButton);
		mBackButton.SetBackground("ButtonClose.png");
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		Vector2 size = Futile.Screen.Size;
		mBackground.DockInside(this, InnerDock.Center, size);
		mDialogSize = new Vector2(800f, Futile.Screen.Size.y - 200f);
		mDialog.DockInside(this, InnerDock.Center, mDialogSize);
		mDialogBackground.DockInside(mDialog, InnerDock.Center, mDialogSize);
		mDialogBackgroundBorder.DockInside(mDialog, InnerDock.Center, mDialogSize);
		mTitle.Color = FUI.Colors.UITitle;
		mTitle.DockInside(mDialog, InnerDock.Top, 480f, 72f, 0f, -32f);
		mEntryList.LayoutListContents();
		mEntryList.AddSignalDeselect(HandleDeselect, mEntrySize.x * 0.5f);
		mEntryList.DockCustom(mDialog, HorizSide.Left, 32f, mDialog, HorizSide.Right, -32f, mDialog, VertSide.Top, -160f, mDialog, VertSide.Bottom, 64f);
		mBackButton.DockInside(mDialog, InnerDock.TopLeft, 72f, 72f, 32f, -32f);
		mBackButton.ColliderScale = new Vector2(1.5f, 1.5f);
		mEntryList.LayoutListContents();
		MarkDirtyRecursive();
		mEntryList.SetScrollPosition(mEntryList.GetMinScrollX(), mEntryList.GetMinScrollY());
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		FillLevelEntries();
		mEntryList.ScrollToTop();
		TransitionIn(mDialog);
	}

	protected void FillLevelEntries()
	{
		mEntries.Clear();
		mEntryList.RemoveItems(true);
		if (mAddRandomChoice)
		{
			FStringPickerEntry fStringPickerEntry = new FStringPickerEntry(this, -1, "Random", FLoc.GetString("Map.Random.VisibleName"), FLoc.GetString("Map.Random.Details"), string.Empty, "Random" == mCurrentChoice, true);
			mEntryList.AddItem(fStringPickerEntry);
			fStringPickerEntry.DockInside(mEntryList, InnerDock.Center, mEntrySize.x, mEntrySize.y, 0f, 0f);
			mEntryList.AddItem(fStringPickerEntry);
			mEntries.Add(fStringPickerEntry);
		}
		for (int i = 0; i < mChoices.Count; i++)
		{
			FStringPickerEntry fStringPickerEntry2 = new FStringPickerEntry(this, i, mChoices[i].Name, mChoices[i].VisibleName, mChoices[i].Details, mChoices[i].Icon, mChoices[i].Name == mCurrentChoice, mChoices[i].Selectable);
			mEntryList.AddItem(fStringPickerEntry2);
			fStringPickerEntry2.DockInside(mEntryList, InnerDock.Center, mEntrySize.x, mEntrySize.y, 0f, 0f);
			mEntries.Add(fStringPickerEntry2);
		}
		mEntryList.LayoutListContents();
		MarkDirtyRecursive();
		mEntryList.SetScrollPosition(mEntryList.GetMinScrollX(), mEntryList.GetMinScrollY());
	}

	protected void HandleBackButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButtonClose");
		TransitionOut(mDialog);
	}

	protected void CloseDialog()
	{
		FApp.SoundManager.PlaySoundSet("UIButtonClose");
		TransitionOut(mDialog);
	}

	protected void HandleDeselect()
	{
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

	public void SelectEntry(int slotIndex, string name)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		mCurrentChoice = mDefaultChoice;
		if (slotIndex >= 0 && slotIndex < mChoices.Count)
		{
			if (!mChoices[slotIndex].Selectable)
			{
				return;
			}
			mCurrentChoice = mChoices[slotIndex].Name;
		}
		if (m_mChoiceChangedEvent != null)
		{
			m_mChoiceChangedEvent(mCurrentChoice);
		}
		TransitionOut(mDialog);
	}

	public void AddChoiceChangedEvent(Action<string> d)
	{
		mChoiceChangedEvent -= d;
		mChoiceChangedEvent += d;
	}

	public void ClearChoices()
	{
		mChoices.Clear();
	}

	public void AddChoice(string name, string visibleName, string details, string icon)
	{
		mChoices.Add(new FPageStringPickerString(name, visibleName, details, icon, true));
	}

	public void AddChoice(string name, string visibleName, string details, string icon, bool selectable)
	{
		mChoices.Add(new FPageStringPickerString(name, visibleName, details, icon, selectable));
	}
}
