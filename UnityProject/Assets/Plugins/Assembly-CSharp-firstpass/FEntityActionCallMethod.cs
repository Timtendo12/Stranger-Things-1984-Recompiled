using System;

// Rewritten from libil2cpp.so 1.0.605 (FEntityActionCallMethod @ 0x164dbe4..0x164dd34, Ghidra vaddr).
public class FEntityActionCallMethod : FEntityAction
{
	protected Action mMethod;

	protected bool mCallMethodIfCanceled;

	public FEntityActionCallMethod(Action method, bool callMethodIfCanceled = false)
	{
		mMethod = method;
		mCallMethodIfCanceled = callMethodIfCanceled;
	}

	public override void OnStart()
	{
		mMethod();
	}

	public override void OnStop()
	{
		if (mCallMethodIfCanceled && mState == FEntityActionState.NotStarted)
		{
			FLog.UnityWarning("cancel call");
			mMethod();
		}
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		return false;
	}
}
