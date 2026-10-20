using UnityEngine;

public class FUIScrollListHorizontal : FUIScrollList
{
	private const float cBeginFadeoutMinGlideVelocity = 0.1f;

	private FUIHorizontalScrollbar mScrollbar;

	public Vector2 ItemMargin { get; set; }

	public FUIScrollListHorizontal(string name, Vector2 defaultListSize, float defaultItemWidth, bool allocateScrollBar = true)
		: base(null, default, default)
	{
	}

	protected override void HandleUpdate()
	{
	}

	public override void UpdateSizes(Vector2 defaultListSize, Vector2 defaultListItemSize)
	{
	}

	public override void LayoutListContents()
	{
	}

	public float GetScrollToItemX(int index, bool centerItem)
	{
		return 0f;
	}

	public override void ScrollToIndex(int index, bool center)
	{
	}
}
