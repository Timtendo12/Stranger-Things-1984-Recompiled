using UnityEngine;

public class FLineStackNode : FNodeStack
{
	protected Vector2 mSize;

	public Vector2 Size => default;

	public string DefaultFont { get; set; }

	public float DefaultPointSize { get; set; }

	public Sides DefaultJustify { get; set; }

	public Vector2 DefaultSize { get; set; }

	public FLineStackNode(string name, Modes mode, float margin)
		: base(null, Modes.FromBottom, 0f)
	{
	}

	public void AddLine(string text)
	{
	}

	public void AddLine(string text, string font, float pointSize, Sides justify, Vector2 size)
	{
	}
}
