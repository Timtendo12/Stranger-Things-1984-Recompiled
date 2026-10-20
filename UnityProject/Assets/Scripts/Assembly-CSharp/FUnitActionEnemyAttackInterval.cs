using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitActionEnemyAttackInterval @ 0x1729144..0x1729414, Ghidra vaddr).
public class FUnitActionEnemyAttackInterval : FUnitActionWork
{
	protected float mCompleteTime;

	protected float mIntervalTime;

	public override bool IsAttack => true;

	public FUnitActionEnemyAttackInterval(int newID, FUnit owner, FUnit targetUnit, float intervalTime)
		: base(newID, owner, targetUnit)
	{
		mIntervalTime = intervalTime;
	}

	public override bool Start()
	{
		mCompleteTime = mOwner.World.Gametime + mIntervalTime;
		bool started = base.Start();
		if (started)
		{
			mOwner.PlayAnim("AttackCharge");
		}
		Vector3 targetPos = mTargetUnit.GO.transform.position;
		Vector3 myPos = mOwner.GO.transform.position;
		mOwner.SetFacing(FUnit.GetFacingFromDir(new Vector2(targetPos.x - myPos.x, targetPos.y - myPos.y)));
		return started;
	}

	public override void DoWork()
	{
		if (!(mCompleteTime >= mOwner.World.Gametime))
		{
			FUnitActionAttack attack = new FUnitActionAttack(mOwner.World.GetNextActionID(), mOwner, mTargetUnit, false, "Attack");
			attack.DoesInterrupt = true;
			mOwner.AI.AddAction(attack);
			mOwner.AI.AddAction(new FUnitActionWait(mOwner.World.GetNextActionID(), mOwner, mIntervalTime));
			SetState(States.Success);
		}
	}
}
