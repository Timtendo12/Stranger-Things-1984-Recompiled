using System.Collections.Generic;
using System.IO;

// Rewritten from libil2cpp.so 1.0.605 (FKerningPairs @ 0x168fcd8..0x1690074, Ghidra vaddr).
public class FKerningPairs
{
	private Dictionary<int, int> Pairs = new Dictionary<int, int>();

	public int GetPair(int key)
	{
		int amount = 0;
		if (!Pairs.TryGetValue(key, out amount))
		{
			amount = 0;
		}
		return amount;
	}

	public void AddPair(int first, int second, int amount)
	{
		if ((first | second) >= 0)
		{
			Pairs[second | (first << 16)] = amount;
		}
	}

	public void Write(Stream stream)
	{
		BinaryWriter writer = new BinaryWriter(stream);
		writer.Write(Pairs.Count);
		foreach (KeyValuePair<int, int> pair in Pairs)
		{
			writer.Write(pair.Key);
			writer.Write(pair.Value);
		}
	}

	public void Read(Stream stream)
	{
		Pairs = new Dictionary<int, int>();
		BinaryReader reader = new BinaryReader(stream);
		for (int count = reader.ReadInt32(); count > 0; count--)
		{
			int key = reader.ReadInt32();
			int value = reader.ReadInt32();
			Pairs[key] = value;
		}
	}
}
