using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUIScrollListItemButton @ 0x17dbed8..0x17dc5bc, Ghidra vaddr).
public class FUIScrollListItemButton : FUIScrollListItem
{
	public static Vector2 cNewTextSize = new Vector2(72f, 40f);

	public static Vector2 cQualityIconSize = new Vector2(36f, 36f);

	public static Vector2 cQualityIconOffset = new Vector2(8f, 8f);

	protected int mIndex;

	protected FUIButton mButton;

	protected Vector2 mVisualSize;

	protected Vector2 mMargin;

	protected Action<int, FTouch> mButtonAction;

	protected string mBackground;

	protected string mSelectedBackground;

	protected bool mSelected;

	protected FSprite mSelectionBackground;

	public FUIButton Button => mButton;

	public int Index => mIndex;

	public FUICollider Collider => mButton.Collider;

	public FUIScrollListItemButton(string name)
	{
		mIndex = -1;
		mVisualSize = Vector2.zero;
		mBackground = string.Empty;
		mMargin = Vector2.zero;
		mSelectedBackground = string.Empty;
		Name = name;
	}

	public virtual bool Initialize(int index, Vector2 size, Vector2 margin, string background, string selectedBackground, Action<int, FTouch> buttonAction)
	{
		mIndex = index;
		base.ItemSize = size;
		mMargin = margin;
		mVisualSize = new Vector2(size.x - margin.x * 2f, size.y - margin.y * 2f);
		if (buttonAction != null)
		{
			mButtonAction = (Action<int, FTouch>)Delegate.Remove(mButtonAction, buttonAction);
			mButtonAction = (Action<int, FTouch>)Delegate.Combine(mButtonAction, buttonAction);
		}
		mBackground = background;
		mSelectedBackground = selectedBackground;
		mSelectionBackground = new FSprite("BlackSquare.png");
		mSelectionBackground.SetParent(this);
		mSelectionBackground.DockInside(this, InnerDock.Center, base.ItemSize.x + 12f, base.ItemSize.y + 12f);
		mSelectionBackground.Visible = false;
		mButton = new FUIButton(Name + index.ToString());
		mButton.SetParent(this);
		if (!string.IsNullOrEmpty(mBackground))
		{
			mButton.SetBackground(mBackground);
		}
		mButton.DockInside(this, InnerDock.Center, base.ItemSize.x, base.ItemSize.y);
		mButton.AddHandleRelease(HandleButton);
		mButton.ScalePressed = 1f;
		return true;
	}

	public override void SetParentContainerCollider(FUICollider parentCollider)
	{
		if (mButton != null && mButton.Collider != null)
		{
			mButton.Collider.SetParentContainerCollider(parentCollider);
		}
	}

	public override void Select(bool v, bool force = false)
	{
		if (mButton == null || (mSelected == v && !force))
		{
			return;
		}
		if (v && !string.IsNullOrEmpty(mSelectedBackground))
		{
			mButton.SetBackground(mSelectedBackground);
		}
		else if (!string.IsNullOrEmpty(mBackground))
		{
			mButton.SetBackground(mBackground);
		}
		mSelected = v;
		if (mSelectionBackground != null)
		{
			mSelectionBackground.Visible = v;
		}
		MarkDirtyRecursive();
	}

	protected void HandleButton(FUIButton button, FTouch touch)
	{
		if (mButtonAction != null)
		{
			mButtonAction(mIndex, touch);
		}
	}

	public override bool IsMouseOver()
	{
		if (mButton != null && mButton.Collider.ContainsPoint(Futile.GetMousePosition()))
		{
			return true;
		}
		return false;
	}

	public override bool IsMouseOver(Vector2 mousePos)
	{
		if (mButton != null && mButton.Collider.ContainsPoint(mousePos))
		{
			return true;
		}
		return false;
	}
}
