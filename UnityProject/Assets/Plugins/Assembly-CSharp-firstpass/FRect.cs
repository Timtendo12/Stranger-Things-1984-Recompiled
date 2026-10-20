using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FRect @ 0x166d9f8..0x166db08, Ghidra vaddr).
public class FRect
{
	public Vector3 Min;

	public Vector3 Max;

	public Vector3 MinXMinZ => Min;

	public Vector3 MinXMaxZ => new Vector3(Min.x, Min.y, Max.z);

	public Vector3 MaxXMaxZ => Max;

	public Vector3 MaxXMinZ => new Vector3(Max.x, Max.y, Min.z);

	public FRect()
	{
		Min = Vector3.zero;
		Max = Vector3.zero;
	}

	public FRect(Vector3 position, Vector3 size)
	{
		Min.y = 0f;
		Max.y = 0f;
		Min.x = position.x - size.x * 0.5f;
		Min.z = position.z - size.z * 0.5f;
		Max.x = position.x + size.x * 0.5f;
		Max.z = position.z + size.z * 0.5f;
	}
}
