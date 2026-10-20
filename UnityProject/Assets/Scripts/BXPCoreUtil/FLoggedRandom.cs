using System;

public class FLoggedRandom : FRandom
{
	public string SyncLabel;

	protected Action<string, int> LogInt;

	protected Action<string, uint> LogUint;

	protected Action<string, Fix64> LogFix64;

	protected Action<string, FixVector3> LogFixVector3;

	protected Action<string, float> LogFloat;

	protected Action<string, double> LogDouble;

	public FLoggedRandom(int seed, string label, Action<string, int> logInt, Action<string, uint> logUint, Action<string, Fix64> logFix64, Action<string, FixVector3> logFixVector3, Action<string, float> logFloat, Action<string, double> logDouble)
	{
	}

	public override uint RangeUInt(uint minInclusive, uint maxInclusive)
	{
		return 0u;
	}

	public override int RangeInt(int minInclusive, int maxInclusive)
	{
		return 0;
	}

	public override int RangeIntExclusive(int minInclusive, int maxExclusive)
	{
		return 0;
	}

	public override Fix64 RangeFix64(Fix64 min, Fix64 max)
	{
		return default;
	}

	public override FixVector3 RangeFixVector3(Fix64 min, Fix64 max)
	{
		return default;
	}

	public override uint RangeUInt(uint minInclusive, uint maxInclusive, string infoString)
	{
		return 0u;
	}

	public override int RangeInt(int minInclusive, int maxInclusive, string infoString)
	{
		return 0;
	}

	public override int RangeIntExclusive(int minInclusive, int maxExclusive, string infoString)
	{
		return 0;
	}

	public override Fix64 RangeFix64(Fix64 min, Fix64 max, string infoString)
	{
		return default;
	}

	public override FixVector3 RangeFixVector3(Fix64 min, Fix64 max, string infoString)
	{
		return default;
	}

	public override float RangeFloat(float min, float max)
	{
		return 0f;
	}

	public override double RangeDouble(double min, double max)
	{
		return 0.0;
	}

	public override int NextInt()
	{
		return 0;
	}
}
