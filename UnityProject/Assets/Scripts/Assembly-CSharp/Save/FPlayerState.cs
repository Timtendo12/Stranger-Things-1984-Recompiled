using System;
using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FPlayerState @ 0x1817770..0x1818358, Ghidra vaddr).
[Serializable]
public class FPlayerState
{
	public string SerializedOutfits;

	public int SaveVersion;

	public int SaveType;

	public double TotalGameTime;

	public int MaxHealth;

	public int Health;

	public int MaxCoins;

	public int Coins;

	public int MaxKeyCards;

	public int KeyCards;

	public List<string> Collectibles;

	public string CurrentCharacter;

	public string LastCharacter;

	public int MaxRocks;

	public int Rocks;

	public int MaxPudding;

	public int Pudding;

	public int CurrentChapter;

	public string CurrentWorld;

	public string CurrentRoom;

	public List<string> UnlockedDoors;

	public List<int> CurrentQuests;

	public List<int> CompletedQuests;

	public List<string> DeadUnits;

	public List<FProtoUnitStartData> SpawnedUnits;

	public List<string> TemporarilyDeadUnits;

	public List<SavedRoomDesc> VisitedRooms;

	public List<string> DialogueIndices;

	public List<SavedUnitInfoDesc> SavedUnitInfo;

	public FPlayerState()
	{
		TotalGameTime = 0.0;
		SaveVersion = FPlayerStats.cCurrentSaveVersion;
		SaveType = 0;
		MaxHealth = FPlayerStats.cStartingHearts * FPlayerStats.cHitpointsPerHeart;
		Health = FPlayerStats.cStartingHearts * FPlayerStats.cHitpointsPerHeart;
		MaxCoins = 999;
		MaxKeyCards = 99;
		Collectibles = new List<string>();
		CurrentChapter = 0;
		CurrentWorld = "";
		MaxRocks = 20;
		Rocks = 10;
		MaxPudding = 50;
		CurrentRoom = "";
		CurrentCharacter = "Hopper";
		LastCharacter = "";
		UnlockedDoors = new List<string>();
		CurrentQuests = new List<int>();
		CompletedQuests = new List<int>();
		DeadUnits = new List<string>();
		TemporarilyDeadUnits = new List<string>();
		VisitedRooms = new List<SavedRoomDesc>();
		DialogueIndices = new List<string>();
		SavedUnitInfo = new List<SavedUnitInfoDesc>();
		SpawnedUnits = new List<FProtoUnitStartData>();
	}

	public FPlayerState(FPlayerState other)
	{
		SerializedOutfits = other.SerializedOutfits;
		SaveVersion = other.SaveVersion;
		SaveType = other.SaveType;
		TotalGameTime = other.TotalGameTime;
		CurrentCharacter = other.CurrentCharacter;
		MaxCoins = other.MaxCoins;
		Coins = other.Coins;
		MaxHealth = other.MaxHealth;
		Health = other.Health;
		MaxKeyCards = other.MaxKeyCards;
		KeyCards = other.KeyCards;
		CurrentRoom = other.CurrentRoom;
		MaxPudding = other.MaxPudding;
		Pudding = other.Pudding;
		MaxRocks = other.MaxRocks;
		Rocks = other.Rocks;
		LastCharacter = other.LastCharacter;
		CurrentChapter = other.CurrentChapter;
		CurrentWorld = other.CurrentWorld;
		CurrentQuests = new List<int>(other.CurrentQuests);
		CompletedQuests = new List<int>(other.CompletedQuests);
		DeadUnits = new List<string>(other.DeadUnits);
		UnlockedDoors = new List<string>(other.UnlockedDoors);
		Collectibles = new List<string>(other.Collectibles);
		DialogueIndices = new List<string>(other.DialogueIndices);
		VisitedRooms = other.VisitedRooms.ConvertAll((SavedRoomDesc room) => new SavedRoomDesc(room));
		SavedUnitInfo = other.SavedUnitInfo.ConvertAll((SavedUnitInfoDesc unitInfo) => new SavedUnitInfoDesc(unitInfo));
		SpawnedUnits = new List<FProtoUnitStartData>(other.SpawnedUnits);
		TemporarilyDeadUnits = new List<string>();
	}

	public float GetPercentComplete()
	{
		if (FApp.ProtoCollectibleManager == null)
		{
			return 0f;
		}
		return (float)Collectibles.Count / (float)FApp.ProtoCollectibleManager.ProtoCollectibles.Count * 100f;
	}

	public string GetPercentCompleteString(bool singleLine)
	{
		float percentComplete = GetPercentComplete();
		string text = singleLine ? FLoc.Format("PercentCompleteFormatSingleLine", percentComplete) : FLoc.Format("PercentCompleteFormat", percentComplete);
		return text.Replace(".0", "");
	}
}
