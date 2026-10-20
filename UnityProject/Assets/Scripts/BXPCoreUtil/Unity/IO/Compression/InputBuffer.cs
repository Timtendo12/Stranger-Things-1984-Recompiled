namespace Unity.IO.Compression
{
	internal class InputBuffer
	{
		private byte[] buffer;

		private int start;

		private int end;

		private uint bitBuffer;

		private int bitsInBuffer;

		public int AvailableBits => 0;

		public int AvailableBytes => 0;

		public bool EnsureBitsAvailable(int count)
		{
			return false;
		}

		public uint TryLoad16Bits()
		{
			return 0u;
		}

		private uint GetBitMask(int count)
		{
			return 0u;
		}

		public int GetBits(int count)
		{
			return 0;
		}

		public int CopyTo(byte[] output, int offset, int length)
		{
			return 0;
		}

		public bool NeedsInput()
		{
			return false;
		}

		public void SetInput(byte[] buffer, int offset, int length)
		{
		}

		public void SkipBits(int n)
		{
		}

		public void SkipToByteBoundary()
		{
		}
	}
}
