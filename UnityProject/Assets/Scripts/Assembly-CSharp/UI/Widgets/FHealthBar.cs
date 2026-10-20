using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FHealthBar @ 0x17ed96c..0x17ee390, Ghidra vaddr).
public class FHealthBar : FNode
{
	public const float cForegroundAnimationLengthDown = 0.2f;

	public const float cForegroundAnimationLengthUp = 0.4f;

	public const float cMidgroundAnimationDelay = 0.1f;

	public const float cMidgroundAnimationLength = 0.1f;

	protected FNodeStack mBackground;

	protected FSprite mLeftFrameCap;

	protected FSprite mRightFrameCap;

	protected FSprite mLeftFillCap;

	protected FSprite mRightFillCap;

	protected FSlicedSprite mForeground;

	protected float mPercentage;

	public float Percentage
	{
		get
		{
			return mPercentage;
		}
		set
		{
			SetPercentage(value);
		}
	}

	public FHealthBar(string name)
		: base(name)
	{
		mBackground = new FNodeStack(base.Name + ".BGStack", FNodeStack.Modes.FromLeft, 0f);
		mBackground.SetParent(this);
		mBackground.ExpandToFill(this);
		mLeftFrameCap = new FSprite(base.Name + ".LeftFrameCap", "HealthFrame_Left.png");
		mLeftFrameCap.SetParent(this);
		mRightFrameCap = new FSprite(base.Name + ".RightFrameCap", "HealthFrame_Middle03.png");
		mRightFrameCap.SetParent(this);
		mForeground = new FSlicedSprite("Healthbar_Middle.png");
		mForeground.Unparent();
		mLeftFillCap = new FSprite(base.Name + ".LeftFillCap", "Healthbar_Left.png");
		mLeftFillCap.SetParent(this);
		mRightFillCap = new FSprite(base.Name + ".RightFillCap", "Healthbar_Right.png");
		mRightFillCap.SetParent(this);
	}

	public override void SelfResizeExtents()
	{
		base.SelfResizeExtents();
		if (mPercentage <= 0f)
		{
			if (mForeground.HasParent)
			{
				mForeground.Unparent();
			}
			if (mLeftFillCap.HasParent)
			{
				mLeftFillCap.Unparent();
			}
			if (mRightFillCap.HasParent)
			{
				mRightFillCap.Unparent();
			}
			return;
		}
		if (mForeground.Parent != this)
		{
			mForeground.SetParent(this);
		}
		if (mLeftFillCap.Parent != this)
		{
			mLeftFillCap.SetParent(this);
		}
		if (mRightFillCap.Parent != this)
		{
			mRightFillCap.SetParent(this);
		}
		Rect extents = Extents;
		float width = extents.width;
		float height = extents.height;
		float ratio = width / height;
		int num = int.MaxValue;
		if (!float.IsPositiveInfinity(ratio))
		{
			num = (int)ratio - 1;
		}
		while (mBackground.GetChildCount() < num)
		{
			int count = mBackground.GetChildCount();
			Vector2 offset = ((count == 0) ? new Vector2(36f, 0f) : Vector2.zero);
			int variant = FRandom.Generic.RangeInt(1, 2);
			FSprite child = new FSprite(string.Format("HealthFrame_Middle0{0}.png", variant));
			mBackground.StackChild(child, new Vector2(height, height), offset);
		}
		mLeftFrameCap.DockInside(this, InnerDock.Left, height, height);
		mRightFrameCap.DockInside(this, InnerDock.Right, height, height);
		mForeground.DockInside(this, InnerDock.Left, (width - 32f) * mPercentage, height, 16f, 0f);
		mLeftFillCap.DockInside(this, InnerDock.Left, height, height);
		if (mPercentage >= 1f)
		{
			mRightFillCap.SetSprite("Healthbar_Right.png");
			mRightFillCap.DockInside(this, InnerDock.Right, height, height);
		}
		else
		{
			mRightFillCap.SetSprite("Healthbar_Cap.png");
			mRightFillCap.DockOutside(mForeground, OuterDock.Right, height * 0.25f, height);
		}
	}

	public void SetBackgroundColor(Color color)
	{
		mBackground.Color = color;
	}

	public void FlashForeground(float duration)
	{
		AdditiveTintColor = new Color(0f, 0f, 0f, 0f);
		Go.killAllAbstractTweensWithTarget(this);
		GoTweenConfig config = new GoTweenConfig().colorProp("AdditiveTintColor", new Color(1f, 1f, 1f, 0.8f)).setIterations(2, GoLoopType.PingPong);
		Go.addTween(new GoTween(this, duration * 0.5f, config));
	}

	public void SetPercentage(float v, bool force = false, bool skipTween = false)
	{
		if (!force && Mathf.Abs(mPercentage - v) < Mathf.Epsilon)
		{
			return;
		}
		mPercentage = Mathf.Clamp01(v);
		MarkDirtyRecursive();
	}
}
