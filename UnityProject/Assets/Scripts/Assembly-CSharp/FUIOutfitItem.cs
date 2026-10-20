using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUIOutfitItem @ 0x17f9854..0x17f9eac, Ghidra vaddr).
public class FUIOutfitItem : FUIScrollListItem
{
	protected FUICollider mCollider;

	protected FSprite mIcon;

	protected FLabel mLabel;

	protected Action<int> mOnRelease;

	public int Index { get; protected set; }

	public bool Collected { get; protected set; }

	public bool Selected { get; protected set; }

	public FUIOutfitItem(int index, bool collected, bool selected, Action<int> onRelease)
	{
		Index = index;
		Name = "Outfit." + index.ToString();
		Collected = collected;
		Selected = selected;
		mOnRelease = onRelease;
		mCollider = new FUICollider(Name + ".Collider");
		mCollider.SetParent(this);
		mCollider.AddSignalRelease(HandleRelease, 40f);
		mCollider.ExpandToFill(this, 0f, 0f);
		mIcon = new FSprite(Name + ".StateIcon", "HopperUpgrade000_Icon.png");
		mIcon.ExpandToFill(this, 0f, 0f);
		mIcon.SetParent(this);
		mLabel = new FLabel(Name + ".Title", "DefaultStyle", (index + 1).ToString(), 24f, true, true, false);
		mLabel.ShrinkToFitContainer(true);
		mLabel.Color = FColorNames.Get("DarkGray");
		mLabel.ExpandToFill(this, 0f, 0f);
		mLabel.SetParent(this);
		MarkDirtyRecursive();
		UpdateStatus(Collected, Selected, "");
	}

	public void Layout()
	{
	}

	public void UpdateStatus(bool collected, bool selected, string characterName)
	{
		Collected = collected;
		Selected = selected;
		if (!collected)
		{
			mIcon.Unparent();
			mLabel.SetParent(this);
		}
		else
		{
			mIcon.SetParent(this);
			mIcon.SetSprite(Selected ? OutfitHandler.GetSelectedSpriteforCharacter(characterName) : OutfitHandler.GetUnselectedSpriteforCharacter(characterName));
			mLabel.Unparent();
		}
	}

	public override void SetParentContainerCollider(FUICollider parentCollider)
	{
		if (mCollider != null)
		{
			mCollider.SetParentContainerCollider(parentCollider);
		}
	}

	protected void HandleRelease(FUICollider collider, FTouch touch)
	{
		if (Collected)
		{
			FApp.SoundManager.PlaySoundSet("UIButton");
			if (mOnRelease != null)
			{
				mOnRelease(Index);
			}
		}
	}
}
