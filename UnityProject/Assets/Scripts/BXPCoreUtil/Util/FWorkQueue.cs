using System;
using System.Collections.Generic;
using System.Threading;

// Rewritten from libil2cpp.so 1.0.605 (FWorkQueue @ 0x18901f4..0x18909b8, Ghidra vaddr).
public class FWorkQueue
{
	protected class FWorkItem
	{
		public Action WorkAction;

		public Action WorkDoneAction;
	}

	protected List<Thread> mThreads = new List<Thread>();

	protected bool mWorkersShouldRun = true;

	protected Queue<FWorkItem> mQueue = new Queue<FWorkItem>();

	protected FDispatcher mDispatcher = new FDispatcher();

	public FWorkQueue(int threadCount)
	{
#if UNITY_WEBGL && !UNITY_EDITOR
		// port guard: browsers have no threads; DoWork runs the work inline instead.
		threadCount = 0;
#endif
		for (int i = 0; i < threadCount; i++)
		{
			Thread thread = new Thread(ThreadWorker);
			// port guard: worker threads must not keep the Windows player alive after Application.Quit
			thread.IsBackground = true;
			thread.Start();
			mThreads.Add(thread);
		}
	}

	~FWorkQueue()
	{
		// port guard: joining the worker threads from the finalizer at player shutdown crashes the Mono runtime on Windows
		mWorkersShouldRun = false;
	}

	public void DoWork(Action workAction, Action workDoneAction = null)
	{
		FWorkItem item = new FWorkItem();
		item.WorkAction = workAction;
		item.WorkDoneAction = workDoneAction;
		if (mThreads.Count == 0)
		{
			// port guard (web): no worker threads, so do the work now and keep the done callback on Update
			item.WorkAction?.Invoke();
			if (item.WorkDoneAction != null)
			{
				mDispatcher.Invoke(item.WorkDoneAction);
			}
			return;
		}
		lock (mQueue)
		{
			mQueue.Enqueue(item);
			Monitor.Pulse(mQueue);
		}
	}

	public void Update()
	{
		mDispatcher.ExecutePendingActions(loopTilQueueIsEmpty: true);
	}

	protected void ThreadWorker()
	{
		while (mWorkersShouldRun)
		{
			FWorkItem fWorkItem = null;
			lock (mQueue)
			{
				while (true)
				{
					if (mQueue.Count > 0)
					{
						fWorkItem = mQueue.Dequeue();
						break;
					}
					if (!mWorkersShouldRun)
					{
						break;
					}
					Monitor.Wait(mQueue);
				}
			}
			if (fWorkItem != null)
			{
				if (fWorkItem.WorkAction != null)
				{
					fWorkItem.WorkAction();
				}
				if (fWorkItem.WorkDoneAction != null)
				{
					mDispatcher.Invoke(fWorkItem.WorkDoneAction);
				}
			}
		}
	}

	public void Shutdown()
	{
		mWorkersShouldRun = false;
		lock (mQueue)
		{
			Monitor.PulseAll(mQueue);
		}
		foreach (Thread mThread in mThreads)
		{
			mThread.Join();
		}
	}
}
