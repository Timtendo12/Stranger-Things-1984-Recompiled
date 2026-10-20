// Static defaults rewritten from libil2cpp.so 1.0.605 (FConfigCore..cctor @ 0x1648634, Ghidra vaddr).
public class FConfigCore
{
	static FConfigCore()
	{
		WinMouseHack = true;
		EventBasedMouse = true;
		OldTouchManager = false;
		LowResSupport = true;
		AssetBundleUnloading = true;
		AbFX = true;
		AbSound = true;
		AbUnits = true;
		AbAtlas = true;
		AbAtlasStartOnly = true;
		AbDelayLoad = false;
		AssetJITPreload = true;
		JITUnits = true;
		JITSounds = true;
		JITLog = false;
		AbAsync = true;
		JITMaxCache = false;
		JITTimeout = 2f;
		JITImproved = true;
		LoadOnDemandFX = true;
		LoadOnDemandSound = true;
		LoadIfAvailable = false;
		AtlasUnloading = true;
		CompressedAtlases = true;
		LowMemTest = false;
		DisableConsoleSpew = false;
		BackgroundDownloads = true;
		BundleDownloadTimeout = 20;
		DynamicFonts = true;
		HybridDynamicFonts = false;
		DynamicFontsFallback = false;
		DynamicSuperSample = false;
		DynamicMaxFonts = false;
		SoundFix = true;
		SoundDataUnloading = false;
		TestDisable7330 = false;
		GCFix1 = true;
		LoadDelayFix = true;
		DisableDynamicIcons = false;
		PoolFLetterQuad = false;
		PreloadAllBundles = false;
		LoadDelayedBundles = true;
		SceneDelayLoad = true;
		QueuePreloadAssetBundle = false;
	}

	public static bool WinMouseHack;

	public static bool EventBasedMouse;

	public static bool LabelGeomCache;

	public static string AssetBundleRootURL;

	public static bool IgnoreStreamingAssetBundles;

	public static bool OldTouchManager;

	public static bool LowResSupport;

	public static bool AssetBundleUnloading;

	public static bool AbFX;

	public static bool AbSound;

	public static bool AbUnits;

	public static bool AbAtlas;

	public static bool AbAtlasStartOnly;

	public static bool AbDelayLoad;

	public static bool AssetJITPreload;

	public static bool JITUnits;

	public static bool JITSounds;

	public static bool JITLog;

	public static bool AbAsync;

	public static bool JITMaxCache;

	public static float JITTimeout;

	public static bool JITImproved;

	public static bool LoadOnDemandFX;

	public static bool LoadOnDemandSound;

	public static bool LoadIfAvailable;

	public static bool AtlasUnloading;

	public static bool CompressedAtlases;

	public static bool LowMemTest;

	public static bool DisableConsoleSpew;

	public static bool BackgroundDownloads;

	public static int BundleDownloadTimeout;

	public static bool DynamicFonts;

	public static bool HybridDynamicFonts;

	public static bool DynamicFontsFallback;

	public static bool DynamicSuperSample;

	public static bool DynamicMaxFonts;

	public static bool SoundFix;

	public static bool SoundDataUnloading;

	public static bool TestDisable7330;

	public static bool GCFix1;

	public static bool LoadDelayFix;

	public static bool DisableDynamicIcons;

	public static bool PoolFLetterQuad;

	public static bool PreloadAllBundles;

	public static bool LoadDelayedBundles;

	public static bool SceneDelayLoad;

	public static bool QueuePreloadAssetBundle;

	public static bool ObfuscateAndCompressInEditor { get; set; }

	public static bool FailOnBadSignatureInEditor { get; set; }

	public static bool ForceUnityLog { get; set; }
}
