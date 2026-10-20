using System;

// Rewritten from libil2cpp.so 1.0.605 (FUnitActionCallMethod @ 0x17288f8..0x1728960, Ghidra vaddr).
public class FUnitActionCallMethod : FUnitActionWork
{
	protected Action mAction;

	public FUnitActionCallMethod(int newID, FUnit owner, Action action)
		: base(newID, owner, null)
	{
		mAction = action;
	}

	public override void DoWork()
	{
		if (mAction != null)
		{
			mAction();
		}
		SetState(States.Success);
	}
}
