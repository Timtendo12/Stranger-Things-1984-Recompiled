// Rewritten from libil2cpp.so 1.0.605 (FUnitActionWait @ 0x172bfe4..0x172c114, Ghidra vaddr).
public class FUnitActionWait : FUnitActionBase
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

	public FUnitActionWait(int newID, FUnit owner, float waitTime)
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
		mOwner.EnableInput(false);
		return true;
	}

	public override void Stop()
	{
		mState = States.Finished;
		mOwner.EnableInput(true);
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
