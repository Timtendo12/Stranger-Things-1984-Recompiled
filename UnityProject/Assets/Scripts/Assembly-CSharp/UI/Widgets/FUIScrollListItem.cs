using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUIScrollListItem @ 0x17dbdc4..0x17dbed8, Ghidra vaddr).
public class FUIScrollListItem : FNode
{
	protected Vector2 mItemSize;

	protected bool mHasItemSize;

	public FUIScrollList List { get; set; }

	public Vector2 ItemSize
	{
		get
		{
			return mItemSize;
		}
		set
		{
			mItemSize = value;
			mHasItemSize = true;
		}
	}

	public bool HasItemSize => mHasItemSize;

	public virtual void Select(bool v, bool force = false)
	{
		// base implementation is a no-op
	}

	public virtual bool IsMouseOver()
	{
		return false;
	}

	public virtual bool IsMouseOver(Vector2 mousePos)
	{
		return false;
	}

	public virtual void SetParentContainerCollider(FUICollider parentCollider)
	{
		throw new System.NotImplementedException("You must implment this function, even if it is a no-op");
	}
}
