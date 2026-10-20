using System;

public struct FixQuaternion : IEquatable<FixQuaternion>, IFixSplineInterp<FixQuaternion>, IFormattable
{
	public Fix64 x;

	public Fix64 y;

	public Fix64 z;

	public Fix64 w;

	public static readonly FixQuaternion identity;

	private static readonly Fix64 EqualityCutoff;

	private static Fix64 SlerpDotCutoff;

	private static Fix64 UpCutoff;

	private static Fix64 DownCutoff;

	public FixQuaternion normalized => default;

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

	public FixVector3 eulerAngles => default;

	public FixVector3 eulerAnglesRads => default;

	public FixQuaternion(Fix64 vx, Fix64 vy, Fix64 vz, Fix64 vw)
	{
		x = default;
		y = default;
		z = default;
		w = default;
	}

	public FixQuaternion(FixVector3 vec, Fix64 vw)
	{
		x = default;
		y = default;
		z = default;
		w = default;
	}

	public static FixQuaternion Make(float x, float y, float z, float w)
	{
		return default;
	}

	public static FixQuaternion Make(double x, double y, double z, float w)
	{
		return default;
	}

	public static FixQuaternion AngleAxisRads(Fix64 angle, FixVector3 axis)
	{
		return default;
	}

	public static FixQuaternion AngleAxis(Fix64 angle, FixVector3 axis)
	{
		return default;
	}

	public override bool Equals(object obj)
	{
		return false;
	}

	public void Set(Fix64 vx, Fix64 vy, Fix64 vz, Fix64 vw)
	{
	}

	public void SetFromToRotation(FixVector3 fromDirection, FixVector3 toDirection)
	{
	}

	public void Normalize()
	{
	}

	public static FixQuaternion Normalize(FixQuaternion value)
	{
		return default;
	}

	public bool Equals(FixQuaternion other)
	{
		return false;
	}

	public static bool operator ==(FixQuaternion lhs, FixQuaternion rhs)
	{
		return false;
	}

	public static bool operator !=(FixQuaternion lhs, FixQuaternion rhs)
	{
		return false;
	}

	public override int GetHashCode()
	{
		return 0;
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

	public static Fix64 Dot(FixQuaternion a, FixQuaternion b)
	{
		return default;
	}

	public Fix64 Dot(FixQuaternion b)
	{
		return default;
	}

	public static FixQuaternion operator *(FixQuaternion lhs, FixQuaternion rhs)
	{
		return default;
	}

	public static FixVector3 operator *(FixQuaternion rotation, FixVector3 point)
	{
		return default;
	}

	public static FixQuaternion operator /(FixQuaternion lhs, FixQuaternion rhs)
	{
		return default;
	}

	public static FixQuaternion RotateXRads(Fix64 angleRads)
	{
		return default;
	}

	public static FixQuaternion RotateX(Fix64 angleDegrees)
	{
		return default;
	}

	public static FixQuaternion RotateYRads(Fix64 angleRads)
	{
		return default;
	}

	public static FixQuaternion RotateY(Fix64 angleDegrees)
	{
		return default;
	}

	public static FixQuaternion RotateZRads(Fix64 angleRads)
	{
		return default;
	}

	public static FixQuaternion RotateZ(Fix64 angleDegrees)
	{
		return default;
	}

	public static FixQuaternion Euler(Fix64 x, Fix64 y, Fix64 z)
	{
		return default;
	}

	public static FixQuaternion Euler(FixVector3 euler)
	{
		return default;
	}

	public static Fix64 AngleRads(FixQuaternion a, FixQuaternion b)
	{
		return default;
	}

	public static Fix64 Angle(FixQuaternion a, FixQuaternion b)
	{
		return default;
	}

	public Fix64 AngleRads()
	{
		return default;
	}

	public Fix64 Angle()
	{
		return default;
	}

	public static FixQuaternion operator -(FixQuaternion a)
	{
		return default;
	}

	public static FixQuaternion FromToRotation(FixVector3 fromDirection, FixVector3 toDirection)
	{
		return default;
	}

	public static FixQuaternion operator *(Fix64 k, FixQuaternion v)
	{
		return default;
	}

	public static FixQuaternion operator *(FixQuaternion v, Fix64 k)
	{
		return default;
	}

	public static FixQuaternion operator +(FixQuaternion a, FixQuaternion b)
	{
		return default;
	}

	public static FixQuaternion operator -(FixQuaternion a, FixQuaternion b)
	{
		return default;
	}

	public static FixQuaternion SlerpUnclamped(FixQuaternion from, FixQuaternion to, Fix64 t)
	{
		return default;
	}

	public static FixQuaternion Slerp(FixQuaternion from, FixQuaternion to, Fix64 t)
	{
		return default;
	}

	public static FixQuaternion RotateTowardsRads(FixQuaternion from, FixQuaternion to, Fix64 maxRadsDelta)
	{
		return default;
	}

	public static FixQuaternion RotateTowards(FixQuaternion from, FixQuaternion to, Fix64 maxDegreesDelta)
	{
		return default;
	}

	public void ToAngleAxisRads(out Fix64 angle, out FixVector3 axis)
	{
		angle = default;
		axis = default;
	}

	public void ToAngleAxis(out Fix64 angle, out FixVector3 axis)
	{
		angle = default;
		axis = default;
	}

	public static FixQuaternion Inverse(FixQuaternion rotation)
	{
		return default;
	}

	public static FixQuaternion LerpUnclamped(FixQuaternion from, FixQuaternion to, Fix64 t)
	{
		return default;
	}

	public static FixQuaternion Lerp(FixQuaternion from, FixQuaternion to, Fix64 t)
	{
		return default;
	}

	public static FixQuaternion LookRotation(FixVector3 forward, FixVector3 up)
	{
		return default;
	}

	public static FixQuaternion CreateFromRotationMatrix(FixMatrix4x4 r)
	{
		return default;
	}

	public static FixQuaternion LookRotation(FixVector3 forward)
	{
		return default;
	}

	public void SetLookRotation(FixVector3 forward, FixVector3 up)
	{
	}

	public void SetLookRotation(FixVector3 forward)
	{
	}

	private static FixVector3 NormalizeAnglesRads(FixVector3 angles)
	{
		return default;
	}

	private static Fix64 NormalizeAngleRads(Fix64 angle)
	{
		return default;
	}

	public bool ApproximatelyEquals(FixQuaternion v, Fix64 epsilon)
	{
		return false;
	}

	public FixQuaternion Add(FixQuaternion b)
	{
		return default;
	}

	public FixQuaternion Subtract(FixQuaternion b)
	{
		return default;
	}

	public FixQuaternion MultiplyScalar(Fix64 a)
	{
		return default;
	}

	public void BinWrite(FBinWriter writer)
	{
	}

	public void BinRead(FBinReader reader)
	{
	}
}
