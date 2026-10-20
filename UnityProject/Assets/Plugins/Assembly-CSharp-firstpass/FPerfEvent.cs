using System.Diagnostics;

// Rewritten from libil2cpp.so 1.0.605 (FPerfEvent @ 0x1662fa4..0x1663098, Ghidra vaddr).
public class FPerfEvent
{
	protected long StartTimestamp;

	public string Name { get; set; }

	public FPerfEvent(string name)
	{
		Name = name;
		StartTimestamp = Stopwatch.GetTimestamp();
	}

	public long Stop()
	{
		return Stopwatch.GetTimestamp() - StartTimestamp;
	}
}
