using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FNodeDialogBubble : FNode
{
	public const float cTextPtSize = 48f;

	public const string cBGSprite = "TextBubble01.png";

	public const string cTabSprite = "TextBubbleTab01.png";

	public const string cTextFont = "DefaultStyle";

	public static readonly Vector2 cBGSize;

	public static readonly Vector2 cTabSize;

	public static readonly Vector2 cTextMaxSize;

	protected FSlicedSprite mBackground;

	protected FUICollider mCollider;

	protected FSprite mTab;

	protected FLabel mText;

	protected Fix64 mDuration;

	[CompilerGenerated]
	private Action m_HandleTap;

	public bool Dismissed { get; set; }

	protected event Action HandleTap;

	public FNodeDialogBubble(string name, string locString, Action tapAction)
	{
	}

	public Vector2 GetBubbleSize()
	{
		return default;
	}

	public void FlipBubble(bool flipx, bool flipy)
	{
	}

	public void SetTabLocation(OuterDock type, float xOffset, float yOffset, bool flipx, bool flipy)
	{
	}

	public void HandleColliderRelease(FUICollider collider, FTouch touch)
	{
	}
}
