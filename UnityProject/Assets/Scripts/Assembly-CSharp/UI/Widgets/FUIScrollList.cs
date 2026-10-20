using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUIScrollList @ 0x17d9940..0x17db3cc, Ghidra vaddr).
public abstract class FUIScrollList : FScrollNode
{
	protected List<FUIScrollListItem> mListItems = new List<FUIScrollListItem>();

	[CompilerGenerated]
	private Action m_mSignalDeselect;

	protected Vector2 mDefaultListSize;

	protected Vector2 mDefaultListItemSize;

	public Vector2 ListContainerPosition => mNodeToScroll.Position;

	public float DeselectDistance { get; set; }

	public List<FUIScrollListItem> ListItems => mListItems;

	public int NumItems => mListItems.Count;

	protected event Action mSignalDeselect;

	public FUIScrollList(string name, Vector2 defaultListSize, Vector2 defaultListItemSize)
		: base(name)
	{
		mCanZoom = false;
		mDefaultListSize = defaultListSize;
		mMinZoomScale = 1f;
		mMaxZoomScale = 1f;
		mDefaultListItemSize = defaultListItemSize;
		DeselectDistance = Mathf.Min(defaultListSize.x, defaultListSize.y) * 0.5f;
		mCollider.SetParent(this);
		mCollider.ExpandToFill(this);
		mNodeToScroll = new FDimNode(Name + "ListContainer");
		mNodeToScroll.SetParent(this);
		mNodeToScroll.DockInside(this, InnerDock.Top, defaultListSize.x, defaultListSize.y);
	}

	public abstract void LayoutListContents();

	protected override void HandleTapMove(FUICollider collider, FTouch touch)
	{
		base.HandleTapMove(collider, touch);
		if (mTouchStarted && this.mSignalDeselect != null)
		{
			Vector2 vector = mNodeToScroll.Position - mDragStartScrollPos;
			if (DeselectDistance * DeselectDistance < vector.x * vector.x + vector.y * vector.y)
			{
				this.mSignalDeselect();
			}
		}
	}

	public virtual void UpdateSizes(Vector2 defaultListSize, Vector2 defaultListItemSize)
	{
		mDefaultListSize = defaultListSize;
		mDefaultListItemSize = defaultListItemSize;
		DeselectDistance = Mathf.Min(defaultListSize.x, defaultListSize.y) * 0.5f;
		mNodeToScroll.DockInside(this, InnerDock.Top, defaultListSize.x, defaultListSize.y);
	}

	public void AddSignalDeselect(Action d, float distance = 0f)
	{
		this.mSignalDeselect -= d;
		this.mSignalDeselect += d;
		DeselectDistance = distance;
	}

	public override void SelfResizeExtents()
	{
		Vector2 scrollPosition = ScrollPosition;
		base.SelfResizeExtents();
		ScrollPosition = scrollPosition;
	}

	public FUIScrollListItem GetItem(int index)
	{
		if (index >= 0 && index < mListItems.Count)
		{
			return mListItems[index];
		}
		return null;
	}

	public bool AddItem(FUIScrollListItem node)
	{
		if (node == null || mNodeToScroll == null)
		{
			return false;
		}
		if (mListItems.Contains(node))
		{
			return false;
		}
		mListItems.Add(node);
		node.SetParent(mNodeToScroll);
		node.List = this;
		node.MarkDirtyRecursive();
		node.SetParentContainerCollider(mCollider);
		return true;
	}

	public void RemoveItems(bool fullCleanUpNodes = true)
	{
		for (int i = 0; i < mListItems.Count; i++)
		{
			FUIScrollListItem node = mListItems[i];
			if (fullCleanUpNodes)
			{
				FNode.Cleanup(node);
			}
			else
			{
				node.Unparent();
				node.RemoveConstraints();
			}
		}
		mListItems.Clear();
	}

	public void UnselectAllItems()
	{
		for (int i = 0; i < mListItems.Count; i++)
		{
			mListItems[i].Select(v: false);
		}
	}

	public void SelectItem(int index)
	{
		if (index >= 0 && index < mListItems.Count)
		{
			mListItems[index].Select(v: true);
		}
	}

	public FUIScrollListItem GetHoverItem()
	{
		Vector2 mousePosition = Futile.GetMousePosition();
		if (!mCollider.ContainsPoint(mousePosition))
		{
			return null;
		}
		for (int i = 0; i < mListItems.Count; i++)
		{
			if (mListItems[i].IsMouseOver(mousePosition))
			{
				return mListItems[i];
			}
		}
		return null;
	}

	public abstract void ScrollToIndex(int index, bool center);
}
