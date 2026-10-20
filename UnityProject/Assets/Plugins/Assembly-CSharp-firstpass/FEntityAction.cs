// Rewritten from libil2cpp.so 1.0.605 (FEntityAction @ 0x164c0d4..0x164c67c, Ghidra vaddr).
public abstract class FEntityAction
{
	protected IEntity mTarget;

	protected FEntityAction mParent;

	public static Fix64 DeltaTimeThreshhold = (Fix64)1E-06f;

	public static Fix64 Indeterminate = Fix64.MinValue;

	public static Fix64 Infinite = Fix64.MaxValue;

	protected Fix64 mDuration = Indeterminate;

	protected Fix64 mElapsed = Fix64.Zero;

	protected FEntityActionState mState;

	public IEntity Target => mTarget;

	public FEntityAction Parent => mParent;

	public Fix64 Duration => mDuration;

	public bool InfiniteLength => mDuration == Infinite;

	public bool IndeterminateLength => mDuration == Indeterminate;

	public Fix64 Elapsed => mElapsed;

	public FEntityActionState State => mState;

	public bool IgnoreStopOnDamage { get; set; }

	public virtual void SetTarget(IEntity target)
	{
		mTarget = target;
		OnSetTarget();
	}

	public virtual void OnSetTarget()
	{
	}

	public virtual void SetParent(FEntityAction parent)
	{
		mParent = parent;
		OnSetParent();
	}

	public virtual void OnSetParent()
	{
	}

	public virtual Fix64 Update(Fix64 deltaTime)
	{
		if (mState == FEntityActionState.Running)
		{
			mElapsed += deltaTime;
			Fix64 remainder = Fix64.Zero;
			if (!IndeterminateLength && mElapsed > mDuration)
			{
				remainder = mElapsed - mDuration;
				mElapsed = mDuration;
				deltaTime -= remainder;
			}
			bool keepRunning = OnUpdate(deltaTime);
			deltaTime = remainder;
			if (!keepRunning)
			{
				Stop();
			}
		}
		return deltaTime;
	}

	public abstract bool OnUpdate(Fix64 deltaTime);

	public virtual void Start()
	{
		if (mState != FEntityActionState.Running)
		{
			mState = FEntityActionState.Running;
			mElapsed = Fix64.Zero;
			OnStart();
		}
	}

	public virtual void OnStart()
	{
	}

	public virtual void Stop()
	{
		if (mState != FEntityActionState.Stopped)
		{
			OnStop();
			mState = FEntityActionState.Stopped;
			if (mParent == null)
			{
				mTarget.RemoveAction(this);
			}
		}
	}

	public virtual void OnStop()
	{
	}
}
