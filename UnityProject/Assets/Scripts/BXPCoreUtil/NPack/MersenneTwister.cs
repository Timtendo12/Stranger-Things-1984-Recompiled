using System;

namespace NPack
{
	// Rewritten from libil2cpp.so 1.0.605 (NPack.MersenneTwister @ 0x18a0e00..0x18a1ae4, Ghidra vaddr). Standard MT19937 (NPack).
	public class MersenneTwister : Random
	{
		private const int N = 624;

		private const int M = 397;

		private const uint MatrixA = 2567483615u;

		private const uint UpperMask = 2147483648u;

		private const uint LowerMask = 2147483647u;

		private const uint TemperingMaskB = 2636928640u;

		private const uint TemperingMaskC = 4022730752u;

		private uint[] _mt;

		private short _mti;

		private static readonly uint[] _mag01 = new uint[2] { 0u, 2567483615u };

		private const double FiftyThreeBitsOf1s = 9007199254740991.0;

		private const double Inverse53BitsOf1s = 1.1102230246251568E-16;

		private const double OnePlus53BitsOf1s = 9007199254740992.0;

		private const double InverseOnePlus53BitsOf1s = 1.1102230246251565E-16;

		public MersenneTwister(int seed)
		{
			_mt = new uint[624];
			init((uint)seed);
		}

		public MersenneTwister()
			: this(new Random().Next())
		{
		}

		public MersenneTwister(uint[] initKey, short index = -1)
		{
			_mt = new uint[624];
			if (initKey == null)
			{
				throw new ArgumentNullException("initKey");
			}
			if (index >= 0)
			{
				_mt = new uint[624];
				Array.Copy(initKey, _mt, 624);
				_mti = index;
			}
			else
			{
				init(initKey);
			}
		}

		public virtual uint NextUInt32()
		{
			return GenerateUInt32();
		}

		public virtual uint NextUInt32(uint maxValue)
		{
			return (uint)((double)GenerateUInt32() / (4294967295.0 / (double)maxValue));
		}

		public virtual uint NextUInt32(uint minValue, uint maxValue)
		{
			if (minValue > maxValue || maxValue - minValue == 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return (uint)((double)GenerateUInt32() / (4294967295.0 / (double)(maxValue - minValue)) + (double)minValue);
		}

		public override int Next()
		{
			return Next(int.MaxValue);
		}

		public override int Next(int maxValue)
		{
			if (maxValue <= 1)
			{
				if (maxValue < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				return 0;
			}
			return (int)(NextDouble() * (double)maxValue);
		}

		public override int Next(int minValue, int maxValue)
		{
			if (minValue > maxValue)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (maxValue - minValue == 0)
			{
				return maxValue;
			}
			return Next(maxValue - minValue) + minValue;
		}

		public override void NextBytes(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException();
			}
			for (int i = 0; i < buffer.Length; i++)
			{
				buffer[i] = (byte)Next(256);
			}
		}

		public override double NextDouble()
		{
			return compute53BitRandom(0.0, InverseOnePlus53BitsOf1s);
		}

		public double NextDouble(bool includeOne)
		{
			if (includeOne)
			{
				return compute53BitRandom(0.0, Inverse53BitsOf1s);
			}
			return NextDouble();
		}

		public double NextDoublePositive()
		{
			return compute53BitRandom(0.5, Inverse53BitsOf1s);
		}

		public float NextSingle()
		{
			return (float)NextDouble();
		}

		public float NextSingle(bool includeOne)
		{
			return (float)NextDouble(includeOne);
		}

		public float NextSinglePositive()
		{
			return (float)NextDoublePositive();
		}

		protected uint GenerateUInt32()
		{
			uint y;
			if (_mti >= 624)
			{
				int kk = 0;
				for (; kk < 227; kk++)
				{
					y = (_mt[kk] & 0x80000000u) | (_mt[kk + 1] & 0x7FFFFFFFu);
					_mt[kk] = _mt[kk + 397] ^ (y >> 1) ^ _mag01[y & 1];
				}
				for (; kk < 623; kk++)
				{
					y = (_mt[kk] & 0x80000000u) | (_mt[kk + 1] & 0x7FFFFFFFu);
					_mt[kk] = _mt[kk + -227] ^ (y >> 1) ^ _mag01[y & 1];
				}
				y = (_mt[623] & 0x80000000u) | (_mt[0] & 0x7FFFFFFFu);
				_mt[623] = _mt[396] ^ (y >> 1) ^ _mag01[y & 1];
				_mti = 0;
			}
			y = _mt[_mti++];
			y ^= temperingShiftU(y);
			y ^= temperingShiftS(y) & 0x9D2C5680u;
			y ^= temperingShiftT(y) & 0xEFC60000u;
			return y ^ temperingShiftL(y);
		}

		private static uint temperingShiftU(uint y)
		{
			return y >> 11;
		}

		private static uint temperingShiftS(uint y)
		{
			return y << 7;
		}

		private static uint temperingShiftT(uint y)
		{
			return y << 15;
		}

		private static uint temperingShiftL(uint y)
		{
			return y >> 18;
		}

		public uint[] GetState()
		{
			return _mt;
		}

		public short GetIndex()
		{
			return _mti;
		}

		private void init(uint seed)
		{
			_mt[0] = seed;
			for (_mti = 1; _mti < 624; _mti++)
			{
				_mt[_mti] = 1812433253u * (_mt[_mti - 1] ^ (_mt[_mti - 1] >> 30)) + (uint)_mti;
			}
		}

		private void init(uint[] key)
		{
			init(19650218u);
			int i = 1;
			int j = 0;
			for (int k = ((key.Length > 624) ? key.Length : 624); k > 0; k--)
			{
				_mt[i] = (_mt[i] ^ ((_mt[i - 1] ^ (_mt[i - 1] >> 30)) * 1664525u)) + key[j] + (uint)j;
				i++;
				j++;
				if (i >= 624)
				{
					_mt[0] = _mt[623];
					i = 1;
				}
				if (j >= key.Length)
				{
					j = 0;
				}
			}
			for (int k = 623; k > 0; k--)
			{
				_mt[i] = (_mt[i] ^ ((_mt[i - 1] ^ (_mt[i - 1] >> 30)) * 1566083941u)) - (uint)i;
				i++;
				if (i >= 624)
				{
					_mt[0] = _mt[623];
					i = 1;
				}
			}
			_mt[0] = 2147483648u;
		}

		private double compute53BitRandom(double translate, double scale)
		{
			uint a = GenerateUInt32() >> 5;
			uint b = GenerateUInt32() >> 6;
			return ((double)a * 67108864.0 + (double)b + translate) * scale;
		}

		public void Save(FBinWriter writer)
		{
			writer.Write(_mt);
			writer.Write(_mti);
		}

		public bool Load(FBinReader reader)
		{
			_mt = reader.ReadUIntArray();
			_mti = reader.ReadShort();
			return true;
		}

		public static MersenneTwister CreateFromReader(FBinReader reader)
		{
			MersenneTwister mersenneTwister = new MersenneTwister(0);
			mersenneTwister.Load(reader);
			return mersenneTwister;
		}
	}
}
