using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FQueuedAssetReqest @ 0x16163f8..0x1617074, Ghidra vaddr).
// PC port: there are no asset bundles; requests are served synchronously from Unity Resources (see FAssetPreloader).
public class FQueuedAssetReqest
{
	public enum eState
	{
		NotRequested = 0,
		BundleRequestWaiting = 1,
		RequestedWaiting = 2,
		Finished = 3,
		Failed = 4,
		DupeRequest = 5
	}

	public bool Async;

	public bool AsyncBundle;

	public string mPath;

	public FAssetLoadResultUnion LoadResultUnion;

	public eState State;

	public int Retries;

	public bool BundleOnly;

	public FAssetBundleEntry BundleEntry;

	public void StartTimer()
	{
	}

	public double GetElapsed()
	{
		return 0.0;
	}

	public void LogTime(int category)
	{
	}

	public FQueuedAssetReqest(string path, bool aysnc)
	{
		mPath = path;
		Async = aysnc;
		AsyncBundle = false;
		LoadResultUnion = new FAssetLoadResultUnion();
		LoadResultUnion.mOwner = this;
		State = eState.NotRequested;
	}

	public void StartBundleRequest(FAssetBundleEntry entry)
	{
		// PC port: no bundles, go straight to the resource request
		BundleEntry = entry;
		MakeRequest();
	}

	public void MakeRequest()
	{
		if (BundleOnly)
		{
			State = eState.Finished;
			return;
		}
		LoadResultUnion.mLoadResult = Resources.Load(mPath);
		State = ((LoadResultUnion.mLoadResult != null) ? eState.Finished : eState.Failed);
	}

	public bool AsyncFizzled()
	{
		return false;
	}

	public void Update(bool checkFizzled)
	{
		if (State == eState.NotRequested || State == eState.BundleRequestWaiting || State == eState.RequestedWaiting)
		{
			MakeRequest();
		}
	}

	private bool Retry()
	{
		Retries++;
		return false;
	}
}
