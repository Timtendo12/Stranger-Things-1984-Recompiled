using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FMatrix @ 0x016900fc..0x01690aec, Ghidra vaddr).
public class FMatrix
{
	public float a = 1f;

	public float b;

	public float c;

	public float d = 1f;

	public float tx;

	public float ty;

	public FMatrix Clone()
	{
		FMatrix m = new FMatrix();
		m.a = a;
		m.b = b;
		m.c = c;
		m.d = d;
		m.tx = tx;
		m.ty = ty;
		return m;
	}

	public void CopyValues(FMatrix sourceMatrix)
	{
		a = sourceMatrix.a;
		b = sourceMatrix.b;
		c = sourceMatrix.c;
		d = sourceMatrix.d;
		tx = sourceMatrix.tx;
		ty = sourceMatrix.ty;
	}

	public void SetRotateThenScale(float x, float y, float scaleX, float scaleY, float rotationInRadians)
	{
		float sin = (float)Math.Sin(rotationInRadians);
		float cos = (float)Math.Cos(rotationInRadians);
		tx = x;
		ty = y;
		a = cos * scaleX;
		b = sin * scaleY;
		c = -(sin * scaleX);
		d = cos * scaleY;
	}

	public void SetScaleThenRotate(float x, float y, float scaleX, float scaleY, float rotationInRadians)
	{
		float sin = (float)Math.Sin(rotationInRadians);
		float cos = (float)Math.Cos(rotationInRadians);
		tx = x;
		ty = y;
		a = cos * scaleX;
		b = sin * scaleX;
		c = -(sin * scaleY);
		d = cos * scaleY;
	}

	public void SetScale(float scaleX, float scaleY)
	{
		a = scaleX;
		d = scaleY;
		b = 0f;
		c = 0f;
		tx = 0f;
		ty = 0f;
	}

	public void SetScaleSkewRotate(float x, float y, float scaleX, float scaleY, float skewXRadians, float skewYRadians, float rotationInRadians)
	{
		float sin = (float)Math.Sin(rotationInRadians);
		float cos = (float)Math.Cos(rotationInRadians);
		float skewXTan = (float)Math.Tan(skewXRadians);
		float skewYTan = (float)Math.Tan(skewYRadians);
		tx = x;
		ty = y;
		c = (cos * skewXTan - sin) * scaleY;
		d = (sin * skewXTan + cos) * scaleY;
		a = (cos - sin * skewYTan) * scaleX;
		b = (cos * skewYTan + sin) * scaleX;
	}

	public void SetScaleSkewRotateT(float x, float y, float scaleX, float scaleY, float skewXTan, float skewYTan, float rotationSin, float rotationCos)
	{
		tx = x;
		ty = y;
		a = (rotationCos - skewYTan * rotationSin) * scaleX;
		b = (skewYTan * rotationCos + rotationSin) * scaleX;
		c = (skewXTan * rotationCos - rotationSin) * scaleY;
		d = (skewXTan * rotationSin + rotationCos) * scaleY;
	}

	public void Translate(float deltaX, float deltaY)
	{
		tx += deltaX;
		ty += deltaY;
	}

	public void Translate(Vector2 t)
	{
		tx += t.x;
		ty += t.y;
	}

	public void Scale(float scaleX, float scaleY)
	{
		tx *= scaleX;
		ty *= scaleY;
		c *= scaleX;
		d *= scaleY;
		a *= scaleX;
		b *= scaleY;
	}

	public void Rotate(float rotationInRadians)
	{
		float sin = (float)Math.Sin(rotationInRadians);
		float cos = (float)Math.Cos(rotationInRadians);
		float oldA = a;
		float oldB = b;
		float oldC = c;
		float oldD = d;
		float oldTX = tx;
		float oldTY = ty;
		a = oldA * cos - oldB * sin;
		b = oldA * sin + oldB * cos;
		c = oldC * cos - oldD * sin;
		d = oldC * sin + oldD * cos;
		tx = oldTX * cos - oldTY * sin;
		ty = oldTX * sin + oldTY * cos;
	}

	public void RotateInPlace(float rotationInRadians)
	{
		float sin = (float)Math.Sin(rotationInRadians);
		float cos = (float)Math.Cos(rotationInRadians);
		float oldA = a;
		float oldB = b;
		float oldC = c;
		float oldD = d;
		a = oldA * cos - oldB * sin;
		b = oldA * sin + oldB * cos;
		c = oldC * cos - oldD * sin;
		d = oldC * sin + oldD * cos;
	}

	public void MakeSkew(float xRadians, float yRadians)
	{
		a = 1f;
		b = (float)Math.Tan(yRadians);
		c = (float)Math.Tan(xRadians);
		d = 1f;
		tx = 0f;
		ty = 0f;
	}

	public float GetScaleX()
	{
		return Mathf.Sqrt(a * a + b * b);
	}

	public float GetScaleY()
	{
		return Mathf.Sqrt(c * c + d * d);
	}

	public float GetRotation()
	{
		Vector2 v = GetNewTransformedVector(new Vector2(0f, 1f));
		return Mathf.Atan2(v.y - ty, v.x - tx) - (float)Math.PI / 2f;
	}

	public void Concat(FMatrix other)
	{
		float newA = a * other.a + b * other.c;
		float newB = a * other.b + b * other.d;
		float newC = c * other.a + d * other.c;
		float newD = c * other.b + d * other.d;
		float newTX = tx * other.a + ty * other.c + other.tx;
		float newTY = tx * other.b + ty * other.d + other.ty;
		a = newA;
		b = newB;
		c = newC;
		d = newD;
		tx = newTX;
		ty = newTY;
	}

	public void ConcatOther(FMatrix other)
	{
		float oldA = a;
		float oldB = b;
		float oldC = c;
		float oldD = d;
		a = oldA * other.a + other.b * oldC;
		b = oldD * other.b + oldB * other.a;
		c = oldA * other.c + oldC * other.d;
		d = oldD * other.d + other.c * oldB;
		tx = tx + oldA * other.tx + other.ty * oldC;
		ty = ty + oldD * other.ty + other.tx * oldB;
	}

	public void ConcatAndCopyValues(FMatrix first, FMatrix second)
	{
		a = first.a * second.a + first.b * second.c;
		b = first.a * second.b + first.b * second.d;
		c = first.c * second.a + first.d * second.c;
		d = first.c * second.b + first.d * second.d;
		tx = first.tx * second.a + first.ty * second.c + second.tx;
		ty = first.tx * second.b + first.ty * second.d + second.ty;
	}

	public void PreScale(float scaleX, float scaleY, FMatrix second)
	{
		a = second.a * scaleX;
		b = second.b * scaleX;
		c = second.c * scaleY;
		d = second.d * scaleY;
		tx = second.tx;
		ty = second.ty;
	}

	public void Invert()
	{
		float oldA = a;
		float oldB = b;
		float oldC = c;
		float oldD = d;
		float oldTX = tx;
		float oldTY = ty;
		float bottom = 1f / (oldA * oldD - oldB * oldC);
		a = oldD * bottom;
		b = -oldB * bottom;
		c = -oldC * bottom;
		d = oldA * bottom;
		tx = (oldC * oldTY - oldD * oldTX) * bottom;
		ty = -((oldA * oldTY - oldB * oldTX) * bottom);
	}

	public void InvertAndCopyValues(FMatrix other)
	{
		float bottom = 1f / (other.a * other.d - other.b * other.c);
		a = other.d * bottom;
		b = -other.b * bottom;
		c = -other.c * bottom;
		d = other.a * bottom;
		tx = (other.c * other.ty - other.d * other.tx) * bottom;
		ty = -((other.a * other.ty - other.b * other.tx) * bottom);
	}

	public Vector2 GetNewTransformedVector(Vector2 vector)
	{
		return new Vector2(vector.x * a + vector.y * c + tx, vector.x * b + vector.y * d + ty);
	}

	public Vector3 GetVector3FromLocalVector2(Vector2 localVector, float z)
	{
		return new Vector3(localVector.x * a + localVector.y * c + tx, localVector.x * b + localVector.y * d + ty, z);
	}

	public void ApplyVector3FromLocalVector2(out Vector3 outVector, Vector2 localVector, float z)
	{
		outVector.x = tx + a * localVector.x + c * localVector.y;
		outVector.y = ty + b * localVector.x + d * localVector.y;
		outVector.z = z;
	}

	public void Identity()
	{
		a = 1f;
		b = 0f;
		c = 0f;
		d = 1f;
		tx = 0f;
		ty = 0f;
	}

	public override string ToString()
	{
		return string.Format("[[Matrix A:{0} B:{1} C:{2} D:{3} TX:{4} TY:{5} ]]", a, b, c, d, tx, ty);
	}
}
