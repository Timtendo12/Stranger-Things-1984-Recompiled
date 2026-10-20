// Rewritten from libil2cpp.so 1.0.605 (FEntityActionSpawn @ 0x164d0fc..0x164d3bc, Ghidra vaddr).
public class FEntityActionSpawn : FEntityActionContainer
{
	public FEntityActionSpawn(params FEntityAction[] actions)
		: base(actions)
	{
	}

	public override void OnSetTarget()
	{
		base.OnSetTarget();
		mDuration = Fix64.Zero;
		for (int i = 0; i < mChildren.Count; i++)
		{
			if (mChildren[i].IndeterminateLength)
			{
				mDuration = FEntityAction.Indeterminate;
				return;
			}
			mDuration = Fix64.Max(mDuration, mChildren[i].Duration);
		}
	}

	public override bool OnUpdate(Fix64 deltaTime)
	{
		bool allStopped = true;
		for (int i = 0; i < mChildren.Count; i++)
		{
			mChildren[i].Update(deltaTime);
			allStopped = mChildren[i].State == FEntityActionState.Stopped && allStopped;
		}
		return !allStopped;
	}

	public override void OnStart()
	{
		for (int i = 0; i < mChildren.Count; i++)
		{
			mChildren[i].Start();
		}
	}
}
