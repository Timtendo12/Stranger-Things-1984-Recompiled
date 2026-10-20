using System;

// Rewritten from libil2cpp.so 1.0.605 (FPatherPoint @ 0x18150e0..0x1815270, Ghidra vaddr).
public class FPatherPoint : IComparable
{
	public FPoint2 Point = new FPoint2(0, 0);

	public FPatherPoint PrevPoint;

	public int Direction;

	public float Cost;

	public float Estimate;

	public void Init()
	{
		Point.Invalidate();
		PrevPoint = null;
		Estimate = 0f;
		Direction = 0;
	}

	public int CompareTo(object obj)
	{
		FPatherPoint other = obj as FPatherPoint;
		if (other == null)
		{
			throw new ArgumentException("Comparing to non FSimPatherPoint.", "CompareTo");
		}
		float a = Cost + Estimate;
		float b = other.Cost + other.Estimate;
		if (a < b)
		{
			return -1;
		}
		return (b < a) ? 1 : 0;
	}
}
