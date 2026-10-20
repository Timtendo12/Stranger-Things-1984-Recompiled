using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FBinWriterExtensions @ 0x16459e4..0x1645b38, Ghidra vaddr).
public static class FBinWriterExtensions
{
	public static void Write(this FBinWriter w, Vector2 v)
	{
		w.Write(v.x);
		w.Write(v.y);
	}

	public static void Write(this FBinWriter w, Vector3 v)
	{
		w.Write(v.x);
		w.Write(v.y);
		w.Write(v.z);
	}

	public static void Write(this FBinWriter w, Color c)
	{
		w.Write(c.r);
		w.Write(c.g);
		w.Write(c.b);
		w.Write(c.a);
	}

	public static void Write(this FBinWriter w, Quaternion v)
	{
		w.Write(v.x);
		w.Write(v.y);
		w.Write(v.z);
		w.Write(v.w);
	}
}
