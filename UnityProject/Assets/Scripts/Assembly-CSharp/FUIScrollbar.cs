using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUIScrollbar @ 0x17d8204..0x17d96bc, Ghidra vaddr).
public class FUIScrollbar : FNode
{
	private static readonly Color cDefaultGutterColor = new Color(1f, 1f, 1f, 0.8f);

	private const string cDefaultGutterSprite = "ScrollBarGutter.png";

	private const string cDefaultThumbSprite = "ScrollBarPip.png";

	private const string cDefaultButtonUpSprite = "ScrollArrowUp.png";

	private const string cDefaultButtonDownSprite = "ScrollArrowDown.png";

	private const float cDefaultButtonScrollAmount = 40f;

	private const float cScrollBarFadeInDuration = 0.25f;

	private const float cScrollBarFadeOutDuration = 1f;

	private static readonly Vector2 cDefaultThumbMargin = Vector2.zero;

	public static readonly Vector2 cDefaultButtonSizeDesktop = new Vector2(32f, 32f);

	public static readonly Vector2 cDefaultButtonSizeMobile = new Vector2(16f, 32f);

	protected FUIScrollList mList;

	protected Vector2 mSize = Vector2.zero;

	protected Vector2 mButtonSize = Vector2.zero;

	protected Vector2 mGutterSize = Vector2.zero;

	protected Vector2 mThumbSize = Vector2.zero;

	protected float mButtonScrollAmount = 40f;

	protected FUIButton mButtonUp;

	protected FUIButton mButtonDown;

	protected FUIButton mThumb;

	protected FSlicedSprite mGutter;

	protected Color mButtonDisabledColor = new Color(0.3f, 0.3f, 0.3f, 1f);

	protected bool mUpdateButtons;

	protected Vector2 mLastScrollPosition = Vector2.zero;

	protected Vector2 mLastScrollNodeSize = Vector2.zero;

	protected bool mDragging;

	protected Vector2 mDragOffset = Vector2.zero;

	protected readonly bool mMobileScrollbarStyle;

	protected FUICollider mGutterCollider;

	public bool UseMobileScrollbarStyle => mMobileScrollbarStyle;

	public FUIScrollList List => mList;

	public Color ButtonDisabledColor
	{
		get
		{
			return mButtonDisabledColor;
		}
		set
		{
			mButtonDisabledColor = value;
		}
	}

	public bool ShouldBeVisible { get; set; }

	public FUIScrollbar(string name, FUIScrollList list, Vector2 size)
		: base(name)
	{
		FAssert.That(list != null, "list == null");
		mList = list;
		mSize = size;
		mMobileScrollbarStyle = true;
		mButtonSize = cDefaultButtonSizeMobile;
		CalculateGutterSize();
		mButtonScrollAmount = 40f;
		mThumbSize = new Vector2(mGutterSize.x, mGutterSize.x);
		mGutter = new FSlicedSprite("ScrollBarGutter.png");
		mGutter.SetParent(this);
		mGutter.Color = cDefaultGutterColor;
		mGutter.DockInside(this, InnerDock.Center, mGutterSize.x, mGutterSize.y);
		if (!mMobileScrollbarStyle)
		{
			mGutterCollider = new FUICollider("GutterCollider");
			mGutterCollider.SetParent(mGutter);
			mGutterCollider.AddSignalPress(HandleGutterPress);
			mGutterCollider.ExpandToFill(mGutter);
			if (!mMobileScrollbarStyle)
			{
				mButtonUp = new FUIButton(Name + "ButtonUp");
				mButtonUp.SetParent(this);
				mButtonUp.SetBackground("ScrollArrowUp.png");
				mButtonUp.AddHandleTap(HandleButtonUp);
				mButtonUp.DockInside(this, InnerDock.Top, mButtonSize.x, mButtonSize.y);
				mButtonDown = new FUIButton(Name + "ButtonDown");
				mButtonDown.SetParent(this);
				mButtonDown.SetBackground("ScrollArrowDown.png");
				mButtonDown.AddHandleTap(HandleButtonDown);
				mButtonDown.DockInside(this, InnerDock.Bottom, mButtonSize.x, mButtonSize.y);
			}
		}
		mThumb = new FUIButton(Name + "Thumb");
		mThumb.SetParent(mGutter);
		mThumb.SetBackground("ScrollBarPip.png");
		mThumb.AddHandleTap(HandleThumbTap);
		mThumb.AddHandleMove(HandleThumbMove);
		mThumb.AddHandleRelease(HandleThumbRelease);
		mThumb.AddHandleCancel(HandleThumbCancel);
		mThumb.DockInside(mGutter, InnerDock.Center, mThumbSize.x, mThumbSize.y);
		if (mMobileScrollbarStyle)
		{
			mThumb.Collider.Enabled = false;
		}
	}

	public static Vector2 GetButtonSize()
	{
		return cDefaultButtonSizeMobile;
	}

	public void UpdateSize(Vector2 size)
	{
		mSize = size;
		CalculateGutterSize();
		if (mGutter != null)
		{
			mGutter.DockInside(this, InnerDock.Center, mGutterSize.x, mGutterSize.y);
		}
	}

	protected void CalculateGutterSize()
	{
		float num;
		if (!mMobileScrollbarStyle)
		{
			ShouldBeVisible = true;
			num = mSize.y - mButtonSize.y * 2f;
		}
		else
		{
			ShouldBeVisible = false;
			num = mSize.y;
		}
		mGutterSize = new Vector2(mSize.x - cDefaultThumbMargin.x * 2f, num - cDefaultThumbMargin.y * 2f);
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		Futile.Instance.RemoveSignalUpdate(HandleUpdate);
		Futile.Instance.AddSignalUpdate(HandleUpdate);
		mUpdateButtons = true;
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		Futile.Instance.RemoveSignalUpdate(HandleUpdate);
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
		if (mList == null)
		{
			return;
		}
		if (mMobileScrollbarStyle)
		{
			float alpha = base.Alpha;
			if (!ShouldBeVisible)
			{
				if (alpha > 0f)
				{
					base.Alpha = Mathf.Max(alpha - Time.deltaTime / 1f, 0f);
				}
			}
			else if (alpha < 1f)
			{
				base.Alpha = Mathf.Min(alpha + Time.deltaTime / 0.25f, 1f);
			}
		}
		if (!mUpdateButtons && Mathf.Abs(mLastScrollPosition.y - mList.ScrollPosition.y) < Mathf.Epsilon && Mathf.Abs(mLastScrollNodeSize.y - mList.ScrollNodeHeight) < Mathf.Epsilon)
		{
			return;
		}
		mLastScrollPosition = mList.ScrollPosition;
		mLastScrollNodeSize.y = mList.ScrollNodeHeight;
		mUpdateButtons = false;
		if (mButtonUp != null)
		{
			mButtonUp.Color = (mList.IsAtTop() ? mButtonDisabledColor : Color.white);
		}
		if (mButtonDown != null)
		{
			mButtonDown.Color = (mList.IsAtBottom() ? mButtonDisabledColor : Color.white);
		}
		if (mThumb == null)
		{
			MarkDirtyRecursive();
			return;
		}
		float scrollNodeHeight = mList.ScrollNodeHeight;
		if (scrollNodeHeight <= 0f || mDragging)
		{
			MarkDirtyRecursive();
			return;
		}
		float height = mList.Height;
		float num = height / mList.ScrollNodeHeight;
		if (num >= 1f)
		{
			mThumbSize.y = mGutterSize.y;
			mThumb.DockInside(mGutter, InnerDock.Center, mThumbSize.x, mGutterSize.y);
			base.Visible = false;
		}
		else
		{
			base.Visible = true;
			mThumbSize.y = num * mGutterSize.y;
			float scrollNodeHeight2 = mList.ScrollNodeHeight;
			float height2 = mList.Height;
			float y = mList.ScrollPosition.y;
			float minScrollY = mList.GetMinScrollY();
			float maxScrollY = mList.GetMaxScrollY();
			float scrollNodeHeight3 = mList.ScrollNodeHeight;
			float num2 = Mathf.Clamp(y, minScrollY, maxScrollY);
			// 0x17d8f68..0x17d9080: DockInside(thumb size x, y, xOffset 0, yOffset = -(clamp(scrollY, min, max) + (nodeH - listH) * 0.5) / nodeH * gutterH), verified in the disassembly
			mThumb.DockInside(mGutter, InnerDock.Top, mThumbSize.x, mThumbSize.y, 0f, (0f - (num2 + (scrollNodeHeight2 - height2) * 0.5f) / scrollNodeHeight3) * mGutterSize.y);
		}
		MarkDirtyRecursive();
	}

	protected void HandleButtonUp(FUIButton button, FTouch touch)
	{
		if (mList != null && !mList.IsAtTop())
		{
			FApp.SoundManager.PlaySoundSet("UIButton");
			mList.ScrollUp(mButtonScrollAmount);
			mUpdateButtons = true;
		}
	}

	protected void HandleButtonDown(FUIButton button, FTouch touch)
	{
		if (mList != null && !mList.IsAtBottom())
		{
			FApp.SoundManager.PlaySoundSet("UIButton");
			mList.ScrollDown(mButtonScrollAmount);
			mUpdateButtons = true;
		}
	}

	protected void HandleThumbTap(FUIButton button, FTouch touch)
	{
		if (mList != null && mThumb != null)
		{
			mDragOffset = mThumb.GlobalToLocal(touch.Position);
			mDragging = true;
		}
	}

	public void PageUp()
	{
		if (mList != null && !mList.IsAtTop())
		{
			mList.ScrollUp(mList.Height);
		}
	}

	public void PageDown()
	{
		if (mList != null && !mList.IsAtBottom())
		{
			mList.ScrollDown(mList.Height);
		}
	}

	protected void HandleGutterPress(FUICollider collider, FTouch touch)
	{
		if (mThumb != null)
		{
			float y = GlobalToLocal(touch.Position).y;
			Rect extents = mThumb.Extents;
			if (extents.y + extents.height < y)
			{
				PageUp();
			}
			else if (extents.y > y)
			{
				PageDown();
			}
		}
	}

	protected void HandleThumbMove(FUIButton button, FTouch touch)
	{
		if (mList == null || mThumb == null || !mDragging || mGutterSize.y < Mathf.Epsilon)
		{
			return;
		}
		if (mList.GetMinScrollY() != 0f)
		{
			float y = GlobalToLocal(touch.Position).y;
			float num = mGutterSize.y * 0.5f;
			float num2 = mThumbSize.y * 0.5f;
			float num3 = y - mDragOffset.y;
			float num4 = num - num2;
			float num5 = num2 - num;
			float num6 = Mathf.Clamp(num3, num5, num4);
			mThumb.Position = new Vector2(0f, num6);
			float maxScrollY = mList.GetMaxScrollY();
			float minScrollY = mList.GetMinScrollY();
			float num7 = (1f - (num6 - num5) / (num4 - num5)) * (maxScrollY - minScrollY) + mList.GetMinScrollY();
			mList.SetScrollPosition(0f, num7);
			float scrollNodeHeight = mList.ScrollNodeHeight;
			float height = mList.Height;
			float scrollNodeHeight2 = mList.ScrollNodeHeight;
			mThumb.DockInside(mGutter, InnerDock.Top, mThumbSize.x, mThumbSize.y, 0f, (0f - (num7 + (scrollNodeHeight - height) * 0.5f) / scrollNodeHeight2) * mGutterSize.y);
		}
		else
		{
			mDragging = false;
		}
	}

	protected void HandleThumbRelease(FUIButton button, FTouch touch)
	{
		mDragging = false;
	}

	protected void HandleThumbCancel(FUIButton button, FTouch touch)
	{
		mDragging = false;
	}
}
