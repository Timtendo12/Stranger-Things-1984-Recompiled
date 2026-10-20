using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FMaterialCleanup @ 0x165f8d8..0x165fb0c, Ghidra vaddr).
public class FMaterialCleanup : MonoBehaviour
{
	protected HashSet<Material> mMaterials = new HashSet<Material>();

	public void AddMaterial(Material mat)
	{
		mMaterials.Add(mat);
	}

	public void OnDestroy()
	{
		foreach (Material mMaterial in mMaterials)
		{
			Object.Destroy(mMaterial);
		}
	}
}
