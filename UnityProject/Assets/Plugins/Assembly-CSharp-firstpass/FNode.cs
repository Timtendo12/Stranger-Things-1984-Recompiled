using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FNode @ 0x016a3dd4..0x019eeb98, Ghidra vaddr).
// BonusXP's Futile FNode: merges FContainer (mChildNodes), adds extents/constraints (docking), entity actions,
// focus, root motion, render-layer tracking, recycle ids, Make3D and clipping.
public class FNode : IDisposable, IDelayedDispose, IEntity
{
	public enum Sides
	{
		Left = 0,
		Right = 1,
		Top = 2,
		Bottom = 3,
		CenterX = 4,
		CenterY = 5
	}

	public enum LayoutStyles
	{
		Landscape = 0,
		Portrait = 1,
		Horizontal = 2,
		Vertical = 3
	}

	public class RLTracking
	{
		public FNode mNode;

		public FRenderLayer mLayer;

		public int mStartingVert = -1;

		public int mNumTris = 1;

		public int mLayerGeneration;

		public int mNumVerts;

		public int mStartingIndex = -1;
	}

	private static Queue<FNode> MarkDirtyQueue = new Queue<FNode>();

	private static Stack<FNode> Ancestors = new Stack<FNode>();

	protected Vector2 mPosition;

	protected float mScaleX;

	protected float mScaleY;

	protected float mRotation;

	protected float mSkewX;

	protected float mSkewY;

	protected float mRotationSin;

	protected float mRotationCos;

	protected float mSkewXTan;

	protected float mSkewYTan;

	protected float mSortZ;

	protected FNode mParent;

	protected bool mMatrixIsDirty;

	protected FMatrix mMatrix;

	protected FMatrix mConcatenatedMatrix;

	protected FMatrix mInverseConcatenatedMatrix;

	protected FMatrix mScreenConcatenatedMatrix;

	protected FMatrix mScreenInverseConcatenatedMatrix;

	protected bool mNeedSpecialMatrices;

	protected float mAlpha;

	protected float mConcatenatedAlpha;

	protected bool mAlphaIsDirty;

	protected Color mColor;

	protected Color mConcatenatedColor;

	protected bool mOnStage;

	protected int mDepth;

	protected FStage mStage;

	protected internal bool mVisible = true;

	public Rect _mExtentsDoNotCallDirectly;

	protected List<FNode> mChildNodes = new List<FNode>();

	protected int mOldChildNodesHash;

	protected bool mShouldSortByZ;

	protected bool mUseChildExtents;

	public object mData;

	public Color mAdditiveTintColor;

	protected FEntityActionList mEntityActionList;

	protected bool mSnapPosition = true;

	protected bool mHasFocus;

	protected bool mWantsFocus;

	protected internal bool mDirty = true;

	protected bool mUseConcatenatedColor = true;

	protected bool mUseConcatenatedAlpha = true;

	public bool mDeferRedraw;

	private FConstraint mLeftConstraint;

	private FConstraint mRightConstraint;

	private FConstraint mTopConstraint;

	private FConstraint mBottomConstraint;

	protected List<FNode> mConstraintChildren = new List<FNode>();

	private bool InteralExtentsMode;

	private bool mChangedToInvis;

	private bool CachedClippingInfo;

	private Vector2 CachedMinClip;

	private Vector2 CachedMaxClip;

	private bool mParentClippingCashed;

	public float HackFinalRotation;

	public float HackFinalY;

	private bool mDebugMe;

	public bool DebugMe2;

	private bool mUserPosition;

	public const int cRecycleTop = -2;

	public int ManualRecycleID = -1;

	public int AutoRecycleID;

	private bool RecycleAddressDirty = true;

	public List<int> RecycleAddress = new List<int>();

	public int RecycleCounter;

	public F3DTracker RecycleRef;

	public bool mMaking3D;

	protected FRenderLayer mLastRenderLayer;

	public bool mRenderLayerDirty;

	public bool UseRootMotion;

	protected bool RootPositionDirty;

	protected bool RootInitialPosSet;

	protected Vector2 RootInitialPos;

	public FNode mMotionParentRef;

	protected Vector2 ParentRootInitialPos;

	public bool NoRenderLayerSharing;

	private static int mMake3DRenderOrder;

	public int mRenderOrder3D;

	public List<RLTracking> mRenderLayerTracking = new List<RLTracking>();

	public static bool RootMotionEnabled;

	protected Vector3 mLastRootMotionPosition = Vector3.zero;

	public int mBadTextureFrame = -1;

	public int mInValidationCounter;

	private bool mClippingDepthUpdatedLastFrame;

	private bool mExtentsCalculatedLastFrame;

	private bool mClippingSteadyLastFrame;

	private int childGuid;

	private FNode[] mScratchNode4 = new FNode[4];

	private const float cEpsilonSqr = 1E-10f;

	public bool HasParent => mParent != null;

	public List<FNode> ConstraintChildren => mConstraintChildren;

	public Rect Extents
	{
		get
		{
			if (InteralExtentsMode)
			{
				return InternalExtents;
			}
			if (mDirty)
			{
				ResizeConstraintsInGraph();
			}
			return _mExtentsDoNotCallDirectly;
		}
	}

	private Rect InternalExtents
	{
		get
		{
			if (mDirty)
			{
				InternalUpdateConstraints();
			}
			return _mExtentsDoNotCallDirectly;
		}
	}

	public virtual float Width
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public virtual float Height
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public bool UseChildExtents
	{
		get
		{
			return mUseChildExtents;
		}
		set
		{
			mUseChildExtents = value;
		}
	}

	public string DebugName => Name;

	public string Name { get; set; }

	public bool Visible
	{
		get
		{
			return mVisible;
		}
		set
		{
			if (mVisible != value)
			{
				mVisible = value;
				mAlphaIsDirty = true;
				if (!value)
				{
					mChangedToInvis = true;
				}
			}
		}
	}

	public Vector2 Position
	{
		get
		{
			InternalUpdateConstraints();
			return mPosition;
		}
		set
		{
			if (UseRootMotion && !RootInitialPosSet)
			{
				RootInitialPos = value;
				RootInitialPosSet = true;
				if (mMotionParentRef != null)
				{
					mMotionParentRef.InternalUpdateConstraints();
					ParentRootInitialPos = mMotionParentRef.mPosition;
				}
			}
			if (!InteralExtentsMode)
			{
				mUserPosition = true;
			}
			float dx = mPosition.x - value.x;
			float dy = mPosition.y - value.y;
			if (dx * dx + dy * dy >= cEpsilonSqr)
			{
				mPosition = value;
				if (!UseRootMotion)
				{
					mMatrixIsDirty = true;
				}
				else
				{
					RootPositionDirty = true;
				}
			}
		}
	}

	public float x
	{
		get
		{
			InternalUpdateConstraints();
			return mPosition.x;
		}
		set
		{
			if (!InteralExtentsMode)
			{
				mUserPosition = true;
			}
			if (mPosition.x != value)
			{
				mPosition.x = value;
				mMatrixIsDirty = true;
			}
		}
	}

	public float y
	{
		get
		{
			InternalUpdateConstraints();
			return mPosition.y;
		}
		set
		{
			if (!InteralExtentsMode)
			{
				mUserPosition = true;
			}
			if (mPosition.y != value)
			{
				mPosition.y = value;
				mMatrixIsDirty = true;
			}
		}
	}

	public virtual float sortZ
	{
		get
		{
			return mSortZ;
		}
		set
		{
			mSortZ = value;
		}
	}

	public float ScaleX
	{
		get
		{
			return mScaleX;
		}
		set
		{
			if (mScaleX != value)
			{
				mScaleX = value;
				mMatrixIsDirty = true;
			}
		}
	}

	public float ScaleY
	{
		get
		{
			return mScaleY;
		}
		set
		{
			if (mScaleY != value)
			{
				mScaleY = value;
				mMatrixIsDirty = true;
			}
		}
	}

	public float Scale
	{
		get
		{
			return mScaleX;
		}
		set
		{
			if (mScaleX != value || mScaleY != value)
			{
				mScaleX = value;
				mScaleY = value;
				mMatrixIsDirty = true;
			}
		}
	}

	public float Rotation
	{
		get
		{
			return mRotation;
		}
		set
		{
			SetRotation(value);
		}
	}

	public float SkewX
	{
		get
		{
			return mSkewX;
		}
		set
		{
			if (mSkewX != value)
			{
				mSkewX = value;
				mMatrixIsDirty = true;
				mSkewXTan = (float)Math.Tan(value * ((float)Math.PI / 180f));
			}
		}
	}

	public float SkewY
	{
		get
		{
			return mSkewY;
		}
		set
		{
			if (mSkewY != value)
			{
				mSkewY = value;
				mMatrixIsDirty = true;
				mSkewYTan = (float)Math.Tan(value * ((float)Math.PI / 180f));
			}
		}
	}

	public bool MatrixIsDirty
	{
		get
		{
			return mMatrixIsDirty;
		}
		protected set
		{
			mMatrixIsDirty = value;
		}
	}

	public bool AlphaIsDirty
	{
		get
		{
			return mAlphaIsDirty;
		}
		protected set
		{
			mAlphaIsDirty = value;
		}
	}

	public FNode Parent => mParent;

	public int Depth => mDepth;

	public bool OnStage => mOnStage;

	public virtual int TouchPriority
	{
		get
		{
			return mDepth;
		}
		set
		{
		}
	}

	public virtual FMatrix Matrix => mMatrix;

	public virtual FMatrix ConcatenatedMatrix => mConcatenatedMatrix;

	public virtual FMatrix InverseConcatenatedMatrix
	{
		get
		{
			if (!mNeedSpecialMatrices)
			{
				CreateSpecialMatrices();
			}
			return mInverseConcatenatedMatrix;
		}
	}

	public virtual FMatrix ScreenConcatenatedMatrix
	{
		get
		{
			if (!mNeedSpecialMatrices)
			{
				CreateSpecialMatrices();
			}
			return mScreenConcatenatedMatrix;
		}
	}

	public virtual FMatrix ScreenInverseConcatenatedMatrix
	{
		get
		{
			if (!mNeedSpecialMatrices)
			{
				CreateSpecialMatrices();
			}
			return mScreenInverseConcatenatedMatrix;
		}
	}

	public float Alpha
	{
		get
		{
			return mAlpha;
		}
		set
		{
			float clamped = Math.Max(0f, Math.Min(1f, value));
			if (mAlpha != clamped)
			{
				mAlpha = clamped;
				mAlphaIsDirty = true;
			}
		}
	}

	public float ConcatenatedAlpha => mConcatenatedAlpha;

	public virtual Color Color
	{
		get
		{
			return mColor;
		}
		set
		{
			float dr = mColor.r - value.r;
			float dg = mColor.g - value.g;
			float db = mColor.b - value.b;
			float da = mColor.a - value.a;
			if (da * da + db * db + dr * dr + dg * dg >= cEpsilonSqr)
			{
				mColor = value;
				mAlphaIsDirty = true;
			}
		}
	}

	public Color ConcatenatedColor => mConcatenatedColor;

	public FStage Stage
	{
		get
		{
			return mStage;
		}
		set
		{
			mStage = value;
		}
	}

	public bool ShouldSortByZ => mShouldSortByZ;

	public virtual Color AdditiveTintColor
	{
		get
		{
			return mAdditiveTintColor;
		}
		set
		{
			float dr = mAdditiveTintColor.r - value.r;
			float dg = mAdditiveTintColor.g - value.g;
			float db = mAdditiveTintColor.b - value.b;
			float da = mAdditiveTintColor.a - value.a;
			if (da * da + db * db + dr * dr + dg * dg >= cEpsilonSqr)
			{
				mAdditiveTintColor = value;
				mAlphaIsDirty = true;
			}
		}
	}

	public virtual Color ConcatenatedAdditiveTintColor
	{
		get
		{
			Color color = AdditiveTintColor;
			if (mParent != null)
			{
				color += mParent.ConcatenatedAdditiveTintColor;
			}
			return color;
		}
	}

	public FEntityActionList EntityActionList => mEntityActionList;

	public bool ClipToExtents { get; set; }

	public bool HasFocus
	{
		get
		{
			return mHasFocus;
		}
		set
		{
			mHasFocus = value;
		}
	}

	public bool WantsFocus
	{
		get
		{
			return mWantsFocus;
		}
		set
		{
			mWantsFocus = value;
		}
	}

	public bool SnapPosition => mSnapPosition;

	public bool Dirty => mDirty;

	public bool DebugMe
	{
		get
		{
			return mDebugMe;
		}
		set
		{
			mDebugMe = value;
		}
	}

	private bool UserPosition
	{
		get
		{
			return mUserPosition;
		}
		set
		{
			mUserPosition = value;
		}
	}

	public bool SepiaUnder { get; set; }

	public FNode()
		: this(string.Empty)
	{
	}

	public FNode(string name)
	{
		Name = name;
		mEntityActionList = new FEntityActionList(this);
		mDepth = 0;
		mPosition = Vector2.zero;
		ScaleX = 1f;
		ScaleY = 1f;
		SetRotation(0f);
		SkewX = 0f;
		SkewY = 0f;
		mSortZ = 0f;
		mAlphaIsDirty = false;
		mColor = new Color(1f, 1f, 1f, 1f);
		mConcatenatedColor = new Color(1f, 1f, 1f, 1f);
		mAlpha = 1f;
		mConcatenatedAlpha = 1f;
		mAdditiveTintColor = FUtil.TransparentBlack;
		mMatrix = new FMatrix();
		mConcatenatedMatrix = new FMatrix();
		mMatrixIsDirty = false;
		mHasFocus = false;
	}

	public void SetVisibleRecursive(bool value)
	{
		if (mVisible != value)
		{
			mVisible = value;
			mAlphaIsDirty = true;
		}
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			mChildNodes[i].SetVisibleRecursive(value);
		}
	}

	public void SetRotation(float value)
	{
		float rotation = value;
		if (rotation > 360f)
		{
			rotation -= 360f;
		}
		if (rotation < -360f)
		{
			rotation += 360f;
		}
		if (mRotation != rotation)
		{
			mMatrixIsDirty = true;
		}
		mRotation = rotation;
		double radians = rotation * (0f - (float)Math.PI / 180f);
		mRotationSin = (float)Math.Sin(radians);
		mRotationCos = (float)Math.Cos(radians);
	}

	public void SetShouldSortByZ(bool value)
	{
		if (mShouldSortByZ == value)
		{
			return;
		}
		mShouldSortByZ = value;
		if (!value)
		{
			if (mOnStage)
			{
				Futile.Instance.RemoveSignalUpdate(HandleUpdate);
			}
		}
		else if (mOnStage)
		{
			Futile.Instance.AddSignalUpdate(HandleUpdate);
		}
	}

	public void SetSnapPosition(bool value)
	{
		if (mSnapPosition != value)
		{
			mSnapPosition = value;
			mMatrixIsDirty = true;
		}
		if (value)
		{
			return;
		}
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			mChildNodes[i].SetSnapPosition(false);
		}
	}

	protected void CreateSpecialMatrices()
	{
		mNeedSpecialMatrices = true;
		mInverseConcatenatedMatrix = new FMatrix();
		mScreenConcatenatedMatrix = new FMatrix();
		mScreenInverseConcatenatedMatrix = new FMatrix();
		mInverseConcatenatedMatrix.InvertAndCopyValues(mConcatenatedMatrix);
		if (mStage != null)
		{
			mScreenConcatenatedMatrix.ConcatAndCopyValues(mConcatenatedMatrix, mStage.ScreenConcatenatedMatrix);
		}
		else
		{
			mScreenConcatenatedMatrix.CopyValues(mConcatenatedMatrix);
		}
		mScreenInverseConcatenatedMatrix.InvertAndCopyValues(mScreenConcatenatedMatrix);
	}

	public void ClearUserPosition()
	{
		Position = Vector2.zero;
		mUserPosition = false;
	}

	public Vector2 ScreenToLocal(Vector2 screenVector)
	{
		FScreen screen = Futile.Screen;
		float inverseScale = 1f / Futile.DisplayScale;
		return ScreenInverseConcatenatedMatrix.GetNewTransformedVector(new Vector2((screenVector.x - screen.OriginX * screen.PixelWidth) * inverseScale, (screenVector.y - screen.OriginY * screen.PixelHeight) * inverseScale));
	}

	public Vector2 LocalToStage(Vector2 localVector)
	{
		return mConcatenatedMatrix.GetNewTransformedVector(localVector);
	}

	public Vector2 StageToLocal(Vector2 globalVector)
	{
		return InverseConcatenatedMatrix.GetNewTransformedVector(globalVector);
	}

	public Vector2 LocalToGlobal(Vector2 localVector)
	{
		return ScreenConcatenatedMatrix.GetNewTransformedVector(localVector);
	}

	public Vector2 LocalToScreen(Vector2 localVector)
	{
		return Futile.GlobalPosToScreen(LocalToGlobal(localVector));
	}

	public Vector2 GlobalToLocal(Vector2 globalVector)
	{
		return ScreenInverseConcatenatedMatrix.GetNewTransformedVector(globalVector);
	}

	public Vector2 LocalToLocal(FNode otherNode, Vector2 otherVector)
	{
		return GlobalToLocal(otherNode.LocalToGlobal(otherVector));
	}

	public void DebugMeConditional()
	{
	}

	public void UpdateMatrix()
	{
		if (!mMatrixIsDirty)
		{
			return;
		}
		mMatrixIsDirty = false;
		mMatrix.SetScaleSkewRotateT(mPosition.x, mPosition.y, mScaleX, mScaleY, mSkewXTan, mSkewYTan, mRotationSin, mRotationCos);
		if (mParent != null)
		{
			mConcatenatedMatrix.ConcatAndCopyValues(mMatrix, mParent.ConcatenatedMatrix);
		}
		else
		{
			mConcatenatedMatrix.CopyValues(mMatrix);
		}
		if (mNeedSpecialMatrices)
		{
			mInverseConcatenatedMatrix.InvertAndCopyValues(mConcatenatedMatrix);
			if (mStage != null)
			{
				mScreenConcatenatedMatrix.ConcatAndCopyValues(mConcatenatedMatrix, mStage.ScreenConcatenatedMatrix);
			}
			mScreenInverseConcatenatedMatrix.InvertAndCopyValues(mScreenConcatenatedMatrix);
		}
		UpdateLocalMatrix();
	}

	protected virtual void UpdateDepthMatrixAlpha(bool shouldForceDirty, bool shouldUpdateDepth)
	{
		if (shouldUpdateDepth)
		{
			mDepth = (mStage != null) ? mStage.NextNodeDepth++ : 0;
		}
		if (mMatrixIsDirty || shouldForceDirty)
		{
			mMatrixIsDirty = false;
			mMatrix.SetScaleSkewRotateT(mPosition.x, mPosition.y, mScaleX, mScaleY, mSkewXTan, mSkewYTan, mRotationSin, mRotationCos);
			if (mParent != null)
			{
				mConcatenatedMatrix.ConcatAndCopyValues(mMatrix, mParent.ConcatenatedMatrix);
			}
			else
			{
				mConcatenatedMatrix.CopyValues(mMatrix);
			}
			UpdateLocalMatrix();
		}
		// Shipped code refreshes the special matrices on every call (outside the dirty block).
		if (mNeedSpecialMatrices)
		{
			mInverseConcatenatedMatrix.InvertAndCopyValues(mConcatenatedMatrix);
			if (mStage != null)
			{
				mScreenConcatenatedMatrix.ConcatAndCopyValues(mConcatenatedMatrix, mStage.ScreenConcatenatedMatrix);
			}
			mScreenInverseConcatenatedMatrix.InvertAndCopyValues(mScreenConcatenatedMatrix);
		}
		if (mAlphaIsDirty || shouldForceDirty)
		{
			mAlphaIsDirty = false;
			if (mUseConcatenatedAlpha && mParent != null)
			{
				mConcatenatedAlpha = mParent.mConcatenatedAlpha * mAlpha;
			}
			else
			{
				mConcatenatedAlpha = mAlpha;
			}
			if (mUseConcatenatedColor && mParent != null)
			{
				mConcatenatedColor = mParent.mConcatenatedColor * mColor;
			}
			else
			{
				mConcatenatedColor = mColor;
			}
		}
	}

	protected virtual void RecursiveUpdateDepthMatrixAlphaAndForceDirty(bool shouldForceDirty, bool shouldUpdateDepth)
	{
		UpdateDepthMatrixAlpha(shouldForceDirty, shouldUpdateDepth);
		mMatrixIsDirty = true;
		mAlphaIsDirty = true;
		int count = mChildNodes.Count;
		for (int i = 0; i < count; i++)
		{
			mChildNodes[i].RecursiveUpdateDepthMatrixAlphaAndForceDirty(shouldForceDirty, shouldUpdateDepth);
		}
	}

	protected virtual void UpdateLocalMatrix()
	{
	}

	public virtual bool CanCull()
	{
		int count = mChildNodes.Count;
		for (int i = 0; i < count; i++)
		{
			if (!mChildNodes[i].CanCull())
			{
				return false;
			}
		}
		return true;
	}

	public void CalculateExtentsInScreenSpace(out Vector2 min, out Vector2 max, bool applyOffset = true)
	{
		Rect extents = Extents;
		float halfWidth = extents.width * 0.5f;
		float halfHeight = extents.height * 0.5f;
		min = new Vector2(x - halfWidth, y - halfHeight);
		max = new Vector2(halfWidth + x, halfHeight + y);
		if (mParent != null)
		{
			min = mParent.LocalToGlobal(min);
			max = mParent.LocalToGlobal(max);
		}
		if (applyOffset)
		{
			Vector2 offset = new Vector2(Futile.Screen.Width, Futile.Screen.Height) * 0.5f;
			min += offset;
			max += offset;
		}
		if (!mUseChildExtents)
		{
			return;
		}
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			mChildNodes[i].CalculateExtentsInScreenSpace(out Vector2 childMin, out Vector2 childMax, applyOffset);
			min = Vector2.Min(min, childMin);
			max = Vector2.Max(max, childMax);
		}
	}

	public virtual void DeferredRedraw(bool shouldForceDirty, bool shouldUpdateDepth)
	{
		RedrawInternal(shouldForceDirty, shouldUpdateDepth);
	}

	public virtual void Redraw(bool shouldForceDirty, bool shouldUpdateDepth)
	{
		if (!mDeferRedraw)
		{
			RedrawInternal(shouldForceDirty, shouldUpdateDepth);
		}
		else
		{
			mStage.DeferNodeRedraw(this);
		}
	}

	private void ClearTrackingRecursive()
	{
		ClearRenderLayerTracking();
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			FNode child = mChildNodes[i];
			if (child != null)
			{
				child.ClearTrackingRecursive();
			}
		}
	}

	private void RedrawInternal(bool shouldForceDirty, bool shouldUpdateDepth)
	{
		if (!mVisible)
		{
			if (mChangedToInvis)
			{
				mChangedToInvis = false;
				ClearTrackingRecursive();
			}
			return;
		}
		int count = mChildNodes.Count;
		for (int i = 0; i < count; i++)
		{
			if (mChildNodes[i].mDirty)
			{
				mChildNodes[i].ResizeConstraintsInGraph();
			}
		}
		bool hasClip = Futile.GetTopClippingRect(out Rect clip);
		bool wasMatrixDirty = mMatrixIsDirty;
		bool wasAlphaDirty = mAlphaIsDirty;
		RedrawPrep(shouldForceDirty, shouldUpdateDepth, wasMatrixDirty, wasAlphaDirty);
		bool parentClippingCached = mParentClippingCashed;
		bool cachedClippingInfo = CachedClippingInfo;
		bool oldMode = FStage.HackOldMode;
		bool childRootDirty = false;
		if (wasMatrixDirty || shouldForceDirty || RootPositionDirty)
		{
			CachedClippingInfo = false;
			mParentClippingCashed = false;
			if ((wasMatrixDirty || shouldForceDirty) && wasMatrixDirty && !oldMode)
			{
				UpdateMatrix();
			}
			else if (UseRootMotion && RootPositionDirty)
			{
				childRootDirty = true;
				mMatrixIsDirty = true;
				UpdateMatrix();
			}
			mExtentsCalculatedLastFrame = false;
		}
		float minX = 0f;
		float minY = 0f;
		float maxX = 0f;
		float maxY = 0f;
		if (ClipToExtents || hasClip)
		{
			Vector2 min;
			Vector2 max;
			bool calculated;
			bool reuse = cachedClippingInfo && mExtentsCalculatedLastFrame;
			if (!oldMode && (CachedClippingInfo || reuse || mClippingSteadyLastFrame))
			{
				min = CachedMinClip;
				max = CachedMaxClip;
				calculated = false;
				mClippingSteadyLastFrame = false;
				CachedClippingInfo = true;
			}
			else
			{
				if (UseRootMotion && RootPositionDirty)
				{
					mMatrixIsDirty = true;
					UpdateMatrix();
				}
				calculated = true;
				CalculateExtentsInScreenSpace(out min, out max, true);
				if ((min - CachedMinClip).sqrMagnitude < cEpsilonSqr && (max - CachedMaxClip).sqrMagnitude < cEpsilonSqr)
				{
					mClippingSteadyLastFrame = true;
				}
				CachedClippingInfo = true;
				CachedMinClip = min;
				CachedMaxClip = max;
			}
			mExtentsCalculatedLastFrame = calculated;
			minX = min.x;
			minY = min.y;
			maxX = max.x;
			maxY = max.y;
		}
		bool visible = !hasClip || (clip.width >= FMath.Epsilon && clip.height >= FMath.Epsilon && minX <= clip.x + clip.width && clip.x <= maxX && minY <= clip.y + clip.height && clip.y <= maxY);
		if (visible)
		{
			if (!ClipToExtents)
			{
				Futile.StartRedraw(this);
			}
			else
			{
				Futile.StartRedraw(this, minX, minY, maxX, maxY);
			}
			UpdateDepthMatrixAlpha(shouldForceDirty, shouldUpdateDepth);
			RedrawInternal(shouldForceDirty, shouldUpdateDepth, wasMatrixDirty, wasAlphaDirty);
			for (int j = 0; j < count; j++)
			{
				FNode child = mChildNodes[j];
				if (childRootDirty)
				{
					child.RootPositionDirty = true;
				}
				child.Redraw(wasMatrixDirty || shouldForceDirty || wasAlphaDirty, shouldUpdateDepth);
			}
			RootPositionDirty = false;
			Futile.StopRedraw(ClipToExtents);
		}
		else if (!oldMode && (parentClippingCached || mClippingDepthUpdatedLastFrame))
		{
			mClippingDepthUpdatedLastFrame = false;
		}
		else
		{
			RecursiveUpdateDepthMatrixAlphaAndForceDirty(shouldForceDirty, shouldUpdateDepth);
			mParentClippingCashed = true;
			mClippingDepthUpdatedLastFrame = true;
		}
	}

	public bool HasSameRecycleSignature(FNode other)
	{
		FNode a = this;
		FNode b = other;
		while (true)
		{
			if (a.AutoRecycleID != b.AutoRecycleID && (a.ManualRecycleID == -1 || a.ManualRecycleID != b.ManualRecycleID))
			{
				return false;
			}
			a = a.mParent;
			b = b.mParent;
			if (a == null || b == null)
			{
				return a == null && b == null;
			}
		}
	}

	public bool HasSameRecycleSignature(List<int> recycleAddress)
	{
		UpdateRecycleAddress();
		if (RecycleAddress.Count != recycleAddress.Count)
		{
			return false;
		}
		for (int i = 0; i < RecycleAddress.Count; i++)
		{
			if (RecycleAddress[i] != recycleAddress[i])
			{
				return false;
			}
		}
		return true;
	}

	public void SetManualRecycleID(int id)
	{
		ManualRecycleID = id;
		RecycleAddressDirty = true;
		UpdateRecycleAddress();
	}

	public void UpdateRecycleAddress()
	{
		if (!RecycleAddressDirty)
		{
			return;
		}
		RecycleAddressDirty = false;
		RecycleAddress.Clear();
		for (FNode node = this; node != null; node = node.mParent)
		{
			if (node.ManualRecycleID == -2)
			{
				RecycleAddress.Add(1000);
				return;
			}
			// Shipped code tests this node's ManualRecycleID (not the walked node's) to pick the id kind.
			RecycleAddress.Add((ManualRecycleID == -1) ? node.AutoRecycleID : node.ManualRecycleID);
		}
	}

	public GameObject Make3D(bool shouldForceDirty, bool shouldUpdateDepth, F3DTracker topMost = null, GameObject existingGO = null, bool checkForNullStage = false)
	{
		mMaking3D = true;
		if (mStage == null && checkForNullStage)
		{
			return null;
		}
		UpdateDepthMatrixAlpha(true, true);
		if (topMost == null)
		{
			mMake3DRenderOrder = 0;
		}
		mRenderOrder3D = ++mMake3DRenderOrder;
		GameObject go = existingGO;
		if (go == null)
		{
			go = new GameObject("Make3D" + Name + (mVisible ? "" : "_Invis"));
		}
		F3DTracker tracker = go.GetOrCreateComponent<F3DTracker>();
		tracker.FutileDepth = mRenderOrder3D;
		F3DTracker top = (topMost == null) ? tracker : topMost;
		tracker.TopTracker = top;
		top.RegisterDepth(mRenderOrder3D);
		top.RegisterTracker(tracker);
		UpdateRecycleAddress();
		tracker.SetRecycleAddress(RecycleAddress);
		if (this is FLabel label)
		{
			top.RegisterLabel(label);
		}
		int count = mChildNodes.Count;
		for (int i = 0; i < count; i++)
		{
			if (mChildNodes[i].mDirty)
			{
				mChildNodes[i].ResizeConstraintsInGraph();
			}
		}
		bool hasClip = Futile.GetTopClippingRect(out Rect clip);
		bool wasMatrixDirty = mMatrixIsDirty;
		bool wasAlphaDirty = mAlphaIsDirty;
		RedrawPrep(shouldForceDirty, shouldUpdateDepth, wasMatrixDirty, wasAlphaDirty);
		Vector2 min = Vector2.zero;
		Vector2 max = Vector2.zero;
		if (hasClip || ClipToExtents)
		{
			CalculateExtentsInScreenSpace(out min, out max, true);
		}
		bool visible = !hasClip || (clip.width >= FMath.Epsilon && clip.height >= FMath.Epsilon && min.x <= clip.x + clip.width && clip.x <= max.x && min.y <= clip.y + clip.height && clip.y <= max.y);
		if (visible)
		{
			if (!ClipToExtents)
			{
				Futile.StartRedraw(this);
			}
			else
			{
				Futile.StartRedraw(this, min.x, min.y, max.x, max.y);
			}
			RecycleRef = tracker;
			Make3DInternal(tracker);
			for (int j = 0; j < count; j++)
			{
				FNode child = mChildNodes[j];
				child.UpdateRecycleAddress();
				F3DTracker existing = top.FindMatchingExistingTracker(child);
				GameObject existingChildGO = null;
				if (existing != null)
				{
					existingChildGO = existing.gameObject;
					top.Recycling(existing);
				}
				GameObject childGO = child.Make3D(wasAlphaDirty || wasMatrixDirty || shouldForceDirty, shouldUpdateDepth, top, existingChildGO);
				if (childGO == null)
				{
					return null;
				}
				childGO.transform.parent = go.transform;
				childGO.transform.position = Vector3.zero;
				childGO.transform.localScale = Vector3.one;
			}
			Futile.StopRedraw(ClipToExtents);
			if (!mVisible)
			{
				go.SetActive(false);
			}
			if (go.transform.childCount == 0)
			{
				UnityEngine.Object.Destroy(go);
			}
		}
		else
		{
			RecursiveUpdateDepthMatrixAlphaAndForceDirty(shouldForceDirty, shouldUpdateDepth);
		}
		mMaking3D = false;
		return go;
	}

	protected virtual GameObject Make3DInternal(F3DTracker tracker)
	{
		return null;
	}

	public virtual void RedrawPrep(bool shouldForceDirty, bool shouldUpdateDepth, bool wasMatrixDirty, bool wasAlphaDirty)
	{
	}

	public virtual void RedrawInternal(bool shouldForceDirty, bool shouldUpdateDepth, bool wasMatrixDirty, bool wasAlphaDirty)
	{
	}

	public virtual void PopulateRenderLayer()
	{
	}

	public virtual void HandleAddedToStage()
	{
		if (mOnStage)
		{
			return;
		}
		mOnStage = true;
		if (mWantsFocus)
		{
			Futile.Instance.AddFocusNode(this);
		}
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			FNode child = mChildNodes[i];
			child.mStage = mStage;
			child.HandleAddedToStage();
		}
		if (mShouldSortByZ && !mMaking3D)
		{
			Futile.Instance.AddSignalUpdate(HandleUpdate);
		}
		if (mEntityActionList.Count > 0 && !mMaking3D)
		{
			Futile.Instance.AddSignalUpdate(UpdateActions);
		}
		RecursiveUpdateDepthMatrixAlphaAndForceDirty(true, true);
	}

	public virtual void HandleRemovedFromStage()
	{
		if (mOnStage)
		{
			mOnStage = false;
			int count = mChildNodes.Count;
			for (int i = 0; i < count; i++)
			{
				FNode child = mChildNodes[i];
				child.HandleRemovedFromStage();
				child.mStage = null;
			}
		}
		if (!(Futile.Instance != null))
		{
			return;
		}
		if (mWantsFocus)
		{
			Futile.Instance.RemoveFocusNode(this);
		}
		Futile.Instance.RemoveSignalUpdate(HandleUpdate);
		Futile.Instance.RemoveSignalUpdate(UpdateActions);
	}

	protected virtual void HandleUpdate()
	{
		if (SortByZ() && mStage != null)
		{
			mStage.HandleFacetsChanged();
		}
	}

	public virtual void HandleAddedToParent(FNode parent)
	{
		if (mParent == parent)
		{
			return;
		}
		if (mParent != null)
		{
			mParent.RemoveChild(this);
		}
		mParent = parent;
		for (FNode node = mParent; node != null; node = node.mParent)
		{
			if (!node.mSnapPosition)
			{
				SetSnapPosition(false);
				break;
			}
		}
	}

	public bool AnyParentHasSnapOff()
	{
		FNode node = mParent;
		while (node != null && node.mSnapPosition)
		{
			node = node.mParent;
		}
		return node != null;
	}

	public virtual void HandleRemovedFromParent()
	{
		mParent = null;
	}

	public virtual bool CanGainFocus()
	{
		return false;
	}

	public virtual void HandleGainFocus()
	{
		mHasFocus = true;
		Futile.Instance.AddSignalKeyInput(HandleKeyInput);
	}

	public virtual void HandleLoseFocus()
	{
		mHasFocus = false;
		Futile.Instance.RemoveSignalKeyInput(HandleKeyInput);
	}

	public virtual void HandleKeyInput(string input)
	{
	}

	public void RemoveFromStage()
	{
		if (mStage != null)
		{
			mStage.RemoveChild(this);
		}
	}

	public void Unparent()
	{
		if (mParent != null)
		{
			mParent.RemoveChild(this);
		}
	}

	public void Unparent(AbstractGoTween tween)
	{
		if (mParent != null)
		{
			mParent.RemoveChild(this);
		}
	}

	public void MoveToTop()
	{
		if (mParent != null)
		{
			SetParent(mParent);
		}
	}

	public void MoveToBottom()
	{
		if (mParent != null)
		{
			mParent.AddChildAtIndex(this, 0);
		}
	}

	public void SetParent(FNode node)
	{
		FAssert.That(node != this, "You should not try to set a node as a parent of itself.");
		if (node != null && node != this)
		{
			node.AddChild(this);
		}
	}

	public virtual void AddChild(FNode node)
	{
		if (node.mParent != null && node.mParent != this)
		{
			FLog.UnityError("trying to give node '" + node.Name + "' a duplicate parent '" + Name + "', removing old one: ");
			if (node.mParent != null)
			{
				node.mParent.RemoveChild(node);
			}
			return;
		}
		int index = mChildNodes.IndexOf(node);
		if (index == -1)
		{
			node.HandleAddedToParent(this);
			mChildNodes.Add(node);
			if (mOnStage)
			{
				node.mStage = mStage;
				node.HandleAddedToStage();
			}
		}
		else if (index != mChildNodes.Count - 1)
		{
			mChildNodes.RemoveAt(index);
			mChildNodes.Add(node);
			if (mOnStage)
			{
				mStage.HandleFacetsChanged();
			}
		}
		node.AutoRecycleID = ++childGuid;
	}

	public bool AddChildBefore(FNode node, FNode referenceNode)
	{
		int index = mChildNodes.IndexOf(referenceNode);
		if (index >= 0)
		{
			AddChildAtIndex(node, index);
		}
		return index >= 0;
	}

	public void AddChildAtIndex(FNode node, int newIndex)
	{
		if (node.mParent != null && node.mParent != this)
		{
			FLog.UnityError("trying to give node '" + node.Name + "' a duplicate parent '" + Name + "', removing old one: ");
			if (node.mParent != null)
			{
				node.mParent.RemoveChild(node);
			}
			return;
		}
		int oldIndex = mChildNodes.IndexOf(node);
		int index = Math.Min(newIndex, mChildNodes.Count);
		if (oldIndex == index)
		{
			return;
		}
		if (oldIndex == -1)
		{
			node.HandleAddedToParent(this);
			mChildNodes.Insert(index, node);
			if (mOnStage)
			{
				node.mStage = mStage;
				node.HandleAddedToStage();
			}
		}
		else
		{
			mChildNodes.RemoveAt(oldIndex);
			mChildNodes.Insert((oldIndex < index) ? (index - 1) : index, node);
			if (mOnStage)
			{
				mStage.HandleFacetsChanged();
			}
		}
		if (node.AutoRecycleID == -1)
		{
			node.AutoRecycleID = index;
		}
	}

	public virtual void RemoveChild(FNode node)
	{
		if (node.mParent != null && node.mParent != this)
		{
			// Shipped code prints this node's parent name (this.mParent), not node.mParent.
			FLog.UnityError("attempting to remove node '" + node.Name + "' from wrong parent '" + mParent?.Name + "'");
			return;
		}
		node.HandleRemovedFromParent();
		if (node.mStage != null)
		{
			node.mStage.mChildNodes.Remove(node);
		}
		if (mOnStage)
		{
			node.HandleRemovedFromStage();
			node.mStage = null;
			node.ClearTrackingRecursive();
		}
		mChildNodes.Remove(node);
	}

	public void RemoveChildren()
	{
		while (mChildNodes.Count > 0)
		{
			RemoveChild(mChildNodes[0]);
		}
	}

	public int GetChildCount()
	{
		return mChildNodes.Count;
	}

	public FNode GetChildAt(int childIndex)
	{
		return mChildNodes[childIndex];
	}

	protected static int ZComparison(FNode a, FNode b)
	{
		float delta = a.sortZ - b.sortZ;
		if (delta < 0f)
		{
			return -1;
		}
		return (delta > 0f) ? 1 : 0;
	}

	protected bool SortByZ()
	{
		mChildNodes.InsertionSort(ZComparison);
		int hash = 269;
		int count = mChildNodes.Count;
		for (int i = 0; i < count; i++)
		{
			hash = mChildNodes[i].GetHashCode() + hash * 17;
		}
		int oldHash = mOldChildNodesHash;
		mOldChildNodesHash = hash;
		return hash != oldHash;
	}

	public virtual void Invalidate()
	{
	}

	protected virtual bool HasConstraintError()
	{
		if (mParent != null && !HasConstraints())
		{
			return true;
		}
		return false;
	}

	public virtual void MarkDirty()
	{
		mDirty = true;
	}

	public virtual void MarkDirtyRecursive()
	{
		MarkDirtyQueue.Clear();
		MarkDirtyQueue.Enqueue(this);
		while (MarkDirtyQueue.Count > 0)
		{
			FNode node = MarkDirtyQueue.Dequeue();
			if (node.HasConstraintChildren())
			{
				List<FNode> children = node.mConstraintChildren;
				for (int i = 0; i < children.Count; i++)
				{
					MarkDirtyQueue.Enqueue(children[i]);
				}
			}
			node.mDirty = true;
		}
	}

	public void ClearPosition()
	{
		mUserPosition = false;
		MarkDirty();
	}

	public virtual void ResizeConstraintsInGraph()
	{
		InternalUpdateConstraints();
	}

	protected void SafeUpdateConstraints()
	{
		InternalUpdateConstraints();
	}

	private void InternalUpdateConstraints()
	{
		if (!mDirty)
		{
			return;
		}
		Vector2 position = mPosition;
		mDirty = false;
		if (mLeftConstraint != null && mLeftConstraint.Constraint != null && mLeftConstraint.Constraint.mDirty)
		{
			mLeftConstraint.Constraint.InternalUpdateConstraints();
		}
		if (mRightConstraint != null && mRightConstraint.Constraint != null && mRightConstraint.Constraint.mDirty)
		{
			mRightConstraint.Constraint.InternalUpdateConstraints();
		}
		if (mTopConstraint != null && mTopConstraint.Constraint != null && mTopConstraint.Constraint.mDirty)
		{
			mTopConstraint.Constraint.InternalUpdateConstraints();
		}
		if (mBottomConstraint != null && mBottomConstraint.Constraint != null && mBottomConstraint.Constraint.mDirty)
		{
			mBottomConstraint.Constraint.InternalUpdateConstraints();
		}
		InteralExtentsMode = true;
		SelfResizeExtents();
		InteralExtentsMode = false;
		if (mUserPosition)
		{
			Position = position;
		}
	}

	public virtual void SelfResizeExtents()
	{
		if (HasConstraints() && CalculateExtents())
		{
			bool internalMode = InteralExtentsMode;
			Rect extents = Extents;
			InteralExtentsMode = internalMode;
			Position = new Vector2(extents.x + extents.width * 0.5f, extents.y + extents.height * 0.5f);
		}
	}

	private static bool CheckNextNodeInConstraintGraphForCircularDependency(FNode node)
	{
		if (Ancestors.Contains(node))
		{
			Ancestors.Push(node);
			return true;
		}
		FNode[] next = node.mScratchNode4;
		int count = 0;
		if (node.mLeftConstraint != null && node.mLeftConstraint.Constraint != null && !FastExists(next, node.mLeftConstraint.Constraint))
		{
			next[count++] = node.mLeftConstraint.Constraint;
		}
		if (node.mRightConstraint != null && node.mRightConstraint.Constraint != null && !FastExists(next, node.mRightConstraint.Constraint))
		{
			next[count++] = node.mRightConstraint.Constraint;
		}
		if (node.mTopConstraint != null && node.mTopConstraint.Constraint != null && !FastExists(next, node.mTopConstraint.Constraint))
		{
			next[count++] = node.mTopConstraint.Constraint;
		}
		if (node.mBottomConstraint != null && node.mBottomConstraint.Constraint != null && !FastExists(next, node.mBottomConstraint.Constraint))
		{
			next[count++] = node.mBottomConstraint.Constraint;
		}
		if (count == 0)
		{
			return false;
		}
		for (int i = 0; i < count; i++)
		{
			Ancestors.Push(node);
			if (CheckNextNodeInConstraintGraphForCircularDependency(next[i]))
			{
				return true;
			}
			Ancestors.Pop();
		}
		return false;
	}

	private static bool FastExists(FNode[] array, FNode node)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == node)
			{
				return true;
			}
		}
		return false;
	}

	public static bool DoesConstraintGraphHaveACircularDependency(FNode originalNodeBeingConstrained)
	{
		Ancestors.Clear();
		bool circular = CheckNextNodeInConstraintGraphForCircularDependency(originalNodeBeingConstrained);
		string message = string.Empty;
		if (circular)
		{
			FNode[] chain = Ancestors.ToArray();
			for (int i = 0; i < chain.Length; i++)
			{
				if (i != 0)
				{
					message += " <- ";
				}
				message += chain[i].Name;
			}
		}
		FAssert.That(!circular, message);
		return circular;
	}

	private void AddConstraintInternal(Constraint side, FNode refNode, Sides refSide, float offset = 0f)
	{
		if (refNode == null)
		{
			FAssert.That(false, "Adding constraint to null node.");
			return;
		}
		Invalidate();
		FConstraint constraint;
		switch (side)
		{
		case Constraint.Left:
			if (mLeftConstraint == null)
			{
				mLeftConstraint = MakeConstraint(Sides.Left, refNode, refSide, offset);
				refNode.AddConstraintChild(this);
				return;
			}
			constraint = mLeftConstraint;
			break;
		case Constraint.Right:
			if (mRightConstraint == null)
			{
				mRightConstraint = MakeConstraint(Sides.Right, refNode, refSide, offset);
				refNode.AddConstraintChild(this);
				return;
			}
			constraint = mRightConstraint;
			break;
		case Constraint.Top:
			if (mTopConstraint == null)
			{
				mTopConstraint = MakeConstraint(Sides.Top, refNode, refSide, offset);
				refNode.AddConstraintChild(this);
				return;
			}
			constraint = mTopConstraint;
			break;
		case Constraint.Bottom:
			if (mBottomConstraint == null)
			{
				mBottomConstraint = MakeConstraint(Sides.Bottom, refNode, refSide, offset);
				refNode.AddConstraintChild(this);
				return;
			}
			constraint = mBottomConstraint;
			break;
		default:
			return;
		}
		FNode oldRef = constraint.Constraint;
		constraint.Constraint = refNode;
		constraint.ConstraintSide = refSide;
		constraint.ConstraintOffset = offset;
		if (oldRef != refNode)
		{
			refNode.AddConstraintChild(this);
			if (!HasConstraint(oldRef))
			{
				oldRef.mConstraintChildren.Remove(this);
			}
		}
	}

	// The shipped code inlines the FConstraint constructor (plain field stores); properties are set explicitly here.
	private static FConstraint MakeConstraint(Sides side, FNode refNode, Sides refSide, float offset)
	{
		FConstraint constraint = new FConstraint(side, refNode, refSide, offset);
		constraint.Side = side;
		constraint.Constraint = refNode;
		constraint.ConstraintSide = refSide;
		constraint.ConstraintOffset = offset;
		return constraint;
	}

	public void ExpandToFill(FNode nodeToFill, Vector2 additionalSize)
	{
		ExpandToFill(nodeToFill, additionalSize.x, additionalSize.y);
	}

	public void ExpandToFill(FNode nodeToFill, float additionalWidth = 0f, float additionalHeight = 0f)
	{
		// Shipped code uses additionalWidth for all four sides (additionalHeight is ignored).
		AddConstraintInternal(Constraint.Left, nodeToFill, Sides.Left, 0f - additionalWidth);
		AddConstraintInternal(Constraint.Right, nodeToFill, Sides.Right, additionalWidth);
		AddConstraintInternal(Constraint.Top, nodeToFill, Sides.Top, additionalWidth);
		AddConstraintInternal(Constraint.Bottom, nodeToFill, Sides.Bottom, 0f - additionalWidth);
		DoesConstraintGraphHaveACircularDependency(this);
		MarkDirtyRecursive();
	}

	public void DockInside(FNode refNode, InnerDock type, Vector2 size, Vector2 offset)
	{
		DockInside(refNode, type, size.x, size.y, offset.x, offset.y);
	}

	public void DockInside(FNode refNode, InnerDock type, Vector2 size, float xOffset = 0f, float yOffset = 0f)
	{
		DockInside(refNode, type, size.x, size.y, xOffset, yOffset);
	}

	public void DockInside(FNode refNode, InnerDock type, float width, float height, Vector2 offset)
	{
		DockInside(refNode, type, width, height, offset.x, offset.y);
	}

	public void DockInside(FNode refNode, InnerDock type, float width, float height, float xOffset = 0f, float yOffset = 0f)
	{
		FAssert.That(refNode != null, "DockInside\t() - null refNode!");
		switch (type)
		{
		case InnerDock.TopLeft:
		case InnerDock.Left:
		case InnerDock.BottomLeft:
			AddConstraintInternal(Constraint.Left, refNode, Sides.Left, xOffset);
			AddConstraintInternal(Constraint.Right, refNode, Sides.Left, width + xOffset);
			break;
		case InnerDock.Top:
		case InnerDock.Center:
		case InnerDock.Bottom:
			AddConstraintInternal(Constraint.Left, refNode, Sides.CenterX, xOffset - width * 0.5f);
			AddConstraintInternal(Constraint.Right, refNode, Sides.CenterX, width * 0.5f + xOffset);
			break;
		case InnerDock.TopRight:
		case InnerDock.Right:
		case InnerDock.BottomRight:
			AddConstraintInternal(Constraint.Left, refNode, Sides.Right, xOffset - width);
			AddConstraintInternal(Constraint.Right, refNode, Sides.Right, xOffset);
			break;
		default:
			DoesConstraintGraphHaveACircularDependency(this);
			MarkDirtyRecursive();
			return;
		}
		switch (type)
		{
		case InnerDock.TopLeft:
		case InnerDock.Top:
		case InnerDock.TopRight:
			AddConstraintInternal(Constraint.Bottom, refNode, Sides.Top, yOffset - height);
			AddConstraintInternal(Constraint.Top, refNode, Sides.Top, yOffset);
			break;
		case InnerDock.Left:
		case InnerDock.Center:
		case InnerDock.Right:
			AddConstraintInternal(Constraint.Bottom, refNode, Sides.CenterY, yOffset - height * 0.5f);
			AddConstraintInternal(Constraint.Top, refNode, Sides.CenterY, height * 0.5f + yOffset);
			break;
		default:
			AddConstraintInternal(Constraint.Bottom, refNode, Sides.Bottom, yOffset);
			AddConstraintInternal(Constraint.Top, refNode, Sides.Bottom, height + yOffset);
			break;
		}
		DoesConstraintGraphHaveACircularDependency(this);
		MarkDirtyRecursive();
	}

	public void DockInsideVerticalExpandHorizontal(FNode refNode, InnerDock type, float height, float leftMargin, float rightMargin, float yOffset)
	{
		FAssert.That(refNode != null, "DockInside\t() - null refNode!");
		switch (type)
		{
		case InnerDock.TopLeft:
		case InnerDock.Top:
		case InnerDock.TopRight:
			AddConstraintInternal(Constraint.Bottom, refNode, Sides.Top, yOffset - height);
			AddConstraintInternal(Constraint.Top, refNode, Sides.Top, yOffset);
			break;
		case InnerDock.Left:
		case InnerDock.Center:
		case InnerDock.Right:
			AddConstraintInternal(Constraint.Bottom, refNode, Sides.CenterY, yOffset - height * 0.5f);
			AddConstraintInternal(Constraint.Top, refNode, Sides.CenterY, height * 0.5f + yOffset);
			break;
		case InnerDock.BottomLeft:
		case InnerDock.Bottom:
		case InnerDock.BottomRight:
			AddConstraintInternal(Constraint.Bottom, refNode, Sides.Bottom, yOffset);
			AddConstraintInternal(Constraint.Top, refNode, Sides.Bottom, height + yOffset);
			break;
		}
		AddConstraintInternal(Constraint.Left, refNode, Sides.Left, leftMargin);
		AddConstraintInternal(Constraint.Right, refNode, Sides.Right, 0f - rightMargin);
	}

	public void DockOutside(FNode refNode, OuterDock type, Vector2 size, Vector2 offset)
	{
		DockOutside(refNode, type, size.x, size.y, offset.x, offset.y);
	}

	public void DockOutside(FNode refNode, OuterDock type, Vector2 size, float xOffset = 0f, float yOffset = 0f)
	{
		DockOutside(refNode, type, size.x, size.y, xOffset, yOffset);
	}

	public void DockOutside(FNode refNode, OuterDock type, float width, float height, Vector2 offset)
	{
		DockOutside(refNode, type, width, height, offset.x, offset.y);
	}

	public void DockOutside(FNode refNode, OuterDock type, float width, float height, float xOffset = 0f, float yOffset = 0f)
	{
		FAssert.That(refNode != null, "DockOutside() - null refNode!");
		// Horizontal placement.
		switch (type)
		{
		case OuterDock.TopLeft:
		case OuterDock.Left:
		case OuterDock.LeftAlignTop:
		case OuterDock.LeftAlignBottom:
		case OuterDock.BottomLeft:
			AddConstraintInternal(Constraint.Left, refNode, Sides.Left, xOffset - width);
			AddConstraintInternal(Constraint.Right, refNode, Sides.Left, xOffset);
			break;
		case OuterDock.Top:
		case OuterDock.Bottom:
			AddConstraintInternal(Constraint.Left, refNode, Sides.CenterX, xOffset - width * 0.5f);
			AddConstraintInternal(Constraint.Right, refNode, Sides.CenterX, width * 0.5f + xOffset);
			break;
		case OuterDock.TopAlignLeft:
		case OuterDock.BottomAlignLeft:
			AddConstraintInternal(Constraint.Left, refNode, Sides.Left, xOffset);
			AddConstraintInternal(Constraint.Right, refNode, Sides.Left, width + xOffset);
			break;
		case OuterDock.TopAlignRight:
		case OuterDock.BottomAlignRight:
			AddConstraintInternal(Constraint.Left, refNode, Sides.Right, xOffset - width);
			AddConstraintInternal(Constraint.Right, refNode, Sides.Right, xOffset);
			break;
		case OuterDock.TopRight:
		case OuterDock.Right:
		case OuterDock.RightAlignTop:
		case OuterDock.RightAlignBottom:
		case OuterDock.BottomRight:
			AddConstraintInternal(Constraint.Left, refNode, Sides.Right, xOffset);
			AddConstraintInternal(Constraint.Right, refNode, Sides.Right, width + xOffset);
			break;
		default:
			DoesConstraintGraphHaveACircularDependency(this);
			MarkDirtyRecursive();
			return;
		}
		// Vertical placement.
		switch (type)
		{
		case OuterDock.TopLeft:
		case OuterDock.Top:
		case OuterDock.TopAlignLeft:
		case OuterDock.TopAlignRight:
		case OuterDock.TopRight:
			AddConstraintInternal(Constraint.Bottom, refNode, Sides.Top, yOffset);
			AddConstraintInternal(Constraint.Top, refNode, Sides.Top, height + yOffset);
			break;
		case OuterDock.Left:
		case OuterDock.Right:
			AddConstraintInternal(Constraint.Bottom, refNode, Sides.CenterY, yOffset - height * 0.5f);
			AddConstraintInternal(Constraint.Top, refNode, Sides.CenterY, height * 0.5f + yOffset);
			break;
		case OuterDock.LeftAlignTop:
		case OuterDock.RightAlignTop:
			AddConstraintInternal(Constraint.Bottom, refNode, Sides.Top, yOffset - height);
			AddConstraintInternal(Constraint.Top, refNode, Sides.Top, yOffset);
			break;
		case OuterDock.LeftAlignBottom:
		case OuterDock.RightAlignBottom:
			AddConstraintInternal(Constraint.Bottom, refNode, Sides.Bottom, yOffset);
			AddConstraintInternal(Constraint.Top, refNode, Sides.Bottom, height + yOffset);
			break;
		default:
			AddConstraintInternal(Constraint.Bottom, refNode, Sides.Bottom, yOffset - height);
			AddConstraintInternal(Constraint.Top, refNode, Sides.Bottom, yOffset);
			break;
		}
		DoesConstraintGraphHaveACircularDependency(this);
		MarkDirtyRecursive();
	}

	public Sides GetSideFromHorizSide(HorizSide hs)
	{
		switch (hs)
		{
		case HorizSide.Left:
			return Sides.Left;
		case HorizSide.Right:
			return Sides.Right;
		case HorizSide.CenterX:
			return Sides.CenterX;
		default:
			FAssert.Fail("Invalid switch statement argument!");
			return Sides.CenterX;
		}
	}

	public Sides GetSideFromVertSide(VertSide vs)
	{
		switch (vs)
		{
		case VertSide.Bottom:
			return Sides.Bottom;
		case VertSide.Top:
			return Sides.Top;
		case VertSide.CenterY:
			return Sides.CenterY;
		default:
			FAssert.Fail("Invalid switch statement argument!");
			return Sides.CenterY;
		}
	}

	public void DockCustom(FNode leftRefNode, HorizSide leftRefSide, FNode rightRefNode, HorizSide rightRefSide, FNode topRefNode, VertSide topRefSide, FNode bottomRefNode, VertSide bottomRefSide)
	{
		DockCustom(leftRefNode, leftRefSide, 0f, rightRefNode, rightRefSide, 0f, topRefNode, topRefSide, 0f, bottomRefNode, bottomRefSide, 0f);
	}

	public void DockCustom(FNode leftRefNode, HorizSide leftRefSide, float leftRefOffset, FNode rightRefNode, HorizSide rightRefSide, float rightRefOffset, FNode topRefNode, VertSide topRefSide, float topRefOffset, FNode bottomRefNode, VertSide bottomRefSide, float bottomRefOffset)
	{
		FAssert.That(leftRefNode != null, "DockCustom() - Null leftRefNode!");
		FAssert.That(rightRefNode != null, "DockCustom() - Null rightRefNode!");
		FAssert.That(topRefNode != null, "DockCustom() - Null topRefNode!");
		FAssert.That(bottomRefNode != null, "DockCustom() - Null bottomRefNode!");
		Sides leftSide = GetSideFromHorizSide(leftRefSide);
		Sides rightSide = GetSideFromHorizSide(rightRefSide);
		Sides topSide = GetSideFromVertSide(topRefSide);
		Sides bottomSide = GetSideFromVertSide(bottomRefSide);
		AddConstraintInternal(Constraint.Left, leftRefNode, leftSide, leftRefOffset);
		AddConstraintInternal(Constraint.Right, rightRefNode, rightSide, rightRefOffset);
		AddConstraintInternal(Constraint.Top, topRefNode, topSide, topRefOffset);
		AddConstraintInternal(Constraint.Bottom, bottomRefNode, bottomSide, bottomRefOffset);
		DoesConstraintGraphHaveACircularDependency(this);
		MarkDirtyRecursive();
	}

	public bool HasConstraints()
	{
		return mLeftConstraint != null && mLeftConstraint.Constraint != null && mRightConstraint != null && mRightConstraint.Constraint != null && mTopConstraint != null && mTopConstraint.Constraint != null && mBottomConstraint != null && mBottomConstraint.Constraint != null;
	}

	public bool HasConstraint(FNode node)
	{
		return (mLeftConstraint != null && mLeftConstraint.Constraint == node) || (mRightConstraint != null && mRightConstraint.Constraint == node) || (mTopConstraint != null && mTopConstraint.Constraint == node) || (mBottomConstraint != null && mBottomConstraint.Constraint == node);
	}

	public bool HasConstraintChildren()
	{
		return mConstraintChildren.Count > 0;
	}

	public void AddConstraintChild(FNode child)
	{
		if (!mConstraintChildren.Contains(child))
		{
			mConstraintChildren.Add(child);
		}
	}

	public void RemoveConstraints()
	{
		if (mLeftConstraint != null && mLeftConstraint.Constraint != null)
		{
			mLeftConstraint.Constraint.mConstraintChildren.Remove(this);
		}
		if (mRightConstraint != null && mRightConstraint.Constraint != null)
		{
			mRightConstraint.Constraint.mConstraintChildren.Remove(this);
		}
		if (mTopConstraint != null && mTopConstraint.Constraint != null)
		{
			mTopConstraint.Constraint.mConstraintChildren.Remove(this);
		}
		if (mBottomConstraint != null && mBottomConstraint.Constraint != null)
		{
			mBottomConstraint.Constraint.mConstraintChildren.Remove(this);
		}
		mLeftConstraint = null;
		mRightConstraint = null;
		mTopConstraint = null;
		mBottomConstraint = null;
	}

	public float GetExtent(Sides side)
	{
		Rect extents = Extents;
		switch (side)
		{
		case Sides.Left:
			return extents.x;
		case Sides.Right:
			return extents.width + extents.x;
		case Sides.Top:
			return extents.height + extents.y;
		case Sides.Bottom:
			return extents.y;
		case Sides.CenterX:
			return extents.x + extents.width * 0.5f;
		case Sides.CenterY:
			return extents.y + extents.height * 0.5f;
		default:
			return 0f;
		}
	}

	private float GetConstraintEdge(FConstraint constraint, bool horizontal)
	{
		FNode refNode = constraint.Constraint;
		bool savedMode = refNode.InteralExtentsMode;
		if (InteralExtentsMode)
		{
			refNode.InteralExtentsMode = true;
		}
		float edge = refNode.GetExtent(constraint.ConstraintSide);
		if (constraint.Constraint == mParent)
		{
			FNode parent = constraint.Constraint;
			parent.InternalUpdateConstraints();
			float parentPos = horizontal ? parent.mPosition.x : parent.mPosition.y;
			edge -= parentPos;
			if (!(parent is FUICollider) || !(this is FUITooltip))
			{
				Rect parentExtents = parent.Extents;
				float center = horizontal ? (parentExtents.x + parentExtents.width * 0.5f) : (parentExtents.y + parentExtents.height * 0.5f);
				edge += parentPos - center;
			}
		}
		constraint.Constraint.InteralExtentsMode = savedMode;
		return edge + constraint.ConstraintOffset;
	}

	public virtual bool CalculateExtents()
	{
		if (mLeftConstraint == null || mLeftConstraint.Constraint == null || mLeftConstraint.Side != Sides.Left)
		{
			return false;
		}
		if (mRightConstraint == null || mRightConstraint.Constraint == null || mRightConstraint.Side != Sides.Right)
		{
			return false;
		}
		if (mTopConstraint == null || mTopConstraint.Constraint == null || mTopConstraint.Side != Sides.Top)
		{
			return false;
		}
		if (mBottomConstraint == null || mBottomConstraint.Constraint == null || mBottomConstraint.Side != Sides.Bottom)
		{
			return false;
		}
		float left = GetConstraintEdge(mLeftConstraint, true);
		float right = GetConstraintEdge(mRightConstraint, true);
		float top = GetConstraintEdge(mTopConstraint, false);
		float bottom = GetConstraintEdge(mBottomConstraint, false);
		_mExtentsDoNotCallDirectly.x = left;
		_mExtentsDoNotCallDirectly.y = bottom;
		_mExtentsDoNotCallDirectly.width = right - left;
		_mExtentsDoNotCallDirectly.height = top - bottom;
		return true;
	}

	public void SetExtentsToScreen()
	{
		FScreen screen = Futile.Screen;
		_mExtentsDoNotCallDirectly.width = screen.Width;
		_mExtentsDoNotCallDirectly.height = screen.Height;
		_mExtentsDoNotCallDirectly.x = 0f - screen.HalfWidth;
		_mExtentsDoNotCallDirectly.y = 0f - screen.HalfHeight;
	}

	public void SetExtents(float left, float top, float width, float height)
	{
		_mExtentsDoNotCallDirectly.x = left;
		_mExtentsDoNotCallDirectly.y = top;
		_mExtentsDoNotCallDirectly.width = width;
		_mExtentsDoNotCallDirectly.height = height;
	}

	~FNode()
	{
		Dispose(false);
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	public virtual void Dispose(bool disposing)
	{
		List<FNode> children = new List<FNode>(mChildNodes);
		foreach (FNode child in children)
		{
			if (child != null)
			{
				child.Dispose(disposing);
			}
		}
		FDelayedDestroyQueue.RequestDelayedDispose(this);
	}

	public virtual void DelayedDispose()
	{
		if (mParent != null)
		{
			mParent.RemoveChild(this);
		}
		RemoveConstraints();
		if (mStage != null)
		{
			mStage.RemoveChild(this);
			mStage = null;
		}
		Go.killAllAbstractTweensWithTarget(this);
	}

	public virtual float PlayAnim(string name, bool recursive)
	{
		float result = 0f;
		if (!recursive)
		{
			return 0f;
		}
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			float length = mChildNodes[i].PlayAnim(name, true);
			if (length > result)
			{
				result = length;
			}
		}
		return result;
	}

	public virtual float GetAnimLength(string name, bool recursive)
	{
		float result = 0f;
		if (!recursive)
		{
			return 0f;
		}
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			float length = mChildNodes[i].GetAnimLength(name, true);
			if (length > result)
			{
				result = length;
			}
		}
		return result;
	}

	public virtual void SetAnimSpeed(float speed, bool recursive)
	{
		if (!recursive)
		{
			return;
		}
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			mChildNodes[i].SetAnimSpeed(speed, true);
		}
	}

	public virtual FAnimState PlayAnimType(int animType, float animSpeed, bool recursive, IAnimTagResponder responder)
	{
		if (!recursive)
		{
			return null;
		}
		FAnimState best = null;
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			FAnimState state = mChildNodes[i].PlayAnimType(animType, animSpeed, true, responder);
			if (state != null && (best == null || state.Duration > best.Duration))
			{
				best = state;
			}
		}
		return best;
	}

	public Coroutine StartCoroutine(IEnumerator routine)
	{
		return Futile.Instance.StartCoroutine(routine);
	}

	public void BeginLifespan(float timeInSeconds)
	{
		StartCoroutine(DoLifespan(timeInSeconds));
	}

	protected IEnumerator DoLifespan(float timeInSeconds)
	{
		yield return new WaitForSeconds(timeInSeconds);
		Dispose();
	}

	public virtual void UpdateActions()
	{
		mEntityActionList.UpdateActions();
		if (mEntityActionList.Count <= 0)
		{
			Futile.Instance.RemoveSignalUpdate(UpdateActions);
		}
	}

	public bool HasAction(FEntityAction action)
	{
		return mEntityActionList != null && mEntityActionList.HasAction(action);
	}

	public void AddAction(FEntityAction action)
	{
		if (mEntityActionList.AddAction(action) && mEntityActionList.Count == 1 && mOnStage)
		{
			Futile.Instance.AddSignalUpdate(UpdateActions);
		}
	}

	public void RemoveAction(FEntityAction action)
	{
		mEntityActionList.RemoveAction(action);
		if (mEntityActionList.Count <= 0)
		{
			Futile.Instance.RemoveSignalUpdate(UpdateActions);
		}
	}

	public void RemoveAllActions()
	{
		int i = 0;
		while (i < mEntityActionList.Count)
		{
			if (!mEntityActionList.RemoveAction(mEntityActionList.Actions[i]))
			{
				i++;
			}
		}
		if (mEntityActionList.Count > 0)
		{
			return;
		}
		if (!(Futile.Instance != null))
		{
			FAssert.Fail("Futile.Instance == null. This can happen if you stop editor during teaching flow.  need to decide if just a null check is ok here");
			return;
		}
		Futile.Instance.RemoveSignalUpdate(UpdateActions);
	}

	public void RemoveAllActionsRecursive(bool stopActions)
	{
		if (stopActions)
		{
			StopAllActions();
		}
		RemoveAllActions();
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			mChildNodes[i].RemoveAllActionsRecursive(stopActions);
		}
	}

	public void StopAllActions(bool fromDamage = false)
	{
		mEntityActionList.StopAllActions(fromDamage);
		if (mEntityActionList.Count <= 0)
		{
			Futile.Instance.RemoveSignalUpdate(UpdateActions);
		}
	}

	public void StopTweenActionsOfProperty(string propertyName)
	{
		mEntityActionList.StopTweenActionsOfProperty(propertyName);
		if (mEntityActionList.Count <= 0)
		{
			Futile.Instance.RemoveSignalUpdate(UpdateActions);
		}
	}

	public virtual void SetAnimatorBool(string paramName, bool paramValue, bool recursive = true)
	{
		if (!recursive)
		{
			return;
		}
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			mChildNodes[i].SetAnimatorBool(paramName, paramValue, true);
		}
	}

	public virtual void SetAnimatorFloat(string paramName, float paramValue, bool recursive = true)
	{
		if (!recursive)
		{
			return;
		}
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			mChildNodes[i].SetAnimatorFloat(paramName, paramValue, true);
		}
	}

	public virtual void SetAnimatorInt(string paramName, int paramValue, bool recursive = true)
	{
		if (!recursive)
		{
			return;
		}
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			mChildNodes[i].SetAnimatorInt(paramName, paramValue, true);
		}
	}

	public static bool IsSideHorizontal(Sides side)
	{
		return side == Sides.Left || side == Sides.Right || side == Sides.CenterX;
	}

	public static bool IsSideVertical(Sides side)
	{
		return side == Sides.Top || side == Sides.Bottom || side == Sides.CenterY;
	}

	public override string ToString()
	{
		string details = GetStringDetails();
		string prefix = string.IsNullOrEmpty(Name) ? "(" : (Name + " (");
		string text = string.IsNullOrEmpty(details) ? (prefix + GetType().ToString()) : (prefix + GetType().ToString() + ": " + details);
		return text + " " + GetRecursiveChildCount().ToString() + ")";
	}

	public virtual string GetStringDetails()
	{
		return "";
	}

	public T FindNodeOfType<T>() where T : FNode
	{
		if (this is T result)
		{
			return result;
		}
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			T found = mChildNodes[i].FindNodeOfType<T>();
			if (found != null)
			{
				return found;
			}
		}
		return null;
	}

	public FNode FindChildWithName(string name, bool recurse)
	{
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			if (mChildNodes[i].Name == name)
			{
				return mChildNodes[i];
			}
			if (recurse)
			{
				FNode found = mChildNodes[i].FindChildWithName(name, true);
				if (found != null)
				{
					return found;
				}
			}
		}
		return null;
	}

	public int GetRecursiveChildCount()
	{
		int count = GetChildCount();
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			count += mChildNodes[i].GetRecursiveChildCount();
		}
		return count;
	}

	public bool HasParentInHierarchy(FNode parentToCheck)
	{
		FNode node = this;
		do
		{
			node = node.mParent;
			if (node == parentToCheck)
			{
				break;
			}
		}
		while (node != null);
		return node != null;
	}

	public bool HasChild(FNode node)
	{
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			if (mChildNodes[i] == node || mChildNodes[i].HasChild(node))
			{
				return true;
			}
		}
		return false;
	}

	public virtual void Refresh()
	{
	}

	public void KillTweens(bool recursive)
	{
		Go.killAllTweensWithTarget(this);
		Go.killAllAbstractTweensWithTarget(this);
		if (!recursive)
		{
			return;
		}
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			mChildNodes[i].KillTweens(true);
		}
	}

	public static void Cleanup(FNode node)
	{
		if (node != null)
		{
			if (node.mParent != null)
			{
				node.mParent.RemoveChild(node);
			}
			node.RemoveConstraints();
			node.Dispose();
		}
	}

	public FNode GetSepiaUnderNode()
	{
		if (SepiaUnder)
		{
			return this;
		}
		for (int i = mChildNodes.Count - 1; i >= 0; i--)
		{
			if (mChildNodes[i].mVisible)
			{
				FNode found = mChildNodes[i].GetSepiaUnderNode();
				if (found != null)
				{
					return found;
				}
			}
		}
		return null;
	}

	protected void Bookkeeping1(FRenderLayer prevLayer, FRenderLayerSpriteAtlas renderLayer)
	{
		ClearRenderLayerTracking();
	}

	protected void Bookkeeping(FRenderLayer prevLayer, FRenderLayerSpriteAtlas renderLayer)
	{
		if (!renderLayer.mBoundNodes.Contains(this))
		{
			renderLayer.mBoundNodes.Add(this);
		}
		if (prevLayer != null && prevLayer != renderLayer)
		{
			prevLayer.mBoundNodes.Remove(this);
			if (prevLayer.mBoundNodes.Count == 0)
			{
				mStage.Renderer.ReleaseLiveLayer(prevLayer, false);
			}
		}
	}

	public void SetRootMotionRecursive()
	{
		if (!RootMotionEnabled)
		{
			return;
		}
		UseRootMotion = true;
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			mChildNodes[i].SetRootMotionRecursive();
		}
	}

	public void SetRootMotionRecursive(FNode refMotion)
	{
		if (!RootMotionEnabled)
		{
			return;
		}
		UseRootMotion = true;
		if (mMotionParentRef == null)
		{
			mMotionParentRef = refMotion;
		}
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			mChildNodes[i].SetRootMotionRecursive(refMotion);
		}
	}

	public void SetRootMotionDirtyRecursive()
	{
		RootPositionDirty = true;
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			mChildNodes[i].SetRootMotionDirtyRecursive();
		}
	}

	public void SetMatrixDirtyRecursive()
	{
		mMatrixIsDirty = true;
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			mChildNodes[i].SetMatrixDirtyRecursive();
		}
	}

	public void SetMatrixDirty()
	{
		mMatrixIsDirty = true;
	}

	public void OnDestroy()
	{
		ClearRenderLayerTracking();
		if (mLastRenderLayer == null || mStage == null || mStage.Renderer == null)
		{
			return;
		}
		mLastRenderLayer.mBoundNodes.Remove(this);
		if (mLastRenderLayer.mBoundNodes.Count == 0)
		{
			mStage.Renderer.ReleaseLiveLayer(mLastRenderLayer, false);
		}
	}

	public FRenderLayer GetLastRenderLayer()
	{
		return mLastRenderLayer;
	}

	private void SetSiblingLayersSortingDirty()
	{
		if (mParent == null)
		{
			return;
		}
		for (int i = 0; i < mParent.mChildNodes.Count; i++)
		{
			FNode sibling = mParent.mChildNodes[i];
			if (sibling != this && sibling.mLastRenderLayer != null)
			{
				sibling.mLastRenderLayer.SetBoundNodeSortingDirty();
			}
		}
	}

	public void SetDebugMeRecursive(bool value)
	{
		mDebugMe = value;
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			if (mChildNodes[i] != null)
			{
				mChildNodes[i].SetDebugMeRecursive(value);
			}
		}
	}

	public virtual void RemoveFromOldRenderLayer(FRenderLayer prevLayer)
	{
	}

	public void NewRLTracking(FRenderLayer layer, int vertOffset, int indexOffset, int numTris, int numVerts)
	{
		RLTracking tracking = new RLTracking();
		tracking.mNode = this;
		tracking.mLayer = layer;
		tracking.mStartingVert = vertOffset;
		tracking.mNumTris = numTris;
		tracking.mStartingIndex = indexOffset;
		tracking.mLayerGeneration = layer.mGeneration;
		tracking.mNumVerts = numVerts;
		mRenderLayerTracking.Add(tracking);
		layer.mRenderLayerTracking.Add(tracking);
	}

	public void RemoveTrackingReference(RLTracking tracking)
	{
		if (mLastRenderLayer == tracking.mLayer)
		{
			mLastRenderLayer = null;
		}
		mRenderLayerTracking.Remove(tracking);
	}

	public void DebugNotifyTracked()
	{
		// Shipped body iterates the tracking list without side effects (debug output stripped).
	}

	public void ClearRenderLayerTracking(bool forceRenderLayerUpdate = false)
	{
		for (int i = 0; i < mRenderLayerTracking.Count; i++)
		{
			RLTracking tracking = mRenderLayerTracking[i];
			if (tracking != null && tracking.mLayer != null && tracking.mLayerGeneration == tracking.mLayer.mGeneration)
			{
				RemoveFromOldRenderLayer(tracking, forceRenderLayerUpdate);
			}
		}
		mRenderLayerTracking.Clear();
	}

	public void RemoveFromOldRenderLayer(RLTracking tracking, bool forceRenderLayerUpdate = false)
	{
		if (tracking.mLayer is FRenderLayerSpriteAtlas spriteLayer)
		{
			spriteLayer.RemoveByTracking(tracking);
			if (forceRenderLayerUpdate)
			{
				spriteLayer.UpdateLayerMeshIfNeeded();
			}
		}
	}

	public void DisableRenderLayerSharingRecursive(bool value)
	{
		NoRenderLayerSharing = value;
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			mChildNodes[i].DisableRenderLayerSharingRecursive(value);
		}
	}

	public void PrimeAtlasRecursive()
	{
		PrimeAtlas();
		for (int i = 0; i < mChildNodes.Count; i++)
		{
			mChildNodes[i].PrimeAtlasRecursive();
		}
	}

	protected virtual void PrimeAtlas()
	{
	}

	protected Vector3 CalcRootPosition()
	{
		if (!UseRootMotion)
		{
			return Vector3.zero;
		}
		if (!RootPositionDirty)
		{
			return mLastRootMotionPosition;
		}
		Vector2 delta = mPosition - RootInitialPos;
		if (mMotionParentRef != null)
		{
			mMotionParentRef.InternalUpdateConstraints();
			delta += mMotionParentRef.mPosition - ParentRootInitialPos;
		}
		return new Vector3(delta.x, delta.y, 0f);
	}

	public virtual void ReComputeLabelFormatting()
	{
	}

	public bool DetectRenderLayerClosed()
	{
		if (mLastRenderLayer != null)
		{
			return mLastRenderLayer.VirtualCloseFrame == Time.frameCount;
		}
		return false;
	}
}
