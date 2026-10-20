using UnityEngine;

public class FAnimRunner : MonoBehaviour
{
	public FAnimDef AnimDef;

	public IAnimTagResponder TagResponder;

	public FAnimEntry AnimEntry;

	protected float mLastUpdateAnimTime;

	protected AnimationState mAnimationState;

	protected FAnimStateAnimRunner mNodeAnimState;

	protected int mCurrentAnimTypeID;

	protected int mCurrentAnimActionID;

	protected bool mPlayed;

	protected bool mFiredEndTag;

	public int CurrentAnimTypeID => 0;

	public int GetCurrentAnimTypeID()
	{
		return 0;
	}

	public float GetAnimationTimeNormalized()
	{
		return 0f;
	}

	public int GetCurrentAnimActionID()
	{
		return 0;
	}

	public void SetCurrentAnimActionID(int actionID)
	{
	}

	public FAnimState PlayAnim(int animType, FAnimEntry.AnimFacing facing, float animSpeed, int actionID, bool crossFade = true, float startAnimTimeNormalized = -1f)
	{
		return null;
	}

	public void Update()
	{
	}

	public void RunFinalTags()
	{
	}
}
