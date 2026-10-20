using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoEaseSinusoidal @ 0x016ce478..0x016ce640, Ghidra vaddr).
public static class GoEaseSinusoidal
{
	public static float EaseIn(float t, float b, float c, float d)
	{
		return c - (float)Math.Cos((double)(t / d) * (Math.PI / 2.0)) * c + b;
	}

	public static float EaseOut(float t, float b, float c, float d)
	{
		return (float)Math.Sin((double)(t / d) * (Math.PI / 2.0)) * c + b;
	}

	public static float EaseInOut(float t, float b, float c, float d)
	{
		return b + c * -0.5f * ((float)Math.Cos((double)t * Math.PI / (double)d) - 1f);
	}
}
