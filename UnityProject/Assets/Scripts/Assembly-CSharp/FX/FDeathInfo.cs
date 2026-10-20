using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FDeathInfo @ 0x16f5ba0..0x16f5c0c, Ghidra vaddr).
public class FDeathInfo : MonoBehaviour
{
	public FGibInfo GibInfo;

	private void OnDrawGizmos()
	{
		if (GibInfo != null && GibInfo.HasGibs)
		{
			GibInfo.DrawSpawnGizmo(base.transform.position);
		}
	}
}
