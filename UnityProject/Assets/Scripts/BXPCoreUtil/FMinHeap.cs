using System;
using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FMinHeap<T> @ 0x1fac41c..0x1facbd4, Ghidra vaddr).
public class FMinHeap<T> where T : IComparable
{
	protected List<T> mData;

	public List<T> Data => mData;

	public int Count => mData.Count;

	public FMinHeap()
	{
		mData = new List<T>();
	}

	public FMinHeap(int capacity)
	{
		mData = new List<T>(capacity);
	}

	public void Insert(T o)
	{
		mData.Add(o);
		int num = mData.Count - 1;
		while (num > 0)
		{
			int num2 = ((num + 1) >> 1) - 1;
			if (mData[num2].CompareTo(mData[num]) <= 0)
			{
				return;
			}
			T value = mData[num];
			mData[num] = mData[num2];
			mData[num2] = value;
			num = num2;
		}
	}

	public T ExtractMin()
	{
		if (mData.Count < 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		T result = mData[0];
		mData[0] = mData[mData.Count - 1];
		mData.RemoveAt(mData.Count - 1);
		MinHeapify(0);
		return result;
	}

	public T Peek()
	{
		return mData[0];
	}

	protected void MinHeapify(int i)
	{
		while (true)
		{
			int num = i * 2 + 1;
			int num2 = i * 2 + 2;
			int num3 = ((num < mData.Count && mData[num].CompareTo(mData[i]) < 0) ? num : i);
			if (num2 < mData.Count && mData[num2].CompareTo(mData[num3]) < 0)
			{
				num3 = num2;
			}
			if (num3 == i)
			{
				return;
			}
			T value = mData[i];
			mData[i] = mData[num3];
			mData[num3] = value;
			i = num3;
		}
	}

	public void Clear(List<T> recycleList = null)
	{
		if (recycleList != null)
		{
			for (int i = 0; i < mData.Count; i++)
			{
				recycleList.Add(mData[i]);
			}
		}
		mData.Clear();
	}
}
