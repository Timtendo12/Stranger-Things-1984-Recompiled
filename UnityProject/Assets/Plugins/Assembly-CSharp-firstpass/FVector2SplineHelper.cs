using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FVector2SplineHelper @ 0x1672670..0x16727b0, Ghidra vaddr).
public class FVector2SplineHelper : ISplineHelper<Vector2>
{
	public static FVector2SplineHelper Instance = new FVector2SplineHelper();

	public Vector2 Add(Vector2 a, Vector2 b)
	{
		return a + b;
	}

	public Vector2 MultiplyScalar(float a, Vector2 b)
	{
		return b * a;
	}

	public Vector2 Subtract(Vector2 a, Vector2 b)
	{
		return a - b;
	}

	public Vector2 BinRead(FBinReader reader)
	{
		return reader.ReadVector2();
	}

	public void BinWrite(FBinWriter writer, Vector2 a)
	{
		writer.Write(a);
	}

	public static FHermiteSpline<Vector2> CreateCatmullRom(Vector2[] keys, float[] times, bool loopTangents)
	{
		return FHermiteSpline<Vector2>.CreateCatmullRom(Instance, keys, times, loopTangents);
	}
}
