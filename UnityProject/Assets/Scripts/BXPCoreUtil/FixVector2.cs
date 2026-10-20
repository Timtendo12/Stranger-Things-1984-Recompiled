using System;

public struct FixVector2 : IEquatable<FixVector2>, IFormattable
{
	public Fix64 x;

	public Fix64 y;

	public static readonly FixVector2 down;

	public static readonly FixVector2 left;

	public static readonly FixVector2 one;

	public static readonly FixVector2 right;

	public static readonly FixVector2 up;

	public static readonly FixVector2 zero;

	public static Fix64 MinNormalizeMagnitude;

	public Fix64 magnitude => default;

	public Fix64 sqrMagnitude => default;

	public FixVector2 normalized => default;

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

	public FixVector2(Fix64 vx, Fix64 vy)
	{
		x = default;
		y = default;
	}

	public static FixVector2 Make(float x, float y)
	{
		return default;
	}

	public static FixVector2 Make(double x, double y)
	{
		return default;
	}

	public void SetFloat(float xFloat, float yFloat)
	{
	}

	public void SetDouble(double xDouble, double yDouble)
	{
	}

	public override bool Equals(object obj)
	{
		return false;
	}

	public bool Equals(FixVector2 other)
	{
		return false;
	}

	public void Set(Fix64 vx, Fix64 vy)
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

	public static FixVector2 operator +(FixVector2 a, FixVector2 b)
	{
		return default;
	}

	public static FixVector2 operator -(FixVector2 a, FixVector2 b)
	{
		return default;
	}

	public static FixVector2 operator -(FixVector2 a)
	{
		return default;
	}

	public static FixVector2 operator *(Fix64 k, FixVector2 v)
	{
		return default;
	}

	public static FixVector2 operator *(FixVector2 v, Fix64 k)
	{
		return default;
	}

	public static FixVector2 operator /(FixVector2 v, Fix64 k)
	{
		return default;
	}

	public static bool operator ==(FixVector2 a, FixVector2 b)
	{
		return false;
	}

	public static bool operator !=(FixVector2 a, FixVector2 b)
	{
		return false;
	}

	public static Fix64 Distance(FixVector2 a, FixVector2 b)
	{
		return default;
	}

	public static Fix64 Dot(FixVector2 a, FixVector2 b)
	{
		return default;
	}

	public void Normalize()
	{
	}

	public static FixVector2 Normalize(FixVector2 value)
	{
		return default;
	}

	public static Fix64 SqrMagnitude(FixVector2 a)
	{
		return default;
	}

	public static Fix64 AngleRadians(FixVector2 from, FixVector2 to)
	{
		return default;
	}

	public static Fix64 Angle(FixVector2 from, FixVector2 to)
	{
		return default;
	}

	public static FixVector2 ClampMagnitude(FixVector2 vector, Fix64 maxLength)
	{
		return default;
	}

	public static FixVector2 Lerp(FixVector2 from, FixVector2 to, Fix64 t)
	{
		return default;
	}

	public static FixVector2 LerpUnclamped(FixVector2 from, FixVector2 to, Fix64 t)
	{
		return default;
	}

	public static FixVector2 Max(FixVector2 a, FixVector2 b)
	{
		return default;
	}

	public static FixVector2 Min(FixVector2 a, FixVector2 b)
	{
		return default;
	}

	public static FixVector2 MoveTowards(FixVector2 current, FixVector2 target, Fix64 maxDistanceDelta)
	{
		return default;
	}

	public static FixVector2 Project(FixVector2 vector, FixVector2 onNormal)
	{
		return default;
	}

	public static FixVector2 ProjectOntoNormalized(FixVector2 vector, FixVector2 onNormalNormalized)
	{
		return default;
	}

	public static FixVector2 Reflect(FixVector2 inDirection, FixVector2 inNormal)
	{
		return default;
	}

	public static FixVector2 Scale(FixVector2 a, FixVector2 b)
	{
		return default;
	}

	public static FixVector2 Slerp(FixVector2 from, FixVector2 to, Fix64 t)
	{
		return default;
	}

	public static FixVector2 RotateTowards(FixVector2 from, FixVector2 to, Fix64 maxRadiansDelta, Fix64 maxMagnitudeDelta)
	{
		return default;
	}

	public static FixVector2 SmoothDamp(FixVector2 current, FixVector2 target, ref FixVector2 currentVelocity, Fix64 smoothTime, Fix64 maxSpeed, Fix64 deltaTime)
	{
		return default;
	}

	public static FixVector2 Exclude(FixVector2 excludeThis, FixVector2 fromThat)
	{
		return default;
	}
}
