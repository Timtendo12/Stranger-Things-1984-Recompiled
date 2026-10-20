using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIBase @ 0x172c318..0x172f754, Ghidra vaddr).
public class FUnitAIBase
{
	public enum InputResult
	{
		None = 0,
		Handled = 1,
		NotHandled = 2,
		Ignored = 3
	}

	public static float cMoveTolerance = 0.02f;

	protected int mInputEnabled;

	protected FUnit mOwner;

	protected List<FUnitActionBase> mActions = new List<FUnitActionBase>();

	protected Dictionary<Type, float> mActionsOnCooldown = new Dictionary<Type, float>();

	protected float mLastAttackTime;

	protected int mLastAttackTargetID = -1;

	protected FPoint2 mSpawnPoint = FPoint2.invalid;

	protected List<FPoint2> mPatrolPoints = new List<FPoint2>();

	protected float mResetWaitTime;

	protected float mResetTime = -1f;

	protected bool mInitialized;

	protected bool mWaitingOnReset;

	[CompilerGenerated]
	private Action<FUnit> m_mOnSingleResetAction;

	private List<FUnitActionBase> mActionsToRemove = new List<FUnitActionBase>();

	public bool InputEnabled
	{
		get
		{
			return mInputEnabled < 1;
		}
		set
		{
			if (value)
			{
				mInputEnabled = Mathf.Max(mInputEnabled - 1, 0);
			}
			else
			{
				mInputEnabled++;
			}
		}
	}

	public FUnit Owner => mOwner;

	public List<FUnitActionBase> Actions => mActions;

	public float LastAttackTime => mLastAttackTime;

	public int LastAttackTargetID => mLastAttackTargetID;

	public int CurrentTargetID => GetCurrentTargetID();

	public FPoint2 CurrentTargetTile => GetCurrentTargetTile();

	public FPoint2 SpawnPoint
	{
		get
		{
			return mSpawnPoint;
		}
		set
		{
			mSpawnPoint = value;
		}
	}

	public List<FPoint2> PatrolPoints => mPatrolPoints;

	public int CurrentPatrolPointIndex { get; set; }

	public float ResetTime => mResetTime;

	public bool DoesReset => mResetTime > 0f;

	public bool WaitingOnReset => mWaitingOnReset;

	protected event Action<FUnit> mOnSingleResetAction
	{
		add
		{
			m_mOnSingleResetAction = (Action<FUnit>)Delegate.Combine(m_mOnSingleResetAction, value);
		}
		remove
		{
			m_mOnSingleResetAction = (Action<FUnit>)Delegate.Remove(m_mOnSingleResetAction, value);
		}
	}

	public event Action<FUnit> OnSingleResetAction
	{
		add
		{
			mOnSingleResetAction -= value;
			mOnSingleResetAction += value;
		}
		remove
		{
			mOnSingleResetAction -= value;
		}
	}

	public FUnitAIBase(FUnit owner)
	{
		mOwner = owner;
		CurrentPatrolPointIndex = -1;
		InputEnabled = true;
		mResetTime = owner.StartData.GetObjectProperty<float>("ResetTime");
	}

	public virtual bool AddAction(FUnitActionBase newAction)
	{
		if (newAction != null && newAction.Owner == mOwner && !GetActionTypeOnCooldown(newAction.GetType()))
		{
			if (mActions.Contains(newAction))
			{
				return false;
			}
			if (newAction.DoesInterrupt)
			{
				StopActions();
			}
			mActions.Add(newAction);
			return true;
		}
		return false;
	}

	public void MarkDirty()
	{
		mInitialized = false;
	}

	protected virtual void Init()
	{
		mInitialized = true;
	}

	public void StopActions()
	{
		for (int i = 0; i < mActions.Count; i++)
		{
			mActions[i].Stop();
		}
		mActions.Clear();
	}

	public bool GetHasMoveAction()
	{
		int i;
		for (i = 0; i < mActions.Count && !mActions[i].IsMove; i++)
		{
		}
		return i < mActions.Count;
	}

	public bool GetHasIdleAction()
	{
		int i;
		for (i = 0; i < mActions.Count && !mActions[i].IsIdle; i++)
		{
		}
		return i < mActions.Count;
	}

	public bool GetHasActionOfType<T>()
	{
		int i;
		for (i = 0; i < mActions.Count && !(mActions[i] is T); i++)
		{
		}
		return i < mActions.Count;
	}

	public int GetCurrentTargetID()
	{
		for (int i = 0; i < mActions.Count; i++)
		{
			if (mActions[i].TargetID > -1)
			{
				return mActions[i].TargetID;
			}
		}
		return -1;
	}

	public FPoint2 GetCurrentTargetTile()
	{
		for (int i = 0; i < mActions.Count; i++)
		{
			if (mActions[i].TargetTile != FPoint2.invalid)
			{
				return mActions[i].TargetTile;
			}
		}
		return FPoint2.invalid;
	}

	public FUnitActionBase GetCurrentAction()
	{
		for (int i = 0; i < mActions.Count; i++)
		{
			if (mActions[i].IsFailed)
			{
				return null;
			}
			if (!mActions[i].IsFinished)
			{
				return mActions[i];
			}
		}
		return null;
	}

	public virtual void Update(float deltaTime)
	{
		if (mOwner != null && mOwner.Alive)
		{
			if (!mInitialized)
			{
				Init();
			}
			CheckActionCooldowns();
			CheckAnimations();
			CheckActions(deltaTime);
			CheckResetTimer(deltaTime);
		}
	}

	public void RegisterInteraction(bool final = false)
	{
		if (final)
		{
			mResetTime = -1f;
			mOwner.Room.UnitHasBeenReset(mOwner);
			mOwner.ShowCountdown(false);
			mWaitingOnReset = false;
		}
		if (mResetTime > 0f)
		{
			mResetWaitTime = 0f;
			mWaitingOnReset = true;
			mOwner.Room.UnitWaitingForReset(mOwner);
			mOwner.ShowCountdown(true, mResetTime);
		}
	}

	protected void CheckActions(float deltaTime)
	{
		int count = mActions.Count;
		if (count > 0)
		{
			for (int i = 0; i < count && mActions.Count > i; i++)
			{
				FUnitActionBase action = mActions[i];
				action.Update(deltaTime);
				if (!action.IsFinished)
				{
					break;
				}
				if (!action.IsSuccessful)
				{
					StopActions();
					mActionsToRemove.Clear();
					return;
				}
				mActionsToRemove.Add(action);
			}
		}
		for (int j = 0; j < mActionsToRemove.Count; j++)
		{
			mActions.Remove(mActionsToRemove[j]);
		}
		mActionsToRemove.Clear();
	}

	protected virtual void CheckAnimations()
	{
		if (mOwner.AnimController != null && !mOwner.AnimController.IsAnimating())
		{
			mOwner.SetIdle();
		}
	}

	protected void CheckActionCooldowns()
	{
		if (mActionsOnCooldown.Count < 1)
		{
			return;
		}
		List<Type> expired = new List<Type>();
		foreach (KeyValuePair<Type, float> entry in mActionsOnCooldown)
		{
			if (entry.Value <= mOwner.World.Gametime)
			{
				expired.Add(entry.Key);
			}
		}
		for (int i = 0; i < expired.Count; i++)
		{
			mActionsOnCooldown.Remove(expired[i]);
		}
	}

	protected void CheckResetTimer(float deltaTime)
	{
		if (mResetTime > 0f)
		{
			mResetWaitTime += deltaTime;
			if (mWaitingOnReset && mResetTime < mResetWaitTime)
			{
				Reset();
				mWaitingOnReset = false;
				mResetWaitTime = mResetTime;
				mOwner.Room.UnitHasBeenReset(mOwner);
				mOwner.ShowCountdown(false);
			}
		}
	}

	public virtual void ReleaseStuff()
	{
	}

	protected virtual void Reset()
	{
		mOwner.LoopingSoundHelper.StopAllLoopingSounds();
		mOwner.FullReset();
		if (m_mOnSingleResetAction != null)
		{
			m_mOnSingleResetAction(mOwner);
		}
		m_mOnSingleResetAction = null;
	}

	public virtual bool CollidedWithUnit(FUnit other)
	{
		FUnitActionBase current = GetCurrentAction();
		if (current != null)
		{
			return current.CollidedWithUnit(other);
		}
		return false;
	}

	public virtual InputResult HandleWorldTap(Vector3 screenPos, bool considerUnits, bool force = false)
	{
		return InputResult.None;
	}

	public virtual InputResult HandleTileTap(FPoint2 tile, bool force = false)
	{
		return InputResult.None;
	}

	public virtual InputResult HandleUnitTap(FUnit target, bool force = false)
	{
		return InputResult.None;
	}

	public virtual bool ProcessMessage(FUnitMessage m)
	{
		return false;
	}

	public bool DoMoveToPoint(FUnit targetUnit, int range, FPath path, bool singleAxis = false, bool checkUnits = true, string overrideWalkAnim = "", bool pathfindAtEveryWaypoint = true, Action<FUnit> moveSuccessCallback = null, Action<FUnit> moveFailCallback = null)
	{
		if (mOwner == null)
		{
			return false;
		}
		FUnitActionMove move = new FUnitActionMove(mOwner.World.GetNextActionID(), mOwner);
		move.SetPath(path, targetUnit, range, singleAxis);
		move.OverrideWalkAnim = overrideWalkAnim;
		move.PathfindAtEveryWaypoint = pathfindAtEveryWaypoint;
		move.CheckUnits = checkUnits;
		move.MoveSuccessAction = (Action<FUnit>)Delegate.Combine(move.MoveSuccessAction, moveSuccessCallback);
		move.MoveFailAction = (Action<FUnit>)Delegate.Combine(move.MoveFailAction, moveFailCallback);
		if (mOwner.IsPlayer && targetUnit.IsTag("Attackable") && CanInteract(targetUnit))
		{
			FPoint2 lastWaypoint = path.LastWaypoint;
			if (lastWaypoint.DistanceTo(targetUnit.TilePosition) <= 1f && FApp.PlayerStats.IsAtMaxHealth)
			{
				move.IsAttackRush = true;
			}
		}
		return AddAction(move);
	}

	protected bool CanInteract(FUnit target)
	{
		if (target == null)
		{
			return false;
		}
		return target.ProtoUnit.CheckInteractRequirements();
	}

	public bool DoMoveToPoint(FUnit targetUnit, int range, bool singleAxis = false)
	{
		FUnitActionMove move = new FUnitActionMove(mOwner.World.GetNextActionID(), mOwner);
		move.SetTarget(targetUnit, range, singleAxis);
		return AddAction(move);
	}

	public bool DoMoveToPoint(FPoint2 targetPosition, int range, bool singleAxis = false, bool checkUnits = true, bool pathFind = true, string overrideWalkAnim = "", float overrideSpeed = -1f, List<int> ignoreUnits = null, Action<FUnit> moveSuccessCallback = null, Action<FUnit> moveFailCallback = null)
	{
		if (mOwner == null)
		{
			return false;
		}
		FUnitActionMove move = new FUnitActionMove(mOwner.World.GetNextActionID(), mOwner);
		if (!pathFind)
		{
			FPath path = new FPath();
			path.AddWaypoint(mOwner.TilePosition);
			path.AddWaypoint(targetPosition);
			move.SetPath(path, null, 0f, false);
		}
		else
		{
			move.SetTargetPosition(targetPosition, range, singleAxis);
		}
		if (overrideWalkAnim.Valid())
		{
			move.OverrideWalkAnim = overrideWalkAnim;
		}
		move.OverrideSpeed = overrideSpeed;
		move.CheckUnits = checkUnits;
		move.MoveSuccessAction = (Action<FUnit>)Delegate.Combine(move.MoveSuccessAction, moveSuccessCallback);
		move.MoveFailAction = (Action<FUnit>)Delegate.Combine(move.MoveFailAction, moveFailCallback);
		move.IgnoreUnits = ignoreUnits;
		return AddAction(move);
	}

	public bool DoWork(FPoint2 targetPoint)
	{
		if (mOwner.Weapon != null && mOwner.Weapon.CanTargetGround && mOwner.CanAttackWithWeapon())
		{
			FUnitActionAttack attack = TryAttack(targetPoint);
			if (attack != null)
			{
				return AddAction(attack);
			}
		}
		return DoMoveToPoint(targetPoint, 0, false, true, true, string.Empty);
	}

	protected FUnitActionAttack TryAttack(FPoint2 targetPoint)
	{
		if (targetPoint == mOwner.TilePosition)
		{
			return null;
		}
		if (!CanAttack())
		{
			mOwner.World.SpawnFlyOffText(FLoc.GetString("NoAmmo"), mOwner.Position, new Vector2(32f, 32f));
			return null;
		}
		string animToPlay = "Attack";
		if (mOwner.Weapon.AttackAnimOverride.Valid())
		{
			animToPlay = mOwner.Weapon.AttackAnimOverride;
		}
		return new FUnitActionAttack(mOwner.World.GetNextActionID(), mOwner, targetPoint, true, animToPlay);
	}

	public bool DoWork(FUnit targetUnit)
	{
		if (targetUnit == null)
		{
			return false;
		}
		if (mOwner == null)
		{
			return false;
		}
		TryEquipSpecialWeapon(targetUnit);
		FUnitActionWork workAction = GetAvailableWorkAction(targetUnit);
		int range;
		bool singleAxis;
		if (!targetUnit.IsTag("Attackable") || workAction is FUnitActionInteract || targetUnit.IsTag("Pushable"))
		{
			range = 0;
			singleAxis = true;
		}
		else if (mOwner.Weapon == null)
		{
			range = 0;
			singleAxis = true;
		}
		else
		{
			range = mOwner.Weapon.Range;
			bool adjacent = mOwner.IsAdjacent(targetUnit);
			if (!adjacent && range > 1 && mOwner.Weapon.SingleAxis)
			{
				singleAxis = true;
			}
			else
			{
				if (mOwner.IsPlayer && mOwner.Weapon.Type == "Ranged")
				{
					if (workAction == null)
					{
						return false;
					}
					return AddAction(workAction);
				}
				singleAxis = false;
			}
		}
		FPath path = new FPath();
		FPather.PathResult result = mOwner.World.FindPath(mOwner, mOwner.TilePosition, targetUnit, targetUnit.TilePosition, range, null, path, singleAxis);
		if (result < FPather.PathResult.Partial)
		{
			return false;
		}
		if (result == FPather.PathResult.Complete)
		{
			return DoMoveToPoint(targetUnit, range, path, singleAxis, true, string.Empty, true);
		}
		if (result == FPather.PathResult.Partial)
		{
			DoMoveToPoint(targetUnit, range, path, singleAxis, true, string.Empty, true);
			return true;
		}
		if (workAction == null)
		{
			return false;
		}
		return AddAction(workAction);
	}

	protected virtual void TryEquipSpecialWeapon(FUnit targetUnit)
	{
		mOwner.SyncWeaponWithProto();
	}

	protected FUnitActionWork GetAvailableWorkAction(FUnit targetUnit)
	{
		if (targetUnit.IsTag("Door") && targetUnit.CanBeOpened())
		{
			return new FUnitActionOpenDoor(mOwner.World.GetNextActionID(), mOwner, targetUnit);
		}
		if (targetUnit.IsTag("Pushable"))
		{
			return new FUnitActionPushObject(mOwner.World.GetNextActionID(), mOwner, targetUnit);
		}
		if (targetUnit.IsTag("Openable"))
		{
			if (mOwner.CanOpen(targetUnit))
			{
				return TryAttack(targetUnit);
			}
			string currentCharacter = FApp.PlayerStats.CurrentCharacter;
			string requiredCharacter = targetUnit.ProtoUnit.GetInteractRequirementsRequiredCharacter();
			if (!string.IsNullOrEmpty(requiredCharacter) && requiredCharacter != FApp.PlayerStats.CurrentCharacter && (FApp.PlayerStats.HasCollectible(requiredCharacter) || requiredCharacter == "Hopper"))
			{
				mOwner.World.SetPlayerCharacter(requiredCharacter);
				if (mOwner.CanOpen(targetUnit))
				{
					return TryAttack(targetUnit);
				}
				mOwner.World.SetPlayerCharacter(currentCharacter);
			}
		}
		if (targetUnit.IsTag("Attackable") && CanInteract(targetUnit))
		{
			return TryAttack(targetUnit);
		}
		if (targetUnit.IsTag("Interactable"))
		{
			return new FUnitActionInteract(mOwner.World.GetNextActionID(), mOwner, targetUnit);
		}
		return null;
	}

	protected FUnitActionAttack TryAttack(FUnit targetUnit)
	{
		mOwner.SetLastAttackTarget(targetUnit.ID);
		if (!CanAttack())
		{
			string key = ((mOwner.Weapon == null) ? "CantAttack" : "NoAmmo");
			mOwner.World.SpawnFlyOffText(FLoc.GetString(key), mOwner.Position, new Vector2(32f, 32f));
			return null;
		}
		string animToPlay = "Attack";
		if (mOwner.Weapon != null && mOwner.Weapon.AttackAnimOverride.Valid())
		{
			animToPlay = mOwner.Weapon.AttackAnimOverride;
		}
		return new FUnitActionAttack(mOwner.World.GetNextActionID(), mOwner, targetUnit, true, animToPlay);
	}

	protected bool CanAttack()
	{
		if (mOwner.IsPlayer)
		{
			return mOwner.CanAttackWithWeapon();
		}
		return true;
	}

	public void SetLastAttackTarget(int targetID)
	{
		mLastAttackTargetID = targetID;
		mLastAttackTime = mOwner.World.Gametime;
	}

	public void PutActionOnCooldown(FUnitActionBase action, float duration)
	{
		mActionsOnCooldown[action.GetType()] = mOwner.World.Gametime + duration;
	}

	public bool GetActionTypeOnCooldown(Type actionType)
	{
		return mActionsOnCooldown.ContainsKey(actionType);
	}

	public int FindNextPatrolPointIndex(bool checkPath = true, bool reverse = false)
	{
		if (mPatrolPoints.Count < 2)
		{
			return -1;
		}
		if (mOwner.Room == null)
		{
			return -1;
		}
		int index = CurrentPatrolPointIndex;
		if (index < 0)
		{
			index = 0;
			CurrentPatrolPointIndex = 0;
		}
		int step = (reverse ? (-1) : 1);
		while (true)
		{
			int next = index + step;
			if (next >= mPatrolPoints.Count)
			{
				next = 0;
			}
			index = mPatrolPoints.Count - 1;
			if (next > -1)
			{
				index = next;
			}
			if (index == CurrentPatrolPointIndex)
			{
				return 0;
			}
			FPoint2 point = mPatrolPoints[index];
			Vector3 pos = mOwner.Position;
			float dx = (float)point.x - pos.x;
			float dy = (float)point.y - pos.y;
			float distance = Mathf.Sqrt(dx * dx + dy * dy);
			if (distance < 0.1f || !checkPath)
			{
				if (0.1f <= distance)
				{
					return index;
				}
			}
			else if (mOwner.CanPath(mPatrolPoints[index], true))
			{
				return index;
			}
		}
	}

	public bool IsOnPatrolPoint()
	{
		int i;
		for (i = 0; i < mPatrolPoints.Count && !(mOwner.TilePosition == mPatrolPoints[i]); i++)
		{
		}
		return i < mPatrolPoints.Count;
	}

	public virtual string GetAnimName(string animName)
	{
		if (animName == "Walk" && mOwner.ProtoUnit.WalkAnimOverride.Valid())
		{
			animName = mOwner.ProtoUnit.WalkAnimOverride;
		}
		return animName;
	}
}
