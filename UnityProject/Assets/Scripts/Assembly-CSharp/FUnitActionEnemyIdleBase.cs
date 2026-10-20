// Rewritten from libil2cpp.so 1.0.605 (FUnitActionEnemyIdleBase @ 0x1729414..0x172989c, Ghidra vaddr).
public class FUnitActionEnemyIdleBase : FUnitActionBase
{
	public enum States
	{
		None = 0,
		Evaluate = 1,
		Finished = 2
	}

	protected const float cMaxExistTime = 5f;

	protected static int cAttackAnimType = -1;

	protected States mState;

	protected FUnitAIBase mMyAI;

	protected int mWorkFailCount;

	public States State => mState;

	public override bool IsIdle => true;

	public FUnitActionEnemyIdleBase(int newID, FUnit owner)
		: base(newID, owner)
	{
		mState = States.None;
		mMyAI = mOwner.AI;
		if (cAttackAnimType == -1)
		{
			cAttackAnimType = FAnimDefManager.GetAnimTypeID("Attack");
		}
	}

	public override bool GetIsFinished()
	{
		return mState == States.Finished;
	}

	public override bool GetIsSuccessful()
	{
		return true;
	}

	public override bool Start()
	{
		bool hasAI = mMyAI != null;
		mState = (hasAI ? States.Evaluate : States.Finished);
		return hasAI;
	}

	public override void Stop()
	{
	}

	protected void SetState(States newState)
	{
		mState = newState;
	}

	public override void Update(float deltaTime)
	{
		if (!GetIsFinished())
		{
			if (mState == States.None)
			{
				Start();
			}
			if (mState == States.Evaluate && Evaluate())
			{
				mState = States.Finished;
			}
		}
	}

	public virtual bool Evaluate()
	{
		if (mState == States.Finished)
		{
			return true;
		}
		if (mOwner.World == null)
		{
			return false;
		}
		FUnit player = FWorld.Player;
		if (player == null)
		{
			return false;
		}
		if (!player.AliveAndWell)
		{
			return false;
		}
		if (!player.Interactable)
		{
			return false;
		}
		FUnitAIEnemy enemyAI = mMyAI as FUnitAIEnemy;
		if (enemyAI == null)
		{
			return false;
		}
		if (enemyAI.State == FUnitAIEnemy.EnemyState.LEASHING)
		{
			enemyAI.SetState(FUnitAIEnemy.EnemyState.IDLE);
		}
		else if (enemyAI.State == FUnitAIEnemy.EnemyState.AGGROD)
		{
			if (mWorkFailCount > 3)
			{
				enemyAI.Leash();
				return true;
			}
			if (mOwner.AnimController.Animator.CurrentAnimTypeID == cAttackAnimType)
			{
				return true;
			}
			if (mMyAI.DoWork(FWorld.Player))
			{
				return true;
			}
			if (mMyAI.GetActionTypeOnCooldown(typeof(FUnitActionAttack)))
			{
				return false;
			}
			mWorkFailCount++;
			return false;
		}
		if (enemyAI.MoveToNextWaypoint())
		{
			return true;
		}
		return false;
	}
}
