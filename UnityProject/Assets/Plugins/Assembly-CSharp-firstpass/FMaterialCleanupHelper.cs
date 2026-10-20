using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FMaterialCleanupHelper @ 0x165fb0c..0x165fbd4, Ghidra vaddr).
public static class FMaterialCleanupHelper
{
	public static void TrackMaterialForCleanup(this GameObject gameObj, Material mat)
	{
		FMaterialCleanup fMaterialCleanup = gameObj.GetComponent<FMaterialCleanup>();
		if (fMaterialCleanup == null)
		{
			fMaterialCleanup = gameObj.AddComponent<FMaterialCleanup>();
		}
		fMaterialCleanup.AddMaterial(mat);
	}
}
