using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoEaseBounce @ 0x016cd83c..0x016cd9b0, Ghidra vaddr).
public static class GoEaseBounce
{
	public static float EaseOut(float t, float b, float c, float d)
	{
		t /= d;
		if ((double)t < 1.0 / 2.75)
		{
			t = t * t * 7.5625f;
		}
		else if ((double)t < 2.0 / 2.75)
		{
			t -= 1.5f / 2.75f;
			t = t * t * 7.5625f + 0.75f;
		}
		else if ((double)t < 2.5 / 2.75)
		{
			t -= 2.25f / 2.75f;
			t = t * t * 7.5625f + 0.9375f;
		}
		else
		{
			t -= 2.625f / 2.75f;
			t = t * t * 7.5625f + 0.984375f;
		}
		return t * c + b;
	}

	public static float EaseIn(float t, float b, float c, float d)
	{
		return c - EaseOut(d - t, 0f, c, d) + b;
	}

	public static float EaseInOut(float t, float b, float c, float d)
	{
		if (t < d * 0.5f)
		{
			return (c - EaseOut(d - (t + t), 0f, c, d) + 0f) * 0.5f + b;
		}
		return EaseOut(t + t - d, 0f, c, d) * 0.5f + c * 0.5f + b;
	}
}
