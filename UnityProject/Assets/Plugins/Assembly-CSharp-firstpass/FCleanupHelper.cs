using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FCleanupHelper @ 0x16474cc..0x1647760, Ghidra vaddr).
public class FCleanupHelper : MonoBehaviour
{
	protected HashSet<Object> mCleanupObjects = new HashSet<Object>();

	public void AddCleanupObject(Object obj)
	{
		mCleanupObjects.Add(obj);
	}

	public void RemoveCleanupObject(Object obj)
	{
		mCleanupObjects.Remove(obj);
	}

	public void OnDestroy()
	{
		foreach (Object mCleanupObject in mCleanupObjects)
		{
			Object.Destroy(mCleanupObject);
		}
		mCleanupObjects.Clear();
	}
}
