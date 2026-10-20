using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUIHeartPieceItem @ 0x17f875c..0x17f8bec, Ghidra vaddr).
public class FUIHeartPieceItem : FUIScrollListItem
{
	protected FUICollider mCollider;

	protected FSprite mIcon;

	protected FLabel mLabel;

	public int Index { get; protected set; }

	public bool Collected { get; protected set; }

	public bool IsSelected { get; protected set; }

	public FUIHeartPieceItem(int index, bool collected)
	{
		Index = index;
		Name = "HeartPiece." + index.ToString();
		Collected = collected;
		mCollider = new FUICollider(Name + ".Collider");
		mCollider.SetParent(this);
		mCollider.AddSignalRelease(HandleRelease, 40f);
		mCollider.ExpandToFill(this, 0f, 0f);
		mIcon = new FSprite(Name + ".StateIcon", "HUDHeartPiece.png");
		mIcon.ExpandToFill(this, 0f, 0f);
		mIcon.SetParent(this);
		mLabel = new FLabel(Name + ".Title", "DefaultStyle", (index + 1).ToString(), 24f, true, true, false);
		mLabel.ShrinkToFitContainer(true);
		mLabel.Color = FColorNames.Get("DarkGray");
		mLabel.ExpandToFill(this, 0f, 0f);
		mLabel.SetParent(this);
		MarkDirtyRecursive();
		UpdateStatus(Collected);
	}

	public void Layout()
	{
	}

	public void UpdateStatus(bool collected)
	{
		Collected = collected;
		if (!collected)
		{
			mIcon.Unparent();
			mLabel.SetParent(this);
		}
		else
		{
			mIcon.SetParent(this);
			mLabel.Unparent();
		}
	}

	public override void SetParentContainerCollider(FUICollider parentCollider)
	{
		if (mCollider != null)
		{
			mCollider.SetParentContainerCollider(parentCollider);
		}
	}

	protected void HandleRelease(FUICollider collider, FTouch touch)
	{
	}

	public override void Select(bool v, bool force = false)
	{
		IsSelected = v;
	}
}
