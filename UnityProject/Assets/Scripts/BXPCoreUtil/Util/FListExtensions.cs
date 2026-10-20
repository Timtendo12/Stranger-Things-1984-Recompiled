using System;
using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FListExtensions @ 0x19ec2d4..0x19ee1fc, Ghidra vaddr).
public static class FListExtensions
{
	public static int Partition<T>(this IList<T> list, Comparison<T> comparison, int left, int right)
	{
		T pivot = list[left];
		while (left < right)
		{
			while (left < right && comparison(list[right], pivot) > 0)
			{
				right--;
			}
			if (left != right)
			{
				list[left] = list[right];
				left++;
			}
			while (left < right && comparison(list[left], pivot) < 0)
			{
				left++;
			}
			if (left != right)
			{
				list[right] = list[left];
				right--;
			}
		}
		list[left] = pivot;
		return left;
	}

	public static T QuickSelect<T>(this IList<T> list, int k, Comparison<T> comparison, int left, int right)
	{
		while (true)
		{
			int pivotIndex = list.Partition(comparison, left, right);
			if (pivotIndex < k)
			{
				left = pivotIndex + 1;
				continue;
			}
			if (pivotIndex <= k)
			{
				return list[pivotIndex];
			}
			right = pivotIndex - 1;
		}
	}

	public static T QuickSelect<T>(this IList<T> list, int k, Comparison<T> comparison)
	{
		return list.QuickSelect(k, comparison, 0, list.Count - 1);
	}

	public static T QuickSelect<T>(this IList<T> list, int k, IComparer<T> comparer)
	{
		return list.QuickSelect(k, (T x, T y) => comparer.Compare(x, y));
	}

	public static T QuickSelect<T>(this IList<T> list, int k) where T : IComparable<T>
	{
		return list.QuickSelect(k, (T x, T y) => x.CompareTo(y));
	}

	public static void Shuffle<T>(this IList<T> list, int startIndex = 0, int endIndex = -1)
	{
		if (endIndex < 0 || endIndex > list.Count - 1)
		{
			endIndex = list.Count - 1;
		}
		for (int i = startIndex; i < endIndex; i++)
		{
			int j = (int)FRandom.Generic.RangeUInt((uint)i, (uint)endIndex);
			T a = list[i];
			T b = list[j];
			list[i] = b;
			list[j] = a;
		}
	}

	public static void Shuffle<T>(this IList<T> list, FRandom random)
	{
		if (list == null || random == null)
		{
			return;
		}
		int count = list.Count;
		for (int i = 0; i <= count - 2; i++)
		{
			int j = random.RangeIntExclusive(i, count);
			T a = list[i];
			T b = list[j];
			list[i] = b;
			list[j] = a;
		}
	}

	public static bool Contains<T>(this IList<T> list, T item, int startIndex)
	{
		if (item == null)
		{
			for (int i = startIndex; i < list.Count; i++)
			{
				if (list[i] == null)
				{
					return true;
				}
			}
			return false;
		}
		EqualityComparer<T> comparer = EqualityComparer<T>.Default;
		for (int i = startIndex; i < list.Count; i++)
		{
			if (comparer.Equals(list[i], item))
			{
				return true;
			}
		}
		return false;
	}
}
