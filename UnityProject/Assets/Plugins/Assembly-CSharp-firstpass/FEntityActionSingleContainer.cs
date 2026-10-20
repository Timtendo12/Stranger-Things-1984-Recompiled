// Rewritten from libil2cpp.so 1.0.605 (FEntityActionSingleContainer @ 0x164c67c..0x164c734, Ghidra vaddr).
public abstract class FEntityActionSingleContainer : FEntityAction
{
	protected FEntityAction mChild;

	public FEntityAction Child => mChild;

	public FEntityActionSingleContainer(FEntityAction child)
	{
		mChild = child;
		child.SetParent(this);
	}

	public override void OnSetTarget()
	{
		mChild.SetTarget(mTarget);
	}
}
