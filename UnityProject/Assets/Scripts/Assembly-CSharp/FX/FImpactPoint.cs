using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FImpactPoint @ 0x1788f9c..0x1789048, Ghidra vaddr).
[Serializable]
public class FImpactPoint
{
	public Transform Transform;

	public bool Front;

	public bool Back;

	public bool Left;

	public bool Right;

	public FImpactPoint(Transform transform, bool front, bool back, bool left, bool right)
	{
		Transform = transform;
		Front = front;
		Back = back;
		Left = left;
		Right = right;
	}

	public bool IsValidForDirection(Cardinal incomingDirection)
	{
		if ((incomingDirection != Cardinal.Front || !Front) && (incomingDirection != Cardinal.Back || !Back) && (incomingDirection != Cardinal.Left || !Left))
		{
			if (incomingDirection == Cardinal.Right)
			{
				return Right;
			}
			return false;
		}
		return true;
	}
}
