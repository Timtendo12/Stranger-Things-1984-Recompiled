using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FStringPickerEntry @ 0x17ae304..0x17aeb6c, Ghidra vaddr).
public class FStringPickerEntry : FUIScrollListItem
{
	protected FUICollider mCollider;

	protected FSlicedSprite mBackground;

	protected string mEntryName;

	protected FLabel mEntryLabel;

	protected FLabel mDetailsLabel;

	protected FSprite mIcon;

	protected FPageStringPicker mPageStringPickerParent;

	protected int mIndex = -1;

	protected bool mSelectable = true;

	public FStringPickerEntry(FPageStringPicker parent, int index, string name, string visibleName, string details, string icon, bool current, bool selectable)
	{
		mPageStringPickerParent = parent;
		mIndex = index;
		mEntryName = name;
		mSelectable = selectable;
		mCollider = new FUICollider("StringPickerEntryCollider", false);
		mCollider.SetParent(this);
		mCollider.AddSignalRelease(HandleRelease, 40f);
		mCollider.ExpandToFill(this, 0f, 0f);
		mBackground = new FSlicedSprite("UIBox001.png");
		mBackground.SetParent(this);
		mBackground.DockInside(this, InnerDock.Center, parent.EntrySize);
		mIcon = new FSprite();
		mIcon.SetParent(this);
		if (!string.IsNullOrEmpty(icon) && Futile.AtlasManager.GetElementWithName(icon, true) != null)
		{
			mIcon.SetSprite(icon);
		}
		else
		{
			mIcon.Visible = false;
		}
		float num = parent.EntrySize.y - 8f;
		mIcon.DockInside(this, InnerDock.Left, num, num, parent.EntryMargin.x + parent.EntryMargin.x, 0f);
		mEntryLabel = new FLabel("DefaultStyle", string.Empty, 48f, true, true, false);
		mEntryLabel.SetParent(this);
		mEntryLabel.Color = ((!mSelectable) ? Color.grey : ((!current) ? FUI.Colors.UITitle : FUI.Colors.UIEmphasis));
		mEntryLabel.Text = visibleName;
		mEntryLabel.ShrinkToFitContainer(true);
		FNode leftRef = (mIcon.Visible ? mIcon : this);
		HorizSide leftSide = (mIcon.Visible ? HorizSide.Right : HorizSide.Left);
		mEntryLabel.DockCustom(leftRef, leftSide, parent.EntryMargin.x, this, HorizSide.Right, 0f - parent.EntryMargin.x, mBackground, VertSide.Top, 0f - parent.EntryMargin.y, mBackground, VertSide.Top, -48f - parent.EntryMargin.y);
		mEntryLabel.DockCustom(leftRef, leftSide, parent.EntryMargin.x, this, HorizSide.Right, 0f - parent.EntryMargin.x, mBackground, VertSide.Top, 0f - parent.EntryMargin.y, mBackground, VertSide.Top, 0f - mEntryLabel.TextHeight - parent.EntryMargin.y);
		if (string.IsNullOrEmpty(details))
		{
			details = string.Empty;
		}
		mDetailsLabel = new FLabel("DefaultStyle", details, 36f, false, true, false);
		mDetailsLabel.SetParent(this);
		mDetailsLabel.Wrap = true;
		mDetailsLabel.ClipWidth = parent.EntrySize.x - mIcon.Extents.width - (parent.EntryMargin.x + parent.EntryMargin.x);
		mDetailsLabel.Color = ((!mSelectable) ? Color.grey : ((!current) ? FUI.Colors.UITitle : FUI.Colors.UIEmphasis));
		mDetailsLabel.DockCustom(leftRef, leftSide, parent.EntryMargin.x, this, HorizSide.Right, 0f - parent.EntryMargin.x, mEntryLabel, VertSide.Bottom, 0f, this, VertSide.Bottom, parent.EntryMargin.y);
		base.ItemSize = parent.EntrySize;
	}

	public override void SetParentContainerCollider(FUICollider parentCollider)
	{
		if (mCollider != null)
		{
			mCollider.SetParentContainerCollider(parentCollider);
		}
	}

	public override void SelfResizeExtents()
	{
		if (HasConstraints() && CalculateExtents())
		{
			Rect extents = Extents;
			Position = new Vector2(extents.x + extents.width * 0.5f, extents.y + extents.height * 0.5f);
		}
	}

	protected void HandleRelease(FUICollider collider, FTouch touch)
	{
		if (mPageStringPickerParent != null && mSelectable)
		{
			mPageStringPickerParent.SelectEntry(mIndex, mEntryName);
		}
	}
}
