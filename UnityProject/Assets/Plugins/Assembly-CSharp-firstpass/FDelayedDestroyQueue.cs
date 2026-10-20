using System.Threading;
using System;
using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FDelayedDestroyQueue @ 0x16494ec..0x1649f54, Ghidra vaddr).
public class FDelayedDestroyQueue : MonoBehaviour
{
	protected static int mMainThreadID;

	protected static bool mInitialized;

	protected static List<GameObject> mQueue = new List<GameObject>();

	protected static List<IDelayedDispose> mDisposeQueue = new List<IDelayedDispose>();

	public static void Init()
	{
		if (Application.isPlaying)
		{
			GameObject gameObject = new GameObject("zDelayedDestroyQueue");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<FDelayedDestroyQueue>();
		}
	}

	public void Awake()
	{
		mMainThreadID = Thread.CurrentThread.ManagedThreadId;
		mInitialized = true;
	}

	public void Update()
	{
		lock (mQueue)
		{
			foreach (GameObject item in mQueue)
			{
				UnityEngine.Object.Destroy(item);
			}
			mQueue.Clear();
		}
		lock (mDisposeQueue)
		{
			IDelayedDispose[] array = mDisposeQueue.ToArray();
			mDisposeQueue.Clear();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].DelayedDispose();
			}
		}
	}

	public static bool IsMainThread()
	{
		if (!mInitialized)
		{
			FLog.UnityError("FDelayedDestroyQueue not yet initialized");
			throw new InvalidOperationException("FDelayedDestroyQueue not yet initialized");
		}
		return Thread.CurrentThread.ManagedThreadId == mMainThreadID;
	}

	public static void RequestDestroy(GameObject obj, bool alwaysDelay = false)
	{
		lock (mQueue)
		{
			mQueue.Add(obj);
		}
	}

	public static void RequestDelayedDispose(IDelayedDispose obj, bool alwaysDelay = false)
	{
		lock (mDisposeQueue)
		{
			mDisposeQueue.Add(obj);
		}
	}
}
