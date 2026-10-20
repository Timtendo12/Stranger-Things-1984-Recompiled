using UnityEngine;

public class FUIHorizontalScrollbar : FNode
{
	private static readonly Color cDefaultGutterColor;

	private const string cDefaultGutterSprite = "ScrollBarGutter.png";

	private const string cDefaultThumbSprite = "ScrollBarPip.png";

	private const string cDefaultButtonLeftSprite = "ScrollArrowLeft.png";

	private const string cDefaultButtonRightSprite = "ScrollArrowRight.png";

	private const float cDefaultButtonScrollAmount = 40f;

	private const float cScrollBarFadeInDuration = 0.25f;

	private const float cScrollBarFadeOutDuration = 1f;

	private static readonly Vector2 cDefaultThumbMargin;

	public static readonly Vector2 cDefaultButtonSizeDesktop;

	public static readonly Vector2 cDefaultButtonSizeMobile;

	protected FUIScrollList mList;

	protected Vector2 mSize;

	protected Vector2 mButtonSize;

	protected Vector2 mGutterSize;

	protected Vector2 mThumbSize;

	protected float mButtonScrollAmount;

	protected FUIButton mButtonLeft;

	protected FUIButton mButtonRight;

	protected FUIButton mThumb;

	protected FSlicedSprite mGutter;

	protected Color mButtonDisabledColor;

	protected bool mUpdateButtons;

	protected Vector2 mLastScrollPosition;

	protected Vector2 mLastScrollNodeSize;

	protected bool mDragging;

	protected Vector2 mDragOffset;

	protected readonly bool mMobileScrollbarStyle;

	protected FUICollider mGutterCollider;

	public bool UseMobileScrollbarStyle => false;

	public FUIScrollList List => null;

	public Color ButtonDisabledColor
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public bool ShouldBeVisible { get; set; }

	public FUIHorizontalScrollbar(string name, FUIScrollList list, Vector2 size)
	{
	}

	public static Vector2 GetButtonSize()
	{
		return default;
	}

	public void UpdateSize(Vector2 size)
	{
	}

	protected void CalculateGutterSize()
	{
	}

	public override void HandleAddedToStage()
	{
	}

	public override void HandleRemovedFromStage()
	{
	}

	protected override void HandleUpdate()
	{
	}

	protected void HandleButtonLeft(FUIButton button, FTouch touch)
	{
	}

	protected void HandleButtonRight(FUIButton button, FTouch touch)
	{
	}

	protected void HandleThumbTap(FUIButton button, FTouch touch)
	{
	}

	public void PageRight()
	{
	}

	public void PageLeft()
	{
	}

	protected void HandleGutterPress(FUICollider collider, FTouch touch)
	{
	}

	protected void HandleThumbMove(FUIButton button, FTouch touch)
	{
	}

	protected void HandleThumbRelease(FUIButton button, FTouch touch)
	{
	}

	protected void HandleThumbCancel(FUIButton button, FTouch touch)
	{
	}
}
