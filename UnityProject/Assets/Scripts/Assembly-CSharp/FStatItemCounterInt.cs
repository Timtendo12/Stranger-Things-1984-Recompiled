using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FStatItemCounterInt @ 0x17cf930..0x17d02d0, Ghidra vaddr).
public class FStatItemCounterInt : FStatItemCounter<int>
{
	public override void Add(int amount)
	{
		mCurrent += amount;
		if (mLifetimeLevelMax < mCurrent)
		{
			mLifetimeLevelMax = mCurrent;
		}
		mSessionTotal += amount;
		if (mLifetimeSessionMax < mSessionTotal)
		{
			mLifetimeSessionMax = mSessionTotal;
		}
		mLifetimeTotal += amount;
		FireChangeEvent();
	}

	public override void InternalLevelStart()
	{
		mCurrent = 0;
	}

	public override void InternalSessionStart()
	{
		mSessionTotal = 0;
		mSessionLevelMax = 0;
		mSessionLevelCount = 0;
	}

	public override void Clear()
	{
		mSessionLevelCount = 0;
		mLifetimeSessionMax = 0;
		mLifetimeTotal = 0;
		mLifetimeLevelCount = 0;
		mLifetimeLevelMax = 0;
		mSessionTotal = 0;
		mSessionLevelMax = 0;
	}

	public override void SaveXml(XmlElement root)
	{
		root.AddAttribute("Current", mCurrent);
		root.AddAttribute("SessionTotal", mSessionTotal);
		root.AddAttribute("SessionLevelMax", mSessionLevelMax);
		root.AddAttribute("SessionLevelCount", mSessionLevelCount);
		root.AddAttribute("LifetimeTotal", mLifetimeTotal);
		root.AddAttribute("LifetimeLevelMax", mLifetimeLevelMax);
		root.AddAttribute("LifetimeSessionMax", mLifetimeSessionMax);
		root.AddAttribute("LifetimeLevelCount", mLifetimeLevelCount);
		root.AddAttribute("LifetimeSessionCount", mLifetimeSessionCount);
	}

	public override void LoadXml(XmlElement root)
	{
		mCurrent = root.GetAttrib("Current", mCurrent);
		mSessionTotal = root.GetAttrib("SessionTotal", mSessionTotal);
		mSessionLevelMax = root.GetAttrib("SessionLevelMax", mSessionLevelMax);
		mSessionLevelCount = root.GetAttrib("SessionLevelCount", mSessionLevelCount);
		mLifetimeTotal = root.GetAttrib("LifetimeTotal", mLifetimeTotal);
		mLifetimeLevelMax = root.GetAttrib("LifetimeLevelMax", mLifetimeLevelMax);
		mLifetimeSessionMax = root.GetAttrib("LifetimeSessionMax", mLifetimeSessionMax);
		mLifetimeLevelCount = root.GetAttrib("LifetimeLevelCount", mLifetimeLevelCount);
		mLifetimeSessionCount = root.GetAttrib("LifetimeSessionCount", mLifetimeSessionCount);
	}

	public override void Log()
	{
		FLog.UnityWarning(string.Concat(base.Name, "--  Current:", mCurrent.ToString(), "  SessionTotal:", mSessionTotal.ToString(), "  SessionLevelMax:", mSessionLevelMax.ToString(), "  SessionLevelCount:", mSessionLevelCount.ToString(), "  LifetimeTotal:", mLifetimeTotal.ToString(), "  LifetimeLevelMax:", mLifetimeLevelMax.ToString(), "  LifetimeSessionMax:", mLifetimeSessionMax.ToString(), "  LifetimeLevelCount:", mLifetimeLevelCount.ToString(), "  LifetimeSessionCount:", mLifetimeSessionCount.ToString()));
	}
}
