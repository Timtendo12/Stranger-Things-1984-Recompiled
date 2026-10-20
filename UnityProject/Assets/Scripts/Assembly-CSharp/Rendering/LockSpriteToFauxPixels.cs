using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (LockSpriteToFauxPixels @ 0x177058c..0x1770810, Ghidra vaddr).
public class LockSpriteToFauxPixels : MonoBehaviour
{
	public const int PixelsPerUnit = 16;

	private void Update()
	{
		Transform parent = base.transform.parent;
		if (parent != null)
		{
			Vector3 position = parent.position;
			float x = (float)(int)Math.Round(position.x * 16f) / 16f;
			float y = (float)(int)Math.Round(position.y * 16f) / 16f;
			base.transform.localPosition = new Vector3(x - position.x, y - position.y, 0f - position.z);
		}
	}
}
