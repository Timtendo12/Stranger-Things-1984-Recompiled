using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPluginRelay @ 0x17bec90..0x17c2100, Ghidra vaddr). Android/iOS native-plugin paths are PC no-ops.
public class FPluginRelay : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CDisableUIInternal_003Ed__28 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return null;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return null;
			}
		}

		[DebuggerHidden]
		public _003CDisableUIInternal_003Ed__28(int _003C_003E1__state)
		{
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
		}
	}

	[CompilerGenerated]
	private sealed class _003CDoReportBug_003Ed__37 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public FPluginRelay _003C_003E4__this;

		public string reporterName;

		public string comments;

		private bool _003CshouldEmail_003E5__2;

		private bool _003CshouldGoToForum_003E5__3;

		private string _003CsavegamePath_003E5__4;

		private string _003CscreenshotPath_003E5__5;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return null;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return null;
			}
		}

		[DebuggerHidden]
		public _003CDoReportBug_003Ed__37(int _003C_003E1__state)
		{
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
		}
	}

	[CompilerGenerated]
	private sealed class _003CTakeScreenshotInternal_003Ed__35 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public bool showUI;

		public bool showPause;

		public string path;

		public bool jpg;

		private int _003CantiAliasing_003E5__2;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return null;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return null;
			}
		}

		[DebuggerHidden]
		public _003CTakeScreenshotInternal_003Ed__35(int _003C_003E1__state)
		{
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
		}
	}

	public static FPluginRelay Instance;

	protected List<string> mPendingURLs = new List<string>();

	protected bool mReadyForURLs;

	public bool BadHashHackFlag;

	protected static bool mNeedsLandscapeReverseHack;

	protected static bool mHasFocus;

	protected static byte[] cObfuscator;

	protected static string cSlightlyObfuscatedSalt;

	public static bool HasFocus
	{
		get
		{
			return mHasFocus;
		}
	}

	public static bool IsAppShuttingDown { get; protected set; }

	public static bool IsTakingScreenshot { get; protected set; }

	public static bool ScreenshotShowUI { get; protected set; }

	public static void Init()
	{
		IsTakingScreenshot = false;
		ScreenshotShowUI = false;
		if (Application.isPlaying)
		{
			GameObject relayObject = new GameObject("zPluginRelay");
			UnityEngine.Object.DontDestroyOnLoad(relayObject);
			Instance = relayObject.AddComponent<FPluginRelay>();
			FSaveFile.BadHashEvent += Instance.BadHashEvent;
		}
	}

	public void BadHashEvent()
	{
		BadHashHackFlag = true;
	}

	public void Update()
	{
		if (!FApp.HandlingUserStateChange && FApp.Instance != null && FApp.PlayerData != null)
		{
			FPlayerData playerData = FApp.PlayerData;
			if ((playerData.NeedSave || playerData.QueuedSave) && (!playerData.mUseCloudSave || !playerData.mIsSavingToCS))
			{
				playerData.RealSave(null, false);
			}
		}
		if (mHasFocus)
		{
			FApp.SyncCursorLimits();
		}
	}

	public void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			FApp.WillResignActive();
		}
		else
		{
			FApp.DidBecomeActive();
		}
	}

	public void OnApplicationFocus(bool focus)
	{
		mHasFocus = focus;
		if (focus)
		{
			FApp.SyncCursorLimits();
		}
	}

	public void NewsUpdated(string newsJSON)
	{
		// Empty in the shipped build.
	}

	public void FAppConfigSaved(string unused)
	{
		FLog.UnityLog("plugin relay FAppConfigSaved");
		FApp.ConfigSaved();
	}

	public static void DisableUI(float time)
	{
		Instance.StartCoroutine(Instance.DisableUIInternal(time));
	}

	[IteratorStateMachine(typeof(_003CDisableUIInternal_003Ed__28))]
	public IEnumerator DisableUIInternal(float time)
	{
		yield return new WaitForEndOfFrame();
	}

	public void HandleCustomURL(string url)
	{
		if (!mReadyForURLs)
		{
			mPendingURLs.Add(url);
			FLog.UnityLog("defer custom url: " + url);
			return;
		}
		FLog.UnityLog("HandleCustomURL: " + url);
		string text = null;
		if (url.StartsWith("legend://"))
		{
			text = FCoreUtil.TrimPrefix(url, "legend://");
		}
		else if (url.StartsWith("http://legendapp.bonusxp.com/"))
		{
			text = FCoreUtil.TrimPrefix(url, "http://legendapp.bonusxp.com/");
		}
		if (text == null)
		{
			Uri uri = new Uri(url);
			if (System.Text.RegularExpressions.Regex.Match(uri.Host, "^(.*\\.)?facebook\\.com$").Success && System.Text.RegularExpressions.Regex.Match(uri.AbsolutePath, "^/?cavemaniagame/?").Success)
			{
				return;
			}
			Application.OpenURL(url);
		}
		else if (text.StartsWith("Items/"))
		{
			string text2 = FCoreUtil.TrimPrefix(text, "Items/");
			FLog.UnityLog("process item link, id=" + text2);
			FApp.TrackEvent("ItemsURL");
			// TODO(port): the shipped code requests https://items.bonusxp.com/grant over HTTP and feeds ParseItemResponse; the item server is offline [left: item server is offline]
		}
	}

	protected static void HandleGameUri(Uri uri)
	{
	}

	public static void ReportBug()
	{
		if (FApp.HUD.PageBugReport != null)
		{
			if (!FApp.HUD.PageBugReport.On)
			{
				FApp.HUD.PushPage(FApp.HUD.PageBugReport, true, false);
			}
		}
		else if (Instance != null)
		{
			Instance.StartCoroutine(Instance.DoReportBug(string.Empty, string.Empty));
		}
	}

	public static void ReportBug(string reporterName, string comments)
	{
		if (Instance != null)
		{
			Instance.StartCoroutine(Instance.DoReportBug(reporterName, comments));
		}
	}

	public static string GetScreenshotName(bool jpg)
	{
		return FUtil.PersistentDataPath + "/Screenshot_" + DateTime.Now.ToString("yyyy-dd-M.HH-mm-ss.fff") + (jpg ? ".jpg" : ".png");
	}

	public static void TakeScreenshot(string path, bool jpg, bool showUI, bool showPause)
	{
		if (Instance != null)
		{
			Instance.StartCoroutine(Instance.TakeScreenshotInternal(path, jpg, showUI, showPause));
		}
	}

	[IteratorStateMachine(typeof(_003CTakeScreenshotInternal_003Ed__35))]
	public IEnumerator TakeScreenshotInternal(string path, bool jpg, bool showUI, bool showPause)
	{
		IsTakingScreenshot = true;
		ScreenshotShowUI = showUI;
		yield return new WaitForEndOfFrame();
		try
		{
			Texture2D texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
			texture2D.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
			texture2D.Apply();
			byte[] bytes = (jpg ? texture2D.EncodeToJPG() : texture2D.EncodeToPNG());
			System.IO.File.WriteAllBytes(path, bytes);
			UnityEngine.Object.Destroy(texture2D);
		}
		catch (Exception ex)
		{
			FLog.UnityWarning("TakeScreenshotInternal failed: " + ex.Message);
		}
		IsTakingScreenshot = false;
		ScreenshotShowUI = false;
	}

	public static string GetSavegameName(string stem)
	{
		return stem + "_" + DateTime.Now.ToString("yyyy-dd-M.HH-mm-ss.fff");
	}

	[IteratorStateMachine(typeof(_003CDoReportBug_003Ed__37))]
	public IEnumerator DoReportBug(string reporterName, string comments)
	{
		// PC: no mail/zip upload; the report text is only logged (the shipped code mails a zip with the savegame and screenshot, 0x17c0fe0)
		if (string.IsNullOrEmpty(reporterName))
		{
			reporterName = "unknown";
		}
		string screenshotPath = GetScreenshotName(jpg: true);
		yield return StartCoroutine(TakeScreenshotInternal(screenshotPath, jpg: true, showUI: true, showPause: false));
		FLog.Log("From: " + reporterName + " Comments: " + comments + "\nDetails: " + FVersionInfo.GetVersionString() + " - " + FVersionInfo.Branch + " - " + DateTime.Now.ToString() + "\n");
	}

	public void AppInitComplete()
	{
		FLog.UnityLog("app init complete");
		mReadyForURLs = true;
		foreach (string url in mPendingURLs)
		{
			HandleCustomURL(url);
		}
		mPendingURLs.Clear();
	}

	public void ChartboostDidShowInterstitial(string param)
	{
		// Empty in the shipped build.
	}

	public void ChartboostDidFailToLoadInterstitial(string param)
	{
		// Empty in the shipped build.
	}

	public static bool ParseItemResponse(string text, string id, string random)
	{
		// the item-grant server is offline on PC
		return false;
	}

	public static void OnPlayerAccessChanged()
	{
		if (FApp.HandlingUserStateChange)
		{
			return;
		}
		FLog.UnityLog("FPluginRelay :: OnPlayerAccessChanged  START!");
		Instance.StartCoroutine(FApp.HandleUserStateChange());
	}

	public void OnDestroy()
	{
		FFileWatcher.Shutdown();
		FApp.DestroyWorld(false);
	}

	public void OnApplicationQuit()
	{
		IsAppShuttingDown = true;
	}
}
