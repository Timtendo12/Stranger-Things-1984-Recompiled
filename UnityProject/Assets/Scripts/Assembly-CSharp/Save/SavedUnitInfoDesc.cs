using System;
using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (SavedUnitInfoDesc @ 0x18170ec..0x18176a8, Ghidra vaddr).
[Serializable]
public class SavedUnitInfoDesc
{
	public List<string> Keys = new List<string>();

	public List<string> Values = new List<string>();

	public string UnitIdentifier { get; set; }

	public SavedUnitInfoDesc()
	{
	}

	public SavedUnitInfoDesc(SavedUnitInfoDesc other)
	{
		UnitIdentifier = other.UnitIdentifier;
		Keys = new List<string>(other.Keys);
		Values = new List<string>(other.Values);
	}

	public void Set(string keyLookup, string value)
	{
		int num = 0;
		foreach (string key in Keys)
		{
			if (key == keyLookup)
			{
				Values[num] = value;
				return;
			}
			num++;
		}
		Keys.Add(keyLookup);
		Values.Add(value);
	}

	public string GetValue(string keyLookup)
	{
		int num = 0;
		foreach (string key in Keys)
		{
			if (key == keyLookup)
			{
				return Values[num];
			}
			num++;
		}
		return null;
	}
}
