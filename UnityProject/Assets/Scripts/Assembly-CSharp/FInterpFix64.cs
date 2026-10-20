using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FInterpFix64 @ 0x1789a9c..0x1789c78, Ghidra vaddr).
public class FInterpFix64
{
	protected float mLast;

	protected Fix64 mCurrent;

	public Fix64 Current
	{
		get
		{
			return mCurrent;
		}
		set
		{
			mCurrent = value;
		}
	}

	public float Last => mLast;

	public FInterpFix64()
	{
	}

	public FInterpFix64(float last, Fix64 current)
	{
		mLast = last;
		mCurrent = current;
	}

	public float GetInterpolated(float t)
	{
		float num = (float)mCurrent;
		return mLast + Mathf.Clamp01(t) * (num - mLast);
	}

	public static implicit operator Fix64(FInterpFix64 v)
	{
		return v.mCurrent;
	}

	public void SetImmediate(Fix64 v)
	{
		mCurrent = v;
		mLast = (float)v;
	}

	public void Set(Fix64 v)
	{
		mCurrent = v;
	}

	public void Advance()
	{
		mLast = (float)mCurrent;
	}
}
