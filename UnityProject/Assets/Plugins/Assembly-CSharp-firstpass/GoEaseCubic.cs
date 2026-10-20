using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoEaseCubic @ 0x016cdb9c..0x016cdc28, Ghidra vaddr).
public static class GoEaseCubic
{
	public static float EaseIn(float t, float b, float c, float d)
	{
		t /= d;
		return t * t * t * c + b;
	}

	public static float EaseOut(float t, float b, float c, float d)
	{
		t = t / d - 1f;
		return (t * t * t + 1f) * c + b;
	}

	public static float EaseInOut(float t, float b, float c, float d)
	{
		t /= d * 0.5f;
		if (t < 1f)
		{
			return t * t * c * 0.5f * t + b;
		}
		t -= 2f;
		return c * 0.5f * (t * t * t + 2f) + b;
	}
}
