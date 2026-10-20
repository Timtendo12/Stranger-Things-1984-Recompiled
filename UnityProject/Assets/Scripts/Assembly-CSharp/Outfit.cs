using System;

// Rewritten from libil2cpp.so 1.0.605 (Outfit @ 0x1772dbc..0x1772dec, Ghidra vaddr).
[Serializable]
public class Outfit
{
	public string Name;

	public bool IsCurrent;

	public bool IsUnlocked;

	public Outfit(string name)
	{
		Name = name;
	}
}
