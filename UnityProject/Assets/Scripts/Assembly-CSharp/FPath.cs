using UnityEngine;
using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FPath @ 0x1814b2c..0x18150e0, Ghidra vaddr).
public class FPath
{
	protected List<FPoint2> mWaypoints = new List<FPoint2>();

	public List<FPoint2> Waypoints => mWaypoints;

	public int NumberWaypoints => mWaypoints.Count;

	public FPoint2 FirstWaypoint
	{
		get
		{
			if (mWaypoints.Count > 0)
			{
				return mWaypoints[0];
			}
			return FPoint2.invalid;
		}
	}

	public FPoint2 LastWaypoint
	{
		get
		{
			if (mWaypoints.Count > 0)
			{
				return mWaypoints[mWaypoints.Count - 1];
			}
			return FPoint2.invalid;
		}
	}

	public void AddWaypoints(List<FPoint2> waypoints)
	{
		for (int i = 0; i < waypoints.Count; i++)
		{
			mWaypoints.Add(waypoints[i]);
		}
	}

	public void AddWaypoint(FPoint2 waypoint)
	{
		mWaypoints.Add(waypoint);
	}

	public void InsertWaypoint(int index, FPoint2 waypoint)
	{
		mWaypoints.Insert(index, waypoint);
	}

	public void RemoveWaypoint(int index)
	{
		if (index < 0)
		{
			index = mWaypoints.Count - index;
			index = Mathf.Max(index, 0);
		}
		mWaypoints.RemoveAt(index);
	}

	public void Clear()
	{
		mWaypoints.Clear();
	}

	public FPoint2 GetWaypoint(int index)
	{
		if (index >= 0 && index < mWaypoints.Count)
		{
			return mWaypoints[index];
		}
		return FPoint2.invalid;
	}
}
