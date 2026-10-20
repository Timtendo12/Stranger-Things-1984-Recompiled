using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitActionCharge @ 0x1728720..0x1728e28, Ghidra vaddr).
public class FUnitActionCharge : FUnitActionBase
{
	public enum States
	{
		None = 0,
		Move = 1,
		Fail = 2,
		Success = 3
	}

	protected const float cDustSpawnInterval = 0.05f;

	protected States mState;

	protected Vector2 mTargetPoint;

	protected FUnit mTargetUnit;

	protected float mLastDustSpawn;

	public States State => mState;

	public FUnitActionCharge(int newID, FUnit owner, FUnit target, Vector2 targetPoint)
		: base(newID, owner)
	{
		mTargetPoint = targetPoint;
		mTargetUnit = target;
		mState = States.None;
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
		bool fresh = mState == States.None;
		if (fresh)
		{
			StartSlide();
			mState = States.Move;
		}
		return fresh;
	}

	public override void Stop()
	{
		if (mOwner != null)
		{
			StopSlide();
		}
	}

	protected void SetState(States newState)
	{
		if (newState == States.Move)
		{
			StartSlide();
		}
		else if (newState == States.Fail || newState == States.Success)
		{
			StopSlide();
		}
		mState = newState;
	}

	protected void StartSlide()
	{
		if (mOwner.IsPlayer && mOwner.AI != null)
		{
			mOwner.AI.InputEnabled = false;
		}
		mOwner.PlayAnim("Charge");
		mOwner.LoopingSoundHelper.PlayLoopingSound("ObjectFloorSlide");
	}

	protected void StopSlide()
	{
		if (mOwner.IsPlayer && mOwner.AI != null)
		{
			mOwner.AI.InputEnabled = true;
		}
		mOwner.LoopingSoundHelper.StopLoopingSound("ObjectFloorSlide");
		mOwner.PlayAnim("Attack", false);
		mOwner.AI.PutActionOnCooldown(this, mOwner.Weapon.Cooldown);
	}

	public override void Update(float deltaTime)
	{
		if (GetIsFinished())
		{
			return;
		}
		if (mState == States.None)
		{
			Start();
		}
		States next = States.Fail;
		if (mState != States.None)
		{
			float gametime = mOwner.World.Gametime;
			if (mLastDustSpawn + 0.05f <= gametime)
			{
				Vector3 pos = mOwner.Position;
				float offX = FRandom.Generic.RangeFloat(-0.25f, 0.25f);
				float offY = FRandom.Generic.RangeFloat(-0.25f, 0.25f);
				mOwner.World.SpawnEffect("DashDust", new Vector2(pos.x + offX, pos.y + offY), 20);
				mLastDustSpawn = mOwner.World.Gametime;
			}
			FUnit.FUnitMoveResult result = mOwner.MoveTowards(mTargetPoint, 15f, deltaTime, false);
			if (result != FUnit.FUnitMoveResult.Fail_Generic)
			{
				if (result != FUnit.FUnitMoveResult.Fail_UnitCollision)
				{
					Vector3 now = mOwner.Position;
					float dx = mTargetPoint.x - now.x;
					float dy = mTargetPoint.y - now.y;
					if (FUnitAIBase.cMoveTolerance <= Mathf.Sqrt(dx * dx + dy * dy))
					{
						return;
					}
				}
				next = States.Success;
			}
		}
		StopSlide();
		mState = next;
	}

	public override bool CollidedWithUnit(FUnit other)
	{
		StopSlide();
		mState = States.Success;
		return true;
	}
}
