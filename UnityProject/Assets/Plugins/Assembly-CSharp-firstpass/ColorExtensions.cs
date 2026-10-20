using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (ColorExtensions @ 0x16cacc4..0x16cacd4, Ghidra vaddr).
public static class ColorExtensions
{
	public static Color CloneWithNewAlpha(this Color color, float alpha)
	{
		return new Color(color.r, color.g, color.b, alpha);
	}

	public static Color CloneWithMultipliedAlpha(this Color color, float alpha)
	{
		return new Color(color.r, color.g, color.b, color.a * alpha);
	}

	public static void ApplyMultipliedAlpha(this Color color, ref Color targetColor, float alpha)
	{
		targetColor.r = color.r;
		targetColor.g = color.g;
		targetColor.b = color.b;
		targetColor.a = color.a * alpha;
	}
}
