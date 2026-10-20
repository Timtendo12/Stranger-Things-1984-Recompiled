using System;
using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FStage @ 0x016c512c..0x016c5ea4, Ghidra vaddr).
public class FStage : FNode
{
	public int NextNodeDepth;

	public int Index;

	protected bool mNeedsDepthUpdate;

	protected internal bool mNeedsTextureUpdate;

	protected FRenderer mRenderer;

	protected FMatrix mIdentityMatrix;

	protected bool mDoesRendererNeedTransformChange;

	protected FStageTransform mTransform;

	protected FMatrix mFollowMatrix = new FMatrix();

	protected FNode mFollowTarget;

	protected bool mShouldFollowScale;

	protected bool mShouldFollowRotation;

	protected bool mIsRedrawing;

	private List<FNode> mDeferredRedrawNodes = new List<FNode>();

	private int mFrameTillFullDepthUpdate = 60;

	public static bool DoFullDepthUpdate;

	public static bool HackDirtyStage;

	public static bool HackDepthDirty;

	public static bool HackOldMode;

	public static bool HackMouseHoverDisable;

	public static bool HackBuffStatusOptDisable;

	public static bool HackVisUnitOptDisable;

	public override FMatrix Matrix => mIdentityMatrix;

	public override FMatrix ConcatenatedMatrix => mIdentityMatrix;

	public override FMatrix InverseConcatenatedMatrix => mIdentityMatrix;

	public FMatrix ScreenMatrix => mMatrix;

	public override FMatrix ScreenConcatenatedMatrix => mConcatenatedMatrix;

	public override FMatrix ScreenInverseConcatenatedMatrix => mInverseConcatenatedMatrix;

	public FRenderer Renderer => mRenderer;

	public FStageTransform Transform => mTransform;

	public new float ScaleX
	{
		get
		{
			return mScaleX;
		}
		set
		{
			throw new NotSupportedException("Stage scale must be uniform! Use stage.scale instead");
		}
	}

	public new float ScaleY
	{
		get
		{
			return mScaleY;
		}
		set
		{
			throw new NotSupportedException("Stage scale must be uniform! Use stage.scale instead");
		}
	}

	public FStage(string name)
		: base()
	{
		Name = name;
		mStage = this;
		mRenderer = new FRenderer(this);
		mIdentityMatrix = new FMatrix();
		mIdentityMatrix.Identity();
		mInverseConcatenatedMatrix = new FMatrix();
		mScreenConcatenatedMatrix = new FMatrix();
		mScreenInverseConcatenatedMatrix = new FMatrix();
		SetExtentsToScreen();
	}

	public override void SelfResizeExtents()
	{
		SetExtentsToScreen();
		Rect extents = Extents;
		Position = new Vector2(extents.x + extents.width * 0.5f, extents.y + extents.height * 0.5f);
	}

	public void HandleAddedToFutile()
	{
		HandleAddedToStage();
		Futile.Screen.AddSignalResize(MarkDirtyRecursive);
	}

	public void HandleRemovedFromFutile()
	{
		mRenderer.Clear();
		HandleRemovedFromStage();
		Futile.Screen.RemoveSignalResize(MarkDirtyRecursive);
	}

	public void HandleFacetsChanged()
	{
		mNeedsDepthUpdate = true;
	}

	public void HandleUpdateTextures()
	{
		mNeedsTextureUpdate = true;
	}

	public void DeferNodeRedraw(FNode nodeToDeferRedraw)
	{
		if (nodeToDeferRedraw != null)
		{
			mDeferredRedrawNodes.Add(nodeToDeferRedraw);
		}
	}

	protected override void UpdateDepthMatrixAlpha(bool shouldForceDirty, bool shouldUpdateDepth)
	{
		if (shouldUpdateDepth)
		{
			mDepth = NextNodeDepth++;
		}
		if (mMatrixIsDirty || shouldForceDirty)
		{
			MatrixIsDirty = false;
			mMatrix.SetScaleThenRotate(mPosition.x, mPosition.y, mScaleX, mScaleY, mRotation * (0f - (float)Math.PI / 180f));
			mConcatenatedMatrix.CopyValues(mMatrix);
			mInverseConcatenatedMatrix.InvertAndCopyValues(mConcatenatedMatrix);
			mDoesRendererNeedTransformChange = true;
		}
		if (mAlphaIsDirty || shouldForceDirty)
		{
			mAlphaIsDirty = false;
			mConcatenatedAlpha = mAlpha;
			mConcatenatedColor = mColor;
		}
	}

	public override void Redraw(bool shouldForceDirty, bool shouldUpdateDepth)
	{
		mIsRedrawing = true;
		UpdateFollow();
		bool hackDirtyStage = HackDirtyStage;
		if (HackDepthDirty)
		{
			mNeedsDepthUpdate = true;
		}
		DoFullDepthUpdate = false;
		mFrameTillFullDepthUpdate--;
		if (mFrameTillFullDepthUpdate < 1)
		{
			mFrameTillFullDepthUpdate = 60;
			DoFullDepthUpdate = true;
		}
		if (mNeedsTextureUpdate)
		{
			mRenderer.UpdateLayerTextures();
			mNeedsTextureUpdate = false;
		}
		bool needsDepthUpdate = mNeedsDepthUpdate;
		bool hackOldMode = HackOldMode;
		mNeedsDepthUpdate = false;
		bool depthChanged = hackOldMode || needsDepthUpdate;
		if (!depthChanged)
		{
			shouldForceDirty = hackDirtyStage || shouldForceDirty;
		}
		else
		{
			NextNodeDepth = Index * 10000;
			mRenderer.StartRender();
			shouldUpdateDepth = true;
			shouldForceDirty = true;
		}
		bool wasAlphaDirty = mAlphaIsDirty;
		UpdateDepthMatrixAlpha(shouldForceDirty, shouldUpdateDepth);
		int count = mChildNodes.Count;
		for (int i = 0; i < count; i++)
		{
			if (mChildNodes[i].mVisible)
			{
				if (mChildNodes[i].mDirty)
				{
					mChildNodes[i].ResizeConstraintsInGraph();
				}
				mChildNodes[i].Redraw(shouldForceDirty || wasAlphaDirty, shouldUpdateDepth);
			}
		}
		int deferredCount = mDeferredRedrawNodes.Count;
		for (int j = 0; j < deferredCount; j++)
		{
			mDeferredRedrawNodes[j].DeferredRedraw(shouldForceDirty || wasAlphaDirty, shouldUpdateDepth);
		}
		mDeferredRedrawNodes.Clear();
		if (depthChanged)
		{
			mRenderer.EndRender();
			Futile.TouchManager.HandleDepthChange();
		}
		if (mDoesRendererNeedTransformChange)
		{
			mDoesRendererNeedTransformChange = false;
			mTransform.Position = new Vector3(mPosition.x, mPosition.y, 0f);
			mTransform.Rotation = Quaternion.AngleAxis(mRotation, Vector3.back);
			mTransform.LocalScale = new Vector3(mScaleX, mScaleX, mScaleX);
			mRenderer.UpdateLayerTransforms();
		}
		mIsRedrawing = false;
	}

	public void CenterOn(Vector2 globalPosition)
	{
		if (mFollowTarget != null)
		{
			mFollowTarget = null;
		}
		Vector2 local = GlobalToLocal(globalPosition);
		mFollowMatrix.SetScaleThenRotate(0f, 0f, mScaleX, mScaleY, mRotation * (0f - (float)Math.PI / 180f));
		Vector2 transformed = mFollowMatrix.GetNewTransformedVector(local);
		x = 0f - transformed.x;
		y = 0f - transformed.y;
	}

	public void Follow(FNode followTarget, bool shouldFollowScale, bool shouldFollowRotation)
	{
		mFollowTarget = followTarget;
		mShouldFollowScale = shouldFollowScale;
		mShouldFollowRotation = shouldFollowRotation;
	}

	protected void UpdateFollow()
	{
		if (mFollowTarget == null)
		{
			return;
		}
		if (mFollowTarget.Stage == null)
		{
			mFollowTarget = null;
			return;
		}
		if (mShouldFollowScale)
		{
			Scale = 1f / mFollowTarget.ConcatenatedMatrix.GetScaleX();
		}
		if (mShouldFollowRotation)
		{
			SetRotation(mFollowTarget.ConcatenatedMatrix.GetRotation() * 57.295776f);
		}
		mFollowMatrix.SetScaleThenRotate(0f, 0f, mScaleX, mScaleY, mRotation * (0f - (float)Math.PI / 180f));
		FMatrix targetMatrix = mFollowTarget.ConcatenatedMatrix;
		Vector2 transformed = mFollowMatrix.GetNewTransformedVector(new Vector2(targetMatrix.tx, mFollowTarget.ConcatenatedMatrix.ty));
		x = 0f - transformed.x;
		y = 0f - transformed.y;
	}

	public void Unfollow(FNode targetToUnfollow, bool shouldResetPosition)
	{
		if (targetToUnfollow == null || mFollowTarget == targetToUnfollow)
		{
			mFollowTarget = null;
			if (shouldResetPosition)
			{
				ResetPosition();
			}
		}
	}

	public void LateUpdate()
	{
		mRenderer.Update();
	}

	public void ResetPosition()
	{
		if (x != 0f || y != 0f || mScaleX != 1f || mScaleY != 1f || mRotation != 0f)
		{
			x = 0f;
			y = 0f;
			mScaleX = 1f;
			mScaleY = 1f;
			mRotation = 0f;
			MatrixIsDirty = true;
		}
	}
}
