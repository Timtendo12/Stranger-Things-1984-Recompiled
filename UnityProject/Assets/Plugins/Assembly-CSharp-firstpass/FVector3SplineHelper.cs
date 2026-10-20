using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FVector3SplineHelper @ 0x16727b0..0x1672914, Ghidra vaddr).
public class FVector3SplineHelper : ISplineHelper<Vector3>
{
	public static FVector3SplineHelper Instance = new FVector3SplineHelper();

	public Vector3 Add(Vector3 a, Vector3 b)
	{
		return a + b;
	}

	public Vector3 MultiplyScalar(float a, Vector3 b)
	{
		return b * a;
	}

	public Vector3 Subtract(Vector3 a, Vector3 b)
	{
		return a - b;
	}

	public Vector3 BinRead(FBinReader reader)
	{
		return reader.ReadVector3();
	}

	public void BinWrite(FBinWriter writer, Vector3 a)
	{
		writer.Write(a);
	}

	public static FHermiteSpline<Vector3> CreateCatmullRom(Vector3[] keys, float[] times, bool loopTangents)
	{
		return FHermiteSpline<Vector3>.CreateCatmullRom(Instance, keys, times, loopTangents);
	}
}
