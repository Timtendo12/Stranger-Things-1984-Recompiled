using System;
using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FMemoryPoolSimple`1 @ 0x1fabf38..0x1fac41c, Ghidra vaddr).
public class FMemoryPoolSimple<T> where T : class, new()
{
	private const int cMaxPoolItems = 1000;

	private Stack<object> mData = new Stack<object>();

	private static int NoFreeCount;

	private static int FreeCount;

	private static int CreateCount;

	private static int GetCount;

	private static int MaxCount;

	private static int FoundCount;

	public void Free(T val)
	{
		if (val == null)
		{
			FAssert.Fail("Tried to release null pointer into FMemoryPoolSimple");
			return;
		}
		if (mData.Count < cMaxPoolItems)
		{
			mData.Push(val);
			FreeCount++;
			MaxCount = Math.Max(MaxCount, mData.Count);
		}
		else
		{
			NoFreeCount++;
		}
	}

	public T Get()
	{
		if (!FConfigCore.PoolFLetterQuad)
		{
			return new T();
		}
		GetCount++;
		if (mData.Count > 0)
		{
			T item = mData.Pop() as T;
			if (item != null)
			{
				FoundCount++;
				return item;
			}
		}
		CreateCount++;
		return new T();
	}
}
