using System;
using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageTabbed @ 0x17b02ec..0x17b14f4, Ghidra vaddr).
// Base class of the store/spindle pages; no subclass exists in the recovered game content.
public class FPageTabbed : FPageShell
{
	public static readonly Vector2 cSpindleMargin = new Vector2(32f, 16f);

	public static readonly Vector2 cTabMargin = new Vector2(32f, 0f);

	public static readonly Vector2 cButtonTabSize = new Vector2(214f, 174f);

	public static readonly Vector2 cButtonTabGlowSize = new Vector2(304f, 228f);

	public static readonly Vector2 cButtonTabSizeSelected = new Vector2(214f, 226f);

	public static readonly Vector2 cButtonTabGlowSizeSelected = new Vector2(304f, 296f);

	public static readonly Vector2 cButtonTabTextOffset = new Vector2(0f, 32f);

	protected FNode mWholeNode;

	protected FNode mContents;

	protected Vector2 mWholeNodeSize = Vector2.zero;

	protected Vector2 mWholeNodePosition = Vector2.zero;

	protected FSlicedSprite mWholeNodeBackground;

	protected bool mSetNodePositions;

	protected int mMode = -1;

	protected int mNextAddedToStageMode = -1;

	protected FNodeStack mButtonTabStack;

	protected List<FUIButton> mButtonTabs = new List<FUIButton>();

	protected FPageOkDialog mWaitDialog;

	protected TransitionStyles mTransitionInStyle = TransitionStyles.Left;

	protected TransitionStyles mTransitionOutStyle = TransitionStyles.Left;

	public TransitionStyles TransitionInStyle
	{
		get
		{
			return mTransitionInStyle;
		}
		set
		{
			mTransitionInStyle = value;
		}
	}

	public TransitionStyles TransitionOutStyle
	{
		get
		{
			return mTransitionOutStyle;
		}
		set
		{
			mTransitionOutStyle = value;
		}
	}

	public int Mode => mMode;

	public FPageTabbed(string name, int defaultTabMode)
		: base(name, null, useCollider: false)
	{
		base.Modal = true;
		mMode = defaultTabMode;
		SetDefaultTransitionInFade();
		mWholeNode = new FNode(base.Name + ".WholeNode");
		mWholeNode.SetParent(mDialog);
		mWholeNodeBackground = new FSlicedSprite(base.Name + ".CenterNode.Background", "Backgrounds/SpindlePage");
		mWholeNodeBackground.SetParent(mWholeNode);
		mContents = new FNode(base.Name + ".Contents");
		mContents.SetParent(mWholeNodeBackground);
		mButtonTabStack = new FNodeStack(base.Name + ".ButtonTabStack", FNodeStack.Modes.CenteredX, cTabMargin.x);
		mButtonTabStack.SetParent(mWholeNode);
		AddTabs();
	}

	protected virtual void AddTabs()
	{
	}

	protected virtual void OnSetMode(int prevMode, int currentMode)
	{
		LayoutTabs();
		LayoutContent();
	}

	public virtual bool SetMode(string modeString, bool immediate)
	{
		return false;
	}

	public void SetMode(int newMode)
	{
		FLog.UnityLog(string.Format("FPageStore.SetMode from Mode({0}) to Mode({1})", mMode.ToString(), newMode.ToString()));
		int prevMode = mMode;
		mMode = newMode;
		for (int i = 0; i < mButtonTabs.Count; i++)
		{
			if (mButtonTabs[i].Data == mMode)
			{
				mButtonTabs[i].HandleGainFocus();
			}
			else
			{
				mButtonTabs[i].HandleLoseFocus();
			}
		}
		OnSetMode(prevMode, mMode);
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		float num = Futile.Screen.Width / 1080f;
		mWholeNodeSize = new Vector2(900f * num, 1920f * num);
		mWholeNodeBackground.ExpandToFill(mWholeNode, 0f, 0f);
		mContents.DockCustom(mWholeNodeBackground, HorizSide.Left, num * 72f, mWholeNodeBackground, HorizSide.Right, num * -72f, mWholeNodeBackground, VertSide.Top, 0f - cButtonTabSize.y, mWholeNodeBackground, VertSide.Bottom, 64f);
		LayoutTabs();
		LayoutContent();
		MarkDirtyRecursive();
	}

	protected virtual void LayoutContent()
	{
	}

	protected void LayoutTabs()
	{
		if (mButtonTabs == null || mButtonTabStack == null)
		{
			return;
		}
		int count = mButtonTabs.Count;
		mButtonTabStack.DockInside(mWholeNode, InnerDock.TopLeft, cButtonTabSize.x * (float)count + cTabMargin.x * (float)(count - 1), cButtonTabSizeSelected.y, 96f, 4f);
		mButtonTabStack.ClearStack(false);
		for (int i = 0; i < mButtonTabs.Count; i++)
		{
			if (mMode == i)
			{
				mButtonTabStack.StackChild(mButtonTabs[mMode], cButtonTabSizeSelected, Vector2.zero);
				mButtonTabs[mMode].SetTextExtents(cButtonTabSize, cButtonTabTextOffset);
				mButtonTabs[mMode].SetFocusSpriteExtent(cButtonTabGlowSizeSelected, Vector2.zero);
			}
			else
			{
				mButtonTabStack.StackChild(mButtonTabs[i], cButtonTabSize, new Vector2(0f, (cButtonTabSizeSelected.y - cButtonTabSize.y) * 0.5f));
				mButtonTabs[i].SetTextExtents(cButtonTabSize, cButtonTabTextOffset);
				mButtonTabs[i].SetFocusSpriteExtent(cButtonTabGlowSize, Vector2.zero);
			}
		}
		mButtonTabStack.MarkDirtyRecursive();
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		if (mNextAddedToStageMode > -1)
		{
			SetMode(mNextAddedToStageMode);
			mNextAddedToStageMode = -1;
		}
	}

	protected void ClearWaitDialog()
	{
		if (mWaitDialog != null)
		{
			mWaitDialog.Close();
			mWaitDialog = null;
		}
	}

	protected void EndWaitWithMessage(string message)
	{
		if (mWaitDialog != null)
		{
			mWaitDialog.SetButton0Visible(true);
			mWaitDialog.SetButton0Text(FLoc.GetString("OkButton"));
			mWaitDialog.AndroidBackButton = FPageOkDialog.AndroidBackButtonType.Button0;
			mWaitDialog.SetBodyText(message);
			mWaitDialog.TurnOffSpinner();
			mWaitDialog = null;
		}
	}

	protected void ShowWaitDialog(string title, string body, Action cancelPress = null)
	{
		if (mWaitDialog == null)
		{
			mWaitDialog = FApp.HUD.OpenWaitDialog(title, body, cancelPress, null, cancelPress, TransitionStyles.Fade, cancelPress != null);
		}
	}

	public void OnPlayerMetaStateReceived()
	{
		LayoutContent();
		MarkDirtyRecursive();
	}

	protected override void TransitionIn(FNode dialog)
	{
		// the shipped code only does this when the node's dirty flag (FNode+0x103, protected internal in firstpass) is set
		mWholeNode.ResizeConstraintsInGraph();
		if (!mSetNodePositions)
		{
			mWholeNodePosition = mWholeNode.Position;
			mSetNodePositions = true;
		}
		base.Modal = true;
		Vector2 position;
		if (mTransitionInStyle == TransitionStyles.Bottom)
		{
			position = new Vector2(mWholeNodePosition.x, mWholeNodePosition.y - Futile.Screen.Height + -1f);
		}
		else if (mTransitionInStyle == TransitionStyles.Top)
		{
			position = new Vector2(mWholeNodePosition.x, mWholeNodePosition.y + Futile.Screen.Height + 1f);
		}
		else
		{
			position = new Vector2(mWholeNodePosition.x - mWholeNode.Extents.width + -1f, mWholeNodePosition.y);
		}
		mWholeNode.Position = position;
		// 0x17b1178: setEaseType(0) = Linear (duration 0.3 @0x899cd0)
		GoTweenConfig config = new GoTweenConfig().vector2Prop("Position", mWholeNodePosition, false).setEaseType(GoEaseType.Linear).onComplete(OnCompleteTransitionIn);
		Go.to(mWholeNode, 0.3f, config);
	}

	protected override void TransitionOut(FNode dialog)
	{
		// the shipped code only does this when the node's dirty flag (FNode+0x103, protected internal in firstpass) is set
		mWholeNode.ResizeConstraintsInGraph();
		base.TransitionOutInProgress = true;
		Vector2 vector;
		if (mTransitionOutStyle == TransitionStyles.Bottom)
		{
			vector = new Vector2(mWholeNodePosition.x, mWholeNodePosition.y - Futile.Screen.Height + -1f);
		}
		else if (mTransitionOutStyle == TransitionStyles.Top)
		{
			vector = new Vector2(mWholeNodePosition.x, mWholeNodePosition.y + Futile.Screen.Height + 1f);
		}
		else
		{
			vector = new Vector2(mWholeNodePosition.x - mWholeNode.Extents.width + -1f, mWholeNodePosition.y);
		}
		GoTweenConfig config = new GoTweenConfig().vector2Prop("Position", vector, false).setEaseType(GoEaseType.Linear).onComplete(OnCompleteTransitionOut); // 0x17b13a8: setEaseType(0) = Linear
		Go.to(mWholeNode, 0.2f, config);
	}
}
