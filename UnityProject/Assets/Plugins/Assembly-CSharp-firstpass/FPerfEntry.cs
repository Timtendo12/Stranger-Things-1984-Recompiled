using System;
using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FPerfEntry @ 0x16623ac..0x1662c64, Ghidra vaddr).
public class FPerfEntry
{
	public const int cFrameHistory = 1000;

	protected CircularBuffer<long> mFrameTicks = new CircularBuffer<long>(1000, allowOverflow: true);

	protected CircularBuffer<long> mFrameCalls = new CircularBuffer<long>(1000, allowOverflow: true);

	protected CircularBuffer<long> mFrameMaxTicks = new CircularBuffer<long>(1000, allowOverflow: true);

	protected CircularBuffer<long> mFrameLengths = new CircularBuffer<long>(1000, allowOverflow: true);

	protected List<FPerfInterval> mIntervals = new List<FPerfInterval>
	{
		new FPerfInterval(100),
		new FPerfInterval(1000)
	};

	public string Name { get; set; }

	public int InEventRefCount { get; set; }

	public long TotalTicks { get; set; }

	public long CurrentFrameTicks { get; set; }

	public long OverallMaxTicks { get; set; }

	public long CurrentFrameMaxTicks { get; set; }

	public int TotalCalls { get; set; }

	public int CurrentFrameCalls { get; set; }

	public List<FPerfInterval> Intervals => mIntervals;

	public void AddElapsed(long elapsedTicks)
	{
		CurrentFrameTicks += elapsedTicks;
		TotalTicks += elapsedTicks;
		OverallMaxTicks = Math.Max(OverallMaxTicks, elapsedTicks);
		CurrentFrameMaxTicks = Math.Max(CurrentFrameMaxTicks, elapsedTicks);
		TotalCalls++;
		CurrentFrameCalls++;
	}

	public void FrameEnd(long frameTicks)
	{
		foreach (FPerfInterval mInterval in mIntervals)
		{
			int maxFrameCount = mInterval.MaxFrameCount;
			if (mInterval.FrameCount < maxFrameCount)
			{
				mInterval.FrameCount++;
			}
			if (maxFrameCount <= mFrameTicks.Size)
			{
				// drop the oldest sample of this interval's window
				mInterval.TotalFrameLengthTicks -= mFrameLengths.Peek(maxFrameCount);
				mInterval.TotalTicks -= mFrameTicks.Peek(maxFrameCount);
				mInterval.TotalCalls -= mFrameCalls.Peek(maxFrameCount);
				if (mInterval.MaxTicks == mFrameMaxTicks.Peek(maxFrameCount))
				{
					long num = CurrentFrameMaxTicks;
					for (int num2 = maxFrameCount - 1; num2 >= 1; num2--)
					{
						num = Math.Max(num, mFrameMaxTicks.Peek(num2));
					}
					mInterval.MaxTicks = num;
				}
			}
			mInterval.TotalFrameLengthTicks += frameTicks;
			mInterval.TotalCalls += CurrentFrameCalls;
			mInterval.TotalTicks += CurrentFrameTicks;
			mInterval.MaxTicks = Math.Max(mInterval.MaxTicks, CurrentFrameMaxTicks);
		}
		mFrameLengths.Put(frameTicks);
		mFrameTicks.Put(CurrentFrameTicks);
		mFrameCalls.Put(CurrentFrameCalls);
		mFrameMaxTicks.Put(CurrentFrameMaxTicks);
		CurrentFrameTicks = 0L;
		CurrentFrameCalls = 0;
		CurrentFrameMaxTicks = 0L;
	}

	public void Log()
	{
		FLog.Log("==== " + Name + " ====");
		foreach (FPerfInterval mInterval in mIntervals)
		{
			if (mInterval.FrameCount > 0)
			{
				// TODO(port): the shipped line format (10 string pieces, 0x1662874) is not reproduced exactly [left: debug perf log line]
				FLog.Log("  last " + mInterval.MaxFrameCount + " frames: avg " + mInterval.AverageMSPerFrame.ToString("F3") + "ms/frame, " + mInterval.AverageCallsPerFrame.ToString("F2") + " calls/frame, max " + mInterval.MaxMS.ToString("F3") + "ms");
			}
		}
	}
}
