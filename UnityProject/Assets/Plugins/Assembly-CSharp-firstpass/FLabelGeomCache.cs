using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FLabelGeomCache @ 0x16ad45c..0x16ad648, Ghidra vaddr).
public class FLabelGeomCache
{
	public Dictionary<FTuple<FAtlas, FShader>, FLabelGeomCacheEntry> Entries = new Dictionary<FTuple<FAtlas, FShader>, FLabelGeomCacheEntry>();

	public bool IsDirty = true;

	public void Clear()
	{
		Entries.Clear();
	}

	public FLabelGeomCacheEntry GetOrCreateEntry(FAtlas atlas, FShader shader)
	{
		FTuple<FAtlas, FShader> key = new FTuple<FAtlas, FShader>(atlas, shader);
		FLabelGeomCacheEntry entry = Entries.Get(key);
		if (entry == null)
		{
			entry = new FLabelGeomCacheEntry();
			Entries[key] = entry;
		}
		return entry;
	}
}
