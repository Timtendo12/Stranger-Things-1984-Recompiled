using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FProtoCollectible @ 0x017c326c..0x017c3478, Ghidra vaddr).
public class FProtoCollectible
{
	public enum Types
	{
		None = 0,
		Gnome = 1,
		HeartPiece = 2,
		Eggo = 3,
		Film = 4,
		Character = 5,
		QuestItem = 6,
		DungeonItem = 7,
		CharacterUpgrade = 8,
		Tool = 9,
		Outfit = 10
	}

	public int DBID { get; protected set; }

	public Types Type { get; protected set; }

	public string Name { get; protected set; }

	public string VisibleName => FLoc.GetString("Collectible." + Name + ".Name", true);

	public string Details => FLoc.GetString("Collectible." + Name + ".Details", true);

	public int NumberHeartPieces { get; protected set; }

	public bool Permanent { get; protected set; }

	public FProtoCollectible()
	{
		Name = "";
		DBID = -1;
		NumberHeartPieces = 0;
		Permanent = false;
	}

	public bool LoadXml(XmlElement root)
	{
		Name = root.GetAttrib("Name", Name);
		string attrib = root.GetAttrib("Type", Type.ToString());
		if (!string.IsNullOrEmpty(attrib))
		{
			Types result = Types.None;
			if (FEnum.TryParse(attrib, ref result))
			{
				Type = result;
			}
		}
		DBID = root.GetAttrib("DBID", DBID);
		NumberHeartPieces = root.GetAttrib("HeartPieces", NumberHeartPieces);
		Permanent = root.GetAttrib("Permanent", Permanent);
		return true;
	}
}
