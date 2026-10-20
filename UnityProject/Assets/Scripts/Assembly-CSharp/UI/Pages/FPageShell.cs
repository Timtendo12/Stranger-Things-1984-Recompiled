using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageShell @ 0x17ad904..0x17ae304, Ghidra vaddr).
public class FPageShell : FUIPage
{
	public const float cPanelMargin = 16f;

	public static readonly Vector2 cStackSize = new Vector2(420f, 450f);

	public static readonly Vector2 cStackMargin = new Vector2(32f, 32f);

	public static readonly Vector2 cStackButtonSize = new Vector2(420f, 108f);

	public static readonly Vector2 cStackButtonMargin = new Vector2(8f, 16f);

	protected Vector2 mDialogSize = Vector2.zero;

	protected FUICollider mDialogCollider;

	protected FNode mDialog;

	protected FSprite mBackground;

	protected FPageShell mBackPage;

	protected FPageShell mNextPage;

	protected string mBackgroundImage = string.Empty;

	protected string mTitleText = string.Empty;

	protected string mTitleDetailsText = string.Empty;

	protected bool mDestroyWorldOnTransitionInComplete = true;

	protected Action mTransitionInComplete;

	protected Action mTransitionOutComplete;

	public FNode Dialog => mDialog;

	public FPageShell BackPage
	{
		get
		{
			return mBackPage;
		}
		set
		{
			mBackPage = value;
		}
	}

	public FPageShell NextPage
	{
		get
		{
			return mNextPage;
		}
		set
		{
			mNextPage = value;
		}
	}

	public virtual string TitleText => mTitleText;

	public string TitleDetailsText => mTitleDetailsText;

	public string PregameArea { get; set; }

	public bool PregameAreaValid { get; set; }

	public FPageShell(string name, string backgroundImage, bool useCollider)
		: base(name)
	{
		mBackgroundImage = backgroundImage;
		if (useCollider)
		{
			mDialogCollider = new FUICollider(base.Name + ".DialogCollider", false);
			mDialogCollider.SetParent(this);
		}
		mDialog = new FNode(base.Name + ".Dialog");
		mDialog.SetParent(this);
		if (!string.IsNullOrEmpty(mBackgroundImage))
		{
			mBackground = new FSprite(base.Name + ".BackgroundImage", mBackgroundImage);
			mBackground.SetParent(mDialog);
		}
		PregameAreaValid = false;
	}

	public override void LayoutPage()
	{
		FAssert.That(mDialog != null, "Layout called on null dialog.");
		base.LayoutPage();
		mDialogSize = Futile.Screen.Size;
		if (mDialog != null)
		{
			mDialog.DockInside(this, InnerDock.Center, mDialogSize);
		}
		if (mDialogCollider != null)
		{
			mDialogCollider.DockInside(mDialog, InnerDock.Center, mDialogSize);
		}
		if (mBackground != null)
		{
			mBackground.DockInside(mDialog, InnerDock.Center, mDialogSize);
		}
		MarkDirtyRecursive();
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		Futile.Instance.RemoveSignalUpdate(HandleUpdate);
		Futile.Instance.AddSignalUpdate(HandleUpdate);
		if (mDialog != null)
		{
			TransitionIn(mDialog);
		}
		if (PregameAreaValid)
		{
			if (PregameArea != null)
			{
				FPregameSceneManager.ActivateArea(PregameArea);
			}
			else
			{
				FPregameSceneManager.DeactivateAll();
			}
		}
	}

	public virtual void SetupSpindle()
	{
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		Futile.Instance.RemoveSignalUpdate(HandleUpdate);
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
	}

	public override void SelfResizeExtents()
	{
		base.SelfResizeExtents();
	}

	public virtual void DataCleared()
	{
	}

	protected override void OnCompleteTransitionIn(AbstractGoTween tween)
	{
		base.OnCompleteTransitionIn(tween);
		if (mTransitionInComplete != null)
		{
			mTransitionInComplete();
		}
	}

	protected override void OnCompleteTransitionOut(AbstractGoTween tween)
	{
		base.OnCompleteTransitionOut(tween);
		if (mTransitionOutComplete != null)
		{
			mTransitionOutComplete();
		}
	}

	public override void StopAllTransitions()
	{
		if (mDialog != null)
		{
			Go.killAllTweensWithTarget(mDialog);
		}
	}

	public override void TransitionIn()
	{
		TransitionIn(mDialog);
	}

	public void TransitionIn(Action transitionInAction)
	{
		if (transitionInAction != null)
		{
			mTransitionInComplete = (Action)Delegate.Remove(mTransitionInComplete, transitionInAction);
			mTransitionInComplete = (Action)Delegate.Combine(mTransitionInComplete, transitionInAction);
		}
		TransitionIn(mDialog);
	}

	public override void TransitionOut()
	{
		TransitionOut(mDialog);
	}

	public void TransitionOut(Action transitionOutAction)
	{
		if (transitionOutAction != null)
		{
			mTransitionOutComplete = (Action)Delegate.Remove(mTransitionOutComplete, transitionOutAction);
			mTransitionOutComplete = (Action)Delegate.Combine(mTransitionOutComplete, transitionOutAction);
		}
		TransitionOut(mDialog);
	}

	public virtual bool HasNewStuff()
	{
		// The shipped body returns false.
		return false;
	}
}
