using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageGenericError @ 0x17a5804..0x17a6078, Ghidra vaddr).
public class FPageGenericError : FUIPage
{
	protected FNode mDialog;

	protected FNode mErrorNode;

	protected FSlicedSprite mErrorBackground;

	protected FLabel mErrorLabel;

	protected float mErrorTimer;

	public FPageGenericError()
		: base("GenericError")
	{
		mDialog = new FNode("GenericErrorDialog");
		mDialog.SetParent(this);
		mErrorNode = new FNode("GenericErrorNode");
		mErrorNode.SetParent(mDialog);
		mErrorBackground = new FSlicedSprite(base.Name + ".Error.Background", "BlackSquare.png");
		mErrorBackground.Alpha = 0.7f;
		mErrorBackground.SetParent(mErrorNode);
		mErrorBackground.ExpandToFill(mErrorNode);
		mErrorLabel = new FLabel(base.Name + ".Error.Label", "DefaultStyle", string.Empty, 48f, true, true, false);
		mErrorLabel.Color = FColorNames.Get("UIError");
		mErrorLabel.Wrap = true;
		mErrorLabel.SetParent(mErrorNode);
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		Vector2 size = Futile.Screen.Size;
		if (mDialog != null)
		{
			mDialog.DockInside(this, InnerDock.Center, size);
		}
		float num = (int)(size.x * 0.5f);
		mErrorNode.DockInside(mDialog, InnerDock.Bottom, num, 96f, 0f, 96f);
		num -= 4f;
		mErrorLabel.DockInside(mErrorNode, InnerDock.Center, num, 92f, Vector2.zero);
		mErrorLabel.ClipWidth = num;
		MarkDirtyRecursive();
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		LayoutPage();
		TransitionIn(mDialog);
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
		if (mErrorTimer > 0f)
		{
			mErrorTimer -= Time.deltaTime;
			if (mErrorTimer <= Mathf.Epsilon)
			{
				TurnOffError(false);
			}
		}
	}

	public void TurnOnError(string text, bool immediate)
	{
		if (mErrorNode != null && mErrorLabel != null)
		{
			if (!base.On)
			{
				FApp.HUD.PushPage(this, true, false);
			}
			FApp.SoundManager.PlaySoundSet("UIFail");
			mErrorLabel.Text = text;
			mErrorTimer = 2f;
		}
	}

	public void TurnOffError(bool immediate)
	{
		if (mErrorNode != null && mErrorNode.Visible)
		{
			FApp.HUD.PopPage(this, false);
			mErrorTimer = 0f;
		}
	}

	protected void CloseDialog()
	{
		FApp.SoundManager.PlaySoundSet("UIButtonClose");
		TransitionOut(mDialog);
	}
}
