using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPanelConsole @ 0x17b1b1c..0x17b3af4, Ghidra vaddr).
// Developer-only debug console; PageDebug is not created in the shipped UI (FHUD never assigns the debug stage), kept for API completeness.
public class FPanelConsole : FNode
{
	public const float cDefaultHeight = 0.3f;

	public const float cDefaultAlpha = 1f;

	public const float cCommandLineHeight = 36f;

	public static readonly Vector2 cDefaultMargin = new Vector2(8f, 8f);

	protected Vector2 mSize = Vector2.zero;

	protected FSlicedSprite mBackground;

	protected Vector2 mHistorySize = Vector2.zero;

	protected FSlicedSprite mHistoryBackground;

	protected FUIScrollList mHistoryList;

	protected int mHistoryIndex = -1;

	protected Vector2 mCommandLineSize = Vector2.zero;

	protected FTextEntry mCommandLineEntry;

	protected FSlicedSprite mCommandLineBackground;

	protected float mConsoleHeight = 0.3f;

	protected float mConsoleAlpha;

	protected FNodeStack mButtonStack;

	protected Vector2 mButtonStackSize = Vector2.zero;

	protected FUIButton mCloseButton;

	protected FUIButton mNextCommandButton;

	protected FUIButton mPreviousCommandButton;

	protected FUIButton mExecuteCommandButton;

	protected FUIButton mResetSaveButton;

	protected FUIButton mSaveSlotsButton;

	protected bool IsOutputting;

	public Vector2 Size
	{
		get
		{
			return mSize;
		}
		set
		{
			mSize = value;
		}
	}

	public float ConsoleHeight
	{
		get
		{
			return mConsoleHeight;
		}
		set
		{
			mConsoleHeight = value;
		}
	}

	public float ConsoleAlpha
	{
		get
		{
			return mConsoleAlpha;
		}
		set
		{
			mConsoleAlpha = value;
		}
	}

	public FPanelConsole()
		: base("PanelConsole")
	{
		mSize = new Vector2(Futile.Screen.Width, Futile.Screen.Height * mConsoleHeight);
		mButtonStackSize = new Vector2(Futile.Screen.Width - (cDefaultMargin.x + cDefaultMargin.x), 64f);
		mHistorySize = new Vector2(Futile.Screen.Width + cDefaultMargin.x * -2f, mSize.y - 64f - 36f + cDefaultMargin.y * -4f);
		mCommandLineSize = new Vector2(Futile.Screen.Width - (cDefaultMargin.x + cDefaultMargin.x), 36f);
		mBackground = new FSlicedSprite("WhiteSquare.png");
		mBackground.SetParent(this);
		mBackground.Color = new Color(0.3f, 0.3f, 0.3f, 1f);
		mButtonStack = new FNodeStack("ConsoleButtonStack", FNodeStack.Modes.FromLeft, 32f);
		mButtonStack.SetParent(this);
		mCloseButton = AllocateConsoleButton(Name + ".CloseButton", "CloseButton", HandleCloseButton);
		mNextCommandButton = AllocateConsoleButton(Name + ".NextCommandButton", "NextButton", HandleNextCommandButton);
		mPreviousCommandButton = AllocateConsoleButton(Name + ".PrevCommandButton", "PrevButton", HandlePreviousCommandButton);
		mExecuteCommandButton = AllocateConsoleButton(Name + ".ExecuteCommandButton", "ExecuteButton", HandleExecuteCommandButton);
		mResetSaveButton = AllocateConsoleButton(Name + ".EditConfig", "EditConfigButton", HandleEditConfigButton);
		mSaveSlotsButton = AllocateConsoleButton(Name + ".ResetSaveButton", "SaveSlotsButton", HandleSaveSlotsButton);
		mHistoryBackground = new FSlicedSprite("BlackSquare.png");
		mHistoryBackground.SetParent(this);
		mHistoryList = new FUIScrollListVertical("ServerListList", mHistorySize, 30f, true);
		mHistoryList.SetParent(mHistoryBackground);
		mCommandLineBackground = new FSlicedSprite("BlackSquare.png");
		mCommandLineBackground.SetParent(this);
		mCommandLineEntry = new FTextEntry("PanelConsoleCommandLineEntry", "FixedWidthStroke", 20f, HorizSide.Left, VertSide.CenterY, 256, false, true);
		mCommandLineEntry.AddHandleEnter(HandleCommandLineEnter);
		mCommandLineEntry.AddHandleEscape(HandleCommandLineEscape);
		mCommandLineEntry.AddHandleBackquote(HandleCommandLineBackquote);
		mCommandLineEntry.SetParent(mCommandLineBackground);
		mBackground.DockInside(this, InnerDock.Center, mSize);
		mButtonStack.DockInside(this, InnerDock.Top, mButtonStackSize.x, mButtonStackSize.y, cDefaultMargin.x, 0f - cDefaultMargin.y);
		mHistoryBackground.DockInside(this, InnerDock.Top, mHistorySize.x, mHistorySize.y, 0f, 0f - mButtonStackSize.y - (cDefaultMargin.y + cDefaultMargin.y));
		mHistoryList.ExpandToFill(mHistoryBackground, 0f, 0f);
		mCommandLineBackground.DockInside(this, InnerDock.Bottom, mCommandLineSize.x, mCommandLineSize.y, 0f, cDefaultMargin.y);
		mCommandLineEntry.ExpandToFill(mCommandLineBackground, 0f, 0f);
		mHistoryList.LayoutListContents();
		mButtonStack.MarkDirtyRecursive();
	}

	private FUIButton AllocateConsoleButton(string name, string locKey, System.Action<FUIButton, FTouch> handler)
	{
		FUIButton button = new FUIButton(name);
		button.SetBackground("UIBox001.png");
		button.SetText(FLoc.GetString(locKey), "FixedWidthStroke", 48f, 0f);
		button.AddHandleRelease(handler);
		mButtonStack.StackChild(button, new Vector2(128f, 64f));
		return button;
	}

	public void PopulateHistory()
	{
		if (mHistoryList == null)
		{
			return;
		}
		mHistoryList.RemoveItems();
		if (FConsoleCore.History != null)
		{
			foreach (FConsoleHistoryEntry entry in FConsoleCore.History)
			{
				mHistoryList.AddItem(new FConsoleHistoryItem(mSize, entry.Text, entry.TextType));
			}
		}
		mHistoryList.LayoutListContents();
		mHistoryList.MarkDirtyRecursive();
		mHistoryList.ScrollToBottom();
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		ClearCommandLine();
		PopulateHistory();
		mHistoryList.ScrollToBottom();
		FConsoleCore.OutputEvent -= HandleOutput;
		FConsoleCore.OutputEvent += HandleOutput;
		FConsoleCore.WarningEvent -= HandleOutput;
		FConsoleCore.WarningEvent += HandleOutput;
		FConsoleCore.ErrorEvent -= HandleOutput;
		FConsoleCore.ErrorEvent += HandleOutput;
		FConsoleCore.CommandExecutedEvent -= HandleCommandExecuted;
		FConsoleCore.CommandExecutedEvent += HandleCommandExecuted;
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		FConsoleCore.OutputEvent -= HandleOutput;
		FConsoleCore.WarningEvent -= HandleOutput;
		FConsoleCore.ErrorEvent -= HandleOutput;
		FConsoleCore.CommandExecutedEvent -= HandleCommandExecuted;
	}

	public void HandleCommandLineEnter(FNode node, string text)
	{
		if (text.Valid())
		{
			FConsoleCore.Execute(text, true);
			FApp.PlayerData.AddConsoleCommand(text);
			FApp.PlayerData.Save();
			ClearCommandLine();
		}
	}

	public void HandleCommandLineEscape(FNode node, string text)
	{
		mCommandLineEntry.SetText(string.Empty);
		mHistoryIndex = -1;
	}

	public void HandleCommandLineBackquote(FNode node, string text)
	{
	}

	public void HandleOutput(string text)
	{
		if (!IsOutputting)
		{
			IsOutputting = true;
			PopulateHistory();
			IsOutputting = false;
		}
	}

	public void HandleCommandExecuted(string text)
	{
		PopulateHistory();
	}

	public void ClearCommandLine()
	{
		mCommandLineEntry.SetText(string.Empty);
		mHistoryIndex = -1;
	}

	public void SelectPreviousCommand()
	{
		System.Collections.Generic.List<string> commands = FApp.PlayerData.ConsoleCommands;
		if (commands.Count < 1)
		{
			ClearCommandLine();
			return;
		}
		int index = mHistoryIndex;
		if (index < 2 || index == -1)
		{
			index = commands.Count;
		}
		mHistoryIndex = index - 1;
		mCommandLineEntry.SetText(commands[index - 1]);
	}

	public void SelectNextCommand()
	{
		System.Collections.Generic.List<string> commands = FApp.PlayerData.ConsoleCommands;
		if (commands.Count < 1)
		{
			ClearCommandLine();
			return;
		}
		int next = 0;
		if (mHistoryIndex >= 0)
		{
			mHistoryIndex++;
			if (mHistoryIndex < commands.Count)
			{
				next = mHistoryIndex;
			}
		}
		mHistoryIndex = next;
		mCommandLineEntry.SetText(commands[next]);
	}

	protected void HandleCloseButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		FApp.HUD.PageDebug.ToggleConsole();
	}

	protected void HandleNextCommandButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		SelectNextCommand();
	}

	protected void HandlePreviousCommandButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		SelectPreviousCommand();
	}

	protected void HandleExecuteCommandButton(FUIButton button, FTouch touch)
	{
		FApp.SoundManager.PlaySoundSet("UIButton");
		HandleCommandLineEnter(mCommandLineEntry, mCommandLineEntry.Text);
	}

	protected void HandleSaveSlotsButton(FUIButton button, FTouch touch)
	{
		FApp.HUD.PushPage(FApp.HUD.PageSaveSlots, true, false);
	}

	protected void HandleEditConfigButton(FUIButton button, FTouch touch)
	{
	}
}
