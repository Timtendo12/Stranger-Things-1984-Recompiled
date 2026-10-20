using System.Diagnostics;

namespace Unity.IO.Compression
{
	internal class FastEncoderWindow
	{
		private byte[] window;

		private int bufPos;

		private int bufEnd;

		private const int FastEncoderHashShift = 4;

		private const int FastEncoderHashtableSize = 2048;

		private const int FastEncoderHashMask = 2047;

		private const int FastEncoderWindowSize = 8192;

		private const int FastEncoderWindowMask = 8191;

		private const int FastEncoderMatch3DistThreshold = 16384;

		internal const int MaxMatch = 258;

		internal const int MinMatch = 3;

		private const int SearchDepth = 32;

		private const int GoodLength = 4;

		private const int NiceLength = 32;

		private const int LazyMatchThreshold = 6;

		private ushort[] prev;

		private ushort[] lookup;

		public int BytesAvailable => 0;

		public DeflateInput UnprocessedInput => null;

		public int FreeWindowSpace => 0;

		public void FlushWindow()
		{
		}

		private void ResetWindow()
		{
		}

		public void CopyBytes(byte[] inputBuffer, int startIndex, int count)
		{
		}

		public void MoveWindows()
		{
		}

		private uint HashValue(uint hash, byte b)
		{
			return 0u;
		}

		private uint InsertString(ref uint hash)
		{
			return 0u;
		}

		private void InsertStrings(ref uint hash, int matchLen)
		{
		}

		internal bool GetNextSymbolOrMatch(Match match)
		{
			return false;
		}

		private int FindMatch(int search, out int matchPos, int searchDepth, int niceLength)
		{
			matchPos = default;
			return 0;
		}

		[Conditional("DEBUG")]
		private void VerifyHashes()
		{
		}

		private uint RecalculateHash(int position)
		{
			return 0u;
		}
	}
}
