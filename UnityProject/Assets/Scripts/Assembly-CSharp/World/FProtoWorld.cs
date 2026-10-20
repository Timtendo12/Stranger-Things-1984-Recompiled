using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using TiledSharp;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FProtoWorld @ 0x0181a604..0x019efd84, Ghidra vaddr).
public class FProtoWorld
{
	public const int cTileWidth = 32;

	public const int cTileHeight = 32;

	protected bool mInitialized;

	public Dictionary<string, string> LoadData = new Dictionary<string, string>();

	protected int[][] mTileMap;

	public List<int[][]> CosmeticTileLayers = new List<int[][]>();

	protected List<FProtoUnitStartData> mProtoUnitStartDatas = new List<FProtoUnitStartData>();

	protected List<FProtoDoorStartData> mProtoDoorStartDatas = new List<FProtoDoorStartData>();

	protected List<FEntryData> mEntryDatas = new List<FEntryData>();

	protected List<FPointData> mPointDatas = new List<FPointData>();

	protected List<FProtoRoom> mProtoRooms = new List<FProtoRoom>();

	protected XmlElement mRootXML;

	public bool Initialized => mInitialized;

	public string Name { get; protected set; }

	public int ID { get; protected set; }

	public FPoint2 Start { get; protected set; }

	public int Height { get; protected set; }

	public int Width { get; protected set; }

	public string ThemeMusic { get; protected set; }

	public bool UpsideDown { get; protected set; }

	public string Tileset { get; protected set; }

	public string TintColor { get; protected set; }

	public bool SaveUnitStates { get; protected set; }

	public bool NoSave { get; protected set; }

	public string SpecialMapType { get; protected set; }

	public string MapPack
	{
		get
		{
			if (LoadData.ContainsKey("MapPack"))
			{
				return LoadData["MapPack"];
			}
			return Name;
		}
	}

	public List<FProtoRoom> ProtoRooms => mProtoRooms;

	public string StartRoom { get; protected set; }

	public bool LoadXml(XmlElement root, int id)
	{
		ID = id;
		Name = root.GetAttrib("Name", "");
		UpsideDown = root.GetAttrib("UpsideDown", false);
		TintColor = root.GetAttribute("TintColor");
		SaveUnitStates = root.GetAttrib("SaveUnitStates", true);
		NoSave = root.GetAttrib("NoSave", false);
		Start = FPoint2.invalid;
		mRootXML = root;
		foreach (XmlNode node in mRootXML.ChildNodes)
		{
			XmlElement element = node as XmlElement;
			if (element == null)
			{
				continue;
			}
			if (element.Name == "ThemeMusic")
			{
				string text = element.GetAttribute("Name");
				if (!string.IsNullOrEmpty(text))
				{
					ThemeMusic = text;
				}
			}
			else if (element.Name == "SpecialMapType")
			{
				string text2 = element.GetAttribute("Name");
				if (!string.IsNullOrEmpty(text2))
				{
					SpecialMapType = text2;
				}
			}
			else if (element.Name == "LoadData")
			{
				foreach (XmlAttribute attribute in element.Attributes)
				{
					if (!string.IsNullOrEmpty(attribute.Name) && !string.IsNullOrEmpty(attribute.Value))
					{
						LoadData[attribute.Name] = attribute.Value;
					}
				}
			}
		}
		return true;
	}

	public bool Init()
	{
		if (mInitialized)
		{
			return true;
		}
		foreach (XmlNode node in mRootXML.ChildNodes)
		{
			XmlElement element = node as XmlElement;
			if (element == null)
			{
				continue;
			}
			if (element.Name == "MapFile")
			{
				string text = element.GetAttribute("Name");
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				TmxMap tmxMap = JsonConvert.DeserializeObject<TmxMap>(FUtil.LoadTextAsset("Data/Maps/" + text));
				if (tmxMap == null)
				{
					return false;
				}
				Height = tmxMap.height;
				Width = tmxMap.width;
				if (tmxMap.layers == null)
				{
					continue;
				}
				foreach (TmxLayer layer in tmxMap.layers)
				{
					string name = layer.name;
					if (name == "Tiles")
					{
						LoadTileLayer(layer.data, out mTileMap);
					}
					else if (name == "Units")
					{
						LoadObjects(layer.objects);
					}
					else if (name == "Rooms")
					{
						LoadRooms(layer.objects);
					}
					else if (layer.type == "tilelayer")
					{
						LoadTileLayer(layer.data, out var cosmeticLayer);
						CosmeticTileLayers.Add(cosmeticLayer);
					}
				}
			}
			else if (element.Name == "TileSet")
			{
				string text2 = element.GetAttribute("Name");
				if (!string.IsNullOrEmpty(text2))
				{
					Tileset = text2;
					FTilesetManager.AddTileset(Tileset);
				}
			}
		}
		if (Start == FPoint2.invalid)
		{
			FLog.Log("No Start point defined for map: " + Name + "!", true);
		}
		if (mProtoUnitStartDatas.Count == 0)
		{
			FLog.Log("No Units defined for map: " + Name + ". It's gonna be pretty lonely in there.", true);
		}
		if (mProtoRooms.Count == 0)
		{
			FLog.Log("No Rooms defined for map: " + Name + ". Will default to basic camera follow.", true);
			mProtoRooms.Add(new FProtoRoom("Room_0", new Rect(0f, 0f, Width, Height)));
		}
		foreach (FProtoRoom room in mProtoRooms)
		{
			Rect bounds = room.Bounds;
			int minY = (int)bounds.y;
			int minX = (int)bounds.x;
			int maxY = (int)(bounds.y + bounds.height);
			int maxX = (int)(bounds.x + bounds.width);
			room.TileMap = ExtractRoomTiles(mTileMap, minX, minY, maxX, maxY);
			foreach (int[][] cosmeticLayer2 in CosmeticTileLayers)
			{
				room.CosmeticTileLayers.Add(ExtractRoomTiles(cosmeticLayer2, minX, minY, maxX, maxY));
			}
			List<FProtoUnitStartData> movedUnits = new List<FProtoUnitStartData>();
			foreach (FProtoUnitStartData unitData in mProtoUnitStartDatas)
			{
				if (RoomContains(bounds, unitData.Position))
				{
					room.ProtoUnitStartDatas.Add(unitData);
					unitData.Position = new FPoint2(unitData.Position.x, unitData.Position.y);
					unitData.Room = room.Name;
					movedUnits.Add(unitData);
				}
			}
			foreach (FProtoUnitStartData item in movedUnits)
			{
				mProtoUnitStartDatas.Remove(item);
			}
			List<FProtoDoorStartData> movedDoors = new List<FProtoDoorStartData>();
			foreach (FProtoDoorStartData doorData in mProtoDoorStartDatas)
			{
				if (RoomContains(bounds, doorData.Position))
				{
					room.ProtoDoorStartDatas.Add(doorData);
					doorData.Position = new FPoint2(doorData.Position.x, doorData.Position.y);
					doorData.Room = room.Name;
					movedDoors.Add(doorData);
				}
			}
			foreach (FProtoDoorStartData item2 in movedDoors)
			{
				mProtoDoorStartDatas.Remove(item2);
			}
			List<FEntryData> movedEntries = new List<FEntryData>();
			foreach (FEntryData entryData in mEntryDatas)
			{
				if (RoomContains(bounds, entryData.Position))
				{
					room.EntryDatas.Add(entryData);
					entryData.Position = new FPoint2(entryData.Position.x, entryData.Position.y);
					movedEntries.Add(entryData);
				}
			}
			foreach (FEntryData item3 in movedEntries)
			{
				mEntryDatas.Remove(item3);
			}
			List<FPointData> movedPoints = new List<FPointData>();
			foreach (FPointData pointData in mPointDatas)
			{
				if (RoomContains(bounds, pointData.Position))
				{
					room.PointDatas.Add(pointData);
					pointData.Position = new FPoint2(pointData.Position.x, pointData.Position.y);
					movedPoints.Add(pointData);
				}
			}
			foreach (FPointData item4 in movedPoints)
			{
				mPointDatas.Remove(item4);
			}
			if (RoomContains(bounds, Start))
			{
				StartRoom = room.Name;
				Start = new FPoint2(Start.x, Start.y);
			}
		}
		mProtoUnitStartDatas.Clear();
		mProtoDoorStartDatas.Clear();
		mInitialized = true;
		return true;
	}

	private static bool RoomContains(Rect bounds, FPoint2 pos)
	{
		float x = pos.x;
		float y = pos.y;
		return bounds.x <= x && x < bounds.x + bounds.width && bounds.y <= y && y < bounds.y + bounds.height;
	}

	private static int[][] ExtractRoomTiles(int[][] source, int minX, int minY, int maxX, int maxY)
	{
		int[][] result = new int[maxY - minY][];
		for (int i = 0; i < maxY - minY; i++)
		{
			result[i] = new int[maxX - minX];
			if (minX < maxX)
			{
				int[] src = source[minY + i];
				for (int j = 0; j < maxX - minX; j++)
				{
					result[i][j] = src[minX + j];
				}
			}
		}
		return result;
	}

	protected bool LoadRooms(TmxObject[] rooms)
	{
		if (rooms == null || rooms.Length == 0)
		{
			return false;
		}
		int roomIndex = 0;
		for (int i = 0; i < rooms.Length; i++)
		{
			TmxObject tmxObject = rooms[i];
			if (tmxObject.height < 64 || tmxObject.width < 64)
			{
				continue;
			}
			int tileX = tmxObject.x / 32;
			int tileTop = Height - tmxObject.y / 32;
			int tileBottom = tileTop - tmxObject.height / 32;
			int tileRight = tileX + tmxObject.width / 32;
			int minX = Mathf.Clamp(tileX, 0, Width);
			int minY = Mathf.Clamp(tileBottom, 0, Height);
			int maxX = Mathf.Clamp(tileRight, 0, Width);
			int maxY = Mathf.Clamp(tileTop, 0, Height);
			FProtoRoom fProtoRoom = new FProtoRoom("Room_" + roomIndex, new Rect(minX, minY, (float)maxX - (float)minX, (float)maxY - (float)minY));
			if (tmxObject.properties != null)
			{
				fProtoRoom.EnemyEncounterRoom = GetObjectProperty<bool>(tmxObject.properties, "EnemyEncounterRoom");
				fProtoRoom.EnemyEncounterDoorOverride = GetObjectProperty<string>(tmxObject.properties, "EnemyEncounterDoorOverride");
				fProtoRoom.AreaName = GetObjectProperty<string>(tmxObject.properties, "AreaName");
			}
			mProtoRooms.Add(fProtoRoom);
			roomIndex++;
		}
		return true;
	}

	protected bool LoadObjects(TmxObject[] objs)
	{
		if (objs == null || objs.Length == 0)
		{
			return false;
		}
		for (int i = 0; i < objs.Length; i++)
		{
			TmxObject tmxObject = objs[i];
			string type = tmxObject.type;
			if (!type.Valid())
			{
				continue;
			}
			int tileX = tmxObject.x / 32;
			int tileY = ~(tmxObject.y / 32) + Height;
			if (type == "Start")
			{
				Start = new FPoint2(tileX, tileY);
			}
			else if (type == "Entry")
			{
				if (tmxObject.name.Valid())
				{
					// The shipped code has no null check on properties here, so it uses the object overload.
					string leadsTo = GetObjectProperty<string>((object)tmxObject.properties, "LeadsTo");
					int showChapterTitle = GetObjectProperty<int>((object)tmxObject.properties, "ShowChapterTitle");
					FEntryData fEntryData = new FEntryData();
					if (showChapterTitle == 0)
					{
						showChapterTitle = -1;
					}
					fEntryData.Name = tmxObject.name;
					fEntryData.Position = new FPoint2(tileX, tileY);
					fEntryData.LeadsTo = leadsTo;
					fEntryData.ShowChapterTitle = showChapterTitle;
					mEntryDatas.Add(fEntryData);
				}
			}
			else if (type == "Point")
			{
				string name = tmxObject.name;
				if (name.Valid())
				{
					FPointData fPointData = new FPointData();
					fPointData.Name = name;
					fPointData.Position = new FPoint2(tileX, tileY);
					mPointDatas.Add(fPointData);
				}
			}
			else if (type == "Unit")
			{
				FProtoUnitStartData unitStartData = GetUnitStartData(tmxObject);
				if (unitStartData != null)
				{
					if (string.IsNullOrEmpty(unitStartData.Name))
					{
						unitStartData.Name = unitStartData.GetProtoUnit.Name + mProtoUnitStartDatas.Count;
					}
					mProtoUnitStartDatas.Add(unitStartData);
				}
			}
			else if (type == "Door")
			{
				FProtoUnitStartData unitStartData2 = GetUnitStartData(tmxObject);
				if (unitStartData2 == null)
				{
					FLog.UnityWarning("Invalid door in " + Name + "!!");
					continue;
				}
				string text = unitStartData2.GetObjectProperty<string>("Facing");
				FAnimEntry.AnimFacing facing = (!text.Valid()) ? FAnimEntry.AnimFacing.LEFT : ((FAnimEntry.AnimFacing)Enum.Parse(typeof(FAnimEntry.AnimFacing), text.ToUpperInvariant()));
				FProtoDoorStartData fProtoDoorStartData = new FProtoDoorStartData();
				fProtoDoorStartData.Locked = false;
				fProtoDoorStartData.CloseOnSpawn = false;
				fProtoDoorStartData.UnlockPersists = false;
				fProtoDoorStartData.Name = unitStartData2.Name;
				fProtoDoorStartData.Position = unitStartData2.Position;
				fProtoDoorStartData.ProtoUnit = unitStartData2.ProtoUnit;
				fProtoDoorStartData.Locked = unitStartData2.GetObjectProperty<bool>("Locked");
				fProtoDoorStartData.UnlockPersists = unitStartData2.GetObjectProperty<bool>("UnlockPersists");
				fProtoDoorStartData.Facing = facing;
				fProtoDoorStartData.PropertyKeys = unitStartData2.PropertyKeys;
				fProtoDoorStartData.PropertyValues = unitStartData2.PropertyValues;
				mProtoDoorStartDatas.Add(fProtoDoorStartData);
			}
		}
		return true;
	}

	protected FProtoUnitStartData GetUnitStartData(TmxObject unitInfo)
	{
		if (FApp.ProtoUnitManager == null)
		{
			return null;
		}
		Dictionary<string, object> properties = unitInfo.properties;
		if (properties == null)
		{
			return null;
		}
		FProtoUnitStartData fProtoUnitStartData = new FProtoUnitStartData();
		fProtoUnitStartData.PropertyKeys = properties.Keys.ToArray();
		fProtoUnitStartData.PropertyValues = properties.Values.ToArray();
		string text = fProtoUnitStartData.GetObjectProperty<string>("Proto");
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		if (FApp.ProtoUnitManager.GetProtoUnit(text, true) == null)
		{
			FLog.UnityError(Name + "::LoadObjects: '" + text + "' is not a valid ProtoUnit.");
			return null;
		}
		fProtoUnitStartData.ProtoUnit = text;
		fProtoUnitStartData.Position = new FPoint2(unitInfo.x / 32, ~(unitInfo.y / 32) + Height);
		fProtoUnitStartData.Name = unitInfo.name;
		fProtoUnitStartData.LootTableOverride = fProtoUnitStartData.GetObjectProperty<string>("LootTableOverride");
		string text2 = fProtoUnitStartData.GetObjectProperty<string>("Facing");
		if (text2.Valid())
		{
			fProtoUnitStartData.Facing = (FAnimEntry.AnimFacing)Enum.Parse(typeof(FAnimEntry.AnimFacing), text2.ToUpperInvariant());
		}
		int num = 0;
		do
		{
			string text3 = fProtoUnitStartData.GetObjectProperty<string>("PatrolPoint" + num);
			if (string.IsNullOrEmpty(text3))
			{
				break;
			}
			fProtoUnitStartData.PatrolPoints.Add(text3);
			num++;
		}
		while (num < 10);
		if (fProtoUnitStartData.PatrolPoints.Count < 1)
		{
			string text4 = fProtoUnitStartData.GetObjectProperty<string>("PatrolPoint");
			if (!string.IsNullOrEmpty(text4))
			{
				fProtoUnitStartData.PatrolPoints.Add(text4);
			}
		}
		fProtoUnitStartData.ProtoCollectible = fProtoUnitStartData.GetObjectProperty<string>("Collectible");
		string text5 = fProtoUnitStartData.GetObjectProperty<string>("PurchasePrice");
		if (string.IsNullOrEmpty(text5))
		{
			return fProtoUnitStartData;
		}
		int num2 = Convert.ToInt32(text5);
		if (num2 > 0)
		{
			fProtoUnitStartData.PurchasePrice = num2;
		}
		return fProtoUnitStartData;
	}

	public List<FProtoUnitStartData> GetStartDatasWithTag(string tag)
	{
		List<FProtoUnitStartData> list = new List<FProtoUnitStartData>();
		foreach (FProtoRoom room in mProtoRooms)
		{
			list.AddRange(room.ProtoUnitStartDatas.Where((FProtoUnitStartData pusd) => pusd.ProtoUnit != null && pusd.GetProtoUnit.IsTag(tag)));
			list.AddRange(room.ProtoDoorStartDatas.Where((FProtoDoorStartData pdsd) => pdsd.ProtoUnit != null && pdsd.GetProtoUnit.IsTag(tag)).Select((FProtoDoorStartData pdsd) => (FProtoUnitStartData)pdsd));
		}
		return list;
	}

	protected bool LoadTileLayer(int[] tiles, out int[][] layer)
	{
		layer = new int[Height][];
		for (int i = 0; i < Height; i++)
		{
			layer[i] = new int[Width];
			for (int j = 0; j < Width; j++)
			{
				layer[i][j] = tiles[j + i * Width] - 1;
			}
		}
		layer = layer.Reverse().ToArray();
		return true;
	}

	protected T GetObjectProperty<T>(object dictMaybe, string propName)
	{
		if (dictMaybe is Dictionary<string, object> dict)
		{
			return GetObjectProperty<T>(dict, propName);
		}
		return default(T);
	}

	protected T GetObjectProperty<T>(Dictionary<string, object> dict, string propName)
	{
		object value = default(T);
		if (dict.ContainsKey(propName))
		{
			value = dict[propName];
		}
		if (!(value is T))
		{
			value = Convert.ChangeType(value, typeof(T));
		}
		return (T)value;
	}
}
