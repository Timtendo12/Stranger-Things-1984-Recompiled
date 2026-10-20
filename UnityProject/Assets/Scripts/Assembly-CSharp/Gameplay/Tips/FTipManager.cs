using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FTipManager @ 0x17d268c..0x17d2ccc, Ghidra vaddr).
public class FTipManager
{
	protected static List<FTip> mTips = new List<FTip>();

	protected static int mCurrentIndex = 0;

	public static bool Init()
	{
		mCurrentIndex = FApp.PlayerData.TipIndex;
		int i = 0;
		while (true)
		{
			string name = "Tip." + string.Format("{0:D4}", i);
			string details = FLoc.GetString(name, true);
			if (!details.Valid())
			{
				break;
			}
			FTip tip = new FTip(mTips.Count);
			tip.Name = name;
			tip.Details = details;
			mTips.Add(tip);
			i++;
		}
		return true;
	}

	public static FTip GetNextTip()
	{
		if (mTips.Count == 0)
		{
			return null;
		}
		mCurrentIndex++;
		if (mCurrentIndex >= mTips.Count)
		{
			mCurrentIndex = 0;
		}
		FApp.PlayerData.TipIndex = mCurrentIndex;
		return mTips[mCurrentIndex];
	}

	public static FTip GetRandomTip()
	{
		if (mTips.Count == 0)
		{
			return null;
		}
		return mTips[FRandom.Generic.RangeIntExclusive(0, mTips.Count)];
	}
}
