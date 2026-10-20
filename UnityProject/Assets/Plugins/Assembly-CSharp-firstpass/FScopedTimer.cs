using System;
using System.Diagnostics;

// Rewritten from libil2cpp.so 1.0.605 (FScopedTimer @ 0x1667ac8..0x1667bf8, Ghidra vaddr).
public class FScopedTimer : IDisposable
{
	protected Stopwatch mStopwatch;

	protected Action<double> mOnComplete;

	public FScopedTimer(Action<double> onComplete)
	{
		mOnComplete = onComplete;
		mStopwatch = Stopwatch.StartNew();
	}

	public void Dispose()
	{
		mStopwatch.Stop();
		double totalSeconds = mStopwatch.Elapsed.TotalSeconds;
		if (mOnComplete != null)
		{
			mOnComplete(totalSeconds);
		}
	}
}
