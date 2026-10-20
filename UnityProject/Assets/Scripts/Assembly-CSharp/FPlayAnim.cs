using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPlayAnim @ 0x17b7fc8..0x17b80f8, Ghidra vaddr).
public class FPlayAnim : MonoBehaviour
{
	public string AnimName;

	public void Start()
	{
		FGameUtil.ExecuteAtEndOfFrame(delegate
		{
			base.gameObject.PlayAnim(AnimName, 1f, true, -1f);
		});
	}
}
