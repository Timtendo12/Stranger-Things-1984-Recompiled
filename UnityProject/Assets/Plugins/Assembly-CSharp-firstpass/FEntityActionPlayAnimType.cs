// Rewritten from libil2cpp.so 1.0.605 (FEntityActionPlayAnimType @ 0x164d6a8..0x164d968, Ghidra vaddr).
using UnityEngine;

public class FEntityActionPlayAnimType : FEntityAction
{
	protected int mAnimType;

	protected float mAnimSpeed;

	protected bool mRecursive;

	protected bool mSpinOnStoppedAnim;

	protected bool mZeroDuration;

	protected IAnimTagResponder mTagResponder;

	public FAnimState mAnimState;

	public int AnimType => mAnimType;

	public float AnimSpeed => mAnimSpeed;

	public bool Recursive => mRecursive;

	public bool SpinOnStoppedAnim => mSpinOnStoppedAnim;

	public bool ZeroDuration => mZeroDuration;

	public IAnimTagResponder TagResponder => mTagResponder;

	public FEntityActionPlayAnimType(int animType, float animSpeed, bool recursive, IAnimTagResponder tagResponder, bool zeroDuration = false, bool spinOnStoppedAnim = false)
	{
		mTagResponder = tagResponder;
		mAnimType = animType;
		mAnimSpeed = animSpeed;
		mRecursive = recursive;
		mZeroDuration = zeroDuration;
		mSpinOnStoppedAnim = spinOnStoppedAnim;
	}

	public FEntityActionPlayAnimType(AnimTypes animType, float animSpeed, bool recursive, IAnimTagResponder tagResponder, bool zeroDuration = false, bool spinOnStoppedAnim = false)
		: this((int)animType, animSpeed, recursive, tagResponder, zeroDuration, spinOnStoppedAnim)
	{
	}

	public override void OnSetTarget()
	{
	}

	public override void OnStart()
	{
		mAnimState = mTarget.PlayAnimType(mAnimType, mAnimSpeed, mRecursive, mTagResponder);
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		if (mZeroDuration)
		{
			return false;
		}
		if (mAnimState == null)
		{
			return false;
		}
		if (!mAnimState.Enabled)
		{
			return false;
		}
		// 0x164d91c: compares against the first static float of FMath (FMath.Epsilon)
		if (!mSpinOnStoppedAnim && mAnimState.Speed < FMath.Epsilon)
		{
			return false;
		}
		return mAnimState.Duration > mAnimState.CurrentTime;
	}
}
