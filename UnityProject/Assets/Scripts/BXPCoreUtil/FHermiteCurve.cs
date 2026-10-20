// Rewritten from libil2cpp.so 1.0.605 (FHermiteCurve<> @ 0x1f6e488..0x1f6fff4, Ghidra vaddr).
public class FHermiteCurve<T>
{
	protected T[] mControls;

	public ISplineHelper<T> Helper;

	public T[] Controls => mControls;

	public FHermiteCurve(ISplineHelper<T> helper)
	{
		mControls = new T[4];
		Helper = helper;
	}

	public FHermiteCurve(T startKey, T endKey, T startTan, T endTan)
	{
		mControls = new T[4];
		Set(startKey, endKey, startTan, endTan);
	}

	public static FHermiteCurve<T> MakeTCB(ISplineHelper<T> helper, T previousKey, T startKey, T endKey, T nextKey, float previousT, float startT, float endT, float nextT, float startTension, float startContinuity, float startBias, float endTension, float endContinuity, float endBias)
	{
		FHermiteCurve<T> fHermiteCurve = new FHermiteCurve<T>(helper);
		fHermiteCurve.SetTCB(previousKey, startKey, endKey, nextKey, previousT, startT, endT, nextT, startTension, startContinuity, startBias, endTension, endContinuity, endBias);
		return fHermiteCurve;
	}

	public static FHermiteCurve<T> MakeCatmullRom(ISplineHelper<T> helper, T previousKey, T startKey, T endKey, T nextKey, float previousT, float startT, float endT, float nextT)
	{
		FHermiteCurve<T> fHermiteCurve = new FHermiteCurve<T>(helper);
		fHermiteCurve.SetCatmullRom(previousKey, startKey, endKey, nextKey, previousT, startT, endT, nextT);
		return fHermiteCurve;
	}

	public T Evaluate(float t)
	{
		return Helper.Add(mControls[0], Helper.MultiplyScalar(t, Helper.Add(mControls[1], Helper.MultiplyScalar(t, Helper.Add(mControls[2], Helper.MultiplyScalar(t, mControls[3]))))));
	}

	public T EvaluateTangent(float t)
	{
		return Helper.Add(mControls[1], Helper.MultiplyScalar(t, Helper.Add(Helper.MultiplyScalar(2f, mControls[2]), Helper.MultiplyScalar(t * 3f, mControls[3]))));
	}

	public T EvaluateSecondDerivative(float t)
	{
		return Helper.Add(Helper.MultiplyScalar(2f, mControls[2]), Helper.MultiplyScalar(t * 6f, mControls[3]));
	}

	public void Set(T startKey, T endKey, T startTan, T endTan)
	{
		T val = Helper.Subtract(endKey, startKey);
		mControls[0] = startKey;
		mControls[1] = startTan;
		mControls[2] = Helper.Subtract(Helper.Subtract(Helper.MultiplyScalar(3f, val), Helper.MultiplyScalar(2f, startTan)), endTan);
		mControls[3] = Helper.Add(Helper.Add(Helper.MultiplyScalar(-2f, val), startTan), endTan);
	}

	public void SetTCB(T previousKey, T startKey, T endKey, T nextKey, float previousT, float startT, float endT, float nextT, float startTension, float startContinuity, float startBias, float endTension, float endContinuity, float endBias)
	{
		T a = Helper.Subtract(endKey, startKey);
		T b = Helper.Subtract(startKey, previousKey);
		float num = (1f - startTension) * 0.5f;
		T a2 = Helper.MultiplyScalar(num * (1f - startContinuity) * (1f - startBias), a);
		T b2 = Helper.MultiplyScalar(num * (startContinuity + 1f) * (startBias + 1f), b);
		T val = Helper.Add(a2, b2);
		float num2 = endT - startT;
		float a3 = 1f;
		if (FMath.Epsilon < endT - previousT)
		{
			a3 = num2 * 2f / (endT - previousT);
		}
		T startTan = Helper.MultiplyScalar(a3, val);
		T a4 = Helper.Subtract(nextKey, endKey);
		float num3 = (1f - endTension) * 0.5f;
		T a5 = Helper.MultiplyScalar(num3 * (endContinuity + 1f) * (1f - endBias), a4);
		T b3 = Helper.MultiplyScalar(num3 * (1f - endContinuity) * (endBias + 1f), a);
		T val2 = Helper.Add(a5, b3);
		float a6 = 1f;
		if (FMath.Epsilon < nextT - startT)
		{
			a6 = num2 * 2f / (nextT - startT);
		}
		T endTan = Helper.MultiplyScalar(a6, val2);
		Set(startKey, endKey, startTan, endTan);
	}

	public void SetCatmullRom(T previousKey, T startKey, T endKey, T nextKey, float previousT, float startT, float endT, float nextT)
	{
		SetTCB(previousKey, startKey, endKey, nextKey, previousT, startT, endT, nextT, 0f, 0f, 0f, 0f, 0f, 0f);
	}

	public void BinWrite(FBinWriter writer)
	{
		for (int i = 0; i < mControls.Length; i++)
		{
			Helper.BinWrite(writer, mControls[i]);
		}
	}

	public void BinRead(FBinReader reader)
	{
		for (int i = 0; i < mControls.Length; i++)
		{
			mControls[i] = Helper.BinRead(reader);
		}
	}
}
