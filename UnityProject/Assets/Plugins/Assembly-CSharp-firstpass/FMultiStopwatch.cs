using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

// Rewritten from libil2cpp.so 1.0.605 (FMultiStopwatch @ 0x166b3ec..0x166c0d0, Ghidra vaddr).
// Start/Stop/Restart/LogStopwatchData are [Conditional("STOPWATCH")] in the shipped build.
public class FMultiStopwatch
{
	public enum Sorting
	{
		cUnsorted = 0,
		cAscending = 1,
		cDescending = 2
	}

	private string mStopwatchName;

	private Dictionary<string, Stopwatch> mStopwatches;

	private Dictionary<string, List<double>> mSampleTimes;

	private Dictionary<string, double> mCumulativeTimes;

	public FMultiStopwatch(string name)
	{
		mStopwatchName = name;
		mStopwatches = new Dictionary<string, Stopwatch>();
		mSampleTimes = new Dictionary<string, List<double>>();
		mCumulativeTimes = new Dictionary<string, double>();
	}

	private void RecordSample(string name, Stopwatch stopwatch)
	{
		if (!mSampleTimes.TryGetValue(name, out var value))
		{
			value = new List<double>();
			mSampleTimes[name] = value;
		}
		double totalMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
		value.Add(totalMilliseconds);
		double value2 = 0.0;
		mCumulativeTimes.TryGetValue(name, out value2);
		mCumulativeTimes[name] = totalMilliseconds + value2;
		stopwatch.Reset();
	}

	[Conditional("STOPWATCH")]
	public void Start(string name)
	{
		if (!mStopwatches.TryGetValue(name, out var value))
		{
			value = new Stopwatch();
			mStopwatches[name] = value;
		}
		value.Start();
	}

	[Conditional("STOPWATCH")]
	public void Stop(string name)
	{
		if (mStopwatches.TryGetValue(name, out var value))
		{
			value.Stop();
			RecordSample(name, value);
		}
	}

	[Conditional("STOPWATCH")]
	public void Restart(string name)
	{
		if (!mStopwatches.TryGetValue(name, out var value))
		{
			value = new Stopwatch();
			mStopwatches[name] = value;
		}
		else
		{
			value.Stop();
			RecordSample(name, value);
		}
		value.Start();
	}

	[Conditional("STOPWATCH")]
	public void LogStopwatchData(Sorting sort)
	{
		// TODO(port): the shipped summary layout (0x166bb2c) is only approximated [left: debug perf summary text]
		StringBuilder stringBuilder = new StringBuilder();
		List<KeyValuePair<string, double>> list = new List<KeyValuePair<string, double>>(mCumulativeTimes);
		if (sort == Sorting.cAscending)
		{
			list.Sort((KeyValuePair<string, double> a, KeyValuePair<string, double> b) => a.Value.CompareTo(b.Value));
		}
		else if (sort == Sorting.cDescending)
		{
			list.Sort((KeyValuePair<string, double> a, KeyValuePair<string, double> b) => b.Value.CompareTo(a.Value));
		}
		stringBuilder.AppendFormat("==== BEGIN MULTI STOPWATCH SUMMARY: {0} ====", mStopwatchName);
		stringBuilder.AppendLine();
		foreach (KeyValuePair<string, double> item in list)
		{
			int num = (mSampleTimes.ContainsKey(item.Key) ? mSampleTimes[item.Key].Count : 0);
			stringBuilder.AppendFormat("{0} {1} ({2} samples)", item.Key, item.Value, num);
			stringBuilder.AppendLine();
		}
		stringBuilder.AppendFormat("==== END MULTI STOPWATCH SUMMARY: {0} ====", mStopwatchName);
		FLog.FinalUnityLog(stringBuilder.ToString());
	}
}
