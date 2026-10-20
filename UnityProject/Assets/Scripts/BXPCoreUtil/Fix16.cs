using System;

public struct Fix16 : IEquatable<Fix16>, IComparable<Fix16>
{
	private readonly int m_rawValue;

	private static readonly Fix16[][] Fix16AtanCacheIndex;

	private static readonly Fix16[] Fix16AtanCacheValue;

	public static readonly Fix16 FourDivPi;

	public static readonly Fix16 FourDivPi2;

	public static readonly Fix16 X4CorrectionComponent;

	public static readonly Fix16 PiDiv4;

	public static readonly Fix16 ThreePiDiv4;

	public static readonly Fix16 MaxValue;

	public static readonly Fix16 MinValue;

	public static readonly Fix16 Overflow;

	public static readonly Fix16 Pi;

	public static readonly Fix16 E;

	public static readonly Fix16 One;

	public static readonly Fix16 Zero;

	public static readonly ushort[] SinLut;

	static Fix16()
	{
	}

	public static explicit operator Fix16(int a)
	{
		return default;
	}

	public static explicit operator float(Fix16 a)
	{
		return 0f;
	}

	public static explicit operator double(Fix16 a)
	{
		return 0.0;
	}

	public static explicit operator decimal(Fix16 a)
	{
		return 0m;
	}

	public static explicit operator int(Fix16 a)
	{
		return 0;
	}

	public static explicit operator Fix16(float a)
	{
		return default;
	}

	public static explicit operator Fix16(double a)
	{
		return default;
	}

	public static Fix16 Abs(Fix16 x)
	{
		return default;
	}

	public static Fix16 Floor(Fix16 x)
	{
		return default;
	}

	public static Fix16 Ceil(Fix16 x)
	{
		return default;
	}

	public static Fix16 Min(Fix16 x, Fix16 y)
	{
		return default;
	}

	public static Fix16 Max(Fix16 x, Fix16 y)
	{
		return default;
	}

	public static Fix16 Clamp(Fix16 x, Fix16 min, Fix16 max)
	{
		return default;
	}

	public static Fix16 operator +(Fix16 x, Fix16 y)
	{
		return default;
	}

	public static Fix16 operator -(Fix16 x, Fix16 y)
	{
		return default;
	}

	public static Fix16 SAdd(Fix16 a, Fix16 b)
	{
		return default;
	}

	public static Fix16 SSub(Fix16 a, Fix16 b)
	{
		return default;
	}

	public static Fix16 operator *(Fix16 x, Fix16 y)
	{
		return default;
	}

	public static Fix16 SMul(Fix16 a, Fix16 b)
	{
		return default;
	}

	private static byte Clz(uint x)
	{
		return 0;
	}

	public static Fix16 operator /(Fix16 x, Fix16 y)
	{
		return default;
	}

	public static Fix16 SDiv(Fix16 inArg0, Fix16 inArg1)
	{
		return default;
	}

	public static Fix16 Sqrt(Fix16 x)
	{
		return default;
	}

	public static Fix16 SinParabola(Fix16 inAngle)
	{
		return default;
	}

	public static Fix16 Sin(Fix16 inAngle)
	{
		return default;
	}

	public static Fix16 Cos(Fix16 inAngle)
	{
		return default;
	}

	public static Fix16 Tan(Fix16 inAngle)
	{
		return default;
	}

	public static Fix16 Asin(Fix16 x)
	{
		return default;
	}

	public static Fix16 Atan2(Fix16 inY, Fix16 inX)
	{
		return default;
	}

	public static Fix16 Atan(Fix16 x)
	{
		return default;
	}

	public static Fix16 Acos(Fix16 x)
	{
		return default;
	}

	public static Fix16 operator %(Fix16 x, Fix16 y)
	{
		return default;
	}

	public static Fix16 operator >>(Fix16 x, int shift)
	{
		return default;
	}

	public static Fix16 operator <<(Fix16 x, int shift)
	{
		return default;
	}

	public static Fix16 operator -(Fix16 x)
	{
		return default;
	}

	public static bool operator >(Fix16 x, Fix16 y)
	{
		return false;
	}

	public static bool operator <(Fix16 x, Fix16 y)
	{
		return false;
	}

	public static bool operator >=(Fix16 x, Fix16 y)
	{
		return false;
	}

	public static bool operator <=(Fix16 x, Fix16 y)
	{
		return false;
	}

	public static bool operator ==(Fix16 x, Fix16 y)
	{
		return false;
	}

	public static bool operator !=(Fix16 x, Fix16 y)
	{
		return false;
	}

	public static Fix16 operator ++(Fix16 x)
	{
		return default;
	}

	public static Fix16 operator --(Fix16 x)
	{
		return default;
	}

	public static Fix16 FromRaw(int i)
	{
		return default;
	}

	private Fix16(int rawValue)
	{
		m_rawValue = 0;
	}

	private Fix16(uint rawValue)
	{
		m_rawValue = 0;
	}

	public override string ToString()
	{
		return null;
	}

	public bool Equals(Fix16 other)
	{
		return false;
	}

	public int CompareTo(Fix16 other)
	{
		return 0;
	}

	public override bool Equals(object obj)
	{
		return false;
	}

	public override int GetHashCode()
	{
		return 0;
	}
}
