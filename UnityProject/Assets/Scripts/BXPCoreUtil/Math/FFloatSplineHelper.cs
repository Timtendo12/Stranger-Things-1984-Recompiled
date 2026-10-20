// Rewritten from libil2cpp.so 1.0.605 (FFloatSplineHelper @ 0x186ae28..0x186af90, Ghidra vaddr).
public class FFloatSplineHelper : ISplineHelper<float>
{
	public static FFloatSplineHelper Instance = new FFloatSplineHelper();

	public float Add(float a, float b)
	{
		return a + b;
	}

	public float MultiplyScalar(float a, float b)
	{
		return b * a;
	}

	public float Subtract(float a, float b)
	{
		return a - b;
	}

	public static FHermiteSpline<float> CreateCatmullRom(float[] keys, float[] times, bool loopTangents)
	{
		return FHermiteSpline<float>.CreateCatmullRom(Instance, keys, times, loopTangents);
	}
}
