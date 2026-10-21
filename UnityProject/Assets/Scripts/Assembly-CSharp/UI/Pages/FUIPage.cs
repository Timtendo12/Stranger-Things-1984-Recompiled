using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUIPage @ 0x17d4200..0x17d6bd8, Ghidra vaddr).
public class FUIPage : FNode, FInputHandlerInterface
{
	public enum TransitionStyles
	{
		None = 0,
		Fade = 1,
		Left = 2,
		Right = 3,
		Top = 4,
		Bottom = 5,
		Position = 6,
		FadePosition = 7,
		Scale = 8,
		ScalePosition = 9
	}

	public const float cLetterboxAnimTime = 0.35f;

	protected bool mHandlingResize;

	protected FNode mLetterboxTopBar;

	protected FUICollider mLetterboxTopBarCollider;

	protected FSprite mLetterBoxTopBarBackground;

	protected FNode mLetterboxBottomBar;

	protected FUICollider mLetterboxBottomBarCollider;

	protected FSprite mLetterboxBottomBarBackground;

	protected bool mDoTransition;

	protected bool mDarkened;

	public TransitionStyles TransitionStyle { get; set; }

	public bool DoTransition
	{
		get
		{
			return mDoTransition;
		}
		set
		{
			mDoTransition = value;
		}
	}

	public Vector2 TransitionInPosition { get; set; }

	public GoEaseType TransitionDialogInEaseType { get; set; }

	public float TransitionDialogInTime { get; set; }

	public float TransitionFadeInTime { get; set; }

	public Vector2 TransitionOutPosition { get; set; }

	public GoEaseType TransitionDialogOutEaseType { get; set; }

	public float TransitionDialogOutTime { get; set; }

	public float TransitionFadeOutTime { get; set; }

	public bool IsTransitioning => TransitionInInProgress || TransitionOutInProgress;

	public bool TransitionInInProgress { get; set; }

	public bool TransitionOutInProgress { get; set; }

	public bool On { get; set; }

	public bool IsTopPage => FApp.HUD.TopPage == this;

	public bool Modal { get; set; }

	public bool Darkened => mDarkened;

	public bool DarkenLower { get; set; }

	public FUIPage(string n)
	{
		Name = n;
		On = false;
		SetDefaultTransitionInFade();
		DarkenLower = false;
		Modal = false;
		TransitionInInProgress = false;
		TransitionOutInProgress = false;
		mLetterboxTopBar = new FNode();
		mLetterboxTopBarCollider = new FUICollider("", false);
		mLetterboxTopBarCollider.SetParent(mLetterboxTopBar);
		mLetterBoxTopBarBackground = new FSprite("WhiteSquare.png");
		mLetterBoxTopBarBackground.SetParent(mLetterboxTopBar);
		mLetterBoxTopBarBackground.Color = new Color(0f, 0f, 0f, 1f);
		mLetterboxBottomBar = new FNode();
		mLetterboxBottomBarCollider = new FUICollider("", false);
		mLetterboxBottomBarCollider.SetParent(mLetterboxBottomBar);
		mLetterboxBottomBarBackground = new FSprite("WhiteSquare.png");
		mLetterboxBottomBarBackground.SetParent(mLetterboxBottomBar);
		mLetterboxBottomBarBackground.Color = new Color(0f, 0f, 0f, 1f);
	}

	public virtual void LayoutPage()
	{
		mLetterboxTopBar.DockInside(this, InnerDock.Top, Futile.Screen.Width, 150f, 0f, 0f);
		mLetterboxTopBarCollider.ExpandToFill(mLetterboxTopBar, 0f, 0f);
		mLetterBoxTopBarBackground.ExpandToFill(mLetterboxTopBar, 0f, 0f);
		mLetterboxBottomBar.DockInside(this, InnerDock.Bottom, Futile.Screen.Width, 150f, 0f, 0f);
		mLetterboxBottomBarCollider.ExpandToFill(mLetterboxBottomBar, 0f, 0f);
		mLetterboxBottomBarBackground.ExpandToFill(mLetterboxBottomBar, 0f, 0f);
		MarkDirtyRecursive();
		float width = Futile.Screen.Width;
		float height = Futile.Screen.Height;
		_mExtentsDoNotCallDirectly = new Rect(width * -0.5f, height * -0.5f, width, height);
	}

	protected void SetDefaultTransitionInFade()
	{
		TransitionStyle = TransitionStyles.Fade;
		TransitionDialogInEaseType = GoEaseType.BackOut;
		TransitionInPosition = Vector2.zero;
		TransitionOutPosition = Vector2.zero;
		TransitionDialogOutEaseType = GoEaseType.BackIn;
		TransitionDialogOutTime = 0.25f;
		TransitionFadeOutTime = 0.1f;
		TransitionDialogInTime = 0.25f;
		TransitionFadeInTime = 0.15f;
	}

	protected void SetDefaultTransitionInRight()
	{
		TransitionStyle = TransitionStyles.Right;
		TransitionDialogOutEaseType = GoEaseType.SineIn;
		TransitionDialogInEaseType = GoEaseType.ExpoOut;
		TransitionDialogInTime = 0.25f;
		TransitionFadeInTime = 0.1f;
		TransitionDialogOutTime = 0.25f;
		TransitionFadeOutTime = 0.1f;
	}

	protected override bool HasConstraintError()
	{
		return false;
	}

	public override void HandleAddedToStage()
	{
		TransitionInInProgress = false;
		TransitionOutInProgress = false;
		Futile.Instance.RemoveSignalUpdate(HandleUpdate);
		Futile.Instance.AddSignalUpdate(HandleUpdate);
		base.HandleAddedToStage();
		On = true;
		Color = new Color(1f, 1f, 1f, 1f);
		FApp.TrackScreenView(Name);
	}

	public override void HandleRemovedFromStage()
	{
		Futile.Instance.RemoveSignalUpdate(HandleUpdate);
		base.HandleRemovedFromStage();
		On = false;
	}

	public override void SelfResizeExtents()
	{
		float width = Futile.Screen.Width;
		float height = Futile.Screen.Height;
		_mExtentsDoNotCallDirectly = new Rect(width * -0.5f, height * -0.5f, width, height);
		Position = new Vector2(width * -0.5f + width * 0.5f, height * -0.5f + height * 0.5f);
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
	}

	public virtual bool HandleInput(float deltaTime)
	{
		return false;
	}

	protected Vector2 GetTransitionInPosition(FNode node, TransitionStyles transitionStyle)
	{
		switch (transitionStyle)
		{
		case TransitionStyles.Position:
		case TransitionStyles.FadePosition:
		case TransitionStyles.ScalePosition:
			return TransitionInPosition;
		default:
			return Vector2.zero;
		}
	}

	protected Vector2 GetTransitionOutPosition(FNode node, TransitionStyles transitionStyle)
	{
		switch (transitionStyle)
		{
		case TransitionStyles.Left:
			if (node == null)
			{
				return Vector2.zero;
			}
			return new Vector2(-1f - node.Extents.width, 0f);
		case TransitionStyles.Right:
			if (node == null)
			{
				return Vector2.zero;
			}
			return new Vector2(node.Extents.width + 1f, 0f);
		case TransitionStyles.Top:
			if (node == null)
			{
				return Vector2.zero;
			}
			return new Vector2(0f, node.Extents.height + 1f);
		case TransitionStyles.Bottom:
			if (node == null)
			{
				return Vector2.zero;
			}
			return new Vector2(0f, -1f - node.Extents.height);
		case TransitionStyles.Position:
		case TransitionStyles.FadePosition:
		case TransitionStyles.ScalePosition:
			return TransitionOutPosition;
		default:
			return Vector2.zero;
		}
	}

	public virtual void TransitionIn()
	{

	}

	protected virtual void TransitionIn(FNode dialog)
	{
		TransitionIn(dialog, TransitionDialogInTime, TransitionFadeInTime, TransitionStyle);
	}

	protected virtual void TransitionIn(FNode dialog, float dialogTime, float fadeTime, TransitionStyles transitionStyle)
	{
		if (!mDoTransition)
		{
			if (!On)
			{
				FLog.UnityWarning("Page " + Name + " skipping TransitionIn, but it's not On!");
			}
			mDoTransition = true;
			return;
		}
		TransitionInInProgress = true;
		Vector2 inPosition = GetTransitionInPosition(dialog, transitionStyle);
		Vector2 outPosition = GetTransitionOutPosition(dialog, transitionStyle);
		if (dialog == null)
		{
			return;
		}
		GoTweenConfig config;
		switch (transitionStyle)
		{
		case TransitionStyles.Left:
		case TransitionStyles.Right:
		case TransitionStyles.Top:
		case TransitionStyles.Bottom:
		case TransitionStyles.Position:
			dialog.Position = outPosition;
			config = new GoTweenConfig().vector2Prop("Position", inPosition, false).setEaseType(TransitionDialogInEaseType);
			break;
		case TransitionStyles.Fade:
		case TransitionStyles.FadePosition:
			dialog.Position = inPosition;
			dialog.Alpha = 0f;
			config = new GoTweenConfig().floatProp("Alpha", 1f, false);
			break;
		case TransitionStyles.ScalePosition:
			dialog.Position = outPosition;
			dialog.Scale = 0.01f;
			config = new GoTweenConfig().vector2Prop("Position", inPosition, false).floatProp("Scale", 1f, false).setEaseType(TransitionDialogInEaseType);
			break;
		case TransitionStyles.Scale:
			dialog.Position = inPosition;
			dialog.Scale = 0f;
			config = new GoTweenConfig().floatProp("Scale", 1f, false).setEaseType(TransitionDialogInEaseType);
			break;
		default:
			return;
		}
		config = config.onComplete(OnCompleteTransitionIn);
		Go.to(dialog, dialogTime, config);
	}

	public virtual void TransitionOut()
	{

	}

	protected virtual void TransitionOut(FNode dialog)
	{
		TransitionOut(dialog, TransitionDialogOutTime, TransitionFadeOutTime, TransitionStyle);
	}

	protected virtual void TransitionOut(FNode dialog, float dialogTime, float fadeTime, TransitionStyles transitionStyle)
	{
		TransitionOutInProgress = true;
		Vector2 outPosition = GetTransitionOutPosition(dialog, transitionStyle);
		if (dialog == null)
		{
			return;
		}
		GoTweenConfig config;
		switch (transitionStyle)
		{
		case TransitionStyles.Left:
		case TransitionStyles.Right:
		case TransitionStyles.Top:
		case TransitionStyles.Bottom:
		case TransitionStyles.Position:
			config = new GoTweenConfig().vector2Prop("Position", outPosition, false).setEaseType(TransitionDialogOutEaseType);
			break;
		case TransitionStyles.Fade:
		case TransitionStyles.FadePosition:
			config = new GoTweenConfig().floatProp("Alpha", 0f, false);
			break;
		case TransitionStyles.ScalePosition:
			config = new GoTweenConfig().vector2Prop("Position", outPosition, false).floatProp("Scale", 0f, false).setEaseType(TransitionDialogOutEaseType);
			break;
		case TransitionStyles.Scale:
			config = new GoTweenConfig().floatProp("Scale", 0f, false).setEaseType(TransitionDialogOutEaseType);
			break;
		default:
			return;
		}
		config = config.onComplete(OnCompleteTransitionOut);
		Go.to(dialog, dialogTime, config);
	}

	public virtual void StopAllTransitions()
	{

	}

	public virtual void ForceTransitionComplete()
	{
		OnCompleteTransitionIn(null);
	}

	protected virtual void OnCompleteTransitionIn(AbstractGoTween tween)
	{
		TransitionInInProgress = false;
	}

	protected virtual void OnCompleteTransitionOut(AbstractGoTween tween)
	{
		TransitionOutInProgress = false;
		FApp.HUD.RemovePage(this);
	}

	protected virtual void OnCompleteTransitionOut()
	{
		TransitionOutInProgress = false;
		FApp.HUD.RemovePage(this);
	}

	public virtual void TweenColor(Color color, float duration)
	{
		KillTweens(false);
		AddAction(new FEntityActionTweenPropertyToColor(duration, "Color", color, GoEaseType.Linear));
	}

	public void FlyoffString(string font, int fontSize, string text, Color color, Vector2 startPosition, Vector2 endPosition, float endScale, float delay)
	{
		FLabel st = new FLabel(font, text, fontSize, true, true, false);
		st.SetParent(this);
		st.DockInside(this, InnerDock.Center, 160f, 48f, 0f, 0f);
		st.Color = color;
		st.Position = startPosition;
		st.Scale = 0f;
		st.AddAction(new FEntityActionSequence(new FEntityAction[6]
		{
			new FEntityActionDelay(delay, false),
			new FEntityActionTweenPropertyToFloat(0.05f, "Scale", 0.5f, GoEaseType.SineInOut),
			new FEntityActionSpawn(new FEntityAction[2]
			{
				new FEntityActionTweenPropertyToVector2(0.3f, "Position", startPosition, endPosition, GoEaseType.SineInOut),
				new FEntityActionTweenPropertyToFloat(0.3f, "Scale", 1f, GoEaseType.SineInOut)
			}),
			new FEntityActionSpawn(new FEntityAction[1]
			{
				new FEntityActionTweenPropertyToFloat(0.5f, "Scale", endScale, GoEaseType.SineInOut)
			}),
			new FEntityActionSpawn(new FEntityAction[1]
			{
				new FEntityActionTweenPropertyToFloat(0.2f, "Alpha", 0f, GoEaseType.SineInOut)
			}),
			new FEntityActionCallMethod(delegate
			{
				st.Unparent();
			}, true)
		}));
	}

	public void FlyoffString2(string font, int fontSize, string text, Color color, Vector2 position, string icon, Color iconColor)
	{
		FNode n = new FNode();
		n.SetParent(this);
		n.DockInside(this, InnerDock.Center, 1f, 1f, 0f, 0f);
		FLabel label = new FLabel(font, text, fontSize, true, true, false);
		label.SetParent(n);
		label.Color = color;
		label.Justify(Sides.Left, Sides.CenterY);
		label.DockInside(n, InnerDock.Center, 160f, 48f, 24f, 0f);
		if (icon.Valid())
		{
			FSprite sprite = new FSprite(icon);
			sprite.SetParent(n);
			sprite.Color = iconColor;
			sprite.DockOutside(label, OuterDock.Left, 48f, 48f, 0f, 0f);
		}
		n.Position = new Vector2(position.x + 200f, position.y + 0f);
		n.Alpha = 0f;
		n.SkewX = 40f;
		n.AddAction(new FEntityActionSequence(new FEntityAction[4]
		{
			new FEntityActionSpawn(new FEntityAction[3]
			{
				new FEntityActionTweenPropertyToFloat(0.1f, "Alpha", 1f, GoEaseType.Linear),
				new FEntityActionTweenPropertyToVector2(0.5f, "Position", position, GoEaseType.BackOut),
				new FEntityActionTweenPropertyToFloat(0.5f, "SkewX", 0f, GoEaseType.BackOut)
			}),
			new FEntityActionDelay(0.6f, false),
			new FEntityActionSpawn(new FEntityAction[2]
			{
				new FEntityActionTweenPropertyToFloat(0.3f, "Alpha", 0f, GoEaseType.Linear),
				new FEntityActionTweenPropertyToVector2(0.3f, "Position", new Vector2(position.x, position.y + -20f), GoEaseType.QuadIn)
			}),
			new FEntityActionCallMethod(delegate
			{
				n.Unparent();
			}, true)
		}));
	}

	protected void AddLetterboxBars()
	{
		mLetterboxTopBar.SetParent(this);
		mLetterboxBottomBar.SetParent(this);
		mLetterboxTopBar.MoveToBottom();
		mLetterboxBottomBar.MoveToBottom();
	}

	protected virtual void AnimateLetterboxBars(bool animIn, bool moveToBottom = true, float animTime = -1f)
	{
		float time = cLetterboxAnimTime;
		if (animTime >= 0f)
		{
			time = animTime;
		}
		if (!animIn)
		{
			float height = Extents.height;
			mLetterboxTopBar.StopAllActions(false);
			mLetterboxBottomBar.StopAllActions(false);
			float bottomY = 75f - height * 0.5f;
			float topY = height * 0.5f + -75f + 5f;
			if (mLetterboxTopBar.OnStage)
			{
				mLetterboxTopBar.y = topY;
				mLetterboxTopBar.AddAction(new FEntityActionSequence(new FEntityAction[2]
				{
					new FEntityActionTweenPropertyToFloat(time, "y", topY, topY + 150f, GoEaseType.QuadIn),
					new FEntityActionCallMethod(RemoveLetterboxBars, true)
				}));
				mLetterboxBottomBar.y = bottomY;
				mLetterboxBottomBar.AddAction(new FEntityActionTweenPropertyToFloat(time, "y", bottomY, bottomY + -150f, GoEaseType.QuadIn));
			}
			else
			{
				mLetterboxTopBar.y = topY + 150f;
				mLetterboxBottomBar.y = bottomY + -150f;
				RemoveLetterboxBars();
			}
			return;
		}
		if (mLetterboxTopBar.OnStage)
		{
			return;
		}
		mLetterboxTopBar.SetParent(this);
		mLetterboxBottomBar.SetParent(this);
		if (moveToBottom)
		{
			mLetterboxTopBar.MoveToBottom();
			mLetterboxBottomBar.MoveToBottom();
		}
		float h = Extents.height;
		float y = h * 0.5f + 75f + 5f;
		mLetterboxTopBar.y = y;
		mLetterboxTopBar.StopAllActions(false);
		mLetterboxTopBar.AddAction(new FEntityActionTweenPropertyToFloat(time, "y", y, y + -150f, GoEaseType.QuadOut));
		float by = -75f - h * 0.5f;
		mLetterboxBottomBar.y = by;
		mLetterboxBottomBar.StopAllActions(false);
		mLetterboxBottomBar.AddAction(new FEntityActionSequence(new FEntityAction[2]
		{
			new FEntityActionTweenPropertyToFloat(time, "y", by, by + 150f, GoEaseType.QuadOut),
			new FEntityActionCallMethod(LetterboxBarsTransitionedIn, true)
		}));
	}

	protected virtual void RemoveLetterboxBars()
	{
		mLetterboxTopBar.Unparent();
		mLetterboxBottomBar.Unparent();
	}

	public virtual void LetterboxBarsTransitionedIn()
	{

	}

	public virtual bool HandleBackKey()
	{
		return false;
	}

	// PC port: the collider PCInput focuses first on this page (null = top-left-most).
	public virtual FUICollider PCDefaultFocus => null;

	public virtual void PreloadImages()
	{

	}

	public virtual void UnloadImages()
	{

	}

	public virtual void Darken()
	{
		if (mDarkened)
		{
			return;
		}
		mDarkened = true;
		Color = new Color(0.5f, 0.5f, 0.5f, 1f);
	}

	public virtual void UnDarken()
	{
		if (mDarkened)
		{
			mDarkened = false;
			Color = new Color(1f, 1f, 1f, 1f);
		}
	}
}
