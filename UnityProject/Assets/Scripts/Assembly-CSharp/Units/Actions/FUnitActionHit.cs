// Rewritten from libil2cpp.so 1.0.605 (FUnitActionHit @ 0x1729b5c..0x1729d28, Ghidra vaddr).
public class FUnitActionHit : FUnitActionBase
{
	public enum States
	{
		None = 0,
		Wait = 1,
		Finished = 2
	}

	protected float mCompleteTime;

	protected float mWaitTime;

	protected States mState;

	public States State => mState;

	public FUnitActionHit(int newID, FUnit owner, float waitTime = 0.2f)
		: base(newID, owner)
	{
		mWaitTime = waitTime;
	}

	public override bool GetIsSuccessful()
	{
		return true;
	}

	public override bool GetIsFinished()
	{
		return mState == States.Finished;
	}

	public override bool Start()
	{
		mState = States.Wait;
		mCompleteTime = mOwner.World.Gametime + mWaitTime;
		mOwner.PlayAnim("Hit", false);
		if (mOwner.AI is FUnitAIEnemy enemyAI)
		{
			enemyAI.SetState(FUnitAIEnemy.EnemyState.AGGROD);
		}
		return true;
	}

	public override void Stop()
	{
		mState = States.Finished;
	}

	protected void SetState(States newState)
	{
		mState = newState;
	}

	public override void Update(float deltaTime)
	{
		if (mState != States.Finished)
		{
			if (mState == States.None)
			{
				Start();
			}
			if (mCompleteTime < mOwner.World.Gametime)
			{
				Stop();
			}
		}
	}
}
