using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FCreditsListItem @ 0x179d020..0x179d400, Ghidra vaddr).
public class FCreditsListItem : FUIScrollListItem
{
	private const float cTitleBorder = 15f;

	private const float cTextBorder = 40f;

	protected readonly float cTitleLabelWidth;

	protected readonly float cTextLabelWidth;

	protected FLabel mTitleLabel;

	protected FLabel mDescriptionLabel;

	protected readonly float cItemWidth;

	protected const float cBetweenItemSpacing = 20f;

	public FCreditsListItem(Vector2 size, string title, string description)
	{
		cItemWidth = size.x;
		cTitleLabelWidth = size.x + -15f + -15f;
		cTextLabelWidth = size.x + -40f + -40f;
		mTitleLabel = new FLabel("DefaultStyle", string.Empty, 48f, true, true, false);
		mTitleLabel.Color = FUI.Colors.UIEmphasis;
		mTitleLabel.JustificationX = Sides.Left;
		mTitleLabel.JustificationY = Sides.Top;
		mTitleLabel.Text = title;
		mTitleLabel.SetParent(this);
		mTitleLabel.DockInside(this, InnerDock.Top, cTitleLabelWidth, size.y, 0f, -15f);
		mDescriptionLabel = new FLabel("DefaultStyle", string.Empty, 36f, true, true, false);
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
		float num = 15f;
		if (mTitleLabel != null)
		{
			float num2 = 0f;
			if (!string.IsNullOrEmpty(mTitleLabel.Text))
			{
				num2 = mTitleLabel.TextHeight;
			}
			mTitleLabel.DockInside(this, InnerDock.Top, cTitleLabelWidth, num2, 0f, -15f);
			num = num2 + 15f;
		}
		float num3 = 0f;
		if (mDescriptionLabel != null)
		{
			if (!string.IsNullOrEmpty(mDescriptionLabel.Text))
			{
				num3 = mDescriptionLabel.TextHeight;
			}
			mDescriptionLabel.DockOutside(mTitleLabel, OuterDock.Bottom, cTextLabelWidth, num3, 0f, -15f);
		}
		base.ItemSize = new Vector2(cItemWidth, num + num3 + 20f);
	}
}
