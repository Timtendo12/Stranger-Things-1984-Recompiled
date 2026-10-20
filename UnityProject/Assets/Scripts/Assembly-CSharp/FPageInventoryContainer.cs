using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageInventoryContainer @ 0x17f9e7c..0x17fa528, Ghidra vaddr).
public class FPageInventoryContainer : FNode
{
	protected Vector2 mSize = Vector2.zero;

	protected FSlicedSprite mBorder;

	protected FSprite mTitleBackground;

	protected FLabel mTitle;

	protected FUIScrollListGrid mGrid;

	protected List<FUIScrollListItem> mGridItems = new List<FUIScrollListItem>();

	public List<FUIScrollListItem> GridItems => mGridItems;

	public FPageInventoryContainer(string name, string titleLoc)
		: base(name)
	{
		mBorder = new FSlicedSprite(Name + ".Border", "DialogBorder001.png", -1f);
		mBorder.SetParent(this);
		mTitleBackground = new FSprite(Name + ".TitleBackground", "BlackSquare.png");
		mTitleBackground.SetParent(this);
		mTitle = new FLabel(Name + ".Title", "DefaultStyle", FLoc.GetString(titleLoc), 16f, true, true, false);
		mTitle.SetParent(this);
		mGrid = new FUIScrollListGrid(Name + ".Grid", Vector2.zero, Vector2.zero, false);
		mGrid.SetParent(this);
	}

	public void Layout(Vector2 size, Vector2 gridSize, Vector2 titleSize, Vector2 itemSize, List<FUIScrollListItem> items)
	{
		mSize = size;
		mBorder.DockInside(this, InnerDock.Center, size.x, size.y, 0f, 0f);
		mTitleBackground.DockInside(this, InnerDock.TopLeft, titleSize.x, titleSize.y, titleSize.y * 0.5f, titleSize.y * 0.5f);
		mTitle.DockInside(mTitleBackground, InnerDock.Center, titleSize.x, titleSize.y, 0f, 0f);
		mGrid.DockInside(this, InnerDock.Center, gridSize.x, gridSize.y, 0f, 0f);
		if (items == null)
		{
			return;
		}
		mGrid.RemoveItems();
		mGridItems.Clear();
		mGrid.UpdateSizes(gridSize, itemSize);
		for (int i = 0; i < items.Count; i++)
		{
			mGrid.AddItem(items[i]);
			mGridItems.Add(items[i]);
		}
		mGrid.LayoutListContents();
	}
}
