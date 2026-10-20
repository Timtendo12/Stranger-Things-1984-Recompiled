using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (F3DTrackerAtlasHelper @ 0x1683140..0x16835e0, Ghidra vaddr).
public class F3DTrackerAtlasHelper
{
	public static F3DTracker CurrentlyBakingTopNode;

	public static ExternalAtlasTrackingItem CurrentExternalAtlasTrackingItem;

	public static Dictionary<FAtlas, bool> mAllReferencedAtlases;

	public static void IncrementAtlas(FAtlas atlas)
	{
		if (!FAtlasManager.AtlasUnloadingEnabled)
		{
			return;
		}
		mAllReferencedAtlases[atlas] = true;
		if (CurrentExternalAtlasTrackingItem == null)
		{
			return;
		}
		if (CurrentExternalAtlasTrackingItem.mReservedAtlases == null)
		{
			CurrentExternalAtlasTrackingItem.mReservedAtlases = new Dictionary<FAtlas, bool>();
		}
		if (!CurrentExternalAtlasTrackingItem.mReservedAtlases.ContainsKey(atlas))
		{
			atlas.IncrementUseCount(true);
			CurrentExternalAtlasTrackingItem.mReservedAtlases[atlas] = true;
		}
	}

	public static void ResetAtlasUseCount3d()
	{
		foreach (KeyValuePair<FAtlas, bool> item in mAllReferencedAtlases)
		{
			item.Key.mUseCount3d = 0;
		}
		mAllReferencedAtlases.Clear();
	}

	static F3DTrackerAtlasHelper()
	{
		CurrentlyBakingTopNode = null;
		CurrentExternalAtlasTrackingItem = null;
		mAllReferencedAtlases = new Dictionary<FAtlas, bool>();
	}
}
