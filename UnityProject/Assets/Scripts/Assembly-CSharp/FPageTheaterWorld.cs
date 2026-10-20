using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageTheaterWorld @ 0x1812104..0x18129cc, Ghidra vaddr).
public class FPageTheaterWorld : FUIPage
{
	protected FUICollider mCollider;

	protected FNode mDialog;

	protected FUIButton mCloseButton;

	protected FSprite mHUDCover;

	public FPageTheaterWorld(string name)
		: base(name)
	{
		base.TransitionStyle = TransitionStyles.Fade;
		mCollider = new FUICollider("TheaterMenu", false);
		mCollider.SetParent(this);
		mDialog = new FNode("DialogTheaterMenu");
		mDialog.SetParent(this);
		mHUDCover = new FSprite(Name + ".HUDCover", "BlackSquare.png");
		mHUDCover.SetParent(mDialog);
		mCloseButton = new FUIButton(Name + ".CloseButton");
		mCloseButton.SetBackground("ButtonClose.png");
		mCloseButton.AddHandleTap(HandleCancelButton);
		mCloseButton.SetParent(mDialog);
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		mCollider.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		mDialog.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		Vector2 hud = FApp.HUD.PageLegendWorld.HUDSize;
		mHUDCover.DockInside(mDialog, Futile.Screen.IsWidescreen ? InnerDock.Right : InnerDock.Top, hud.x, hud.y, 0f, 0f);
		mCloseButton.DockInside(mDialog, InnerDock.TopRight, 128f, 128f, -32f, -32f);
		MarkDirtyRecursive();
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		TransitionIn(mDialog);
		FApp.HUD.PageLegendWorld.x = Futile.Screen.Width;
		ShowCloseButton(true);
	}

	public void ShowCloseButton(bool show)
	{
		if (show)
		{
			mCloseButton.SetParent(mDialog);
		}
		else
		{
			mCloseButton.Unparent();
		}
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		TransitionOut(mDialog);
		FApp.HUD.PageLegendWorld.x = 0f;
	}

	protected void HandleCancelButton(FUIButton button, FTouch touch)
	{
		Close();
	}

	protected void Close()
	{
		if (FApp.World != null)
		{
			FWorldTheater theater = FApp.World as FWorldTheater;
			if (theater != null)
			{
				theater.ExitTheater();
			}
		}
		FApp.SoundManager.PlaySoundSet("UI.Inventory.Close");
	}
}
