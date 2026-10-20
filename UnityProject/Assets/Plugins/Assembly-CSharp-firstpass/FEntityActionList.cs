using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FEntityActionList @ 0x164fee8..0x1650760, Ghidra vaddr).
public class FEntityActionList
{
	private IEntity mOwner;

	protected List<FEntityAction> mActions = new List<FEntityAction>();

	protected List<FEntityAction> mDeferredRemoveActions;

	protected int mDeferringActionsRefCount;

	public List<FEntityAction> Actions => mActions;

	public int Count => mActions.Count;

	public FEntityActionList(IEntity owner)
	{
		mOwner = owner;
	}

	protected virtual void StartDeferringActionRemoval()
	{
		mDeferringActionsRefCount++;
		if (mDeferredRemoveActions == null)
		{
			mDeferredRemoveActions = new List<FEntityAction>();
		}
	}

	protected virtual void StopDeferringActionRemoval()
	{
		mDeferringActionsRefCount--;
		if (mDeferringActionsRefCount < 1)
		{
			List<FEntityAction> list = mDeferredRemoveActions;
			if (list == null)
			{
				FLog.UnityError("mDeferredRemoveActions is null but shouldn't be!");
				return;
			}
			mDeferredRemoveActions = null;
			for (int i = 0; i < list.Count; i++)
			{
				RemoveAction(list[i]);
			}
		}
	}

	public virtual void UpdateActions()
	{
		Fix64 deltaTime = (Fix64)UnityEngine.Time.deltaTime;
		StartDeferringActionRemoval();
		for (int i = 0; i < mActions.Count; i++)
		{
			mActions[i].Update(deltaTime);
		}
		StopDeferringActionRemoval();
	}

	public virtual bool HasAction(FEntityAction action)
	{
		return mActions.Contains(action);
	}

	public virtual bool AddAction(FEntityAction action)
	{
		if (mActions.Contains(action))
		{
			FLog.UnityError("Attempting to add duplicate node action.");
			return false;
		}
		if (action is FEntityActionTweenPropertyBase)
		{
			StopTweenActionsOfProperty(((FEntityActionTweenPropertyBase)action).PropertyName);
		}
		action.SetTarget(mOwner);
		mActions.Add(action);
		action.Start();
		return true;
	}

	public virtual bool RemoveAction(FEntityAction action)
	{
		if (mDeferredRemoveActions == null)
		{
			mActions.Remove(action);
			return true;
		}
		mDeferredRemoveActions.Add(action);
		return false;
	}

	public virtual void StopAllActions(bool fromDamage = false)
	{
		StartDeferringActionRemoval();
		for (int i = 0; i < mActions.Count; i++)
		{
			if (!fromDamage || !mActions[i].IgnoreStopOnDamage)
			{
				mActions[i].Stop();
			}
		}
		StopDeferringActionRemoval();
	}

	public virtual void StopTweenActionsOfProperty(string propertyName)
	{
		StartDeferringActionRemoval();
		for (int i = 0; i < mActions.Count; i++)
		{
			FEntityAction action = mActions[i];
			if (action is FEntityActionTweenPropertyBase && ((FEntityActionTweenPropertyBase)action).PropertyName == propertyName)
			{
				action.Stop();
			}
		}
		StopDeferringActionRemoval();
	}
}
