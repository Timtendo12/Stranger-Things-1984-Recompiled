using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FutileRender @ 0x16a9848..0x16a9920, Ghidra vaddr).
// Lives on the helper camera; Futile renders everything after that camera finishes.
public class FutileRender : MonoBehaviour
{
	public void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void OnPostRender()
	{
		Futile.Instance.Render();
	}
}
