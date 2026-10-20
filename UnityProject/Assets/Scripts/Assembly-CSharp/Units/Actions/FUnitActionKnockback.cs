using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitActionKnockback @ 0x1724ed8..0x172a934, Ghidra vaddr).
public class FUnitActionKnockback : FUnitActionBase
{
	public enum States
	{
		None = 0,
		Move = 1,
		Fail = 2,
		Success = 3
	}

	protected States mState;

	protected FPoint2 mEndPoint;

	protected int mCurrentWaypointIndex = -1;

	protected bool mCollidedThisSegment;

	protected int mPushDist;

	protected int mNumFails;

	protected Vector2 mDirection;

	protected FPoint2 mStartPoint;

	protected bool mPlayHitAnimation = true;

	public States State => mState;

	public override FPoint2 TargetTile => mEndPoint;

	public Vector2 Direction => mDirection;

	public FUnitActionKnockback(int newID, FUnit owner, FPoint2 endPoint, bool playHitAnimation = true)
		: base(newID, owner)
	{
		mEndPoint = endPoint;
		mState = States.None;
		mStartPoint = mOwner.TilePosition;
		mCurrentWaypointIndex = -1;
		if (mOwner.AI != null)
		{
			mOwner.AI.InputEnabled = false;
		}
		mPushDist = (int)mOwner.TilePosition.DistanceTo(endPoint);
		Vector2 toEnd = endPoint.ToVector2();
		Vector3 pos = mOwner.Position;
		float dx = toEnd.x - pos.x;
		float dy = toEnd.y - pos.y;
		float length = Mathf.Sqrt(dx * dx + dy * dy);
		mDirection = ((length > 1E-05f) ? new Vector2(dx / length, dy / length) : Vector2.zero);
		mPlayHitAnimation = playHitAnimation;
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

	public void ShortCircuit()
	{
		Vector2 toEnd = mEndPoint.ToVector2();
		Vector3 pos = mOwner.Position;
		float dx = toEnd.x - pos.x;
		float dy = toEnd.y - pos.y;
		float length = Mathf.Sqrt(dx * dx + dy * dy);
		Vector2 dir = ((length > 1E-05f) ? new Vector2(dx / length, dy / length) : Vector2.zero);
		FPoint2 step = FUnit.GetFPDirFromFacing(FUnit.GetFacingFromDir(dir));
		mEndPoint = mOwner.TilePosition + step;
	}

	public override bool Start()
	{
		if (mEndPoint != FPoint2.invalid && mOwner != null && mState == States.None)
		{
			if (mOwner.AI is FUnitAIEnemy enemyAI && !mOwner.ProtoUnit.IsTag("DontAggroFromKnockback"))
			{
				enemyAI.SetState(FUnitAIEnemy.EnemyState.AGGROD);
			}
			StartSlide();
			mState = States.Move;
			return true;
		}
		return false;
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
		if (mPlayHitAnimation)
		{
			mOwner.PlayAnim("Hit", false);
		}
		if (mOwner.ProtoUnit.IsTag("Pushable"))
		{
			mOwner.PlayAnim("Move");
		}
		mOwner.LoopingSoundHelper.PlayLoopingSound("ObjectFloorSlide");
	}

	protected void StopSlide()
	{
		if (mOwner.AI != null)
		{
			mOwner.AI.InputEnabled = true;
		}
		if (mOwner.ProtoUnit.IsTag("Pushable"))
		{
			mOwner.SetIdle();
		}
		mOwner.LoopingSoundHelper.StopLoopingSound("ObjectFloorSlide");
		if (mStartPoint == mOwner.TilePosition)
		{
			return;
		}
		FTile tile = mOwner.World.GetTile(mOwner.TilePosition);
		if (tile == null)
		{
			return;
		}
		for (int i = 0; i < tile.Units.Count; i++)
		{
			mOwner.SendMessage(tile.Units[i], "KnockbackEndedOnTile");
		}
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
			if (mState == States.None)
			{
				StopSlide();
				mState = States.Fail;
				return;
			}
		}
		FUnit.FUnitMoveResult result = mOwner.MoveTowards(mEndPoint.ToVector2(), 15f, deltaTime, false);
		if (result == FUnit.FUnitMoveResult.Fail_Generic)
		{
			StopSlide();
			mState = States.Fail;
			return;
		}
		Vector2 toEnd = mEndPoint.ToVector2();
		Vector3 pos = mOwner.Position;
		float dx = toEnd.x - pos.x;
		float dy = toEnd.y - pos.y;
		float length = Mathf.Sqrt(dx * dx + dy * dy);
		if (length < FUnitAIBase.cMoveTolerance)
		{
			StopSlide();
			mState = States.Success;
		}
		else
		{
			Vector2 dir = ((length > 1E-05f) ? new Vector2(dx / length, dy / length) : Vector2.zero);
			Vector2 diff = mDirection - dir;
			if (diff.sqrMagnitude >= 1E-10f)
			{
				StopSlide();
				mState = States.Success;
			}
		}
		if (result == FUnit.FUnitMoveResult.Fail_UnitCollision)
		{
			mNumFails++;
		}
		if (mNumFails >= 4)
		{
			StopSlide();
			mState = States.Success;
		}
	}

	public override bool CollidedWithUnit(FUnit other)
	{
		if (mCollidedThisSegment)
		{
			return true;
		}
		Vector3 pos = mOwner.Position;
		float dx = (float)mEndPoint.x - pos.x;
		float dy = (float)mEndPoint.y - pos.y;
		if (Mathf.Sqrt(dx * dx + dy * dy) <= 0.2f)
		{
			return true;
		}
		if (other != null)
		{
			Vector3 otherPos = other.Position;
			if (dx * (otherPos.x - pos.x) + dy * (otherPos.y - pos.y) < 0.5f)
			{
				return true;
			}
			if (other.ProtoUnit.IsTag("Pushable") && !other.Interactable)
			{
				return true;
			}
			Vector2 dir = mOwner.GetTileDirectionToUnit(other);
			mCollidedThisSegment = true;
			other.Push(dir, mPushDist, false, false, true);
			FPoint2 endPoint;
			if (mOwner.CanPush(other, dir, mPushDist, out endPoint))
			{
				return true;
			}
			if (!other.IsPlayer && other.AI is FUnitAIEnemy enemyAI)
			{
				enemyAI.SetState(FUnitAIEnemy.EnemyState.AGGROD);
			}
			StopSlide();
			mState = States.Success;
			return true;
		}
		return false;
	}
}
