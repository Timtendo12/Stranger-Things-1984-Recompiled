using System.Collections.Generic;

public static class FSaveRef
{
	public static string GetSaveTag<T>(this T obj) where T : ISaveRef
	{
		return null;
	}

	public static void WriteRef<T>(this FSavegameWriter writer, T obj) where T : ISaveRef
	{
	}

	private static void Load<T>(this T obj, FSavegameReader reader) where T : ISaveRef
	{
	}

	public static T ReadRef<T>(this FSavegameReader reader) where T : class, ISaveRef, new()
	{
		return null;
	}

	public static void ReadRefIntoInstance<T>(this FSavegameReader reader, T instance) where T : class, ISaveRef, new()
	{
	}

	public static T ReadRefUnchecked<T>(this FSavegameReader reader) where T : class, ISaveRef
	{
		return null;
	}

	public static void WriteRefList<T>(this FSavegameWriter writer, List<T> list) where T : class, ISaveRef, new()
	{
	}

	public static List<T> ReadRefList<T>(this FSavegameReader reader) where T : class, ISaveRef, new()
	{
		return null;
	}

	public static void WriteRefQueue<T>(this FSavegameWriter writer, Queue<T> list) where T : class, ISaveRef, new()
	{
	}

	public static Queue<T> ReadRefQueue<T>(this FSavegameReader reader) where T : class, ISaveRef, new()
	{
		return null;
	}

	public static void WriteStringRefDict<V>(this FSavegameWriter writer, Dictionary<string, V> dict) where V : class, ISaveRef, new()
	{
	}

	public static Dictionary<string, V> ReadStringRefDict<V>(this FSavegameReader reader) where V : class, ISaveRef, new()
	{
		return null;
	}

	public static void WriteIntRefDict<V>(this FSavegameWriter writer, Dictionary<int, V> dict) where V : class, ISaveRef, new()
	{
	}

	public static Dictionary<int, V> ReadIntRefDict<V>(this FSavegameReader reader) where V : class, ISaveRef, new()
	{
		return null;
	}
}
