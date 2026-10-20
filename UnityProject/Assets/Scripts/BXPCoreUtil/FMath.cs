using System;
using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FMath @ 0x0188a48c..0x019eea94, Ghidra vaddr).
public static class FMath
{
	public static float Epsilon;

	public const float cRecip255 = 1f / 255f;

	public const float cTwoPi = (float)Math.PI * 2f;

	public const float cPiOver2 = (float)Math.PI / 2f;

	public const float cThreePiOver2 = 4.712389f;

	public const float cRecipTwoPi = 1f / (2f * (float)Math.PI);

	public const float cDegreesPerRadian = 57.29578f;

	public const float cRadiansPerDegree = (float)Math.PI / 180f;

	private static Dictionary<Type, object> mLerpFuncDictionary;

	static FMath()
	{
		Epsilon = 0.0001f;
		mLerpFuncDictionary = new Dictionary<Type, object>();
		AddLerpFunc(typeof(float), new Func<float, float, float, float>(LerpFloat));
		AddLerpFunc(typeof(int), new Func<int, int, float, int>(LerpInt));
	}

	public static void AddLerpFunc(Type type, object func)
	{
		mLerpFuncDictionary[type] = func;
	}

	public static Func<T, T, float, T> GetLerpFunc<T>()
	{
		return mLerpFuncDictionary[typeof(T)] as Func<T, T, float, T>;
	}

	public static float Clamp01(float x)
	{
		if (x < 0f)
		{
			return 0f;
		}
		if (x > 1f)
		{
			return 1f;
		}
		return x;
	}

	public static float LerpFloat(float a, float b, float t)
	{
		return (b - a) * Clamp01(t) + a;
	}

	public static int LerpInt(int a, int b, float t)
	{
		return (int)(Clamp01(t) * (float)(b - a) + (float)a);
	}

	public static int LerpIntRound(int a, int b, float t)
	{
		return RoundToInt(Clamp01(t) * (float)(b - a) + (float)a);
	}

	public static int FloorToInt(double x)
	{
		return (int)Math.Floor(x);
	}

	public static int FloorToInt(float x)
	{
		return (int)Math.Floor(x);
	}

	public static int RoundToInt(double x)
	{
		return (int)Math.Round(x);
	}

	public static int RoundToInt(float x)
	{
		return (int)Math.Round(x);
	}

	public static float NormalizedDegreesToRadians(float angleDegrees)
	{
		// Shipped code multiplies by cDegreesPerRadian (not cRadiansPerDegree); kept as-is.
		float r = angleDegrees * cDegreesPerRadian;
		while (r > cTwoPi)
		{
			r -= cTwoPi;
		}
		while (r < 0f)
		{
			r += cTwoPi;
		}
		return r;
	}

	public static Fix64 DistanceToLineSegment(FixVector3 testPoint, FixVector3 lineSegmentPoint1, FixVector3 lineSegmentPoint2, out FixVector3 closestPoint)
	{
		return Fix64.Sqrt(DistanceToLineSegmentSquared(testPoint, lineSegmentPoint1, lineSegmentPoint2, out closestPoint));
	}

	public static Fix64 DistanceToLineSegmentSquared(FixVector3 testPoint, FixVector3 lineSegmentPoint1, FixVector3 lineSegmentPoint2, out FixVector3 closestPoint)
	{
		FixVector3 segment = lineSegmentPoint2 - lineSegmentPoint1;
		Fix64 lengthSqr = segment.sqrMagnitude;
		if (lengthSqr < Fix64.Epsilon)
		{
			closestPoint = lineSegmentPoint1;
		}
		else
		{
			Fix64 t = FixVector3.Dot(testPoint - lineSegmentPoint1, segment) / lengthSqr;
			if (t <= Fix64.Zero)
			{
				closestPoint = lineSegmentPoint1;
			}
			else if (t >= Fix64.One)
			{
				closestPoint = lineSegmentPoint2;
			}
			else
			{
				closestPoint = lineSegmentPoint1 + segment * t;
			}
		}
		return (testPoint - closestPoint).sqrMagnitude;
	}

	public static Fix64 DistanceToLineSegment(FixVector3 testPoint, FixVector3 lineSegmentPoint1, FixVector3 lineSegmentPoint2)
	{
		return Fix64.Sqrt(DistanceToLineSegmentSquared(testPoint, lineSegmentPoint1, lineSegmentPoint2));
	}

	public static Fix64 DistanceToLineSegmentSquared(FixVector3 testPoint, FixVector3 lineSegmentPoint1, FixVector3 lineSegmentPoint2)
	{
		FixVector3 closestPoint;
		return DistanceToLineSegmentSquared(testPoint, lineSegmentPoint1, lineSegmentPoint2, out closestPoint);
	}

	public static Fix64 AngleBetween2D(FixVector3 vector1, FixVector3 vector2)
	{
		Fix64 angle = Fix64.Atan2(vector2.z, vector2.x) - Fix64.Atan2(vector1.z, vector1.x);
		if (angle < Fix64.Zero)
		{
			return angle + Fix64.TwoPi;
		}
		return angle;
	}

	public static Cardinal IncomingDirection2D(FixVector3 sourceToTarget, FixVector3 targetFacing)
	{
		Fix64 degrees = Fix64.Rad2Deg * AngleBetween2D(sourceToTarget, targetFacing);
		if (degrees > 45 && degrees <= 135)
		{
			return Cardinal.Left;
		}
		if (degrees > 135 && degrees <= 225)
		{
			return Cardinal.Front;
		}
		if (degrees > 225 && degrees <= 315)
		{
			return Cardinal.Right;
		}
		return Cardinal.Back;
	}

	public static float UnclampedLerp(float from, float to, float t)
	{
		return (to - from) * t + from;
	}

	public static float smoothstep(float x)
	{
		return x * x * (3f - (x + x));
	}

	public static float smoothstep(float minVal, float maxVal, float x)
	{
		float t = Clamp01((x - minVal) / (maxVal - minVal));
		return t * t * (3f - (t + t));
	}

	public static int ManhattanDistance(int px, int py, int qx, int qy)
	{
		return Math.Abs(px - qx) + Math.Abs(py - qy);
	}

	public static float LinearMapping(float x, float fromMin, float fromMax, float toMin, float toMax)
	{
		return (x - fromMin) * (toMax - toMin) / (fromMax - fromMin) + toMin;
	}

	public static Fix64 LinearMapping(Fix64 x, Fix64 fromMin, Fix64 fromMax, Fix64 toMin, Fix64 toMax)
	{
		return toMin + (x - fromMin) * (toMax - toMin) / (fromMax - fromMin);
	}

	public static bool IsCloseToLoopedNormalizedTime(Fix64 val, Fix64 target, Fix64 delta)
	{
		if (Fix64.Abs(val - target) < delta)
		{
			return true;
		}
		return Fix64.Abs(val + Fix64.One - target) < delta;
	}
}
