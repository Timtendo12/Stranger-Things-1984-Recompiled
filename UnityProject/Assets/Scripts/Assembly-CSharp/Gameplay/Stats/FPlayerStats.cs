using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

// Rewritten from libil2cpp.so 1.0.605 (FPlayerStats @ 0x1818358..0x181f200, Ghidra vaddr).
public class FPlayerStats : IRequirementManager<FPlayerStats>
{
	public enum SaveTypes
	{
		NORMAL = 0,
		CLASSIC = 1
	}

	public static int cCurrentSaveVersion = 6;

	public static int cHeartPiecesPerHeart = 4;

	public static int cStartingHearts = 5;

	public static int cHitpointsPerHeart = 1;

	public static int cMaximumMaxHealth = 16;

	protected string mName;

	protected FPlayerState mCurrentPlayerState;

	protected List<FRoom> mPendingSaveRooms = new List<FRoom>();

	protected List<FPlayerState> mSavedPlayerStates = new List<FPlayerState>();

	protected int mCurrentSaveSlot;

	public bool HadOutOfDateSaves;

	protected Action<FPlayerState> mStatsUpdated;

	public static bool CanSaveState
	{
		get
		{
			if (FApp.World != null && FApp.World.InBossEncounter)
			{
				return false;
			}
			return true;
		}
	}

	public string Name { get; set; }

	public SaveTypes SaveType
	{
		get
		{
			return (SaveTypes)mCurrentPlayerState.SaveType;
		}
		set
		{
			mCurrentPlayerState.SaveType = (int)value;
		}
	}

	public double TotalGameTime
	{
		get
		{
			double result = 0.0;
			if (mCurrentSaveSlot >= 0 && mCurrentSaveSlot < mSavedPlayerStates.Count && mSavedPlayerStates[mCurrentSaveSlot] != null)
			{
				result = mSavedPlayerStates[mCurrentSaveSlot].TotalGameTime;
			}
			return result;
		}
		set
		{
			if (mCurrentSaveSlot >= 0 && mCurrentSaveSlot < mSavedPlayerStates.Count && mSavedPlayerStates[mCurrentSaveSlot] != null)
			{
				mSavedPlayerStates[mCurrentSaveSlot].TotalGameTime = value;
				mCurrentPlayerState.TotalGameTime = value;
			}
		}
	}

	public int MaxHealth
	{
		get
		{
			return mCurrentPlayerState.MaxHealth;
		}
		set
		{
			mCurrentPlayerState.MaxHealth = value;
			TryCallStatsUpdated();
		}
	}

	public int Health
	{
		get
		{
			return mCurrentPlayerState.Health;
		}
		set
		{
			SetHealth(value);
		}
	}

	public int MaxCoins
	{
		get
		{
			return mCurrentPlayerState.MaxCoins;
		}
		set
		{
			mCurrentPlayerState.MaxCoins = value;
		}
	}

	public int Coins
	{
		get
		{
			return mCurrentPlayerState.Coins;
		}
		set
		{
			SetCoins(value);
		}
	}

	public int MaxKeyCards
	{
		get
		{
			return mCurrentPlayerState.MaxKeyCards;
		}
		set
		{
			mCurrentPlayerState.MaxKeyCards = value;
		}
	}

	public int KeyCards
	{
		get
		{
			return mCurrentPlayerState.KeyCards;
		}
		set
		{
			SetKeyCards(value);
		}
	}

	public int MaxRocks
	{
		get
		{
			return mCurrentPlayerState.MaxRocks;
		}
		set
		{
			mCurrentPlayerState.MaxRocks = value;
			TryCallStatsUpdated();
		}
	}

	public int Rocks
	{
		get
		{
			return mCurrentPlayerState.Rocks;
		}
		set
		{
			SetRocks(value);
		}
	}

	public int MaxPudding
	{
		get
		{
			return mCurrentPlayerState.MaxPudding;
		}
		set
		{
			mCurrentPlayerState.MaxPudding = value;
		}
	}

	public int Pudding
	{
		get
		{
			return mCurrentPlayerState.Pudding;
		}
		set
		{
			SetPudding(value);
		}
	}

	public int CurrentChapter
	{
		get
		{
			return mCurrentPlayerState.CurrentChapter;
		}
		set
		{
			mCurrentPlayerState.CurrentChapter = value;
		}
	}

	public string CurrentCharacter
	{
		get
		{
			return mCurrentPlayerState.CurrentCharacter;
		}
		set
		{
			mCurrentPlayerState.LastCharacter = mCurrentPlayerState.CurrentCharacter;
			mCurrentPlayerState.CurrentCharacter = value;
			TryCallStatsUpdated();
		}
	}

	public string LastCharacter
	{
		get
		{
			return mCurrentPlayerState.LastCharacter;
		}
		set
		{
			mCurrentPlayerState.LastCharacter = value;
		}
	}

	public string CurrentWorld
	{
		get
		{
			return mCurrentPlayerState.CurrentWorld;
		}
		set
		{
			mCurrentPlayerState.CurrentWorld = value;
		}
	}

	public string CurrentRoom
	{
		get
		{
			return mCurrentPlayerState.CurrentRoom;
		}
		set
		{
			mCurrentPlayerState.CurrentRoom = value;
		}
	}

	public List<string> UnlockedDoors
	{
		get
		{
			return mCurrentPlayerState.UnlockedDoors;
		}
		set
		{
			mCurrentPlayerState.UnlockedDoors = value;
		}
	}

	public List<string> Collectibles
	{
		get
		{
			return mCurrentPlayerState.Collectibles;
		}
		set
		{
			mCurrentPlayerState.Collectibles = value;
		}
	}

	public List<int> CurrentQuests
	{
		get
		{
			return mCurrentPlayerState.CurrentQuests;
		}
		set
		{
			mCurrentPlayerState.CurrentQuests = value;
		}
	}

	public List<int> CompletedQuests
	{
		get
		{
			return mCurrentPlayerState.CompletedQuests;
		}
		set
		{
			mCurrentPlayerState.CompletedQuests = value;
		}
	}

	public List<string> DeadUnits
	{
		get
		{
			return mCurrentPlayerState.DeadUnits;
		}
		set
		{
			mCurrentPlayerState.DeadUnits = value;
		}
	}

	public List<string> TemporarilyDeadUnits
	{
		get
		{
			return mCurrentPlayerState.TemporarilyDeadUnits;
		}
		set
		{
			mCurrentPlayerState.TemporarilyDeadUnits = value;
		}
	}

	public List<SavedRoomDesc> VisitedRooms
	{
		get
		{
			return mCurrentPlayerState.VisitedRooms;
		}
		set
		{
			mCurrentPlayerState.VisitedRooms = value;
		}
	}

	public List<string> DialogueIndices
	{
		get
		{
			return mCurrentPlayerState.DialogueIndices;
		}
		set
		{
			mCurrentPlayerState.DialogueIndices = value;
		}
	}

	public List<SavedUnitInfoDesc> SavedUnitInfo
	{
		get
		{
			return mCurrentPlayerState.SavedUnitInfo;
		}
		set
		{
			mCurrentPlayerState.SavedUnitInfo = value;
		}
	}

	public List<FProtoUnitStartData> SpawnedUnits
	{
		get
		{
			return mCurrentPlayerState.SpawnedUnits;
		}
		set
		{
			mCurrentPlayerState.SpawnedUnits = value;
		}
	}

	public FPlayerState CurrentPlayerState => mCurrentPlayerState;

	public int NumberOfGnomes
	{
		get
		{
			int num = 0;
			if (FApp.ProtoCollectibleManager == null || mCurrentPlayerState.Collectibles == null || mCurrentPlayerState.Collectibles.Count == 0)
			{
				return num;
			}
			foreach (string collectible in mCurrentPlayerState.Collectibles)
			{
				FProtoCollectible protoCollectible = FApp.ProtoCollectibleManager.GetProtoCollectible(collectible);
				if (protoCollectible != null && protoCollectible.Type == FProtoCollectible.Types.Gnome)
				{
					num++;
				}
			}
			return num;
		}
	}

	public bool IsAtMaxHealth => mCurrentPlayerState.Health >= mCurrentPlayerState.MaxHealth;

	public List<FPlayerState> SavedPlayerStates
	{
		get
		{
			return mSavedPlayerStates;
		}
		set
		{
			mSavedPlayerStates = value;
		}
	}

	public int CurrentSaveSlot
	{
		get
		{
			return mCurrentSaveSlot;
		}
		set
		{
			mCurrentSaveSlot = value;
		}
	}

	public event Action<FPlayerState> StatsUpdatedAction
	{
		add
		{
			mStatsUpdated = (Action<FPlayerState>)Delegate.Remove(mStatsUpdated, value);
			mStatsUpdated = (Action<FPlayerState>)Delegate.Combine(mStatsUpdated, value);
		}
		remove
		{
			mStatsUpdated = (Action<FPlayerState>)Delegate.Remove(mStatsUpdated, value);
		}
	}

	public FPlayerStats()
	{
		FLoc.SpecialXMLNodeFunc += ParseSpecialXMLTag;
	}

	public void Init()
	{
		FullWipePlayerState();
		SetCurrentPlayerState(new FPlayerState());
	}

	protected void SetCurrentPlayerState(FPlayerState state)
	{
		mCurrentPlayerState = state;
		if (state != null)
		{
			ApplySpecialEffects();
			if (FAnalytics.Initialized)
			{
				FAnalytics.Sync();
			}
		}
	}

	public void AddPendingSaveRoom(FRoom room)
	{
		if (!(room == null) && !mPendingSaveRooms.Contains(room))
		{
			mPendingSaveRooms.Add(room);
		}
	}

	public void SavePlayerState(FRoom currentRoom = null, bool savePlayerData = true)
	{
		if (!CanSaveState)
		{
			return;
		}
		if (currentRoom != null)
		{
			if (currentRoom.World.ProtoWorld.NoSave)
			{
				return;
			}
			AddPendingSaveRoom(currentRoom);
			SavePendingRooms();
		}
		ApplySpecialEffects();
		mCurrentPlayerState.SerializedOutfits = OutfitHandler.Serialize();
		if (FApp.DialogManager != null)
		{
			FApp.DialogManager.SaveDialogueIndices();
		}
		int count = mSavedPlayerStates.Count;
		int num = mCurrentSaveSlot;
		FPlayerState item = new FPlayerState(mCurrentPlayerState);
		if (num < count)
		{
			mSavedPlayerStates[num] = item;
		}
		else
		{
			mSavedPlayerStates.Add(item);
		}
		if (savePlayerData && FApp.PlayerData != null)
		{
			FApp.PlayerData.Save();
		}
	}

	protected void SavePendingRooms()
	{
		foreach (FRoom mPendingSaveRoom in mPendingSaveRooms)
		{
			SaveRoom(mPendingSaveRoom);
		}
		mPendingSaveRooms.Clear();
	}

	protected void SaveRoom(FRoom room, bool saveUnitStates = true)
	{
		if (saveUnitStates)
		{
			SaveUnitStates(room);
		}
		string roomIdentifier = GetRoomIdentifier(room);
		if (!HasVisitedRoom(GetRoomIdentifier(room)))
		{
			mCurrentPlayerState.VisitedRooms.Add(new SavedRoomDesc
			{
				ID = roomIdentifier
			});
		}
		for (int i = 0; i < mCurrentPlayerState.VisitedRooms.Count; i++)
		{
			if (mCurrentPlayerState.VisitedRooms[i].ID == roomIdentifier)
			{
				mCurrentPlayerState.VisitedRooms[i].EntranceUsed = room.EntranceUsed;
			}
		}
		bool flag = room;
		FProtoWorld protoWorld = FApp.WorldManager.GetProtoWorld(mCurrentPlayerState.CurrentWorld);
		if (mCurrentPlayerState.SaveType == 1 && protoWorld != null)
		{
			if (!flag || room.World.ProtoWorld.MapPack == protoWorld.MapPack)
			{
				return;
			}
		}
		else if (!flag)
		{
			return;
		}
		mCurrentPlayerState.CurrentRoom = room.ProtoRoom.Name;
		mCurrentPlayerState.CurrentWorld = room.World.ProtoWorld.Name;
	}

	public void HardSaveRoom(FRoom room, bool saveUnitStates = true)
	{
		SaveRoom(room, saveUnitStates);
	}

	public void AddSpawnedUnit(FProtoUnitStartData pusd)
	{
		if (!mCurrentPlayerState.SpawnedUnits.Any((FProtoUnitStartData su) => GetUnitIdentifier(su, su.Room, su.World) == GetUnitIdentifier(pusd, pusd.Room, pusd.World)))
		{
			mCurrentPlayerState.SpawnedUnits.Add(pusd);
		}
	}

	protected void SaveUnitStates(FRoom currentRoom)
	{
		if (currentRoom == null)
		{
			return;
		}
		List<FProtoUnitStartData> list = currentRoom.ProtoRoom.ProtoUnitStartDatas;
		if (!currentRoom.World.ProtoWorld.SaveUnitStates)
		{
			list = list.Where((FProtoUnitStartData pusd) => pusd.ProtoUnit != null && pusd.GetProtoUnit.IsTag("ForceSaveState")).ToList();
		}
		foreach (FProtoUnitStartData item in list)
		{
			SaveUnitState(currentRoom, item);
		}
		FProtoUnitStartData[] spawnedUnitsForRoom = GetSpawnedUnitsForRoom(currentRoom);
		for (int i = 0; i < spawnedUnitsForRoom.Length; i++)
		{
			SaveUnitState(currentRoom, spawnedUnitsForRoom[i]);
		}
		List<FProtoDoorStartData> list2 = currentRoom.ProtoRoom.ProtoDoorStartDatas;
		if (!currentRoom.World.ProtoWorld.SaveUnitStates)
		{
			list2 = list2.Where((FProtoDoorStartData pusd) => pusd.ProtoUnit != null && pusd.GetProtoUnit.IsTag("ForceSaveState")).Select((FProtoDoorStartData pusd) => pusd).ToList();
		}
		foreach (FProtoDoorStartData item2 in list2)
		{
			SaveUnitState(currentRoom, item2);
		}
	}

	public FProtoUnitStartData[] GetSpawnedUnitsForRoom(FRoom currentRoom)
	{
		return mCurrentPlayerState.SpawnedUnits.Where((FProtoUnitStartData pusd) => pusd.World == currentRoom.World.ProtoWorld.Name && pusd.Room == currentRoom.ProtoRoom.Name).ToArray();
	}

	protected void SaveUnitState(FRoom currentRoom, FProtoUnitStartData pusd)
	{
		string unitIdentifier = GetUnitIdentifier(pusd, currentRoom);
		FUnit unitByUniqueID = currentRoom.GetUnitByUniqueID(unitIdentifier);
		if (unitByUniqueID != null && unitByUniqueID.AliveAndWell)
		{
			return;
		}
		bool flag = mCurrentPlayerState.DeadUnits.Contains(unitIdentifier);
		bool flag2 = pusd.GetProtoUnit.CheckSpawnRequirements() && !flag;
		bool flag3;
		if (mCurrentPlayerState.SaveType == 1 && !currentRoom.ProtoRoom.EnemyEncounterRoom && pusd.GetProtoUnit.IsTag("EnemyCharacter"))
		{
			flag3 = pusd.GetProtoUnit.IsTag("NoRespawnInClassic") && flag2;
		}
		else
		{
			flag3 = flag2;
		}
		if (flag3)
		{
			mCurrentPlayerState.DeadUnits.Add(unitIdentifier);
		}
		else if (!mCurrentPlayerState.TemporarilyDeadUnits.Contains(unitIdentifier))
		{
			mCurrentPlayerState.TemporarilyDeadUnits.Add(unitIdentifier);
		}
	}

	public bool HasVisitedRoom(FRoom room)
	{
		return HasVisitedRoom(GetRoomIdentifier(room));
	}

	public bool HasVisitedRoom(string roomIdentifier)
	{
		return mCurrentPlayerState.VisitedRooms.Any((SavedRoomDesc r) => r.ID == roomIdentifier);
	}

	public SavedRoomDesc GetRoomDesc(string roomIdentifier)
	{
		return mCurrentPlayerState.VisitedRooms.FirstOrDefault((SavedRoomDesc r) => r.ID == roomIdentifier);
	}

	public bool IsUnitAlive(FUnit unit)
	{
		if (!HasVisitedRoom(GetRoomIdentifier(unit.Room)))
		{
			return true;
		}
		return !mCurrentPlayerState.DeadUnits.Contains(GetUnitIdentifier(unit));
	}

	public bool IsUnitAlive(FProtoUnitStartData pusd, string room, string world)
	{
		if (!HasVisitedRoom(GetRoomIdentifier(room, world)))
		{
			return true;
		}
		return !mCurrentPlayerState.DeadUnits.Contains(GetUnitIdentifier(pusd, room, world));
	}

	public bool IsUnitAlive(FProtoUnitStartData pusd, FRoom room)
	{
		if (!HasVisitedRoom(GetRoomIdentifier(room)))
		{
			return true;
		}
		return !mCurrentPlayerState.DeadUnits.Contains(GetUnitIdentifier(pusd, room));
	}

	public string GetSavedUnitInfo(FUnit unit, string key)
	{
		if (!ShouldSaveUnitInfo(unit))
		{
			return null;
		}
		string unitIdentifier = GetUnitIdentifier(unit);
		SavedUnitInfoDesc savedUnitInfoDesc = mCurrentPlayerState.SavedUnitInfo.FirstOrDefault((SavedUnitInfoDesc desc) => desc.UnitIdentifier == unitIdentifier);
		if (savedUnitInfoDesc != null)
		{
			return savedUnitInfoDesc.GetValue(key);
		}
		return null;
	}

	public void SetSavedUnitInfo(FUnit unit, string key, string value)
	{
		if (!ShouldSaveUnitInfo(unit))
		{
			return;
		}
		string unitIdentifier = GetUnitIdentifier(unit);
		SavedUnitInfoDesc savedUnitInfoDesc = mCurrentPlayerState.SavedUnitInfo.FirstOrDefault((SavedUnitInfoDesc desc) => desc.UnitIdentifier == unitIdentifier);
		if (savedUnitInfoDesc == null)
		{
			savedUnitInfoDesc = new SavedUnitInfoDesc();
			savedUnitInfoDesc.UnitIdentifier = unitIdentifier;
			mCurrentPlayerState.SavedUnitInfo.Add(savedUnitInfoDesc);
		}
		savedUnitInfoDesc.Set(key, value);
	}

	protected bool ShouldSaveUnitInfo(FUnit unit)
	{
		if (unit.Room != null && unit.Room.IsBossRoom())
		{
			return false;
		}
		return true;
	}

	public string GetUnitIdentifier(FUnit unit)
	{
		return GetUnitIdentifier(unit.StartData, unit.Room);
	}

	public string GetUnitIdentifier(FProtoUnitStartData unit, FRoom room)
	{
		return GetRoomIdentifier(room.ProtoRoom.Name, room.World.ProtoWorld.Name) + "|" + unit.Name + "|" + unit.Position.ToStringSimple();
	}

	public string GetUnitIdentifier(FProtoUnitStartData unit, string room, string world)
	{
		return GetRoomIdentifier(room, world) + "|" + unit.Name + "|" + unit.Position.ToStringSimple();
	}

	public string GetRoomIdentifier(FRoom room)
	{
		return GetRoomIdentifier(room.ProtoRoom.Name, room.World.ProtoWorld.Name);
	}

	public string GetRoomIdentifier(string room, string world)
	{
		return world + "|" + room;
	}

	public void Reset()
	{
		if (mCurrentSaveSlot < mSavedPlayerStates.Count)
		{
			SetCurrentPlayerState(new FPlayerState(mSavedPlayerStates[mCurrentSaveSlot]));
			if (FApp.DialogManager != null)
			{
				FApp.DialogManager.LoadDialogueIndices();
			}
			CheckLoadedPlayerState();
			TryCallStatsUpdated();
		}
	}

	protected void CheckLoadedPlayerState()
	{
		if (mCurrentPlayerState.Health <= 2)
		{
			SetHealth(3);
		}
	}

	public void WipePlayerState()
	{
		Reset();
		int num = mCurrentSaveSlot;
		if (num < mSavedPlayerStates.Count)
		{
			mSavedPlayerStates[num] = new FPlayerState();
			SetCurrentPlayerState(mSavedPlayerStates[mCurrentSaveSlot]);
			if (FApp.DialogManager != null)
			{
				FApp.DialogManager.LoadDialogueIndices();
			}
		}
	}

	public void FullWipePlayerState()
	{
		mSavedPlayerStates.Clear();
		mCurrentSaveSlot = 0;
	}

	public FLootDesc[] RollLootTable(FLootTable lootTable)
	{
		if (lootTable == null)
		{
			return new FLootDesc[0];
		}
		List<FLootDesc> list = new List<FLootDesc>();
		lootTable.Evaluate(FApp.PlayerStats, list, FRandom.Generic);
		return list.ToArray();
	}

	public void RollAndApplyLootTable(FLootTable lootTable)
	{
		FLootDesc[] array = RollLootTable(lootTable);
		for (int i = 0; i < array.Length; i++)
		{
			EvaluateLoot(array[i]);
		}
	}

	public void EvaluateLoot(FLootDesc lootDesc)
	{
		string name = lootDesc.Name;
		if (name == "Coin")
		{
			SetCoins(mCurrentPlayerState.Coins + 1);
			return;
		}
		if (name == "KeyCard")
		{
			SetKeyCards(mCurrentPlayerState.KeyCards + 1);
			return;
		}
		if (name == "Health")
		{
			SetHealth(mCurrentPlayerState.Health + 1);
			if (FWorld.Player != null)
			{
				FWorld.Player.SetHP(mCurrentPlayerState.Health);
			}
			return;
		}
		if (name == "Rocks")
		{
			SetRocks(mCurrentPlayerState.Rocks + 1);
			return;
		}
		if (name == "Pudding")
		{
			SetPudding(mCurrentPlayerState.Pudding + 1);
			return;
		}
		FProtoCollectible.Types result = FProtoCollectible.Types.None;
		if (FEnum.TryParse(lootDesc.Name, ref result) && lootDesc.DBID >= 0)
		{
			AddCollectible(lootDesc.DBID);
		}
	}

	protected void TryCallStatsUpdated()
	{
		mStatsUpdated?.Invoke(mCurrentPlayerState);
	}

	public void SetHealth(int v, bool updatePlayer = false)
	{
		mCurrentPlayerState.Health = (v > mCurrentPlayerState.MaxHealth) ? mCurrentPlayerState.MaxHealth : Math.Max(v, 0);
		TryCallStatsUpdated();
	}

	public void SetCoins(int v)
	{
		mCurrentPlayerState.Coins = (v > mCurrentPlayerState.MaxCoins) ? mCurrentPlayerState.MaxCoins : Math.Max(v, 0);
		TryCallStatsUpdated();
	}

	public void SetKeyCards(int v)
	{
		mCurrentPlayerState.KeyCards = (v > mCurrentPlayerState.MaxKeyCards) ? mCurrentPlayerState.MaxKeyCards : Math.Max(v, 0);
		TryCallStatsUpdated();
	}

	public void AddUnlockedDoor(FUnit unit)
	{
		string item = FApp.World.ProtoWorld.Name + "_" + unit.Name;
		if (!mCurrentPlayerState.UnlockedDoors.Contains(item))
		{
			mCurrentPlayerState.UnlockedDoors.Add(item);
		}
	}

	public int GetAmmoAmount(string ammoName)
	{
		if (ammoName == "Rock")
		{
			return mCurrentPlayerState.Rocks;
		}
		if (ammoName == "Pudding")
		{
			return mCurrentPlayerState.Pudding;
		}
		if (ammoName == "Coin")
		{
			return mCurrentPlayerState.Coins;
		}
		return -1;
	}

	public bool UseAmmoAmount(string ammoName, int amount)
	{
		if (ammoName == "Rock")
		{
			int rocks = mCurrentPlayerState.Rocks;
			if (rocks < amount)
			{
				return false;
			}
			SetRocks(rocks - amount);
		}
		else if (ammoName == "Pudding")
		{
			int pudding = mCurrentPlayerState.Pudding;
			if (pudding < amount)
			{
				return false;
			}
			SetPudding(pudding - amount);
		}
		else if (ammoName == "Coin")
		{
			int coins = mCurrentPlayerState.Coins;
			if (coins < amount)
			{
				return false;
			}
			SetCoins(coins - amount);
		}
		return true;
	}

	public void SetRocks(int v)
	{
		mCurrentPlayerState.Rocks = (v > mCurrentPlayerState.MaxRocks) ? mCurrentPlayerState.MaxRocks : Math.Max(v, 0);
		TryCallStatsUpdated();
	}

	public void SetPudding(int v)
	{
		mCurrentPlayerState.Pudding = (v > mCurrentPlayerState.MaxPudding) ? mCurrentPlayerState.MaxPudding : Math.Max(v, 0);
		TryCallStatsUpdated();
	}

	public bool HasCollectible(string collectibleName)
	{
		FProtoCollectible protoCollectible = FApp.ProtoCollectibleManager.GetProtoCollectible(collectibleName);
		if (protoCollectible == null)
		{
			return false;
		}
		return mCurrentPlayerState.Collectibles.Contains(protoCollectible.Name);
	}

	public bool HasCollectible(int dbid)
	{
		FProtoCollectible protoCollectibleByDBID = FApp.ProtoCollectibleManager.GetProtoCollectibleByDBID(dbid);
		if (protoCollectibleByDBID == null)
		{
			return false;
		}
		return mCurrentPlayerState.Collectibles.Contains(protoCollectibleByDBID.Name);
	}

	public bool AddCollectible(string collectibleName)
	{
		if (!collectibleName.Valid())
		{
			return false;
		}
		FProtoCollectible protoCollectible = FApp.ProtoCollectibleManager.GetProtoCollectible(collectibleName);
		if (protoCollectible == null)
		{
			return false;
		}
		if (mCurrentPlayerState.Collectibles.Contains(collectibleName))
		{
			return false;
		}
		int count = mCurrentPlayerState.Collectibles.Count;
		int count2 = FApp.ProtoCollectibleManager.ProtoCollectibles.Count;
		mCurrentPlayerState.Collectibles.Add(collectibleName);
		if (protoCollectible.NumberHeartPieces > 0)
		{
			int maxHealth = mCurrentPlayerState.MaxHealth;
			CalculateMaximumHealth();
			int num = mCurrentPlayerState.MaxHealth - maxHealth;
			if (num != 0 && maxHealth <= mCurrentPlayerState.MaxHealth)
			{
				SetHealth(num + mCurrentPlayerState.Health);
				if (FApp.World != null)
				{
					FApp.World.FillPlayerHealth();
					FGameUtil.DelayExecute(0.1f, delegate
					{
						FApp.World.TryShowDialogue("", "4HeartPiecesCollect");
					});
				}
			}
		}
		if (protoCollectible.Permanent)
		{
			FApp.PlayerData.AddPermanentUnlock(protoCollectible.Name);
		}
		FAnalytics.TrySendAnalytics();
		TryCallStatsUpdated();
		if (count == count2)
		{
			return true;
		}
		if (mCurrentPlayerState.Collectibles.Count == FApp.ProtoCollectibleManager.ProtoCollectibles.Count && FApp.World != null)
		{
			FApp.World.HundredPercent = true;
		}
		return true;
	}

	public bool AddCollectible(int dbid)
	{
		FProtoCollectible protoCollectibleByDBID = FApp.ProtoCollectibleManager.GetProtoCollectibleByDBID(dbid);
		if (protoCollectibleByDBID != null)
		{
			return AddCollectible(protoCollectibleByDBID.Name);
		}
		return false;
	}

	public bool CanStartQuest(int questDBID)
	{
		if (mCurrentPlayerState.CompletedQuests.Contains(questDBID))
		{
			return false;
		}
		return !mCurrentPlayerState.CurrentQuests.Contains(questDBID);
	}

	public bool StartQuest(int questDBID)
	{
		bool flag = CanStartQuest(questDBID);
		if (flag)
		{
			mCurrentPlayerState.CurrentQuests.Add(questDBID);
			TryCallStatsUpdated();
		}
		return flag;
	}

	public bool HasCompletedQuest(int questDBID)
	{
		return mCurrentPlayerState.CompletedQuests.Contains(questDBID);
	}

	public bool HasQuest(int questDBID)
	{
		return mCurrentPlayerState.CurrentQuests.Contains(questDBID);
	}

	public bool CanCompleteQuest(int questDBID, bool mustHaveQuest = true)
	{
		if (!HasCompletedQuest(questDBID) && (!mustHaveQuest || HasQuest(questDBID)))
		{
			FProtoQuest protoQuestByDBID = FApp.ProtoQuestManager.GetProtoQuestByDBID(questDBID);
			if (protoQuestByDBID != null)
			{
				return protoQuestByDBID.CheckRequirements();
			}
		}
		return false;
	}

	public bool CompleteQuest(int questDBID, out string loottable)
	{
		loottable = "";
		if (!CanCompleteQuest(questDBID))
		{
			return false;
		}
		FProtoQuest protoQuestByDBID = FApp.ProtoQuestManager.GetProtoQuestByDBID(questDBID);
		if (protoQuestByDBID == null)
		{
			return false;
		}
		loottable = protoQuestByDBID.LootTable;
		mCurrentPlayerState.CurrentQuests.Remove(questDBID);
		mCurrentPlayerState.CompletedQuests.Add(questDBID);
		return true;
	}

	public int GetNumberHeartPieceCollectibles()
	{
		int num = 0;
		for (int i = 0; i < mCurrentPlayerState.Collectibles.Count; i++)
		{
			FProtoCollectible protoCollectible = FApp.ProtoCollectibleManager.GetProtoCollectible(mCurrentPlayerState.Collectibles[i]);
			if (protoCollectible != null)
			{
				num += protoCollectible.NumberHeartPieces;
			}
		}
		return num;
	}

	public void CalculateMaximumHealth()
	{
		int num = cHeartPiecesPerHeart;
		int num2 = cStartingHearts;
		int numberHeartPieceCollectibles = GetNumberHeartPieceCollectibles();
		int num3 = 0;
		if (cHeartPiecesPerHeart != 0)
		{
			num3 = (numberHeartPieceCollectibles + num * num2) / cHeartPiecesPerHeart;
		}
		MaxHealth = cHitpointsPerHeart * num3;
	}

	public void ApplySpecialEffects()
	{
		if (FApp.ProtoCollectibleManager == null)
		{
			return;
		}
		MaxRocks = 25;
		if (HasCollectible(425))
		{
			MaxRocks = mCurrentPlayerState.MaxRocks + 25;
		}
		if (HasCollectible(421))
		{
			MaxRocks = mCurrentPlayerState.MaxRocks + 25;
		}
		if (HasCollectible(420))
		{
			MaxRocks = mCurrentPlayerState.MaxRocks + 25;
		}
		mCurrentPlayerState.MaxPudding = 50;
		if (HasCollectible(428))
		{
			mCurrentPlayerState.MaxPudding += 50;
		}
		if (FApp.PlayerData == null || FApp.PlayerData.PermanentUnlocks == null)
		{
			return;
		}
		for (int i = 0; i < FApp.PlayerData.PermanentUnlocks.Length; i++)
		{
			AddCollectible(FApp.PlayerData.PermanentUnlocks[i]);
		}
	}

	public float GetPercentComplete()
	{
		return mCurrentPlayerState.GetPercentComplete();
	}

	public string GetPercentCompleteString(bool singleLine)
	{
		return mCurrentPlayerState.GetPercentCompleteString(singleLine);
	}

	public void AddSaveSlot(int saveType = 0)
	{
		FPlayerState fPlayerState = new FPlayerState();
		fPlayerState.SaveType = saveType;
		mSavedPlayerStates.Add(fPlayerState);
	}

	public void DuplicateSave(int slot)
	{
		if (slot >= 0 && slot <= mSavedPlayerStates.Count)
		{
			FPlayerState item = new FPlayerState(mSavedPlayerStates[slot]);
			mSavedPlayerStates.Add(item);
		}
	}

	public void DeleteSaveSlot(int slot)
	{
		if (slot >= 0 && slot <= mSavedPlayerStates.Count)
		{
			mSavedPlayerStates.RemoveAt(slot);
			if (mSavedPlayerStates.Count < 1)
			{
				SetCurrentPlayerState(null);
				return;
			}
			WipePlayerState();
			SwitchSaveSlot(0);
		}
	}

	public bool SwitchSaveSlot(int slot)
	{
		if (slot >= 0)
		{
			int count = mSavedPlayerStates.Count;
			if (slot <= count)
			{
				if (count == slot)
				{
					AddSaveSlot();
				}
				mCurrentSaveSlot = slot;
				Reset();
				return true;
			}
		}
		return false;
	}

	public bool SwitchToFirstSaveSlotOfType(int saveType)
	{
		int i;
		for (i = 0; i < mSavedPlayerStates.Count; i++)
		{
			if (mSavedPlayerStates[i].SaveType == saveType)
			{
				SwitchSaveSlot(i);
				break;
			}
		}
		return i < mSavedPlayerStates.Count;
	}

	public FRequirement<FPlayerStats> AllocateRequirementForXmlElement(XmlElement element)
	{
		switch (element.GetAttribute("Type"))
		{
		case "InMapPack":
			return new FRequirementMapPack();
		case "ResourceCount":
			return new FRequirementResourceCount();
		case "HasQuest":
			return new FRequirementHasQuest();
		case "PlayerAttackTargetIsTag":
			return new FRequirementPlayerAttackTargetIsTag();
		case "CanStartQuest":
			return new FRequirementCanStartQuest();
		case "CanCompleteQuest":
			return new FRequirementCanCompleteQuest();
		case "Collectible":
			return new FRequirementCollectible();
		case "PlayerAttackTargetStunned":
			return new FRequirementPlayerAttackTargetStunned();
		case "CurrentCharacter":
			return new FRequirementCurrentCharacter();
		default:
			return null;
		}
	}

	public string ParseSpecialXMLTag(XmlNode node)
	{
		if (node.Name == "CurrentCharacter")
		{
			return mCurrentPlayerState.CurrentCharacter;
		}
		if (node.Name == "CharacterStat" && node is XmlElement element)
		{
			string attrib = element.GetAttrib("CharacterName", "");
			string attrib2 = element.GetAttrib("StatName", "");
			if (attrib.Valid() && attrib2.Valid())
			{
				FProtoUnit protoUnit = FApp.ProtoUnitManager.GetProtoUnit(attrib, true);
				if (protoUnit != null && attrib2 == "WeaponDamage")
				{
					return protoUnit.GetProtoWeapon().Damage.ToString();
				}
			}
		}
		return null;
	}

	public string Serialize()
	{
		FLog.Log("SAVEDATALOGGING: Serializing playerstats.", true);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(FPlayerState[]));
		using (StringWriter stringWriter = new StringWriter())
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
			{
				xmlSerializer.Serialize(xmlWriter, mSavedPlayerStates.ToArray());
				FLog.Log("SAVEDATALOGGING: Serialize complete, returning playerstats string.", true);
				return stringWriter.ToString();
			}
		}
	}

	public void Deserialize(string xmlStr)
	{
		if (!xmlStr.Valid())
		{
			return;
		}
		HadOutOfDateSaves = false;
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(FPlayerState[]));
		FLog.Log("SAVEDATALOGGING: Deserializing player stats.", true);
		using (StringReader input = new StringReader(xmlStr))
		{
			using (XmlReader xmlReader = XmlReader.Create(input))
			{
				if (!xmlSerializer.CanDeserialize(xmlReader))
				{
					FLog.Log("SAVEDATALOGGING: Serializer cannot deserialize playerstats!! Raw text: " + xmlStr, true);
					return;
				}
				object obj = xmlSerializer.Deserialize(xmlReader);
				if (obj == null)
				{
					FLog.Log("SAVEDATALOGGING: Playerstate array null after deserialization!! Raw text: " + xmlStr, true);
					return;
				}
				FPlayerState[] array = (FPlayerState[])obj;
				List<FPlayerState> list = new List<FPlayerState>();
				for (int i = 0; i < array.Length; i++)
				{
					FPlayerState fPlayerState = array[i];
					if (fPlayerState.SaveVersion < cCurrentSaveVersion)
					{
						HadOutOfDateSaves = true;
						FLog.Log("SAVEDATALOGGING: Out of date saves detected. Exiting.", true);
					}
					else
					{
						list.Add(fPlayerState);
					}
				}
				mSavedPlayerStates = list.ToList();
				mCurrentSaveSlot = 0;
			}
		}
	}
}
