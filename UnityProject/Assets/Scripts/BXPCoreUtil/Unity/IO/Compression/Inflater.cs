namespace Unity.IO.Compression
{
	internal class Inflater
	{
		private static readonly byte[] extraLengthBits;

		private static readonly int[] lengthBase;

		private static readonly int[] distanceBasePosition;

		private static readonly byte[] codeOrder;

		private static readonly byte[] staticDistanceTreeTable;

		private OutputWindow output;

		private InputBuffer input;

		private HuffmanTree literalLengthTree;

		private HuffmanTree distanceTree;

		private InflaterState state;

		private bool hasFormatReader;

		private int bfinal;

		private BlockType blockType;

		private byte[] blockLengthBuffer;

		private int blockLength;

		private int length;

		private int distanceCode;

		private int extraBits;

		private int loopCounter;

		private int literalLengthCodeCount;

		private int distanceCodeCount;

		private int codeLengthCodeCount;

		private int codeArraySize;

		private int lengthCode;

		private byte[] codeList;

		private byte[] codeLengthTreeCodeLength;

		private HuffmanTree codeLengthTree;

		private IFileFormatReader formatReader;

		public int AvailableOutput => 0;

		internal void SetFileFormatReader(IFileFormatReader reader)
		{
		}

		private void Reset()
		{
		}

		public void SetInput(byte[] inputBytes, int offset, int length)
		{
		}

		public bool Finished()
		{
			return false;
		}

		public bool NeedsInput()
		{
			return false;
		}

		public int Inflate(byte[] bytes, int offset, int length)
		{
			return 0;
		}

		private bool Decode()
		{
			return false;
		}

		private bool DecodeUncompressedBlock(out bool end_of_block)
		{
			end_of_block = default;
			return false;
		}

		private bool DecodeBlock(out bool end_of_block_code_seen)
		{
			end_of_block_code_seen = default;
			return false;
		}

		private bool DecodeDynamicBlockHeader()
		{
			return false;
		}
	}
}
