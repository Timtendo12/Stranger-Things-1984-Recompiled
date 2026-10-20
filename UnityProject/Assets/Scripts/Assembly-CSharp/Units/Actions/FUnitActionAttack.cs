using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitActionAttack @ 0x17229ec..0x1728720, Ghidra vaddr).
public class FUnitActionAttack : FUnitActionWork
{
	protected bool mFaceTarget = true;

	protected bool mAttacked;

	protected FPoint2 mTargetPoint = FPoint2.invalid;

	protected string mAnimToPlay = string.Empty;

	public override bool IsAttack => true;

	public override FPoint2 TargetTile => mTargetPoint;

	public FUnitActionAttack(int newID, FUnit owner, FUnit targetUnit, bool faceTarget = true, string animToPlay = "Attack")
		: base(newID, owner, targetUnit)
	{
		mFaceTarget = faceTarget;
		mAnimToPlay = animToPlay;
		mTargetPoint = targetUnit.TilePosition;
	}

	public FUnitActionAttack(int newID, FUnit owner, FPoint2 targetPoint, bool faceTarget = true, string animToPlay = "Attack")
		: base(newID, owner, null)
	{
		mFaceTarget = faceTarget;
		mAnimToPlay = animToPlay;
		mTargetPoint = targetPoint;
	}

	public override bool Start()
	{
		bool fresh = mState == States.None;
		if (fresh)
		{
			SetState(States.Work);
		}
		return fresh;
	}

	public void Finish()
	{
		SetState(States.Success);
	}

	public override void DoWork()
	{
		if (mAttacked)
		{
			return;
		}
		if (mFaceTarget)
		{
			Vector3 myPos = mOwner.GO.transform.position;
			mOwner.SetFacing(FUnit.GetFacingFromDir(new Vector2((float)mTargetPoint.x - myPos.x, (float)mTargetPoint.y - myPos.y)));
		}
		if (mTargetUnit != null && mTargetUnit != mOwner && mOwner.Weapon.Type == "Melee" && !mOwner.IsAdjacent(mTargetUnit) && mOwner.Weapon.Range > 1)
		{
			FUnitActionCharge charge = new FUnitActionCharge(mOwner.World.GetNextActionID(), mOwner, mTargetUnit, mTargetUnit.Position);
			charge.DoesInterrupt = true;
			mOwner.AI.AddAction(charge);
			SetState(States.Success);
			return;
		}
		if (!mOwner.PlayAnim(mAnimToPlay, false))
		{
			mOwner.Attack();
		}
		else
		{
			mOwner.AnimController.Animator.SetAnimSpeed(mOwner.Weapon.AttackSpeedMultiplier);
		}
		mOwner.AI.PutActionOnCooldown(this, mOwner.Weapon.Cooldown);
		mAttacked = true;
	}
}
