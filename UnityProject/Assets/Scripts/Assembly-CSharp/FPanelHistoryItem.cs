using UnityEngine;

public class FPanelHistoryItem : FUIScrollListItem
{
	public enum Types
	{
		None = 0,
		Error = 1,
		Chat = 2,
		Normal = 3
	}

	public readonly float cItemWidth;

	public static Vector2 cDefaultMargin;

	public static Vector2 cDefaultMarginIndented;

	public static float cIndentedOffset;

	public static float cHeight;

	public static Vector2 cPicSize;

	public static Vector2 cFrameSize;

	public static float cTextXOffset;

	public static Vector2 cDefaultTextSize;

	public static Vector2 cPicTextSize;

	public static Color cColorNormal;

	public static Color cColorError;

	protected FPanelHistoryItemData mItemData;

	protected FLabel mTextDisplay;

	public FPanelHistoryItemData Data => null;

	public FPanelHistoryItem(Vector2 size, FPanelHistoryItemData data)
	{
	}

	public void RefreshItemSize()
	{
	}

	public override void SetParentContainerCollider(FUICollider parentCollider)
	{
	}
}
