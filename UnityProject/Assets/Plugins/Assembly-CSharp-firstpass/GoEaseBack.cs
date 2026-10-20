using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoEaseBack @ 0x016cd74c..0x016cd83c, Ghidra vaddr).
public static class GoEaseBack
{
	public static float EaseIn(float t, float b, float c, float d)
	{
		t /= d;
		return t * t * c * (t * 2.70158f + -1.70158f) + b;
	}

	public static float EaseOut(float t, float b, float c, float d)
	{
		t = t / d - 1f;
		return (t * t * (t * 2.70158f + 1.70158f) + 1f) * c + b;
	}

	public static float EaseInOut(float t, float b, float c, float d)
	{
		t /= d * 0.5f;
		if (t < 1f)
		{
			return c * 0.5f * (t * t * (t * 3.5949094f + -2.5949094f)) + b;
		}
		t -= 2f;
		return c * 0.5f * (t * t * (t * 3.5949094f + 2.5949094f) + 2f) + b;
	}
}
