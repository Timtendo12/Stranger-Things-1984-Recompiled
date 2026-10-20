using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoEaseExponential @ 0x016cdfcc..0x016ce22c, Ghidra vaddr).
public static class GoEaseExponential
{
	public static float EaseIn(float t, float b, float c, float d)
	{
		if (t == 0f)
		{
			return b;
		}
		return (float)Math.Pow(2.0, (double)((t / d - 1f) * 10f)) * c + b;
	}

	public static float EaseOut(float t, float b, float c, float d)
	{
		if (t == d)
		{
			return b + c;
		}
		return b + (float)(1.0 - Math.Pow(2.0, (double)(t * -10f / d))) * c;
	}

	public static float EaseInOut(float t, float b, float c, float d)
	{
		if (t == 0f)
		{
			return b;
		}
		if (t == d)
		{
			return b + c;
		}
		t /= d * 0.5f;
		if (t < 1f)
		{
			return c * 0.5f * (float)Math.Pow(2.0, (double)((t - 1f) * 10f)) + b;
		}
		return c * 0.5f * (float)(2.0 - Math.Pow(2.0, (double)((t - 1f) * -10f))) + b;
	}
}
