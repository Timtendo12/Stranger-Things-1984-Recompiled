namespace Unity.IO.Compression
{
	internal static class FastEncoderStatics
	{
		internal static readonly byte[] FastEncoderTreeStructureData;

		internal static readonly byte[] BFinalFastEncoderTreeStructureData;

		internal static readonly uint[] FastEncoderLiteralCodeInfo;

		internal static readonly uint[] FastEncoderDistanceCodeInfo;

		internal static readonly uint[] BitMask;

		internal static readonly byte[] ExtraLengthBits;

		internal static readonly byte[] ExtraDistanceBits;

		internal const int NumChars = 256;

		internal const int NumLengthBaseCodes = 29;

		internal const int NumDistBaseCodes = 30;

		internal const uint FastEncoderPostTreeBitBuf = 34u;

		internal const int FastEncoderPostTreeBitCount = 9;

		internal const uint NoCompressionHeader = 0u;

		internal const int NoCompressionHeaderBitCount = 3;

		internal const uint BFinalNoCompressionHeader = 1u;

		internal const int BFinalNoCompressionHeaderBitCount = 3;

		internal const int MaxCodeLen = 16;

		private static byte[] distLookup;

		static FastEncoderStatics()
		{
		}

		internal static int GetSlot(int pos)
		{
			return 0;
		}

		public static uint BitReverse(uint code, int length)
		{
			return 0u;
		}
	}
}
