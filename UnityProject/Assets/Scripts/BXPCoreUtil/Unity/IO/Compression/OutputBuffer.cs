namespace Unity.IO.Compression
{
	internal class OutputBuffer
	{
		internal struct BufferState
		{
			internal int pos;

			internal uint bitBuf;

			internal int bitCount;
		}

		private byte[] byteBuffer;

		private int pos;

		private uint bitBuf;

		private int bitCount;

		internal int BytesWritten => 0;

		internal int FreeBytes => 0;

		internal int BitsInBuffer => 0;

		internal void UpdateBuffer(byte[] output)
		{
		}

		internal void WriteUInt16(ushort value)
		{
		}

		internal void WriteBits(int n, uint bits)
		{
		}

		internal void FlushBits()
		{
		}

		internal void WriteBytes(byte[] byteArray, int offset, int count)
		{
		}

		private void WriteBytesUnaligned(byte[] byteArray, int offset, int count)
		{
		}

		private void WriteByteUnaligned(byte b)
		{
		}

		internal BufferState DumpState()
		{
			return default;
		}

		internal void RestoreState(BufferState state)
		{
		}
	}
}
