// Rewritten from libil2cpp.so 1.0.605 (FEntityActionFNodeAnimation @ 0x16c63dc..0x16c67a0, Ghidra vaddr).
using UnityEngine;

public class FEntityActionFNodeAnimation : FEntityAction
{
	protected FAnimToCurveHelper mHelper;

	protected float mPlaybackSpeed = 1f;

	protected Vector2 mPositionScale = Vector2.one;

	protected Vector2 mAnchorPosition;

	public FEntityActionFNodeAnimation(FAnimToCurveHelper helper, float playbackSpeed, Vector2 anchorPosition, Vector2 positionScale)
	{
		if (helper == null)
		{
			FAssert.Fail("Helper can't be null");
			return;
		}
		mHelper = helper;
		mPlaybackSpeed = playbackSpeed;
		mDuration = (Fix64)(mHelper.Length / playbackSpeed);
		mPositionScale = positionScale;
		mAnchorPosition = anchorPosition;
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		if (!(mElapsed >= mDuration))
		{
			FNode fNode = mTarget as FNode;
			if (fNode != null)
			{
				float time = (float)mElapsed * mPlaybackSpeed;
				fNode.Alpha = mHelper.ColorA.Evaluate(time);
				fNode.Scale = mHelper.ScaleX.Evaluate(time);
				float x = mHelper.PositionX.Evaluate(time);
				float y = mHelper.PositionY.Evaluate(time);
				fNode.Position = new Vector2(x * mPositionScale.x + mAnchorPosition.x, y * mPositionScale.y + mAnchorPosition.y);
				return true;
			}
			FAssert.Fail("FEntityActionFNodeAnimation needs to be on FNode");
		}
		return false;
	}
}
