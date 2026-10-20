using System.Collections.Generic;

public static class FSaveNonRef
{
	public static void Write<T>(this FSavegameWriter writer, T obj) where T : ISaveNonRef
	{
	}

	public static T Read<T>(this FSavegameReader reader) where T : ISaveNonRef, new()
	{
		return default;
	}

	public static void WriteList<T>(this FSavegameWriter writer, List<T> list) where T : ISaveNonRef, new()
	{
	}

	public static List<T> ReadList<T>(this FSavegameReader reader) where T : ISaveNonRef, new()
	{
		return null;
	}
}
