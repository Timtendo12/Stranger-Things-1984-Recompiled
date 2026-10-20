using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FConsoleHistoryItem @ 0x17b14f4..0x17b1b1c, Ghidra vaddr).
public class FConsoleHistoryItem : FUIScrollListItem
{
	public const float cHeight = 30f;

	protected readonly float cItemWidth;

	public static readonly Vector2 cDefaultMargin = new Vector2(8f, 8f);

	public static readonly Color cColorCommand = new Color(1f, 1f, 1f, 1f);

	public static readonly Color cColorSavedCommand = Color.grey;

	public static readonly Color cColorOutput = new Color(0.52f, 0.48f, 0.85f, 1f);

	public static readonly Color cColorWarning = new Color(1f, 0.9215686f, 0.015686275f, 1f);

	public static readonly Color cColorError = new Color(1f, 0f, 0f, 1f);

	protected string mText = string.Empty;

	protected FConsoleHistoryType mType;

	protected Vector2 mSize = Vector2.zero;

	protected FLabel mTextDisplay;

	public string Text => mText;

	public FConsoleHistoryType Type => mType;

	public bool IsCommand => mType < FConsoleHistoryType.Output;

	public Vector2 Size => mSize;

	public FConsoleHistoryItem(Vector2 size, string text, FConsoleHistoryType textType)
	{
		cItemWidth = size.x;
		mType = textType;
		float margin = (textType >= FConsoleHistoryType.Output && textType <= FConsoleHistoryType.Error) ? (cDefaultMargin.x + 20f) : cDefaultMargin.x;
		mText = text;
		mTextDisplay = new FLabel("FixedWidthStroke", string.Empty, 20f, false, false, false);
		mTextDisplay.SetParent(this);
		mTextDisplay.JustificationX = FNode.Sides.Left;
		mTextDisplay.JustificationY = FNode.Sides.Top;
		mTextDisplay.Text = text;
		mTextDisplay.Color = GetItemTextColor(textType);
		mTextDisplay.Wrap = true;
		mTextDisplay.DockInside(this, InnerDock.TopLeft, size.x, size.y, margin, 0f);
		RefreshItemSize();
	}

	public override void SetParentContainerCollider(FUICollider parentCollider)
	{
	}

	private Color GetItemTextColor(FConsoleHistoryType textType)
	{
		switch (textType)
		{
		case FConsoleHistoryType.Command:
			return cColorCommand;
		case FConsoleHistoryType.SavedCommand:
			return cColorSavedCommand;
		case FConsoleHistoryType.Output:
			return cColorOutput;
		case FConsoleHistoryType.Warning:
			return cColorWarning;
		case FConsoleHistoryType.Error:
			return cColorError;
		default:
			return Color.white;
		}
	}

	public void RefreshItemSize()
	{
		float margin = (mType >= FConsoleHistoryType.Output && mType <= FConsoleHistoryType.Error) ? (cDefaultMargin.x + 20f) : cDefaultMargin.x;
		float height = 0f;
		if (!string.IsNullOrEmpty(mTextDisplay.Text))
		{
			height = mTextDisplay.TextHeight;
		}
		mTextDisplay.DockInside(this, InnerDock.TopLeft, cItemWidth - margin - cDefaultMargin.x, height, margin, 0f);
		ItemSize = new Vector2(cItemWidth, height);
	}
}
