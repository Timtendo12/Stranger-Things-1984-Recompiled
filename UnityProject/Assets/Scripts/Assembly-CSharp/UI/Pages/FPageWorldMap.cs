using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageWorldMap @ 0x18129cc..0x18148f0, Ghidra vaddr).
public class FPageWorldMap : FUIPage
{
	protected FUICollider mCollider;

	protected FNode mDialog;

	protected FUIButton mCloseButton;

	protected FSprite mHUDCover;

	protected Vector2 mLastTouchPos = Vector2.zero;

	protected FLabelDropshadow mPercentCompleteLabel;

	protected FLabelDropshadow mTimeSpentLabel;

	protected FUIButton mLeaveDungeonButton;

	protected bool mTouchDown;

	protected FNodeStack mDungeonItems;

	public bool TouchDown => mTouchDown;

	public FPageWorldMap(string name)
		: base(name)
	{
		Modal = true;
		DarkenLower = true;
		TransitionStyle = TransitionStyles.Fade;
		mCollider = new FUICollider("Map");
		mCollider.AddSignalPress(HandleScreenCollider);
		mCollider.AddSignalMove(HandleScreenColliderMove);
		mCollider.AddSignalCancel(HandleScreenColliderRelease);
		mCollider.AddSignalRelease(HandleScreenColliderRelease, 40f);
		mCollider.SetParent(this);
		mDialog = new FNode("DialogMap");
		mDialog.SetParent(this);
		mHUDCover = new FSprite(Name + ".HUDCover", "BlackSquare.png");
		mHUDCover.SetParent(mDialog);
		mCloseButton = new FUIButton(Name + ".CloseButton");
		mCloseButton.SetBackground("ButtonClose.png");
		mCloseButton.AddHandleTap(HandleCancelButton);
		mCloseButton.SetParent(mDialog);
		mLeaveDungeonButton = new FUIButton(Name + ".LeaveDungeonButton");
		mLeaveDungeonButton.SetParent(mHUDCover);
		mLeaveDungeonButton.SetText(FLoc.GetString("LeaveDungeonButton"), "DefaultStyle", 32f, 14f);
		mLeaveDungeonButton.SetBackground("Button004.png");
		mLeaveDungeonButton.SetForegroundColor(new Color(1f, 0f, 0f, 1f));
		mLeaveDungeonButton.AddHandleRelease(HandleLeaveDungeonButton);
		mPercentCompleteLabel = new FLabelDropshadow(Name + ".PercentCompleteLabel", "DefaultStyle", "0", 24f, false, true, false);
		mPercentCompleteLabel.Wrap(true);
		mPercentCompleteLabel.SetParent(mHUDCover);
		mTimeSpentLabel = new FLabelDropshadow(Name + ".TimeSpentLabel", "DefaultStyle", "0", 24f, false, true, false);
		mTimeSpentLabel.Wrap(true);
		mTimeSpentLabel.SetParent(mHUDCover);
		mDungeonItems = new FNodeStack(Name + ".DungeonItems", FNodeStack.Modes.FromLeft, 32f);
		mDungeonItems.SetParent(mDialog);
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		mCollider.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		mDialog.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		Vector2 hud = FApp.HUD.PageLegendWorld.HUDSize;
		if (!Futile.Screen.IsWidescreen)
		{
			mHUDCover.DockInside(mDialog, InnerDock.Top, hud.x, hud.y, 0f, 0f);
			mCloseButton.DockInside(mDialog, InnerDock.TopRight, 128f, 128f, -32f, -32f);
			mPercentCompleteLabel.DockInside(mHUDCover, InnerDock.Left, 256f, hud.y, 0f, 0f);
			mTimeSpentLabel.DockOutside(mPercentCompleteLabel, OuterDock.Right, 256f, hud.y, 16f, 0f);
			mLeaveDungeonButton.DockOutside(mTimeSpentLabel, OuterDock.Right, 256f, hud.y, 32f, 0f);
		}
		else
		{
			mHUDCover.DockInside(mDialog, InnerDock.Right, hud.x, hud.y, 0f, 0f);
			mCloseButton.DockInside(mDialog, InnerDock.TopRight, 128f, 128f, -32f, -32f);
			mPercentCompleteLabel.DockInside(mHUDCover, InnerDock.Bottom, hud.x, 128f, 0f, 0f);
			mTimeSpentLabel.DockOutside(mPercentCompleteLabel, OuterDock.Top, hud.x, 128f, 0f, 16f);
			mLeaveDungeonButton.DockOutside(mTimeSpentLabel, OuterDock.Top, hud.x, 128f, 0f, 32f);
		}
		mDungeonItems.DockInside(mDialog, InnerDock.BottomLeft, 1000f, 128f, 32f, 16f);
		MarkDirtyRecursive();
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		TransitionIn(mDialog);
		FApp.World.Pause(true, true);
		FGameUtil.ExecuteAtEndOfFrame(FApp.World.ShowMap);
		mPercentCompleteLabel.Text = FApp.PlayerStats.GetPercentCompleteString(false);
		mTimeSpentLabel.Text = FLoc.Format("TimeSpentFormat", FLoc.GetFormattedTime(FApp.PlayerStats.TotalGameTime));
		FApp.HUD.PageLegendWorld.x = Futile.Screen.Width;
		if (FApp.World != null)
		{
			if (FApp.World.ProtoWorld.LoadData.ContainsKey("Exit"))
			{
				mLeaveDungeonButton.SetParent(mHUDCover);
			}
			else
			{
				mLeaveDungeonButton.Unparent();
			}
		}
		SetupDungeonItems();
	}

	protected void SetupDungeonItems()
	{
		mDungeonItems.ClearStack(true);
		if (FApp.World == null)
		{
			return;
		}
		System.Collections.Generic.Dictionary<string, string> data = FApp.World.ProtoWorld.LoadData;
		if (!data.ContainsKey("EggoDBID") || !data.ContainsKey("FilmDBID") || !data.ContainsKey("DungeonItem"))
		{
			return;
		}
		string dungeonItem = data["DungeonItem"];
		if (!data.ContainsKey("DungeonItemDBIDs"))
		{
			return;
		}
		int[] itemIds = data["DungeonItemDBIDs"].Split(',').Select(int.Parse).ToArray();
		int[] eggoIds = new int[1] { int.Parse(data["EggoDBID"]) };
		int[] filmIds = new int[1] { int.Parse(data["FilmDBID"]) };
		mDungeonItems.StackChild(new FDungeonItemNode("DungeonItem", dungeonItem + ".png", itemIds), new Vector2(72f, 128f));
		mDungeonItems.StackChild(new FDungeonItemNode("DungeonItem", "HUDEggo.png", eggoIds), new Vector2(72f, 128f));
		mDungeonItems.StackChild(new FDungeonItemNode("DungeonItem", "HopperUpgrade000_Icon.png", filmIds), new Vector2(72f, 128f));
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		TransitionOut(mDialog);
		FApp.World.UnPause();
		FGameUtil.ExecuteAtEndOfFrame(FApp.World.HideMap);
		FApp.HUD.PageLegendWorld.x = 0f;
	}

	protected void HandleScreenCollider(FUICollider collider, FTouch touch)
	{
		mTouchDown = true;
		mLastTouchPos = touch.Position;
	}

	protected void HandleScreenColliderMove(FUICollider collider, FTouch touch)
	{
		Vector2 delta = (touch.Position - mLastTouchPos) * 0.03125f * 0.25f;
		FApp.World.ScrollWorldMap(delta, true);
		mLastTouchPos = touch.Position;
	}

	protected void HandleScreenColliderRelease(FUICollider collider, FTouch touch)
	{
		mTouchDown = false;
		FApp.World.MapScrollEnded();
	}

	protected void HandleCancelButton(FUIButton button, FTouch touch)
	{
		FApp.HUD.PopPage(this);
		FApp.SoundManager.PlaySoundSet("UI.Inventory.Close");
	}

	protected void HandleLeaveDungeonButton(FUIButton button, FTouch touch)
	{
		if (FApp.World == null)
		{
			return;
		}
		System.Collections.Generic.Dictionary<string, string> data = FApp.World.ProtoWorld.LoadData;
		if (!data.ContainsKey("Exit"))
		{
			return;
		}
		string exit = data["Exit"];
		if (!exit.Valid())
		{
			return;
		}
		string[] parts = exit.Split('.');
		if (parts.Length < 2)
		{
			FLog.UnityWarning("World has Exit property of: " + exit + " which is not in the correct format. Please contact your service provider.");
			return;
		}
		FPlayerStartParams startParams = new FPlayerStartParams();
		startParams.EntryPoint = parts[1];
		startParams.PlaySound = "Will_Pipe_Out";
		FApp.World.TryWorldTransition(parts[0], null, startParams);
		Close();
	}

	protected void Close()
	{
		FApp.HUD.PopPage(this);
		FApp.SoundManager.PlaySoundSet("UI.Inventory.Close");
	}
}
