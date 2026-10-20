using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUIProgressBar @ 0x17d74c4..0x17d81bc, Ghidra vaddr).
public class FUIProgressBar : FNode
{
	public const float cForegroundAnimationLengthDown = 0.2f;

	public const float cForegroundAnimationLengthUp = 0.4f;

	public const float cMidgroundAnimationDelay = 0.1f;

	public const float cMidgroundAnimationLength = 0.1f;

	protected FSlicedSprite mBackground;

	protected FSlicedSprite mMidground;

	protected FSlicedSprite mForeground;

	protected float mPercentage;

	protected List<FUIProgressBarPoint> mMidgroundPoints = new List<FUIProgressBarPoint>();

	protected List<FUIProgressBarPoint> mForegroundPoints = new List<FUIProgressBarPoint>();

	protected Color mDefaultMidgroundColor = Color.white;

	protected Color mDefaultForegroundColor = Color.white;

	protected float mInternalMargin;

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

	public Color DefaultMidgroundColor
	{
		get
		{
			return mDefaultMidgroundColor;
		}
		set
		{
			mDefaultMidgroundColor = value;
		}
	}

	public Color DefaultForegroundColor
	{
		get
		{
			return mDefaultForegroundColor;
		}
		set
		{
			mDefaultForegroundColor = value;
		}
	}

	public FUIProgressBar(string name, string background, string midground, string foreground)
		: base(name)
	{
		FAssert.That(background.Valid(), "FUIProgressBar constructed with invalid background art!");
		FAssert.That(midground.Valid(), "FUIProgressBar constructed with invalid midground art!");
		FAssert.That(foreground.Valid(), "FUIProgressBar constructed with invalid foreground art!");
		mBackground = new FSlicedSprite(background);
		mBackground.SetParent(this);
		mBackground.ExpandToFill(this, 0f, 0f);
		mMidground = new FSlicedSprite(midground);
		mMidground.Unparent();
		mMidground.Color = mDefaultMidgroundColor;
		mForeground = new FSlicedSprite(foreground);
		mForeground.Unparent();
		mForeground.Color = mDefaultForegroundColor;
	}

	public override void SelfResizeExtents()
	{
		base.SelfResizeExtents();
		if (mPercentage <= 0f)
		{
			if (mMidground.HasParent)
			{
				mMidground.Unparent();
			}
			if (mForeground.HasParent)
			{
				mForeground.Unparent();
			}
			return;
		}
		if (mMidground.Parent != this)
		{
			mMidground.SetParent(this);
		}
		if (mForeground.Parent != this)
		{
			mForeground.SetParent(this);
		}
		Rect extents = Extents;
		mMidground.DockInside(this, InnerDock.Left, extents.width * mPercentage + mInternalMargin * -4f, extents.height - (mInternalMargin + mInternalMargin), mInternalMargin, 0f);
		mForeground.DockInside(this, InnerDock.Left, extents.width * mPercentage + mInternalMargin * -4f, extents.height - (mInternalMargin + mInternalMargin), mInternalMargin, 0f);
	}

	public void SetInternalMargin(float v)
	{
		mInternalMargin = v;
	}

	public Color GetMidgroundColor(float percentage)
	{
		for (int i = 0; i < mMidgroundPoints.Count; i++)
		{
			if (percentage <= mMidgroundPoints[i].Percentage)
			{
				return mMidgroundPoints[i].Color;
			}
		}
		return mDefaultMidgroundColor;
	}

	public void AddForegroundPoint(float percentage, Color color)
	{
		for (int i = 0; i < mForegroundPoints.Count; i++)
		{
			if (percentage < mForegroundPoints[i].Percentage)
			{
				mForegroundPoints.Insert(i, new FUIProgressBarPoint(percentage, color));
				return;
			}
		}
		mForegroundPoints.Add(new FUIProgressBarPoint(percentage, color));
	}

	public Color GetForegroundColor(float percentage)
	{
		for (int i = 0; i < mForegroundPoints.Count; i++)
		{
			if (percentage <= mForegroundPoints[i].Percentage)
			{
				return mForegroundPoints[i].Color;
			}
		}
		return mDefaultForegroundColor;
	}

	public void SetBackgroundColor(Color color)
	{
		mBackground.Color = color;
	}

	public void SetForegroundColor(Color color)
	{
		Color current = mForeground.Color;
		float dr = current.r - color.r;
		float dg = current.g - color.g;
		float db = current.b - color.b;
		float da = current.a - color.a;
		if (da * da + db * db + dr * dr + dg * dg < 1E-10f)
		{
			return;
		}
		for (int i = 0; i < mForegroundPoints.Count; i++)
		{
			mForegroundPoints[i].Color = color;
		}
		mForeground.Color = color;
	}

	public void FlashForeground(Color color, float duration)
	{
		// the shipped code ignores 'color' (tweens to white, then to red)
		mForeground.AddAction(new FEntityActionSequence(new FEntityAction[2]
		{
			new FEntityActionTweenPropertyToColor(duration * 0.5f, "Color", Color.white),
			new FEntityActionTweenPropertyToColor(duration * 0.5f, "Color", Color.red)
		}));
	}

	public void SetPercentage(float v, bool force = false, bool skipTween = false)
	{
		if (!force && Mathf.Abs(mPercentage - v) < FMath.Epsilon)
		{
			return;
		}
		mPercentage = Mathf.Clamp01(v);
		Color midgroundColor = GetMidgroundColor(v);
		Color foregroundColor = GetForegroundColor(v);
		mForeground.Color = foregroundColor;
		mMidground.Color = midgroundColor;
		MarkDirtyRecursive();
	}
}
