using System;
using UnityEngine;

public class FRaycastResult : IComparable<FRaycastResult>
{
	public float Distance;

	public Collider Collider;

	public FRaycastResult(RaycastHit hit)
	{
	}

	public int CompareTo(FRaycastResult other)
	{
		return 0;
	}
}
