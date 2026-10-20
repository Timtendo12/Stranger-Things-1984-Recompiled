using System;

internal struct Fix8 : IEquatable<Fix8>, IComparable<Fix8>
{
	private readonly sbyte m_rawValue;

	public static readonly Fix8 One;

	public static readonly Fix8 MinValue;

	public static readonly Fix8 MaxValue;

	public const decimal Precision = 0.0625m;

	public static readonly Fix8 Pi;

	public static readonly Fix8 PiOver2;

	public static readonly Fix8 PiInv;

	public static readonly Fix8 PiOver2Inv;

	public static Fix8 Abs(Fix8 value)
	{
		return default;
	}

	public static Fix8 Floor(Fix8 value)
	{
		return default;
	}

	public static Fix8 Ceiling(Fix8 value)
	{
		return default;
	}

	public static Fix8 Round(Fix8 value)
	{
		return default;
	}

	public static int Sign(Fix8 value)
	{
		return 0;
	}

	public static Fix8 FromRaw(sbyte rawValue)
	{
		return default;
	}

	public static explicit operator decimal(Fix8 value)
	{
		return 0m;
	}

	public static explicit operator Fix8(decimal value)
	{
		return default;
	}

	public static Fix8 operator +(Fix8 x, Fix8 y)
	{
		return default;
	}

	public static Fix8 operator -(Fix8 x, Fix8 y)
	{
		return default;
	}

	private static sbyte AddOverflowHelper(sbyte x, sbyte y, ref bool overflow)
	{
		return 0;
	}

	public static Fix8 operator *(Fix8 x, Fix8 y)
	{
		return default;
	}

	private static int Clz(byte x)
	{
		return 0;
	}

	public static Fix8 operator /(Fix8 x, Fix8 y)
	{
		return default;
	}

	public static Fix8 Sqrt(Fix8 x)
	{
		return default;
	}

	public static Fix8 Sin(Fix8 x)
	{
		return default;
	}

	public static Fix8 operator %(Fix8 x, Fix8 y)
	{
		return default;
	}

	public bool Equals(Fix8 other)
	{
		return false;
	}

	public int CompareTo(Fix8 other)
	{
		return 0;
	}

	public override int GetHashCode()
	{
		return 0;
	}

	public override bool Equals(object obj)
	{
		return false;
	}

	public override string ToString()
	{
		return null;
	}

	private Fix8(sbyte value)
	{
		m_rawValue = 0;
	}
}
