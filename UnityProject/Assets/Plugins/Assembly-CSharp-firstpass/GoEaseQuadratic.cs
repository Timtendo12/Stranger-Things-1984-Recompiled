using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoEaseQuadratic @ 0x016ce2b0..0x016ce330, Ghidra vaddr).
public static class GoEaseQuadratic
{
	public static float EaseIn(float t, float b, float c, float d)
	{
		return t / d * (t / d) * c + b;
	}

	public static float EaseOut(float t, float b, float c, float d)
	{
		return b - t / d * c * (t / d - 2f);
	}

	public static float EaseInOut(float t, float b, float c, float d)
	{
		t /= d * 0.5f;
		if (t < 1f)
		{
			return t * c * 0.5f * t + b;
		}
		t -= 1f;
		return c * -0.5f * (t * (t - 2f) - 1f) + b;
	}
}
