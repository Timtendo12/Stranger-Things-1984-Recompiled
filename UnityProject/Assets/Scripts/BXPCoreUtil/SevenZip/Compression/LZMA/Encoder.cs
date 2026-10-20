// Standard LZMA SDK (public domain), restored to match the shipped signatures.
using System;
using System.IO;
using SevenZip.Compression.LZ;
using SevenZip.Compression.RangeCoder;

namespace SevenZip.Compression.LZMA
{
	public class Encoder : ICoder, ISetCoderProperties, IWriteCoderProperties
	{
		private enum EMatchFinderType
		{
			BT2 = 0,
			BT4 = 1
		}

		private class LiteralEncoder
		{
			public struct Encoder2
			{
				private BitEncoder[] m_Encoders;

				public void Create()
				{
					m_Encoders = new BitEncoder[768];
				}

				public void Init()
				{
					for (int i = 0; i < 768; i++)
					{
						m_Encoders[i].Init();
					}
				}

				public void Encode(SevenZip.Compression.RangeCoder.Encoder rangeEncoder, byte symbol)
				{
					uint num = 1u;
					for (int num2 = 7; num2 >= 0; num2--)
					{
						uint num3 = (uint)((symbol >> num2) & 1);
						m_Encoders[num].Encode(rangeEncoder, num3);
						num = (num << 1) | num3;
					}
				}

				public void EncodeMatched(SevenZip.Compression.RangeCoder.Encoder rangeEncoder, byte matchByte, byte symbol)
				{
					uint num = 1u;
					bool flag = true;
					for (int num2 = 7; num2 >= 0; num2--)
					{
						uint num3 = (uint)((symbol >> num2) & 1);
						uint num4 = num;
						if (flag)
						{
							uint num5 = (uint)((matchByte >> num2) & 1);
							num4 += (1 + num5) << 8;
							flag = num5 == num3;
						}
						m_Encoders[num4].Encode(rangeEncoder, num3);
						num = (num << 1) | num3;
					}
				}

				public uint GetPrice(bool matchMode, byte matchByte, byte symbol)
				{
					uint num = 0u;
					uint num2 = 1u;
					int num3 = 7;
					if (matchMode)
					{
						while (num3 >= 0)
						{
							uint num4 = (uint)(matchByte >> num3) & 1u;
							uint num5 = (uint)(symbol >> num3) & 1u;
							num += m_Encoders[((1 + num4) << 8) + num2].GetPrice(num5);
							num2 = (num2 << 1) | num5;
							if (num4 != num5)
							{
								num3--;
								break;
							}
							num3--;
						}
					}
					while (num3 >= 0)
					{
						uint num6 = (uint)(symbol >> num3) & 1u;
						num += m_Encoders[num2].GetPrice(num6);
						num2 = (num2 << 1) | num6;
						num3--;
					}
					return num;
				}
			}

			private Encoder2[] m_Coders;

			private int m_NumPrevBits;

			private int m_NumPosBits;

			private uint m_PosMask;

			public void Create(int numPosBits, int numPrevBits)
			{
				if (m_Coders == null || m_NumPrevBits != numPrevBits || m_NumPosBits != numPosBits)
				{
					m_NumPosBits = numPosBits;
					m_PosMask = (1u << numPosBits) - 1;
					m_NumPrevBits = numPrevBits;
					uint num = 1u << m_NumPrevBits + m_NumPosBits;
					m_Coders = new Encoder2[num];
					for (uint num2 = 0u; num2 < num; num2++)
					{
						m_Coders[num2].Create();
					}
				}
			}

			public void Init()
			{
				uint num = 1u << m_NumPrevBits + m_NumPosBits;
				for (uint num2 = 0u; num2 < num; num2++)
				{
					m_Coders[num2].Init();
				}
			}

			public Encoder2 GetSubCoder(uint pos, byte prevByte)
			{
				return m_Coders[((pos & m_PosMask) << m_NumPrevBits) + (uint)(prevByte >> 8 - m_NumPrevBits)];
			}
		}

		private class LenEncoder
		{
			private BitEncoder _choice = default(BitEncoder);

			private BitEncoder _choice2 = default(BitEncoder);

			private BitTreeEncoder[] _lowCoder = new BitTreeEncoder[16];

			private BitTreeEncoder[] _midCoder = new BitTreeEncoder[16];

			private BitTreeEncoder _highCoder = new BitTreeEncoder(8);

			public LenEncoder()
			{
				for (uint num = 0u; num < 16; num++)
				{
					_lowCoder[num] = new BitTreeEncoder(3);
					_midCoder[num] = new BitTreeEncoder(3);
				}
			}

			public void Init(uint numPosStates)
			{
				_choice.Init();
				_choice2.Init();
				for (uint num = 0u; num < numPosStates; num++)
				{
					_lowCoder[num].Init();
					_midCoder[num].Init();
				}
				_highCoder.Init();
			}

			public void Encode(SevenZip.Compression.RangeCoder.Encoder rangeEncoder, uint symbol, uint posState)
			{
				if (symbol < 8)
				{
					_choice.Encode(rangeEncoder, 0u);
					_lowCoder[posState].Encode(rangeEncoder, symbol);
					return;
				}
				symbol -= 8;
				_choice.Encode(rangeEncoder, 1u);
				if (symbol < 8)
				{
					_choice2.Encode(rangeEncoder, 0u);
					_midCoder[posState].Encode(rangeEncoder, symbol);
				}
				else
				{
					_choice2.Encode(rangeEncoder, 1u);
					_highCoder.Encode(rangeEncoder, symbol - 8);
				}
			}

			public void SetPrices(uint posState, uint numSymbols, uint[] prices, uint st)
			{
				uint price = _choice.GetPrice0();
				uint price2 = _choice.GetPrice1();
				uint num = price2 + _choice2.GetPrice0();
				uint num2 = price2 + _choice2.GetPrice1();
				uint i;
				for (i = 0u; i < 8; i++)
				{
					if (i >= numSymbols)
					{
						return;
					}
					prices[st + i] = price + _lowCoder[posState].GetPrice(i);
				}
				for (; i < 16; i++)
				{
					if (i >= numSymbols)
					{
						return;
					}
					prices[st + i] = num + _midCoder[posState].GetPrice(i - 8);
				}
				for (; i < numSymbols; i++)
				{
					prices[st + i] = num2 + _highCoder.GetPrice(i - 8 - 8);
				}
			}
		}

		private class LenPriceTableEncoder : LenEncoder
		{
			private uint[] _prices = new uint[4352];

			private uint _tableSize;

			private uint[] _counters = new uint[16];

			public void SetTableSize(uint tableSize)
			{
				_tableSize = tableSize;
			}

			public uint GetPrice(uint symbol, uint posState)
			{
				return _prices[posState * 272 + symbol];
			}

			private void UpdateTable(uint posState)
			{
				SetPrices(posState, _tableSize, _prices, posState * 272);
				_counters[posState] = _tableSize;
			}

			public void UpdateTables(uint numPosStates)
			{
				for (uint num = 0u; num < numPosStates; num++)
				{
					UpdateTable(num);
				}
			}

			public new void Encode(SevenZip.Compression.RangeCoder.Encoder rangeEncoder, uint symbol, uint posState)
			{
				base.Encode(rangeEncoder, symbol, posState);
				if (--_counters[posState] == 0)
				{
					UpdateTable(posState);
				}
			}
		}

		private class Optimal
		{
			public Base.State State;

			public bool Prev1IsChar;

			public bool Prev2;

			public uint PosPrev2;

			public uint BackPrev2;

			public uint Price;

			public uint PosPrev;

			public uint BackPrev;

			public uint Backs0;

			public uint Backs1;

			public uint Backs2;

			public uint Backs3;

			public void MakeAsChar()
			{
				BackPrev = uint.MaxValue;
				Prev1IsChar = false;
			}

			public void MakeAsShortRep()
			{
				BackPrev = 0u;
				Prev1IsChar = false;
			}

			public bool IsShortRep()
			{
				return BackPrev == 0;
			}
		}

		private const uint kIfinityPrice = 268435455u;

		private static byte[] g_FastPos = new byte[2048];

		private Base.State _state = default(Base.State);

		private byte _previousByte;

		private uint[] _repDistances = new uint[4];

		private const int kDefaultDictionaryLogSize = 11;

		private const uint kNumFastBytesDefault = 32u;

		private const uint kNumLenSpecSymbols = 16u;

		private const uint kNumOpts = 4096u;

		private Optimal[] _optimum = new Optimal[4096];

		private IMatchFinder _matchFinder;

		private SevenZip.Compression.RangeCoder.Encoder _rangeEncoder = new SevenZip.Compression.RangeCoder.Encoder();

		private BitEncoder[] _isMatch = new BitEncoder[192];

		private BitEncoder[] _isRep = new BitEncoder[12];

		private BitEncoder[] _isRepG0 = new BitEncoder[12];

		private BitEncoder[] _isRepG1 = new BitEncoder[12];

		private BitEncoder[] _isRepG2 = new BitEncoder[12];

		private BitEncoder[] _isRep0Long = new BitEncoder[192];

		private BitTreeEncoder[] _posSlotEncoder = new BitTreeEncoder[4];

		private BitEncoder[] _posEncoders = new BitEncoder[114];

		private BitTreeEncoder _posAlignEncoder = new BitTreeEncoder(4);

		private LenPriceTableEncoder _lenEncoder = new LenPriceTableEncoder();

		private LenPriceTableEncoder _repMatchLenEncoder = new LenPriceTableEncoder();

		private LiteralEncoder _literalEncoder = new LiteralEncoder();

		private uint[] _matchDistances = new uint[548];

		private uint _numFastBytes = 32u;

		private uint _longestMatchLength;

		private uint _numDistancePairs;

		private uint _additionalOffset;

		private uint _optimumEndIndex;

		private uint _optimumCurrentIndex;

		private bool _longestMatchWasFound;

		private uint[] _posSlotPrices = new uint[256];

		private uint[] _distancesPrices = new uint[512];

		private uint[] _alignPrices = new uint[16];

		private uint _alignPriceCount;

		private uint _distTableSize = 44u;

		private int _posStateBits = 2;

		private uint _posStateMask = 3u;

		private int _numLiteralPosStateBits;

		private int _numLiteralContextBits = 3;

		private uint _dictionarySize = 2048u;

		private uint _dictionarySizePrev = uint.MaxValue;

		private uint _numFastBytesPrev = uint.MaxValue;

		private long nowPos64;

		private bool _finished;

		private Stream _inStream;

		private EMatchFinderType _matchFinderType = EMatchFinderType.BT4;

		private bool _writeEndMark;

		private bool _needReleaseMFStream;

		private uint[] reps = new uint[4];

		private uint[] repLens = new uint[4];

		private const int kPropSize = 5;

		private byte[] properties = new byte[5];

		private uint[] tempPrices = new uint[128];

		private uint _matchPriceCount;

		private static string[] kMatchFinderIDs = new string[2] { "BT2", "BT4" };

		private uint _trainSize;

		static Encoder()
		{
			int num = 2;
			g_FastPos[0] = 0;
			g_FastPos[1] = 1;
			for (byte b = 2; b < 22; b++)
			{
				uint num2 = 1u << (b >> 1) - 1;
				uint num3 = 0u;
				while (num3 < num2)
				{
					g_FastPos[num] = b;
					num3++;
					num++;
				}
			}
		}

		public Encoder()
		{
			for (int i = 0; (long)i < 4096L; i++)
			{
				_optimum[i] = new Optimal();
			}
			for (int j = 0; (long)j < 4L; j++)
			{
				_posSlotEncoder[j] = new BitTreeEncoder(6);
			}
		}

		private static uint GetPosSlot(uint pos)
		{
			if (pos < 2048)
			{
				return g_FastPos[pos];
			}
			if (pos < 2097152)
			{
				return (uint)(g_FastPos[pos >> 10] + 20);
			}
			return (uint)(g_FastPos[pos >> 20] + 40);
		}

		private static uint GetPosSlot2(uint pos)
		{
			if (pos < 131072)
			{
				return (uint)(g_FastPos[pos >> 6] + 12);
			}
			if (pos < 134217728)
			{
				return (uint)(g_FastPos[pos >> 16] + 32);
			}
			return (uint)(g_FastPos[pos >> 26] + 52);
		}

		private void BaseInit()
		{
			_state.Init();
			_previousByte = 0;
			for (uint num = 0u; num < 4; num++)
			{
				_repDistances[num] = 0u;
			}
		}

		private void Create()
		{
			if (_matchFinder == null)
			{
				BinTree binTree = new BinTree();
				int type = 4;
				if (_matchFinderType == EMatchFinderType.BT2)
				{
					type = 2;
				}
				binTree.SetType(type);
				_matchFinder = binTree;
			}
			_literalEncoder.Create(_numLiteralPosStateBits, _numLiteralContextBits);
			if (_dictionarySize != _dictionarySizePrev || _numFastBytesPrev != _numFastBytes)
			{
				_matchFinder.Create(_dictionarySize, 4096u, _numFastBytes, 274u);
				_dictionarySizePrev = _dictionarySize;
				_numFastBytesPrev = _numFastBytes;
			}
		}

		private void SetWriteEndMarkerMode(bool writeEndMarker)
		{
			_writeEndMark = writeEndMarker;
		}

		private void Init()
		{
			BaseInit();
			_rangeEncoder.Init();
			for (uint num = 0u; num < 12; num++)
			{
				for (uint num2 = 0u; num2 <= _posStateMask; num2++)
				{
					uint num3 = (num << 4) + num2;
					_isMatch[num3].Init();
					_isRep0Long[num3].Init();
				}
				_isRep[num].Init();
				_isRepG0[num].Init();
				_isRepG1[num].Init();
				_isRepG2[num].Init();
			}
			_literalEncoder.Init();
			for (uint num = 0u; num < 4; num++)
			{
				_posSlotEncoder[num].Init();
			}
			for (uint num = 0u; num < 114; num++)
			{
				_posEncoders[num].Init();
			}
			_lenEncoder.Init((uint)(1 << _posStateBits));
			_repMatchLenEncoder.Init((uint)(1 << _posStateBits));
			_posAlignEncoder.Init();
			_longestMatchWasFound = false;
			_optimumEndIndex = 0u;
			_optimumCurrentIndex = 0u;
			_additionalOffset = 0u;
		}

		private void ReadMatchDistances(out uint lenRes, out uint numDistancePairs)
		{
			lenRes = 0u;
			numDistancePairs = _matchFinder.GetMatches(_matchDistances);
			if (numDistancePairs != 0)
			{
				lenRes = _matchDistances[numDistancePairs - 2];
				if (lenRes == _numFastBytes)
				{
					lenRes += _matchFinder.GetMatchLen((int)(lenRes - 1), _matchDistances[numDistancePairs - 1], 273 - lenRes);
				}
			}
			_additionalOffset++;
		}

		private void MovePos(uint num)
		{
			if (num != 0)
			{
				_matchFinder.Skip(num);
				_additionalOffset += num;
			}
		}

		private uint GetRepLen1Price(Base.State state, uint posState)
		{
			return _isRepG0[state.Index].GetPrice0() + _isRep0Long[(state.Index << 4) + posState].GetPrice0();
		}

		private uint GetPureRepPrice(uint repIndex, Base.State state, uint posState)
		{
			uint num;
			if (repIndex == 0)
			{
				num = _isRepG0[state.Index].GetPrice0();
				num += _isRep0Long[(state.Index << 4) + posState].GetPrice1();
			}
			else
			{
				num = _isRepG0[state.Index].GetPrice1();
				if (repIndex == 1)
				{
					num += _isRepG1[state.Index].GetPrice0();
				}
				else
				{
					num += _isRepG1[state.Index].GetPrice1();
					num += _isRepG2[state.Index].GetPrice(repIndex - 2);
				}
			}
			return num;
		}

		private uint GetRepPrice(uint repIndex, uint len, Base.State state, uint posState)
		{
			uint price = _repMatchLenEncoder.GetPrice(len - 2, posState);
			return price + GetPureRepPrice(repIndex, state, posState);
		}

		private uint GetPosLenPrice(uint pos, uint len, uint posState)
		{
			uint num;
			uint lenToPosState = Base.GetLenToPosState(len);
			if (pos < 128)
			{
				num = _distancesPrices[lenToPosState * 128 + pos];
			}
			else
			{
				num = _posSlotPrices[(lenToPosState << 6) + GetPosSlot2(pos)] + _alignPrices[pos & 0xF];
			}
			return num + _lenEncoder.GetPrice(len - 2, posState);
		}

		private uint Backward(out uint backRes, uint cur)
		{
			_optimumEndIndex = cur;
			uint posPrev = _optimum[cur].PosPrev;
			uint backPrev = _optimum[cur].BackPrev;
			do
			{
				if (_optimum[cur].Prev1IsChar)
				{
					_optimum[posPrev].MakeAsChar();
					_optimum[posPrev].PosPrev = posPrev - 1;
					if (_optimum[cur].Prev2)
					{
						_optimum[posPrev - 1].Prev1IsChar = false;
						_optimum[posPrev - 1].PosPrev = _optimum[cur].PosPrev2;
						_optimum[posPrev - 1].BackPrev = _optimum[cur].BackPrev2;
					}
				}
				uint num = posPrev;
				uint backPrev2 = backPrev;
				backPrev = _optimum[num].BackPrev;
				posPrev = _optimum[num].PosPrev;
				_optimum[num].BackPrev = backPrev2;
				_optimum[num].PosPrev = cur;
				cur = num;
			}
			while (cur != 0);
			backRes = _optimum[0].BackPrev;
			_optimumCurrentIndex = _optimum[0].PosPrev;
			return _optimumCurrentIndex;
		}

		private uint GetOptimum(uint position, out uint backRes)
		{
			if (_optimumEndIndex != _optimumCurrentIndex)
			{
				uint result = _optimum[_optimumCurrentIndex].PosPrev - _optimumCurrentIndex;
				backRes = _optimum[_optimumCurrentIndex].BackPrev;
				_optimumCurrentIndex = _optimum[_optimumCurrentIndex].PosPrev;
				return result;
			}
			_optimumCurrentIndex = (_optimumEndIndex = 0u);
			uint lenRes;
			uint numDistancePairs;
			if (!_longestMatchWasFound)
			{
				ReadMatchDistances(out lenRes, out numDistancePairs);
			}
			else
			{
				lenRes = _longestMatchLength;
				numDistancePairs = _numDistancePairs;
				_longestMatchWasFound = false;
			}
			uint numAvailableBytes = _matchFinder.GetNumAvailableBytes() + 1;
			if (numAvailableBytes < 2)
			{
				backRes = uint.MaxValue;
				return 1u;
			}
			if (numAvailableBytes > 273)
			{
				numAvailableBytes = 273u;
			}
			uint repMaxIndex = 0u;
			uint i;
			for (i = 0u; i < 4; i++)
			{
				reps[i] = _repDistances[i];
				repLens[i] = _matchFinder.GetMatchLen(-1, reps[i], 273u);
				if (repLens[i] > repLens[repMaxIndex])
				{
					repMaxIndex = i;
				}
			}
			if (repLens[repMaxIndex] >= _numFastBytes)
			{
				backRes = repMaxIndex;
				uint result2 = repLens[repMaxIndex];
				MovePos(result2 - 1);
				return result2;
			}
			if (lenRes >= _numFastBytes)
			{
				backRes = _matchDistances[numDistancePairs - 1] + 4;
				MovePos(lenRes - 1);
				return lenRes;
			}
			byte indexByte = _matchFinder.GetIndexByte(-1);
			byte indexByte2 = _matchFinder.GetIndexByte((int)(0 - _repDistances[0] - 1 - 1));
			if (lenRes < 2 && indexByte != indexByte2 && repLens[repMaxIndex] < 2)
			{
				backRes = uint.MaxValue;
				return 1u;
			}
			_optimum[0].State = _state;
			uint posState = position & _posStateMask;
			_optimum[1].Price = _isMatch[(_state.Index << 4) + posState].GetPrice0() + _literalEncoder.GetSubCoder(position, _previousByte).GetPrice(!_state.IsCharState(), indexByte2, indexByte);
			_optimum[1].MakeAsChar();
			uint matchPrice = _isMatch[(_state.Index << 4) + posState].GetPrice1();
			uint repMatchPrice = matchPrice + _isRep[_state.Index].GetPrice1();
			if (indexByte2 == indexByte)
			{
				uint shortRepPrice = repMatchPrice + GetRepLen1Price(_state, posState);
				if (shortRepPrice < _optimum[1].Price)
				{
					_optimum[1].Price = shortRepPrice;
					_optimum[1].MakeAsShortRep();
				}
			}
			uint lenEnd = ((lenRes >= repLens[repMaxIndex]) ? lenRes : repLens[repMaxIndex]);
			if (lenEnd < 2)
			{
				backRes = _optimum[1].BackPrev;
				return 1u;
			}
			_optimum[1].PosPrev = 0u;
			_optimum[0].Backs0 = reps[0];
			_optimum[0].Backs1 = reps[1];
			_optimum[0].Backs2 = reps[2];
			_optimum[0].Backs3 = reps[3];
			uint num = lenEnd;
			do
			{
				_optimum[num--].Price = kIfinityPrice;
			}
			while (num >= 2);
			for (i = 0u; i < 4; i++)
			{
				uint repLen = repLens[i];
				if (repLen < 2)
				{
					continue;
				}
				uint price = repMatchPrice + GetPureRepPrice(i, _state, posState);
				do
				{
					uint num2 = price + _repMatchLenEncoder.GetPrice(repLen - 2, posState);
					Optimal optimal = _optimum[repLen];
					if (num2 < optimal.Price)
					{
						optimal.Price = num2;
						optimal.PosPrev = 0u;
						optimal.BackPrev = i;
						optimal.Prev1IsChar = false;
					}
				}
				while (--repLen >= 2);
			}
			uint normalMatchPrice = matchPrice + _isRep[_state.Index].GetPrice0();
			num = ((repLens[0] >= 2) ? (repLens[0] + 1) : 2u);
			if (num <= lenRes)
			{
				uint offs = 0u;
				while (num > _matchDistances[offs])
				{
					offs += 2;
				}
				while (true)
				{
					uint distance = _matchDistances[offs + 1];
					uint num3 = normalMatchPrice + GetPosLenPrice(distance, num, posState);
					Optimal optimal2 = _optimum[num];
					if (num3 < optimal2.Price)
					{
						optimal2.Price = num3;
						optimal2.PosPrev = 0u;
						optimal2.BackPrev = distance + 4;
						optimal2.Prev1IsChar = false;
					}
					if (num == _matchDistances[offs])
					{
						offs += 2;
						if (offs == numDistancePairs)
						{
							break;
						}
					}
					num++;
				}
			}
			uint cur = 0u;
			while (true)
			{
				cur++;
				if (cur == lenEnd)
				{
					return Backward(out backRes, cur);
				}
				uint newLen;
				ReadMatchDistances(out newLen, out numDistancePairs);
				if (newLen >= _numFastBytes)
				{
					_numDistancePairs = numDistancePairs;
					_longestMatchLength = newLen;
					_longestMatchWasFound = true;
					return Backward(out backRes, cur);
				}
				position++;
				uint posPrev = _optimum[cur].PosPrev;
				Base.State state;
				if (_optimum[cur].Prev1IsChar)
				{
					posPrev--;
					if (_optimum[cur].Prev2)
					{
						state = _optimum[_optimum[cur].PosPrev2].State;
						if (_optimum[cur].BackPrev2 < 4)
						{
							state.UpdateRep();
						}
						else
						{
							state.UpdateMatch();
						}
					}
					else
					{
						state = _optimum[posPrev].State;
					}
					state.UpdateChar();
				}
				else
				{
					state = _optimum[posPrev].State;
				}
				if (posPrev == cur - 1)
				{
					if (_optimum[cur].IsShortRep())
					{
						state.UpdateShortRep();
					}
					else
					{
						state.UpdateChar();
					}
				}
				else
				{
					uint pos;
					if (_optimum[cur].Prev1IsChar && _optimum[cur].Prev2)
					{
						posPrev = _optimum[cur].PosPrev2;
						pos = _optimum[cur].BackPrev2;
						state.UpdateRep();
					}
					else
					{
						pos = _optimum[cur].BackPrev;
						if (pos < 4)
						{
							state.UpdateRep();
						}
						else
						{
							state.UpdateMatch();
						}
					}
					Optimal optimal3 = _optimum[posPrev];
					if (pos < 4)
					{
						if (pos == 0)
						{
							reps[0] = optimal3.Backs0;
							reps[1] = optimal3.Backs1;
							reps[2] = optimal3.Backs2;
							reps[3] = optimal3.Backs3;
						}
						else if (pos == 1)
						{
							reps[0] = optimal3.Backs1;
							reps[1] = optimal3.Backs0;
							reps[2] = optimal3.Backs2;
							reps[3] = optimal3.Backs3;
						}
						else if (pos == 2)
						{
							reps[0] = optimal3.Backs2;
							reps[1] = optimal3.Backs0;
							reps[2] = optimal3.Backs1;
							reps[3] = optimal3.Backs3;
						}
						else
						{
							reps[0] = optimal3.Backs3;
							reps[1] = optimal3.Backs0;
							reps[2] = optimal3.Backs1;
							reps[3] = optimal3.Backs2;
						}
					}
					else
					{
						reps[0] = pos - 4;
						reps[1] = optimal3.Backs0;
						reps[2] = optimal3.Backs1;
						reps[3] = optimal3.Backs2;
					}
				}
				_optimum[cur].State = state;
				_optimum[cur].Backs0 = reps[0];
				_optimum[cur].Backs1 = reps[1];
				_optimum[cur].Backs2 = reps[2];
				_optimum[cur].Backs3 = reps[3];
				uint curPrice = _optimum[cur].Price;
				indexByte = _matchFinder.GetIndexByte(-1);
				indexByte2 = _matchFinder.GetIndexByte((int)(0 - reps[0] - 1 - 1));
				posState = position & _posStateMask;
				uint curAnd1Price = curPrice + _isMatch[(state.Index << 4) + posState].GetPrice0() + _literalEncoder.GetSubCoder(position, _matchFinder.GetIndexByte(-2)).GetPrice(!state.IsCharState(), indexByte2, indexByte);
				Optimal nextOptimum = _optimum[cur + 1];
				bool nextIsChar = false;
				if (curAnd1Price < nextOptimum.Price)
				{
					nextOptimum.Price = curAnd1Price;
					nextOptimum.PosPrev = cur;
					nextOptimum.MakeAsChar();
					nextIsChar = true;
				}
				matchPrice = curPrice + _isMatch[(state.Index << 4) + posState].GetPrice1();
				repMatchPrice = matchPrice + _isRep[state.Index].GetPrice1();
				if (indexByte2 == indexByte && (nextOptimum.PosPrev >= cur || nextOptimum.BackPrev != 0))
				{
					uint shortRepPrice2 = repMatchPrice + GetRepLen1Price(state, posState);
					if (shortRepPrice2 <= nextOptimum.Price)
					{
						nextOptimum.Price = shortRepPrice2;
						nextOptimum.PosPrev = cur;
						nextOptimum.MakeAsShortRep();
						nextIsChar = true;
					}
				}
				uint numAvailableBytesFull = _matchFinder.GetNumAvailableBytes() + 1;
				numAvailableBytesFull = Math.Min(4095 - cur, numAvailableBytesFull);
				numAvailableBytes = numAvailableBytesFull;
				if (numAvailableBytes < 2)
				{
					continue;
				}
				if (numAvailableBytes > _numFastBytes)
				{
					numAvailableBytes = _numFastBytes;
				}
				if (!nextIsChar && indexByte2 != indexByte)
				{
					uint limit = Math.Min(numAvailableBytesFull - 1, _numFastBytes);
					uint matchLen = _matchFinder.GetMatchLen(0, reps[0], limit);
					if (matchLen >= 2)
					{
						Base.State state2 = state;
						state2.UpdateChar();
						uint posStateNext = (position + 1) & _posStateMask;
						uint nextRepMatchPrice = curAnd1Price + _isMatch[(state2.Index << 4) + posStateNext].GetPrice1() + _isRep[state2.Index].GetPrice1();
						uint offset = cur + 1 + matchLen;
						while (lenEnd < offset)
						{
							_optimum[++lenEnd].Price = kIfinityPrice;
						}
						uint curAndLenPrice = nextRepMatchPrice + GetRepPrice(0u, matchLen, state2, posStateNext);
						Optimal optimal4 = _optimum[offset];
						if (curAndLenPrice < optimal4.Price)
						{
							optimal4.Price = curAndLenPrice;
							optimal4.PosPrev = cur + 1;
							optimal4.BackPrev = 0u;
							optimal4.Prev1IsChar = true;
							optimal4.Prev2 = false;
						}
					}
				}
				uint startLen = 2u;
				for (uint repIndex = 0u; repIndex < 4; repIndex++)
				{
					uint lenTest = _matchFinder.GetMatchLen(-1, reps[repIndex], numAvailableBytes);
					if (lenTest < 2)
					{
						continue;
					}
					uint lenTestTemp = lenTest;
					do
					{
						while (lenEnd < cur + lenTest)
						{
							_optimum[++lenEnd].Price = kIfinityPrice;
						}
						uint curAndLenPrice2 = repMatchPrice + GetRepPrice(repIndex, lenTest, state, posState);
						Optimal optimal5 = _optimum[cur + lenTest];
						if (curAndLenPrice2 < optimal5.Price)
						{
							optimal5.Price = curAndLenPrice2;
							optimal5.PosPrev = cur;
							optimal5.BackPrev = repIndex;
							optimal5.Prev1IsChar = false;
						}
					}
					while (--lenTest >= 2);
					lenTest = lenTestTemp;
					if (repIndex == 0)
					{
						startLen = lenTest + 1;
					}
					if (lenTest >= numAvailableBytesFull)
					{
						continue;
					}
					uint limit2 = Math.Min(numAvailableBytesFull - 1 - lenTest, _numFastBytes);
					uint matchLen2 = _matchFinder.GetMatchLen((int)lenTest, reps[repIndex], limit2);
					if (matchLen2 >= 2)
					{
						Base.State state3 = state;
						state3.UpdateRep();
						uint posStateNext2 = (position + lenTest) & _posStateMask;
						uint curAndLenCharPrice = repMatchPrice + GetRepPrice(repIndex, lenTest, state, posState) + _isMatch[(state3.Index << 4) + posStateNext2].GetPrice0() + _literalEncoder.GetSubCoder(position + lenTest, _matchFinder.GetIndexByte((int)lenTest - 1 - 1)).GetPrice(true, _matchFinder.GetIndexByte((int)lenTest - 1 - (int)(reps[repIndex] + 1)), _matchFinder.GetIndexByte((int)lenTest - 1));
						state3.UpdateChar();
						posStateNext2 = (position + lenTest + 1) & _posStateMask;
						uint nextMatchPrice = curAndLenCharPrice + _isMatch[(state3.Index << 4) + posStateNext2].GetPrice1();
						uint nextRepMatchPrice2 = nextMatchPrice + _isRep[state3.Index].GetPrice1();
						uint offset2 = lenTest + 1 + matchLen2;
						while (lenEnd < cur + offset2)
						{
							_optimum[++lenEnd].Price = kIfinityPrice;
						}
						uint curAndLenPrice3 = nextRepMatchPrice2 + GetRepPrice(0u, matchLen2, state3, posStateNext2);
						Optimal optimal6 = _optimum[cur + offset2];
						if (curAndLenPrice3 < optimal6.Price)
						{
							optimal6.Price = curAndLenPrice3;
							optimal6.PosPrev = cur + lenTest + 1;
							optimal6.BackPrev = 0u;
							optimal6.Prev1IsChar = true;
							optimal6.Prev2 = true;
							optimal6.PosPrev2 = cur;
							optimal6.BackPrev2 = repIndex;
						}
					}
				}
				if (newLen > numAvailableBytes)
				{
					newLen = numAvailableBytes;
					numDistancePairs = 0u;
					while (newLen > _matchDistances[numDistancePairs])
					{
						numDistancePairs += 2;
					}
					_matchDistances[numDistancePairs] = newLen;
					numDistancePairs += 2;
				}
				if (newLen < startLen)
				{
					continue;
				}
				normalMatchPrice = matchPrice + _isRep[state.Index].GetPrice0();
				while (lenEnd < cur + newLen)
				{
					_optimum[++lenEnd].Price = kIfinityPrice;
				}
				uint offs2 = 0u;
				while (startLen > _matchDistances[offs2])
				{
					offs2 += 2;
				}
				uint lenTest2 = startLen;
				while (true)
				{
					uint curBack = _matchDistances[offs2 + 1];
					uint curAndLenPrice4 = normalMatchPrice + GetPosLenPrice(curBack, lenTest2, posState);
					Optimal optimal7 = _optimum[cur + lenTest2];
					if (curAndLenPrice4 < optimal7.Price)
					{
						optimal7.Price = curAndLenPrice4;
						optimal7.PosPrev = cur;
						optimal7.BackPrev = curBack + 4;
						optimal7.Prev1IsChar = false;
					}
					if (lenTest2 == _matchDistances[offs2])
					{
						if (lenTest2 < numAvailableBytesFull)
						{
							uint limit3 = Math.Min(numAvailableBytesFull - 1 - lenTest2, _numFastBytes);
							uint matchLen3 = _matchFinder.GetMatchLen((int)lenTest2, curBack, limit3);
							if (matchLen3 >= 2)
							{
								Base.State state4 = state;
								state4.UpdateMatch();
								uint posStateNext3 = (position + lenTest2) & _posStateMask;
								uint curAndLenCharPrice2 = curAndLenPrice4 + _isMatch[(state4.Index << 4) + posStateNext3].GetPrice0() + _literalEncoder.GetSubCoder(position + lenTest2, _matchFinder.GetIndexByte((int)lenTest2 - 1 - 1)).GetPrice(true, _matchFinder.GetIndexByte((int)lenTest2 - (int)(curBack + 1) - 1), _matchFinder.GetIndexByte((int)lenTest2 - 1));
								state4.UpdateChar();
								posStateNext3 = (position + lenTest2 + 1) & _posStateMask;
								uint nextMatchPrice2 = curAndLenCharPrice2 + _isMatch[(state4.Index << 4) + posStateNext3].GetPrice1();
								uint nextRepMatchPrice3 = nextMatchPrice2 + _isRep[state4.Index].GetPrice1();
								uint offset3 = lenTest2 + 1 + matchLen3;
								while (lenEnd < cur + offset3)
								{
									_optimum[++lenEnd].Price = kIfinityPrice;
								}
								curAndLenPrice4 = nextRepMatchPrice3 + GetRepPrice(0u, matchLen3, state4, posStateNext3);
								optimal7 = _optimum[cur + offset3];
								if (curAndLenPrice4 < optimal7.Price)
								{
									optimal7.Price = curAndLenPrice4;
									optimal7.PosPrev = cur + lenTest2 + 1;
									optimal7.BackPrev = 0u;
									optimal7.Prev1IsChar = true;
									optimal7.Prev2 = true;
									optimal7.PosPrev2 = cur;
									optimal7.BackPrev2 = curBack + 4;
								}
							}
						}
						offs2 += 2;
						if (offs2 == numDistancePairs)
						{
							break;
						}
					}
					lenTest2++;
				}
			}
		}

		private bool ChangePair(uint smallDist, uint bigDist)
		{
			return smallDist < 33554432 && bigDist >= smallDist << 7;
		}

		private void WriteEndMarker(uint posState)
		{
			if (_writeEndMark)
			{
				_isMatch[(_state.Index << 4) + posState].Encode(_rangeEncoder, 1u);
				_isRep[_state.Index].Encode(_rangeEncoder, 0u);
				_state.UpdateMatch();
				uint num = 2u;
				_lenEncoder.Encode(_rangeEncoder, num - 2, posState);
				uint symbol = 63u;
				uint lenToPosState = Base.GetLenToPosState(num);
				_posSlotEncoder[lenToPosState].Encode(_rangeEncoder, symbol);
				int num2 = 30;
				uint num3 = (1u << num2) - 1;
				_rangeEncoder.EncodeDirectBits(num3 >> 4, num2 - 4);
				_posAlignEncoder.ReverseEncode(_rangeEncoder, num3 & 0xF);
			}
		}

		private void Flush(uint nowPos)
		{
			ReleaseMFStream();
			WriteEndMarker(nowPos & _posStateMask);
			_rangeEncoder.FlushData();
			_rangeEncoder.FlushStream();
		}

		public void CodeOneBlock(out long inSize, out long outSize, out bool finished)
		{
			inSize = 0L;
			outSize = 0L;
			finished = true;
			if (_inStream != null)
			{
				_matchFinder.SetStream(_inStream);
				_matchFinder.Init();
				_needReleaseMFStream = true;
				_inStream = null;
				if (_trainSize != 0)
				{
					_matchFinder.Skip(_trainSize);
				}
			}
			if (_finished)
			{
				return;
			}
			_finished = true;
			long num = nowPos64;
			if (nowPos64 == 0)
			{
				if (_matchFinder.GetNumAvailableBytes() == 0)
				{
					Flush((uint)nowPos64);
					return;
				}
				uint lenRes;
				uint numDistancePairs;
				ReadMatchDistances(out lenRes, out numDistancePairs);
				uint num2 = (uint)(int)nowPos64 & _posStateMask;
				_isMatch[(_state.Index << 4) + num2].Encode(_rangeEncoder, 0u);
				_state.UpdateChar();
				byte indexByte = _matchFinder.GetIndexByte((int)(0 - _additionalOffset));
				_literalEncoder.GetSubCoder((uint)nowPos64, _previousByte).Encode(_rangeEncoder, indexByte);
				_previousByte = indexByte;
				_additionalOffset--;
				nowPos64++;
			}
			if (_matchFinder.GetNumAvailableBytes() == 0)
			{
				Flush((uint)nowPos64);
				return;
			}
			while (true)
			{
				uint back;
				uint optimum = GetOptimum((uint)nowPos64, out back);
				uint num3 = (uint)(int)nowPos64 & _posStateMask;
				uint num4 = (_state.Index << 4) + num3;
				if (optimum == 1 && back == uint.MaxValue)
				{
					_isMatch[num4].Encode(_rangeEncoder, 0u);
					byte indexByte2 = _matchFinder.GetIndexByte((int)(0 - _additionalOffset));
					LiteralEncoder.Encoder2 subCoder = _literalEncoder.GetSubCoder((uint)nowPos64, _previousByte);
					if (!_state.IsCharState())
					{
						byte indexByte3 = _matchFinder.GetIndexByte((int)(0 - _repDistances[0] - 1 - _additionalOffset));
						subCoder.EncodeMatched(_rangeEncoder, indexByte3, indexByte2);
					}
					else
					{
						subCoder.Encode(_rangeEncoder, indexByte2);
					}
					_previousByte = indexByte2;
					_state.UpdateChar();
				}
				else
				{
					_isMatch[num4].Encode(_rangeEncoder, 1u);
					if (back < 4)
					{
						_isRep[_state.Index].Encode(_rangeEncoder, 1u);
						if (back == 0)
						{
							_isRepG0[_state.Index].Encode(_rangeEncoder, 0u);
							if (optimum == 1)
							{
								_isRep0Long[num4].Encode(_rangeEncoder, 0u);
							}
							else
							{
								_isRep0Long[num4].Encode(_rangeEncoder, 1u);
							}
						}
						else
						{
							_isRepG0[_state.Index].Encode(_rangeEncoder, 1u);
							if (back == 1)
							{
								_isRepG1[_state.Index].Encode(_rangeEncoder, 0u);
							}
							else
							{
								_isRepG1[_state.Index].Encode(_rangeEncoder, 1u);
								_isRepG2[_state.Index].Encode(_rangeEncoder, back - 2);
							}
						}
						if (optimum == 1)
						{
							_state.UpdateShortRep();
						}
						else
						{
							_repMatchLenEncoder.Encode(_rangeEncoder, optimum - 2, num3);
							_state.UpdateRep();
						}
						uint num5 = _repDistances[back];
						if (back != 0)
						{
							for (uint num6 = back; num6 >= 1; num6--)
							{
								_repDistances[num6] = _repDistances[num6 - 1];
							}
							_repDistances[0] = num5;
						}
					}
					else
					{
						_isRep[_state.Index].Encode(_rangeEncoder, 0u);
						_state.UpdateMatch();
						_lenEncoder.Encode(_rangeEncoder, optimum - 2, num3);
						back -= 4;
						uint posSlot = GetPosSlot(back);
						uint lenToPosState = Base.GetLenToPosState(optimum);
						_posSlotEncoder[lenToPosState].Encode(_rangeEncoder, posSlot);
						if (posSlot >= 4)
						{
							int num7 = (int)((posSlot >> 1) - 1);
							uint num8 = (2 | (posSlot & 1)) << num7;
							uint num9 = back - num8;
							if (posSlot < 14)
							{
								BitTreeEncoder.ReverseEncode(_posEncoders, num8 - posSlot - 1, _rangeEncoder, num7, num9);
							}
							else
							{
								_rangeEncoder.EncodeDirectBits(num9 >> 4, num7 - 4);
								_posAlignEncoder.ReverseEncode(_rangeEncoder, num9 & 0xF);
								_alignPriceCount++;
							}
						}
						uint num10 = back;
						for (uint num11 = 3u; num11 >= 1; num11--)
						{
							_repDistances[num11] = _repDistances[num11 - 1];
						}
						_repDistances[0] = num10;
						_matchPriceCount++;
					}
					_previousByte = _matchFinder.GetIndexByte((int)(optimum - 1 - _additionalOffset));
				}
				_additionalOffset -= optimum;
				nowPos64 += optimum;
				if (_additionalOffset != 0)
				{
					continue;
				}
				if (_matchPriceCount >= 128)
				{
					FillDistancesPrices();
				}
				if (_alignPriceCount >= 16)
				{
					FillAlignPrices();
				}
				inSize = nowPos64;
				outSize = _rangeEncoder.GetProcessedSizeAdd();
				if (_matchFinder.GetNumAvailableBytes() == 0)
				{
					Flush((uint)nowPos64);
					break;
				}
				if (nowPos64 - num >= 4096)
				{
					_finished = false;
					finished = false;
					break;
				}
			}
		}

		private void ReleaseMFStream()
		{
			if (_matchFinder != null && _needReleaseMFStream)
			{
				_matchFinder.ReleaseStream();
				_needReleaseMFStream = false;
			}
		}

		private void SetOutStream(Stream outStream)
		{
			_rangeEncoder.SetStream(outStream);
		}

		private void ReleaseOutStream()
		{
			_rangeEncoder.ReleaseStream();
		}

		private void ReleaseStreams()
		{
			ReleaseMFStream();
			ReleaseOutStream();
		}

		private void SetStreams(Stream inStream, Stream outStream, long inSize, long outSize)
		{
			_inStream = inStream;
			_finished = false;
			Create();
			SetOutStream(outStream);
			Init();
			FillDistancesPrices();
			FillAlignPrices();
			_lenEncoder.SetTableSize(_numFastBytes + 1 - 2);
			_lenEncoder.UpdateTables((uint)(1 << _posStateBits));
			_repMatchLenEncoder.SetTableSize(_numFastBytes + 1 - 2);
			_repMatchLenEncoder.UpdateTables((uint)(1 << _posStateBits));
			nowPos64 = 0L;
		}

		public void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress)
		{
			_needReleaseMFStream = false;
			try
			{
				SetStreams(inStream, outStream, inSize, outSize);
				while (true)
				{
					long processedInSize;
					long processedOutSize;
					bool finished;
					CodeOneBlock(out processedInSize, out processedOutSize, out finished);
					if (finished)
					{
						break;
					}
					if (progress != null)
					{
						progress.SetProgress(processedInSize, processedOutSize);
					}
				}
			}
			finally
			{
				ReleaseStreams();
			}
		}

		public void WriteCoderProperties(Stream outStream)
		{
			properties[0] = (byte)((_posStateBits * 5 + _numLiteralPosStateBits) * 9 + _numLiteralContextBits);
			for (int i = 0; i < 4; i++)
			{
				properties[1 + i] = (byte)((_dictionarySize >> 8 * i) & 0xFFu);
			}
			outStream.Write(properties, 0, 5);
		}

		private void FillDistancesPrices()
		{
			for (uint num = 4u; num < 128; num++)
			{
				uint posSlot = GetPosSlot(num);
				int num2 = (int)((posSlot >> 1) - 1);
				uint num3 = (2 | (posSlot & 1)) << num2;
				tempPrices[num] = BitTreeEncoder.ReverseGetPrice(_posEncoders, num3 - posSlot - 1, num2, num - num3);
			}
			for (uint num4 = 0u; num4 < 4; num4++)
			{
				BitTreeEncoder bitTreeEncoder = _posSlotEncoder[num4];
				uint num5 = num4 << 6;
				uint num6;
				for (num6 = 0u; num6 < _distTableSize; num6++)
				{
					_posSlotPrices[num5 + num6] = bitTreeEncoder.GetPrice(num6);
				}
				for (num6 = 14u; num6 < _distTableSize; num6++)
				{
					_posSlotPrices[num5 + num6] += (uint)(((int)(num6 >> 1) - 1 - 4) << 6);
				}
				uint num7 = num4 * 128;
				uint num8;
				for (num8 = 0u; num8 < 4; num8++)
				{
					_distancesPrices[num7 + num8] = _posSlotPrices[num5 + num8];
				}
				for (; num8 < 128; num8++)
				{
					_distancesPrices[num7 + num8] = _posSlotPrices[num5 + GetPosSlot(num8)] + tempPrices[num8];
				}
			}
			_matchPriceCount = 0u;
		}

		private void FillAlignPrices()
		{
			for (uint num = 0u; num < 16; num++)
			{
				_alignPrices[num] = _posAlignEncoder.ReverseGetPrice(num);
			}
			_alignPriceCount = 0u;
		}

		private static int FindMatchFinder(string s)
		{
			for (int i = 0; i < kMatchFinderIDs.Length; i++)
			{
				if (s == kMatchFinderIDs[i])
				{
					return i;
				}
			}
			return -1;
		}

		public void SetCoderProperties(CoderPropID[] propIDs, object[] properties)
		{
			for (uint num = 0u; num < properties.Length; num++)
			{
				object obj = properties[num];
				switch (propIDs[num])
				{
				case CoderPropID.NumFastBytes:
				{
					if (!(obj is int))
					{
						throw new InvalidParamException();
					}
					int num2 = (int)obj;
					if (num2 < 5 || num2 > 273)
					{
						throw new InvalidParamException();
					}
					_numFastBytes = (uint)num2;
					break;
				}
				case CoderPropID.Algorithm:
					break;
				case CoderPropID.MatchFinder:
				{
					if (!(obj is string))
					{
						throw new InvalidParamException();
					}
					EMatchFinderType matchFinderType = _matchFinderType;
					int num3 = FindMatchFinder(((string)obj).ToUpper());
					if (num3 < 0)
					{
						throw new InvalidParamException();
					}
					_matchFinderType = (EMatchFinderType)num3;
					if (_matchFinder != null && matchFinderType != _matchFinderType)
					{
						_dictionarySizePrev = uint.MaxValue;
						_matchFinder = null;
					}
					break;
				}
				case CoderPropID.DictionarySize:
				{
					if (!(obj is int))
					{
						throw new InvalidParamException();
					}
					int num4 = (int)obj;
					if (num4 < 1 || num4 > 1073741824)
					{
						throw new InvalidParamException();
					}
					_dictionarySize = (uint)num4;
					int i;
					for (i = 0; i < 30 && num4 > (1 << i); i++)
					{
					}
					_distTableSize = (uint)(i * 2);
					break;
				}
				case CoderPropID.PosStateBits:
				{
					if (!(obj is int))
					{
						throw new InvalidParamException();
					}
					int num5 = (int)obj;
					if (num5 < 0 || num5 > 4)
					{
						throw new InvalidParamException();
					}
					_posStateBits = num5;
					_posStateMask = (1u << _posStateBits) - 1;
					break;
				}
				case CoderPropID.LitPosBits:
				{
					if (!(obj is int))
					{
						throw new InvalidParamException();
					}
					int num6 = (int)obj;
					if (num6 < 0 || num6 > 4)
					{
						throw new InvalidParamException();
					}
					_numLiteralPosStateBits = num6;
					break;
				}
				case CoderPropID.LitContextBits:
				{
					if (!(obj is int))
					{
						throw new InvalidParamException();
					}
					int num7 = (int)obj;
					if (num7 < 0 || num7 > 8)
					{
						throw new InvalidParamException();
					}
					_numLiteralContextBits = num7;
					break;
				}
				case CoderPropID.EndMarker:
					if (!(obj is bool))
					{
						throw new InvalidParamException();
					}
					SetWriteEndMarkerMode((bool)obj);
					break;
				default:
					throw new InvalidParamException();
				}
			}
		}

		public void SetTrainSize(uint trainSize)
		{
			_trainSize = trainSize;
		}
	}
}
