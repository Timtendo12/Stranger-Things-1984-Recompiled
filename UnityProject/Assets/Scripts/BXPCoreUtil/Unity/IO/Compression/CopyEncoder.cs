namespace Unity.IO.Compression
{
	internal class CopyEncoder
	{
		private const int PaddingSize = 5;

		private const int MaxUncompressedBlockSize = 65536;

		public void GetBlock(DeflateInput input, OutputBuffer output, bool isFinal)
		{
		}

		private void WriteLenNLen(ushort len, OutputBuffer output)
		{
		}
	}
}
