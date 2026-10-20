using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

// Rewritten from libil2cpp.so 1.0.605 (FFileWatcher @ 0x16fe4a8..0x16ff10c, Ghidra vaddr). Debug-only hot-reload helper:
// a worker thread polls the registered files, DispatchCoroutine fires the change events on the main thread.
public class FFileWatcher
{
	protected static Dictionary<string, FFileWatcherEntry> mEntries;

	protected static HashSet<FFileWatcherEntry> mEventsToFire;

	protected static object mLock;

	protected static Thread mWorkerThread;

	protected static bool mWorkerShouldRun;

	public static void Init()
	{
		if (mEntries == null)
		{
			mEntries = new Dictionary<string, FFileWatcherEntry>();
			mEventsToFire = new HashSet<FFileWatcherEntry>();
			mLock = new object();
		}
		if (mWorkerThread == null)
		{
			mWorkerShouldRun = true;
			mWorkerThread = new Thread(ThreadWorker);
			mWorkerThread.IsBackground = true;
			mWorkerThread.Start();
		}
	}

	public static void Shutdown()
	{
		mWorkerShouldRun = false;
		mWorkerThread = null;
	}

	public static void Add(string filename, Action onChanged)
	{
		Init();
		lock (mLock)
		{
			if (!mEntries.TryGetValue(filename, out var entry))
			{
				entry = new FFileWatcherEntry();
				entry.Filename = filename;
				entry.CheckLastWriteTime(initial: true);
				mEntries.Add(filename, entry);
			}
			entry.OnChanged += onChanged;
		}
	}

	public static IEnumerator DispatchCoroutine()
	{
		while (true)
		{
			if (mEventsToFire != null)
			{
				List<FFileWatcherEntry> ready = null;
				lock (mLock)
				{
					if (mEventsToFire.Count > 0)
					{
						ready = new List<FFileWatcherEntry>(mEventsToFire);
						mEventsToFire.Clear();
					}
				}
				if (ready != null)
				{
					foreach (FFileWatcherEntry item in ready)
					{
						item.FireChangedEvent();
					}
				}
			}
			yield return null;
		}
	}

	public static void ThreadWorker()
	{
		while (mWorkerShouldRun)
		{
			lock (mLock)
			{
				foreach (FFileWatcherEntry value in mEntries.Values)
				{
					if (value.CheckLastWriteTime())
					{
						mEventsToFire.Add(value);
					}
				}
			}
			Thread.Sleep(1000);
		}
	}
}
