using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FRotateForever @ 0x1664488..0x16644ec, Ghidra vaddr).
public class FRotateForever : MonoBehaviour
{
	public Vector3 RotationSpeed;

	private void Update()
	{
		base.transform.Rotate(RotationSpeed.x * Time.deltaTime, RotationSpeed.y * Time.deltaTime, RotationSpeed.z * Time.deltaTime);
	}
}
