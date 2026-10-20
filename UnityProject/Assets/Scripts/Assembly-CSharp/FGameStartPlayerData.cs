using System.Runtime.Serialization;

// Rewritten from libil2cpp.so 1.0.605 (FGameStartPlayerData @ 0x17030cc..0x17032e0, Ghidra vaddr).
[DataContract]
[SaveVersion(0)]
public class FGameStartPlayerData
{
	[DataMember]
	public string PlayerName;

	public bool LocalPlayer;

	[DataMember]
	public int FactionID = -1;

	[DataMember]
	public string DeckName = string.Empty;

	public FGameStartPlayerData()
	{
		FactionID = -1;
	}

	public void Save(FBinWriter writer)
	{
		writer.BeginChunk("FGameStartPlayerData");
		writer.Write(PlayerName);
		writer.Write(LocalPlayer);
		writer.Write(FactionID);
		writer.Write(DeckName);
		writer.EndChunk("FGameStartPlayerData");
	}

	public void Load(FBinReader reader)
	{
		if (reader.BeginChunk("FGameStartPlayerData"))
		{
			PlayerName = reader.ReadString();
			LocalPlayer = reader.ReadBool();
			FactionID = reader.ReadInt();
			DeckName = reader.ReadString();
			if (reader.EndChunk("FGameStartPlayerData"))
			{
				return;
			}
		}
		throw new FSavegameLoadException();
	}
}
