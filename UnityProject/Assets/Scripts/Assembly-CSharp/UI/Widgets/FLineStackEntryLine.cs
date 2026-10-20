using UnityEngine;

public class FLineStackEntryLine : FNode
{
	protected Vector2 mSize;

	protected string mText;

	protected string mFont;

	protected float mPointSize;

	protected Sides mJustify;

	protected FLabel mLabel;

	public Vector2 Size => default;

	public FLineStackEntryLine(string text, string font, float pointSize, Sides justify, Vector2 size)
	{
	}

	public void Layout()
	{
	}
}
