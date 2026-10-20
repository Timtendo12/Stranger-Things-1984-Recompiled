using System;
using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FHermiteSpline<> @ 0x1f77670..0x1f791d0, Ghidra vaddr).
public class FHermiteSpline<T>
{
	protected List<FHermiteCurve<T>> mCurves = new List<FHermiteCurve<T>>();

	protected List<float> mTimes = new List<float>();

	protected float mStartTime;

	public ISplineHelper<T> Helper;

	public int CurveCount => mCurves.Count;

	public float StartTime => mStartTime;

	public float EndTime => mStartTime + TotalTime;

	public float TotalTime
	{
		get
		{
			int num = mTimes.Count - 1;
			if (num >= 0)
			{
				return mTimes[num];
			}
			return 0f;
		}
	}

	public FHermiteSpline(ISplineHelper<T> helper)
	{
		Helper = helper;
	}

	public void SetTCB(T[] keys, float[] times, float[] tensions, float[] continuities, float[] biases, bool loopTangents)
	{
		mCurves.Clear();
		mTimes.Clear();
		int length = keys.Length;
		if (length != times.Length || length != tensions.Length || length != continuities.Length || length != biases.Length)
		{
			throw new ArgumentException("Input array lengths must match.");
		}
		int num = length - 1;
		FHermiteCurve<T> item;
		if (num == 1)
		{
			item = FHermiteCurve<T>.MakeTCB(Helper, keys[0], keys[0], keys[1], keys[1], times[0], times[0], times[1], times[1], tensions[0], continuities[0], biases[0], tensions[1], continuities[1], biases[1]);
		}
		else
		{
			int num2 = (loopTangents ? (length - 2) : 0);
			int num3 = (loopTangents ? 1 : num);
			mCurves.Add(FHermiteCurve<T>.MakeTCB(Helper, keys[num2], keys[0], keys[1], keys[2], times[num2], times[0], times[1], times[2], tensions[0], continuities[0], biases[0], tensions[1], continuities[1], biases[1]));
			for (int i = 0; i < length - 3; i++)
			{
				mCurves.Add(FHermiteCurve<T>.MakeTCB(Helper, keys[i], keys[i + 1], keys[i + 2], keys[i + 3], times[i], times[i + 1], times[i + 2], times[i + 3], tensions[i + 1], continuities[i + 1], biases[i + 1], tensions[i + 2], continuities[i + 2], biases[i + 2]));
			}
			item = FHermiteCurve<T>.MakeTCB(Helper, keys[length - 3], keys[length - 2], keys[num], keys[num3], times[length - 3], times[length - 2], times[num], times[num3], tensions[length - 2], continuities[length - 2], biases[length - 2], tensions[num], continuities[num], biases[num]);
		}
		mCurves.Add(item);
		BuildTimes(times);
	}

	public static FHermiteSpline<T> CreateCatmullRom(ISplineHelper<T> helper, T[] keys, float[] times, bool loopTangents)
	{
		FHermiteSpline<T> fHermiteSpline = new FHermiteSpline<T>(helper);
		fHermiteSpline.SetCatmullRom(keys, times, loopTangents);
		return fHermiteSpline;
	}

	public void SetCatmullRom(T[] keys, float[] times, bool loopTangents)
	{
		mCurves.Clear();
		mTimes.Clear();
		int length = keys.Length;
		if (length != times.Length)
		{
			throw new ArgumentException("Input array lengths must match.");
		}
		int num = length - 1;
		FHermiteCurve<T> item;
		if (num == 1)
		{
			item = FHermiteCurve<T>.MakeCatmullRom(Helper, keys[0], keys[0], keys[1], keys[1], times[0], times[0], times[1], times[1]);
		}
		else
		{
			int num2 = (loopTangents ? (length - 2) : 0);
			int num3 = (loopTangents ? 1 : num);
			mCurves.Add(FHermiteCurve<T>.MakeCatmullRom(Helper, keys[num2], keys[0], keys[1], keys[2], times[num2], times[0], times[1], times[2]));
			for (int i = 0; i < length - 3; i++)
			{
				mCurves.Add(FHermiteCurve<T>.MakeCatmullRom(Helper, keys[i], keys[i + 1], keys[i + 2], keys[i + 3], times[i], times[i + 1], times[i + 2], times[i + 3]));
			}
			item = FHermiteCurve<T>.MakeCatmullRom(Helper, keys[length - 3], keys[length - 2], keys[num], keys[num3], times[length - 3], times[length - 2], times[num], times[num3]);
		}
		mCurves.Add(item);
		BuildTimes(times);
	}

	private void BuildTimes(float[] times)
	{
		mStartTime = times[0];
		for (int i = 0; i < mCurves.Count; i++)
		{
			mTimes.Add(times[i + 1] - mStartTime);
			if (i != 0 && mTimes[i] <= mTimes[i - 1])
			{
				throw new ArgumentException("Times must be monotonically increasing.");
			}
		}
	}

	public void Set(T[] keys, float[] times, T[] inTangents, T[] outTangents)
	{
		mCurves.Clear();
		mTimes.Clear();
		int length = keys.Length;
		if (length < 2)
		{
			throw new ArgumentException("Need at least two keys");
		}
		if (length != times.Length || length != inTangents.Length || length != outTangents.Length)
		{
			throw new ArgumentException("Input array lengths must match.");
		}
		for (int i = 0; i < length - 1; i++)
		{
			float a = times[i + 1] - times[i];
			T startTan = Helper.MultiplyScalar(a, outTangents[i]);
			T endTan = Helper.MultiplyScalar(a, inTangents[i + 1]);
			mCurves.Add(new FHermiteCurve<T>(keys[i], keys[i + 1], startTan, endTan));
		}
		BuildTimes(times);
	}

	public FHermiteCurve<T> GetCurveAndParam(float time, out float param)
	{
		time -= mStartTime;
		int num = mCurves.Count - 1;
		int num2 = num;
		int i = 0;
		int num3 = mTimes.Count - 1;
		while (true)
		{
			num2 = num;
			if (i > num3)
			{
				break;
			}
			int num4;
			int num5;
			while (true)
			{
				num4 = i + num3;
				num5 = ((num4 < 0) ? (num4 + 1) : num4) >> 1;
				if (time <= mTimes[num5])
				{
					break;
				}
				i = num5 + 1;
				if (num3 <= num5)
				{
					goto done;
				}
			}
			num2 = num5;
			if ((uint)(num4 + 1) < 3u)
			{
				break;
			}
			num3 = num5 - 1;
			if (mTimes[num3] < time)
			{
				break;
			}
		}
		done:
		float num6 = ((num2 == 0) ? 0f : mTimes[num2 - 1]);
		float num7 = mTimes[num2];
		param = FMath.Clamp01((time - num6) / (num7 - num6));
		return mCurves[num2];
	}

	public T Evaluate(float time)
	{
		float param;
		return GetCurveAndParam(time, out param).Evaluate(param);
	}

	public T EvaluateTangent(float time)
	{
		float param;
		return GetCurveAndParam(time, out param).EvaluateTangent(param);
	}

	public T EvaluateSecondDerivative(float time)
	{
		float param;
		return GetCurveAndParam(time, out param).EvaluateSecondDerivative(param);
	}

	public T GetFirstKey()
	{
		return mCurves[0].Controls[0];
	}

	public T GetLastKey()
	{
		FHermiteCurve<T> fHermiteCurve = mCurves[mCurves.Count - 1];
		T val = default(T);
		for (int i = 0; i < 4; i++)
		{
			val = Helper.Add(val, fHermiteCurve.Controls[i]);
		}
		return val;
	}
}
