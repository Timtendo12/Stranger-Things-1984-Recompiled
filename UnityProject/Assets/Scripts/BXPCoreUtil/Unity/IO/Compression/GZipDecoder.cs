using System;

namespace Unity.IO.Compression
{
	internal class GZipDecoder : IFileFormatReader
	{
		internal enum GzipHeaderState
		{
			ReadingID1 = 0,
			ReadingID2 = 1,
			ReadingCM = 2,
			ReadingFLG = 3,
			ReadingMMTime = 4,
			ReadingXFL = 5,
			ReadingOS = 6,
			ReadingXLen1 = 7,
			ReadingXLen2 = 8,
			ReadingXLenData = 9,
			ReadingFileName = 10,
			ReadingComment = 11,
			ReadingCRC16Part1 = 12,
			ReadingCRC16Part2 = 13,
			Done = 14,
			ReadingCRC = 15,
			ReadingFileSize = 16
		}

		[Flags]
		internal enum GZipOptionalHeaderFlags
		{
			CRCFlag = 2,
			ExtraFieldsFlag = 4,
			FileNameFlag = 8,
			CommentFlag = 0x10
		}

		private GzipHeaderState gzipHeaderSubstate;

		private GzipHeaderState gzipFooterSubstate;

		private int gzip_header_flag;

		private int gzip_header_xlen;

		private uint expectedCrc32;

		private uint expectedOutputStreamSizeModulo;

		private int loopCounter;

		private uint actualCrc32;

		private long actualStreamSizeModulo;

		public void Reset()
		{
		}

		public bool ReadHeader(InputBuffer input)
		{
			return false;
		}

		public bool ReadFooter(InputBuffer input)
		{
			return false;
		}

		public void UpdateWithBytesRead(byte[] buffer, int offset, int copied)
		{
		}

		public void Validate()
		{
		}
	}
}
