// Rewritten from libil2cpp.so 1.0.605 (FEntityActionDelayFrame @ 0x164fe48..0x164fee8, Ghidra vaddr).
public class FEntityActionDelayFrame : FEntityAction
{
	protected int mFramesToDelay = 1;

	protected int mCurrentFrame;

	public int FramesToDelay => mFramesToDelay;

	public int CurrentFrame => mCurrentFrame;

	public FEntityActionDelayFrame(int numFramesToDelay)
	{
		mFramesToDelay = numFramesToDelay;
		mCurrentFrame = 0;
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		bool result = mCurrentFrame < mFramesToDelay;
		if (result)
		{
			mCurrentFrame++;
		}
		return result;
	}
}
