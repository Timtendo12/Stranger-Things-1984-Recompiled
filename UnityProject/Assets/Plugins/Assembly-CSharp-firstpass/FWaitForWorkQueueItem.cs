using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FWaitForWorkQueueItem @ 0x1681578..0x1681690, Ghidra vaddr).
public class FWaitForWorkQueueItem : CustomYieldInstruction
{
	public bool ShouldKeepWaiting = true;

	public override bool keepWaiting => ShouldKeepWaiting;

	public FWaitForWorkQueueItem(Action workAction)
	{
		FUnityServices.WorkQueue.DoWork(workAction, delegate
		{
			ShouldKeepWaiting = false;
		});
	}
}
