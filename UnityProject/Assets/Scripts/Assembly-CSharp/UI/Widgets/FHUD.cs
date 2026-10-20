using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FHUD @ 0x1711d78..0x1716fbc, Ghidra vaddr).
public class FHUD : MonoBehaviour
{
	protected FStage mUIStage;

	protected FStage mDebugStage;

	protected List<FUIPage> mPageStack = new List<FUIPage>();

	protected bool mInitialized;

	protected FNode mRainFX;

	[CompilerGenerated]
	private Action<float> m_mHandleInputAction;

	protected List<FInputHandlerInterface> mInputHandlerOrderedList = new List<FInputHandlerInterface>(64);

	protected bool mIsLoading;

	protected List<string> mShownDialogs = new List<string>();

	protected List<string> mClosedDialogs = new List<string>();

	protected List<string> mCompletedCameraMoves = new List<string>();

	private List<FSingleTouchableInterface> mScratchpadListTouchableInterface = new List<FSingleTouchableInterface>(64);

	private List<FInputHandlerInterface> mScratchpadListInputHandler = new List<FInputHandlerInterface>(64);

	private Queue<FNode> mScratchpadQueue = new Queue<FNode>(32);

	private Stack<FNode> mScratchpadStack = new Stack<FNode>(32);

	public FPageLegendWorld PageLegendWorld { get; set; }

	public FPageLegendMainMenu PageLegendMainMenu { get; set; }

	public FPageCharacterPicker PageCharacterPicker { get; set; }

	public FPageInventory PageInventory { get; set; }

	public FPagePause PagePause { get; set; }

	public FPageDebug PageDebug { get; set; }

	public FPageLoad PageLoad { get; set; }

	public FPageKeyboard PageKeyboard { get; set; }

	public FPageBugReport PageBugReport { get; set; }

	public FPageWorldMap PageWorldMap { get; set; }

	public FPageSaveSlots PageSaveSlots { get; set; }

	public FPageDungeonComplete PageDungeonComplete { get; set; }

	public FPageGameModeChoice PageGameModeChoice { get; set; }

	public FPageTheaterWorld PageTheaterWorld { get; set; }

	public FPagePregameOptions PagePregameOptions { get; set; }

	public FPageAbout PageAbout { get; set; }

	public FPageLegal PageLegal { get; set; }

	public FPageLoadingScreen PageLoadingScreen { get; set; }

	public FPageBlackScreen PageBlackScreen { get; set; }

	public FPageSaveRecovery PageSaveRecovery { get; set; }


	public FUIPage TopPage => (mPageStack != null && mPageStack.Count > 0) ? mPageStack[mPageStack.Count - 1] : null;

	public List<FUIPage> PageStack => mPageStack;

	public bool IsLoading => mIsLoading;

	public FStage UIStage => mUIStage;

	public bool Initialized => mInitialized;

	public bool Landscape => Futile.Screen.IsLandscape;

	public bool Portrait => Futile.Screen.IsPortrait;

	public bool IsCommandDown => Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);

	public bool IsShiftDown => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

	public bool IsLeftShiftDown => Input.GetKey(KeyCode.LeftShift);

	public bool IsRightShiftDown => Input.GetKey(KeyCode.RightShift);

	public bool IsControlDown => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

	public bool IsLeftControlDown => Input.GetKey(KeyCode.LeftControl);

	public bool IsRightControlDown => Input.GetKey(KeyCode.RightControl);

	public bool IsAltDown => Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

	public bool IsMultiSelectModifierDown => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);

	public List<string> ShownDialogs => mShownDialogs;

	public List<string> ClosedDialogs => mClosedDialogs;

	public List<string> CompletedCameraMoves => mCompletedCameraMoves;

	protected event Action<float> mHandleInputAction;

	public bool PageOpen(FUIPage page)
	{
		return mPageStack.Contains(page);
	}

	private void Awake()
	{
		mInitialized = false;
		FUIButton.TopHudPage = GetTopPageAsNode;
	}

	public FNode GetTopPageAsNode()
	{
		return TopPage;
	}

	private void Start()
	{
		if (PageDebug != null)
		{
			PageDebug.ShowFPS(false);
		}
		FLog.UnityLog("FHUD.Start");
		mUIStage = Futile.GetStage("UI");
		CreateUIPages();
		Futile.Screen.AddSignalResize(RelayoutStackedPages);
		mInitialized = true;
		Futile.TouchManager.OnGetPriorityList = GetOrderedList;
	}

	private void RelayoutStackedPages()
	{
		for (int i = 0; i < mPageStack.Count; i++)
		{
			mPageStack[i].LayoutPage();
		}
	}

	protected void CreateUIPages()
	{
		mPageStack.Clear();
		PageLegendMainMenu = new FPageLegendMainMenu("PageMainMenu");
		PageLegendMainMenu.LayoutPage();
		PageLegendWorld = new FPageLegendWorld("PageLegendWorld");
		PageLegendWorld.LayoutPage();
		PageCharacterPicker = new FPageCharacterPicker("PageCharacterPicker");
		PageCharacterPicker.LayoutPage();
		PageInventory = new FPageInventory("PageInventory");
		PageInventory.LayoutPage();
		PagePause = new FPagePause("Pause");
		PagePause.LayoutPage();
		PageBugReport = new FPageBugReport("PageBugReport");
		PageBugReport.LayoutPage();
		PageWorldMap = new FPageWorldMap("PageWorldMap");
		PageWorldMap.LayoutPage();
		PageSaveSlots = new FPageSaveSlots("PageSaveSlots");
		PageSaveSlots.LayoutPage();
		PageDungeonComplete = new FPageDungeonComplete("PageDungeonComplete");
		PageDungeonComplete.LayoutPage();
		PageGameModeChoice = new FPageGameModeChoice("PageGameModeChoice");
		// shipped code lays out PageDungeonComplete a second time here (not PageGameModeChoice)
		PageDungeonComplete.LayoutPage();
		PageTheaterWorld = new FPageTheaterWorld("PageTheaterWorld");
		PageTheaterWorld.LayoutPage();
		PagePregameOptions = new FPagePregameOptions("PagePregameOptions");
		PagePregameOptions.LayoutPage();
		PageAbout = new FPageAbout("PageAbout");
		PageAbout.LayoutPage();
		PageLoadingScreen = new FPageLoadingScreen("PageLoadingScreen");
		PageLoadingScreen.LayoutPage();
		PageBlackScreen = new FPageBlackScreen("PageBlackScreen");
		PageBlackScreen.LayoutPage();
		PageSaveRecovery = new FPageSaveRecovery("PageSaveRecovery");
		PageSaveRecovery.LayoutPage();
		PageKeyboard = new FPageKeyboard();
		PageKeyboard.LayoutPage();
		Futile.Instance.OpenAppKeyboard = delegate(Action<bool, string> onFinishCallback, FKeyboardOptions keyboardOptions, string text, string placeholderText)
		{
			PageKeyboard.OpenKeyboard(onFinishCallback, keyboardOptions, text, placeholderText);
		};
	}

	protected void CreateDebugPages()
	{
		if (mDebugStage != null)
		{
			PageDebug = new FPageDebug("Debug");
			PageDebug.SetParent(mDebugStage);
			PageDebug.LayoutPage();
		}
	}

	public bool PushPage(FUIPage page, bool doTransition = true, bool alreadyInStackOk = false)
	{
		if (PageLegendMainMenu == page)
		{
			FLog.UnityLog("PushPage Main Menu!!!");
		}
		if (page == null)
		{
			return false;
		}
		if (!mPageStack.Contains(page))
		{
			page.DoTransition = doTransition;
			page.PreloadImages();
			mPageStack.Add(page);
			page.LayoutPage();
			page.SetParent(mUIStage);
			SyncDarken();
		}
		else
		{
			if (!alreadyInStackOk)
			{
				FLog.UnityError("Trying to push page '" + page.Name + "', that is already in stack.");
				return false;
			}
			if (doTransition)
			{
				page.ForceTransitionComplete();
			}
		}
		return true;
	}

	public bool PushPageUnder(FUIPage page, FUIPage oldPage, bool doTransition = true)
	{
		if (page == null)
		{
			return false;
		}
		if (oldPage == null)
		{
			return false;
		}
		if (mPageStack.Contains(page))
		{
			FLog.UnityError("Trying to push page '" + page.Name + "', that is already in stack.");
			return false;
		}
		if (!mPageStack.Contains(oldPage))
		{
			FLog.UnityError("Trying to push page '" + page.Name + "' under page '" + oldPage.Name + "' that is NOT in stack.");
			return false;
		}
		int index = mPageStack.IndexOf(oldPage);
		if (index < 0)
		{
			return false;
		}
		page.DoTransition = doTransition;
		page.PreloadImages();
		mPageStack.Insert(index, page);
		page.LayoutPage();
		mUIStage.AddChildBefore(page, oldPage);
		SyncDarken();
		return true;
	}

	public bool PopPage(FUIPage page, bool popDown = false)
	{
		if (page == null)
		{
			return false;
		}
		if (!mPageStack.Contains(page))
		{
			return false;
		}
		if (!popDown)
		{
			if (mPageStack[mPageStack.Count - 1] != page)
			{
				FLog.UnityError("Trying to pop page '" + page.Name + "', but not top page.");
				return false;
			}
		}
		else
		{
			while (mPageStack[mPageStack.Count - 1] != page)
			{
				FUIPage top = mPageStack[mPageStack.Count - 1];
				mPageStack.Remove(top);
				mUIStage.RemoveChild(top);
			}
		}
		page.UnloadImages();
		mPageStack.Remove(page);
		mUIStage.RemoveChild(page);
		SyncDarken();
		return true;
	}

	public bool RemovePage(FUIPage page)
	{
		if (page == null)
		{
			return false;
		}
		if (!mPageStack.Contains(page))
		{
			return false;
		}
		page.UnloadImages();
		mPageStack.Remove(page);
		mUIStage.RemoveChild(page);
		SyncDarken();
		return true;
	}

	public bool RemovePagesOver(FUIPage page, bool playTransition)
	{
		if (page == null)
		{
			return false;
		}
		if (!mPageStack.Contains(page))
		{
			return false;
		}
		int i = mPageStack.Count;
		while (--i >= 0)
		{
			if (mPageStack[i] == page)
			{
				break;
			}
			if (!playTransition)
			{
				mUIStage.RemoveChild(mPageStack[i]);
				mPageStack[i].UnloadImages();
				mPageStack.RemoveAt(i);
			}
			else
			{
				mPageStack[i].TransitionOut();
			}
			if (i <= 0)
			{
				break;
			}
		}
		SyncDarken();
		return true;
	}

	public bool RemovePagesUnder(FUIPage page, FUIPage ignorePage)
	{
		if (page == null)
		{
			return false;
		}
		if (!mPageStack.Contains(page))
		{
			return false;
		}
		int i = 0;
		while (i < mPageStack.Count && mPageStack[i] != page)
		{
			if (mPageStack[i] != ignorePage)
			{
				mUIStage.RemoveChild(mPageStack[i]);
				mPageStack[i].UnloadImages();
				mPageStack.RemoveAt(i);
				i--;
			}
			i++;
		}
		return true;
	}

	public void RemoveAllUIPages()
	{
		while (mPageStack.Count > 0)
		{
			int index = mPageStack.Count - 1;
			mPageStack[index].StopAllTransitions();
			mUIStage.RemoveChild(mPageStack[index]);
			mPageStack[index].UnloadImages();
			mPageStack.RemoveAt(index);
		}
	}

	public void RefreshPages()
	{
		for (int i = mPageStack.Count - 1; i >= 0; i--)
		{
			mPageStack[i].Refresh();
		}
	}

	public void SyncDarken()
	{
		bool darkenRest = false;
		for (int i = mPageStack.Count - 1; i >= 0; i--)
		{
			if (!darkenRest)
			{
				mPageStack[i].UnDarken();
			}
			else
			{
				mPageStack[i].Darken();
			}
			darkenRest = mPageStack[i].DarkenLower | darkenRest;
		}
		if (FApp.World != null)
		{
			FApp.World.DimWorld(darkenRest);
		}
	}

	public FPageStringPicker StartStringPicker()
	{
		return new FPageStringPicker("StringPickerDialog");
	}

	public void FinishStringPicker(FPageStringPicker p)
	{
		p.LayoutPage();
		PushPage(p);
	}

	public FPageOkDialog OpenOkDialog(string titleText, string bodyText, Action okPress = null, Action returnPress = null, Action escapePress = null, FUIPage.TransitionStyles transition = FUIPage.TransitionStyles.Fade, FPageOkDialog.AndroidBackButtonType androidBackButton = FPageOkDialog.AndroidBackButtonType.Button0)
	{
		FPageOkDialog dialog = new FPageOkDialog("OkDialog");
		dialog.SetupText(titleText, bodyText, true, 24f);
		dialog.SetupSingleButton(FLoc.GetString("OkButton"), 192f, okPress, returnPress, escapePress);
		dialog.SetupFinalize();
		dialog.TransitionStyle = transition;
		dialog.AndroidBackButton = androidBackButton;
		PushPage(dialog);
		return dialog;
	}

	public FPageOkDialog OpenOkDialogNoMarkup(string titleText, string bodyText, Action okPress = null, Action returnPress = null, Action escapePress = null, FUIPage.TransitionStyles transition = FUIPage.TransitionStyles.Fade, FPageOkDialog.AndroidBackButtonType androidBackButton = FPageOkDialog.AndroidBackButtonType.Button0)
	{
		FPageOkDialog dialog = new FPageOkDialog("OkDialog");
		dialog.SetupText(titleText, bodyText, false, 24f);
		dialog.SetupSingleButton(FLoc.GetString("OkButton"), 192f, okPress, returnPress, escapePress);
		dialog.SetupFinalize();
		dialog.TransitionStyle = transition;
		dialog.AndroidBackButton = androidBackButton;
		PushPage(dialog);
		return dialog;
	}

	public FPageOkDialog OpenYesNoDialog(string titleText, string bodyText, string buttonText0, Action press0, string buttonText1, Action press1, Action returnPress, Action escapePress, FUIPage.TransitionStyles transition = FUIPage.TransitionStyles.Fade, FPageOkDialog.AndroidBackButtonType androidBackButton = FPageOkDialog.AndroidBackButtonType.Button1)
	{
		FPageOkDialog dialog = new FPageOkDialog("OkDialog");
		dialog.SetupText(titleText, bodyText, true, 24f);
		dialog.SetupDoubleButton(buttonText0, press0, buttonText1, press1, returnPress, escapePress);
		dialog.SetupFinalize();
		dialog.TransitionStyle = transition;
		dialog.AndroidBackButton = androidBackButton;
		PushPage(dialog);
		FFont.DF_GlobalCounter++;
		return dialog;
	}

	public FPageOkDialog OpenWaitDialog(string titleText, string bodyText, Action cancelPress = null, Action returnPress = null, Action escapePress = null, FUIPage.TransitionStyles transition = FUIPage.TransitionStyles.Fade, bool cancelButton = true)
	{
		FPageOkDialog dialog = new FPageOkDialog("WaitDialog");
		dialog.SetupText(titleText, bodyText, true, 24f);
		dialog.SetupSingleButton(FLoc.GetString("CancelButton"), 192f, cancelPress, returnPress, escapePress);
		if (!cancelButton)
		{
			dialog.SetButton0Visible(false);
		}
		dialog.SetupFinalize();
		dialog.TransitionStyle = transition;
		dialog.TurnOnSpinner();
		dialog.AndroidBackButton = cancelButton ? FPageOkDialog.AndroidBackButtonType.Button0 : FPageOkDialog.AndroidBackButtonType.None;
		PushPage(dialog);
		return dialog;
	}

	public FPageOkDialog OpenVisualOkDialog(FVisualDefinition vd, string animName, string titleText, string bodyText, string buttonText, int buttonWidth, Action okPress = null, Action returnPress = null, Action escapePress = null, FUIPage.TransitionStyles transition = FUIPage.TransitionStyles.Fade, FPageOkDialog.AndroidBackButtonType androidBackButton = FPageOkDialog.AndroidBackButtonType.Button0)
	{
		FPageOkDialog dialog = new FPageOkDialog("CharacterDialog");
		dialog.SetupText(titleText, bodyText, true, 24f);
		dialog.SetupModel(vd, animName);
		dialog.SetupSingleButton(buttonText, buttonWidth, okPress, returnPress, escapePress);
		dialog.SetupFinalize();
		dialog.TransitionStyle = transition;
		dialog.AndroidBackButton = androidBackButton;
		PushPage(dialog);
		return dialog;
	}

	public FPageOkDialog OpenSpriteOkDialog(string sprite, string backSprite, string foreSprite, Vector2 spriteSize, string titleText, string bodyText, string buttonText, int buttonWidth, Action okPress = null, Action returnPress = null, Action escapePress = null, FUIPage.TransitionStyles transition = FUIPage.TransitionStyles.Fade, FPageOkDialog.AndroidBackButtonType androidBackButton = FPageOkDialog.AndroidBackButtonType.Button0)
	{
		FPageOkDialog dialog = new FPageOkDialog("CharacterDialog");
		dialog.SetupText(titleText, bodyText, true, 24f);
		dialog.SetupSprite(sprite, backSprite, foreSprite, spriteSize);
		dialog.SetupSingleButton(buttonText, buttonWidth, okPress, returnPress, escapePress);
		dialog.SetupFinalize();
		dialog.TransitionStyle = transition;
		dialog.AndroidBackButton = androidBackButton;
		PushPage(dialog);
		return dialog;
	}

	protected void Update()
	{
		float deltaTime = Time.deltaTime;
		HandleInput(deltaTime);
		HandleBackKey();
	}

	protected void HandleBackKey()
	{
		if (Input.GetKeyUp(KeyCode.Escape) && mPageStack.Count > 0 && TopPage != null)
		{
			mPageStack[mPageStack.Count - 1].HandleBackKey();
		}
	}

	public void GetOrderedList(List<FSingleTouchableInterface> orderedList)
	{
		orderedList.Clear();
		for (int i = mPageStack.Count - 1; i >= 0; i--)
		{
			GetPriorityOrderListForPage(mPageStack[i], mScratchpadStack, mScratchpadListTouchableInterface);
			for (int j = 0; j < mScratchpadListTouchableInterface.Count; j++)
			{
				orderedList.Add(mScratchpadListTouchableInterface[j]);
			}
		}
		mScratchpadListTouchableInterface.Clear();
		mScratchpadQueue.Clear();
		mScratchpadStack.Clear();
	}

	private void RecursiveNodeDepthList(FNode n, List<FSingleTouchableInterface> list)
	{
		if (n is FSingleTouchableInterface)
		{
			list.Add((FSingleTouchableInterface)n);
		}
		int count = n.GetChildCount();
		for (int i = 0; i < count; i++)
		{
			RecursiveNodeDepthList(n.GetChildAt(i), list);
		}
	}

	public void GetPriorityOrderListForPage(FUIPage p, Stack<FNode> scratchpadStack, List<FSingleTouchableInterface> orderedListForPage)
	{
		orderedListForPage.Clear();
		if (p == null)
		{
			return;
		}
		scratchpadStack.Clear();
		scratchpadStack.Push(p);
		while (scratchpadStack.Count > 0)
		{
			FNode node = scratchpadStack.Pop();
			if (node is FSingleTouchableInterface)
			{
				orderedListForPage.Add((FSingleTouchableInterface)node);
			}
			for (int i = node.GetChildCount() - 1; i >= 0; i--)
			{
				scratchpadStack.Push(node.GetChildAt(i));
			}
		}
		orderedListForPage.Reverse();
	}

	public void GetOrderedList(List<FInputHandlerInterface> orderedList)
	{
		orderedList.Clear();
		for (int i = mPageStack.Count - 1; i >= 0; i--)
		{
			GetPriorityOrderListForPage(mPageStack[i], mScratchpadStack, mScratchpadListInputHandler);
			for (int j = 0; j < mScratchpadListInputHandler.Count; j++)
			{
				orderedList.Add(mScratchpadListInputHandler[j]);
			}
			if (mPageStack[i].Modal)
			{
				break;
			}
		}
		mScratchpadListInputHandler.Clear();
		mScratchpadQueue.Clear();
		mScratchpadStack.Clear();
	}

	private void RecursiveNodeDepthList(FNode n, List<FInputHandlerInterface> list)
	{
		if (n is FInputHandlerInterface)
		{
			list.Add((FInputHandlerInterface)n);
		}
		int count = n.GetChildCount();
		for (int i = 0; i < count; i++)
		{
			RecursiveNodeDepthList(n.GetChildAt(i), list);
		}
	}

	public void GetPriorityOrderListForPage(FUIPage p, Stack<FNode> scratchpadStack, List<FInputHandlerInterface> orderedListForPage)
	{
		orderedListForPage.Clear();
		if (p == null)
		{
			return;
		}
		scratchpadStack.Clear();
		scratchpadStack.Push(p);
		while (scratchpadStack.Count > 0)
		{
			FNode node = scratchpadStack.Pop();
			if (node is FInputHandlerInterface)
			{
				orderedListForPage.Add((FInputHandlerInterface)node);
			}
			for (int i = node.GetChildCount() - 1; i >= 0; i--)
			{
				scratchpadStack.Push(node.GetChildAt(i));
			}
		}
		orderedListForPage.Reverse();
	}

	protected void HandleInput(float deltaTime)
	{
		GetOrderedList(mInputHandlerOrderedList);
		for (int i = mInputHandlerOrderedList.Count - 1; i >= 0; i--)
		{
			FInputHandlerInterface handler = mInputHandlerOrderedList[i];
			if (handler.HandleInput(deltaTime))
			{
				break;
			}
			if (handler is FUIPage && ((FUIPage)handler).Modal)
			{
				break;
			}
		}
		if (this.mHandleInputAction != null)
		{
			this.mHandleInputAction(deltaTime);
		}
		// TODO(port): F10 screenshot / F11 report-bug / keypad debug hotkeys omitted (Android plugin relay) (0x1714798) [left: Android plugin relay hotkeys]
	}

	protected void TestACharacterDialogAction(float flt, string str)
	{
		Debug.Log("The inputs to the function are :  flt = " + flt.ToString() + " and str = " + str);
	}

	protected void TestFoo(int number)
	{
		FLog.UnityLog("TESTFOO:  " + number.ToString());
		FLog.UnityLog("  New Stack: " + mPageStack.Count.ToString() + " Pages ");
		for (int i = mPageStack.Count - 1; i >= 0; i--)
		{
			FLog.UnityLog("   " + i.ToString() + " : " + mPageStack[i].Name);
		}
	}

	protected void TestYes(int number)
	{
		FLog.UnityLog("YES:  " + number.ToString());
	}

	protected void TestNo(int number)
	{
		FLog.UnityLog("No:  " + number.ToString());
	}

	public void ShowFPS(bool show)
	{
		if (PageDebug != null)
		{
			PageDebug.ShowFPS(show);
		}
	}

	public void LoadLevel(string mapName, Action onSuccess, Action onFailure, bool editorStart)
	{
		if (mIsLoading)
		{
			FLog.UnityLog("Skip load, already loading");
			return;
		}
		mIsLoading = true;
		PageLegendWorld.ResetQuestMarkers();
		StartCoroutine(LoadLevelInternal(mapName, onSuccess, onFailure, editorStart));
	}

	public void PushPagesAfterLoad(FGameStartData gameStartData, FUIPage page)
	{
	}

	protected IEnumerator LoadLevelInternal(string mapName, Action onSuccess, Action onFailure, bool editorStart)
	{
		yield return null;
		bool ok = true;
		if (FApp.World == null)
		{
			yield return StartCoroutine(FApp.CreateWorld(mapName));
		}
		if (!ok)
		{
			mIsLoading = false;
			if (onFailure != null)
			{
				onFailure();
			}
			yield break;
		}
		mShownDialogs.Clear();
		mClosedDialogs.Clear();
		mCompletedCameraMoves.Clear();
		if (onSuccess != null)
		{
			onSuccess();
		}
		mIsLoading = false;
	}

	public bool IsOnScreen(Vector2 point)
	{
		return -Futile.Screen.HalfWidth <= point.x && point.x <= Futile.Screen.HalfWidth && -Futile.Screen.HalfHeight <= point.y && point.y <= Futile.Screen.HalfHeight;
	}

	public void CreateFirework(Vector2 position, string prefab = "")
	{
		FGameObjectNode node;
		if (!prefab.Valid())
		{
			node = new FGameObjectNode((FRandom.Generic.RangeInt(0, 1) != 0) ? "Prefabs/FXUI/FXFireworks02" : "Prefabs/FXUI/FXFireworks01");
		}
		else
		{
			node = new FGameObjectNode(prefab);
		}
		node.sortZ = 1000f;
		node.Position = position;
		node.BeginLifespan(1.5f);
		node.SetParent(FApp.HUD.UIStage);
	}

	public FNode GetNodeByName(string name)
	{
		return null;
	}

	public FNode MakeItRain(float duration = 30f, bool addToUIStage = true)
	{
		if (mRainFX != null)
		{
			return mRainFX;
		}
		mRainFX = new FGameObjectNode("Prefabs/FXUI/FXDefeat");
		mRainFX.sortZ = 1000f;
		mRainFX.Position = new Vector2(0f, Futile.Screen.HalfHeight);
		if (duration > 0f)
		{
			mRainFX.BeginLifespan(duration);
		}
		if (addToUIStage)
		{
			mRainFX.SetParent(mUIStage);
		}
		return mRainFX;
	}

	public void StopRain()
	{
		if (mRainFX != null)
		{
			mRainFX.Unparent();
			mRainFX.Dispose();
			mRainFX = null;
		}
	}

	public void UpdateUnitUINodes()
	{
	}

	public void SetConsoleAlpha(float v)
	{
		if (PageDebug != null && PageDebug.Console != null)
		{
			PageDebug.Console.ConsoleAlpha = v;
		}
	}

	public void SetConsoleHeight(float v)
	{
		if (PageDebug != null && PageDebug.Console != null)
		{
			FPanelConsole console = PageDebug.Console;
			console.ConsoleHeight = v;
			console.Size = new Vector2(Futile.Screen.Width, Futile.Screen.Height * v);
			console.DockInside(PageDebug, InnerDock.Top, console.Size.x, console.Size.y, 0f, 0f);
		}
	}

	public static Color GetRankMedalColor(int rank)
	{
		string hex = "888888";
		switch (rank)
		{
		case 0:
			hex = "00fd1e";
			break;
		case 1:
			hex = "00f4fd";
			break;
		case 2:
			hex = "fde800";
			break;
		case 3:
			hex = "e800fd";
			break;
		case 4:
			hex = "fd8f00";
			break;
		}
		return FUtil.ColorFromHex(hex);
	}

	public static string GetRankMedalIcon(int rank)
	{
		switch (rank)
		{
		case 1:
			return "Rank02.png";
		case 2:
			return "Rank03.png";
		case 3:
			return "Rank04.png";
		case 4:
			return "Rank05.png";
		default:
			return "Rank01.png";
		}
	}

	public static Color GetRarityColor(int v)
	{
		string hex = "ffffff";
		switch (v)
		{
		case 1:
			hex = "00ed0b";
			break;
		case 2:
			hex = "2a69c6";
			break;
		case 3:
			hex = "dc57fb";
			break;
		case 4:
			hex = "f19110";
			break;
		}
		return FUtil.ColorFromHex(hex);
	}

	public bool IsTextEntryOn()
	{
		if (PageBugReport != null && PageBugReport.IsTextEntryOn())
		{
			return true;
		}
		return false;
	}

	public void AddHandleInputEvent(Action<float> action)
	{
		mHandleInputAction -= action;
		mHandleInputAction += action;
	}

	public void RemoveHandleInputEvent(Action<float> action)
	{
		mHandleInputAction -= action;
	}

	public void ShowLoadScreen(string title, string body, Action onFullyVisible = null)
	{
		if (PageLoad == null)
		{
			FAssert.Fail("Trying to show load screen before it was set up");
			return;
		}
		PageLoad.Title = title;
		PageLoad.Body = body;
		PageLoad.OnFullyVisible = onFullyVisible;
		PageLoad.TurnOffCancelButton();
		PushPage(PageLoad, true, true);
	}

	public void ShowLoadScreen(string title, string body, IEnumerator coroutineToWaitFor)
	{
		if (PageLoad == null)
		{
			FAssert.Fail("Trying to show load screen before it was set up");
			return;
		}
		PageLoad.Title = title;
		PageLoad.Body = body;
		PageLoad.OnFullyVisible = null;
		PageLoad.CoroutineToWaitFor = coroutineToWaitFor;
		PageLoad.TurnOffCancelButton();
		PushPage(PageLoad, true, true);
	}

	public void CloseOkDialogs()
	{
		if (TopPage != null)
		{
			while (TopPage != null && TopPage is FPageOkDialog)
			{
				PopPage(TopPage);
				if (TopPage == null)
				{
					break;
				}
			}
		}
	}

	public void CloseBubbles()
	{
		if (TopPage != null)
		{
			while (TopPage != null && TopPage is FPageConversationDialog)
			{
				PopPage(TopPage);
				if (TopPage == null)
				{
					break;
				}
			}
		}
	}

	public FEntityAction CreateAndAddPunchScaleAction(FNode node, float amount, float duration, int numberPunches = 1)
	{
		float scale = node.Scale;
		FEntityActionSequence sequence = new FEntityActionSequence();
		if (numberPunches > 0)
		{
			duration /= (float)numberPunches + (float)numberPunches;
			for (int i = numberPunches; i > 0; i--)
			{
				sequence.AddAction(new FEntityActionTweenPropertyToFloat(duration, "Scale", scale + amount, GoEaseType.QuadOut));
				sequence.AddAction(new FEntityActionTweenPropertyToFloat(duration, "Scale", scale, GoEaseType.QuadIn));
			}
		}
		node.AddAction(sequence);
		return sequence;
	}

	public FEntityAction CreateAndAddPunchPositionAction(FNode node, float amount, float duration, int numberPunches = 1)
	{
		Vector2 position = node.Position;
		FEntityActionSequence sequence = new FEntityActionSequence();
		if (numberPunches > 0)
		{
			duration /= (float)numberPunches + (float)numberPunches;
			for (int i = numberPunches; i > 0; i--)
			{
				float dx = FRandom.Generic.RangeFloat(0f - amount, amount);
				float dy = FRandom.Generic.RangeFloat(0f - amount, amount);
				sequence.AddAction(new FEntityActionTweenPropertyToVector2(duration, "Position", new Vector2(position.x + dx, position.y + dy), GoEaseType.QuadOut));
				sequence.AddAction(new FEntityActionTweenPropertyToVector2(duration, "Position", position, GoEaseType.BackOut));
			}
		}
		node.AddAction(sequence);
		return sequence;
	}
}
