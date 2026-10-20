// Rewritten from libil2cpp.so 1.0.605 (FAnimationController @ 0x17e9374..0x17ea508, Ghidra vaddr).
using System.Collections.Generic;
using UnityEngine;

public class FAnimationController : IAnimTagResponder
{
	protected FAnimEntry.AnimFacing mFacing = FAnimEntry.AnimFacing.DOWN;

	protected FAnimDef mAnimDef;

	protected FAnimator mAnimator;

	protected Queue<FAnimStartData> mAnimQueue = new Queue<FAnimStartData>();

	public string UnitName { get; set; }

	public FAnimEntry.AnimFacing Facing
	{
		get
		{
			return mFacing;
		}
		set
		{
			SetFacing(value);
		}
	}

	public FAnimDef AnimDef
	{
		get
		{
			return mAnimDef;
		}
		set
		{
			SetAnimDef(value);
		}
	}

	public GameObject GO { get; set; }

	public float AnimSpeed { get; protected set; }

	public IAnimTagResponder TagResponder { get; set; }

	public FAnimator Animator => mAnimator;

	public FAnimationController(string unitName, FAnimDef animDef, GameObject go)
	{
		UnitName = unitName;
		GO = go;
		SetAnimDef(animDef);
		TagResponder = this;
	}

	public void SetAnimDef(FAnimDef animDef)
	{
		if (animDef != null)
		{
			mAnimDef = animDef;
			mAnimator = FUtil.GetOrCreateComponent<FAnimator>(GO);
		}
	}

	public bool PlayAnim(int animType, bool loop = true, int? startFrame = null, bool fromQueue = false, bool fireTags = true)
	{
		if (GO == null || mAnimDef == null)
		{
			return false;
		}
		if (mAnimator == null)
		{
			return false;
		}
		bool flag = IsAnimating() && mAnimator.CurrentAnimTypeID == animType && mAnimator.Loop == loop;
		FAnimSet animSet = mAnimDef.GetAnimSet(animType);
		if (animSet == null)
		{
			return false;
		}
		FAnimEntry animEntryByFacing = mAnimDef.GetAnimSet(animType).GetAnimEntryByFacing(mFacing);
		if (animEntryByFacing == null)
		{
			return false;
		}
		FAnimation orCreateAnimation = FAnimationManager.GetOrCreateAnimation(UnitName, animEntryByFacing);
		if (orCreateAnimation == null)
		{
			return false;
		}
		if (!fromQueue && !flag && HasAnimsInQueue())
		{
			ClearQueue();
		}
		mAnimator.LogicDriven = mAnimDef.LogicDriven;
		mAnimator.FireTags = fireTags;
		if (mAnimator.SpriteRenderer != null)
		{
			mAnimator.SpriteRenderer.flipX = animEntryByFacing.FlipX;
			mAnimator.SpriteRenderer.flipY = animEntryByFacing.FlipY;
		}
		mAnimator.transform.localEulerAngles = new Vector3(0f, 0f, animEntryByFacing.Rotation);
		mAnimator.CurrentAnimTypeID = animType;
		mAnimator.Play(orCreateAnimation, TagResponder, loop, startFrame);
		return true;
	}

	public bool HasAnimation(string name)
	{
		int animTypeID = FAnimDefManager.GetAnimTypeID(name);
		if (animTypeID < 0)
		{
			return false;
		}
		if (mAnimDef.GetAnimSet(animTypeID) == null)
		{
			return false;
		}
		return mAnimDef.GetAnimSet(animTypeID).GetAnimEntryByFacing(mFacing) != null;
	}

	public bool QueueAnim(int animType, bool loop = true, int? startFrame = null)
	{
		if (GO == null || mAnimDef == null || mAnimDef.GetAnimSet(animType) == null)
		{
			return false;
		}
		mAnimQueue.Enqueue(new FAnimStartData
		{
			AnimType = animType,
			Loop = loop,
			StartFrame = startFrame
		});
		return true;
	}

	public bool HasAnimsInQueue()
	{
		return mAnimQueue.Count > 0;
	}

	public void ClearQueue()
	{
		mAnimQueue.Clear();
	}

	public void SetFacing(FAnimEntry.AnimFacing facing, bool force = false)
	{
		FAnimEntry.AnimFacing animFacing = mFacing;
		mFacing = facing;
		if ((animFacing != facing || force) && mAnimator.CurrentAnimTypeID >= 0 && mAnimator.CurrentAnimation != null)
		{
			PlayAnim(mAnimator.CurrentAnimTypeID, mAnimator.Loop, mAnimator.CurrentFrame, fromQueue: false, mAnimator.FireTags);
		}
	}

	public bool IsAnimating()
	{
		if (!(mAnimator == null))
		{
			return mAnimator.CurrentAnimation != null;
		}
		return false;
	}

	public void EndCurrentLoop()
	{
		if (mAnimator != null && mAnimator.CurrentAnimation != null)
		{
			mAnimator.Loop = false;
		}
	}

	public void StopCurrentAnimation()
	{
		if (mAnimator != null && mAnimator.CurrentAnimation != null)
		{
			mAnimator.Loop = false;
			mAnimator.Stop();
		}
	}

	public void Update(float deltaTime)
	{
		if (!(mAnimator == null) && !IsAnimating() && mAnimQueue.Count > 0)
		{
			FAnimStartData fAnimStartData = mAnimQueue.Dequeue();
			PlayAnim(fAnimStartData.AnimType, fAnimStartData.Loop, fAnimStartData.StartFrame, fromQueue: true);
		}
	}

	public void FireEndTag(int actionID)
	{
	}

	public void FireLoopTag(int actionID)
	{
	}

	public void FireTag(FAnimTag tag, int actionID)
	{
		if (tag.TagType == AnimTagTypes.Sound && tag is FAnimTagSound fAnimTagSound)
		{
			string text = ((!string.IsNullOrEmpty(fAnimTagSound.SoundSetNameVO)) ? fAnimTagSound.SoundSetNameVO : fAnimTagSound.SoundSetName);
			if (!string.IsNullOrEmpty(text))
			{
				FApp.SoundManager.PlaySoundSet(text);
			}
		}
	}
}
