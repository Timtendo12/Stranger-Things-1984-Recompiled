using System.Collections.Generic;

public static class FStatCollectionUtil
{
	public static void ConsolidateStats<TKey>(this Dictionary<TKey, ulong> dictionary, Dictionary<TKey, ulong> newDictionary)
	{
	}

	public static void ConsolidateStats<TKey>(this List<TKey> list, List<TKey> newList)
	{
	}
}
