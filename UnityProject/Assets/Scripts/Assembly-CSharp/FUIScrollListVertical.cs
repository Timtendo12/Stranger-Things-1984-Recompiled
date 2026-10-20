using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUIScrollListVertical @ 0x17d96bc..0x17da2bc, Ghidra vaddr).
public class FUIScrollListVertical : FUIScrollList
{
	private const float cBeginFadeoutMinGlideVelocity = 0.1f;

	private FUIScrollbar mScrollbar;

	public Vector2 ItemMargin { get; set; }

	public FUIScrollListVertical(string name, Vector2 defaultListSize, float defaultItemHeight, bool allocateScrollBar = true)
		: base(name, defaultListSize, new Vector2(defaultListSize.x - FUIScrollbar.cDefaultButtonSizeMobile.x, defaultItemHeight))
	{
		base.ClipToExtents = true;
		ItemMargin = Vector2.zero;
		mCanScrollX = false;
		mCanScrollY = true;
		if (allocateScrollBar)
		{
			mScrollbar = new FUIScrollbar(name + ".scrollbar", this, new Vector2(FUIScrollbar.cDefaultButtonSizeMobile.x, defaultListSize.y));
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
					ScrollUp(20f);
				}
				else if (axis < 0f)
				{
					ScrollDown(20f);
				}
			}
		}
		if (mScrollbar != null)
		{
			mScrollbar.ShouldBeVisible = !mScrollbar.UseMobileScrollbarStyle || mDragging || Mathf.Abs(mVelocity.y) > 0.1f;
		}
		MarkDirtyRecursive();
	}

	public override void UpdateSizes(Vector2 defaultListSize, Vector2 defaultListItemSize)
	{
		base.UpdateSizes(defaultListSize, defaultListItemSize);
		if (mScrollbar != null)
		{
			mScrollbar.UpdateSize(new Vector2(FUIScrollbar.cDefaultButtonSizeMobile.x, defaultListSize.y));
		}
	}

	public override void LayoutListContents()
	{
		mCollider.ExpandToFill(this);
		mNodeToScroll.Position = Vector2.zero;
		float num = 0f;
		for (int i = 0; i < mListItems.Count; i++)
		{
			if (i != 0)
			{
				num += ItemMargin.y;
			}
			FUIScrollListItem fUIScrollListItem = mListItems[i];
			Vector2 vector = (fUIScrollListItem.HasItemSize ? fUIScrollListItem.ItemSize : mDefaultListItemSize);
			fUIScrollListItem.DockInside(mNodeToScroll, InnerDock.TopLeft, vector.x, vector.y, 0f, 0f - num);
			num += vector.y;
		}
		mNodeToScroll.DockInside(this, InnerDock.TopLeft, mDefaultListSize.x, Mathf.Max(mDefaultListSize.y, num), 0f, 0f);
		MarkDirtyRecursive();
	}

	public float GetScrollToItemY(int index, bool centerItem)
	{
		float num;
		if (GetItem(index) == null)
		{
			num = 0f;
		}
		else
		{
			float center = 0f;
			float bottom = 0f;
			if (index >= 0 && index < mListItems.Count)
			{
				float top = 0f;
				for (int i = 0; i < mListItems.Count; i++)
				{
					if (i != 0)
					{
						top += ItemMargin.y;
					}
					FUIScrollListItem fUIScrollListItem = mListItems[i];
					float height = (fUIScrollListItem.HasItemSize ? fUIScrollListItem.ItemSize.y : mDefaultListItemSize.y);
					bottom = top + height;
					if (index == i)
					{
						center = (top + bottom) * 0.5f;
						break;
					}
					top = bottom;
					if (i == mListItems.Count - 1)
					{
						center = 0f;
						bottom = 0f;
					}
				}
			}
			float scrollNodeHeight = ScrollNodeHeight;
			num = (centerItem ? (center - scrollNodeHeight * 0.5f) : (bottom - scrollNodeHeight * 0.5f));
		}
		float minScrollY = GetMinScrollY();
		float maxScrollY = GetMaxScrollY();
		if (num >= minScrollY)
		{
			return Mathf.Min(num, maxScrollY);
		}
		return minScrollY;
	}

	public override void ScrollToIndex(int index, bool center)
	{
		if (GetItem(index) != null)
		{
			float scrollToItemY = GetScrollToItemY(index, center);
			SetScrollPosition(ScrollPosition.x, scrollToItemY);
			mGliding = false;
			mDragging = false;
			mVelocityEntries.Clear();
			mVelocity = Vector2.zero;
		}
	}
}
