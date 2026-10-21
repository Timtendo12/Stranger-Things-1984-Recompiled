using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Rewritten from libil2cpp.so 1.0.605 (FApp @ 0x16e2b48..0x16ee6e0, Ghidra vaddr).
// All former bare TODO(port) stubs have been rewritten (see docs/agent_notes/stubs1.md).
public class FApp
{
	[SaveVersion(0)]
	public static class FSaveGameClientInfo
	{
		// Empty in the shipped build.
		public static void ClearSaveGameClientInfo()
		{
		}
	}

	public enum PerformanceQuality
	{
		NA = 0,
		OK = 1,
		BelowHardLimit = 2,
		BelowSoftLimit = 3
	}

	protected static FApp mInstance;

	protected StreamWriter mLogFile;

	protected static bool mNeedsBootstrap = true;

	protected static bool mInitComplete;

	// PC port: the Netflix SDK is gone; the game runs with one fixed local profile (same id the offline shim used, so saves keep loading).
	public const string LocalPlayerId = "local-player";

	// PC port: replaces Netflix CurrentProfile.PreferredLanguage; a user setting stored in UnityEngine.PlayerPrefs (read before PlayerData exists).
	public const string LanguagePrefKey = "PC.Language";

#if UNITY_WEBGL && !UNITY_EDITOR
	// The web build leaves out the CJK fonts that zh-cn / zh-tw need (PortBuild), so Chinese is not offered there.
	public static readonly string[] SupportedLanguages = { "en", "ar", "cz", "da", "de", "el", "es", "es-es", "fi", "fr", "he", "hi", "hr", "hu", "id", "it", "ja", "ko", "ms", "nb", "nl", "pl", "pt", "pt-pt", "ro", "ru", "sv", "th", "tr", "uk", "vi" };

	private static readonly string[] SupportedLanguageNames = { "English", "Arabic", "Cestina", "Dansk", "Deutsch", "Greek", "Espanol", "Espanol (ES)", "Suomi", "Francais", "Hebrew", "Hindi", "Hrvatski", "Magyar", "Indonesia", "Italiano", "Japanese", "Korean", "Melayu", "Norsk", "Nederlands", "Polski", "Portugues (BR)", "Portugues (PT)", "Romana", "Russian", "Svenska", "Thai", "Turkce", "Ukrainian", "Tieng Viet" };
#else
	public static readonly string[] SupportedLanguages = { "en", "ar", "cz", "da", "de", "el", "es", "es-es", "fi", "fr", "he", "hi", "hr", "hu", "id", "it", "ja", "ko", "ms", "nb", "nl", "pl", "pt", "pt-pt", "ro", "ru", "sv", "th", "tr", "uk", "vi", "zh-cn", "zh-tw" };

	private static readonly string[] SupportedLanguageNames = { "English", "Arabic", "Cestina", "Dansk", "Deutsch", "Greek", "Espanol", "Espanol (ES)", "Suomi", "Francais", "Hebrew", "Hindi", "Hrvatski", "Magyar", "Indonesia", "Italiano", "Japanese", "Korean", "Melayu", "Norsk", "Nederlands", "Polski", "Portugues (BR)", "Portugues (PT)", "Romana", "Russian", "Svenska", "Thai", "Turkce", "Ukrainian", "Tieng Viet", "Chinese (S)", "Chinese (T)" };
#endif

	public static string PreferredLanguage
	{
		get
		{
			string saved = UnityEngine.PlayerPrefs.GetString(LanguagePrefKey, "");
			if (Array.IndexOf(SupportedLanguages, saved) >= 0)
			{
				return saved;
			}
			string system = (FLoc.SystemLocale ?? "").ToLowerInvariant();
			if (Array.IndexOf(SupportedLanguages, system) >= 0)
			{
				return system;
			}
			string lang = FLoc.SystemLanguage;
			if (lang != null && lang.ToLowerInvariant() == "cs")
			{
				return "cz";
			}
			if (lang != null && lang.ToLowerInvariant() == "zh")
			{
				string zh = system.Contains("hans") || system.Contains("cn") ? "zh-cn" : "zh-tw";
				if (Array.IndexOf(SupportedLanguages, zh) >= 0)
				{
					return zh;
				}
			}
			if (lang != null && Array.IndexOf(SupportedLanguages, lang.ToLowerInvariant()) >= 0)
			{
				return lang.ToLowerInvariant();
			}
			return "en";
		}
	}

	public static string LanguageDisplayName(string code)
	{
		int i = Array.IndexOf(SupportedLanguages, code);
		return i >= 0 ? SupportedLanguageNames[i] : code;
	}

	// Stores the choice and re-runs the post-login setup, which rebuilds the HUD pages with the new string tables (live switch).
	private static bool mReopenOptions;

	public static void SetPreferredLanguage(string code, bool reopenOptions = false)
	{
		mReopenOptions = reopenOptions;
		UnityEngine.PlayerPrefs.SetString(LanguagePrefKey, code);
		UnityEngine.PlayerPrefs.Save();
		FPluginRelay.OnPlayerAccessChanged();
	}

	private static bool mHandlingUserStateChange;

	protected static bool mFirstDidBecomeActiveCall = true;

	public static GameObject UnityEventSystemObject;

	public static EventSystem UnityEventSystem;

	protected static bool mInEditor;

	protected static TimeSpan mTimeOffset;

	public const float cTimeMultitaskedOutForProgressRefresh = 60f;

	public const float cTimeMultitaskedOutForPendingReceipts = 30f;

	public const float cTimeMultitaskedOutForStatSession = 300f;

	public const float cMaxTimeScale = 4f;

	public const float cDefaultTimeScale = 1f;

	public const float cTimeScaleAdjustIncrement = 0.25f;

	protected static float mLastStartupYieldTime;

	protected static float mInitPercentage;

	protected static Dictionary<string, Assembly> mVersionTolerantAssemblies = new Dictionary<string, Assembly>();

	public static Stopwatch TimeSinceStartup = Stopwatch.StartNew();

	public const int cResolutionCountdownTime = 15;

	public static FApp Instance
	{
		get
		{
			if (mInstance == null)
			{
				Init();
			}
			return mInstance;
		}
	}

	public static FWorldManager WorldManager { get; set; }

	public static FProtoUnitManager ProtoUnitManager { get; set; }

	public static FProtoWeaponManager ProtoWeaponManager { get; set; }

	public static FProtoCollectibleManager ProtoCollectibleManager { get; set; }

	public static FProtoQuestManager ProtoQuestManager { get; set; }

	public static FLootTableManager LootTableManager { get; set; }

	public static FDialogueManager DialogManager { get; set; }

	public static FMapMarkerManager MapMarkerManager { get; set; }

	public static FWorld World { get; set; }

	public static FPlayerStats PlayerStats { get; set; }

	public static FPlayerData PlayerData { get; set; }

	public static FHUD HUD { get; set; }

	public static FSoundManager SoundManager { get; set; }

	public static FObjectManager ObjectManager { get; set; }

	public static FUnityObjectTracker UnityObjectTracker { get; set; }

	public static FResourceManager ResourceManager { get; set; }

	public static FKeymapper Keymapper { get; set; }

	public static FLazyPrefabPool PrefabPool { get; set; }

	public static bool NeedsBootstrap
	{
		get
		{
			return mNeedsBootstrap;
		}
		set
		{
			mNeedsBootstrap = value;
		}
	}

	public static GameObject ChapterTitleCamera { get; protected set; }

	public static Text ChapterTitle { get; protected set; }

	public static Text ChapterSubTitle { get; protected set; }

	public static AudioListener Listener { get; set; }

	public static string UserConfigFilename => FUtil.PersistentDataPath + "/UserConfig.txt";

	public static string UserCommandsFilename => FUtil.PersistentDataPath + "/UserCommands.txt";

	public static bool QueueShowRequests { get; set; }

	public static DateTime TimeUserMultitaskedOut { get; set; }

	public static bool HandlingUserStateChange => mHandlingUserStateChange;

	public static bool InitComplete => mInitComplete;

	public static bool InEditor
	{
		get
		{
			return mInEditor;
		}
		set
		{
			mInEditor = value;
		}
	}

	public static DateTime UtcNow
	{
		get
		{
			return (DateTime.UtcNow.ToUniversalTime() - mTimeOffset).ToUniversalTime();
		}
		set
		{
			mTimeOffset = DateTime.UtcNow.ToUniversalTime() - value.ToUniversalTime();
		}
	}

	public static float InitPercentage => mInitPercentage;

	public static string SavegamePath => FUtil.PersistentDataPath + "/Savegames/";

	static FApp()
	{
		FCoreUtil.LogHandler = msg => FLog.UnityLog(msg);
		FCoreUtil.LogWarningHandler = msg => FLog.UnityWarning(msg);
		FCoreUtil.LogErrorHandler = msg => FLog.UnityError(msg);
		FCoreUtil.LogFileHandler = msg => FLog.Log(msg);
		FCoreUtil.LogFileWarningHandler = msg => FLog.Warning(msg);
		FCoreUtil.LogFileErrorHandler = msg => FLog.Error(msg);
		SetupAssemblyInfo();
	}

	[Conditional("DEBUG_STARTUP_TIMING")]
	public static void StartupTiming(string str)
	{
	}

	private static void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs args)
	{
		Exception e = args.ExceptionObject as Exception;
		if (!FConfig.FinalCrashDialog)
		{
			return;
		}
		const string separator = "==================================================================";
		FLog.UnityLog(separator);
		FLog.UnityLog("Unhandled Exception: " + e.Message);
		FLog.UnityLog("callstack:");
		FLog.UnityLog(e.StackTrace);
		if (e.InnerException != null)
		{
			FLog.UnityLog("==============================================================");
			FLog.UnityLog("   Inner Exception: " + e.InnerException.Message);
			FLog.UnityLog("   Inner exception callstack: " + e.InnerException.StackTrace);
			FLog.UnityLog("==============================================================");
		}
		FLog.UnityLog(separator);
		WriteCrashReport(e.Message, e.StackTrace, FCoreUtil.SafeToString(e.InnerException),
			e.InnerException == null ? "" : FCoreUtil.SafeToString(e.InnerException.StackTrace));
		FAssert.ShowExceptionDialog(e.ToString(), e.StackTrace);
	}

	public static void WriteCrashReport(string message, string trace, string innerException, string innerExceptionTrace)
	{
		FLog.Error("Crash: " + message + "\n" + trace);
		try
		{
			using (StreamWriter writer = new StreamWriter(new FileStream(GetCrashReportFileName(), FileMode.Create, FileAccess.Write, FileShare.Read)))
			{
				writer.WriteLine("Crash in Version: " + FVersionInfo.GetVersionString());
				writer.WriteLine("Exception: " + message);
				writer.WriteLine("Stack: " + trace);
				if (!string.IsNullOrEmpty(innerException))
				{
					writer.WriteLine("\nInner Exception: " + innerException);
					writer.WriteLine("Inner Exception Stack: " + innerExceptionTrace);
				}
			}
		}
		catch (Exception)
		{
			// PC port: never let the crash writer itself throw
		}
	}

	public static string GetCrashReportFileName()
	{
		return FUtil.PersistentDataPath + "/CrashReport.txt";
	}

	protected static Assembly GetLoadedAssemblyByShortName(string shortName)
	{
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			if (assembly.GetName().Name == shortName)
			{
				return assembly;
			}
		}
		return null;
	}

	protected static void AddVersionTolerantAssembly(string shortName)
	{
		Assembly assembly = GetLoadedAssemblyByShortName(shortName);
		if (assembly != null)
		{
			mVersionTolerantAssemblies[shortName] = assembly;
		}
	}

	protected static void SetupAssemblyInfo()
	{
		AddVersionTolerantAssembly("BXPCoreUtil");
		AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;
	}

	protected static Assembly AssemblyResolver(object sender, ResolveEventArgs args)
	{
		string shortName = new AssemblyName(args.Name).Name;
		mVersionTolerantAssemblies.TryGetValue(shortName, out Assembly assembly);
		return assembly;
	}

	protected static bool CheckStartupYield()
	{
		float now = Time.realtimeSinceStartup;
		bool yield = now - mLastStartupYieldTime >= 0.5f;
		if (yield)
		{
			mLastStartupYieldTime = now;
		}
		return yield;
	}

	public static IEnumerator Init()
	{
		if (mInstance != null)
		{
			yield break;
		}
		FLog.AddUpdateLogHandler(UpdateLogHandler, false);
		UpdateTextureQualityFromAvailMem();
		FAssertHandler.Init();
		FAssert.SendReportEvent += SendAssertExceptionReport;
		AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler;
		FDelayedDestroyQueue.Init();
		FVersionInfo.Init();
		FPluginRelay.Init();
		FConfigSystem.AddClass(typeof(FConfig));
		if (CheckStartupYield())
		{
			yield return null;
		}

		FPhysicsLayers.Init();
		FBucketer.Init("Data/Buckets");
		mInstance = new FApp();
		ParseConfigs();
		FConsole.Init();
		FConsoleCore.Output("Data folder: " + FUtil.PersistentDataPath);
		PlayerStats = new FPlayerStats();
		PlayerStats.Init();
		FTagManager.Init();
		ProtoCollectibleManager = FProtoCollectibleManager.CreateFromFile("Data/ProtoCollectibles");
		ProtoQuestManager = FProtoQuestManager.CreateFromFile("Data/ProtoQuests");
		ProtoWeaponManager = FProtoWeaponManager.CreateFromFile("Data/ProtoWeapons");
		ProtoUnitManager = FProtoUnitManager.CreateFromFile("Data/ProtoUnits");
		WorldManager = FWorldManager.CreateFromFile("Data/Worlds");
		if (CheckStartupYield())
		{
			yield return null;
		}

		mInitPercentage = 10f;
		if (FConfig.LogFileOn)
		{
			FLog.Init("Legend.txt");
			const string rule = "//===================================";
			FLog.Log(rule);
			FLog.Log("//  " + DateTime.Now.ToString());
			FLog.Log("//  Version: " + FVersionInfo.GetVersionString());
			FLog.Log(rule);
		}
		FLog.Log("Device model: " + SystemInfo.deviceModel);
		FLog.Log("Device name: " + SystemInfo.deviceName);
		FLog.Log("Graphics device type: " + SystemInfo.graphicsDeviceType.ToString());
		FLog.Log("Graphics device version: " + SystemInfo.graphicsDeviceVersion);
		FLog.Log("Graphics device name: " + SystemInfo.graphicsDeviceName);
		FLog.Log("Graphics device ID: " + SystemInfo.graphicsDeviceID.ToString());
		FLog.Log("Graphics memory size: " + SystemInfo.graphicsMemorySize.ToString());
		FLog.Log("Graphics shader level: " + SystemInfo.graphicsShaderLevel.ToString());
		Resolution current = Screen.currentResolution;
		FLog.Log(string.Format("Screen: {0} x {1}  {2}Hz  dpi:{3}", current.width, current.height,
			(int)Math.Round(current.refreshRateRatio.value), Screen.dpi));
		FLog.Log("OS version: " + SystemInfo.operatingSystem);
		FLog.Log("Processor type: " + SystemInfo.processorType);
		FLog.Log("Processor count: " + SystemInfo.processorCount.ToString());
		FLog.Log("Processor frequency: " + SystemInfo.processorFrequency.ToString());
		FLog.Log("System Memory: " + SystemInfo.systemMemorySize.ToString());
		int quality = QualitySettings.GetQualityLevel();
		FLog.Log(string.Format("Quality[{0}] = {1}     Note may get overriden later in init!", quality, QualitySettings.names[quality]));
		FLog.Log(string.Format("masterTextureLimit = {0}", QualitySettings.globalTextureMipmapLimit));
		Application.logMessageReceived += OnUnityLogMessage;
		if (CheckStartupYield())
		{
			yield return null;
		}

		mInitPercentage = 20f;
		yield return FGameUtil.StartCoroutine(WorldManager.LoadWorlds(5));

		if (UnityEventSystemObject == null)
		{
			UnityEventSystemObject = GameObject.Find("EventSystem");
		}
		if (UnityEventSystemObject != null)
		{
			UnityEngine.Object.DontDestroyOnLoad(UnityEventSystemObject);
			UnityEventSystem = UnityEventSystemObject.GetComponent<EventSystem>();
		}
		int targetFps = 60;
		Application.targetFrameRate = targetFps;
		FLog.UnityLog("Starting resolution: " + Screen.width.ToString() + ", " + Screen.height.ToString() +
			",  Screen.fullScreen" + Screen.fullScreen.ToString() + ", targetFPS=" + targetFps.ToString() + ".");
		Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreen);
		FUnityServices.Init();
		if (CheckStartupYield())
		{
			yield return null;
		}

		mInitPercentage = 30f;
		yield return FGameUtil.StartCoroutine(WorldManager.LoadWorlds(5));
		// PC port: replaces NetflixSdkManager.Init/CheckAuth; the (single) local profile logs in immediately.
		yield return null;
		FPluginRelay.OnPlayerAccessChanged();
	}

	// <Init>b__148_0: Unity log callback.
	private static void OnUnityLogMessage(string logString, string stackTrace, LogType type)
	{
		if (type != LogType.Error && type != LogType.Assert && type != LogType.Exception)
		{
			return;
		}
		if (string.IsNullOrEmpty(stackTrace))
		{
			stackTrace = new StackTrace().ToString();
		}
		FLog.Error("ERROR: " + logString + "\n" + stackTrace);
		if (type == LogType.Exception)
		{
			WriteCrashReport(logString, stackTrace, null, null);
			if (FConfig.FinalCrashDialog)
			{
				FAssert.ShowExceptionDialog(logString, stackTrace);
			}
			Application.Quit();
		}
	}

	public static IEnumerator HandleUserStateChange()
	{
		bool pushMainMenu;
		bool loadDynamicFontData;

		mHandlingUserStateChange = true;
		string profileId = LocalPlayerId;
		string locale = PreferredLanguage;

		FLog.UnityLog("Clear saved slots if switched profiles :: " + profileId);
		PlayerStats?.FullWipePlayerState();
		FLog.UnityLog("New profile '" + profileId + "' specifies locale (" + locale + ")");

		pushMainMenu = false;
		GameObject hudObject = HUD != null ? HUD.gameObject : null;
		if (hudObject == null)
		{
			hudObject = GameObject.Find("HUD");
		}
		if (hudObject != null)
		{
			FHUD oldHud = hudObject.GetComponent<FHUD>();
			if (oldHud != null)
			{
				pushMainMenu = oldHud.Initialized;
				oldHud.RemoveAllUIPages();
			}
			UnityEngine.Object.Destroy(hudObject);
		}
		FLoc.SetLocaleAndLanguage(locale, locale);
		FColorNames.AddColorsXml("Data/ColorNames");
		FColorNames.Set("Magenta", "FF00FF");
		PlayerStats.Init();
		if (GameObject.Find("Futile") == null)
		{
			GameObject futileObject = new GameObject("Futile");
			UnityEngine.Object.DontDestroyOnLoad(futileObject);
			futileObject.AddComponent<Futile>();
			FutileParams futileParams = new FutileParams(true, true, false, false, ScreenOrientation.LandscapeRight);
			futileParams.AddResolutionLevel(FutileHdMaxLength, 1f, 1f, "HD");
			futileParams.Origin = new Vector2(0.5f, 0.5f);
			Futile.Instance.Init(futileParams);
		}
		if (CheckStartupYield())
		{
			yield return null;
		}

		loadDynamicFontData = FConfigCore.DynamicFonts;
		FFont.SpriteFontsDisabled = FConfigCore.DynamicFonts && !FConfigCore.HybridDynamicFonts;
		Futile.IsInAGame = false;
		Futile.AtlasManager.LoadAtlas("Atlases/UIAtlas", FAtlas.eUnloadStrategy.NeverUnload, FilterMode.Point);
		Futile.AtlasManager.LoadAtlas("Atlases/PregameAtlas", FAtlas.eUnloadStrategy.NeverUnload, FilterMode.Point);
		Futile.AtlasManager.LoadAtlas("Atlases/MainMenuAtlas", FAtlas.eUnloadStrategy.NeverUnload, FilterMode.Point);
		if (CheckStartupYield())
		{
			yield return null;
		}

		mInitPercentage = 40f;
		yield return FGameUtil.StartCoroutine(WorldManager.LoadWorlds(10));
		LoadLooseImageXml("Data/LooseImages");
		if (CheckStartupYield())
		{
			yield return null;
		}

		FFont.ClearDynamicFonts();
		Futile.AtlasManager.LoadFonts();
		if (loadDynamicFontData)
		{
			FFont.LoadAllDynamicFonts();
		}
		if (CheckStartupYield())
		{
			yield return null;
		}

		while (Futile.GetStageCount() > 0)
		{
			Futile.RemoveStage(Futile.GetStageAt(0));
		}
		Futile.AddStage(new FStage("UI"));
		Futile.AddStage(new FStage("Debug"));
		if (CheckStartupYield())
		{
			yield return null;
		}

		mInitPercentage = 50f;
		yield return FGameUtil.StartCoroutine(WorldManager.LoadWorlds(10));
		FFont defaultFont = Futile.AtlasManager.GetFontWithStyleAndSize("DefaultStyle", 24);
		Font unityFont = defaultFont?.mUnityFont;
		if (ChapterTitleCamera == null)
		{
			ChapterTitleCamera = GameObject.Find("TitleCamera");
		}
		if (ChapterTitleCamera != null)
		{
			Transform canvas = ChapterTitleCamera.transform.Find("Canvas");
			if (canvas != null)
			{
				Transform title = canvas.transform.Find("Title");
				if (title != null)
				{
					ChapterTitle = title.GetComponent<Text>();
					if (unityFont != null && ChapterTitle != null)
					{
						ChapterTitle.font = unityFont;
					}
				}
				Transform subTitle = canvas.transform.Find("SubTitle");
				if (subTitle != null)
				{
					ChapterSubTitle = subTitle.GetComponent<Text>();
					if (unityFont != null && ChapterSubTitle != null)
					{
						ChapterSubTitle.font = unityFont;
					}
				}
			}
			ChapterTitleCamera.SetActive(false);
		}
		FLog.Log("Beginning social auth. Local user = " + Social.localUser?.userName);
		if (PrefabPool == null)
		{
			PrefabPool = FLazyPrefabPool.CreateFromXmlFile("Data/PrefabPool");
		}
		if (CheckStartupYield())
		{
			yield return null;
		}

		mInitPercentage = 60f;
		yield return FGameUtil.StartCoroutine(WorldManager.LoadWorlds(10));
		FAnalytics.Init();
		PlayerData = new FPlayerData();
		PlayerData.ProfileId = profileId;
		yield return FGameUtil.TaskAsCoroutine(PlayerData.PreLoad(false, true));
		if (CheckStartupYield())
		{
			yield return null;
		}
		if (CheckStartupYield())
		{
			yield return null;
		}

		FAnimDefManager.Init(false);
		if (CheckStartupYield())
		{
			yield return null;
		}
		if (CheckStartupYield())
		{
			yield return null;
		}

		mInitPercentage = 70f;
		yield return FGameUtil.StartCoroutine(WorldManager.LoadWorlds(10));
		UnityObjectTracker = new FUnityObjectTracker();
		ResourceManager = FResourceManager.CreateFromXmlFile("Data/Resources");
		if (CheckStartupYield())
		{
			yield return null;
		}

		if (SoundManager == null)
		{
			SoundManager = FSoundManager.CreateFromXmlFile("Data/SoundSets");
		}
		PlayerData.ApplyVolumes();
		if (CheckStartupYield())
		{
			yield return null;
		}
		if (CheckStartupYield())
		{
			yield return null;
		}

		if (LootTableManager == null)
		{
			LootTableManager = new FLootTableManager();
			LootTableManager.AddLootTables("Data/Loot");
		}
		if (DialogManager == null)
		{
			DialogManager = FDialogueManager.CreateFromFile("Data/Dialogue");
			DialogManager.LoadDialogueIndices();
		}
		if (MapMarkerManager == null)
		{
			MapMarkerManager = FMapMarkerManager.CreateFromFile("Data/MapMarkers");
		}
		PlayerStats.ApplySpecialEffects();
		mInitPercentage = 80f;
		yield return FGameUtil.StartCoroutine(WorldManager.LoadWorlds(10));

		PlayerData.Load(true);
		FLog.UnityLog("PlayerPrefs is located at " + PreviewLabs.PlayerPrefs.GetFilename());
		GameObject hud = PrefabPool.GetObject("HUD");
		if (hud != null)
		{
			hud.name = "HUD";
			UnityEngine.Object.DontDestroyOnLoad(hud);
			HUD = hud.GetComponent<FHUD>();
		}
		else
		{
			HUD = null;
		}
		if (CheckStartupYield())
		{
			yield return null;
		}
		if (CheckStartupYield())
		{
			yield return null;
		}

		if (!FDebrisTracker.IsInitialized)
		{
			FDebrisTracker.Init(50);
		}
		else
		{
			FDebrisTracker.ClearAll(false);
		}
		if (CheckStartupYield())
		{
			yield return null;
		}

		mInitPercentage = 90f;
		yield return FGameUtil.StartCoroutine(WorldManager.LoadWorlds(20));
		if (pushMainMenu)
		{
			FLog.UnityLog("FApp::HandleUserStateChange::PushMainMenu");
			HUD.PushPage(HUD.PageLegendMainMenu, true, false);
			if (mReopenOptions)
			{
				// PC port: language switch from Options; land back in Options with the new string tables
				mReopenOptions = false;
				HUD.PushPage(HUD.PagePregameOptions, true, false);
			}
		}
		mReopenOptions = false;
		mHandlingUserStateChange = false;
		if (!mInitComplete)
		{
			PostInit(false);
		}
		FLog.UnityLog("HandleUserStateChange -- END");
	}

	// FutileParams.AddResolutionLevel max length for the single "HD" level (float constant @ 0x899b2c).
	private const float FutileHdMaxLength = 750f;

	protected static void ProcessAuth(bool success)
	{
		if (success)
		{
			FLog.Log("Social login succeeded!");
		}
		else
		{
			FLog.Warning("Social login failed!");
		}
	}

	public static void PostInit(bool isNewPlayer)
	{
		if (isNewPlayer)
		{
			PlayerData.ResetNewThings(true);
			PlayerData.Save();
		}
		XmlElement buildNotes = FUtil.LoadXmlAsset("Data/BuildNotes_FINAL");
		if (buildNotes != null)
		{
			FNewsManager.LoadNewsXML(buildNotes);
		}
		FPluginRelay.Instance.AppInitComplete();
		TimeUserMultitaskedOut = DateTime.MaxValue;
		FGibManager.Init();
		FLog.UnityLog("Checking for crash report...");
		mInitComplete = true;
		DidBecomeActive();
	}

	public static void ResetUpgradeableData()
	{
		// Empty in the shipped build.
	}

	public static void ParseConfigs()
	{
		TextAsset gameConfig = Resources.Load("GameConfig", typeof(TextAsset)) as TextAsset;
		if (gameConfig != null)
		{
			FConfigSystem.ProcessString(gameConfig.text);
		}
		string message;
		if (!Application.isEditor)
		{
			string path = Path.Combine(Application.streamingAssetsPath, "ServerConfig.txt");
			string text = null;
			if (File.Exists(path))
			{
				FLog.UnityLog("Parsing config file: " + path);
				text = File.ReadAllText(path);
			}
			if (text != null)
			{
				FConfigSystem.ProcessString(text);
				ParseLocalUserConfig();
				return;
			}
			message = "Config file did not exist: " + path;
		}
		else
		{
			message = "Not parsing ServerConfig.txt because this is the editor";
		}
		FLog.UnityLog(message);
		ParseLocalUserConfig();
	}

	public static void ParseLocalUserConfig()
	{
		FLog.UnityLog("User config location: " + UserConfigFilename);
		if (File.Exists(UserConfigFilename))
		{
			using (StreamReader reader = new StreamReader(UserConfigFilename))
			{
				FConfigSystem.ProcessString(reader.ReadToEnd());
			}
		}
	}

	public static void ExecuteStartupCommands()
	{
		TextAsset commands = Resources.Load("GameCommands", typeof(TextAsset)) as TextAsset;
		if (commands != null)
		{
			FConsoleCore.Execute(commands.text);
		}
		if (File.Exists(UserCommandsFilename))
		{
			using (StreamReader reader = new StreamReader(UserCommandsFilename))
			{
				FConsoleCore.Execute(reader.ReadToEnd());
			}
		}
		// PC-port test aid (not in the shipped game): "<seconds>|<command>" lines in UserCommandsDelayed.txt run that many seconds after startup
		string delayedFile = FUtil.PersistentDataPath + "/UserCommandsDelayed.txt";
		if (File.Exists(delayedFile))
		{
			FGameUtil.StartCoroutine(RunDelayedUserCommands(File.ReadAllLines(delayedFile)));
		}
	}

	private static IEnumerator RunDelayedUserCommands(string[] lines)
	{
		float elapsed = 0f;
		foreach (string line in lines)
		{
			int bar = line.IndexOf('|');
			if (bar <= 0 || !float.TryParse(line.Substring(0, bar), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var at))
			{
				continue;
			}
			while (elapsed < at)
			{
				elapsed += Time.unscaledDeltaTime;
				yield return null;
			}
			FLog.UnityLog("Delayed command: " + line.Substring(bar + 1));
			FConsoleCore.Execute(line.Substring(bar + 1));
		}
	}

	public static void BootstrapComplete()
	{
		ExecuteStartupCommands();
	}

	public static void ConfigSaved()
	{
		ParseConfigs();
	}

	public static IEnumerator CreateWorld(string mapName)
	{
		HUD.RemovePage(HUD.PageLegendMainMenu);
		yield return FGameUtil.StartCoroutine(FWorld.Init(mapName));
	}

	public static void DestroyWorld(bool pushMainMenu)
	{
		if (World == null)
		{
			return;
		}
		Time.timeScale = 1f;
		Futile.EnableCameraClearing(true);
		FLog.Log("FApp::DestroyWorld: Calling World.ReleaseStuff.");
		World.ReleaseStuff();
		FLog.UnityLog("FApp::DestroyWorld: Destroying world.");
		UnityEngine.Object.Destroy(World.gameObject);
		World = null;
		FAnimationManager.ClearLeastUsedUnits();
		Resources.UnloadUnusedAssets();
		GC.Collect();
		if (Futile.Instance != null && pushMainMenu && HUD != null)
		{
			if (HUD.PageLegendWorld != null && HUD.PageLegendWorld.OnStage)
			{
				HUD.RemovePage(HUD.PageLegendWorld);
			}
			FLog.UnityLog("FApp::DestroyWorld::PushMainMenu");
			HUD.PushPage(HUD.PageLegendMainMenu, true, false);
		}
	}

	public static void WillResignActive()
	{
		TimeUserMultitaskedOut = DateTime.UtcNow;
		if (World != null)
		{
			World.WillResignActive();
		}
		if (PlayerData != null)
		{
			PlayerData.RealSave(null, true);
		}
	}

	public static void DidBecomeActive()
	{
		FLog.UnityLog("FApp.DidBecomeActive");
		if (!mInitComplete)
		{
			return;
		}
		FLog.UnityLog(string.Format("FApp.DidBecomeActive - after Init check. mInitComplete={0}", mInitComplete));
		FNotification.ClearAll();
		FNotification.SetBadgeNumber(0);
		TimeSpan away = DateTime.UtcNow - TimeUserMultitaskedOut;
		FLog.UnityLog("out of app for: " + away.TotalSeconds.ToString() + " seconds");
		TimeUserMultitaskedOut = DateTime.MaxValue;
		mFirstDidBecomeActiveCall = false;
	}

	public static void TrackScreenView(string screenName)
	{
		// Empty in the shipped build.
	}

	public static bool IsInternetReachable()
	{
		return true;
	}

	public static void TrackEvent(string eventName)
	{
		// Empty in the shipped build.
	}

	public static void AddNodeConstructorInfo()
	{
		// Empty in the shipped build.
	}

	public static bool LoadLooseImageXml(string filename)
	{
		XmlElement root = FUtil.LoadXmlAsset(filename);
		if (root == null)
		{
			FLog.UnityError("Failed to parse LooseImage file.");
		}
		else
		{
			LoadLooseImageXml(root);
		}
		return root != null;
	}

	public static bool LoadLooseImageXml(XmlElement root)
	{
		foreach (XmlNode node in root.FirstChild.ChildNodes)
		{
			if (node.NodeType == XmlNodeType.Element && node.Name == "Image")
			{
				string name = (node as XmlElement).GetAttrib("Name", "");
				if (FCoreUtil.Valid(name))
				{
					Futile.AtlasManager.LoadImage(name, FilterMode.Bilinear);
				}
			}
		}
		return true;
	}

	public static void OutOfSync()
	{
		// Empty in the shipped build.
	}

	public static void ReloadPlayerData()
	{
		if (PlayerData != null)
		{
			PlayerData.Save();
		}
		PlayerData = new FPlayerData();
		PlayerData.Load(false);
	}

	public static void SendAssertExceptionReport(string message, string callstack, bool isException)
	{
		// PC port: no bug-report upload. 0x16e6064 builds an e-mail/HTTP report.
		FLog.Error((isException ? "Exception: " : "Assert: ") + message + "\n" + callstack);
	}

	public static bool GetAssertExceptionComments(bool isException, out string name, out string comments)
	{
		name = null;
		comments = null;
		return false;
	}

	public static bool GetComments(string title, bool canCancel, out string name, out string comments)
	{
		name = null;
		comments = null;
		return false;
	}

	public static void AddDefaultBugReportFiles(List<string> filenameList)
	{
		FLog.AddFilenameList(filenameList);
		filenameList.Add(UserConfigFilename);
		string export = FUtil.PersistentDataPath + "/PlayerData_export" + DateTime.Now.ToString("yyyy-dd-M.HH-mm-ss.fff") + ".xml";
		PlayerData.RealSave(export, true);
		filenameList.Add(export);
		FileInfo[] records = FCoreUtil.GetFilesSortedByDateDescending(SavegamePath, "*_AutoRecord*.MicroRecord");
		if (records != null)
		{
			for (int i = 0; i < records.Length && i < 3; i++)
			{
				filenameList.Add(records[i].FullName);
			}
		}
	}

	public static void CheckForCrashReport()
	{
		// Empty in the shipped build.
	}

	public static void UpdateQualitySettings()
	{
		int level = Math.Max(PlayerData.ShadowQuality, 0);
		if (level > QualitySettings.names.Length - 1)
		{
			level = QualitySettings.names.Length - 1;
		}
		if (MemoryIsAProblem(out PerformanceQuality quality) && (!mInitComplete || quality == PerformanceQuality.BelowHardLimit))
		{
			FLog.Log(string.Format("Override Quality[{0}] = {1}", 0, QualitySettings.names[0]));
			level = 0;
		}
		QualitySettings.SetQualityLevel(level);
		// PC port: the shipped build sets antiAliasing from PlayerData.EnableAA on some device types only; the 2D camera path
		// renders without MSAA (also through the PCDisplay render target), so the project default of 0 is kept.
		QualitySettings.antiAliasing = 0;
		UpdateTextureQualityFromAvailMem();
	}

	public static void UpdateVideoMode()
	{
		FLog.UnityLog("UpdateVideoMode, Current settings: " + Screen.width.ToString() + " x " + Screen.height.ToString() + " @ " + Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value).ToString() + "Hz " + (Screen.fullScreen ? "Full Screen" : "Windowed"));
		FLog.UnityLog("UpdateVideoMode, New settings: " + PlayerData.ResolutionWidth.ToString() + " x " + PlayerData.ResolutionHeight.ToString() + " @ " + PlayerData.RefreshRate.ToString() + "Hz " + (PlayerData.FullScreen ? "Full Screen" : "Windowed"));
		if (!PlayerData.DoesDesiredVideoModeDiffer())
		{
			FLog.UnityLog("UpdateVideoMode, no changes needed");
			return;
		}
		FLog.UnityLog("UpdateVideoMode, setting new mode");
		int oldWidth = Screen.width;
		int oldHeight = Screen.height;
		bool oldFullScreen = Screen.fullScreen;
		int oldRefresh = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
		// PC port: go through PCDisplay so the saved window / fullscreen choice stays consistent.
		PCDisplay.SetMode(PlayerData.ResolutionWidth, PlayerData.ResolutionHeight, PlayerData.FullScreen);
		// PC port: build the dialog once the window has actually resized (Screen.width/height update on the next frame).
		FGameUtil.DelayExecute(0.3f, delegate
		{
			FPageOkDialog dialog = new FPageOkDialog("OkDialog");
			Coroutine updateCoroutine = FGameUtil.StartCoroutine(UpdateResolutionDialog(dialog));
			dialog.SetupText(LocOr("Resolution.Title", "Keep Display Settings?"), ResolutionBody(15), true, 24f);
			dialog.SetupDoubleButton(LocOr("Resolution.Keep", "Keep"), delegate
			{
				FLog.UnityLog("UpdateVideoMode, keeping new resolution");
				FGameUtil.StopCoroutine(updateCoroutine);
			}, LocOr("Resolution.Revert", "Revert"), delegate
			{
				FLog.UnityLog("UpdateVideoMode, reverting to old resolution");
				PCDisplay.SetMode(oldWidth, oldHeight, oldFullScreen);
				PlayerData.SetResolution(oldWidth, oldHeight, oldRefresh, oldFullScreen, false);
				PlayerData.Save();
			}, null, null);
			dialog.SetupDefaultKeyActions();
			dialog.SetupFinalize();
			dialog.TransitionStyle = FUIPage.TransitionStyles.Fade;
			dialog.AndroidBackButton = FPageOkDialog.AndroidBackButtonType.Button1;
			HUD.PushPage(dialog, true, false);
			FFont.DF_GlobalCounter++; // refresh dynamic font text, like HUD.OpenYesNoDialog
		});
	}

	// PC port: the Resolution.* strings are missing from the shipped string tables (the dialog was never shown in the Netflix build), so fall back to English.
	private static string LocOr(string key, string fallback)
	{
		string text = FLoc.GetString(key);
		return string.IsNullOrEmpty(text) || text == key ? fallback : text;
	}

	private static string ResolutionBody(int seconds)
	{
		string text = FLoc.Format("Resolution.BodyText", seconds);
		return string.IsNullOrEmpty(text) || text == "Resolution.BodyText" ? "Keep these display settings? Reverting in " + seconds.ToString() + "..." : text;
	}

	protected static IEnumerator UpdateResolutionDialog(FPageOkDialog dialog)
	{
		int countdown = 15;
		while (true)
		{
			yield return new WaitForSeconds(1f);
			dialog.SetBodyText(ResolutionBody(countdown));
			countdown--;
			if (countdown < 0)
			{
				break;
			}
		}
		dialog.DoButton1Action();
	}

	public static void DoResetData()
	{
		// Stripped developer feature in the shipped build: only the button click sound survives, gated by two dev flags.
		if (FConfig.BowtiesAreCool && FConfig.EnsureDefeat)
		{
			SoundManager.PlaySoundSet("UIButton");
		}
	}

	public static void DoResetOptions()
	{
		SoundManager.PlaySoundSet("UIButton");
		PlayerData.ClearAllExceptPurchases();
		PlayerData.Save();
	}

	public static void SyncCursorLimits()
	{
		// Empty in the shipped build.
	}

	public static IEnumerator QuitGameCoroutine(Action afterDestroyAction)
	{
		yield return null;
		afterDestroyAction?.Invoke();
	}

	public static void QuitGame()
	{
		if (World == null)
		{
			return;
		}
		HUD.ShowLoadScreen(FLoc.GetString("GenericLoading_Title"), FLoc.GetString("GenericLoading_Body"), () => FGameUtil.StartCoroutine(QuitGameCoroutine(() => HUD.RemoveAllUIPages())));
	}

	public static void RestartGame()
	{
		// Empty in the shipped build.
	}

	public static void AddWorldAttachment(FGameObjectWrapper attachGOW)
	{
		// Empty in the shipped build.
	}

	public static void RemoveWorldAttachment(FGameObjectWrapper gow, bool dispose)
	{
		// Empty in the shipped build.
	}

	public static Camera GetWorldCamera()
	{
		// FWorld+0x40 (the world camera)
		return (World != null) ? World.Camera : null;
	}

	public static string GetFullSavegamePath(string name)
	{
		return SavegamePath + name + ".MicroSave";
	}

	public static string GetFullRecordGamePath(string name)
	{
		return SavegamePath + name + ".MicroRecord";
	}

	public static void PurgeFiles()
	{
		// Empty in the shipped build.
	}

	protected static void UpdateLogHandler(ref string text, bool forUnityConsole)
	{
		if (!forUnityConsole)
		{
			text = "(" + DateTime.UtcNow.ToString("yyyy-dd-MM HH:mm:ss.fffZ") + ") " + text;
		}
	}

	public static void NewFeatureUnlocked(string featureName)
	{
		PlayerData.SetNewThing("Feature." + featureName, true);
		PlayerData.Save();
	}

	public static void NewAvatarUnlocked(string avatarName)
	{
		PlayerData.SetNewThing("Avatar." + avatarName, true);
		PlayerData.Save();
	}

	public static void UpdateTextureQualityFromAvailMem()
	{
		if (MemoryIsAProblem(out PerformanceQuality quality) && (!mInitComplete || quality == PerformanceQuality.BelowHardLimit))
		{
			QualitySettings.globalTextureMipmapLimit = 1;
		}
	}

	public static bool MemoryIsAProblem(out PerformanceQuality perfQuality)
	{
		int memory = SystemInfo.systemMemorySize;
		bool problem = (uint)(memory - 1) < 899u;
		if (problem)
		{
			perfQuality = PerformanceQuality.BelowHardLimit;
			FLog.Log("Low Memory Detected (below hard limit): " + SystemInfo.systemMemorySize.ToString());
		}
		else
		{
			perfQuality = PerformanceQuality.OK;
		}
		return problem;
	}

	protected static bool CalculateOnMobileSmallScreen()
	{
		float dpi = Screen.dpi;
		if (dpi > 0f)
		{
			float w = Screen.width / dpi;
			float h = Screen.height / dpi;
			return Mathf.Sqrt(w * w + h * h) < 6.1f;
		}
		return false;
	}

	public static bool OnMobileSmallScreen()
	{
		// PC port: never a small mobile screen.
		return false;
	}

	public static bool OnMobileLargeScreen()
	{
		return !CalculateOnMobileSmallScreen();
	}
}
