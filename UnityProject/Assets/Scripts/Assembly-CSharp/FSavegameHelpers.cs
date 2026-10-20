using System;
using System.Collections.Generic;

public static class FSavegameHelpers
{
	public static void WriteList<T>(this FBinWriter writer, List<T> list, Action<FBinWriter, T> saveFunc) where T : class
	{
	}

	public static List<T> ReadList<T>(this FBinReader reader, Func<FBinReader, T> createAndLoadFunc) where T : class
	{
		return null;
	}

	public static void WriteLinkedList<T>(this FBinWriter writer, LinkedList<T> list, Action<FBinWriter, T> saveFunc) where T : class
	{
	}

	public static LinkedList<T> ReadLinkedList<T>(this FBinReader reader, Func<FBinReader, T> createAndLoadFunc) where T : class
	{
		return null;
	}

	public static void WriteDictValType<K, V>(this FBinWriter writer, Dictionary<K, V> dict, Action<FBinWriter, K> saveKeyFunc, Action<FBinWriter, V> saveValueFunc) where V : struct
	{
	}

	public static void WriteDict<K, V>(this FBinWriter writer, Dictionary<K, V> dict, Action<FBinWriter, K> saveKeyFunc, Action<FBinWriter, V> saveValueFunc) where V : class
	{
	}

	public static Dictionary<K, V> ReadDict<K, V>(this FBinReader reader, Func<FBinReader, K> createAndLoadKeyFunc, Func<FBinReader, V> createAndLoadValueFunc) where V : class
	{
		return null;
	}

	public static Dictionary<K, V> ReadDictValType<K, V>(this FBinReader reader, Func<FBinReader, K> createAndLoadKeyFunc, Func<FBinReader, V> createAndLoadValueFunc) where V : struct
	{
		return null;
	}
}
