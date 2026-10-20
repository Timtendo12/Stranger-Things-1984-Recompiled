using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

// PC port: no asset bundles exist; all assets come from Unity Resources. Bundle download/preload paths are no-ops (shipped: libil2cpp.so 1.0.605).
public class FAssetBundleManager : MonoBehaviour
{
	public enum States
	{
		NeedToDownload = 0,
		Downloading = 1,
		ErrorDownloading = 2,
		DownloadComplete = 3
	}

	public delegate void LogAssetBundleEventDelegate(string clientActionName, int? phase = null, bool? isBackground = null, long? phaseBytes = null, double? elapsedSec = null, long? bytesDownloaded = null, double? transferRate = null, string bundleName = null, string bundleSha1String = null, long? bundleBytes = null, float? phasePercentComplete = null, string requestURL = null, int? requestResponseCode = null, string requestErrorMessage = null);

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass56_0
	{
		public FAssetBundleEntry entry;

		public byte[] newHash;

		public Exception e;

		internal void _003CLoad_003Eb__0()
		{
		}
	}

	[CompilerGenerated]
	private sealed class _003CBackgroundDownload_003Ed__53 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		private List<int> _003Cphases_003E5__2;

		private List<int> _003CworkingPhases_003E5__3;

		private int _003Ci_003E5__4;

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
		public _003CBackgroundDownload_003Ed__53(int _003C_003E1__state)
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
	private sealed class _003CGetExistingHashes_003Ed__54 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public int phase;

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
		public _003CGetExistingHashes_003Ed__54(int _003C_003E1__state)
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
	private sealed class _003CInit_003Ed__49 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public string filename;

		public LogAssetBundleEventDelegate onLogAssetBundleEvent;

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
		public _003CInit_003Ed__49(int _003C_003E1__state)
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
	private sealed class _003CLoad_003Ed__56 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public int phase;

		private _003C_003Ec__DisplayClass56_0 _003C_003E8__1;

		private DateTime _003CstartTimeUTC_003E5__2;

		private Dictionary<string, FAssetBundleEntry>.ValueCollection.Enumerator _003C_003E7__wrap2;

		private DateTime _003CperBundleStartTimeUTC_003E5__4;

		private FDownloadRequest _003Crequest_003E5__5;

		private DateTime _003CframeStartTime_003E5__6;

		private int _003CtimeTicks_003E5__7;

		private int _003ClastTickBytes_003E5__8;

		private int _003CticksSinceBytesChanged_003E5__9;

		private bool _003CdownloadOk_003E5__10;

		private long _003CaddedBytes_003E5__11;

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
		public _003CLoad_003Ed__56(int _003C_003E1__state)
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

		private void _003C_003Em__Finally1()
		{
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
		}
	}

	[CompilerGenerated]
	private sealed class _003CPreload_003Ed__55 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public int phase;

		private DateTime _003CstartTimeUTC_003E5__2;

		private long _003CphaseBytes_003E5__3;

		private DateTime _003CframeStartTime_003E5__4;

		private float _003CtimePerFrame_003E5__5;

		private Dictionary<string, FAssetBundleEntry>.ValueCollection.Enumerator _003C_003E7__wrap5;

		private FAssetBundleEntry _003Centry_003E5__7;

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
		public _003CPreload_003Ed__55(int _003C_003E1__state)
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

		private void _003C_003Em__Finally1()
		{
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
		}
	}

	protected static FAssetBundleManager mInstance;

	public static Dictionary<string, FAssetBundleEntry> mBundleEntries;

	public static Dictionary<string, AssetBundle> mAssetPathToBundleMap;

	public static Dictionary<string, FAssetBundleEntry> mAssetPathToBundleEntry;

	public static Dictionary<string, string> mSceneNameMap;

	public static Dictionary<string, FAssetBundleEntry> mCanonicalSceneToBundle;

	protected static string mFilename;

	protected static bool mRetryDownload;

	protected static bool mUseStreamingAssets;

	protected static FDownloadRequest mCurrentDownload;

	public static string[] AssetsToNotPreload;

	public static string[] FTUEMaps;

	protected static bool mBackgroundDownloadMode;

	protected static float mBackgroundDownloadBaseProgress;

	protected static float mDownloadRetryTimeout;

	protected static int mLoadPhase;

	protected static Dictionary<int, long> mBytesToDownloadPerPhase;

	public static Action<int> OnDownloadPhaseComplete;

	protected static States mState;

	protected static bool mIntrospectingBundles;

	protected static long mBytesToDownload;

	protected static long mBytesDownloaded;

	protected static long mCurrentDownloadSize;

	protected static string mRootURL;

	public static LogAssetBundleEventDelegate OnLogAssetBundleEvent;

	public static States State => States.DownloadComplete;

	public static bool IsDownloading => false;

	public static bool IsBackgroundDownloading => false;

	public static bool IsFinishedDownloading => true;

	public static bool IsDownloadError => false;

	public static bool IntrospectingBundles => false;

	public static float DownloadPercent => 0f;

	public static float BackgroundDownloadPercent => 0f;

	public static long BytesToDownload => 0L;

	public static long BytesDownloaded => 0L;

	public static long CurrentDownloadSize => 0L;

	[IteratorStateMachine(typeof(_003CInit_003Ed__49))]
	public static IEnumerator Init(string filename, LogAssetBundleEventDelegate onLogAssetBundleEvent)
	{
		return NoopRoutine();
	}

	public static bool CanStartMap(string mapName)
	{
		return true;
	}

	public static float GetPhaseDownloadedPercent(int phase)
	{
		return 0f;
	}

	public static long GetPhaseBytesToDownload(int phase)
	{
		return 0L;
	}

	[IteratorStateMachine(typeof(_003CBackgroundDownload_003Ed__53))]
	public static IEnumerator BackgroundDownload()
	{
		return NoopRoutine();
	}

	[IteratorStateMachine(typeof(_003CGetExistingHashes_003Ed__54))]
	public static IEnumerator GetExistingHashes(int phase)
	{
		return NoopRoutine();
	}

	[IteratorStateMachine(typeof(_003CPreload_003Ed__55))]
	public static IEnumerator Preload(int phase)
	{
		return NoopRoutine();
	}

	[IteratorStateMachine(typeof(_003CLoad_003Ed__56))]
	public static IEnumerator Load(int phase)
	{
		return NoopRoutine();
	}

	public static void RegisterAsyncBundle(FAssetBundleEntry entry)
	{
	}

	public static void RetryDownload()
	{
	}

	public static AssetBundle GetBundle(string bundleName)
	{
		return null;
	}

	public static string GetPlatformString()
	{
		return "Windows";
	}

	public static T Get<T>(string assetPath) where T : UnityEngine.Object
	{
		// PC port: no asset bundles, everything is loaded from Resources.
		return Resources.Load<T>(assetPath);
	}

	public static FAssetLoadResultUnion GetAsync<T>(string assetPath) where T : UnityEngine.Object
	{
		FAssetLoadResultUnion fAssetLoadResultUnion = new FAssetLoadResultUnion();
		fAssetLoadResultUnion.mResourceRequest = Resources.LoadAsync<T>(assetPath);
		return fAssetLoadResultUnion;
	}

	public static void BundleStatus(string search = "")
	{
	}

	public static void UnloadAssetBundles(string search = "")
	{
	}

	private static void UnloadAssetBundle(FAssetBundleEntry entry, bool unloadAllRelatedObjectes = false)
	{
	}

	public static void LoadDelayLoadingAssetBundles(List<string> excludeList = null)
	{
	}

	private static void UpdatePathToBundleMap()
	{
	}

	public static void UnloadBundleByPath(string assetPath, bool unloadAllRelatedObjectes = false)
	{
	}

	public static bool IsAssetDownloaded(string assetPath)
	{
		return true;
	}

	public static T GetAssetFromBundleOnly<T>(string assetPath) where T : UnityEngine.Object
	{
		return Resources.Load<T>(assetPath);
	}

	public static FAssetLoadResultUnion GetAssetFromBundleOnlyAsync<T>(string assetPath) where T : UnityEngine.Object
	{
		return GetAsync<T>(assetPath);
	}

	public static List<T> GetAll<T>(string assetPath) where T : UnityEngine.Object
	{
		return new List<T>(Resources.LoadAll<T>(assetPath));
	}

	protected static void GetAllFromBundleOnly<T>(string assetPathLower, List<T> resultList) where T : UnityEngine.Object
	{
	}

	public static string GetCanonicalSceneName(string sceneName)
	{
		return sceneName;
	}

	public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
	{
		return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, mode);
	}

	public static FAssetBundleEntry GetBundleForScene(string sceneName)
	{
		return null;
	}

	public static void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, mode);
	}

	private static IEnumerator NoopRoutine()
	{
		yield break;
	}
}
