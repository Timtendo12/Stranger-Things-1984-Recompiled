namespace Unity.IO.Compression
{
	internal class OutputWindow
	{
		private const int WindowSize = 32768;

		private const int WindowMask = 32767;

		private byte[] window;

		private int end;

		private int bytesUsed;

		public int FreeBytes => 0;

		public int AvailableBytes => 0;

		public void Write(byte b)
		{
		}

		public void WriteLengthDistance(int length, int distance)
		{
		}

		public int CopyFrom(InputBuffer input, int length)
		{
			return 0;
		}

		public int CopyTo(byte[] output, int offset, int length)
		{
			return 0;
		}
	}
}
