public class FFixHermiteCurve<T> where T : IFixSplineInterp<T>
{
	protected T[] mControls;

	protected Fix64 Six;

	protected Fix64 Three;

	public T[] Controls => null;

	public FFixHermiteCurve()
	{
	}

	public FFixHermiteCurve(T startKey, T endKey, T startTan, T endTan)
	{
	}

	public static FFixHermiteCurve<T> MakeTCB(T previousKey, T startKey, T endKey, T nextKey, Fix64 previousT, Fix64 startT, Fix64 endT, Fix64 nextT, Fix64 startTension, Fix64 startContinuity, Fix64 startBias, Fix64 endTension, Fix64 endContinuity, Fix64 endBias)
	{
		return null;
	}

	public static FFixHermiteCurve<T> MakeCatmullRom(T previousKey, T startKey, T endKey, T nextKey, Fix64 previousT, Fix64 startT, Fix64 endT, Fix64 nextT)
	{
		return null;
	}

	public T Evaluate(Fix64 t)
	{
		return default;
	}

	public T EvaluateTangent(Fix64 t)
	{
		return default;
	}

	public T EvaluateSecondDerivative(Fix64 t)
	{
		return default;
	}

	public void Set(T startKey, T endKey, T startTan, T endTan)
	{
	}

	public void SetTCB(T previousKey, T startKey, T endKey, T nextKey, Fix64 previousT, Fix64 startT, Fix64 endT, Fix64 nextT, Fix64 startTension, Fix64 startContinuity, Fix64 startBias, Fix64 endTension, Fix64 endContinuity, Fix64 endBias)
	{
	}

	public void SetCatmullRom(T previousKey, T startKey, T endKey, T nextKey, Fix64 previousT, Fix64 startT, Fix64 endT, Fix64 nextT)
	{
	}

	public void BinWrite(FBinWriter writer)
	{
	}

	public void BinRead(FBinReader reader)
	{
	}
}
