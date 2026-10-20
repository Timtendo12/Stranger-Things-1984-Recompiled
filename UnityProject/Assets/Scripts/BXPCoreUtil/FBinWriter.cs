using System;
using System.Collections.Generic;
using System.IO;

public class FBinWriter : IDisposable
{
	protected Stream mStream;

	protected BinaryWriter mWriter;

	private bool mDisposed;

	protected Stack<FChunkEntry> mChunkStack;

	protected BinaryReader mReader;

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

	public FBinWriter()
	{
	}

	public FBinWriter(Stream stream)
	{
	}

	public static FBinWriter CreateForFile(string filename)
	{
		return null;
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

	public void Write(FPoint2 v)
	{
	}

	public void Write(FixVector3 v)
	{
	}

	public void Write(FixQuaternion v)
	{
	}

	public void Write(FixMatrix4x4 m)
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

	public void Write(DateTime x)
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

	public void Write(Guid guid)
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

	public byte[] GetBytes()
	{
		return null;
	}

	public byte[] GetRawBytes(out long length)
	{
		length = default;
		return null;
	}

	public void Dispose()
	{
	}

	protected virtual void Dispose(bool disposing)
	{
	}
}
