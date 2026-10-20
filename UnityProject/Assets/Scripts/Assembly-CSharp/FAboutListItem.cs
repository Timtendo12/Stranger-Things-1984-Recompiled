using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FAboutListItem @ 0x1797550..0x179796c, Ghidra vaddr).
public class FAboutListItem : FUIScrollListItem
{
	private const float cTitleBorder = 15f;

	private const float cTextBorder = 40f;

	protected readonly float cTitleLabelWidth;

	protected readonly float cTextLabelWidth;

	protected const float cBetweenItemSpacing = 20f;

	protected readonly float cItemWidth;

	protected FLabel mTitleLabel;

	protected FLabel mDescriptionLabel;

	public FAboutListItem(Vector2 size, string title, string description)
	{
		cItemWidth = size.x;
		cTitleLabelWidth = size.x + -15f + -15f;
		cTextLabelWidth = size.x + -40f + -40f;
		mTitleLabel = new FLabel("DefaultStyle", string.Empty, 36f, false, true, false);
		mTitleLabel.SetParent(this);
		mTitleLabel.Color = FColorNames.Get("LegendRed");
		mTitleLabel.JustificationX = Sides.Left;
		mTitleLabel.JustificationY = Sides.Top;
		mTitleLabel.Text = title;
		mTitleLabel.Wrap = true;
		mTitleLabel.DockInside(this, InnerDock.Top, cTitleLabelWidth, size.y, 0f, -15f);
		mDescriptionLabel = new FLabel("DefaultStyle", string.Empty, 24f, false, true, false);
		mDescriptionLabel.SetParent(this);
		mDescriptionLabel.Color = new Color(1f, 1f, 1f, 1f);
		mDescriptionLabel.JustificationX = Sides.Left;
		mDescriptionLabel.JustificationY = Sides.Top;
		mDescriptionLabel.Wrap = true;
		mDescriptionLabel.Text = description;
		mDescriptionLabel.DockOutside(mTitleLabel, OuterDock.Bottom, cTextLabelWidth, size.y, 0f, -15f);
		RefreshItemSize();
	}

	public override void SetParentContainerCollider(FUICollider parentCollider)
	{
	}

	public void RefreshItemSize()
	{
		float titleHeight = 0f;
		if (!string.IsNullOrEmpty(mTitleLabel.Text))
		{
			titleHeight = mTitleLabel.TextHeight;
		}
		mTitleLabel.DockInside(this, InnerDock.Top, cTitleLabelWidth, titleHeight, 0f, -15f);
		float descriptionHeight = 0f;
		if (!string.IsNullOrEmpty(mDescriptionLabel.Text))
		{
			descriptionHeight = mDescriptionLabel.TextHeight;
		}
		mDescriptionLabel.DockOutside(mTitleLabel, OuterDock.Bottom, cTextLabelWidth, descriptionHeight, 0f, -15f);
		base.ItemSize = new Vector2(cItemWidth, titleHeight + 15f + descriptionHeight + 20f);
	}
}
