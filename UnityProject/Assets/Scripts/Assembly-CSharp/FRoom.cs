using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FRoom @ 0x181a5a4..0x1831af4 + closures @ 0x1716fd4..0x1717a00, Ghidra vaddr).
public class FRoom : MonoBehaviour
{
	public enum RoomStates
	{
		Unvisited = 0,
		Visited = 1,
		Current = 2
	}

	public enum SpawnUnitResult
	{
		None = 0,
		Success = 1,
		Failure = 2,
		Ignored = 3
	}

	protected FTile[][] mMap;

	protected List<FUnit> mUnits = new List<FUnit>();

	protected List<FUnitToRemove> mUnitsToRemove = new List<FUnitToRemove>();

	protected List<FProtoUnitStartData> mUnitsToSpawn = new List<FProtoUnitStartData>();

	protected List<FPoint2> mEntrances = new List<FPoint2>();

	protected List<FProtoUnitStartData> mActiveStartDatas = new List<FProtoUnitStartData>();

	protected int mEntranceUsed = -1;

	protected Coroutine mFadeRoomRoutine;

	protected bool mRoomEncounterFinished;

	protected bool mUnitRemovedThisFrame;

	protected Transform mUnitHolder;

	protected List<int> mUnitsWaitingForReset = new List<int>();

	protected int mTimesSpawned;

	protected bool mRoomShouldFade = true;

	protected SpriteRenderer mRoomSprite;

	public string Name => ProtoRoom.Name;

	public int ID { get; protected set; }

	public FWorld World { get; protected set; }

	public FProtoRoom ProtoRoom { get; protected set; }

	public bool RoomExplored { get; set; }

	public int X { get; protected set; }

	public int Y { get; protected set; }

	public int Height { get; protected set; }

	public int Width { get; protected set; }

	public RoomStates State { get; protected set; }

	public List<FUnit> Units => mUnits;

	public List<FProtoUnitStartData> ActiveStartDatas => mActiveStartDatas;

	public int EntranceUsed
	{
		get
		{
			int num = mEntranceUsed;
			if (num < 0)
			{
				num = 0;
			}
			else if (mEntrances.Count <= num)
			{
				num = 0;
			}
			return num;
		}
	}

	public SpriteRenderer RoomSprite => mRoomSprite;

	public void Create(FProtoRoom protoRoom, FWorld world, int id)
	{
		ID = id;
		ProtoRoom = protoRoom;
		World = world;
		X = (int)protoRoom.Bounds.x;
		Y = (int)protoRoom.Bounds.y;
		Width = (int)protoRoom.Bounds.width;
		Height = (int)protoRoom.Bounds.height;
		SavedRoomDesc roomDesc = FApp.PlayerStats.GetRoomDesc(FApp.PlayerStats.GetRoomIdentifier(this));
		if (roomDesc != null)
		{
			mEntranceUsed = roomDesc.EntranceUsed;
		}
		base.transform.parent = world.transform;
		base.transform.localPosition = new Vector3(X, Y, 0f);
		mUnitHolder = new GameObject("Unit Holder").transform;
		mUnitHolder.parent = base.transform;
		mUnitHolder.localPosition = Vector3.zero;
		mUnitHolder.localEulerAngles = Vector3.zero;
		mTimesSpawned = 0;
		mRoomShouldFade = World.ProtoWorld.MapPack != "WorldMap";
		LoadTiles();
		LoadUnits();
	}

	public void Release()
	{
		for (int i = 0; i < mUnits.Count; i++)
		{
			if (mUnits[i] != null)
			{
				mUnits[i].Release();
			}
		}
	}

	public void ResetRoom()
	{
		ClearUnits();
		LoadUnits();
		ResetPlayerPos();
		mRoomEncounterFinished = false;
		mTimesSpawned = 0;
		if (State != RoomStates.Current)
		{
			SetRoomState(RoomStates.Unvisited, force: true);
		}
	}

	public bool ResetUnit(FUnit unit)
	{
		if (mUnits.Contains(unit))
		{
			FProtoUnitStartData startData = unit.StartData;
			if (startData != null)
			{
				RemoveUnitImmediate(unit);
				AddUnit(startData);
				return true;
			}
		}
		return false;
	}

	public void ResetPlayerPos()
	{
		int index = ((mEntranceUsed < 0) ? 0 : mEntranceUsed);
		FPoint2 fPoint = mEntrances[index];
		FWorld.Player.AI.StopActions();
		Transform transform = FWorld.Player.GO.transform;
		Vector3 position = FWorld.Player.GO.transform.position;
		transform.position = new Vector3(fPoint.x, fPoint.y, position.z);
		FWorld.Player.SyncTilePosition();
	}

	public void SetRoomState(RoomStates state, bool force = false, bool immediate = false)
	{
		RoomStates state2 = State;
		if (state2 == state && !force)
		{
			return;
		}
		State = state;
		if (mFadeRoomRoutine != null)
		{
			FGameUtil.StopCoroutine(mFadeRoomRoutine);
			mFadeRoomRoutine = null;
		}
		switch (State)
		{
		case RoomStates.Unvisited:
			if (mTimesSpawned < 1)
			{
				DoUpdate(0f);
			}
			if (mRoomShouldFade)
			{
				base.gameObject.SetActive(false);
			}
			ShowUnits(false);
			return;
		case RoomStates.Visited:
		{
			if (mTimesSpawned < 1)
			{
				DoUpdate(0f);
			}
			base.gameObject.SetActive(true);
			float delay = 0f;
			if (state2 == RoomStates.Current && !immediate)
			{
				delay = 0.75f;
			}
			mFadeRoomRoutine = FGameUtil.StartCoroutine(FadeRoom(delay));
			break;
		}
		case RoomStates.Current:
			base.gameObject.SetActive(true);
			SetCoverAlpha(0f);
			ShowUnits(true);
			break;
		default:
			return;
		}
		CheckResetRoomEvents();
	}

	protected IEnumerator FadeRoom(float delay)
	{
		float finalAlpha = 0.5f;
		float delayTime = World.Gametime + delay;
		while (delayTime > World.Gametime)
		{
			yield return 0f;
			yield return 0f;
			SyncUnitUINodes();
			if (mRoomShouldFade)
			{
				float num = 1f - (delayTime - World.Gametime) / delay;
				num = Mathf.Clamp01(num);
				SetCoverAlpha(finalAlpha * num + 0f);
			}
		}
		if (mRoomShouldFade)
		{
			SetCoverAlpha(finalAlpha);
		}
		ShowUnits(false);
	}

	protected void ClearUnits()
	{
		for (int num = mUnits.Count - 1; num >= 0; num--)
		{
			RemoveUnitImmediate(mUnits[num]);
		}
	}

	protected void LoadUnits()
	{
		foreach (FProtoUnitStartData protoUnitStartData in ProtoRoom.ProtoUnitStartDatas)
		{
			bool flag = FApp.PlayerStats.IsUnitAlive(protoUnitStartData, this);
			if (!flag)
			{
				FProtoUnit getProtoUnit = protoUnitStartData.GetProtoUnit;
				flag = getProtoUnit != null && getProtoUnit.ResetOnDeath;
			}
			if (flag && GetUnitFromStartData(protoUnitStartData) == null)
			{
				AddUnit(protoUnitStartData);
			}
		}
		FProtoUnitStartData[] spawnedUnitsForRoom = FApp.PlayerStats.GetSpawnedUnitsForRoom(this);
		for (int i = 0; i < spawnedUnitsForRoom.Length; i++)
		{
			FProtoUnitStartData pusd = spawnedUnitsForRoom[i];
			if (FApp.PlayerStats.IsUnitAlive(pusd, this) && GetUnitFromStartData(pusd) == null)
			{
				AddUnit(pusd);
			}
		}
		foreach (FProtoDoorStartData protoDoorStartData in ProtoRoom.ProtoDoorStartDatas)
		{
			if (FApp.PlayerStats.IsUnitAlive(protoDoorStartData, this) && GetUnitFromStartData(protoDoorStartData) == null)
			{
				AddDoor(protoDoorStartData);
			}
		}
	}

	protected FUnit GetUnitFromStartData(FProtoUnitStartData pusd)
	{
		foreach (FUnit mUnit in mUnits)
		{
			if (mUnit.StartData == pusd)
			{
				return mUnit;
			}
		}
		return null;
	}

	public void AddUnit(FProtoUnitStartData pusd)
	{
		mUnitsToSpawn.Add(pusd);
		if (!mActiveStartDatas.Contains(pusd))
		{
			mActiveStartDatas.Add(pusd);
		}
	}

	public void AddDoor(FProtoDoorStartData pdsd)
	{
		mUnitsToSpawn.Add(pdsd);
		if (!mActiveStartDatas.Contains(pdsd))
		{
			mActiveStartDatas.Add(pdsd);
		}
	}

	protected void LoadTiles()
	{
		mEntrances.Clear();
		foreach (FEntryData entryData in ProtoRoom.EntryDatas)
		{
			mEntrances.Add(entryData.Position);
		}
		List<FProtoUnitStartData> list = new List<FProtoUnitStartData>(ProtoRoom.ProtoUnitStartDatas);
		list.AddRange(ProtoRoom.ProtoDoorStartDatas.Select((FProtoDoorStartData d) => (FProtoUnitStartData)d));
		foreach (FProtoUnitStartData item in list)
		{
			if (item.ProtoUnit == null)
			{
				// shipped code leaves the loop here (null-name guard in the original)
				break;
			}
			if (item.GetProtoUnit.IsTag("Entry_Front"))
			{
				mEntrances.Add(item.Position + FUnit.GetFPDirFromFacing(item.Facing));
			}
		}
		Dictionary<int, FProtoTile> tileset = FTilesetManager.GetTileset(World.ProtoWorld.Tileset);
		if (tileset == null)
		{
			return;
		}
		mMap = new FTile[Height][];
		for (int i = 0; i < Height; i++)
		{
			mMap[i] = new FTile[Width];
			for (int j = 0; j < Width; j++)
			{
				int num = ProtoRoom.TileMap[i][j];
				int num2 = X;
				int num3 = Y;
				if (num < 0)
				{
					continue;
				}
				if (!tileset.ContainsKey(num))
				{
					FLog.UnityError("Invalid ProtoTile Key=" + num.ToString() + " at [" + j.ToString() + ", " + i.ToString() + "]");
					continue;
				}
				FProtoTile fProtoTile = tileset[num];
				if (fProtoTile == null)
				{
					FLog.UnityError("Invalid ProtoTile [" + j.ToString() + ", " + i.ToString() + "]");
					continue;
				}
				mMap[i][j] = new FTile(fProtoTile, num2 + j, i + num3);
				mMap[i][j].Create(base.gameObject);
				foreach (int[][] cosmeticTileLayer in ProtoRoom.CosmeticTileLayers)
				{
					int num4 = cosmeticTileLayer[i][j];
					if (num4 >= 0)
					{
						if (!tileset.ContainsKey(num4))
						{
							FLog.UnityError("Invalid ProtoTile Key for cosmetic tile =" + num4.ToString() + " at [" + j.ToString() + ", " + i.ToString() + "]");
							continue;
						}
						FProtoTile fProtoTile2 = tileset[num4];
						if (fProtoTile2 == null)
						{
							FLog.UnityError("Invalid ProtoTile for cosmetic tile [" + j.ToString() + ", " + i.ToString() + "]");
						}
						else
						{
							mMap[i][j].AddCosmeticTile(fProtoTile2);
						}
					}
				}
				if ((j == 0 || j == Width - 1 || i == 0 || i == Height - 1) && mMap[i][j].TerrainPassable)
				{
					mEntrances.Add(new FPoint2(num2 + j, i + num3));
				}
			}
		}
	}

	public void DoUpdate(float deltaTime)
	{
		for (int num = mUnitsToRemove.Count - 1; num >= 0; num--)
		{
			float gametime = World.Gametime;
			if (mUnitsToRemove[num].Delay <= gametime - mUnitsToRemove[num].Time)
			{
				RemoveUnitImmediate(mUnitsToRemove[num].Unit);
				mUnitRemovedThisFrame = true;
				mUnitsToRemove.RemoveAt(num);
			}
		}
		SpawnUnits();
		CheckRoomEvents();
		for (int i = 0; i < mUnits.Count; i++)
		{
			mUnits[i].Update(deltaTime);
		}
	}

	protected void SpawnUnits()
	{
		for (int i = 0; i < mUnitsToSpawn.Count; i++)
		{
			int newID = -1;
			if (SpawnUnit(mUnitsToSpawn[i], out newID) != SpawnUnitResult.Failure)
			{
				mUnitsToSpawn.RemoveAt(i);
				i--;
			}
		}
		mTimesSpawned++;
	}

	public bool SpawnSingleUnit()
	{
		for (int i = 0; i < mUnitsToSpawn.Count; i++)
		{
			int newID = -1;
			if (SpawnUnit(mUnitsToSpawn[i], out newID) != SpawnUnitResult.Failure)
			{
				mUnitsToSpawn.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	public SpawnUnitResult SpawnUnit(FProtoUnitStartData pusd, out int newID, bool force = false)
	{
		newID = -1;
		if (!force)
		{
			if (!string.IsNullOrEmpty(pusd.ProtoCollectible) && pusd.GetProtoUnit.AIType == "Collectible" && FApp.PlayerStats.HasCollectible(pusd.ProtoCollectible))
			{
				return SpawnUnitResult.Ignored;
			}
			if (pusd.ProtoUnit == null)
			{
				return SpawnUnitResult.Failure;
			}
			if (!pusd.GetProtoUnit.CheckSpawnRequirements())
			{
				return SpawnUnitResult.Failure;
			}
			if (!pusd.GetProtoUnit.IsTag("PlayerCharacter"))
			{
				FTile tile;
				if (pusd.GetProtoUnit.Obstructs && (tile = GetTile(pusd.Position)) != null && tile.TerrainPassable && !tile.GetPassable())
				{
					return SpawnUnitResult.Failure;
				}
			}
		}
		int nextUnitID = World.GetNextUnitID();
		FUnit fUnit;
		if (pusd.GetProtoUnit.EmitUnit.Valid())
		{
			fUnit = new FUnitEmitter(this, pusd, nextUnitID);
		}
		else
		{
			fUnit = new FUnit(this, pusd, nextUnitID);
		}
		mUnits.Add(fUnit);
		fUnit.Create(base.gameObject, "");
		fUnit.SetTilePosition(fUnit.TilePosition, force: true);
		fUnit.GO.transform.parent = mUnitHolder;
		FProtoDoorStartData fProtoDoorStartData = pusd as FProtoDoorStartData;
		if (fProtoDoorStartData != null)
		{
			if (fProtoDoorStartData.CloseOnSpawn)
			{
				fUnit.PlayAnim("Close", loop: false, null, fireTags: true);
			}
			if (FApp.PlayerStats != null)
			{
				List<string> unlockedDoors = FApp.PlayerStats.UnlockedDoors;
				string item = World.ProtoWorld.Name + "_" + pusd.Name;
				if (unlockedDoors.Contains(item))
				{
					fUnit.SetDoorLocked(false);
				}
			}
		}
		newID = nextUnitID;
		return SpawnUnitResult.Success;
	}

	public FTile GetTile(FPoint2 position)
	{
		FPoint2 fPoint = position - new FPoint2(X, Y);
		if (fPoint.y >= 0 && fPoint.x >= 0)
		{
			if (Height <= fPoint.y)
			{
				return null;
			}
			if (fPoint.x < Width)
			{
				return mMap[fPoint.y][fPoint.x];
			}
		}
		return null;
	}

	public FPoint2 GetClosestEntranceToPoint(FPoint2 point)
	{
		if (mEntrances != null && mEntrances.Count != 0)
		{
			return mEntrances.OrderBy((FPoint2 e) => e.DistanceTo(point)).First();
		}
		return FPoint2.invalid;
	}

	public FTile GetClosestOpenTile(FPoint2 position, bool fullPassableCheck = false, FUnit ignoreUnit = null, bool onlyHorz = false, bool onlyVert = false, List<FPoint2> excludePoints = null)
	{
		IEnumerable<FTile> source = GetFlatTileList();
		if (excludePoints != null)
		{
			source = source.Where((FTile t) => !excludePoints.Contains(t.Position));
		}
		if (onlyHorz)
		{
			source = source.Where((FTile t) => position.y == t.Position.y);
		}
		if (onlyVert)
		{
			source = source.Where((FTile t) => position.x == t.Position.x);
		}
		Func<FTile, bool> predicate = ((!fullPassableCheck) ? ((Func<FTile, bool>)((FTile t) => t.TerrainPassable)) : ((Func<FTile, bool>)((FTile t) => t.GetPassable(ignoreUnit, new List<int>(), null))));
		return source.Where(predicate).OrderBy((FTile t) => t.Position.DistanceTo(position)).FirstOrDefault();
	}

	public IEnumerable<FTile> GetFlatTileList()
	{
		return mMap.SelectMany((FTile[] row) => row).Where((FTile t) => t != null);
	}

	public FTile GetClosestFullyOpenTile(FPoint2 position, FUnit ignoreUnit = null, List<FPoint2> excludePoints = null)
	{
		IEnumerable<FTile> source = GetFlatTileList().Where((FTile t) => t.GetPassable(ignoreUnit, new List<int>(), "", true));
		if (excludePoints != null)
		{
			source = source.Where((FTile t) => !excludePoints.Contains(t.Position));
		}
		return source.OrderBy((FTile t) => t.Position.DistanceTo(position)).FirstOrDefault();
	}

	public FTile GetTileFromGO(GameObject go)
	{
		FPoint2 fPoint = FPoint2.Parse(go.name);
		if (fPoint == FPoint2.invalid)
		{
			return null;
		}
		return GetTile(fPoint);
	}

	public FUnit GetUnitFromGO(GameObject go)
	{
		int result = -1;
		FUnit result2 = null;
		if (int.TryParse(go.name, out result))
		{
			result2 = GetUnitByID(result);
		}
		return result2;
	}

	public FUnit GetUnitByID(int id)
	{
		for (int i = 0; i < mUnits.Count; i++)
		{
			if (mUnits[i].ID == id)
			{
				return mUnits[i];
			}
		}
		return null;
	}

	public FUnit GetUnitByName(string name)
	{
		EnsureUnitIsReady(name);
		for (int i = 0; i < mUnits.Count; i++)
		{
			if (mUnits[i].Name == name)
			{
				return mUnits[i];
			}
		}
		return null;
	}

	public FUnit GetUnitByUniqueID(string uniqueID)
	{
		// the shipped code calls EnsureUnitIsReady (by name) here, not the ByUniqueID variant
		EnsureUnitIsReady(uniqueID);
		for (int i = 0; i < mUnits.Count; i++)
		{
			if (mUnits[i].UniqueID == uniqueID)
			{
				return mUnits[i];
			}
		}
		return null;
	}

	public FUnit[] GetUnitsByName(string name)
	{
		EnsureUnitIsReady(name);
		List<FUnit> list = new List<FUnit>();
		for (int i = 0; i < mUnits.Count; i++)
		{
			if (mUnits[i].Name == name)
			{
				list.Add(mUnits[i]);
			}
		}
		return list.ToArray();
	}

	protected void EnsureUnitIsReady(string name)
	{
		if (mTimesSpawned < 1 && HasUnit(name))
		{
			DoUpdate(0f);
		}
	}

	protected void EnsureUnitIsReadyByUniqueID(string uniqueID)
	{
		if (mTimesSpawned < 1 && HasUnitByUniqueID(uniqueID))
		{
			DoUpdate(0f);
		}
	}

	public bool HasSpawnedUnits()
	{
		return mTimesSpawned > 0;
	}

	public bool HasUnit(string name)
	{
		foreach (FProtoUnitStartData protoUnitStartData in ProtoRoom.ProtoUnitStartDatas)
		{
			if (protoUnitStartData.Name == name)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasUnitByUniqueID(string uniqueID)
	{
		foreach (FProtoUnitStartData protoUnitStartData in ProtoRoom.ProtoUnitStartDatas)
		{
			if (FApp.PlayerStats.GetUnitIdentifier(protoUnitStartData, this) == uniqueID)
			{
				return true;
			}
		}
		return false;
	}

	protected void RemoveUnitImmediate(FUnit unit)
	{
		unit.RemoveFromTile();
		unit.Release();
		mUnits.Remove(unit);
		for (int i = 0; i < mActiveStartDatas.Count; i++)
		{
			if (mActiveStartDatas[i] == unit.StartData)
			{
				FProtoUnitStartData startData = unit.StartData;
				if (startData != null && mActiveStartDatas.Contains(startData))
				{
					mActiveStartDatas.Remove(startData);
				}
				break;
			}
		}
	}

	public void RemoveUnit(FUnit unit, float delay)
	{
		if (!mUnits.Contains(unit))
		{
			return;
		}
		for (int i = 0; i < mUnitsToRemove.Count; i++)
		{
			if (mUnitsToRemove[i].Unit == unit)
			{
				return;
			}
		}
		FUnitToRemove fUnitToRemove = new FUnitToRemove();
		fUnitToRemove.Unit = unit;
		fUnitToRemove.Delay = delay;
		fUnitToRemove.Time = World.Gametime;
		mUnitsToRemove.Add(fUnitToRemove);
	}

	public void PlayerEnteredRoom(bool resetEnemies = true)
	{
		SetRoomState(RoomStates.Current);
		if (!resetEnemies)
		{
			return;
		}
		foreach (FUnit item in new List<FUnit>(mUnits))
		{
			if (item.IsTag("EnemyCharacter"))
			{
				item.FullReset();
			}
		}
	}

	public void PlayerLeftRoom()
	{
		RemovePendingUnits();
		SetRoomState(RoomStates.Visited, force: false, FWorld.Player.HP < 1);
		if (IsBossRoom() && !mRoomEncounterFinished)
		{
			World.EndBossEncounter(false);
		}
	}

	public void RemovePendingUnits()
	{
		foreach (FUnitToRemove item in new List<FUnitToRemove>(mUnitsToRemove))
		{
			if (item.Unit == null)
			{
				break;
			}
			RemoveUnitImmediate(item.Unit);
		}
		mUnitsToRemove.Clear();
	}

	protected void ShowUnits(bool show)
	{
		foreach (FUnit mUnit in mUnits)
		{
			if (!show)
			{
				mUnit.ShowUINode(false);
				if (!mUnit.AliveAndWell)
				{
					RemoveUnit(mUnit, 0f);
				}
			}
			else
			{
				mUnit.ShowUINode(true);
			}
		}
		mUnitHolder.gameObject.SetActive(show);
	}

	protected void SetCoverAlpha(float alpha)
	{
		if (!(mRoomSprite == null))
		{
			Color color = mRoomSprite.color;
			color.a = 1f - alpha;
			mRoomSprite.color = color;
			base.gameObject.SetActive(0f < 1f - alpha);
		}
	}

	protected void CheckResetRoomEvents()
	{
		if (!mRoomEncounterFinished && ProtoRoom.EnemyEncounterRoom && !IsBossRoom())
		{
			OpenAllDoors(animate: false);
		}
	}

	protected void CheckRoomEvents()
	{
		if (mUnitRemovedThisFrame && !mRoomEncounterFinished && ProtoRoom.EnemyEncounterRoom && !mUnits.Any((FUnit u) => u.ProtoUnit.IsTag("EnemyCharacter")))
		{
			CompleteEnemyEncounter();
		}
	}

	public void CompleteEnemyEncounter()
	{
		mRoomEncounterFinished = true;
		World.PlaySecretGet();
		OpenAllDoors();
	}

	public void PlayerEntranceComplete()
	{
		CheckStartRoomEvents();
		SaveEntranceUsed();
	}

	protected void CheckStartRoomEvents()
	{
		if (ProtoRoom.EnemyEncounterRoom && !mRoomEncounterFinished)
		{
			if (mUnits.Any((FUnit u) => u.ProtoUnit.IsTag("EnemyCharacter")))
			{
				SpawnAndLockAllDoors();
			}
			else
			{
				mRoomEncounterFinished = true;
			}
		}
	}

	protected void SaveEntranceUsed()
	{
		if (mEntranceUsed >= 0 && IsBossRoom())
		{
			return;
		}
		int num = 0;
		float num2 = 2000f;
		int num3 = 0;
		foreach (FPoint2 mEntrance in mEntrances)
		{
			float num4 = mEntrance.DistanceTo(FWorld.Player.TilePosition);
			int num5 = num;
			if (num2 <= num4)
			{
				num5 = num3;
				num4 = num2;
			}
			num2 = num4;
			num++;
			num3 = num5;
		}
		mEntranceUsed = num3;
	}

	public void OpenAllDoors(bool animate = true)
	{
		for (int num = mUnits.Count - 1; num >= 0; num--)
		{
			FUnit fUnit = mUnits[num];
			if (fUnit.Locked)
			{
				if (!animate)
				{
					RemoveUnitImmediate(fUnit);
				}
				else
				{
					fUnit.OpenDoor(force: true);
				}
			}
		}
	}

	public void SpawnAndLockAllDoors()
	{
		int num = 0;
		foreach (FPoint2 entrance in mEntrances)
		{
			if (X != entrance.x && X + Width - 1 != entrance.x && Y != entrance.y && Y + Height - 1 != entrance.y)
			{
				continue;
			}
			FPoint2 entranceCopy = entrance;
			if (mActiveStartDatas.Any((FProtoUnitStartData sd) => sd.Position == entranceCopy && sd.GetProtoUnit.IsTag("Door")))
			{
				continue;
			}
			FTile tile = GetTile(entrance);
			if (tile == null || tile.Units.Any((FUnit u) => u.ProtoUnit.IsTag("Door")))
			{
				continue;
			}
			FProtoDoorStartData fProtoDoorStartData = new FProtoDoorStartData();
			fProtoDoorStartData.UnlockPersists = false;
			fProtoDoorStartData.Locked = true;
			fProtoDoorStartData.CloseOnSpawn = true;
			fProtoDoorStartData.Name = "AutoDoor_" + num.ToString();
			fProtoDoorStartData.Position = entrance;
			fProtoDoorStartData.ProtoUnit = (ProtoRoom.EnemyEncounterDoorOverride.Valid() ? ProtoRoom.EnemyEncounterDoorOverride : "ObjectSchoolDoorEnemyRoom");
			if (fProtoDoorStartData.ProtoUnit == null)
			{
				FApp.ProtoUnitManager.GetProtoUnit("ObjectSchoolDoorEnemyRoom", true);
			}
			if (entrance.x == X)
			{
				fProtoDoorStartData.Facing = FAnimEntry.AnimFacing.RIGHT;
			}
			else if (entrance.x == X + Width - 1)
			{
				fProtoDoorStartData.Facing = FAnimEntry.AnimFacing.LEFT;
			}
			else
			{
				fProtoDoorStartData.Facing = ((entrance.y == Y) ? FAnimEntry.AnimFacing.UP : FAnimEntry.AnimFacing.DOWN);
			}
			AddDoor(fProtoDoorStartData);
			num++;
		}
	}

	public bool UnlockDoor(FUnit door)
	{
		if (ProtoRoom.EnemyEncounterRoom && !mRoomEncounterFinished)
		{
			return false;
		}
		if (!door.ProtoUnit.KeycardUnlockable)
		{
			return false;
		}
		if (FApp.PlayerStats.KeyCards < 1)
		{
			return false;
		}
		FApp.PlayerStats.SetKeyCards(FApp.PlayerStats.KeyCards - 1);
		door.SetDoorLocked(false);
		if (((FProtoDoorStartData)door.StartData).UnlockPersists)
		{
			FApp.PlayerStats.AddUnlockedDoor(door);
		}
		return true;
	}

	public void SetAllTileColor(Color c)
	{
		for (int i = 0; i < mMap.Length; i++)
		{
			for (int j = 0; j < mMap[i].Length; j++)
			{
				FTile fTile = mMap[i][j];
				if (fTile != null)
				{
					fTile.SetTileColor(c, includeUnits: false, animate: false);
				}
			}
		}
	}

	public void SyncTileVisibility()
	{
		if (World == null || FWorld.Player == null)
		{
			return;
		}
		Color color = ((!World.ProtoWorld.TintColor.Valid()) ? Color.white : FUtil.ColorFromHex(World.ProtoWorld.TintColor));
		for (int i = 0; i < mMap.Length; i++)
		{
			for (int j = 0; j < mMap[i].Length; j++)
			{
				float distance = new FPoint2(j + X, i + Y).DistanceTo(FWorld.Player.TilePosition);
				FTile fTile = mMap[i][j];
				float t = Mathf.Clamp01(distance * 0.125f);
				Color color2 = new Color(color.r + (color.r * 0.1f - color.r) * t, color.g + (color.g * 0.1f - color.g) * t, color.b + (color.b * 0.1f - color.b) * t, color.a);
				fTile.SetTileColor(color2, includeUnits: true, animate: true);
			}
		}
	}

	public FPoint2 ClampPoint(FPoint2 point)
	{
		int num = X + Width - 1;
		int num2 = Y + Height - 1;
		int x = ((X <= point.x) ? Math.Min(point.x, num) : X);
		int y = ((Y <= point.y) ? Math.Min(point.y, num2) : Y);
		return new FPoint2(x, y);
	}

	public FPoint2 GetRandomPoint()
	{
		int newX = FRandom.Generic.RangeInt(X, Width + X - 1);
		int newY = FRandom.Generic.RangeInt(Y, Height + Y - 1);
		return new FPoint2(newX, newY);
	}

	public bool ContainsFPoint(FPoint2 point)
	{
		return X <= point.x && point.x < Width + X && Y <= point.y && point.y < Height + Y;
	}

	public void UnitWaitingForReset(FUnit unit)
	{
		if (!mUnitsWaitingForReset.Contains(unit.ID))
		{
			mUnitsWaitingForReset.Add(unit.ID);
		}
	}

	public void UnitHasBeenReset(FUnit unit)
	{
		if (mUnitsWaitingForReset.Contains(unit.ID))
		{
			mUnitsWaitingForReset.Remove(unit.ID);
		}
	}

	public bool IsBossRoom()
	{
		FProtoUnitStartData bossStartData = ProtoRoom.GetBossStartData();
		if (bossStartData == null)
		{
			return false;
		}
		return FApp.PlayerStats.IsUnitAlive(bossStartData, this);
	}

	public FPoint2 GetEntrance(int index)
	{
		if (index >= 0 && index < mEntrances.Count)
		{
			return mEntrances[index];
		}
		return FPoint2.invalid;
	}

	public FAnimEntry.AnimFacing GetSideUnitIsOn(FUnit unit)
	{
		FPoint2 tilePosition = unit.TilePosition;
		if (X == tilePosition.x)
		{
			return FAnimEntry.AnimFacing.LEFT;
		}
		if (Y == tilePosition.y)
		{
			return FAnimEntry.AnimFacing.DOWN;
		}
		if (X + Width - 1 == tilePosition.x)
		{
			return FAnimEntry.AnimFacing.RIGHT;
		}
		return (Y + Height - 1 == tilePosition.y) ? FAnimEntry.AnimFacing.UP : FAnimEntry.AnimFacing.NONE;
	}

	public void SetTexture(Texture2D texture)
	{
		Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(1f / ((float)Width * 2f), 1f / ((float)Height * 2f)), 32f);
		mRoomSprite = base.gameObject.GetOrCreateComponent<SpriteRenderer>();
		mRoomSprite.sprite = sprite;
		if (State == RoomStates.Unvisited)
		{
			SetCoverAlpha(1f);
		}
		else if (State == RoomStates.Visited)
		{
			SetCoverAlpha(0.5f);
		}
	}

	public bool HasTexture()
	{
		return mRoomSprite != null;
	}

	public void SyncUnitUINodes()
	{
		for (int i = 0; i < mUnits.Count; i++)
		{
			mUnits[i].SyncUINode(force: true);
		}
	}
}
