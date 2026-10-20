using System.Diagnostics;

// Rewritten from libil2cpp.so 1.0.605 (FPerfTimer @ 0x1663edc..0x16641e8, Ghidra vaddr).
public class FPerfTimer
{
	protected Stopwatch mStopwatch;

	protected double mLastSplitTime;

	protected bool mWarning;

	public FPerfTimer(bool warning)
	{
		mWarning = warning;
		mStopwatch = Stopwatch.StartNew();
	}

	public void LogSplit(string splitName)
	{
		double num = mStopwatch.Elapsed.TotalSeconds - mLastSplitTime;
		string text = splitName + ": " + num.ToString();
		if (!mWarning)
		{
			FLog.UnityLog(text);
		}
		else
		{
			FLog.UnityWarning(text);
		}
		mLastSplitTime = mStopwatch.Elapsed.TotalSeconds;
	}

	public void LogTotal(string totalName)
	{
		double totalSeconds = mStopwatch.Elapsed.TotalSeconds;
		string text = totalName + ": " + totalSeconds.ToString();
		if (!mWarning)
		{
			FLog.UnityLog(text);
		}
		else
		{
			FLog.UnityWarning(text);
		}
	}
}
