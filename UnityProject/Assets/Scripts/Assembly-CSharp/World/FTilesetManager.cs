using System.Collections.Generic;
using Newtonsoft.Json;
using TiledSharp;

// Rewritten from libil2cpp.so 1.0.605 (FTilesetManager @ 0x1719c24..0x1719ebc, Ghidra vaddr).
public static class FTilesetManager
{
	private static Dictionary<string, Dictionary<int, FProtoTile>> mTilesets = new Dictionary<string, Dictionary<int, FProtoTile>>();

	public static Dictionary<string, Dictionary<int, FProtoTile>> Tilesets => mTilesets;

	public static Dictionary<int, FProtoTile> GetTileset(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		if (!mTilesets.ContainsKey(name))
		{
			return null;
		}
		return mTilesets[name];
	}

	public static bool AddTileset(string name)
	{
		if (mTilesets.ContainsKey(name))
		{
			return false;
		}
		TmxTileset tileset = JsonConvert.DeserializeObject<TmxTileset>(FUtil.LoadTextAsset("Data/Tilesets/" + name));
		if (tileset == null || tileset.tiles == null || tileset.tileproperties == null)
		{
			return false;
		}
		mTilesets[name] = new Dictionary<int, FProtoTile>();
		foreach (KeyValuePair<string, TmxTilesetTile> pair in tileset.tiles)
		{
			int id = int.Parse(pair.Key);
			TmxTileProperties props;
			if (!tileset.tileproperties.TryGetValue(pair.Key, out props))
			{
				props = null;
			}
			mTilesets[name][id] = new FProtoTile();
			mTilesets[name][id].Load(pair.Value.image, props);
		}
		return true;
	}
}
