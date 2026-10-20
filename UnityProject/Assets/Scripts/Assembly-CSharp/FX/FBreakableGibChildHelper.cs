using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FBreakableGibChildHelper @ 0x16f1970..0x16f1994, Ghidra vaddr).
public class FBreakableGibChildHelper : MonoBehaviour
{
	protected FBreakableGib mParent;

	public void SetParent(FBreakableGib parent)
	{
		mParent = parent;
	}

	public void OnCollisionEnter(Collision collision)
	{
		mParent.OnCollisionEnter(collision);
	}
}
