using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FLazyPrefabPoolHelper @ 0x178c038..0x178c0e8, Ghidra vaddr).
public class FLazyPrefabPoolHelper : MonoBehaviour
{
	public void OnDestroy()
	{
		FApp.PrefabPool.ObjectDestroyed(base.gameObject);
	}
}
