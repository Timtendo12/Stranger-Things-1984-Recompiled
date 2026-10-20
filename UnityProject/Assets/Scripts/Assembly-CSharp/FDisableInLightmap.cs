using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FDisableInLightmap @ 0x16f96d8..0x16f9774, Ghidra vaddr).
public class FDisableInLightmap : MonoBehaviour
{
	public float Intensity;

	private void Awake()
	{
		if (Application.isPlaying)
		{
			Object.Destroy(this);
		}
	}
}
