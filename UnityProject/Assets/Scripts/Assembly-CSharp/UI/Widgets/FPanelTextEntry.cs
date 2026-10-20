using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FPanelTextEntry : FNode
{
	protected Vector2 mSize;

	protected FSlicedSprite mBackground;

	protected FSlicedSprite mBackgroundBorder;

	protected FLabel mTitle;

	protected string mTitleString;

	protected FSlicedSprite mTextBackground;

	protected FTextEntry mText;

	protected string mDefaultTextString;

	protected bool mNeedToSetDefaultText;

	protected FLabel mInstruction;

	protected string mInstructionString;

	protected FUIButton mCloseButton;

	protected FUIButton mRandomizeButton;

	protected FUIButton mOkButton;

	[CompilerGenerated]
	private Action<string, bool> m_mHandleDone;

	public Vector2 Size => default;

	protected event Action<string, bool> mHandleDone;

	public FPanelTextEntry(string titleString, string defaultTextString, string instructionString, bool showRandomButton)
	{
	}

	public void Layout()
	{
	}

	public void SetText(string text)
	{
	}

	protected void HandleButtonOk(FUIButton button, FTouch touch)
	{
	}

	public void HandleTextEnter(FNode node, string text)
	{
	}

	public void HandleTextEscape(FNode node, string text)
	{
	}

	protected void HandleRandomizeButton(FUIButton button, FTouch touch)
	{
	}

	protected void HandleCloseButton(FUIButton button, FTouch touch)
	{
	}

	public void Close()
	{
	}

	public void AddHandleDone(Action<string, bool> d)
	{
	}

	public void RemoveHandleDone(Action<string, bool> d)
	{
	}
}
