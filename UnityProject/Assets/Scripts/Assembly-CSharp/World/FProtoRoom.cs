using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FProtoRoom @ 0x01826bec..0x01827050, Ghidra vaddr).
public class FProtoRoom
{
	protected List<FProtoUnitStartData> mProtoUnitStartDatas = new List<FProtoUnitStartData>();

	protected List<FProtoDoorStartData> mProtoDoorStartDatas = new List<FProtoDoorStartData>();

	protected List<FEntryData> mEntryDatas = new List<FEntryData>();

	protected List<FPointData> mPointDatas = new List<FPointData>();

	public List<int[][]> CosmeticTileLayers = new List<int[][]>();

	public string Name { get; protected set; }

	public Rect Bounds { get; protected set; }

	public bool EnemyEncounterRoom { get; set; }

	public string EnemyEncounterDoorOverride { get; set; }

	public int[][] TileMap { get; set; }

	public string AreaName { get; set; }

	public List<FProtoUnitStartData> ProtoUnitStartDatas => mProtoUnitStartDatas;

	public List<FProtoDoorStartData> ProtoDoorStartDatas => mProtoDoorStartDatas;

	public List<FEntryData> EntryDatas => mEntryDatas;

	public List<FPointData> PointDatas => mPointDatas;

	public FProtoRoom(string name, Rect bounds)
	{
		Name = name;
		EnemyEncounterDoorOverride = "";
		Bounds = bounds;
		EnemyEncounterRoom = false;
	}

	public FProtoUnitStartData GetUnitStartData(string name)
	{
		for (int i = 0; i < mProtoUnitStartDatas.Count; i++)
		{
			if (mProtoUnitStartDatas[i].Name == name)
			{
				return mProtoUnitStartDatas[i];
			}
		}
		return null;
	}

	public FPointData GetPointData(string name)
	{
		for (int i = 0; i < mPointDatas.Count; i++)
		{
			if (mPointDatas[i].Name == name)
			{
				return mPointDatas[i];
			}
		}
		return null;
	}

	public bool IsBossRoom()
	{
		return GetBossStartData() != null;
	}

	public FProtoUnitStartData GetBossStartData()
	{
		if (mProtoUnitStartDatas == null)
		{
			return null;
		}
		foreach (FProtoUnitStartData startData in mProtoUnitStartDatas)
		{
			if (startData.GetProtoUnit.IsTag("Boss"))
			{
				return startData;
			}
		}
		return null;
	}
}
