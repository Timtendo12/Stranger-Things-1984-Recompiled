using System;
using System.Collections.Generic;
using System.IO;

public class FBinReader : IDisposable
{
	protected Stream mStream;

	protected BinaryReader mReader;

	private bool mDisposed;

	protected Stack<FChunkEntry> mChunkStack;

	private static List<string> sLastNReadTags;

	private const int cNumTagsToLog = 5;

	public long Position
	{
		get
		{
			return 0L;
		}
		set
		{
		}
	}

	public Stream Stream => null;

	public FBinReader(byte[] bytes)
	{
	}

	public FBinReader(byte[] bytes, int index, int count)
	{
	}

	public FBinReader(Stream stream)
	{
	}

	public static FBinReader CreateForFile(string filename)
	{
		return null;
	}

	public int ReadInt()
	{
		return 0;
	}

	public uint ReadUInt()
	{
		return 0u;
	}

	public long ReadLong()
	{
		return 0L;
	}

	public short ReadShort()
	{
		return 0;
	}

	public DateTime ReadDateTime()
	{
		return default;
	}

	public string ReadString()
	{
		return null;
	}

	public string ReadNullableString()
	{
		return null;
	}

	public float ReadFloat()
	{
		return 0f;
	}

	public Fix64 ReadFix64()
	{
		return default;
	}

	public FPoint2 ReadFPoint2()
	{
		return default;
	}

	public FixVector3 ReadFixVector3()
	{
		return default;
	}

	public FixQuaternion ReadFixQuaternion()
	{
		return default;
	}

	public FixMatrix4x4 ReadFixMatrix4x4()
	{
		return default;
	}

	public bool ReadBool()
	{
		return false;
	}

	public byte ReadByte()
	{
		return 0;
	}

	public ulong ReadUlong()
	{
		return 0uL;
	}

	public int[] ReadIntArray()
	{
		return null;
	}

	public uint[] ReadUIntArray()
	{
		return null;
	}

	public List<int> ReadIntList(bool nullForEmpty = false)
	{
		return null;
	}

	public List<Fix64> ReadFix64List(bool nullForEmpty = false)
	{
		return null;
	}

	public List<FPoint2> ReadFPoint2List(bool nullForEmpty = false)
	{
		return null;
	}

	public byte[] ReadByteArray(int count = -1)
	{
		return null;
	}

	public Guid ReadGuid()
	{
		return default;
	}

	public List<string> ReadStringList(bool nullForEmpty = false)
	{
		return null;
	}

	public string[] ReadStringArray(bool nullForEmpty = false)
	{
		return null;
	}

	public HashSet<int> ReadIntHashSet()
	{
		return null;
	}

	public bool ReadTag(string expectedTag, out uint expectedTagCrc, bool rewindOnFailure = true, bool silent = false)
	{
		expectedTagCrc = default;
		return false;
	}

	public bool ReadTag(string expectedTag, bool rewindOnFailure = true, bool silent = false)
	{
		return false;
	}

	public bool ReadTag(string expectedTag, out int version, int maxVersion = 2147483647)
	{
		version = default;
		return false;
	}

	public bool BeginChunk(string tag, bool throwOnFailure = true)
	{
		return false;
	}

	public bool EndChunk(string tag, bool throwOnFailure = true)
	{
		return false;
	}

	public bool SkipChunk(string tag)
	{
		return false;
	}

	public bool SkipRestOfChunk(string tag, bool throwOnFailure = true)
	{
		return false;
	}

	public bool ReadUnremappedEnumFromInt<T>(out T result, T defaultResult = default(T)) where T : struct, IConvertible
	{
		result = default;
		return false;
	}

	public T ReadUnremappedEnumFromInt<T>(T defaultResult = default(T)) where T : struct, IConvertible
	{
		return default;
	}

	public void Dispose()
	{
	}

	protected virtual void Dispose(bool disposing)
	{
	}
}
