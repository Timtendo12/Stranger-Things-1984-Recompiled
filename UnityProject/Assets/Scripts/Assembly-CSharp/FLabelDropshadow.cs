using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FLabelDropshadow @ 0x1805000..0x180be58, Ghidra vaddr).
public class FLabelDropshadow : FNode
{
	protected FLabel mLabel;

	protected FLabel mDropShadow;

	public string Text
	{
		get
		{
			return mLabel.Text;
		}
		set
		{
			mLabel.Text = value;
			mDropShadow.Text = value;
		}
	}

	public FLabelDropshadow(string name, string fontStyleName, string text, float pointSize, bool shrinkToFitContainer = true, bool usesMarkup = true, bool convertToEllipses = false)
		: base(name)
	{
		mDropShadow = new FLabel(Name + "DropShadow", fontStyleName, text, pointSize, shrinkToFitContainer, usesMarkup, convertToEllipses);
		mDropShadow.Color = new Color(0f, 0f, 1f, 1f);
		mDropShadow.SetParent(this);
		mLabel = new FLabel(Name + "DropShadow", fontStyleName, text, pointSize, shrinkToFitContainer, usesMarkup, convertToEllipses);
		mLabel.SetParent(this);
	}

	public void SetTextColor(Color color, Color dropShadowColor)
	{
		mLabel.Color = color;
		mDropShadow.Color = dropShadowColor;
	}

	public void Wrap(bool wrap)
	{
		mLabel.Wrap = wrap;
		mDropShadow.Wrap = wrap;
	}

	public void Justify(Sides jx, Sides jy = Sides.CenterY)
	{
		mDropShadow.Justify(jx, jy);
		mLabel.Justify(jx, jy);
	}

	public override void SelfResizeExtents()
	{
		base.SelfResizeExtents();
		Layout();
	}

	protected void Layout()
	{
		Rect extents = Extents;
		mDropShadow.DockInside(this, InnerDock.Center, extents.width, extents.height, 2f, -2f);
		extents = Extents;
		mLabel.DockInside(this, InnerDock.Center, extents.width, extents.height, 0f, 0f);
	}

	public void ShrinkToFitContainer(bool which)
	{
		mDropShadow.ShrinkToFitContainer(which);
		mLabel.ShrinkToFitContainer(which);
	}
}
