using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUIInventoryQuestItem @ 0x17f85d4..0x17f875c, Ghidra vaddr).
public class FUIInventoryQuestItem : FUIInventoryItem
{
	public int DBID { get; protected set; }

	public FUIInventoryQuestItem(int index, int dbid, string icon, string labelText, string details, Action<string> onRelease)
		: base(index, icon, labelText, details, onRelease)
	{
		DBID = dbid;
	}

	public override void UpdateStatus(bool collected, string icon, string labelText, bool selected)
	{
		Collected = collected;
		Selected = selected;
		if (collected)
		{
			mIcon.SetSprite(icon + (selected ? ".Selected.png" : ".png"));
			mLabel.SetParent(this);
			mLabel.Text = labelText;
		}
		Collected = collected;
		mIcon.Color = Color.white;
	}

	public void UpdateStatus(bool collected)
	{
		Collected = collected;
		mIcon.Color = Color.white;
	}
}
