// Rewritten from libil2cpp.so 1.0.605 (FPerfInterval @ 0x16621f4..0x16623ac, Ghidra vaddr).
public class FPerfInterval
{
	public int MaxFrameCount;

	public int FrameCount;

	public long TotalTicks;

	public long TotalCalls;

	public long MaxTicks;

	public long TotalFrameLengthTicks;

	public float AverageTicks => (float)TotalTicks / (float)TotalCalls;

	public float AverageMS => FPerf.cMSPerTick * ((float)TotalTicks / (float)TotalCalls);

	public float MaxMS => FPerf.cMSPerTick * (float)MaxTicks;

	public float AverageCallsPerFrame => (float)TotalCalls / (float)FrameCount;

	public float AverageTicksPerFrame => (float)TotalTicks / (float)FrameCount;

	public float AverageMSPerFrame => FPerf.cMSPerTick * ((float)TotalTicks / (float)FrameCount);

	public FPerfInterval(int maxFrameCount)
	{
		MaxFrameCount = maxFrameCount;
	}
}
