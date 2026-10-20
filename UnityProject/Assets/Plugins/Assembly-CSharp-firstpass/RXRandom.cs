using System;

// Rewritten from libil2cpp.so 1.0.605 (RXRandom @ 0x16cbaac..0x16cbe18, Ghidra vaddr).
public class RXRandom
{
	private static Random _randomSource = new Random();

	public static float Float()
	{
		return (float)_randomSource.NextDouble();
	}

	public static float Float(float max)
	{
		return (float)_randomSource.NextDouble() * max;
	}

	public static int Int()
	{
		return _randomSource.Next();
	}

	public static int Int(int max)
	{
		return _randomSource.Next() % max;
	}

	public static float Range(float low, float high)
	{
		return (high - low) * (float)_randomSource.NextDouble() + low;
	}

	public static int Range(int low, int high)
	{
		return _randomSource.Next() % (high - low) + low;
	}
}
