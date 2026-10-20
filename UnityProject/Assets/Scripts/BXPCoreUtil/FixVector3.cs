using System;

public struct FixVector3 : IEquatable<FixVector3>, IFixSplineInterp<FixVector3>, IFormattable
{
	public Fix64 x;

	public Fix64 y;

	public Fix64 z;

	public static readonly FixVector3 back;

	public static readonly FixVector3 down;

	public static readonly FixVector3 forward;

	public static readonly FixVector3 left;

	public static readonly FixVector3 one;

	public static readonly FixVector3 right;

	public static readonly FixVector3 up;

	public static readonly FixVector3 zero;

	public static readonly FixVector3 MinValue;

	public static readonly FixVector3 MaxValue;

	public static Fix64 MinNormalizeMagnitude;

	public Fix64 magnitude => default;

	public Fix64 sqrMagnitude => default;

	public Fix64 magnitudeXZ => default;

	public Fix64 sqrMagnitudeXZ => default;

	public FixVector3 normalized => default;

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

	public FixVector3(Fix64 vx, Fix64 vy, Fix64 vz)
	{
		x = default;
		y = default;
		z = default;
	}

	public FixVector3(Fix64 vx, Fix64 vy)
	{
		x = default;
		y = default;
		z = default;
	}

	public static FixVector3 Make(float x, float y, float z)
	{
		return default;
	}

	public static FixVector3 Make(double x, double y, double z)
	{
		return default;
	}

	public void SetFloat(float xFloat, float yFloat, float zFloat)
	{
	}

	public void SetDouble(double xDouble, double yDouble, double zDouble)
	{
	}

	public override bool Equals(object obj)
	{
		return false;
	}

	public bool Equals(FixVector3 other)
	{
		return false;
	}

	public FixVector3 NormalizedOrDefault(FixVector3 defaultValue)
	{
		return default;
	}

	public void Set(Fix64 vx, Fix64 vy, Fix64 vz)
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

	public static FixVector3 operator +(FixVector3 a, FixVector3 b)
	{
		return default;
	}

	public static FixVector3 operator -(FixVector3 a, FixVector3 b)
	{
		return default;
	}

	public static FixVector3 operator -(FixVector3 a)
	{
		return default;
	}

	public static FixVector3 operator *(Fix64 k, FixVector3 v)
	{
		return default;
	}

	public static FixVector3 operator *(FixVector3 v, Fix64 k)
	{
		return default;
	}

	public static FixVector3 operator /(FixVector3 v, Fix64 k)
	{
		return default;
	}

	public static FixVector3 operator /(FixVector3 v, int k)
	{
		return default;
	}

	public static bool operator ==(FixVector3 a, FixVector3 b)
	{
		return false;
	}

	public static bool operator !=(FixVector3 a, FixVector3 b)
	{
		return false;
	}

	public static FixVector3 Cross(FixVector3 a, FixVector3 b)
	{
		return default;
	}

	public FixVector3 Cross(FixVector3 b)
	{
		return default;
	}

	public Fix64 Distance(FixVector3 b)
	{
		return default;
	}

	public Fix64 DistanceSqr(FixVector3 b)
	{
		return default;
	}

	public static Fix64 Distance(FixVector3 a, FixVector3 b)
	{
		return default;
	}

	public static Fix64 DistanceSqr(FixVector3 a, FixVector3 b)
	{
		return default;
	}

	public static Fix64 DistanceXZ(FixVector3 a, FixVector3 b)
	{
		return default;
	}

	public static Fix64 DistanceSqrXZ(FixVector3 a, FixVector3 b)
	{
		return default;
	}

	public FixVector3 GetDirectionXZ()
	{
		return default;
	}

	public static Fix64 Dot(FixVector3 a, FixVector3 b)
	{
		return default;
	}

	public Fix64 Dot(FixVector3 b)
	{
		return default;
	}

	public Fix64 NormalizeAndGetLength()
	{
		return default;
	}

	public void Normalize()
	{
	}

	public static FixVector3 Normalize(FixVector3 value)
	{
		return default;
	}

	public static Fix64 SqrMagnitude(FixVector3 a)
	{
		return default;
	}

	public static Fix64 AngleRads(FixVector3 from, FixVector3 to)
	{
		return default;
	}

	public static Fix64 Angle(FixVector3 from, FixVector3 to)
	{
		return default;
	}

	public static FixVector3 ClampMagnitude(FixVector3 vector, Fix64 maxLength)
	{
		return default;
	}

	public static FixVector3 Lerp(FixVector3 from, FixVector3 to, Fix64 t)
	{
		return default;
	}

	public static FixVector3 LerpUnclamped(FixVector3 from, FixVector3 to, Fix64 t)
	{
		return default;
	}

	public static FixVector3 Max(FixVector3 a, FixVector3 b)
	{
		return default;
	}

	public static FixVector3 Min(FixVector3 a, FixVector3 b)
	{
		return default;
	}

	public static FixVector3 MoveTowards(FixVector3 current, FixVector3 target, Fix64 maxDistanceDelta)
	{
		return default;
	}

	public static void OrthoNormalize(ref FixVector3 normal, ref FixVector3 tangent)
	{
	}

	public static void OrthoNormalize(ref FixVector3 normal, ref FixVector3 tangent, ref FixVector3 binormal)
	{
	}

	public static FixVector3 Project(FixVector3 vector, FixVector3 onNormal)
	{
		return default;
	}

	public static FixVector3 ProjectOntoNormalized(FixVector3 vector, FixVector3 onNormalNormalized)
	{
		return default;
	}

	public static FixVector3 Reflect(FixVector3 inDirection, FixVector3 inNormal)
	{
		return default;
	}

	public static FixVector3 Scale(FixVector3 a, FixVector3 b)
	{
		return default;
	}

	public static FixVector3 Slerp(FixVector3 from, FixVector3 to, Fix64 t)
	{
		return default;
	}

	public static FixVector3 RotateTowards(FixVector3 from, FixVector3 to, Fix64 maxRadsDelta, Fix64 maxMagnitudeDelta)
	{
		return default;
	}

	public static FixVector3 SmoothDamp(FixVector3 current, FixVector3 target, ref FixVector3 currentVelocity, Fix64 smoothTime, Fix64 maxSpeed, Fix64 deltaTime)
	{
		return default;
	}

	public static FixVector3 ProjectOnPlane(FixVector3 vector, FixVector3 planeNormal)
	{
		return default;
	}

	public static bool EpsilonEqual(FixVector3 p1, FixVector3 p2)
	{
		return false;
	}

	public static bool EpsilonEqualXZ(FixVector3 p1, FixVector3 p2)
	{
		return false;
	}

	public static bool CloseToEqual(FixVector3 p1, FixVector3 p2, Fix64 epsilon)
	{
		return false;
	}

	public FixVector3 Add(FixVector3 b)
	{
		return default;
	}

	public FixVector3 Subtract(FixVector3 b)
	{
		return default;
	}

	public FixVector3 MultiplyScalar(Fix64 a)
	{
		return default;
	}

	public void BinWrite(FBinWriter writer)
	{
	}

	public void BinRead(FBinReader reader)
	{
	}

	public int CompareAxis(FAxis axis, FixVector3 other)
	{
		return 0;
	}

	public bool ApproximatelyEquals(FixVector3 v, Fix64 epsilon)
	{
		return false;
	}
}
