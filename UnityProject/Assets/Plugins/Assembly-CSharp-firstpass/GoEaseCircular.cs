using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoEaseCircular @ 0x016cd9b0..0x016cdb9c, Ghidra vaddr).
public static class GoEaseCircular
{
	public static float EaseIn(float t, float b, float c, float d)
	{
		t /= d;
		return b - (Mathf.Sqrt(1f - t * t) - 1f) * c;
	}

	public static float EaseOut(float t, float b, float c, float d)
	{
		t = t / d - 1f;
		return Mathf.Sqrt(1f - t * t) * c + b;
	}

	public static float EaseInOut(float t, float b, float c, float d)
	{
		t /= d * 0.5f;
		if (t < 1f)
		{
			return -0.5f * c * (Mathf.Sqrt(1f - t * t) - 1f) + b;
		}
		t -= 2f;
		return 0.5f * c * (Mathf.Sqrt(1f - t * t) + 1f) + b;
	}
}
