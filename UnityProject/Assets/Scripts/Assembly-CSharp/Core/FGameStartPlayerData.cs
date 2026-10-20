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
}
