namespace Unity.IO.Compression
{
	internal class FastEncoder
	{
		private FastEncoderWindow inputWindow;

		private Match currentMatch;

		private double lastCompressionRatio;

		internal int BytesInHistory => 0;

		internal DeflateInput UnprocessedInput => null;

		internal double LastCompressionRatio => 0.0;

		internal void FlushInput()
		{
		}

		internal void GetBlock(DeflateInput input, OutputBuffer output, int maxBytesToCopy)
		{
		}

		internal void GetCompressedData(DeflateInput input, OutputBuffer output)
		{
		}

		internal void GetBlockHeader(OutputBuffer output)
		{
		}

		internal void GetBlockFooter(OutputBuffer output)
		{
		}

		private void GetCompressedOutput(DeflateInput input, OutputBuffer output, int maxBytesToCopy)
		{
		}

		private void GetCompressedOutput(OutputBuffer output)
		{
		}

		private bool InputAvailable(DeflateInput input)
		{
			return false;
		}

		private bool SafeToWriteTo(OutputBuffer output)
		{
			return false;
		}

		private void WriteEndOfBlock(OutputBuffer output)
		{
		}

		internal static void WriteMatch(int matchLen, int matchPos, OutputBuffer output)
		{
		}

		internal static void WriteChar(byte b, OutputBuffer output)
		{
		}

		internal static void WriteDeflatePreamble(OutputBuffer output)
		{
		}
	}
}
