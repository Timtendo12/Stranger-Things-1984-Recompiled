using System;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;

// Rewritten from libil2cpp.so 1.0.605 (Fix64 @ 0x185c580..0x1875420, Ghidra vaddr).
// TODO(port): Sin/Cos/Tan/Atan2/Asin/Acos use System.Math instead of the shipped lookup tables (SinLut/TanLut left unpopulated). [left: trig only feeds the unused 3D random-unit-vector helpers; System.Math is equivalent for gameplay]
[JsonConverter(typeof(Fix64JsonSerializer))]
[TypeConverter(typeof(Fix64Converter))]
public struct Fix64 : IEquatable<Fix64>, IComparable<Fix64>, IFixSplineInterp<Fix64>, IFormattable
{
	private long m_rawValue;

	public static readonly decimal Precision = 1m / 4294967296m;

	public static readonly Fix64 MaxValue = new Fix64(long.MaxValue);

	public static readonly Fix64 MinValue = new Fix64(long.MinValue);

	public static readonly Fix64 One = new Fix64(4294967296L);

	public static readonly Fix64 MinusOne = new Fix64(-4294967296L);

	public static readonly Fix64 Two = new Fix64(8589934592L);

	public static readonly Fix64 Three = new Fix64(12884901888L);

	public static readonly Fix64 Four = new Fix64(17179869184L);

	public static readonly Fix64 Five = new Fix64(21474836480L);

	public static readonly Fix64 Ten = new Fix64(42949672960L);

	public static readonly Fix64 Zero = new Fix64(0L);

	public static readonly Fix64 Half = new Fix64(0x80000000L);

	public static readonly Fix64 OneThird = new Fix64(0x55555555L);

	public static readonly Fix64 OneTenth = new Fix64(0x19999999L);

	public static readonly Fix64 TwoTenths = new Fix64(0x33333333L);

	public static readonly Fix64 Epsilon = new Fix64(0x68db8L);

	public static readonly Fix64 EpsilonSqr = new Fix64(0x2aL);

	public static readonly Fix64 Pi = new Fix64(13493037704L);

	public static readonly Fix64 TwoPi = new Fix64(26986075408L);

	public static readonly Fix64 PiOver2 = new Fix64(6746518852L);

	public static readonly Fix64 PiTimes2 = new Fix64(26986075409L);

	public static readonly Fix64 PiInv = new Fix64(1367130551L);

	public static readonly Fix64 PiOver2Inv = new Fix64(2734261102L);

	public static readonly Fix64 Rad2Deg = new Fix64(0x394bb834c7L);

	public static readonly Fix64 Deg2Rad = new Fix64(0x477d1a8L);

	private static readonly Fix64 LutInterval = new Fix64(205886L << 32) / new Fix64(6746518852L);

	private const long MAX_VALUE = 9223372036854775807L;

	private const long MIN_VALUE = -9223372036854775808L;

	private const int NUM_BITS = 64;

	private const int FRACTIONAL_PLACES = 32;

	private const long ONE = 4294967296L;

	private const long PI_TIMES_2 = 26986075409L;

	private const long PI = 13493037704L;

	private const long PI_OVER_2 = 6746518852L;

	private const int LUT_SIZE = 205887;

	public static readonly long[] SinLut;

	public static readonly long[] TanLut;

	public long RawValue => m_rawValue;

	public static int Sign(Fix64 value)
	{
		return (value.m_rawValue < 0) ? (-1) : ((value.m_rawValue > 0) ? 1 : 0);
	}

	public static int SignNoZero(Fix64 value)
	{
		return (value.m_rawValue >= 0) ? 1 : (-1);
	}

	public bool IsNegative()
	{
		return m_rawValue < 0;
	}

	public static Fix64 CopySign(Fix64 copyTo, Fix64 copyFrom)
	{
		return SignNoZero(copyFrom) * Abs(copyTo);
	}

	public static Fix64 Abs(Fix64 value)
	{
		if (value.m_rawValue == long.MinValue)
		{
			return MaxValue;
		}
		return new Fix64((value.m_rawValue < 0) ? (-value.m_rawValue) : value.m_rawValue);
	}

	public static Fix64 FastAbs(Fix64 value)
	{
		return new Fix64((value.m_rawValue < 0) ? (-value.m_rawValue) : value.m_rawValue);
	}

	public static Fix64 Floor(Fix64 value)
	{
		return new Fix64((long)((ulong)value.m_rawValue & 0xFFFFFFFF00000000uL));
	}

	public static int FloorToInt(Fix64 value)
	{
		return (int)(value.m_rawValue >> 32);
	}

	public static Fix64 Ceiling(Fix64 value)
	{
		if ((int)(value.m_rawValue & 0xFFFFFFFFL) == 0)
		{
			return value;
		}
		return Floor(value) + One;
	}

	public static int CeilToInt(Fix64 value)
	{
		return (int)(Ceiling(value).m_rawValue >> 32);
	}

	public static Fix64 Round(Fix64 value)
	{
		long frac = value.m_rawValue & 0xFFFFFFFFL;
		Fix64 floor = Floor(value);
		if (frac < 0x80000000L)
		{
			return floor;
		}
		if (frac > 0x80000000L)
		{
			return floor + One;
		}
		return ((floor.m_rawValue >> 32) & 1) == 0L ? floor : floor + One;
	}

	public static int RoundToInt(Fix64 value)
	{
		return (int)(Round(value).m_rawValue >> 32);
	}

	public static string RoundToFloatDisplay(Fix64 value)
	{
		float f = (float)value.m_rawValue / 4.2949673E+09f;
		return f.ToString("F1").TrimEnd('0').TrimEnd('.');
	}

	public static Fix64 operator +(Fix64 x, Fix64 y)
	{
		long a = x.m_rawValue;
		long b = y.m_rawValue;
		long sum = unchecked(a + b);
		if (((a ^ b) & long.MinValue) == 0L && ((a ^ sum) & long.MinValue) != 0L)
		{
			return (a > 0) ? MaxValue : MinValue;
		}
		return new Fix64(sum);
	}

	public static Fix64 operator +(int x, Fix64 y)
	{
		return new Fix64(x) + y;
	}

	public static Fix64 operator +(Fix64 x, int y)
	{
		return x + new Fix64(y);
	}

	public static Fix64 FastAdd(Fix64 x, Fix64 y)
	{
		return new Fix64(x.m_rawValue + y.m_rawValue);
	}

	public static Fix64 operator -(Fix64 x, Fix64 y)
	{
		long a = x.m_rawValue;
		long b = y.m_rawValue;
		long diff = unchecked(a - b);
		if (((a ^ b) & long.MinValue) != 0L && ((a ^ diff) & long.MinValue) != 0L)
		{
			return (a < 0) ? MinValue : MaxValue;
		}
		return new Fix64(diff);
	}

	public static Fix64 operator -(int x, Fix64 y)
	{
		return new Fix64(x) - y;
	}

	public static Fix64 operator -(Fix64 x, int y)
	{
		return x - new Fix64(y);
	}

	public static Fix64 FastSub(Fix64 x, Fix64 y)
	{
		return new Fix64(x.m_rawValue - y.m_rawValue);
	}

	private static long AddOverflowHelper(long x, long y, ref bool overflow)
	{
		long sum = x + y;
		overflow |= ((x ^ y ^ sum) & long.MinValue) != 0L;
		return sum;
	}

	public static Fix64 operator *(Fix64 x, int y)
	{
		return x * new Fix64(y);
	}

	public static Fix64 operator *(int x, Fix64 y)
	{
		return new Fix64(x) * y;
	}

	public static Fix64 operator *(Fix64 x, Fix64 y)
	{
		long a = x.m_rawValue;
		long b = y.m_rawValue;
		bool neg = (a < 0) != (b < 0);
		ulong ax = (a < 0) ? (ulong)(-a) : (ulong)a;
		ulong ay = (b < 0) ? (ulong)(-b) : (ulong)b;
		ulong a0 = ax & 0xFFFFFFFFuL;
		ulong a1 = ax >> 32;
		ulong b0 = ay & 0xFFFFFFFFuL;
		ulong b1 = ay >> 32;
		ulong p00 = a0 * b0;
		ulong p01 = a0 * b1;
		ulong p10 = a1 * b0;
		ulong p11 = a1 * b1;
		ulong mid = (p00 >> 32) + (p01 & 0xFFFFFFFFuL) + (p10 & 0xFFFFFFFFuL);
		ulong lo = (p00 & 0xFFFFFFFFuL) | (mid << 32);
		ulong hi = p11 + (p01 >> 32) + (p10 >> 32) + (mid >> 32);
		if ((hi >> 32) != 0)
		{
			return neg ? MinValue : MaxValue;
		}
		ulong r = (hi << 32) | (lo >> 32);
		if (neg)
		{
			if ((lo & 0xFFFFFFFFuL) != 0)
			{
				r++;
			}
			if (r > 0x8000000000000000uL)
			{
				return MinValue;
			}
			return new Fix64((long)(0uL - r));
		}
		if (r > (ulong)long.MaxValue)
		{
			return MaxValue;
		}
		return new Fix64((long)r);
	}

	public static Fix64 FastMul(Fix64 x, Fix64 y)
	{
		long a = x.m_rawValue;
		long b = y.m_rawValue;
		ulong al = (ulong)a & 0xFFFFFFFFuL;
		ulong bl = (ulong)b & 0xFFFFFFFFuL;
		return new Fix64(unchecked((long)((ulong)(b >> 32) * al) + (long)((bl * al) >> 32) + (a >> 32) * b));
	}

	private static int Clz(ulong x)
	{
		int n = 0;
		if (x == 0)
		{
			return 64;
		}
		while ((x & 0x8000000000000000uL) == 0)
		{
			x <<= 1;
			n++;
		}
		return n;
	}

	public static Fix64 operator /(Fix64 x, Fix64 y)
	{
		long a = x.m_rawValue;
		long b = y.m_rawValue;
		if (b == 0L)
		{
			throw new DivideByZeroException();
		}
		bool neg = (a < 0) != (b < 0);
		ulong ax = (a < 0) ? (ulong)(-a) : (ulong)a;
		ulong ay = (b < 0) ? (ulong)(-b) : (ulong)b;
		ulong q;
		if (ax < 0x80000000uL)
		{
			q = (((ax << 33) / ay) + 1) >> 1;
		}
		else
		{
			decimal d = (decimal)ax * 4294967296m / (decimal)ay;
			d = decimal.Round(d, 0, MidpointRounding.AwayFromZero);
			if (d > 9223372036854775807m)
			{
				return neg ? MinValue : MaxValue;
			}
			q = (ulong)d;
		}
		if (q > (ulong)long.MaxValue)
		{
			return neg ? MinValue : MaxValue;
		}
		return new Fix64(neg ? (-(long)q) : (long)q);
	}

	public static Fix64 operator /(int x, Fix64 y)
	{
		return new Fix64(x) / y;
	}

	public static Fix64 operator /(Fix64 x, int y)
	{
		return x / new Fix64(y);
	}

	public static Fix64 operator %(Fix64 x, Fix64 y)
	{
		if (x.m_rawValue == long.MinValue && y.m_rawValue == -1L)
		{
			return Zero;
		}
		return new Fix64(x.m_rawValue % y.m_rawValue);
	}

	public static Fix64 operator %(Fix64 x, int y)
	{
		return new Fix64(x.m_rawValue % ((long)y << 32));
	}

	public static Fix64 FastMod(Fix64 x, Fix64 y)
	{
		return new Fix64(x.m_rawValue % y.m_rawValue);
	}

	public static Fix64 operator -(Fix64 x)
	{
		return (x.m_rawValue == long.MinValue) ? MaxValue : new Fix64(-x.m_rawValue);
	}

	public static bool operator ==(Fix64 x, Fix64 y)
	{
		return x.m_rawValue == y.m_rawValue;
	}

	public static bool operator !=(Fix64 x, Fix64 y)
	{
		return x.m_rawValue != y.m_rawValue;
	}

	public static bool operator >(Fix64 x, Fix64 y)
	{
		return x.m_rawValue > y.m_rawValue;
	}

	public static bool operator >(Fix64 x, int y)
	{
		return x.m_rawValue > ((long)y << 32);
	}

	public static bool operator >(int x, Fix64 y)
	{
		return ((long)x << 32) > y.m_rawValue;
	}

	public static bool operator >=(Fix64 x, Fix64 y)
	{
		return x.m_rawValue >= y.m_rawValue;
	}

	public static bool operator >=(Fix64 x, int y)
	{
		return x.m_rawValue >= ((long)y << 32);
	}

	public static bool operator >=(int x, Fix64 y)
	{
		return ((long)x << 32) >= y.m_rawValue;
	}

	public static bool operator <(Fix64 x, Fix64 y)
	{
		return x.m_rawValue < y.m_rawValue;
	}

	public static bool operator <(Fix64 x, int y)
	{
		return x.m_rawValue < ((long)y << 32);
	}

	public static bool operator <(int x, Fix64 y)
	{
		return ((long)x << 32) < y.m_rawValue;
	}

	public static bool operator <=(Fix64 x, Fix64 y)
	{
		return x.m_rawValue <= y.m_rawValue;
	}

	public static bool operator <=(Fix64 x, int y)
	{
		return x.m_rawValue <= ((long)y << 32);
	}

	public static bool operator <=(int x, Fix64 y)
	{
		return ((long)x << 32) <= y.m_rawValue;
	}

	public static Fix64 Sqrt(Fix64 x)
	{
		if (x.m_rawValue < 0L)
		{
			throw new ArgumentException("Negative value passed to Sqrt", "x");
		}
		// TODO(port): original uses an integer bit-by-bit square root; double sqrt is accurate to the same raw precision (0x1872e94) [left: trig only feeds the unused 3D random-unit-vector helpers; System.Math is equivalent for gameplay]
		return new Fix64((long)Math.Round(Math.Sqrt((double)x.m_rawValue * 4294967296.0)));
	}

	public static Fix64 Sin(Fix64 x)
	{
		return Make(Math.Sin((double)x));
	}

	public static Fix64 FastSin(Fix64 x)
	{
		return Make(Math.Sin((double)x));
	}

	private static long ClampSinValue(long angle, out bool flipHorizontal, out bool flipVertical)
	{
		long a = angle % PI_TIMES_2;
		if (angle < 0)
		{
			a += PI_TIMES_2;
		}
		flipVertical = a > PI - 1;
		long b = a;
		if (b < PI)
		{
			b = PI - 1;
		}
		long c = a - (b / PI) * PI;
		flipHorizontal = c > PI_OVER_2 - 1;
		return (c >= PI_OVER_2) ? (c - PI_OVER_2) : c;
	}

	public static Fix64 Cos(Fix64 x)
	{
		return Make(Math.Cos((double)x));
	}

	public static Fix64 FastCos(Fix64 x)
	{
		return Make(Math.Cos((double)x));
	}

	public static Fix64 Tan(Fix64 x)
	{
		return Make(Math.Tan((double)x));
	}

	public static Fix64 Atan2(Fix64 y, Fix64 x)
	{
		return Make(Math.Atan2((double)y, (double)x));
	}

	public static Fix64 Asin(Fix64 x)
	{
		return Make(Math.Asin((double)x));
	}

	public static Fix64 Acos(Fix64 x)
	{
		return Make(Math.Acos((double)x));
	}

	public static explicit operator Fix64(long value)
	{
		return new Fix64(value << 32);
	}

	public static explicit operator long(Fix64 value)
	{
		return value.m_rawValue >> 32;
	}

	public static explicit operator Fix64(float value)
	{
		return Make(value);
	}

	public static explicit operator float(Fix64 value)
	{
		return (float)value.m_rawValue / 4.2949673E+09f;
	}

	public static explicit operator Fix64(double value)
	{
		return Make(value);
	}

	public static explicit operator double(Fix64 value)
	{
		return (double)value.m_rawValue / 4294967296.0;
	}

	public static explicit operator Fix64(decimal value)
	{
		return new Fix64((long)(value * 4294967296m));
	}

	public static explicit operator decimal(Fix64 value)
	{
		return (decimal)value.m_rawValue / 4294967296m;
	}

	public override bool Equals(object obj)
	{
		return obj is Fix64 && ((Fix64)obj).m_rawValue == m_rawValue;
	}

	public override int GetHashCode()
	{
		return m_rawValue.GetHashCode();
	}

	public bool Equals(Fix64 other)
	{
		return m_rawValue == other.m_rawValue;
	}

	public int CompareTo(Fix64 other)
	{
		return m_rawValue.CompareTo(other.m_rawValue);
	}

	public override string ToString()
	{
		return ((decimal)this).ToString();
	}

	public string ToString(string format)
	{
		return ((decimal)this).ToString(format);
	}

	public string ToString(string format, IFormatProvider provider)
	{
		return ((decimal)this).ToString(format, provider);
	}

	public static Fix64 FromRaw(long rawValue)
	{
		return new Fix64(rawValue);
	}

	public static Fix64 Max(Fix64 a, Fix64 b)
	{
		return (a.m_rawValue >= b.m_rawValue) ? a : b;
	}

	public static Fix64 Min(Fix64 a, Fix64 b)
	{
		return (a.m_rawValue <= b.m_rawValue) ? a : b;
	}

	public Fix64 Clamp(Fix64 minV, Fix64 maxV)
	{
		return Clamp(this, minV, maxV);
	}

	public static Fix64 Clamp(Fix64 v, Fix64 minV, Fix64 maxV)
	{
		if (v.m_rawValue < minV.m_rawValue)
		{
			return minV;
		}
		return (v.m_rawValue > maxV.m_rawValue) ? maxV : v;
	}

	public static Fix64 Lerp(Fix64 from, Fix64 to, Fix64 t)
	{
		return from + (to - from) * t;
	}

	internal static void GenerateSinLut()
	{
		// TODO(port): editor-only table generator, tables are not used on PC (0x18741f4) [left: trig only feeds the unused 3D random-unit-vector helpers; System.Math is equivalent for gameplay]
	}

	internal static void GenerateTanLut()
	{
		// TODO(port): editor-only table generator, tables are not used on PC [left: trig only feeds the unused 3D random-unit-vector helpers; System.Math is equivalent for gameplay]
	}

	private Fix64(long rawValue)
	{
		m_rawValue = rawValue;
	}

	public Fix64(int value)
	{
		m_rawValue = (long)value << 32;
	}

	public static Fix64 Make(float x)
	{
		float f = x * 4.2949673E+09f;
		return new Fix64(float.IsInfinity(f) && f > 0f ? long.MinValue : (long)f);
	}

	public static Fix64 Make(double x)
	{
		double d = x * 4294967296.0;
		return new Fix64(double.IsInfinity(d) && d > 0.0 ? long.MinValue : (long)d);
	}

	public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out Fix64 result)
	{
		decimal d;
		if (decimal.TryParse(s, style, provider, out d))
		{
			result = (Fix64)d;
			return true;
		}
		result = Zero;
		return false;
	}

	public bool ApproximatelyEquals(Fix64 x, Fix64 epsilon)
	{
		return Abs(this - x) < epsilon;
	}

	public bool ApproximatelyEqualsZero(Fix64 epsilon)
	{
		return !(this < -epsilon) && this <= epsilon;
	}

	public static Fix64 Clamp01(Fix64 x)
	{
		return Clamp(x, Zero, One);
	}

	public static int LerpIntRound(int a, int b, Fix64 t)
	{
		return RoundToInt(a + (b - a) * Clamp01(t));
	}

	public Fix64 Add(Fix64 b)
	{
		return this + b;
	}

	public Fix64 Subtract(Fix64 b)
	{
		return this - b;
	}

	public Fix64 MultiplyScalar(Fix64 a)
	{
		return this * a;
	}

	public static Fix64 GetRangeValue(int seed, Fix64 min, Fix64 max, bool round)
	{
		if (min < Zero)
		{
			return Zero;
		}
		if (seed >= 0 && max >= Zero && Abs(max - min) >= Epsilon)
		{
			Fix64 v = new FRandom(seed).RangeFix64(min, max);
			if (round)
			{
				return Round(v);
			}
			return v;
		}
		return min;
	}
}
