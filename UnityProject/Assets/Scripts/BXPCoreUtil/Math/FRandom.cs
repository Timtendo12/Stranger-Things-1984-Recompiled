using System.Linq;
using System;
using System.Collections.Generic;
using NPack;

// Rewritten from libil2cpp.so 1.0.605 (FRandom @ 0x188d8a0..0x188e150, Ghidra vaddr).
[SaveVersion(0)]
public class FRandom
{
	protected MersenneTwister mMT;

	public static FRandom Generic = new FRandom();

	protected Fix64 FixRangeFactor;

	public FRandom()
	{
		FixRangeFactor = Fix64.One / (Fix64)2147483647L;
		mMT = new MersenneTwister();
	}

	public FRandom(int seed)
	{
		FixRangeFactor = Fix64.One / (Fix64)2147483647L;
		SetSeed(seed);
	}

	public void SetSeed(int seed)
	{
		mMT = ((seed < 0) ? new MersenneTwister() : new MersenneTwister(seed));
	}

	public virtual uint RangeUInt(uint minInclusive, uint maxInclusive)
	{
		return mMT.NextUInt32(minInclusive, maxInclusive + 1);
	}

	public virtual int RangeInt(int minInclusive, int maxInclusive)
	{
		return mMT.Next(minInclusive, maxInclusive + 1);
	}

	public virtual int RangeIntExclusive(int minInclusive, int maxExclusive)
	{
		return mMT.Next(minInclusive, maxExclusive);
	}

	public virtual float RangeFloat(float min, float max)
	{
		return (max - min) * ((float)mMT.NextUInt32() / 4.2949673E+09f) + min;
	}

	public virtual Fix64 RangeFix64(Fix64 min, Fix64 max)
	{
		Fix64 fix = (Fix64)(long)(mMT.NextUInt32() & 0x7FFFFFFF) * FixRangeFactor;
		return min + (max - min) * fix;
	}

	public virtual double RangeDouble(double min, double max)
	{
		return (max - min) * mMT.NextDouble(includeOne: true) + min;
	}

	public virtual bool RandBool()
	{
		return RangeInt(0, 1) == 1;
	}

	public T GetRandomElement<T>(IEnumerable<T> enumerable)
	{
		if (enumerable == null)
		{
			return default(T);
		}
		int count = enumerable.Count();
		if (count == 0)
		{
			return default(T);
		}
		return enumerable.ElementAt(RangeIntExclusive(0, count));
	}

	public uint[] CopyState()
	{
		uint[] state = mMT.GetState();
		uint[] array = new uint[state.Length + 1];
		Array.Copy(state, array, state.Length);
		array[state.Length] = (uint)mMT.GetIndex();
		return array;
	}

	public void SetState(uint[] state)
	{
		uint index = state[state.Length - 1];
		mMT = new MersenneTwister(state, (short)index);
	}

	public virtual uint RangeUInt(uint minInclusive, uint maxInclusive, string infoString)
	{
		return RangeUInt(minInclusive, maxInclusive);
	}

	public virtual int RangeInt(int minInclusive, int maxInclusive, string infoString)
	{
		return RangeInt(minInclusive, maxInclusive);
	}

	public virtual int RangeIntExclusive(int minInclusive, int maxExclusive, string infoString)
	{
		return RangeIntExclusive(minInclusive, maxExclusive);
	}

	public virtual Fix64 RangeFix64(Fix64 min, Fix64 max, string infoString)
	{
		return RangeFix64(min, max);
	}

	public virtual int NextInt()
	{
		return mMT.Next();
	}
}
