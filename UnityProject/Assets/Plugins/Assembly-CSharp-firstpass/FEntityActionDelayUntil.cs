using System;

// Rewritten from libil2cpp.so 1.0.605 (FEntityActionDelayUntil @ 0x164db44..0x164dbe4, Ghidra vaddr).
public class FEntityActionDelayUntil : FEntityAction
{
	protected Func<bool> mConditionFunc;

	public FEntityActionDelayUntil(Func<bool> conditionFunc)
	{
		mConditionFunc = conditionFunc;
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		return !mConditionFunc();
	}
}
