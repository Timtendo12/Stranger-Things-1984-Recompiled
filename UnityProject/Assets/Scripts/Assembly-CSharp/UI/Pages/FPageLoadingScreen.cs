// Rewritten from libil2cpp.so 1.0.605 (FPageLoadingScreen @ 0x180cd94..0x180d188, Ghidra vaddr).
public class FPageLoadingScreen : FUIPage
{
	protected FNode mUIContainer;

	protected FSprite mBGBackdrop;

	protected FLabelDropshadow mLoadingLabel;

	protected bool mIntroFinished;

	static FPageLoadingScreen()
	{
	}

	public FPageLoadingScreen(string name)
		: base(name)
	{
		mUIContainer = new FNode("UIContainer");
		mUIContainer.SetParent(this);
		mUIContainer.ExpandToFill(this, 0f, 0f);
		mBGBackdrop = new FSprite("BlackSquare.png");
		mBGBackdrop.SetParent(mUIContainer);
		mBGBackdrop.ExpandToFill(mUIContainer, 0f, 0f);
		mLoadingLabel = new FLabelDropshadow(Name + ".Subtitle", "DefaultStyle", FLoc.GetString("Loading"), 48f, true, true, false);
		mLoadingLabel.SetTextColor(new UnityEngine.Color(1f, 1f, 1f, 1f), FUI.Colors.LegendRed);
		mLoadingLabel.Justify(Sides.CenterX, Sides.CenterY);
		mLoadingLabel.SetParent(mUIContainer);
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		mLoadingLabel.DockInside(mUIContainer, InnerDock.Center, 1000f, 72f, 0f, 0f);
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		if (FApp.HUD != null && FApp.HUD.PageLegendWorld != null)
		{
			if (!FApp.HUD.PageLegendWorld.OnStage)
			{
				if (!mBGBackdrop.HasParent)
				{
					mBGBackdrop.SetParent(mUIContainer);
				}
			}
			else if (mBGBackdrop.HasParent)
			{
				mBGBackdrop.Unparent();
			}
		}
	}
}
