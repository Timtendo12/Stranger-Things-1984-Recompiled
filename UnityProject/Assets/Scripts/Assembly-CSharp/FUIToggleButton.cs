using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUIToggleButton @ 0x17dd078..0x17ddbac, Ghidra vaddr).
public class FUIToggleButton : FUIButton
{
	protected FSlicedSprite mForegroundUnchecked;

	protected bool mChecked;

	[CompilerGenerated]
	private Action<FUIToggleButton> m_mHandleCheckedChanged;

	public bool Checked
	{
		get
		{
			return mChecked;
		}
		set
		{
			if (mChecked != value)
			{
				mChecked = value;
				SetToggleVisibility();
				if (m_mHandleCheckedChanged != null)
				{
					m_mHandleCheckedChanged(this);
				}
			}
		}
	}

	protected event Action<FUIToggleButton> mHandleCheckedChanged
	{
		add
		{
			m_mHandleCheckedChanged = (Action<FUIToggleButton>)Delegate.Combine(m_mHandleCheckedChanged, value);
		}
		remove
		{
			m_mHandleCheckedChanged = (Action<FUIToggleButton>)Delegate.Remove(m_mHandleCheckedChanged, value);
		}
	}

	public FUIToggleButton()
		: this(string.Empty)
	{
	}

	public FUIToggleButton(string name)
		: base(name)
	{
		mChecked = true;
	}

	public void SetText(string text, string font, float pointSize, bool labelOnLeft, float offset = 0f)
	{
		SetText(text, font, pointSize, 3f);
		if (mLabel != null)
		{
			float textWidth = mLabel.TextWidth;
			if (!labelOnLeft)
			{
				mLabel.DockCustom(this, HorizSide.Right, offset, this, HorizSide.Right, textWidth + offset, this, VertSide.Top, 0f, this, VertSide.Bottom, 0f);
			}
			else
			{
				mLabel.DockCustom(this, HorizSide.Left, offset - textWidth, this, HorizSide.Left, offset, this, VertSide.Top, 0f, this, VertSide.Bottom, 0f);
			}
		}
	}

	public void SetTextColor(Color col)
	{
		if (mLabel != null)
		{
			mLabel.Color = col;
		}
	}

	public void SetForegroundUnchecked(string foreground)
	{
		if (string.IsNullOrEmpty(foreground))
		{
			return;
		}
		if (mForegroundUnchecked == null)
		{
			mForegroundUnchecked = new FSlicedSprite(foreground, -1f);
			mForegroundUnchecked.SetParent(this);
			mForegroundUnchecked.ExpandToFill(this, 0f, 0f);
		}
		else
		{
			mForegroundUnchecked.SetSprite(foreground);
		}
		mForegroundUnchecked.Color = ForegroundColorNormal;
		SetToggleVisibility();
	}

	public void SetToggleVisibility()
	{
		if (!mChecked)
		{
			if (mForeground != null)
			{
				mForeground.Visible = false;
			}
			if (mForegroundUnchecked != null)
			{
				mForegroundUnchecked.Visible = true;
			}
		}
		else
		{
			if (mForeground != null)
			{
				mForeground.Visible = true;
			}
			if (mForegroundUnchecked != null)
			{
				mForegroundUnchecked.Visible = false;
			}
		}
	}

	public override void HandleTouchBegin(FUICollider button, FTouch touch)
	{
		if (!mEnabled)
		{
			CheckHandleDisabled(touch);
			return;
		}
		GoTweenConfig bgConfig = new GoTweenConfig().colorProp("Color", BackgroundColorPressed);
		if (mBackground != null)
		{
			Go.to(mBackground, TweenTime, bgConfig);
			Go.to(mBackground, TweenTime, new GoTweenConfig().floatProp("Scale", ScalePressed));
		}
		GoTweenConfig fgConfig = new GoTweenConfig().colorProp("Color", ForegroundColorPressed);
		if (mForeground != null)
		{
			Go.to(mForeground, TweenTime, fgConfig);
			Go.to(mForeground, TweenTime, new GoTweenConfig().floatProp("Scale", ScalePressed));
		}
		if (mForegroundUnchecked != null)
		{
			Go.to(mForegroundUnchecked, TweenTime, fgConfig);
			Go.to(mForegroundUnchecked, TweenTime, new GoTweenConfig().floatProp("Scale", ScalePressed));
		}
		CheckHandleTap(touch);
	}

	public override void HandleTouchRelease(FUICollider button, FTouch touch)
	{
		if (mEnabled)
		{
			Checked = !mChecked;
			SetToggleVisibility();
			TweenToNormal();
			CheckHandleRelease(touch);
		}
	}

	public override void HandleTouchCancel(FUICollider button, FTouch touch)
	{
		if (mEnabled)
		{
			TweenToNormal();
			CheckHandleCancel(touch);
		}
	}

	protected override void TweenToNormal()
	{
		GoTweenConfig bgConfig = new GoTweenConfig().colorProp("Color", BackgroundColorNormal);
		if (mBackground != null)
		{
			Go.killAllTweensWithTarget(mBackground);
			Go.to(mBackground, TweenTime, bgConfig);
			Go.to(mBackground, TweenTime, new GoTweenConfig().floatProp("Scale", ScaleNormal));
		}
		GoTweenConfig fgConfig = new GoTweenConfig().colorProp("Color", ForegroundColorNormal);
		if (mForeground != null)
		{
			Go.killAllTweensWithTarget(mForeground);
			Go.to(mForeground, TweenTime, fgConfig);
			Go.to(mForeground, TweenTime, new GoTweenConfig().floatProp("Scale", ScaleNormal));
		}
		if (mForegroundUnchecked != null)
		{
			Go.killAllTweensWithTarget(mForegroundUnchecked);
			Go.to(mForegroundUnchecked, TweenTime, fgConfig);
			Go.to(mForegroundUnchecked, TweenTime, new GoTweenConfig().floatProp("Scale", ScaleNormal));
		}
	}
}
