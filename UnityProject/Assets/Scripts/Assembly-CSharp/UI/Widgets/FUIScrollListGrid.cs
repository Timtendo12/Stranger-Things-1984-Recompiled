using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUIScrollListGrid @ 0x17db3cc..0x17dbdc4, Ghidra vaddr).
public class FUIScrollListGrid : FUIScrollList
{
	public const float cBeginFadeoutMinGlideVelocity = 0.1f;

	protected FUIScrollbar mScrollbar;

	public Vector2 GridMargin { get; set; }

	public Vector2 MouseScrollIncrement { get; set; }

	public FUIScrollListGrid(string name, Vector2 listSize, Vector2 defaultItemSize, bool allocateScrollBar = true)
		: base(name, listSize, defaultItemSize)
	{
		MouseScrollIncrement = new Vector2(0f, 20f);
		GridMargin = Vector2.zero;
		if (allocateScrollBar)
		{
			mScrollbar = new FUIScrollbar(name + ".scrollbar", this, new Vector2(FUIScrollbar.cDefaultButtonSizeMobile.x, listSize.y));
			mScrollbar.SetParent(this);
			mScrollbar.DockCustom(this, HorizSide.Right, 0f - FUIScrollbar.cDefaultButtonSizeMobile.x, this, HorizSide.Right, 0f, this, VertSide.Top, 0f, this, VertSide.Bottom, 0f);
		}
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
		if (Input.mousePresent)
		{
			float axis = Input.GetAxis("Mouse ScrollWheel");
			Vector2 vector = ScreenToLocal(FUtil.mousePosition);
			Rect extents = mCollider.Extents;
			bool inside = extents.x <= vector.x && vector.x <= extents.x + extents.width && extents.y <= vector.y && vector.y <= extents.y + extents.height;
			if (inside)
			{
				if (axis > 0f)
				{
					ScrollUp(MouseScrollIncrement.y);
				}
				else if (axis < 0f)
				{
					ScrollDown(MouseScrollIncrement.y);
				}
			}
		}
		if (mScrollbar != null)
		{
			mScrollbar.ShouldBeVisible = !mScrollbar.UseMobileScrollbarStyle || mDragging || Mathf.Abs(mVelocity.y) > 0.1f;
		}
	}

	public override void UpdateSizes(Vector2 listSize, Vector2 defaultItemSize)
	{
		base.UpdateSizes(listSize, defaultItemSize);
		if (mScrollbar != null)
		{
			mScrollbar.UpdateSize(new Vector2(FUIScrollbar.cDefaultButtonSizeMobile.x, listSize.y));
		}
	}

	public override void LayoutListContents()
	{
		mCollider.ExpandToFill(this);
		mNodeToScroll.Position = Vector2.zero;
		float maxX = 0f;
		float rowHeight = 0f;
		float x = 0f;
		float y = 0f;
		for (int i = 0; i < mListItems.Count; i++)
		{
			FUIScrollListItem fUIScrollListItem = mListItems[i];
			Vector2 vector = (fUIScrollListItem.HasItemSize ? fUIScrollListItem.ItemSize : mDefaultListItemSize);
			fUIScrollListItem.DockInside(mNodeToScroll, InnerDock.TopLeft, vector.x, vector.y, x, y);
			x += vector.x;
			maxX = Mathf.Max(x, maxX);
			rowHeight = Mathf.Max(vector.y, rowHeight);
			if (i < mListItems.Count - 1)
			{
				float nextWidth = mDefaultListItemSize.x;
				FUIScrollListItem next = mListItems[i + 1];
				if (next.HasItemSize)
				{
					nextWidth = next.ItemSize.x;
				}
				x += GridMargin.x;
				if (mDefaultListSize.x < nextWidth + x)
				{
					x = 0f;
					y -= rowHeight + GridMargin.y;
					rowHeight = 0f;
				}
			}
		}
		mNodeToScroll.DockInside(this, InnerDock.TopLeft, Mathf.Max(mDefaultListSize.x, maxX), Mathf.Max(mDefaultListSize.y, 0f - (y - rowHeight)), 0f, 0f);
		MarkDirtyRecursive();
	}

	public float GetMinScrollYToShowIndex(int index)
	{
		float num = GetMinScrollY();
		for (int i = 0; i < index && i < mListItems.Count; i++)
		{
			num += mListItems[i].ItemSize.y;
		}
		return num;
	}

	public float GetMaxScrollYToShowIndex(int index)
	{
		float num = GetMaxScrollY();
		for (int num2 = mListItems.Count - 1; num2 > Mathf.Max(index, -1); num2--)
		{
			num -= mListItems[num2].ItemSize.y;
		}
		return num;
	}

	public override void ScrollToIndex(int index, bool center)
	{
		FUIScrollListItem item = GetItem(index);
		if (item == null)
		{
			return;
		}
		float num = GetMinScrollYToShowIndex(index);
		float maxScrollYToShowIndex = GetMaxScrollYToShowIndex(index);
		if (!center)
		{
			float y = ScrollPosition.y;
			if (y <= num && item.ItemSize.y <= ScrollNodeHeight)
			{
				num = maxScrollYToShowIndex;
				if (maxScrollYToShowIndex <= y)
				{
					return;
				}
			}
		}
		else
		{
			num = (num + maxScrollYToShowIndex) * 0.5f;
		}
		SetScrollPosition(ScrollPosition.x, num);
	}
}
