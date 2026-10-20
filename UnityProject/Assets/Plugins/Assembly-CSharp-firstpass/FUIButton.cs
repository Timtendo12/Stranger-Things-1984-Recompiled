using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUIButton @ 0x16a2ac0..0x16a7d40, Ghidra vaddr).
public class FUIButton : FDimNode
{
	public delegate FNode GetTopHudPageDelegate();

	public const float cDefaultReleaseActivationDistance = 40f;

	public const float cTooltipPressDuration = 0.5f;

	public const float cTooltipHoverDuration = 0.5f;

	public static readonly Color HintTint = new Color(0.5f, 0.7f, 0.5f, 0f);

	public static readonly Color NewlyActiveTint = new Color(0.7f, 0.9f, 0.7f, 0f);

	public static GetTopHudPageDelegate TopHudPage;

	[CompilerGenerated]
	private Action<FUIButton, FTouch> m_mHandleTap;

	[CompilerGenerated]
	private Action<FUIButton, FTouch> m_mHandleMove;

	[CompilerGenerated]
	private Action<FUIButton, FTouch> m_mHandleDisabled;

	[CompilerGenerated]
	private Action<FUIButton, FTouch> m_mHandleRelease;

	[CompilerGenerated]
	private Action<FUIButton, FTouch> m_mHandleCancel;

	[CompilerGenerated]
	private Action<FNode, string> m_mHandleEnter;

	[CompilerGenerated]
	private Action<FNode, string> m_mHandleEscape;

	[CompilerGenerated]
	private Action<FNode, string, bool> m_mHandleTab;

	[CompilerGenerated]
	private Action<FNode, string> m_mHandleBackquote;

	protected FSlicedSprite mBackground;

	protected FSlicedSprite mForeground;

	protected FSlicedSprite mFocusSprite;

	protected FLabel mLabel;

	protected FUICollider mCollider;

	protected bool mEnabled;

	protected FNode mHighlightFX;

	protected FSprite mSpinner;

	protected AbstractGoTween mFlashTween;

	protected FUITooltip mTooltip;

	protected List<FUITooltipLine> mTooltipTextLines;

	protected OuterDock mTooltipDock = OuterDock.Top;

	protected float mTooltipWidth = 400f;

	protected bool mPressed;

	protected float mPressDuration;

	protected Vector2 mColliderScale = Vector2.one;

	public Color BackgroundColorNormal { get; set; } = Color.white;

	public Color BackgroundColorPressed { get; set; } = Color.white;

	public Color ForegroundColorNormal { get; set; } = Color.white;

	public Color ForegroundColorPressed { get; set; } = Color.white;

	public float ScaleNormal { get; set; } = 1f;

	public float ScalePressed { get; set; } = 0.95f;

	public float TweenTime { get; set; } = 0.05f;

	public int Data { get; set; } = -1;

	public FLabel Label => mLabel;

	public string Text
	{
		get
		{
			if (mLabel != null)
			{
				return mLabel.Text;
			}
			return string.Empty;
		}
	}

	public bool Enabled => mEnabled;

	public Vector2 ColliderScale
	{
		get
		{
			return mColliderScale;
		}
		set
		{
			mColliderScale = value;
			MarkDirtyRecursive();
		}
	}

	public FUICollider Collider => mCollider;

	public OuterDock TooltipDock
	{
		get
		{
			return mTooltipDock;
		}
		set
		{
			mTooltipDock = value;
		}
	}

	public string FocusBackgroundSprite { get; set; }

	public string NonFocusBackgroundSprite { get; set; }

	protected event Action<FUIButton, FTouch> mHandleTap;

	protected event Action<FUIButton, FTouch> mHandleMove;

	protected event Action<FUIButton, FTouch> mHandleDisabled;

	protected event Action<FUIButton, FTouch> mHandleRelease;

	protected event Action<FUIButton, FTouch> mHandleCancel;

	protected event Action<FNode, string> mHandleEnter;

	protected event Action<FNode, string> mHandleEscape;

	protected event Action<FNode, string, bool> mHandleTab;

	protected event Action<FNode, string> mHandleBackquote;

	public FUIButton(string name)
	{
		if (!string.IsNullOrEmpty(name))
		{
			Name = "Button" + name;
		}
		mCollider = new FUICollider(Name, false);
		mCollider.AddSignalPress(HandleTouchBegin);
		mCollider.AddSignalMove(HandleTouchMove);
		mCollider.AddSignalRelease(HandleTouchRelease);
		mCollider.ReleaseActivationDistance = 40f;
		mCollider.AddSignalCancel(HandleTouchCancel);
		mCollider.ReleaseActivationDistance = 40f;
		mCollider.SetParent(this);
		mCollider.ExpandToFill(this, 0f, 0f);
		mTooltip = new FUITooltip(Name + "Tooltip");
		mTooltipTextLines = new List<FUITooltipLine>();
		mEnabled = true;
	}

	public void SetBackground(string background)
	{
		if (string.IsNullOrEmpty(background))
		{
			if (mBackground != null && mBackground.Parent != null)
			{
				mBackground.Parent.RemoveChild(mBackground);
			}
			return;
		}
		if (mBackground == null)
		{
			mBackground = new FSlicedSprite(background, -1f);
			mBackground.ExpandToFill(this, 0f, 0f);
		}
		else
		{
			mBackground.SetSprite(background);
		}
		mBackground.SetParent(this);
		if (mBackground.Parent != null)
		{
			mBackground.Parent.AddChildAtIndex(mBackground, 0);
		}
		mBackground.Color = BackgroundColorNormal;
	}

	public void SetBackgroundColor(Color backgroundColor)
	{
		if (mBackground != null)
		{
			Go.killAllTweensWithTarget(mBackground);
			mBackground.Color = backgroundColor;
		}
		BackgroundColorNormal = backgroundColor;
		BackgroundColorPressed = backgroundColor;
	}

	public void SetForeground(string foreground)
	{
		if (string.IsNullOrEmpty(foreground))
		{
			return;
		}
		if (mForeground == null)
		{
			mForeground = new FSlicedSprite(foreground, -1f);
			mForeground.SetParent(this);
			mForeground.ExpandToFill(this, 0f, 0f);
		}
		else
		{
			mForeground.SetSprite(foreground);
		}
		mForeground.Color = ForegroundColorNormal;
	}

	public void SetForegroundColor(Color foregroundColor)
	{
		if (mForeground != null)
		{
			Go.killAllTweensWithTarget(mForeground);
			mForeground.Color = foregroundColor;
		}
		if (mLabel != null)
		{
			Go.killAllTweensWithTarget(mLabel);
			mLabel.Color = foregroundColor;
		}
		ForegroundColorNormal = foregroundColor;
		ForegroundColorPressed = foregroundColor;
	}

	public void SetFocusSprite(string spriteName, Vector2 size)
	{
		if (string.IsNullOrEmpty(spriteName))
		{
			return;
		}
		if (mFocusSprite == null)
		{
			mFocusSprite = new FSlicedSprite(spriteName, -1f);
			mFocusSprite.SetParent(this);
		}
		else
		{
			mFocusSprite.SetSprite(spriteName);
		}
		mFocusSprite.DockInside(this, InnerDock.Center, size, Vector2.zero);
	}

	public void SetFocusSpriteExtent(Vector2 size, Vector2 offset)
	{
		if (mFocusSprite != null)
		{
			mFocusSprite.DockInside(this, InnerDock.Center, size, offset);
		}
	}

	public void SetColliderScale(Vector2 scale)
	{
		mColliderScale = scale;
		MarkDirtyRecursive();
	}

	public override void SelfResizeExtents()
	{
		base.SelfResizeExtents();
		Rect extents = Extents;
		mCollider.DockInside(this, InnerDock.Center, extents.width * mColliderScale.x, extents.height * mColliderScale.y, 0f, 0f);
	}

	public void JustifyText(Sides jx, Sides jy = Sides.CenterY)
	{
		if (mLabel != null)
		{
			mLabel.Justify(jx, jy);
		}
	}

	public void SetText(string text, string font, float pointSize, float border = 14f)
	{
		if (mLabel == null)
		{
			mLabel = new FLabel(Name + "ButtonLabel", font, text, pointSize, true, true, false);
			mLabel.SetParent(this);
			mLabel.ExpandToFill(this, 0f - border);
		}
		mLabel.Color = ForegroundColorNormal;
		mLabel.Text = text;
	}

	public void SetTextExtents(Vector2 size, Vector2 offset)
	{
		if (mLabel != null)
		{
			mLabel.DockInside(this, InnerDock.Center, size, offset);
		}
	}

	public void SetTextWrap(bool v)
	{
		if (mLabel != null)
		{
			mLabel.Wrap = v;
		}
	}

	public void SetText(string text)
	{
		if (mLabel != null)
		{
			mLabel.Text = text;
		}
	}

	public void SetTextShrinkToFitContainer(bool v)
	{
		if (mLabel != null)
		{
			mLabel.ShrinkToFitContainer(v);
		}
	}

	public void CancelPressForTooltip()
	{
		mPressed = false;
	}

	public void CheckHandleTap(FTouch touch)
	{
		mPressed = true;
		if (this.mHandleTap != null)
		{
			this.mHandleTap(this, touch);
		}
	}

	public void CheckHandleDisabled(FTouch touch)
	{
		if (this.mHandleDisabled != null)
		{
			this.mHandleDisabled(this, touch);
		}
	}

	public void CheckHandleRelease(FTouch touch)
	{
		mPressed = false;
		if (this.mHandleRelease != null)
		{
			this.mHandleRelease(this, touch);
		}
	}

	public void CheckHandleCancel(FTouch touch)
	{
		mPressed = false;
		if (this.mHandleCancel != null)
		{
			this.mHandleCancel(this, touch);
		}
	}

	public virtual void HandleTouchBegin(FUICollider button, FTouch touch)
	{
		if (!mEnabled)
		{
			if (this.mHandleDisabled != null)
			{
				this.mHandleDisabled(this, touch);
			}
			return;
		}
		GoTweenConfig bgConfig = new GoTweenConfig().colorProp("Color", BackgroundColorPressed);
		if (mBackground != null)
		{
			Go.killAllTweensWithTarget(mBackground);
			Go.to(mBackground, TweenTime, bgConfig);
		}
		GoTweenConfig fgConfig = new GoTweenConfig().colorProp("Color", ForegroundColorPressed);
		if (mForeground != null)
		{
			Go.killAllTweensWithTarget(mForeground);
			Go.to(mForeground, TweenTime, fgConfig);
		}
		if (mLabel != null)
		{
			Go.killAllTweensWithTarget(mLabel);
			Go.to(mLabel, TweenTime, fgConfig);
		}
		Go.killAllTweensWithTarget(this);
		Go.to(this, TweenTime, new GoTweenConfig().floatProp("Scale", ScalePressed));
		mPressed = true;
		if (this.mHandleTap != null)
		{
			this.mHandleTap(this, touch);
		}
	}

	public virtual void HandleTouchMove(FUICollider button, FTouch touch)
	{
		if (mEnabled && this.mHandleMove != null)
		{
			this.mHandleMove(this, touch);
		}
	}

	public virtual void HandleTouchRelease(FUICollider button, FTouch touch)
	{
		if (mEnabled)
		{
			TweenToNormal();
			mPressed = false;
			if (this.mHandleRelease != null)
			{
				this.mHandleRelease(this, touch);
			}
		}
	}

	public virtual void HandleTouchCancel(FUICollider button, FTouch touch)
	{
		if (mEnabled)
		{
			TweenToNormal();
			mPressed = false;
			if (this.mHandleCancel != null)
			{
				this.mHandleCancel(this, touch);
			}
		}
	}

	public virtual void Enable(bool v, bool force = false)
	{
		if (force || mEnabled != v)
		{
			mEnabled = v;
			Color = v ? Color.white : Color.grey;
		}
	}

	public virtual void Disable(bool changeColor, bool force)
	{
		if (force || mEnabled)
		{
			mEnabled = false;
			if (changeColor)
			{
				Color = Color.grey;
			}
		}
	}

	public void ClearTooltip()
	{
		mTooltipTextLines.Clear();
		if (mTooltip.Parent != null)
		{
			mTooltip.Parent.RemoveChild(mTooltip);
		}
	}

	public void AddCustomTooltipTextLine(FUITooltipLine line)
	{
		mTooltipTextLines.Add(line);
	}

	public void AddTooltipTextLine(string locString, bool localize, Color color, HorizSide justifyX)
	{
		if (localize)
		{
			locString = FLoc.GetString(locString);
		}
		FUITooltipLine line = new FUITooltipLine(locString, true, true, "DefaultStyle", 36f, color, justifyX);
		mTooltipTextLines.Add(line);
	}

	public void AddTooltipTextLine(string locString)
	{
		string text = FLoc.GetString(locString);
		FUITooltipLine line = new FUITooltipLine(text, true, true, "DefaultStyle", 36f, FColorNames.Get("UITitle"), HorizSide.Right);
		mTooltipTextLines.Add(line);
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		Futile.Instance.AddSignalUpdate(HandleTooltipUpdate);
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		Futile.Instance.RemoveSignalUpdate(HandleTooltipUpdate);
	}

	private Vector2 GetTooltipOffset()
	{
		int dock = (int)mTooltipDock;
		if ((uint)dock < 16u)
		{
			return new Vector2(cTooltipOffsetX[dock], cTooltipOffsetY[dock]);
		}
		return Vector2.zero;
	}

	private static readonly float[] cTooltipOffsetX = new float[16]
	{
		-20f, 0f, 0f, 0f, 20f, -20f, -20f, -20f, 20f, 20f,
		20f, -20f, 0f, 0f, 0f, 20f
	};

	private static readonly float[] cTooltipOffsetY = new float[16]
	{
		20f, 20f, 20f, 20f, 20f, 0f, 0f, 0f, 0f, 0f,
		0f, -20f, -20f, -20f, -20f, -20f
	};

	protected void HandleTooltipUpdate()
	{
		if (!mPressed)
		{
			mPressDuration = 0f;
		}
		else
		{
			mPressDuration += Time.deltaTime;
		}
		if (mTooltipTextLines.Count > 0 && mPressDuration >= 0.5f && TopHudPage != null)
		{
			FNode topPage = TopHudPage();
			if (topPage != null)
			{
				FNode p = this;
				do
				{
					p = p.Parent;
					if (p == topPage)
					{
						break;
					}
				}
				while (p != null);
				if (p != null)
				{
					mTooltip.SetParent(this);
					mTooltip.Show(mTooltipWidth, mTooltipTextLines);
					mTooltip.DockOutside(this, mTooltipDock, mTooltip.TooltipSize, GetTooltipOffset());
					return;
				}
			}
		}
		if (mTooltip.Parent != null)
		{
			mTooltip.Parent.RemoveChild(mTooltip);
		}
	}

	public void ReturnToNormal()
	{
		TweenToNormal();
	}

	protected virtual void TweenToNormal()
	{
		GoTweenConfig bgConfig = new GoTweenConfig().colorProp("Color", BackgroundColorNormal);
		if (mBackground != null)
		{
			Go.killAllTweensWithTarget(mBackground);
			Go.to(mBackground, TweenTime, bgConfig);
		}
		GoTweenConfig fgConfig = new GoTweenConfig().colorProp("Color", ForegroundColorNormal);
		if (mForeground != null)
		{
			Go.killAllTweensWithTarget(mForeground);
			Go.to(mForeground, TweenTime, fgConfig);
		}
		if (mLabel != null)
		{
			Go.killAllTweensWithTarget(mLabel);
			Go.to(mLabel, TweenTime, fgConfig);
		}
		Go.killAllTweensWithTarget(this);
		Go.to(this, TweenTime, new GoTweenConfig().floatProp("Scale", ScaleNormal));
	}

	public override bool CalculateExtents()
	{
		bool baseResult = base.CalculateExtents();
		bool labelResult = mLabel == null || mLabel.CalculateExtents();
		return baseResult & labelResult;
	}

	public void Flash()
	{
		Flash(HintTint);
	}

	public void FlashNewlyActive(bool useColor, bool useScale)
	{
		Go.killAllTweensWithTarget(this);
		if (useColor)
		{
			AdditiveTintColor = new Color(0f, 0f, 0f, 0f);
			GoTweenConfig colorConfig = new GoTweenConfig().colorProp("AdditiveTintColor", NewlyActiveTint).setIterations(2, GoLoopType.PingPong);
			Go.addTween(new GoTween(this, 0.1f, colorConfig));
		}
		if (useScale)
		{
			GoTweenConfig scaleConfig = new GoTweenConfig().floatProp("Scale", 1.1f).setIterations(2, GoLoopType.PingPong);
			Go.addTween(new GoTween(this, 0.15f, scaleConfig));
		}
	}

	public void Flash(Color additiveColor)
	{
		Go.killAllAbstractTweensWithTarget(this);
		AdditiveTintColor = new Color(0f, 0f, 0f, 0f);
		GoTweenConfig config = new GoTweenConfig().colorProp("AdditiveTintColor", additiveColor).setIterations(-1, GoLoopType.PingPong);
		mFlashTween = new GoTween(this, 0.2f, config);
		Go.addTween(mFlashTween);
	}

	public void ClearFlash()
	{
		if (mFlashTween != null)
		{
			Go.removeTween(mFlashTween);
			mFlashTween = null;
			AdditiveTintColor = new Color(0f, 0f, 0f, 0f);
		}
	}

	public void AddHandleTap(Action<FUIButton, FTouch> d)
	{
		mHandleTap -= d;
		mHandleTap += d;
	}

	public void RemoveHandleTap(Action<FUIButton, FTouch> d)
	{
		mHandleTap -= d;
	}

	public void AddHandleMove(Action<FUIButton, FTouch> d)
	{
		mHandleMove -= d;
		mHandleMove += d;
	}

	public void RemoveHandleMove(Action<FUIButton, FTouch> d)
	{
		mHandleMove -= d;
	}

	public void AddHandleDisabled(Action<FUIButton, FTouch> d)
	{
		mHandleDisabled -= d;
		mHandleDisabled += d;
	}

	public void RemoveHandleDisabled(Action<FUIButton, FTouch> d)
	{
		mHandleDisabled -= d;
	}

	public void ClearHandleRelease()
	{
		this.mHandleRelease = null;
	}

	public void AddHandleRelease(Action<FUIButton, FTouch> d)
	{
		mHandleRelease -= d;
		mHandleRelease += d;
	}

	public void RemoveHandleRelease(Action<FUIButton, FTouch> d)
	{
		mHandleRelease -= d;
	}

	public void AddHandleCancel(Action<FUIButton, FTouch> d)
	{
		mHandleCancel -= d;
		mHandleCancel += d;
	}

	public void RemoveHandleCancel(Action<FUIButton, FTouch> d)
	{
		mHandleCancel -= d;
	}

	public bool IsMouseOver()
	{
		return mCollider.ContainsPoint(Futile.GetMousePosition());
	}

	public void TurnOnHighlight()
	{
		if (mHighlightFX == null)
		{
			mHighlightFX = new FGameObjectNode("Prefabs/FX/UIPilotTeach");
			mHighlightFX.sortZ = 1000f;
			mHighlightFX.SetParent(this);
		}
	}

	public void TurnOffHighlight()
	{
		if (mHighlightFX != null)
		{
			if (mHighlightFX.Parent != null)
			{
				mHighlightFX.Parent.RemoveChild(mHighlightFX);
			}
			mHighlightFX.Dispose();
			mHighlightFX = null;
		}
	}

	public void TurnOnSpinner()
	{
		if (mSpinner == null)
		{
			mSpinner = new FSprite("Spinner.png");
			mSpinner.SetParent(this);
			mSpinner.Color = new Color(0f, 1f, 1f, 1f);
			float height = Height;
			float width = Width;
			float size = (width <= height) ? width : height;
			mSpinner.DockInside(this, InnerDock.Center, size, size, 0f, 0f);
			mSpinner.AddAction(new FEntityActionRepeat(-1, new FEntityActionTweenPropertyByFloat(1.5f, "Rotation", 360f)));
		}
	}

	public void TurnOffSpinner()
	{
		FNode.Cleanup(mSpinner);
		mSpinner = null;
	}

	public override bool CanGainFocus()
	{
		return mEnabled;
	}

	public override void HandleGainFocus()
	{
		base.HandleGainFocus();
		if (mBackground != null && !string.IsNullOrEmpty(FocusBackgroundSprite))
		{
			SetBackground(FocusBackgroundSprite);
		}
		if (mFocusSprite != null)
		{
			mFocusSprite.SetParent(this);
		}
	}

	public override void HandleLoseFocus()
	{
		if (mBackground != null && !string.IsNullOrEmpty(NonFocusBackgroundSprite))
		{
			SetBackground(NonFocusBackgroundSprite);
		}
		if (mFocusSprite != null && mFocusSprite.Parent != null)
		{
			mFocusSprite.Parent.RemoveChild(mFocusSprite);
		}
		base.HandleLoseFocus();
	}

	public override void HandleKeyInput(string input)
	{
		if (!mHasFocus)
		{
			return;
		}
		for (int i = 0; i < input.Length; i++)
		{
			char c = input[i];
			if (c == FUtil.ASCIIEscape)
			{
				if (this.mHandleEscape != null)
				{
					this.mHandleEscape(this, string.Empty);
				}
			}
			else if (c == FUtil.ASCIIHorizontalTab)
			{
				if (this.mHandleTab != null)
				{
					this.mHandleTab(this, string.Empty, true);
				}
			}
			else if (c == FUtil.ASCIIVerticalTab)
			{
				if (this.mHandleTab != null)
				{
					this.mHandleTab(this, string.Empty, false);
				}
			}
			else if (c == '`')
			{
				if (this.mHandleBackquote != null)
				{
					this.mHandleBackquote(this, string.Empty);
				}
			}
			else if (c == FUtil.ASCIILineFeed || c == FUtil.ASCIICarriageReturn)
			{
				if (this.mHandleEnter != null)
				{
					this.mHandleEnter(this, string.Empty);
				}
			}
		}
	}

	public void AddHandleEnter(Action<FNode, string> d)
	{
		mHandleEnter -= d;
		mHandleEnter += d;
	}

	public void AddHandleEscape(Action<FNode, string> d)
	{
		mHandleEscape -= d;
		mHandleEscape += d;
	}

	public void AddHandleTab(Action<FNode, string, bool> d)
	{
		mHandleTab -= d;
		mHandleTab += d;
	}

	public void AddHandleBackquote(Action<FNode, string> d)
	{
		mHandleBackquote -= d;
		mHandleBackquote += d;
	}
}
