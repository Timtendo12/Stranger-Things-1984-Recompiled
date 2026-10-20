using System;

// Rewritten from libil2cpp.so 1.0.605 (FScopedPerf @ 0x1667a94..0x1667ac8, Ghidra vaddr).
public class FScopedPerf : IDisposable
{
	protected string mName;

	public FScopedPerf(string name)
	{
		mName = name;
	}

	public void Dispose()
	{
		// The shipped body is empty (FPerf.End call compiled out).
	}
}
