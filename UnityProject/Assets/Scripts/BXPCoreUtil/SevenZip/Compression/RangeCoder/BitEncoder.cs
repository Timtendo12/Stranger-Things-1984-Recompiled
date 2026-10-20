// Standard LZMA SDK (public domain), restored to match the shipped signatures.
namespace SevenZip.Compression.RangeCoder
{
	internal struct BitEncoder
	{
		public const int kNumBitModelTotalBits = 11;

		public const uint kBitModelTotal = 2048u;

		private const int kNumMoveBits = 5;

		private const int kNumMoveReducingBits = 2;

		public const int kNumBitPriceShiftBits = 6;

		private uint Prob;

		private static uint[] ProbPrices;

		public void Init()
		{
			Prob = kBitModelTotal >> 1;
		}

		public void UpdateModel(uint symbol)
		{
			if (symbol == 0)
			{
				Prob += (kBitModelTotal - Prob) >> kNumMoveBits;
			}
			else
			{
				Prob -= Prob >> kNumMoveBits;
			}
		}

		public void Encode(Encoder encoder, uint symbol)
		{
			uint num = (encoder.Range >> kNumBitModelTotalBits) * Prob;
			if (symbol == 0)
			{
				encoder.Range = num;
				Prob += (kBitModelTotal - Prob) >> kNumMoveBits;
			}
			else
			{
				encoder.Low += num;
				encoder.Range -= num;
				Prob -= Prob >> kNumMoveBits;
			}
			while (encoder.Range < Encoder.kTopValue)
			{
				encoder.Range <<= 8;
				encoder.ShiftLow();
			}
		}

		static BitEncoder()
		{
			ProbPrices = new uint[kBitModelTotal >> kNumMoveReducingBits];
			const int kNumBits = kNumBitModelTotalBits - kNumMoveReducingBits;
			for (int i = kNumBits - 1; i >= 0; i--)
			{
				uint start = 1u << (kNumBits - i - 1);
				uint end = 1u << (kNumBits - i);
				for (uint j = start; j < end; j++)
				{
					ProbPrices[j] = ((uint)i << kNumBitPriceShiftBits) + (((end - j) << kNumBitPriceShiftBits) >> (kNumBits - i - 1));
				}
			}
		}

		public uint GetPrice(uint symbol)
		{
			return ProbPrices[(((Prob - symbol) ^ (uint)(-(int)symbol)) & (kBitModelTotal - 1)) >> kNumMoveReducingBits];
		}

		public uint GetPrice0()
		{
			return ProbPrices[Prob >> kNumMoveReducingBits];
		}

		public uint GetPrice1()
		{
			return ProbPrices[(kBitModelTotal - Prob) >> kNumMoveReducingBits];
		}
	}
}
