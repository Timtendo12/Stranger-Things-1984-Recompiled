using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FAssetBundleEntry @ 0x160c110..0x160ced4, Ghidra vaddr).
// PC port: asset bundles are not used (everything is in Resources); only the pure accessors are real.
public class FAssetBundleEntry
{
	public string Name;

	public string Url;

	public string LocalFilename;

	public byte[] Sha1;

	public byte[] ExistingFileSha1;

	public long FileSize;

	public int Phase;

	public bool AlwaysStream;

	public AssetBundle Bundle;

	public bool Unloaded;

	public List<string> mStrippedNames;

	public bool DontPreload;

	public bool FileDownloaded;

	public bool BundleKnownGood;

	public bool DelayLoading;

	public bool ExistingHashFoundThisSession;

	public AssetBundleCreateRequest mBundleRequest;

	public string HashFilename => LocalFilename + ".hash";

	public string Sha1String => FCoreUtil.HexString(Sha1);

	public AssetBundleCreateRequest PreLoadBundleFromLocalFileAsync()
	{
		// PC port: no bundles
		return null;
	}

	public bool LoadBundleFromLocalFile(Dictionary<string, AssetBundle> assetPathToBundleMap, Dictionary<string, string> sceneNameMap, bool quiet, bool forceLoad = false, string otherPath = "")
	{
		// PC port: no bundles
		return false;
	}

	public void LoadExistingHashIfPossible()
	{
		// PC port: no bundles
	}
}
