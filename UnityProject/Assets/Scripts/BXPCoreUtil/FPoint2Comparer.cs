using System.Collections.Generic;

public class FPoint2Comparer : IEqualityComparer<FPoint2>
{
	public bool Equals(FPoint2 a, FPoint2 b)
	{
		return false;
	}

	public int GetHashCode(FPoint2 a)
	{
		return 0;
	}
}
