using System;
using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitActionMove @ 0x172a934..0x172b8fc, Ghidra vaddr).
public class FUnitActionMove : FUnitActionBase
{
	public enum States
	{
		None = 0,
		Path = 1,
		Move = 2,
		Fail = 3,
		Success = 4
	}

	public const string cDefaultAttackRushParticle = "DashDust";

	protected States mState;

	protected FPath mPath;

	protected int mCurrentWaypointIndex = -1;

	protected bool mOwnsMoveTarget;

	protected int mTargetUnitID = -1;

	protected FPoint2 mTargetPosition = FPoint2.invalid;

	protected float mTargetRange;

	protected List<FUnit> mIgnoreUnits = new List<FUnit>();

	protected int mRetries;

	protected int mRetryCap = -1;

	protected bool mSingleAxis;

	protected bool mPlayingWalkAnim;

	public bool CheckUnits = true;

	public string OverrideWalkAnim = string.Empty;

	public float OverrideSpeed = -1f;

	public bool IsAttackRush;

	public string TrailParticle = string.Empty;

	public float ParticleSpawnInterval = 0.05f;

	public bool PathfindAtEveryWaypoint = true;

	public List<int> IgnoreUnits;

	public Action<FUnit> MoveSuccessAction;

	public Action<FUnit> MoveFailAction;

	public bool SetAnimation = true;

	protected float mLastParticleSpawn;

	protected bool mStopped;

	public States State => mState;

	public override int TargetID => mTargetUnitID;

	public override FPoint2 TargetTile => mTargetPosition;

	public override bool IsMove => true;

	public FUnitActionMove(int newID, FUnit owner)
		: base(newID, owner)
	{
		mState = States.None;
		mCurrentWaypointIndex = -1;
		mOwnsMoveTarget = false;
	}

	public void SetPath(FPath path, FUnit targetUnit, float targetRange, bool singleAxis)
	{
		if (mState != States.None)
		{
			FLog.UnityWarning("SetPath called on Move action out of None state.");
			return;
		}
		mPath = path;
		if (targetUnit != null)
		{
			mTargetRange = targetRange;
			mTargetUnitID = targetUnit.ID;
		}
		mSingleAxis = singleAxis;
		mRetryCap = 1;
	}

	public void SetTarget(FUnit targetUnit, float targetRange, bool singleAxis)
	{
		if (mState != States.None)
		{
			FLog.UnityWarning("SetTarget called on Move action out of None state.");
		}
		else if (targetUnit != null)
		{
			mTargetRange = targetRange;
			mRetryCap = 1;
			mTargetUnitID = targetUnit.ID;
			mSingleAxis = singleAxis;
		}
	}

	public void SetTargetPosition(FPoint2 targetPosition, float targetRange, bool singleAxis)
	{
		if (mState != States.None)
		{
			FLog.UnityWarning("SetTargetPosition called on Move action out of None state.");
			return;
		}
		mTargetPosition = targetPosition;
		mTargetRange = targetRange;
		mRetryCap = 1;
		mSingleAxis = singleAxis;
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
		if (mOwner == null)
		{
			return false;
		}
		if (mState == States.None)
		{
			States next = States.Path;
			if (mPath != null && mPath.NumberWaypoints > 0)
			{
				next = States.Move;
			}
			SetState(next);
			return true;
		}
		return false;
	}

	public override void Stop()
	{
		if (!mStopped)
		{
			mStopped = true;
			if (mOwner == null)
			{
				return;
			}
			if (mOwner.IsPlayer && mOwnsMoveTarget)
			{
				mOwner.World.ClearMoveIndicator();
			}
			mOwner.AnimController.EndCurrentLoop();
			if (!GetIsSuccessful() && MoveFailAction != null)
			{
				MoveFailAction(mOwner);
			}
		}
	}

	protected void SetState(States newState)
	{
		while (newState == States.Path)
		{
			if (mRetries < mRetryCap || mRetryCap < 0)
			{
				mRetries++;
				if (mPath == null)
				{
					mPath = new FPath();
				}
				else
				{
					mPath.Clear();
				}
				FPoint2 goal = mTargetPosition;
				FUnit targetUnit = mOwner.World.GetUnitByID(mTargetUnitID);
				if (goal == FPoint2.invalid && targetUnit != null)
				{
					goal = targetUnit.TilePosition;
				}
				newState = States.Fail;
				if (!(goal == FPoint2.invalid))
				{
					FPather.PathResult result = mOwner.World.FindPath(mOwner, mOwner.TilePosition, targetUnit, goal, (int)mTargetRange, IgnoreUnits, mPath, mSingleAxis, CheckUnits);
					switch (result)
					{
					case FPather.PathResult.Partial:
					case FPather.PathResult.Complete:
						newState = States.Move;
						break;
					case FPather.PathResult.InRangeAtStart:
						newState = States.Success;
						break;
					}
				}
			}
			else
			{
				newState = States.Fail;
			}
		}
		if (newState != States.Move)
		{
			FUnit owner;
			System.Action<FUnit> callback;
			if (newState == States.Fail)
			{
				mOwner.AnimController.EndCurrentLoop();
				callback = MoveFailAction;
			}
			else
			{
				if (newState != States.Success)
				{
					mState = newState;
					return;
				}
				if (mPlayingWalkAnim)
				{
					mOwner.AnimController.StopCurrentAnimation();
				}
				if (TargetID >= 0)
				{
					mOwner.AI.DoWork(mOwner.World.GetUnitByID(TargetID));
				}
				callback = MoveSuccessAction;
			}
			if (callback != null)
			{
				callback(mOwner);
			}
			owner = mOwner;
			if (owner.IsPlayer)
			{
				owner.World.PlayerMoveComplete(owner.AI == null || owner.AI.InputEnabled);
			}
			mState = newState;
			return;
		}
		mCurrentWaypointIndex = ((mOwner.TilePosition == mPath.FirstWaypoint && mPath.NumberWaypoints >= 2) ? 1 : 0);
		string anim = (OverrideWalkAnim.Valid() ? OverrideWalkAnim : "Walk");
		FAnimator animator = mOwner.AnimController.Animator;
		bool playing = animator.IsPlaying(anim) || mOwner.PlayAnim(anim);
		mPlayingWalkAnim = playing;
		if (mOwner.IsPlayer)
		{
			if (!mOwner.World.InputEnabled || (mOwner.AI != null && !mOwner.AI.InputEnabled))
			{
				mState = newState;
				return;
			}
			bool indicator;
			if (mTargetUnitID < 0)
			{
				indicator = mOwner.World.SetMoveIndicator(mPath.LastWaypoint);
			}
			else
			{
				FUnit target = mOwner.Room.GetUnitByID(mTargetUnitID);
				if (target == null)
				{
					mState = newState;
					return;
				}
				indicator = mOwner.World.SetMoveIndicator(target);
			}
			mOwnsMoveTarget = indicator;
		}
		mState = newState;
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
				SetState(States.Fail);
				return;
			}
			if (GetIsFinished())
			{
				return;
			}
		}
		mIgnoreUnits.Clear();
		FUnit target = mOwner.World.GetUnitByID(mTargetUnitID);
		if (target != null)
		{
			if (target.Dying || !target.Alive)
			{
				SetState(States.Fail);
				return;
			}
			if (IsInRange(target))
			{
				SetState(States.Success);
				return;
			}
			mIgnoreUnits.Add(target);
		}
		int index = mCurrentWaypointIndex;
		if (index < 0 || index >= mPath.NumberWaypoints)
		{
			SetState(States.Fail);
			return;
		}
		int newIndex = mCurrentWaypointIndex;
		float speed = OverrideSpeed;
		if (speed <= 0f)
		{
			speed = ((mOwner.ProtoUnit != null) ? (mOwner.ProtoUnit.Speed * mOwner.ProtoUnit.SpeedModifier) : 1f);
		}
		if (IsAttackRush && mOwner.Weapon != null && mOwner.Weapon.ChargeRange > 0 && mPath.NumberWaypoints - mCurrentWaypointIndex <= mOwner.Weapon.ChargeRange)
		{
			speed *= 4f;
			TrailParticle = "DashDust";
		}
		if (mOwner.MoveTowards(mPath, mCurrentWaypointIndex, speed, deltaTime, SetAnimation, out newIndex) == FUnit.FUnitMoveResult.Success)
		{
			if (TrailParticle.Valid())
			{
				float gametime = mOwner.World.Gametime;
				if (mLastParticleSpawn + ParticleSpawnInterval <= gametime)
				{
					Vector3 pos = mOwner.Position;
					float offX = FRandom.Generic.RangeFloat(-0.25f, 0.25f);
					float offY = FRandom.Generic.RangeFloat(-0.25f, 0.25f);
					mOwner.World.SpawnEffect("DashDust", new Vector2(pos.x + offX, pos.y + offY), 20);
					mLastParticleSpawn = mOwner.World.Gametime;
				}
			}
			int lastIndex = mCurrentWaypointIndex;
			mCurrentWaypointIndex = newIndex;
			if (newIndex < 0)
			{
				SetState(States.Fail);
			}
			else if (newIndex < mPath.NumberWaypoints)
			{
				if (!mOwner.IsPlayer && lastIndex != newIndex && PathfindAtEveryWaypoint && mTargetUnitID > 0)
				{
					FUnit moving = mOwner.World.GetUnitByID(mTargetUnitID);
					if (moving != null && moving.TilePosition == mPath.LastWaypoint)
					{
						mRetries--;
						SetState(States.Path);
					}
				}
			}
			else
			{
				SetState(States.Success);
			}
		}
		else if (target == null || !IsInRange(target))
		{
			SetState((mRetries < mRetryCap) ? States.Path : States.Fail);
		}
		else
		{
			SetState(States.Success);
		}
	}

	protected bool IsInRange(FUnit targetUnit)
	{
		float distance = mOwner.DistanceToUnitEdge(targetUnit, mSingleAxis);
		if (mTargetRange + FUnitAIBase.cMoveTolerance < distance)
		{
			return false;
		}
		return mOwner.CanSee(targetUnit, CheckUnits);
	}
}
