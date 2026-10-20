using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

// Rewritten from libil2cpp.so 1.0.605 (FGameStartData @ 0x17032e0..0x1704274, Ghidra vaddr).
[DataContract]
[SaveVersion(1)]
public class FGameStartData
{
	public const int cMinVersionForPlayerRandomization = 1;

	[DataMember]
	public Dictionary<int, FGameStartPlayerData> Players = new Dictionary<int, FGameStartPlayerData>();

	public List<int> PlayersToRandomize = new List<int>();

	public int Seed = -1;

	public string GameGUID;

	public bool SkipPregame;

	public FBinReader SavegameReader;

	[DataMember]
	public Guid GameStateID { get; set; }

	[DataMember]
	public string CampaignName { get; set; }

	[DataMember]
	public string MapFilename { get; set; }

	[DataMember]
	public MultiplayerMode MultiplayerMode { get; set; }

	[DataMember]
	public GameModes GameMode { get; set; }

	[DataMember]
	public GameDifficulties GameDifficulty { get; set; }

	[DataMember]
	public Fix64 DifficultyModifier { get; set; }

	[DataMember]
	public GameTypes GameType { get; set; }

	[DataMember]
	public int ChallengeIndex { get; set; }

	[DataMember]
	public int ChallengeIndexDefault { get; set; }

	[DataMember]
	public int ChallengeIndexMin { get; set; }

	[DataMember]
	public int ChallengeIndexMax { get; set; }

	[DataMember]
	public int ChallengeDropshipLevel { get; set; }

	[DataMember]
	public bool TrackGameRecord { get; set; }

	public bool IsSavegame => SavegameReader != null;

	public void SetPlayersToRandomize(int[] playerIDs)
	{
		PlayersToRandomize = new List<int>();
		if (playerIDs == null)
		{
			return;
		}
		for (int i = 0; i < playerIDs.Length; i++)
		{
			int num = playerIDs[i];
			if (!PlayersToRandomize.Contains(num) && Players.ContainsKey(num))
			{
				PlayersToRandomize.Add(num);
			}
		}
	}

	public void RandomizePlayers()
	{
		if (PlayersToRandomize == null || PlayersToRandomize.Count < 2)
		{
			return;
		}
		FRandom random = ((Seed >= 0) ? new FRandom(Seed) : FRandom.Generic);
		List<int> shuffled = new List<int>(PlayersToRandomize);
		shuffled.Shuffle(random);
		Dictionary<int, int> map = new Dictionary<int, int>();
		for (int i = 0; i < PlayersToRandomize.Count; i++)
		{
			map.Add(PlayersToRandomize[i], shuffled[i]);
		}
		Dictionary<int, FGameStartPlayerData> dictionary = new Dictionary<int, FGameStartPlayerData>();
		foreach (KeyValuePair<int, FGameStartPlayerData> player in Players)
		{
			int value = -1;
			if (!map.TryGetValue(player.Key, out value))
			{
				dictionary.Add(player.Key, player.Value);
			}
			else
			{
				dictionary.Add(value, player.Value);
			}
		}
		Players = dictionary;
	}

	public FGameStartData()
	{
		GameDifficulty = GameDifficulties.Normal;
		TrackGameRecord = true;
		DifficultyModifier = Fix64.One;
	}

	public FGameStartData(string campaignname, string filename, GameModes mode, GameDifficulties difficulty = GameDifficulties.Normal)
		: this()
	{
		CampaignName = campaignname;
		MapFilename = filename;
		GameMode = mode;
		GameDifficulty = difficulty;
		TrackGameRecord = true;
		DifficultyModifier = Fix64.One;
	}

	public bool GetIsMultiplayer()
	{
		return GameMode == GameModes.Multiplayer;
	}

	public static FGameStartData CreateGenericStartData(string campaignname, string filename, GameModes mode, GameDifficulties difficulty = GameDifficulties.Normal)
	{
		FGameStartData fGameStartData = new FGameStartData(campaignname, filename, mode, difficulty);
		fGameStartData.DifficultyModifier = Fix64.One;
		fGameStartData.Players[1] = new FGameStartPlayerData();
		return fGameStartData;
	}

	public static FGameStartData CreateGenericStartDataPractice(string filename, GameModes mode)
	{
		FGameStartData fGameStartData = new FGameStartData(null, filename, mode, GameDifficulties.Normal);
		FGameStartPlayerData value = new FGameStartPlayerData();
		fGameStartData.Players[1] = value;
		fGameStartData.TrackGameRecord = false;
		return fGameStartData;
	}

	public FGameStartPlayerData GetData(int playerID)
	{
		return Players.Get(playerID);
	}

	public void Save(FBinWriter writer)
	{
		writer.BeginChunk("FGameStartData");
		writer.Write(CampaignName);
		writer.Write(MapFilename);
		writer.Write(Players.Count);
		foreach (KeyValuePair<int, FGameStartPlayerData> player in Players)
		{
			writer.Write(player.Key);
			player.Value.Save(writer);
		}
		writer.Write(PlayersToRandomize);
		writer.Write(Seed);
		writer.Write(GameGUID);
		writer.Write(DifficultyModifier);
		writer.Write(ChallengeIndex);
		writer.Write(ChallengeIndexMin);
		writer.Write(ChallengeIndexMax);
		writer.Write(ChallengeDropshipLevel);
		writer.Write(TrackGameRecord);
		writer.EndChunk("FGameStartData");
	}

	public void Load(FBinReader reader)
	{
		if (reader.BeginChunk("FGameStartData"))
		{
			CampaignName = reader.ReadString();
			MapFilename = reader.ReadString();
			int num = reader.ReadInt();
			while (num > 0)
			{
				int key = reader.ReadInt();
				FGameStartPlayerData fGameStartPlayerData = new FGameStartPlayerData();
				fGameStartPlayerData.Load(reader);
				Players[key] = fGameStartPlayerData;
				num--;
			}
			PlayersToRandomize = reader.ReadIntList();
			Seed = reader.ReadInt();
			GameGUID = reader.ReadString();
			DifficultyModifier = reader.ReadFix64();
			ChallengeIndex = reader.ReadInt();
			ChallengeIndexMin = reader.ReadInt();
			ChallengeIndexMax = reader.ReadInt();
			ChallengeDropshipLevel = reader.ReadInt();
			TrackGameRecord = reader.ReadBool();
			if (reader.EndChunk("FGameStartData"))
			{
				return;
			}
		}
		throw new FSavegameLoadException();
	}
}
