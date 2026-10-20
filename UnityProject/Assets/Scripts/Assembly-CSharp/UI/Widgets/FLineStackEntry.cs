using System.Collections.Generic;
using UnityEngine;

public class FLineStackEntry : FNode
{
	protected Vector2 mSize;

	protected FNodeStack mLineStack;

	protected List<FLineStackEntryLine> mEntries;

	public Vector2 Size => default;

	public FLineStackEntry(string name)
	{
	}

	public void AddLine(string text, string font, float pointSize, Sides justify, Vector2 size)
	{
	}

	public void Layout()
	{
	}
}
