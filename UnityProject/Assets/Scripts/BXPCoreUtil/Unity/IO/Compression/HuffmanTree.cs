namespace Unity.IO.Compression
{
	internal class HuffmanTree
	{
		internal const int MaxLiteralTreeElements = 288;

		internal const int MaxDistTreeElements = 32;

		internal const int EndOfBlockCode = 256;

		internal const int NumberOfCodeLengthTreeElements = 19;

		private int tableBits;

		private short[] table;

		private short[] left;

		private short[] right;

		private byte[] codeLengthArray;

		private int tableMask;

		private static HuffmanTree staticLiteralLengthTree;

		private static HuffmanTree staticDistanceTree;

		public static HuffmanTree StaticLiteralLengthTree => null;

		public static HuffmanTree StaticDistanceTree => null;

		static HuffmanTree()
		{
		}

		public HuffmanTree(byte[] codeLengths)
		{
		}

		private static byte[] GetStaticLiteralTreeLength()
		{
			return null;
		}

		private static byte[] GetStaticDistanceTreeLength()
		{
			return null;
		}

		private uint[] CalculateHuffmanCode()
		{
			return null;
		}

		private void CreateTable()
		{
		}

		public int GetNextSymbol(InputBuffer input)
		{
			return 0;
		}
	}
}
