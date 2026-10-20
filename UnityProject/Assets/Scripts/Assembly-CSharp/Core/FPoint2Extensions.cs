using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPoint2Extensions @ 0x17c274c..0x17c275c, Ghidra vaddr).
public static class FPoint2Extensions
{
	public static Vector2 ToVector2(this FPoint2 fp2)
	{
		return new Vector2(fp2.x, fp2.y);
	}

	public static FPoint2 ToFPoint2(this Vector2 v2)
	{
		return new FPoint2(Mathf.RoundToInt(v2.x), Mathf.RoundToInt(v2.y));
	}
}
