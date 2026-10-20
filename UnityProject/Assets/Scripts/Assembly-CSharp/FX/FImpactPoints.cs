using System.Collections.Generic;
using UnityEngine;

public class FImpactPoints : MonoBehaviour
{
	public List<FImpactPoint> Points;

	public void Clear()
	{
	}

	public void ParseDirectionString(string str, out bool front, out bool back, out bool left, out bool right)
	{
		front = default;
		back = default;
		left = default;
		right = default;
	}

	public void Add(Transform transform, string validDirections)
	{
	}

	public void Add(Transform transform, bool front, bool back, bool left, bool right)
	{
	}

	public FImpactPoint GetRandomPoint(FRandom random)
	{
		return null;
	}

	public FImpactPoint GetPointForDirection(Cardinal incomingDirection, FRandom random)
	{
		return null;
	}
}
