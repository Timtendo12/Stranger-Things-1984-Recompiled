using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoEaseQuintic @ 0x016ce3cc..0x016ce478, Ghidra vaddr).
public static class GoEaseQuintic
{
	public static float EaseIn(float t, float b, float c, float d)
	{
		t /= d;
		return t * t * t * t * t * c + b;
	}

	public static float EaseOut(float t, float b, float c, float d)
	{
		t = t / d - 1f;
		return (t * t * t * t * t + 1f) * c + b;
	}

	public static float EaseInOut(float t, float b, float c, float d)
	{
		t /= d * 0.5f;
		if (t < 1f)
		{
			return t * t * t * t * c * 0.5f * t + b;
		}
		t -= 2f;
		return c * 0.5f * (t * t * t * t * t + 2f) + b;
	}
}
