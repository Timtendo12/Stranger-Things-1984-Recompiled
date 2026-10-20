using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUIProgressBarPoint @ 0x17d7474, Ghidra vaddr).
public class FUIProgressBarPoint
{
	public float Percentage;

	public Color Color;

	public FUIProgressBarPoint(float percentage, Color color)
	{
		Percentage = percentage;
		Color = color;
	}
}
