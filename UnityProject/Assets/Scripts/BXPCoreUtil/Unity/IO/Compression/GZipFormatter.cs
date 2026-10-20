namespace Unity.IO.Compression
{
	internal class GZipFormatter : IFileFormatWriter
	{
		private byte[] headerBytes;

		private uint _crc32;

		private long _inputStreamSizeModulo;

		internal GZipFormatter()
		{
		}

		internal GZipFormatter(int compressionLevel)
		{
		}

		public byte[] GetHeader()
		{
			return null;
		}

		public void UpdateWithBytesRead(byte[] buffer, int offset, int bytesToCopy)
		{
		}

		public byte[] GetFooter()
		{
			return null;
		}

		internal void WriteUInt32(byte[] b, uint value, int startIndex)
		{
		}
	}
}
