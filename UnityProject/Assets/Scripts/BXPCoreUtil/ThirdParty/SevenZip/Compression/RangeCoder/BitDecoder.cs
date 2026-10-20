// Standard LZMA SDK (public domain), restored to match the shipped signatures.
namespace SevenZip.Compression.RangeCoder
{
	internal struct BitDecoder
	{
		public const int kNumBitModelTotalBits = 11;

		public const uint kBitModelTotal = 2048u;

		private const int kNumMoveBits = 5;

		private uint Prob;

		public void UpdateModel(int numMoveBits, uint symbol)
		{
			if (symbol == 0)
			{
				Prob += (kBitModelTotal - Prob) >> numMoveBits;
			}
			else
			{
				Prob -= Prob >> numMoveBits;
			}
		}

		public void Init()
		{
			Prob = kBitModelTotal >> 1;
		}

		public uint Decode(Decoder rangeDecoder)
		{
			uint num = (rangeDecoder.Range >> kNumBitModelTotalBits) * Prob;
			if (rangeDecoder.Code < num)
			{
				rangeDecoder.Range = num;
				Prob += (kBitModelTotal - Prob) >> kNumMoveBits;
				if (rangeDecoder.Range < Decoder.kTopValue)
				{
					rangeDecoder.Code = (rangeDecoder.Code << 8) | (byte)rangeDecoder.Stream.ReadByte();
					rangeDecoder.Range <<= 8;
				}
				return 0u;
			}
			rangeDecoder.Range -= num;
			rangeDecoder.Code -= num;
			Prob -= Prob >> kNumMoveBits;
			if (rangeDecoder.Range < Decoder.kTopValue)
			{
				rangeDecoder.Code = (rangeDecoder.Code << 8) | (byte)rangeDecoder.Stream.ReadByte();
				rangeDecoder.Range <<= 8;
			}
			return 1u;
		}
	}
}
