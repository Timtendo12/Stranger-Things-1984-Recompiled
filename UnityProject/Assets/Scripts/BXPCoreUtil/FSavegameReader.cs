using System;
using System.Collections.Generic;

public class FSavegameReader : FBinReader
{
	protected FBinReader mInputReader;

	protected bool mDisposeReader;

	protected Dictionary<int, object> mSavegameIDToRef;

	protected Dictionary<Type, int> mVersionMap;

	protected Dictionary<Type, FSaveEnumInfo> mEnumLoadMap;

	public FSavegameReader(FBinReader reader, bool disposeReader, string fileTag)
		: base((byte[])null)
	{
	}

	public FSavegameReader(string filename, string fileTag)
		: base((byte[])null)
	{
	}

	protected void StartLoad(string fileTag)
	{
	}

	public T RemapEnumFromLong<T>(long val, T defaultVal) where T : struct, IConvertible
	{
		return default;
	}

	public void AddLoadRef(int refID, object obj)
	{
	}

	public T GetLoadRef<T>(int objID) where T : class
	{
		return null;
	}

	public Type LoadTypeRef()
	{
		return null;
	}

	public T CreateTypeFromReader<T>() where T : class, new()
	{
		return null;
	}

	public T CreateTypeFromReaderUnchecked<T>() where T : class
	{
		return null;
	}

	public int GetVersion(Type type)
	{
		return 0;
	}

	public T ReadEnum<T>(T defaultVal) where T : struct, IConvertible
	{
		return default;
	}

	public List<T> ReadEnumList<T>(T defaultVal) where T : struct, IConvertible
	{
		return null;
	}

	public List<T> ReadList<T>(Func<FSavegameReader, T> createAndLoadFunc) where T : class
	{
		return null;
	}

	public LinkedList<T> ReadLinkedList<T>(Func<FSavegameReader, T> createAndLoadFunc) where T : class
	{
		return null;
	}

	public Queue<T> ReadQueue<T>(Func<FSavegameReader, T> createAndLoadFunc) where T : class
	{
		return null;
	}

	public Dictionary<K, V> ReadDict<K, V>(Func<FSavegameReader, K> createAndLoadKeyFunc, Func<FSavegameReader, V> createAndLoadValueFunc) where V : class
	{
		return null;
	}

	public Dictionary<K, V> ReadDictValType<K, V>(Func<FSavegameReader, K> createAndLoadKeyFunc, Func<FSavegameReader, V> createAndLoadValueFunc) where V : struct
	{
		return null;
	}

	public List<T> ReadNonRefList<T>(Func<FSavegameReader, T> loadFunc)
	{
		return null;
	}
}
