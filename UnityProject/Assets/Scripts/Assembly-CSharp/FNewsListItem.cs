using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FNewsListItem @ 0x17a7dd0..0x17a84d4, Ghidra vaddr).
public class FNewsListItem : FUIScrollListItem
{
	protected const float cTitleBorder = 15f;

	protected const float cTextBorder = 40f;

	protected readonly float cTitleLabelWidth;

	protected readonly float cTextLabelWidth;

	protected const float cMoreInfoButtonHeight = 80f;

	protected const float cMoreInfoButtonWidth = 250f;

	protected const float cBetweenItemSpacing = 60f;

	protected readonly float cItemWidth;

	protected FLabel mTitleLabel;

	protected FLabel mDescriptionLabel;

	protected FUIButton mMoreInfoButton;

	protected string mURL;

	public FNewsListItem(FUIScrollList scrollList, float width, string title, string description, string url)
	{
		mURL = url;
		cItemWidth = width;
		cTitleLabelWidth = width + -15f + -15f;
		cTextLabelWidth = width + -40f + -40f;
		bool flag = string.IsNullOrEmpty(mURL);
		mMoreInfoButton = new FUIButton("MoreInfoButton");
		mMoreInfoButton.SetParent(this);
		mMoreInfoButton.AddHandleRelease(HandleMoreInfoButton);
		mMoreInfoButton.SetBackground("ButtonInternal.png");
		mMoreInfoButton.Collider.SetParentContainerCollider(scrollList.Collider);
		mMoreInfoButton.SetText(FLoc.GetString("NewsDialog.MoreInfoButton"), "DefaultStyle", 48f, 14f);
		mMoreInfoButton.DockInside(this, InnerDock.TopRight, 250f, 80f, -15f, 0f);
		mMoreInfoButton.Visible = !flag;
		mTitleLabel = new FLabel("DefaultStyle", string.Empty, 48f, true, true, false);
		mTitleLabel.SetParent(this);
		mTitleLabel.Color = FUI.Colors.UIEmphasis;
		mTitleLabel.JustificationX = Sides.Left;
		mTitleLabel.JustificationY = Sides.Top;
		mTitleLabel.Text = title;
		mTitleLabel.DockInside(this, InnerDock.Left, flag ? cTitleLabelWidth : (cTitleLabelWidth + -250f), 52f, 15f, 0f);
		mDescriptionLabel = new FLabel("DefaultStyle", string.Empty, 36f, false, true, false);
		mDescriptionLabel.SetParent(this);
		mDescriptionLabel.JustificationX = Sides.Left;
		mDescriptionLabel.JustificationY = Sides.Top;
		mDescriptionLabel.Color = new Color(1f, 1f, 1f, 1f);
		mDescriptionLabel.Wrap = true;
		mDescriptionLabel.Text = description;
		mDescriptionLabel.DockOutside(mTitleLabel, OuterDock.Bottom, cTextLabelWidth, 100f, 0f, -15f);
		RefreshItemSize();
	}

	public override void SetParentContainerCollider(FUICollider parentCollider)
	{
	}

	public void RefreshItemSize()
	{
		float num = 0f;
		if (!string.IsNullOrEmpty(mTitleLabel.Text))
		{
			num = mTitleLabel.TextHeight;
		}
		bool flag = string.IsNullOrEmpty(mURL);
		mTitleLabel.DockInside(this, InnerDock.Top, flag ? cTitleLabelWidth : (cTitleLabelWidth + -250f), num, -15f, 0f);
		float num2 = 0f;
		if (!string.IsNullOrEmpty(mDescriptionLabel.Text))
		{
			num2 = mDescriptionLabel.TextHeight;
		}
		mDescriptionLabel.DockOutside(mTitleLabel, OuterDock.Bottom, cTextLabelWidth, num2, 0f, -15f);
		base.ItemSize = new Vector2(cItemWidth, num2 + 95f + 60f);
	}

	protected void HandleMoreInfoButton(FUIButton button, FTouch touch)
	{
		if (!string.IsNullOrEmpty(mURL))
		{
			FApp.SoundManager.PlaySoundSet("UIButton");
			FPluginRelay.Instance.HandleCustomURL(mURL);
		}
	}
}
