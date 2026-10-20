// Rewritten from libil2cpp.so 1.0.605 (FUnitActionStunned @ 0x172bc04..0x172bfe4, Ghidra vaddr).
public class FUnitActionStunned : FUnitActionBase
{
	public enum States
	{
		None = 0,
		Stunned = 1,
		Fail = 2,
		Success = 3
	}

	protected States mState;

	protected float mStunTime;

	protected float mTotalStunTime;

	protected FUnitAIEnemy mMyAI;

	public States State => mState;

	public float TimeRemaining => mStunTime - mOwner.World.Gametime;

	public FUnitActionStunned(int newID, FUnit owner, float stunTime)
		: base(newID, owner)
	{
		mState = States.None;
		mOwner.EnableInput(false);
		mTotalStunTime = stunTime;
		mMyAI = mOwner.AI as FUnitAIEnemy;
		mStunTime = mOwner.World.Gametime + mTotalStunTime;
		if (mMyAI == null)
		{
			Stop();
			mState = States.Fail;
		}
	}

	public override bool GetIsFailed()
	{
		return mState == States.Fail;
	}

	public override bool GetIsSuccessful()
	{
		return mState == States.Success;
	}

	public override bool GetIsFinished()
	{
		return mState == States.Fail || mState == States.Success;
	}

	public override bool Start()
	{
		SetState(States.Stunned);
		return true;
	}

	public override void Stop()
	{
		if (mOwner == null)
		{
			return;
		}
		if (mMyAI != null)
		{
			mMyAI.PayingAttention = true;
		}
		mOwner.EnableInput(true);
		mOwner.SetIdle();
	}

	protected void SetState(States newState)
	{
		if (newState == States.Stunned)
		{
			mStunTime = mOwner.World.Gametime + mTotalStunTime;
			if (mMyAI != null)
			{
				mMyAI.PayingAttention = false;
			}
			mOwner.PlayAnim("Stun");
		}
		else if (newState == States.Fail || newState == States.Success)
		{
			Stop();
		}
		mState = newState;
	}

	public override void Update(float deltaTime)
	{
		if (IsFinished)
		{
			return;
		}
		States next;
		if (mState == States.None)
		{
			Start();
		}
		if (mState == States.None)
		{
			next = States.Fail;
		}
		else
		{
			if (mOwner.World.Gametime < mStunTime)
			{
				return;
			}
			next = States.Success;
		}
		Stop();
		mState = next;
	}
}
