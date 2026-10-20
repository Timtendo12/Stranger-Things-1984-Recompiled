using System;
using System.Collections.Generic;

public class FSavegameWriter : IDisposable
{
	protected FBinWriter mUncompressedWriter;

	protected FBinWriter mOutputWriter;

	protected bool mDisposeWriter;

	protected Dictionary<object, int> mSavegameRefToID;

	public FBinWriter BinWriter => null;

	public FSavegameWriter(FBinWriter writer, bool disposeWriter, string fileTag)
	{
	}

	public FSavegameWriter(string filename, string fileTag)
	{
	}

	protected void StartSave(string fileTag)
	{
	}

	public void Dispose()
	{
	}

	public int GetSaveRef(object obj, out bool isNew)
	{
		isNew = default;
		return 0;
	}

	public void SaveTypeRef(Type type)
	{
	}

	public void WriteEnum<T>(T enumVal) where T : struct, IConvertible
	{
	}

	public void WriteEnumList<T>(List<T> enumList) where T : struct, IConvertible
	{
	}

	public void WriteList<T>(List<T> list, Action<FSavegameWriter, T> saveFunc) where T : class
	{
	}

	public void WriteLinkedList<T>(LinkedList<T> list, Action<FSavegameWriter, T> saveFunc) where T : class
	{
	}

	public void WriteQueue<T>(Queue<T> queue, Action<FSavegameWriter, T> saveFunc) where T : class
	{
	}

	public void WriteDictValType<K, V>(Dictionary<K, V> dict, Action<FSavegameWriter, K> saveKeyFunc, Action<FSavegameWriter, V> saveValueFunc) where V : struct
	{
	}

	public void WriteDict<K, V>(Dictionary<K, V> dict, Action<FSavegameWriter, K> saveKeyFunc, Action<FSavegameWriter, V> saveValueFunc) where V : class
	{
	}

	public void WriteNonRefList<T>(List<T> list, Action<FSavegameWriter, T> saveFunc)
	{
	}

	public void Write(int x)
	{
	}

	public void Write(long x)
	{
	}

	public void Write(uint x)
	{
	}

	public void Write(string x)
	{
	}

	public void WriteNullableString(string x)
	{
	}

	public void Write(float x)
	{
	}

	public void Write(Fix64 x)
	{
	}

	public void Write(FPoint2 x)
	{
	}

	public void Write(FixVector3 x)
	{
	}

	public void Write(FixQuaternion x)
	{
	}

	public void Write(FixMatrix4x4 x)
	{
	}

	public void Write(bool x)
	{
	}

	public void Write(byte x)
	{
	}

	public void Write(ulong x)
	{
	}

	public void Write(short x)
	{
	}

	public void Write(int[] x)
	{
	}

	public void Write(uint[] x)
	{
	}

	public void Write(List<int> x)
	{
	}

	public void Write(List<Fix64> x)
	{
	}

	public void Write(List<FPoint2> x)
	{
	}

	public void Write(byte[] x, bool writeSize = true)
	{
	}

	public void WriteRawBytes(byte[] x, int offset, int count)
	{
	}

	public void Write(Guid x)
	{
	}

	public void Write(List<string> x)
	{
	}

	public void Write(string[] x)
	{
	}

	public void Write(HashSet<int> x)
	{
	}

	public uint WriteTag(string tag)
	{
		return 0u;
	}

	public uint WriteTag(string tag, int version)
	{
		return 0u;
	}

	public void BeginChunk(string tag)
	{
	}

	public bool EndChunk(string tag)
	{
		return false;
	}
}
