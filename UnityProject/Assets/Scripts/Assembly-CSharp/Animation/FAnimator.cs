// Rewritten from libil2cpp.so 1.0.605 (FAnimator @ 0x17e9e8c..0x17eb5e8, Ghidra vaddr).
using UnityEngine;

public class FAnimator : MonoBehaviour
{
	public SpriteRenderer SpriteRenderer;

	protected float mLoopWaitTime;

	protected float mTimeBetweenFrames;

	protected float mTimer;

	protected float mAnimSpeed = 1f;

	protected int mLoopCount;

	protected bool mPaused;

	public bool Playing => CurrentAnimation != null;

	public FAnimation CurrentAnimation { get; private set; }

	public int CurrentFrame { get; private set; }

	public bool Loop { get; set; }

	public IAnimTagResponder TagResponder { get; private set; }

	public int CurrentAnimTypeID { get; set; }

	public int CurrentAnimActionID { get; protected set; }

	public bool FireTags { get; set; }

	public bool LogicDriven { get; set; }

	private void Awake()
	{
		if (!SpriteRenderer)
		{
			SpriteRenderer = GetComponent<SpriteRenderer>();
		}
	}

	private void OnEnable()
	{
		if (CurrentAnimation != null)
		{
			float animSpeed = mAnimSpeed;
			ForcePlay(CurrentAnimation, TagResponder, Loop, CurrentFrame);
			mAnimSpeed = animSpeed;
			mTimeBetweenFrames = 1f / (float)CurrentAnimation.FPS / animSpeed;
		}
	}

	public void Play(FAnimation animation, IAnimTagResponder responder, bool loop = true, int? startFrame = null)
	{
		if (animation == null)
		{
			Debug.LogWarning("could not find animation " + base.name);
			return;
		}
		if (!startFrame.HasValue)
		{
			startFrame = ((CurrentAnimation == animation && loop) ? (CurrentFrame + 1) : 0);
		}
		ForcePlay(animation, responder, loop, startFrame.Value);
	}

	public void ForcePlay(FAnimation animation, IAnimTagResponder responder, bool loop = true, int startFrame = 0)
	{
		if (animation == null)
		{
			return;
		}
		CurrentAnimation = animation;
		Loop = loop;
		CurrentFrame = ((startFrame >= 0) ? Mathf.Min(startFrame, CurrentAnimation.Frames.Length - 1) : 0);
		if (CurrentFrame < 0)
		{
			CurrentAnimation = null;
			return;
		}
		if (SpriteRenderer == null)
		{
			SpriteRenderer = GetComponent<SpriteRenderer>();
		}
		SpriteRenderer.sprite = animation.Frames[CurrentFrame];
		CurrentAnimActionID = FApp.World.GetNextActionID();
		TagResponder = responder;
		StopAllCoroutines();
		mLoopWaitTime = 0f;
		mAnimSpeed = 1f;
		mTimer = 0f;
		mTimeBetweenFrames = 1f / (float)CurrentAnimation.FPS;
		CheckTags();
		mPaused = false;
	}

	public void SetAnimSpeed(float speed)
	{
		mAnimSpeed = speed;
		mTimeBetweenFrames = 1f / (float)CurrentAnimation.FPS / speed;
	}

	protected void SyncAnimSpeed()
	{
		mTimeBetweenFrames = 1f / (float)CurrentAnimation.FPS / mAnimSpeed;
	}

	public bool IsPlaying(string name)
	{
		if (CurrentAnimation != null)
		{
			return CurrentAnimation.Name == name;
		}
		return false;
	}

	public void Stop()
	{
		if (CurrentAnimation != null)
		{
			Loop = false;
			CurrentFrame = CurrentAnimation.Frames.Length;
		}
	}

	public void Pause()
	{
		mPaused = true;
	}

	public void UnPause()
	{
		mPaused = false;
	}

	public void Update()
	{
		if (FApp.World == null || FApp.World.State == FWorld.GameState.ANIM_PAUSED || FApp.World.State == FWorld.GameState.PAUSED || (LogicDriven && FApp.World.State == FWorld.GameState.LOGIC_PAUSED))
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (CurrentAnimation == null || mPaused)
		{
			return;
		}
		if (!Loop && CurrentFrame >= CurrentAnimation.Frames.Length - 1)
		{
			CurrentAnimation = null;
		}
		else if (mLoopWaitTime <= 0f)
		{
			if (mTimeBetweenFrames > mTimer)
			{
				mTimer += deltaTime;
				return;
			}
			if (!(mTimeBetweenFrames < mTimer))
			{
				return;
			}
			int currentFrame = CurrentFrame;
			do
			{
				mTimer -= mTimeBetweenFrames;
				NextFrame(CurrentAnimation);
			}
			while (mTimeBetweenFrames < mTimer);
			if (currentFrame != CurrentFrame)
			{
				SpriteRenderer.sprite = CurrentAnimation.Frames[CurrentFrame];
			}
		}
		else
		{
			mLoopWaitTime -= deltaTime;
		}
	}

	protected void NextFrame(FAnimation animation)
	{
		CurrentFrame++;
		if (CurrentFrame >= animation.Frames.Length)
		{
			if (!Loop)
			{
				CurrentFrame = animation.Frames.Length - 1;
			}
			else
			{
				CurrentFrame = 0;
				mLoopWaitTime = CurrentAnimation.AnimEntry.LoopInterval;
				mLoopCount++;
			}
		}
		CheckTags();
	}

	protected void CheckTags()
	{
		if (TagResponder == null || CurrentAnimation == null || !FireTags)
		{
			return;
		}
		if (CurrentFrame == CurrentAnimation.Frames.Length)
		{
			if (Loop)
			{
				TagResponder.FireLoopTag(CurrentAnimActionID);
			}
			else
			{
				TagResponder.FireEndTag(CurrentAnimActionID);
			}
		}
		else
		{
			CurrentAnimation.AnimEntry.EvaluateTags(CurrentFrame, CurrentAnimation.Frames.Length, CurrentAnimActionID, TagResponder, mLoopCount == 0);
		}
	}

	public float GetCurrentAnimLength()
	{
		if (CurrentAnimation == null)
		{
			return 0f;
		}
		return mTimeBetweenFrames * (float)CurrentAnimation.Frames.Length;
	}
}
