// Rewritten from libil2cpp.so 1.0.605 (FEntityActionDelay @ 0x164d968..0x164db44, Ghidra vaddr).
public class FEntityActionDelay : FEntityAction
{
	protected static bool sPaused;

	protected bool mCheckIfPaused;

	public static void SetPaused(bool which)
	{
		sPaused = which;
	}

	public FEntityActionDelay(float delayTime, bool checkIfPaused = false)
	{
		mDuration = (Fix64)delayTime;
		mCheckIfPaused = checkIfPaused;
	}

	public override Fix64 Update(Fix64 deltaTime)
	{
		if (mCheckIfPaused && sPaused)
		{
			return deltaTime;
		}
		return base.Update(deltaTime);
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		return !(mElapsed >= mDuration);
	}
}
