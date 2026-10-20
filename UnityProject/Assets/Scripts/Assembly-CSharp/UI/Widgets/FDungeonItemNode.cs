using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FDungeonItemNode @ 0x1813e44..0x1814b2c, Ghidra vaddr).
public class FDungeonItemNode : FNode
{
	protected FSprite mItemSprite;

	protected FLabelDropshadow mNumLabel;

	protected int[] mItemDBIDs;

	public FDungeonItemNode(string name, string itemSprite, int[] itemDBIDs)
		: base(name)
	{
		if (itemSprite.Valid() && itemDBIDs != null && itemDBIDs.Length != 0)
		{
			mItemSprite = new FSprite(Name + ".ItemSprite", itemSprite);
			mItemSprite.SetParent(this);
			mNumLabel = new FLabelDropshadow(Name + ".NumLabel", "DefaultStyle", string.Empty, 24f, false, true, false);
			mNumLabel.SetTextColor(FUI.Colors.White, FUI.Colors.Black);
			mNumLabel.Justify(Sides.CenterX, Sides.CenterY);
			mNumLabel.SetParent(this);
			mItemDBIDs = itemDBIDs;
			UpdateCount();
		}
	}

	public override void SelfResizeExtents()
	{
		base.SelfResizeExtents();
		Rect extents = Extents;
		mItemSprite.DockInside(this, InnerDock.Top, extents.width, extents.width, 0f, 0f);
		extents = Extents;
		mNumLabel.DockInside(this, InnerDock.Bottom, extents.width, extents.height - extents.width, 0f, 0f);
	}

	protected void UpdateCount()
	{
		if (mItemDBIDs == null)
		{
			return;
		}
		int count = 0;
		for (int i = 0; i < mItemDBIDs.Length; i++)
		{
			if (FApp.PlayerStats.HasCollectible(mItemDBIDs[i]))
			{
				count++;
			}
		}
		mNumLabel.Text = count.ToString() + "/" + mItemDBIDs.Length.ToString();
	}
}
