using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FTextSpan @ 0x16a03bc..0x16a0420, Ghidra vaddr).
public class FTextSpan
{
	public Color Color = Color.white;

	public virtual int Length => 0;

	// 0x16a03c4: returns string.Empty (slot 0x3208640)
	public virtual string WrapString => "";

	public virtual FAtlas Atlas => null;

	public virtual bool IsWhiteSpaceAtIndex(int index)
	{
		return false;
	}
}
