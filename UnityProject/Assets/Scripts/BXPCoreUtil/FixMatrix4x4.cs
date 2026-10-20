using System;

public struct FixMatrix4x4 : IEquatable<FixMatrix4x4>, IFormattable
{
	public Fix64 m00;

	public Fix64 m01;

	public Fix64 m02;

	public Fix64 m03;

	public Fix64 m10;

	public Fix64 m11;

	public Fix64 m12;

	public Fix64 m13;

	public Fix64 m20;

	public Fix64 m21;

	public Fix64 m22;

	public Fix64 m23;

	public Fix64 m30;

	public Fix64 m31;

	public Fix64 m32;

	public Fix64 m33;

	public static readonly FixMatrix4x4 zero;

	public static readonly FixMatrix4x4 identity;

	public static readonly Fix64 cInverseMinDeterminant;

	public bool isIdentity => false;

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

	public Fix64 this[int row, int column]
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public Fix64 determinant => default;

	public FixMatrix4x4 inverse => default;

	public FixMatrix4x4 transpose => default;

	public FixMatrix4x4(Fix64 n00, Fix64 n01, Fix64 n02, Fix64 n03, Fix64 n10, Fix64 n11, Fix64 n12, Fix64 n13, Fix64 n20, Fix64 n21, Fix64 n22, Fix64 n23, Fix64 n30, Fix64 n31, Fix64 n32, Fix64 n33)
	{
		m00 = default;
		m01 = default;
		m02 = default;
		m03 = default;
		m10 = default;
		m11 = default;
		m12 = default;
		m13 = default;
		m20 = default;
		m21 = default;
		m22 = default;
		m23 = default;
		m30 = default;
		m31 = default;
		m32 = default;
		m33 = default;
	}

	public override bool Equals(object other)
	{
		return false;
	}

	public bool Equals(FixMatrix4x4 other)
	{
		return false;
	}

	public static bool operator ==(FixMatrix4x4 a, FixMatrix4x4 b)
	{
		return false;
	}

	public static bool operator !=(FixMatrix4x4 a, FixMatrix4x4 b)
	{
		return false;
	}

	public override int GetHashCode()
	{
		return 0;
	}

	public FixVector4 GetColumn(int i)
	{
		return default;
	}

	public FixVector4 GetRow(int i)
	{
		return default;
	}

	public static FixMatrix4x4 operator *(FixMatrix4x4 lhs, FixMatrix4x4 rhs)
	{
		return default;
	}

	public static FixVector4 operator *(FixMatrix4x4 lhs, FixVector4 v)
	{
		return default;
	}

	public FixVector3 MultiplyPoint(FixVector3 v)
	{
		return default;
	}

	public FixVector3 MultiplyPoint3x4(FixVector3 v)
	{
		return default;
	}

	public FixVector3 MultiplyVector(FixVector3 v)
	{
		return default;
	}

	public static FixMatrix4x4 Scale(FixVector3 v)
	{
		return default;
	}

	public static FixMatrix4x4 Translate(FixVector3 t)
	{
		return default;
	}

	public static FixMatrix4x4 Rotate(FixQuaternion q)
	{
		return default;
	}

	public static FixMatrix4x4 RotateFromNormalized(FixVector3 forward, FixVector3 up, FixVector3 right)
	{
		return default;
	}

	public void SetColumn(int i, FixVector4 v)
	{
	}

	public void SetRow(int i, FixVector4 v)
	{
	}

	public static FixMatrix4x4 Inverse(FixMatrix4x4 m)
	{
		return default;
	}

	public static FixMatrix4x4 Transpose(FixMatrix4x4 m)
	{
		return default;
	}

	public static FixMatrix4x4 TRS(FixVector3 pos, FixQuaternion q, FixVector3 s)
	{
		return default;
	}

	public void SetTRS(FixVector3 pos, FixQuaternion q, FixVector3 s)
	{
	}

	public static FixMatrix4x4 Ortho(Fix64 left, Fix64 right, Fix64 bottom, Fix64 top, Fix64 zNear, Fix64 zFar)
	{
		return default;
	}

	public static FixMatrix4x4 Perspective(Fix64 fov, Fix64 aspect, Fix64 zNear, Fix64 zFar)
	{
		return default;
	}

	public void DecomposeTRS(out FixVector3 pos, out FixQuaternion q, out FixVector3 s)
	{
		pos = default;
		q = default;
		s = default;
	}

	public string ToString(string format)
	{
		return null;
	}

	public string ToString(string format, IFormatProvider provider)
	{
		return null;
	}

	public override string ToString()
	{
		return null;
	}
}
