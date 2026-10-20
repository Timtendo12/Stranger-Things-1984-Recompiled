using System;
using System.Collections.Generic;
using UnityEngine;

public class FPanelHistoryLog : FNode
{
	public const float cCommandLineHeight = 36f;

	public const float cDefaultTimeout = 5f;

	public static readonly Vector2 cDefaultSize;

	public static readonly Vector2 cDefaultMargin;

	public static readonly Vector2 cHistorySize;

	public static readonly Vector2 cCommandLineSize;

	public static readonly Vector2 cCommandLineEntrySize;

	protected FNode mPanelHistoryLogParent;

	protected FSlicedSprite mBackground;

	protected FSlicedSprite mBackgroundBorder;

	protected FUIScrollList mHistoryList;

	protected List<FPanelHistoryItemData> mHistoryItems;

	protected int mHistoryIndex;

	protected float mTimeout;

	protected FTextEntry mCommandLineEntry;

	protected FSlicedSprite mCommandLineBackground;

	protected bool mTurningOff;

	protected List<FPanelHistoryItemData> mDialogEntries;

	protected FPanelHistoryItemData mLastEntry;

	protected FEntityActionSequence mCurWaitSeq;

	protected bool mLastChatContinueIsTimer;

	protected DateTime mLastChatContinueTime;

	protected bool WaitForChat { get; set; }

	public bool IsChatOn => false;

	public bool IsTurningOff => false;

	public bool HackEnterHandled { get; set; }

	public FPanelHistoryLog(FNode parentNode)
	{
	}

	public void PopulateHistory()
	{
	}

	public override void HandleAddedToStage()
	{
	}

	public override void HandleRemovedFromStage()
	{
	}

	protected override void HandleUpdate()
	{
	}

	public void HandleCommandLineEnter(FNode node, string text)
	{
	}

	public void HandleCommandLineEscape(FNode node, string text)
	{
	}

	public void HandleCommandLineBackquote(FNode node, string text)
	{
	}

	public void AddOutput(string text)
	{
	}

	public void AddOutput(string text, Color color, bool bold)
	{
	}

	public void AddChat(string text, Color color)
	{
	}

	public void AddHistoryItem(string text, FPanelHistoryItem.Types textType, Color color, bool indent, bool bold)
	{
	}

	protected void AddToList(FPanelHistoryItemData data)
	{
	}

	public void ClearHistory()
	{
	}

	public void ClearCommandLine()
	{
	}

	public void TurnOnLeaveChatOpen()
	{
	}

	public void TurnOn(bool chat, bool showBackground)
	{
	}

	public void TurnOn(bool chat, bool showBackground, float timeout)
	{
	}

	public void TurnOff()
	{
	}

	public void PageUp()
	{
	}

	public void PageDown()
	{
	}

	public void StartGame()
	{
	}

	public void DoPauseGameUI()
	{
	}

	public void DoResumeGameUI()
	{
	}
}
