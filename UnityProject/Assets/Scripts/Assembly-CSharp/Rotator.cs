using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (Rotator @ 0x1773850..0x1773860, Ghidra vaddr).
public class Rotator : MonoBehaviour
{
	public float rotateSpeed = 5f;

	private void Update()
	{
		base.transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
	}
}
