using System;
using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (ListExtensions @ 0x1a18674..0x1a187b0, Ghidra vaddr).
public static class ListExtensions
{
	public static T GetLastObject<T>(this List<T> list)
	{
		return list[list.Count - 1];
	}

	public static void InsertionSort<T>(this List<T> list, Comparison<T> comparison)
	{
		int count = list.Count;
		for (int i = 1; i < count; i++)
		{
			T item = list[i];
			int j = i;
			while (j > 0 && comparison(list[j - 1], item) > 0)
			{
				list[j] = list[j - 1];
				j--;
			}
			list[j] = item;
		}
	}
}
