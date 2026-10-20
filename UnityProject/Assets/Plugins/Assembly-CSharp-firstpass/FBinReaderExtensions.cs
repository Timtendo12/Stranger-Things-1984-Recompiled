using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FBinReaderExtensions @ 0x16457ec..0x1645960, Ghidra vaddr).
public static class FBinReaderExtensions
{
	public static Vector2 ReadVector2(this FBinReader r)
	{
		float x = r.ReadFloat();
		float y = r.ReadFloat();
		return new Vector2(x, y);
	}

	public static Vector3 ReadVector3(this FBinReader r)
	{
		float x = r.ReadFloat();
		float y = r.ReadFloat();
		float z = r.ReadFloat();
		return new Vector3(x, y, z);
	}

	public static Color ReadColor(this FBinReader r)
	{
		float r2 = r.ReadFloat();
		float g = r.ReadFloat();
		float b = r.ReadFloat();
		float a = r.ReadFloat();
		return new Color(r2, g, b, a);
	}

	public static Quaternion ReadQuaternion(this FBinReader r)
	{
		float x = r.ReadFloat();
		float y = r.ReadFloat();
		float z = r.ReadFloat();
		float w = r.ReadFloat();
		return new Quaternion(x, y, z, w);
	}
}
