using System;
using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FTuple @ 0x1f..., Ghidra vaddr). Standard value-equality tuple; the exact hash mixing is not significant.
public static class FTuple
{
	public static FTuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
	{
		return new FTuple<T1, T2>(item1, item2);
	}

	public static FTuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
	{
		return new FTuple<T1, T2, T3>(item1, item2, item3);
	}

	public static FTuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
	{
		return new FTuple<T1, T2, T3, T4>(item1, item2, item3, item4);
	}

	public static FTuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
	{
		return new FTuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
	}
}

public class FTuple<T1, T2> : IFormattable
{
	private static readonly IEqualityComparer<T1> Item1Comparer = EqualityComparer<T1>.Default;

	private static readonly IEqualityComparer<T2> Item2Comparer = EqualityComparer<T2>.Default;

	public T1 Item1 { get; set; }

	public T2 Item2 { get; set; }

	public FTuple(T1 item1, T2 item2)
	{
		Item1 = item1;
		Item2 = item2;
	}

	public override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 31 + ((Item1 == null) ? 0 : Item1Comparer.GetHashCode(Item1));
		hash = hash * 31 + ((Item2 == null) ? 0 : Item2Comparer.GetHashCode(Item2));
		return hash;
	}

	public override bool Equals(object obj)
	{
		FTuple<T1, T2> other = obj as FTuple<T1, T2>;
		if (other == null)
		{
			return false;
		}
		return Item1Comparer.Equals(Item1, other.Item1) && Item2Comparer.Equals(Item2, other.Item2);
	}

	public override string ToString()
	{
		return ToString(null, null);
	}

	public string ToString(string format, IFormatProvider formatProvider)
	{
		return string.Format(formatProvider, "({0}, {1})", Item1, Item2);
	}
}

public class FTuple<T1, T2, T3>
{
	private static readonly IEqualityComparer<T1> Item1Comparer = EqualityComparer<T1>.Default;

	private static readonly IEqualityComparer<T2> Item2Comparer = EqualityComparer<T2>.Default;

	private static readonly IEqualityComparer<T3> Item3Comparer = EqualityComparer<T3>.Default;

	public T1 Item1 { get; set; }

	public T2 Item2 { get; set; }

	public T3 Item3 { get; set; }

	public FTuple(T1 item1, T2 item2, T3 item3)
	{
		Item1 = item1;
		Item2 = item2;
		Item3 = item3;
	}

	public override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 31 + ((Item1 == null) ? 0 : Item1Comparer.GetHashCode(Item1));
		hash = hash * 31 + ((Item2 == null) ? 0 : Item2Comparer.GetHashCode(Item2));
		hash = hash * 31 + ((Item3 == null) ? 0 : Item3Comparer.GetHashCode(Item3));
		return hash;
	}

	public override bool Equals(object obj)
	{
		FTuple<T1, T2, T3> other = obj as FTuple<T1, T2, T3>;
		if (other == null)
		{
			return false;
		}
		return Item1Comparer.Equals(Item1, other.Item1) && Item2Comparer.Equals(Item2, other.Item2) && Item3Comparer.Equals(Item3, other.Item3);
	}

	public override string ToString()
	{
		return ToString(null, null);
	}

	public string ToString(string format, IFormatProvider formatProvider)
	{
		return string.Format(formatProvider, "({0}, {1}, {2})", Item1, Item2, Item3);
	}
}

public class FTuple<T1, T2, T3, T4>
{
	private static readonly IEqualityComparer<T1> Item1Comparer = EqualityComparer<T1>.Default;

	private static readonly IEqualityComparer<T2> Item2Comparer = EqualityComparer<T2>.Default;

	private static readonly IEqualityComparer<T3> Item3Comparer = EqualityComparer<T3>.Default;

	private static readonly IEqualityComparer<T4> Item4Comparer = EqualityComparer<T4>.Default;

	public T1 Item1 { get; set; }

	public T2 Item2 { get; set; }

	public T3 Item3 { get; set; }

	public T4 Item4 { get; set; }

	public FTuple(T1 item1, T2 item2, T3 item3, T4 item4)
	{
		Item1 = item1;
		Item2 = item2;
		Item3 = item3;
		Item4 = item4;
	}

	public override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 31 + ((Item1 == null) ? 0 : Item1Comparer.GetHashCode(Item1));
		hash = hash * 31 + ((Item2 == null) ? 0 : Item2Comparer.GetHashCode(Item2));
		hash = hash * 31 + ((Item3 == null) ? 0 : Item3Comparer.GetHashCode(Item3));
		hash = hash * 31 + ((Item4 == null) ? 0 : Item4Comparer.GetHashCode(Item4));
		return hash;
	}

	public override bool Equals(object obj)
	{
		FTuple<T1, T2, T3, T4> other = obj as FTuple<T1, T2, T3, T4>;
		if (other == null)
		{
			return false;
		}
		return Item1Comparer.Equals(Item1, other.Item1) && Item2Comparer.Equals(Item2, other.Item2) && Item3Comparer.Equals(Item3, other.Item3) && Item4Comparer.Equals(Item4, other.Item4);
	}

	public override string ToString()
	{
		return ToString(null, null);
	}

	public string ToString(string format, IFormatProvider formatProvider)
	{
		return string.Format(formatProvider, "({0}, {1}, {2}, {3})", Item1, Item2, Item3, Item4);
	}
}

public class FTuple<T1, T2, T3, T4, T5>
{
	private static readonly IEqualityComparer<T1> Item1Comparer = EqualityComparer<T1>.Default;

	private static readonly IEqualityComparer<T2> Item2Comparer = EqualityComparer<T2>.Default;

	private static readonly IEqualityComparer<T3> Item3Comparer = EqualityComparer<T3>.Default;

	private static readonly IEqualityComparer<T4> Item4Comparer = EqualityComparer<T4>.Default;

	private static readonly IEqualityComparer<T5> Item5Comparer = EqualityComparer<T5>.Default;

	public T1 Item1 { get; set; }

	public T2 Item2 { get; set; }

	public T3 Item3 { get; set; }

	public T4 Item4 { get; set; }

	public T5 Item5 { get; set; }

	public FTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
	{
		Item1 = item1;
		Item2 = item2;
		Item3 = item3;
		Item4 = item4;
		Item5 = item5;
	}

	public override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 31 + ((Item1 == null) ? 0 : Item1Comparer.GetHashCode(Item1));
		hash = hash * 31 + ((Item2 == null) ? 0 : Item2Comparer.GetHashCode(Item2));
		hash = hash * 31 + ((Item3 == null) ? 0 : Item3Comparer.GetHashCode(Item3));
		hash = hash * 31 + ((Item4 == null) ? 0 : Item4Comparer.GetHashCode(Item4));
		hash = hash * 31 + ((Item5 == null) ? 0 : Item5Comparer.GetHashCode(Item5));
		return hash;
	}

	public override bool Equals(object obj)
	{
		FTuple<T1, T2, T3, T4, T5> other = obj as FTuple<T1, T2, T3, T4, T5>;
		if (other == null)
		{
			return false;
		}
		return Item1Comparer.Equals(Item1, other.Item1) && Item2Comparer.Equals(Item2, other.Item2) && Item3Comparer.Equals(Item3, other.Item3) && Item4Comparer.Equals(Item4, other.Item4) && Item5Comparer.Equals(Item5, other.Item5);
	}

	public override string ToString()
	{
		return ToString(null, null);
	}

	public string ToString(string format, IFormatProvider formatProvider)
	{
		return string.Format(formatProvider, "({0}, {1}, {2}, {3}, {4})", Item1, Item2, Item3, Item4, Item5);
	}
}
