using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FTextEntry @ 0x169f11c..0x16a03bc, Ghidra vaddr).
public class FTextEntry : FDimNode
{
	public enum Modes
	{
		None = 0,
		Alphanumeric = 1,
		Alpha = 2,
		Numeric = 3
	}

	protected Modes mMode = Modes.Alphanumeric;

	protected FLabel mTextDisplay;

	protected FUICollider mCollider;

	protected string mTextString = string.Empty;

	protected Sides mDefaultJustificationX = Sides.CenterX;

	protected Color mTextColor = new Color(1f, 1f, 1f, 1f);

	protected float mCursorWidth = 16f;

	protected Vector2 mCursorOffset = new Vector2(32f, 0f);

	protected FSprite mCursor;

	[CompilerGenerated]
	private Action<FNode, string> m_mHandleEnter;

	[CompilerGenerated]
	private Action<FNode, string> m_mHandleEscape;

	[CompilerGenerated]
	private Action<FNode, string, bool> m_mHandleTab;

	[CompilerGenerated]
	private Action<FNode, string> m_mHandleBackquote;

	[CompilerGenerated]
	private Action<FNode> m_mHandleWantFocus;

	protected bool mEditing = true;

	protected readonly int cMaxInputLength;

	protected bool mNeedsEscaped = true;

	public FKeyboardOptions KeyboardOptions;

	public string EscapedText => FCoreUtil.EscapeXML(mTextString);

	public string Text
	{
		get
		{
			if (mNeedsEscaped)
			{
				return EscapedText;
			}
			return mTextString;
		}
	}

	protected event Action<FNode, string> mHandleEnter;

	protected event Action<FNode, string> mHandleEscape;

	protected event Action<FNode, string, bool> mHandleTab;

	protected event Action<FNode, string> mHandleBackquote;

	protected event Action<FNode> mHandleWantFocus;

	public FTextEntry(string name, string font, float fontSize, HorizSide justificationX, VertSide justificationY, int maxInputLength, bool allowWrap = false, bool needsEscaped = true)
		: base(name)
	{
		cMaxInputLength = maxInputLength;
		mWantsFocus = true;
		ClipToExtents = true;
		mTextDisplay = new FLabel(font, string.Empty, fontSize, false, false, false);
		mTextDisplay.SetParent(this);
		mTextDisplay.Color = mTextColor;
		mTextDisplay.Wrap = allowWrap;
		mTextDisplay.UsesMarkup = false;
		Sides x = Sides.CenterX;
		if (justificationX == HorizSide.Right)
		{
			x = Sides.Right;
		}
		Sides defaultX = Sides.Left;
		if (justificationX != HorizSide.Left)
		{
			defaultX = x;
		}
		mDefaultJustificationX = defaultX;
		mTextDisplay.JustificationX = defaultX;
		switch (justificationY)
		{
		case VertSide.Bottom:
			mTextDisplay.JustificationY = Sides.Bottom;
			break;
		case VertSide.Top:
			mTextDisplay.JustificationY = Sides.Top;
			break;
		default:
			mTextDisplay.JustificationY = Sides.CenterY;
			break;
		}
		mNeedsEscaped = needsEscaped;
		mTextDisplay.ExpandToFill(this, mCursorWidth * -2f, 0f);
		mCollider = new FUICollider(Name + ".Collider", false);
		mCollider.SetParent(this);
		mCollider.ExpandToFill(this, 0f, 0f);
		mCollider.AddSignalPress(HandleColliderPress);
		KeyboardOptions.KeyboardType = TouchScreenKeyboardType.Default;
		KeyboardOptions.Autocorrect = true;
		if (!Futile.OnSmallScreen)
		{
			mCursorWidth = 3f;
		}
		mCursor = new FSprite("WhiteSquare.png");
		mCursor.SetParent(this);
		mCursor.Color = mTextColor;
		mCursor.DockInside(this, InnerDock.Center, mCursorWidth, fontSize, mCursorOffset.x, mCursorOffset.y);
		mCursor.AddAction(new FEntityActionRepeat(-1, new FEntityActionSequence(new FEntityAction[2]
		{
			new FEntityActionTweenPropertyToColor(0.3f, "Color", Color.grey),
			new FEntityActionTweenPropertyToColor(0.3f, "Color", new Color(1f, 1f, 1f, 1f))
		})));
		SyncDisplay();
	}

	public override void HandleAddedToStage()
	{
		SyncDisplay();
		base.HandleAddedToStage();
		Futile.Instance.RemoveSignalUpdate(HandleUpdate);
		Futile.Instance.AddSignalUpdate(HandleUpdate);
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		Futile.Instance.RemoveSignalUpdate(HandleUpdate);
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
	}

	public void SetSecure(bool which)
	{
		KeyboardOptions.Secure = which;
	}

	public void SetKeyboardType(TouchScreenKeyboardType newType)
	{
		KeyboardOptions.KeyboardType = newType;
	}

	public void SetKeyboardMultiLine(bool multiline)
	{
		KeyboardOptions.Multiline = multiline;
	}

	public void SetKeyboardAutocorrect(bool autocorrect)
	{
		KeyboardOptions.Autocorrect = autocorrect;
	}

	protected void SyncDisplay()
	{
		string display = mTextString;
		if (KeyboardOptions.Secure)
		{
			display = new string('*', mTextString.Length);
		}
		mTextDisplay.Text = display;
		float textWidth = 0f;
		if (mTextDisplay.HasConstraints() && !string.IsNullOrEmpty(mTextDisplay.Text))
		{
			textWidth = mTextDisplay.TextWidth;
		}
		Sides justification = ((!(textWidth <= Mathf.Epsilon) && !(textWidth + mCursorWidth + mCursorOffset.x <= Width)) ? Sides.Right : mDefaultJustificationX);
		mTextDisplay.JustificationX = justification;
		if (!mEditing)
		{
			mCursor.Visible = false;
		}
		else
		{
			float extra = (Futile.OnSmallScreen ? 6f : 0f);
			Vector2 cursorPosition;
			if (!string.IsNullOrEmpty(mTextString))
			{
				cursorPosition = new Vector2(extra + mTextDisplay.CursorPosition.x, mTextDisplay.CursorPosition.y);
			}
			else
			{
				cursorPosition = mTextDisplay.DefaultCursorPosition;
			}
			mCursor.Position = cursorPosition;
		}
		MarkDirtyRecursive();
	}

	public void SetText(string text, bool escaped = false)
	{
		if (escaped)
		{
			text = FCoreUtil.UnEscapeXML(text);
		}
		text = text.StripEmoji();
		if (cMaxInputLength < text.Length)
		{
			text = text.Substring(0, cMaxInputLength);
		}
		mTextString = text;
		SyncDisplay();
	}

	protected bool ProcessDelete()
	{
		int length = mTextString.Length;
		if (0 < length)
		{
			mTextString = mTextString.Remove(length - 1);
		}
		return 0 < length;
	}

	protected bool ProcessAdd(char letter)
	{
		int length = mTextString.Length;
		if (length < cMaxInputLength)
		{
			mTextString += letter.ToString();
		}
		return length < cMaxInputLength;
	}

	public override void HandleKeyInput(string input)
	{
		if (!mEditing)
		{
			return;
		}
		string stripped = input.StripEmoji();
		if (TouchScreenKeyboard.visible)
		{
			mTextString = stripped;
			SyncDisplay();
			return;
		}
		bool changed = false;
		for (int i = 0; i < stripped.Length; i++)
		{
			char c = stripped[i];
			if (c == FUtil.ASCIIEscape)
			{
				if (this.mHandleEscape != null)
				{
					this.mHandleEscape(this, mTextString);
				}
			}
			else if (c == FUtil.ASCIIHorizontalTab)
			{
				if (this.mHandleTab != null)
				{
					this.mHandleTab(this, mTextString, true);
				}
			}
			else if (c == FUtil.ASCIIVerticalTab)
			{
				if (this.mHandleTab != null)
				{
					this.mHandleTab(this, mTextString, false);
				}
			}
			else if (c == '`' && this.mHandleBackquote != null)
			{
				this.mHandleBackquote(this, mTextString);
			}
			else if (c == '`')
			{
				changed |= ProcessAdd('`');
			}
			else if (c == FUtil.ASCIILineFeed || c == FUtil.ASCIICarriageReturn)
			{
				if (this.mHandleEnter != null)
				{
					this.mHandleEnter(this, mNeedsEscaped ? EscapedText : mTextString);
				}
				else
				{
					changed |= ProcessAdd('\r');
				}
			}
			else if (c == FUtil.ASCIIDelete || stripped[0] == FUtil.ASCIIBackspace)
			{
				changed |= ProcessDelete();
			}
			else
			{
				changed |= ProcessAdd(c);
			}
		}
		if (changed)
		{
			SyncDisplay();
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

	public void AddHandleWantFocus(Action<FNode> d)
	{
		mHandleWantFocus -= d;
		mHandleWantFocus += d;
	}

	public void SetEditing(bool v)
	{
		mEditing = v;
		if (mCursor != null)
		{
			mCursor.Visible = v;
		}
	}

	public virtual void HandleColliderPress(FUICollider collider, FTouch touch)
	{
		if (this.mHandleWantFocus != null)
		{
			this.mHandleWantFocus(this);
		}
		if (Futile.Instance.OpenAppKeyboard != null)
		{
			Futile.Instance.OpenAppKeyboard(delegate(bool didFinish, string text)
			{
				if (didFinish)
				{
					SetText(text);
					if (this.mHandleEnter != null)
					{
						this.mHandleEnter(this, mNeedsEscaped ? EscapedText : mTextString);
					}
				}
			}, KeyboardOptions, mNeedsEscaped ? EscapedText : mTextString, string.Empty);
		}
	}

	public override bool CanGainFocus()
	{
		return true;
	}

	public override void HandleGainFocus()
	{
		base.HandleGainFocus();
		SetEditing(true);
		SyncDisplay();
	}

	public override void HandleLoseFocus()
	{
		SetEditing(false);
		base.HandleLoseFocus();
	}
}
