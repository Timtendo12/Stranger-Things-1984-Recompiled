using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageConversationDialog @ 0x1799d00..0x179cfd8, Ghidra vaddr).
// Not opened by any recovered game content (FHUD only pops it if present).
public class FPageConversationDialog : FUIPage
{
	public enum AndroidBackButtonType
	{
		None = 0,
		Button0 = 1,
		Button1 = 2
	}

	public enum ConversationPlacements
	{
		None = 0,
		Left = 1,
		Center = 2,
		Right = 3
	}

	protected ConversationPlacements mPlacement;

	protected FUICollider mScreenCollider;

	protected FNode mDialog;

	protected FNode mContainer;

	protected FSlicedSprite mBackground;

	protected FSprite mAvatar;

	protected FSprite mAvatarFrame;

	protected FLabel mTitleLabel;

	protected FLabel mBodyLabel;

	protected FUICollider mContinueCollider;

	protected FUIButton mButtonContinue;

	protected float mDuration;

	protected float mDurationTimer;

	protected bool mTransitioningIn;

	protected bool mPressed;

	[CompilerGenerated]
	private Action m_mTransitionOutAction;

	public AndroidBackButtonType AndroidBackButton { get; set; }

	protected event Action mTransitionOutAction
	{
		add
		{
			m_mTransitionOutAction = (Action)Delegate.Combine(m_mTransitionOutAction, value);
		}
		remove
		{
			m_mTransitionOutAction = (Action)Delegate.Remove(m_mTransitionOutAction, value);
		}
	}

	public FPageConversationDialog(string name)
		: base(name)
	{
		base.Modal = true;
		AndroidBackButton = AndroidBackButtonType.Button0;
		base.TransitionStyle = TransitionStyles.Top;
		mTransitioningIn = false;
		m_mTransitionOutAction = null;
		mPressed = false;
		mDurationTimer = 0f;
	}

	public void SetupData(bool modal, float duration, string avatarSprite, string titleText, Color titleColor, string bodyText, ConversationPlacements placement, Action continueAction)
	{
		base.Modal = modal;
		mDuration = duration;
		mPlacement = placement;
		if (modal)
		{
			mScreenCollider = new FUICollider(base.Name + ".ScreenCollider", false);
			mScreenCollider.AddSignalPress(HandleScreenCollider);
			mScreenCollider.SetParent(this);
		}
		mDialog = new FNode(base.Name + ".Dialog");
		mDialog.SetParent(this);
		mContainer = new FNode(base.Name + ".Container");
		mContainer.SetParent(mDialog);
		mBackground = new FSlicedSprite(base.Name + ".Background", modal ? "ConversationBackground.png" : "TextBubble01.png");
		mBackground.SetParent(mContainer);
		mContinueCollider = new FUICollider(base.Name + ".ContinueCollider", false);
		mContinueCollider.SetParent(mContainer);
		mContinueCollider.AddSignalRelease(HandleContinueColliderRelease, 40f);
		mTitleLabel = new FLabel(base.Name + ".Title", "DefaultStyle", titleText, 48f, true, true, false);
		mTitleLabel.SetParent(mContainer);
		mTitleLabel.ClipAndWrap(true);
		mTitleLabel.Color = titleColor;
		mBodyLabel = new FLabel(base.Name + ".Body", "DefaultStyle", bodyText, 48f, true, true, false);
		mBodyLabel.SetParent(mContainer);
		if (!modal)
		{
			mBodyLabel.Color = new Color(0f, 0f, 0f, 1f);
		}
		mBodyLabel.ClipAndWrap(true);
		mAvatar = new FSprite(base.Name + ".Avatar", avatarSprite);
		mAvatar.SetParent(mContainer);
		mAvatarFrame = new FSprite(base.Name + ".AvatarFrame", "ConversationAvatarFrame.png");
		mAvatarFrame.SetParent(mContainer);
		if (modal)
		{
			mButtonContinue = new FUIButton(base.Name + ".ContinueButton");
			mButtonContinue.SetParent(mContainer);
			mButtonContinue.SetBackground("ConversationNextButton.png");
			mButtonContinue.AddHandleRelease(HandleContinueButtonRelease);
		}
		m_mTransitionOutAction = continueAction;
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		mDialog.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		if (mScreenCollider != null)
		{
			mScreenCollider.DockInside(this, InnerDock.Center, Futile.Screen.Size);
		}
		mContinueCollider.DockInside(mContainer, InnerDock.Center, 800f, 265f, 0f, 0f);
		switch (mPlacement)
		{
		case ConversationPlacements.Right:
			mContainer.DockInside(mDialog, InnerDock.TopRight, 800f, 265f, -32f, -192f);
			mBackground.DockInside(mContainer, InnerDock.Left, 800f, 265f, 0f, 0f);
			mBackground.ScaleX = -1f;
			mAvatar.DockInside(mContainer, InnerDock.Right, 256f, 220f, -15f, 16f);
			mAvatarFrame.DockInside(mContainer, InnerDock.Right, 265f, 265f, -9f, 0f);
			mTitleLabel.DockInside(mContainer, InnerDock.TopRight, 436f, 48f, -285f, -28f);
			mBodyLabel.DockOutside(mTitleLabel, OuterDock.Bottom, 436f, 162f, 0f, 0f);
			if (mButtonContinue != null)
			{
				mButtonContinue.DockInside(mContainer, InnerDock.Left, 64f, 64f, 0f, 0f);
			}
			break;
		case ConversationPlacements.Center:
		case ConversationPlacements.Left:
			if (mPlacement == ConversationPlacements.Left)
			{
				mContainer.DockInside(mDialog, InnerDock.TopLeft, 800f, 265f, 32f, -192f);
			}
			else
			{
				// 0x179bc94..0x179bcb8: s2 = 48.0, s3 = 0 for the Center placement (Left uses 32, -192)
				mContainer.DockInside(mDialog, InnerDock.Top, 800f, 265f, 48f, 0f);
			}
			mBackground.DockInside(mContainer, InnerDock.Right, 800f, 265f, 0f, 0f);
			mBackground.ScaleX = 1f;
			mAvatar.DockInside(mContainer, InnerDock.Left, 256f, 220f, 15f, 16f);
			mAvatarFrame.DockInside(mContainer, InnerDock.Left, 265f, 265f, 9f, 0f);
			mTitleLabel.DockInside(mContainer, InnerDock.TopLeft, 436f, 48f, 285f, -28f);
			mBodyLabel.DockOutside(mTitleLabel, OuterDock.Bottom, 436f, 162f, 0f, 0f);
			if (mButtonContinue != null)
			{
				mButtonContinue.DockInside(mContainer, InnerDock.Right, 64f, 64f, 0f, 0f);
			}
			break;
		}
		if (mButtonContinue == null)
		{
			MarkDirtyRecursive();
			return;
		}
		FEntityAction[] actions = new FEntityAction[5]
		{
			new FEntityActionDelay(1f),
			new FEntityActionTweenPropertyToFloat(0.12f, "Scale", 1.25f, GoEaseType.ExpoOut),
			new FEntityActionTweenPropertyToFloat(0.08f, "Scale", 1f, GoEaseType.ExpoOut),
			new FEntityActionTweenPropertyToFloat(0.12f, "Scale", 1.2f, GoEaseType.ExpoOut),
			new FEntityActionTweenPropertyToFloat(0.08f, "Scale", 1f, GoEaseType.ExpoOut)
		};
		mButtonContinue.AddAction(new FEntityActionRepeat(-1, new FEntityActionSequence(actions)));
		MarkDirtyRecursive();
	}

	protected Vector2 GetTransitionPosition()
	{
		Vector2 position = mContainer.Position;
		switch (mPlacement)
		{
		case ConversationPlacements.Right:
			return new Vector2(position.x + (mDialog.Extents.width + mContainer.Extents.width) * 0.5f + 1f, position.y);
		case ConversationPlacements.Center:
			return new Vector2(position.x, position.y + (mDialog.Extents.height + mContainer.Extents.height) * 0.5f + 1f);
		case ConversationPlacements.Left:
			return new Vector2(position.x - (mDialog.Extents.width + mContainer.Extents.width) * 0.5f - 1f, position.y);
		default:
			return position;
		}
	}

	public override void TransitionIn()
	{
		Vector2 transitionPosition = GetTransitionPosition();
		Vector2 position = mContainer.Position;
		mContainer.Position = transitionPosition;
		mContainer.Alpha = 0f;
		mTransitioningIn = true;
		FEntityAction[] actions = new FEntityAction[2]
		{
			new FEntityActionTweenPropertyToFloat(0.35f, "Alpha", 1f, GoEaseType.Linear),
			new FEntityActionTweenPropertyToVector2(0.35f, "Position", position, GoEaseType.Linear)
		};
		mContainer.AddAction(new FEntityActionSequence(new FEntityActionSpawn(actions), new FEntityActionCallMethod(TransitionInDone)));
	}

	protected void TransitionInDone()
	{
		mTransitioningIn = false;
	}

	public override void TransitionOut()
	{
		Vector2 transitionPosition = GetTransitionPosition();
		mContainer.Alpha = 1f;
		FEntityActionSpawn child = new FEntityActionSpawn(new FEntityActionTweenPropertyToFloat(0.25f, "Alpha", 0f, GoEaseType.Linear), new FEntityActionTweenPropertyToVector2(0.25f, "Position", transitionPosition, GoEaseType.Linear));
		if (m_mTransitionOutAction == null)
		{
			mContainer.AddAction(new FEntityActionSequence(child, new FEntityActionCallMethod(delegate
			{
				FApp.HUD.RemovePage(this);
			})));
		}
		else
		{
			mContainer.AddAction(new FEntityActionSequence(child, new FEntityActionCallMethod(m_mTransitionOutAction), new FEntityActionCallMethod(delegate
			{
				FApp.HUD.RemovePage(this);
			})));
		}
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		TransitionIn();
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
		if (!base.Modal && mDurationTimer < mDuration)
		{
			mDurationTimer += Time.deltaTime;
			if (mDurationTimer >= mDuration)
			{
				HandleContinue();
			}
		}
	}

	protected void HandleContinueButtonRelease(FUIButton button, FTouch touch)
	{
		HandleContinue();
	}

	protected void HandleContinueColliderRelease(FUICollider button, FTouch touch)
	{
		HandleContinue();
	}

	protected void HandleContinue()
	{
		if (!mPressed && !mTransitioningIn)
		{
			mPressed = true;
			FApp.SoundManager.PlaySoundSet("UIButton");
			TransitionOut();
		}
	}

	public void HandleScreenCollider(FUICollider collider, FTouch touch)
	{
		if (mContainer != null && !mPressed && !mTransitioningIn)
		{
			Flash();
		}
	}

	protected void Flash()
	{
		if (mContainer != null && !mPressed && !mTransitioningIn)
		{
			mContainer.RemoveAllActions();
			mContainer.AdditiveTintColor = new Color(0f, 0f, 0f, 0f);
			FEntityAction[] actions = new FEntityAction[4]
			{
				new FEntityActionTweenPropertyToColor(0.1f, "AdditiveTintColor", new Color(0.2f, 0.2f, 0.2f, 0f), GoEaseType.Linear),
				new FEntityActionTweenPropertyToColor(0.07f, "AdditiveTintColor", new Color(0f, 0f, 0f, 0f), GoEaseType.Linear),
				new FEntityActionTweenPropertyToColor(0.1f, "AdditiveTintColor", new Color(0.2f, 0.2f, 0.2f, 0f), GoEaseType.Linear),
				new FEntityActionTweenPropertyToColor(0.07f, "AdditiveTintColor", new Color(0f, 0f, 0f, 0f), GoEaseType.Linear)
			};
			mContainer.AddAction(new FEntityActionSequence(actions));
		}
	}
}
