using System;
using System.Collections.Generic;

internal class FFuncComparer<T> : IComparer<T>
{
	protected Comparison<T> mComparisonFunc;

	public FFuncComparer(Comparison<T> comparisonFunc)
	{
	}

	public int Compare(T x, T y)
	{
		return 0;
	}
}
