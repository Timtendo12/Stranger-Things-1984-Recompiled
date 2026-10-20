using System.Collections.Generic;

public class FFixHermiteSpline<T> where T : IFixSplineInterp<T>
{
	protected List<FFixHermiteCurve<T>> mCurves;

	protected List<Fix64> mTimes;

	protected Fix64 mStartTime;

	public int CurveCount => 0;

	public Fix64 StartTime => default;

	public Fix64 EndTime => default;

	public Fix64 TotalTime => default;

	public void SetTCB(T[] keys, Fix64[] times, Fix64[] tensions, Fix64[] continuities, Fix64[] biases, bool loopTangents)
	{
	}

	public static FFixHermiteSpline<T> CreateCatmullRom(T[] keys, Fix64[] times, bool loopTangents)
	{
		return null;
	}

	public void SetCatmullRom(T[] keys, Fix64[] times, bool loopTangents)
	{
	}

	public void Set(T[] keys, Fix64[] times, T[] inTangents, T[] outTangents)
	{
	}

	public FFixHermiteCurve<T> GetCurveAndParam(Fix64 time, out Fix64 param)
	{
		param = default;
		return null;
	}

	public T Evaluate(Fix64 time)
	{
		return default;
	}

	public T EvaluateTangent(Fix64 time)
	{
		return default;
	}

	public T EvaluateSecondDerivative(Fix64 time)
	{
		return default;
	}

	public void BinWrite(FBinWriter writer)
	{
	}

	public void BinRead(FBinReader reader)
	{
	}
}
