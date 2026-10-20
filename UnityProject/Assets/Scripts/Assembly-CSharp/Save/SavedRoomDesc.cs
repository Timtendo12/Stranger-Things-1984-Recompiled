using System;

[Serializable]
public class SavedRoomDesc
{
	public string ID { get; set; }

	public int EntranceUsed { get; set; }

	public SavedRoomDesc()
	{
	}

	public SavedRoomDesc(SavedRoomDesc other)
	{
		ID = other.ID;
		EntranceUsed = other.EntranceUsed;
	}
}
