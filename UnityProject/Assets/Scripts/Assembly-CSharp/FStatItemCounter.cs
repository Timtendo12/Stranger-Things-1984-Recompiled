using Newtonsoft.Json;

// Rewritten from libil2cpp.so 1.0.605 (FStatItemCounter<T> @ 0x1faf348..0x1faf41c, Ghidra vaddr).
public abstract class FStatItemCounter<T> : FStatItem
{
	protected T mLifetimeTotal;

	protected T mLifetimeLevelMax;

	protected T mLifetimeSessionMax;

	protected T mSessionTotal;

	protected T mSessionLevelMax;

	protected T mCurrent;

	protected int mLifetimeLevelCount;

	protected int mLifetimeSessionCount;

	protected int mSessionLevelCount;

	[JsonProperty]
	public T LifetimeTotal
	{
		get
		{
			return mLifetimeTotal;
		}
		protected set
		{
			mLifetimeTotal = value;
		}
	}

	[JsonProperty]
	public T LifetimeLevelMax
	{
		get
		{
			return mLifetimeLevelMax;
		}
		protected set
		{
			mLifetimeLevelMax = value;
		}
	}

	[JsonProperty]
	public T LifetimeSessionMax
	{
		get
		{
			return mLifetimeSessionMax;
		}
		protected set
		{
			mLifetimeSessionMax = value;
		}
	}

	[JsonProperty]
	public T SessionTotal
	{
		get
		{
			return mSessionTotal;
		}
		protected set
		{
			mSessionTotal = value;
		}
	}

	[JsonProperty]
	public T SessionLevelMax
	{
		get
		{
			return mSessionLevelMax;
		}
		protected set
		{
			mSessionLevelMax = value;
		}
	}

	[JsonProperty]
	public T Current
	{
		get
		{
			return mCurrent;
		}
		protected set
		{
			mCurrent = value;
		}
	}

	[JsonProperty]
	public int LifetimeLevelCount
	{
		get
		{
			return mLifetimeLevelCount;
		}
		protected set
		{
			mLifetimeLevelCount = value;
		}
	}

	[JsonProperty]
	public int LifetimeSessionCount
	{
		get
		{
			return mLifetimeSessionCount;
		}
		protected set
		{
			mLifetimeSessionCount = value;
		}
	}

	[JsonProperty]
	public int SessionLevelCount
	{
		get
		{
			return mSessionLevelCount;
		}
		protected set
		{
			mSessionLevelCount = value;
		}
	}

	public override void LevelStart()
	{
		mLifetimeLevelCount++;
		mSessionLevelCount++;
		InternalLevelStart();
	}

	public override void SessionStart()
	{
		mLifetimeSessionCount++;
		InternalSessionStart();
	}

	public abstract void Add(T amount);

	public abstract void Clear();

	public abstract void InternalLevelStart();

	public abstract void InternalSessionStart();
}
