using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPhysicsLayers @ 0x17b7934..0x17b7fc8, Ghidra vaddr).
public class FPhysicsLayers
{
	protected const int cLayerMax = 32;

	protected static int[] mLayerIndices;

	protected static int[] mLayerCollisionMasks;

	public static int DefaultIndex => mLayerIndices[0];

	public static int TransparentFXIndex => mLayerIndices[1];

	public static int IgnoreRaycastIndex => mLayerIndices[2];

	public static int WaterIndex => mLayerIndices[3];

	public static int UIIndex => mLayerIndices[4];

	public static int GroundIndex => mLayerIndices[5];

	public static int HeroIndex => mLayerIndices[6];

	public static int UnitIndex => mLayerIndices[7];

	public static int ObstaclesIndex => mLayerIndices[10];

	public static int GetLayerIndex(Layers layer)
	{
		return mLayerIndices[(int)layer];
	}

	public static int GetLayerMask(Layers layer)
	{
		return 1 << GetLayerIndex(layer);
	}

	public static int GetLayerCollisionMask(Layers layer)
	{
		return GetLayerCollisionMaskByIndex(GetLayerIndex(layer));
	}

	public static int GetLayerCollisionMaskByIndex(int index)
	{
		return mLayerCollisionMasks[index];
	}

	public static void Init()
	{
		string[] names = Enum.GetNames(typeof(Layers));
		mLayerIndices = new int[names.Length];
		for (int i = 0; i < names.Length; i++)
		{
			mLayerIndices[i] = LayerMask.NameToLayer(names[i]);
		}
		mLayerCollisionMasks = new int[32];
		for (int j = 0; j < 32; j++)
		{
			int mask = 0;
			for (int k = 0; k < 32; k++)
			{
				if (!Physics.GetIgnoreLayerCollision(j, k))
				{
					mask |= 1 << k;
				}
			}
			mLayerCollisionMasks[j] = mask;
		}
	}
}
