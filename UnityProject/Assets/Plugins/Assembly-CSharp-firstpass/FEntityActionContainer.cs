using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FEntityActionContainer @ 0x164c734..0x164cad8, Ghidra vaddr).
public abstract class FEntityActionContainer : FEntityAction
{
	protected List<FEntityAction> mChildren = new List<FEntityAction>();

	public List<FEntityAction> Children => mChildren;

	public FEntityActionContainer()
	{
	}

	public FEntityActionContainer(params FEntityAction[] actions)
	{
		for (int i = 0; i < actions.Length; i++)
		{
			mChildren.Add(actions[i]);
			actions[i].SetParent(this);
		}
	}

	public void AddAction(FEntityAction action)
	{
		mChildren.Add(action);
		action.SetParent(this);
	}

	public override void OnSetTarget()
	{
		for (int i = 0; i < mChildren.Count; i++)
		{
			mChildren[i].SetTarget(mTarget);
		}
	}
}
