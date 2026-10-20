using TiledSharp;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FProtoTile @ 0x1821cfc..0x1822014, Ghidra vaddr).
public class FProtoTile
{
	protected string mImagePath;

	public string TileType { get; protected set; } = "Tile";

	public bool Passable { get; protected set; }

	public bool Hole { get; protected set; }

	public bool Load(string imagePath, TmxTileProperties properties)
	{
		if (!imagePath.Valid())
		{
			return false;
		}
		string name = imagePath.Substring(imagePath.IndexOf("Tiles/") + "Tiles/".Length).Split('.')[0];
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		mImagePath = name;
		LoadProperties(properties);
		return true;
	}

	public bool LoadProperties(TmxTileProperties properties)
	{
		if (properties != null)
		{
			Passable = properties.Passable;
			Hole = properties.Hole;
			if (properties.TileType.Valid())
			{
				TileType = properties.TileType;
			}
		}
		return properties != null;
	}

	public string GetSpritePath()
	{
		if (mImagePath.Valid())
		{
			return Application.dataPath + "/Art/Tiles/" + mImagePath + ".png";
		}
		return null;
	}
}
