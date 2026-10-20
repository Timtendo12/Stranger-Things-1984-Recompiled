using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (ExternalAtlasTrackingItem @ 0x1682e18..0x1683140, Ghidra vaddr).
public class ExternalAtlasTrackingItem
{
	public Dictionary<FAtlas, bool> mReservedAtlases;

	public void Release()
	{
		if (FAtlasManager.AtlasUnloadingEnabled && mReservedAtlases != null)
		{
			foreach (KeyValuePair<FAtlas, bool> item in mReservedAtlases)
			{
				item.Key.DecrementUseCount(true);
			}
			mReservedAtlases.Clear();
		}
	}
}
