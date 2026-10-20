using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUIInventoryItem @ 0x17f7f90..0x17f85d4, Ghidra vaddr).
public class FUIInventoryItem : FUIScrollListItem
{
	protected FUICollider mCollider;

	protected FSprite mIcon;

	protected FLabelDropshadow mLabel;

	protected Action<string> mOnRelease;

	public int Index { get; protected set; }

	public string Details { get; protected set; }

	public bool Collected { get; protected set; }

	public bool Selected { get; protected set; }

	public FUIInventoryItem(int index, string icon, string labelText, string details, Action<string> onRelease)
	{
		Index = index;
		Details = details;
		Name = "InventoryItem." + Index.ToString();
		mOnRelease = onRelease;
		mCollider = new FUICollider(Name + ".Collider");
		mCollider.SetParent(this);
		mCollider.AddSignalRelease(HandleRelease, 40f);
		mCollider.ExpandToFill(this, 0f, 0f);
		string element = ((Futile.AtlasManager.GetElementWithName(icon, true) != null) ? icon : "BlackSquare.png");
		mIcon = new FSprite(Name + ".StateIcon", element);
		mIcon.ExpandToFill(this, 0f, 0f);
		mIcon.SetParent(this);
		mLabel = new FLabelDropshadow(Name + ".Title", "DefaultStyle", labelText, 24f, true, true, false);
		mLabel.ShrinkToFitContainer(true);
		mLabel.ExpandToFill(this, 0f, 0f);
		mLabel.SetParent(this);
		MarkDirtyRecursive();
	}

	public override void SetParentContainerCollider(FUICollider parentCollider)
	{
		if (mCollider != null)
		{
			mCollider.SetParentContainerCollider(parentCollider);
		}
	}

	public void Layout()
	{
	}

	protected void HandleRelease(FUICollider collider, FTouch touch)
	{
		if (!string.IsNullOrEmpty(Details))
		{
			FApp.SoundManager.PlaySoundSet("UIButton");
			if (mOnRelease != null)
			{
				mOnRelease(Details);
			}
		}
	}

	public virtual void UpdateStatus(bool collected, string icon, string labelText, bool selected)
	{
		Collected = collected;
		Selected = selected;
		if (!collected)
		{
			mIcon.Unparent();
			mLabel.Unparent();
			return;
		}
		mIcon.SetParent(this);
		mIcon.SetSprite(icon + (Selected ? ".Selected.png" : ".png"));
		mLabel.SetParent(this);
		mLabel.Text = labelText;
	}
}
