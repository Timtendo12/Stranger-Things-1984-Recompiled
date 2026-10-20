// Rewritten from libil2cpp.so 1.0.605 (FEntityActionPlayRawAnim @ 0x164d3bc..0x164d6a8, Ghidra vaddr).
public class FEntityActionPlayRawAnim : FEntityAction
{
	protected string mAnimName;

	protected bool mRecursive;

	protected bool mZeroDuration;

	public string AnimName => mAnimName;

	public bool Recursive => mRecursive;

	public bool ZeroDuration => mZeroDuration;

	public FEntityActionPlayRawAnim(string animName, bool recursive, bool zeroDuration = false)
	{
		mAnimName = animName;
		mRecursive = recursive;
		mZeroDuration = zeroDuration;
	}

	public override void OnSetTarget()
	{
		if (!mZeroDuration)
		{
			mDuration = (Fix64)mTarget.GetAnimLength(mAnimName, mRecursive);
		}
	}

	public override void OnStart()
	{
		mTarget.PlayAnim(mAnimName, mRecursive);
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		if (!mZeroDuration)
		{
			return !(mElapsed >= mDuration);
		}
		return false;
	}
}
