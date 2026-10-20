using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUITooltipLine @ 0x16a5630..0x16a5700, Ghidra vaddr).
public struct FUITooltipLine
{
	private string mText;

	private bool mUsesMarkup;

	private bool mWrap;

	private string mFontName;

	private float mPointSize;

	private Color mTextColor;

	private HorizSide mJustifyX;

	public string Text => mText;

	public bool UsesMarkup => mUsesMarkup;

	public bool Wrap => mWrap;

	public string FontName => mFontName;

	public float PointSize => mPointSize;

	public Color TextColor => mTextColor;

	public HorizSide JustifyX => mJustifyX;

	public FUITooltipLine(string text, bool usesMarkup, bool wrap, string fontName, float pointSize, Color textColor, HorizSide justifyX)
	{
		// null fontName falls back to string.Empty (0x16a5630)
		mText = text;
		mUsesMarkup = usesMarkup;
		mWrap = wrap;
		mFontName = fontName ?? string.Empty;
		mPointSize = pointSize;
		mTextColor = textColor;
		mJustifyX = justifyX;
	}
}
