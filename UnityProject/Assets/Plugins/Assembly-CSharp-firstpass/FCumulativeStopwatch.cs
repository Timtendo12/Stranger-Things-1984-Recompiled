using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

// Rewritten from libil2cpp.so 1.0.605 (FCumulativeStopwatch @ 0x166a99c..0x166b3ec, Ghidra vaddr).
// AddTimingInterval/Restart/LogStopwatchData are [Conditional("STOPWATCH")] in the shipped build.
public class FCumulativeStopwatch
{
	public enum Sorting
	{
		cUnsorted = 0,
		cAscending = 1,
		cDescending = 2
	}

	private string mStopwatchName;

	private Stopwatch mStopwatch;

	private Dictionary<string, double> mCumulativeTimes;

	private double mCumulativeTime;

	public FCumulativeStopwatch(string name)
	{
		mStopwatchName = name;
		mStopwatch = new Stopwatch();
		mCumulativeTimes = new Dictionary<string, double>();
		mCumulativeTime = 0.0;
		mStopwatch.Start();
	}

	[Conditional("STOPWATCH")]
	public void AddTimingInterval(string str)
	{
		mStopwatch.Stop();
		double totalMilliseconds = mStopwatch.Elapsed.TotalMilliseconds;
		double value = 0.0;
		mCumulativeTime += totalMilliseconds;
		mCumulativeTimes.TryGetValue(str, out value);
		mCumulativeTimes[str] = totalMilliseconds + value;
		mStopwatch.Reset();
		mStopwatch.Start();
	}

	[Conditional("STOPWATCH")]
	public void Restart(string name)
	{
		mStopwatchName = name;
		mCumulativeTimes.Clear();
		mCumulativeTime = 0.0;
		mStopwatch.Reset();
		mStopwatch.Start();
	}

	[Conditional("STOPWATCH")]
	public void LogStopwatchData(Sorting sort)
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<KeyValuePair<string, double>> list = new List<KeyValuePair<string, double>>(mCumulativeTimes.Count);
		int val = 0;
		foreach (KeyValuePair<string, double> mCumulativeTime2 in mCumulativeTimes)
		{
			list.Add(mCumulativeTime2);
			val = System.Math.Max(val, mCumulativeTime2.Key.Length);
		}
		switch (sort)
		{
		case Sorting.cAscending:
			list.Sort((KeyValuePair<string, double> a, KeyValuePair<string, double> b) => a.Value.CompareTo(b.Value));
			break;
		case Sorting.cDescending:
			list.Sort((KeyValuePair<string, double> a, KeyValuePair<string, double> b) => b.Value.CompareTo(a.Value));
			break;
		}
		stringBuilder.AppendLine("=====================================================");
		stringBuilder.AppendFormat("==== BEGIN CUMULATIVE STOPWATCH SUMMARY: {0} ====", mStopwatchName);
		stringBuilder.AppendLine("=====================================================");
		stringBuilder.AppendFormat("{0} {1} ({2:P2})", "Total".PadRight(val), mCumulativeTime, mCumulativeTime / mCumulativeTime);
		for (int i = 0; i < list.Count; i++)
		{
			stringBuilder.AppendFormat("{0} {1} ({2:P2})", list[i].Key.PadRight(val), list[i].Value, list[i].Value / mCumulativeTime);
		}
		stringBuilder.AppendLine("=====================================================");
		stringBuilder.AppendFormat("==== END CUMULATIVE STOPWATCH SUMMARY: {0} ====", mStopwatchName);
		stringBuilder.AppendLine("=====================================================");
		FLog.FinalUnityLog(stringBuilder.ToString());
	}
}
