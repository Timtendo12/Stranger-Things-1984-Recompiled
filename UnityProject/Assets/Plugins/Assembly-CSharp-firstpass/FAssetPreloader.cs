using System;
using System.Collections.Generic;
using UnityEngine;

// PC port: simplified preloader over Unity Resources (shipped: libil2cpp.so 1.0.605).
public class FAssetPreloader
{
	private class MultiRef
	{
		public WeakReference mWeakRef;

		public object mDirectRef;

		public bool mKeepAlive;

		public MultiRef(object obj, bool keepAlive)
		{
			mWeakRef = new WeakReference(obj);
			mKeepAlive = keepAlive;
			if (keepAlive)
			{
				mDirectRef = obj;
			}
		}

		public void UpdateRef(object obj)
		{
			mWeakRef = new WeakReference(obj);
			if (mKeepAlive)
			{
				mDirectRef = obj;
			}
		}

		public void UpgradeToDirectRef()
		{
			mKeepAlive = true;
			mDirectRef = mWeakRef.Target;
		}

		public void ClearDirectRef()
		{
			mKeepAlive = false;
			mDirectRef = null;
		}

		public bool IsAlive()
		{
			return mDirectRef != null || mWeakRef.IsAlive;
		}

		public T GetRef<T>() where T : class
		{
			return (mDirectRef ?? mWeakRef.Target) as T;
		}
	}

	private static Dictionary<string, MultiRef> mPreloadedResources = new Dictionary<string, MultiRef>();

	private static List<FQueuedAssetReqest> mPreloadQueue = new List<FQueuedAssetReqest>();

	public static bool Log;

	public static void UpdateQueue()
	{
		// PC port: assets load synchronously from Resources; nothing is queued.
	}

	public static void ClearRecords()
	{
		mPreloadedResources.Clear();
		mPreloadQueue.Clear();
	}

	public static int GetQueueLength()
	{
		return mPreloadQueue.Count;
	}

	public static UnityEngine.Object GetPreloadedAsset(string path)
	{
		return GetPreloaded(path);
	}

	public static FAssetLoadResultUnion GetPreloadedAssetAsync(string path)
	{
		UnityEngine.Object preloaded = GetPreloaded(path);
		if (preloaded == null)
		{
			return null;
		}
		FAssetLoadResultUnion result = new FAssetLoadResultUnion();
		result.mLoadResult = preloaded;
		return result;
	}

	public static void RegisterLoaded(string path, UnityEngine.Object loaded, bool preloaded)
	{
		SetPreloaded(path, loaded, preloaded);
	}

	public static UnityEngine.Object PreloadAsset(string path)
	{
		UnityEngine.Object obj = GetPreloaded(path);
		if (obj == null)
		{
			obj = Resources.Load(path);
			SetPreloaded(path, obj, preloaded: true);
		}
		return obj;
	}

	public static FQueuedAssetReqest GetFromQueue(string path)
	{
		return null;
	}

	public static void QueuePreloadAsset(string path, bool async)
	{
		PreloadAsset(path);
	}

	public static void QueuePreloadAssetBundle(FAssetBundleEntry entry)
	{
		// PC port: no asset bundles
	}

	public static void ClearMultiRefs()
	{
		mPreloadedResources.Clear();
	}

	private static UnityEngine.Object GetPreloaded(string path)
	{
		if (path != null && mPreloadedResources.TryGetValue(path.ToLowerInvariant(), out var value))
		{
			return value.GetRef<UnityEngine.Object>();
		}
		return null;
	}

	private static void SetPreloaded(string path, UnityEngine.Object obj, bool preloaded)
	{
		if (path != null && obj != null)
		{
			string key = path.ToLowerInvariant();
			if (mPreloadedResources.TryGetValue(key, out var value))
			{
				value.UpdateRef(obj);
			}
			else
			{
				mPreloadedResources[key] = new MultiRef(obj, preloaded);
			}
		}
	}
}
