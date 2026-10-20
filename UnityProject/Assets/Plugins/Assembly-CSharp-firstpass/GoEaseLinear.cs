using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (GoEaseLinear @ 0x016ce22c..0x016ce2b0, Ghidra vaddr).
public static class GoEaseLinear
{
	public static float EaseNone(float t, float b, float c, float d)
	{
		return t * c / d + b;
	}

	public static float Punch(float t, float b, float c, float d)
	{
		if (t == 0f)
		{
			return 0f;
		}
		t /= d;
		if (t == 1f)
		{
			return 0f;
		}
		return Mathf.Pow(2f, t * -10f) * c * Mathf.Sin(t * 6.2831855f / 0.3f);
	}
}
