using System;
using System.Reflection;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoTweenUtils @ 0x016d4f08..0x019fb3f4, Ghidra vaddr).
public static class GoTweenUtils
{
	public static Func<float, float, float, float, float> easeFunctionForType(GoEaseType easeType)
	{
		switch (easeType)
		{
		case GoEaseType.SineIn:
			return GoEaseSinusoidal.EaseIn;
		case GoEaseType.SineOut:
			return GoEaseSinusoidal.EaseOut;
		case GoEaseType.SineInOut:
			return GoEaseSinusoidal.EaseInOut;
		case GoEaseType.QuadIn:
			return GoEaseQuadratic.EaseIn;
		case GoEaseType.QuadOut:
			return GoEaseQuadratic.EaseOut;
		case GoEaseType.QuadInOut:
			return GoEaseQuadratic.EaseInOut;
		case GoEaseType.CubicIn:
			return GoEaseCubic.EaseIn;
		case GoEaseType.CubicOut:
			return GoEaseCubic.EaseOut;
		case GoEaseType.CubicInOut:
			return GoEaseCubic.EaseInOut;
		case GoEaseType.QuartIn:
			return GoEaseQuartic.EaseIn;
		case GoEaseType.QuartOut:
			return GoEaseQuartic.EaseOut;
		case GoEaseType.QuartInOut:
			return GoEaseQuartic.EaseInOut;
		case GoEaseType.QuintIn:
			return GoEaseQuintic.EaseIn;
		case GoEaseType.QuintOut:
			return GoEaseQuintic.EaseOut;
		case GoEaseType.QuintInOut:
			return GoEaseQuintic.EaseInOut;
		case GoEaseType.ExpoIn:
			return GoEaseExponential.EaseIn;
		case GoEaseType.ExpoOut:
			return GoEaseExponential.EaseOut;
		case GoEaseType.ExpoInOut:
			return GoEaseExponential.EaseInOut;
		case GoEaseType.CircIn:
			return GoEaseCircular.EaseIn;
		case GoEaseType.CircOut:
			return GoEaseCircular.EaseOut;
		case GoEaseType.CircInOut:
			return GoEaseCircular.EaseInOut;
		case GoEaseType.ElasticIn:
			return GoEaseElastic.EaseIn;
		case GoEaseType.ElasticOut:
			return GoEaseElastic.EaseOut;
		case GoEaseType.ElasticInOut:
			return GoEaseElastic.EaseInOut;
		case GoEaseType.BackIn:
			return GoEaseBack.EaseIn;
		case GoEaseType.BackOut:
			return GoEaseBack.EaseOut;
		case GoEaseType.BackInOut:
			return GoEaseBack.EaseInOut;
		case GoEaseType.BounceIn:
			return GoEaseBounce.EaseIn;
		case GoEaseType.BounceOut:
			return GoEaseBounce.EaseOut;
		case GoEaseType.BounceInOut:
			return GoEaseBounce.EaseInOut;
		default:
			return GoEaseLinear.EaseNone;
		}
	}

	public static T setterForProperty<T>(object targetObject, string propertyName)
	{
		PropertyInfo property = targetObject.GetType().GetProperty(propertyName);
		if (property == null)
		{
			FAssert.Fail("could not find property with name: " + propertyName);
			return default(T);
		}
		if (property.GetSetMethod() == null)
		{
			FAssert.Fail("no setter for property: " + propertyName);
			return default(T);
		}
		return (T)(object)Delegate.CreateDelegate(typeof(T), targetObject, property.GetSetMethod());
	}

	public static T getterForProperty<T>(object targetObject, string propertyName)
	{
		PropertyInfo property = targetObject.GetType().GetProperty(propertyName);
		if (property == null)
		{
			FAssert.Fail("could not find property with name: " + propertyName);
			return default(T);
		}
		if (property.GetGetMethod() == null)
		{
			FAssert.Fail("no getter for property: " + propertyName);
			return default(T);
		}
		return (T)(object)Delegate.CreateDelegate(typeof(T), targetObject, property.GetGetMethod());
	}

	public static Color unclampedColorLerp(Color c1, Color diff, float value)
	{
		return new Color(c1.r + diff.r * value, c1.g + diff.g * value, c1.b + diff.b * value, c1.a + diff.a * value);
	}

	public static Vector2 unclampedVector2Lerp(Vector2 v1, Vector2 diff, float value)
	{
		return new Vector2(v1.x + diff.x * value, v1.y + diff.y * value);
	}

	public static Vector3 unclampedVector3Lerp(Vector3 v1, Vector3 diff, float value)
	{
		return new Vector3(v1.x + diff.x * value, v1.y + diff.y * value, v1.z + diff.z * value);
	}

	public static Vector4 unclampedVector4Lerp(Vector4 v1, Vector4 diff, float value)
	{
		return new Vector4(v1.x + diff.x * value, v1.y + diff.y * value, v1.z + diff.z * value, v1.w + diff.w * value);
	}
}
