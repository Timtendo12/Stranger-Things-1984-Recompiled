using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FSaveRecoveryNode @ 0x180faf4..0x18101f0, Ghidra vaddr).
public class FSaveRecoveryNode : FUIButton
{
	protected FLabel mNormalSave;

	protected FLabel mClassicSave;

	protected FPlayerStats mPlayerStats;

	public FPlayerStats PlayerStats => mPlayerStats;

	public FSaveRecoveryNode(string name, string titleText)
		: base(name)
	{
		SetText(titleText, "DefaultStyle", 48f, 0f);
		mLabel.Color = FUI.Colors.LegendRed;
		mNormalSave = new FLabel(Name + ".OtherInfo", "DefaultStyle", string.Empty, 24f, true, true, false);
		mNormalSave.Justify(FNode.Sides.CenterX, FNode.Sides.CenterY);
		mNormalSave.SetParent(this);
		mNormalSave.Wrap = true;
		mClassicSave = new FLabel(Name + ".OtherInfo", "DefaultStyle", string.Empty, 24f, true, true, false);
		mClassicSave.Justify(FNode.Sides.CenterX, FNode.Sides.CenterY);
		mClassicSave.SetParent(this);
		mClassicSave.Wrap = true;
	}

	public override void SelfResizeExtents()
	{
		base.SelfResizeExtents();
		float width = Extents.width - 32f;
		mLabel.DockInside(this, InnerDock.Top, width, 48f, 0f, -16f);
		mNormalSave.DockOutside(mLabel, OuterDock.Bottom, width, 24f, 0f, 0f);
		mClassicSave.DockOutside(mNormalSave, OuterDock.Bottom, width, 24f, 0f, 0f);
	}

	public void SetSaveInfo(FPlayerStats stats)
	{
		mPlayerStats = stats;
		if (stats.SwitchToFirstSaveSlotOfType(0))
		{
			float percent = stats.GetPercentComplete();
			if (percent <= 0f)
			{
				mNormalSave.Text = string.Empty;
			}
			else
			{
				mNormalSave.Text = string.Format("{0}: {1:0.0}%", FLoc.GetString("NormalButton"), percent);
			}
		}
		if (stats.SwitchToFirstSaveSlotOfType(1))
		{
			float percent2 = stats.GetPercentComplete();
			if (percent2 <= 0f)
			{
				mClassicSave.Text = string.Empty;
			}
			else
			{
				mClassicSave.Text = string.Format("{0}: {1:0.0}%", FLoc.GetString("ClassicButton"), percent2);
			}
		}
	}
}
