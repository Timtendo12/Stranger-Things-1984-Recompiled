using System;
using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FDispatcher @ 0x186a818..0x186ae28, Ghidra vaddr).
public class FDispatcher
{
	protected List<Action> mPendingActions = new List<Action>();

	protected List<Action> mPendingActions2 = new List<Action>();

	public bool Immediate { get; set; }

	public void Invoke(Action action)
	{
		if (Immediate)
		{
			action();
			return;
		}
		lock (mPendingActions)
		{
			mPendingActions.Add(action);
		}
	}

	public void ExecutePendingActions(bool loopTilQueueIsEmpty)
	{
		lock (mPendingActions)
		{
			do
			{
				if (mPendingActions.Count < 1)
				{
					break;
				}
				List<Action> list = mPendingActions;
				mPendingActions = mPendingActions2;
				mPendingActions2 = list;
				foreach (Action item in mPendingActions2)
				{
					item();
				}
				mPendingActions2.Clear();
			}
			while (loopTilQueueIsEmpty);
		}
	}

	public int CountPendingActions()
	{
		lock (mPendingActions)
		{
			return mPendingActions.Count;
		}
	}
}
