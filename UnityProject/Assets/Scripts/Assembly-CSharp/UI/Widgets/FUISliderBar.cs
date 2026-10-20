using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUISliderBar @ 0x17dc5bc..0x17dd074, Ghidra vaddr).
public class FUISliderBar : FDimNode
{
	protected FSlicedSprite mSliderBarSprite;

	protected FUIButton mSliderButton;

	protected Color mSliderBarColor = Color.white;

	protected Color mSliderButtonColor = Color.white;

	protected float mValue;

	protected bool mDragging;

	protected Vector2 mButtonDragOffset = Vector2.zero;

	[CompilerGenerated]
	private Action<FUISliderBar, float> m_mHandleSliderValueChanged;

	public bool ContinuousUpdates = true;

	public float MinValue;

	public float MaxValue = 1f;

	public float QuantizeStep;

	public float NormalizedValue
	{
		get
		{
			return Mathf.Clamp01((mValue - MinValue) / (MaxValue - MinValue));
		}
		set
		{
			float n = Mathf.Clamp01(value);
			mValue = MinValue + (MaxValue - MinValue) * Mathf.Clamp01(n);
			if (QuantizeStep > FMath.Epsilon)
			{
				mValue = QuantizeStep * Mathf.Round(mValue / QuantizeStep);
			}
			UpdateSliderButtonPos();
		}
	}

	public Color SliderBarColor
	{
		get
		{
			return mSliderBarColor;
		}
		set
		{
			mSliderBarColor = value;
			if (mSliderBarSprite != null)
			{
				mSliderBarSprite.Color = value;
			}
		}
	}

	public Color SliderButtonColor
	{
		get
		{
			return mSliderButtonColor;
		}
		set
		{
			mSliderButtonColor = value;
			if (mSliderButton != null)
			{
				mSliderButton.Color = value;
			}
		}
	}

	protected event Action<FUISliderBar, float> mHandleSliderValueChanged
	{
		add
		{
			m_mHandleSliderValueChanged = (Action<FUISliderBar, float>)Delegate.Combine(m_mHandleSliderValueChanged, value);
		}
		remove
		{
			m_mHandleSliderValueChanged = (Action<FUISliderBar, float>)Delegate.Remove(m_mHandleSliderValueChanged, value);
		}
	}

	public FUISliderBar(string name)
	{
		if (!string.IsNullOrEmpty(name))
		{
			Name = "SliderBar" + name;
		}
	}

	public float GetSliderValue()
	{
		return mValue;
	}

	public void SetSliderValue(float value)
	{
		mValue = Mathf.Clamp(value, MinValue, MaxValue);
		UpdateSliderButtonPos();
	}

	public void SetSliderBar(string barSpriteName)
	{
		if (string.IsNullOrEmpty(barSpriteName))
		{
			return;
		}
		if (mSliderBarSprite == null)
		{
			mSliderBarSprite = new FSlicedSprite(barSpriteName, -1f);
			mSliderBarSprite.SetParent(this);
			mSliderBarSprite.ExpandToFill(this, 0f, 0f);
		}
		else
		{
			mSliderBarSprite.SetSprite(barSpriteName);
		}
		mSliderBarSprite.Color = mSliderBarColor;
	}

	public void SetSliderButton(string buttonBackground, float width, float height)
	{
		if (string.IsNullOrEmpty(buttonBackground))
		{
			return;
		}
		if (mSliderButton == null)
		{
			mSliderButton = new FUIButton(Name + "Button");
			mSliderButton.AddHandleTap(HandleButtonTap);
			mSliderButton.AddHandleMove(HandleButtonMove);
			mSliderButton.AddHandleRelease(HandleButtonRelease);
			mSliderButton.AddHandleCancel(HandleButtonCancel);
			mSliderButton.SetParent(this);
			mSliderButton.DockInside(this, InnerDock.Center, width, height, 0f, 0f);
		}
		mSliderButton.SetBackground(buttonBackground);
		mSliderButton.Color = mSliderButtonColor;
	}

	public override void SelfResizeExtents()
	{
		base.SelfResizeExtents();
		UpdateSliderButtonPos();
	}

	protected void UpdateSliderButtonPos()
	{
		if (mSliderBarSprite != null && mSliderButton != null)
		{
			float barWidth = Width;
			float buttonWidth = mSliderButton.Width;
			float norm = Mathf.Clamp01(Mathf.Clamp01((mValue - MinValue) / (MaxValue - MinValue)));
			float half = (barWidth - buttonWidth) * 0.5f;
			mSliderButton.Position = new Vector2((half + half) * norm - half, 0f);
		}
	}

	protected void HandleButtonTap(FUIButton button, FTouch touch)
	{
		if (mSliderButton != null)
		{
			mButtonDragOffset = mSliderButton.GlobalToLocal(touch.Position);
			mDragging = true;
		}
	}

	protected void HandleButtonMove(FUIButton button, FTouch touch)
	{
		if (mSliderBarSprite == null || mSliderButton == null || !mDragging)
		{
			return;
		}
		float x = GlobalToLocal(touch.Position).x - mButtonDragOffset.x;
		float half = (mSliderBarSprite.Width - mSliderButton.Width) * 0.5f;
		x = Mathf.Clamp(x, 0f - half, half);
		mSliderButton.Position = new Vector2(x, 0f);
		if (!(Mathf.Abs(half + half) <= FMath.Epsilon))
		{
			NormalizedValue = (half + x) / (half + half);
			if (ContinuousUpdates && m_mHandleSliderValueChanged != null)
			{
				m_mHandleSliderValueChanged(this, mValue);
			}
		}
	}

	protected void HandleButtonRelease(FUIButton button, FTouch touch)
	{
		mDragging = false;
		if (!ContinuousUpdates && m_mHandleSliderValueChanged != null)
		{
			m_mHandleSliderValueChanged(this, mValue);
		}
	}

	protected void HandleButtonCancel(FUIButton button, FTouch touch)
	{
		mDragging = false;
		if (!ContinuousUpdates && m_mHandleSliderValueChanged != null)
		{
			m_mHandleSliderValueChanged(this, mValue);
		}
	}

	public void AddHandleSliderValueChanged(Action<FUISliderBar, float> d)
	{
		mHandleSliderValueChanged -= d;
		mHandleSliderValueChanged += d;
	}

	public void RemoveHandleSliderValueChanged(Action<FUISliderBar, float> d)
	{
		mHandleSliderValueChanged -= d;
	}
}
