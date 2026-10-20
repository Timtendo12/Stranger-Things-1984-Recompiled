using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoEaseElastic @ 0x016cdc28..0x016cdfd4, Ghidra vaddr).
public class GoEaseElastic
{
	public static float EaseIn(float t, float b, float c, float d)
	{
		if (t == 0f)
		{
			return b;
		}
		if (t / d == 1f)
		{
			return b + c;
		}
		float num = d * 0.3f;
		float num2 = t / d - 1f;
		return b + (float)(Math.Sin((double)(num2 * d - num * 0.25f) * (-Math.PI * 2.0) / (double)num) * Math.Pow(2.0, (double)(num2 * 10f)) * (double)c);
	}

	public static float EaseOut(float t, float b, float c, float d)
	{
		if (t == 0f)
		{
			return b;
		}
		t /= d;
		if (t == 1f)
		{
			return b + c;
		}
		float num = d * 0.3f;
		return (float)(Math.Sin((double)(t * d - num * 0.25f) * (Math.PI * 2.0) / (double)num) * Math.Pow(2.0, (double)(t * -10f)) * (double)c + (double)c + (double)b);
	}

	public static float EaseInOut(float t, float b, float c, float d)
	{
		if (t == 0f)
		{
			return b;
		}
		t /= d * 0.5f;
		if (t == 2f)
		{
			return b + c;
		}
		float num = d * 0.45f;
		float num2 = t - 1f;
		if (t < 1f)
		{
			return b + (float)(Math.Sin((double)(num2 * d - num * 0.25f) * (-Math.PI * 2.0) / (double)num) * Math.Pow(2.0, (double)(num2 * 10f)) * (double)c) * 0.5f;
		}
		return (float)(Math.Sin((double)(num2 * d - num * 0.25f) * (Math.PI * 2.0) / (double)num) * Math.Pow(2.0, (double)(num2 * -10f)) * (double)c * 0.5 + (double)c + (double)b);
	}
}
