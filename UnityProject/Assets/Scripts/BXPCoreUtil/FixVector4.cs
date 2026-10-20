using System;

public struct FixVector4 : IEquatable<FixVector4>, IFormattable
{
	public Fix64 x;

	public Fix64 y;

	public Fix64 z;

	public Fix64 w;

	public static readonly FixVector4 back;

	public static readonly FixVector4 down;

	public static readonly FixVector4 forward;

	public static readonly FixVector4 left;

	public static readonly FixVector4 one;

	public static readonly FixVector4 right;

	public static readonly FixVector4 up;

	public static readonly FixVector4 zero;

	public static Fix64 MinNormalizeMagnitude;

	public Fix64 magnitude => default;

	public Fix64 sqrMagnitude => default;

	public FixVector4 normalized => default;

	public Fix64 this[int index]
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public FixVector4(Fix64 vx, Fix64 vy, Fix64 vz, Fix64 vw)
	{
		x = default;
		y = default;
		z = default;
		w = default;
	}

	public FixVector4(Fix64 vx, Fix64 vy, Fix64 vz)
	{
		x = default;
		y = default;
		z = default;
		w = default;
	}

	public FixVector4(Fix64 vx, Fix64 vy)
	{
		x = default;
		y = default;
		z = default;
		w = default;
	}

	public static FixVector4 FromFloat(float x, float y, float z, float w)
	{
		return default;
	}

	public static FixVector4 FromDouble(double x, double y, double z, double w)
	{
		return default;
	}

	public void SetFloat(float xFloat, float yFloat, float zFloat, float wFloat)
	{
	}

	public void SetDouble(double xDouble, double yDouble, double zDouble, double wDouble)
	{
	}

	public override bool Equals(object obj)
	{
		return false;
	}

	public bool Equals(FixVector4 other)
	{
		return false;
	}

	public void Set(Fix64 vx, Fix64 vy, Fix64 vz, Fix64 vw)
	{
	}

	public override string ToString()
	{
		return null;
	}

	public string ToString(string format)
	{
		return null;
	}

	public string ToString(string format, IFormatProvider provider)
	{
		return null;
	}

	public override int GetHashCode()
	{
		return 0;
	}

	public static FixVector4 operator +(FixVector4 a, FixVector4 b)
	{
		return default;
	}

	public static FixVector4 operator -(FixVector4 a, FixVector4 b)
	{
		return default;
	}

	public static FixVector4 operator -(FixVector4 a)
	{
		return default;
	}

	public static FixVector4 operator *(Fix64 k, FixVector4 v)
	{
		return default;
	}

	public static FixVector4 operator *(FixVector4 v, Fix64 k)
	{
		return default;
	}

	public static FixVector4 operator /(FixVector4 v, Fix64 k)
	{
		return default;
	}

	public static bool operator ==(FixVector4 a, FixVector4 b)
	{
		return false;
	}

	public static bool operator !=(FixVector4 a, FixVector4 b)
	{
		return false;
	}

	public static Fix64 Distance(FixVector4 a, FixVector4 b)
	{
		return default;
	}

	public static Fix64 Dot(FixVector4 a, FixVector4 b)
	{
		return default;
	}

	public void Normalize()
	{
	}

	public static FixVector4 Normalize(FixVector4 value)
	{
		return default;
	}

	public static Fix64 SqrMagnitude(FixVector4 a)
	{
		return default;
	}

	public static Fix64 AngleRadians(FixVector4 from, FixVector4 to)
	{
		return default;
	}

	public static Fix64 Angle(FixVector4 from, FixVector4 to)
	{
		return default;
	}

	public static FixVector4 ClampMagnitude(FixVector4 vector, Fix64 maxLength)
	{
		return default;
	}

	public static FixVector4 Lerp(FixVector4 from, FixVector4 to, Fix64 t)
	{
		return default;
	}

	public static FixVector4 LerpUnclamped(FixVector4 from, FixVector4 to, Fix64 t)
	{
		return default;
	}

	public static FixVector4 Max(FixVector4 a, FixVector4 b)
	{
		return default;
	}

	public static FixVector4 Min(FixVector4 a, FixVector4 b)
	{
		return default;
	}

	public static FixVector4 MoveTowards(FixVector4 current, FixVector4 target, Fix64 maxDistanceDelta)
	{
		return default;
	}

	public static FixVector4 Scale(FixVector4 a, FixVector4 b)
	{
		return default;
	}
}
