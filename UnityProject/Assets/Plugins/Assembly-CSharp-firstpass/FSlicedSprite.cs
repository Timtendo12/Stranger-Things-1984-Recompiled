using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FSlicedSprite @ 0x16c1aa0..0x16c3aec, Ghidra vaddr).
public class FSlicedSprite : FFacetNode
{
	// Index lists recovered from the <PrivateImplementationDetails> initializer data in global-metadata.dat.
	private static readonly int[] cIndexListFull;

	private static readonly int[] cIndexListBorderOnly;

	private static readonly int[] cIndexListCenterOnly;

	public static float DefaultAnchorX;

	public static float DefaultAnchorY;

	protected Vector2 mLocalScale = Vector2.one;

	protected FMatrix mLocalConcatenatedMatrix = new FMatrix();

	protected bool mIsMeshDirty;

	protected bool mAreLocalVerticesDirty;

	protected Rect mLocalRect;

	protected Rect mTextureRect;

	protected float mAnchorX = DefaultAnchorX;

	protected float mAnchorY = DefaultAnchorY;

	protected Vector2[] mLocalVertices = new Vector2[16];

	protected Vector2[] mUVs = new Vector2[16];

	protected static Vector3[] mTempTransformedVertices;

	protected float mBorderLeft;

	protected float mBorderTop;

	protected float mBorderRight;

	protected float mBorderBottom;

	protected bool mShowInterior = true;

	protected bool mShowBorder = true;

	protected bool mSlicedUVs = true;

	protected bool mFlipX;

	protected bool mFlipY;

	public virtual Rect TextureRect => mTextureRect;

	public virtual Rect LocalRect => mLocalRect;

	public override float Width
	{
		get
		{
			return mLocalScale.x * mTextureRect.width;
		}
		set
		{
			float scale = value / mTextureRect.width;
			if (scale != mLocalScale.x)
			{
				mLocalScale.x = scale;
				MatrixIsDirty = true;
			}
		}
	}

	public override float Height
	{
		get
		{
			return mLocalScale.y * mTextureRect.height;
		}
		set
		{
			float scale = value / mTextureRect.height;
			if (scale != mLocalScale.y)
			{
				mLocalScale.y = scale;
				MatrixIsDirty = true;
			}
		}
	}

	public virtual float AnchorX
	{
		get
		{
			return mAnchorX;
		}
		set
		{
			if (mAnchorX != value)
			{
				mAnchorX = value;
				mAreLocalVerticesDirty = true;
			}
		}
	}

	public virtual float AnchorY
	{
		get
		{
			return mAnchorY;
		}
		set
		{
			if (mAnchorY != value)
			{
				mAnchorY = value;
				mAreLocalVerticesDirty = true;
			}
		}
	}

	public bool ShowInterior
	{
		get
		{
			return mShowInterior;
		}
		set
		{
			mShowInterior = value;
			mAreLocalVerticesDirty = true;
		}
	}

	public bool ShowBorder
	{
		get
		{
			return mShowBorder;
		}
		set
		{
			mShowBorder = value;
			mAreLocalVerticesDirty = true;
		}
	}

	public bool SetHack { get; set; }

	public bool SlicedUVs
	{
		get
		{
			return mSlicedUVs;
		}
		set
		{
			mSlicedUVs = value;
			mAreLocalVerticesDirty = true;
		}
	}

	protected FSlicedSprite()
	{
		SetHack = false;
	}

	public FSlicedSprite(string elementName, float border = -1f)
	{
		SetSprite(elementName, border, border, border, border);
	}

	public FSlicedSprite(string name, string elementName, float border = -1f)
		: base(name)
	{
		SetSprite(elementName, border, border, border, border);
	}

	public FSlicedSprite(string elementName, float borderLeft, float borderTop, float borderRight, float borderBottom)
	{
		SetSprite(elementName, borderLeft, borderTop, borderRight, borderBottom);
	}

	public void FlipSprite(bool x, bool y)
	{
		mFlipX = x;
		mFlipY = y;
		mAreLocalVerticesDirty = true;
	}

	protected override void UpdateLocalMatrix()
	{
		mLocalConcatenatedMatrix.PreScale(mLocalScale.x, mLocalScale.y, mConcatenatedMatrix);
	}

	public override void HandleElementChanged()
	{
		base.HandleElementChanged();
		mAreLocalVerticesDirty = true;
	}

	public override void RedrawInternal(bool shouldForceDirty, bool shouldUpdateDepth, bool wasMatrixDirty, bool wasAlphaDirty)
	{
		if (mElement != null && mElement.Redraw())
		{
			HandleElementChanged();
		}
		if (shouldForceDirty && wasMatrixDirty)
		{
			if (!UseRootMotion)
			{
				mIsMeshDirty = true;
			}
		}
		else if (shouldUpdateDepth || wasAlphaDirty || shouldForceDirty)
		{
			mIsMeshDirty = true;
		}
		if (wasMatrixDirty)
		{
			if (!UseRootMotion)
			{
				mIsMeshDirty = true;
			}
			else
			{
				RootPositionDirty = true;
			}
		}
		if (mAreLocalVerticesDirty || wasMatrixDirty)
		{
			UpdateLocalVertices();
		}
		Vector3 rootPosition = CalcRootPosition();
		if (UseRootMotion && mLastRenderLayer != null)
		{
			int frameCount = Time.frameCount;
			FRenderLayer layer = mLastRenderLayer;
			if (layer.mRenderLayerTracking.Count > 1 && RootPositionDirty && layer.LastFrameAccessed == frameCount && layer.RootMotionPosition != rootPosition)
			{
				mIsMeshDirty = true;
			}
			layer.LastFrameAccessed = frameCount;
		}
		bool populate = true;
		if (!DetectRenderLayerClosed())
		{
			if (!mIsMeshDirty && !mRenderLayerDirty)
			{
				populate = false;
				if (mLastRenderLayer != null)
				{
					if (mLastRenderLayer.NeedsRerender())
					{
						populate = true;
					}
					else if (((FRenderLayerSpriteAtlas)mLastRenderLayer).vertices.Count == 0)
					{
						DebugMeConditional();
					}
				}
				if (!populate)
				{
					mStage.Renderer.HackUpdateCameraRenderLayer(mLastRenderLayer);
				}
			}
		}
		else
		{
			mIsMeshDirty = true;
		}
		if (populate)
		{
			mRenderLayerDirty = false;
			if (UseRootMotion && mLastRenderLayer != null)
			{
				mLastRenderLayer.SetRootPosition(Vector3.zero);
				RootPositionDirty = true;
				RootInitialPos = mPosition;
				if (mMotionParentRef != null)
				{
					ParentRootInitialPos = mMotionParentRef.Position;
				}
			}
			PopulateRenderLayer();
			if (UseRootMotion && mLastRenderLayer != null)
			{
				mLastRenderLayer.UpdateLayerMeshIfNeeded();
			}
		}
		if (RootInitialPosSet && RootPositionDirty && mLastRenderLayer != null)
		{
			RootPositionDirty = false;
			mLastRenderLayer.SetRootPosition(rootPosition);
			mLastRootMotionPosition = rootPosition;
		}
	}

	protected override GameObject Make3DInternal(F3DTracker tracker)
	{
		RecycleCounter = 0;
		FRenderLayerSpriteAtlas renderLayer;
		mStage.Renderer.Get3DFacetRenderLayer(out renderLayer, mElement.Atlas, mShader, mNumberOfVerticesNeeded, false, RecycleRef, RecycleCounter);
		UpdateLocalVertices();
		PopulateRenderLayerInternal(renderLayer);
		GameObject renderGO = renderLayer.GetRenderGO();
		renderLayer.BakeMesh3D();
		MeshFilter meshFilter = renderGO.GetOrCreateComponent<MeshFilter>();
		meshFilter.mesh = renderLayer.GetMesh();
		renderGO.SetActive(true);
		tracker.AddRenderLayer(renderGO, renderLayer);
		return renderGO;
	}

	protected virtual void UpdateLocalVertices()
	{
		mAreLocalVerticesDirty = false;
		mNumberOfVerticesNeeded = ((mShowInterior ? 1 : 0) << 2) | (mShowBorder ? 32 : 0);
		float sourceWidth = mElement.SourceSize.x;
		float sourceHeight = mElement.SourceSize.y;
		float scaleX = mLocalScale.x;
		float scaleY = mLocalScale.y;
		float texLeft = -(mAnchorX * sourceWidth);
		float texBottom = -(mAnchorY * sourceHeight);
		mTextureRect = new Rect(texLeft, texBottom, sourceWidth, sourceHeight);
		float rectWidth = mElement.SourceRect.width;
		float rectHeight = mElement.SourceRect.height;
		mLocalRect.width = rectWidth;
		mLocalRect.height = rectHeight;
		float left = texLeft + mElement.SourceRect.x / scaleX;
		float bottom = texBottom + (sourceHeight - rectHeight - mElement.SourceRect.y) / scaleY;
		mLocalRect.x = left;
		mLocalRect.y = bottom;
		float borderRight = mBorderRight;
		float borderLeft = mBorderLeft;
		float borderBottom = mBorderBottom;
		float borderTop = mBorderTop;
		if (mFlipX)
		{
			FCoreUtil.Swap(ref borderLeft, ref borderRight);
		}
		if (mFlipY)
		{
			FCoreUtil.Swap(ref borderTop, ref borderBottom);
		}
		float top = sourceHeight + texBottom - mElement.SourceRect.y / scaleY;
		float right = sourceWidth + texLeft - (sourceWidth - rectWidth - mElement.SourceRect.x) / scaleX;
		float innerBottom = bottom + borderBottom / scaleY;
		float innerTop = top - borderTop / scaleY;
		float innerLeft = left + borderLeft / scaleX;
		float innerRight = right - borderRight / scaleX;
		mLocalVertices[0] = new Vector2(left, bottom);
		mLocalVertices[1] = new Vector2(left, innerBottom);
		mLocalVertices[2] = new Vector2(left, innerTop);
		mLocalVertices[3] = new Vector2(left, top);
		mLocalVertices[4] = new Vector2(innerLeft, bottom);
		mLocalVertices[5] = new Vector2(innerLeft, innerBottom);
		mLocalVertices[6] = new Vector2(innerLeft, innerTop);
		mLocalVertices[7] = new Vector2(innerLeft, top);
		mLocalVertices[8] = new Vector2(innerRight, bottom);
		mLocalVertices[9] = new Vector2(innerRight, innerBottom);
		mLocalVertices[10] = new Vector2(innerRight, innerTop);
		mLocalVertices[11] = new Vector2(innerRight, top);
		mLocalVertices[12] = new Vector2(right, bottom);
		mLocalVertices[13] = new Vector2(right, innerBottom);
		mLocalVertices[14] = new Vector2(right, innerTop);
		mLocalVertices[15] = new Vector2(right, top);
		Vector2 uvBottomLeft = mElement.uvBottomLeft;
		Vector2 up = mElement.uvTopLeft - uvBottomLeft;
		Vector2 across = mElement.uvBottomRight - uvBottomLeft;
		if (!mSlicedUVs)
		{
			for (int i = 0; i < 16; i++)
			{
				float u = (mLocalVertices[i].x - left) / rectWidth;
				float v = (mLocalVertices[i].y - bottom) / rectHeight;
				mUVs[i] = new Vector2(up.x * v + mElement.uvBottomLeft.x + across.x * u, up.y * v + mElement.uvBottomLeft.y + across.y * u);
			}
		}
		else
		{
			// Unflipped border values are used for the UVs (the original reads the fields before swapping).
			float w = mTextureRect.width;
			float h = mTextureRect.height;
			float b = mBorderBottom / h;
			float t = mBorderTop / h;
			float l = mBorderLeft / w;
			float r = mBorderRight / w;
			Vector2 tl = mElement.uvTopLeft;
			Vector2 tr = mElement.uvTopRight;
			Vector2 br = mElement.uvBottomRight;
			mUVs[0] = mElement.uvBottomLeft;
			mUVs[1] = mElement.uvBottomLeft + up * b;
			mUVs[2] = tl - up * t;
			mUVs[3] = tl;
			mUVs[4] = mElement.uvBottomLeft + across * l;
			mUVs[5] = mElement.uvBottomLeft + across * l + up * b;
			mUVs[6] = tl + across * l - up * t;
			mUVs[7] = tl + across * l;
			mUVs[8] = br - across * r;
			mUVs[9] = br - across * r + up * b;
			mUVs[10] = tr - across * r - up * t;
			mUVs[11] = tr - across * r;
			mUVs[12] = br;
			mUVs[13] = br + up * b;
			mUVs[14] = tr - up * t;
			mUVs[15] = tr;
		}
		if (mFlipX)
		{
			for (int j = 0; j < 8; j++)
			{
				int k = j ^ 15;
				if (!mElement.IsRotated)
				{
					FCoreUtil.Swap(ref mUVs[j].x, ref mUVs[k].x);
				}
				else
				{
					FCoreUtil.Swap(ref mUVs[j].y, ref mUVs[k].y);
				}
			}
		}
		if (mFlipY)
		{
			for (int c = 0; c < 16; c += 4)
			{
				if (!mElement.IsRotated)
				{
					FCoreUtil.Swap(ref mUVs[c].y, ref mUVs[c + 3].y);
					FCoreUtil.Swap(ref mUVs[c + 1].y, ref mUVs[c + 2].y);
				}
				else
				{
					FCoreUtil.Swap(ref mUVs[c].x, ref mUVs[c + 3].x);
					FCoreUtil.Swap(ref mUVs[c + 1].x, ref mUVs[c + 2].x);
				}
			}
		}
		mIsMeshDirty = true;
	}

	public override void PopulateRenderLayer()
	{
		FRenderLayer prevLayer = mLastRenderLayer;
		FRenderLayerSpriteAtlas renderLayer;
		mStage.Renderer.GetRenderLayerSpriteAtlas(out renderLayer, NoRenderLayerSharing, mElement.Atlas, mShader, mNumberOfVerticesNeeded, false, mShouldSortByZ);
		Bookkeeping1(prevLayer, renderLayer);
		PopulateRenderLayerInternal(renderLayer);
		mLastRenderLayer = renderLayer;
		Bookkeeping(prevLayer, renderLayer);
	}

	protected void PopulateRenderLayerInternal(FRenderLayerSpriteAtlas renderLayer)
	{
		if (!mOnStage)
		{
			base.PopulateRenderLayer();
			return;
		}
		mIsMeshDirty = false;
		int[] indexList;
		if (mShowInterior)
		{
			indexList = mShowBorder ? cIndexListFull : cIndexListCenterOnly;
		}
		else
		{
			indexList = mShowBorder ? cIndexListBorderOnly : null;
		}
		Color tint = ConcatenatedAdditiveTintColor;
		Color color = mConcatenatedColor.CloneWithMultipliedAlpha(mConcatenatedAlpha);
		if (indexList == null)
		{
			base.PopulateRenderLayer();
			return;
		}
		int vertexIndex = renderLayer.vertices.Count;
		int count = indexList.Length;
		NewRLTracking(renderLayer, vertexIndex, renderLayer.indexCount, count / 4 * 2, count & ~3);
		var vertices = renderLayer.vertices;
		var uvs = renderLayer.uvs;
		var colors = renderLayer.colors;
		var colors2 = renderLayer.colors2;
		for (int i = 0; i < mLocalVertices.Length; i++)
		{
			mLocalConcatenatedMatrix.ApplyVector3FromLocalVector2(out mTempTransformedVertices[i], mLocalVertices[i], 0f);
		}
		for (int j = 0; j < indexList.Length; j++)
		{
			vertices.Add(mTempTransformedVertices[indexList[j]]);
			uvs.Add(mUVs[indexList[j]]);
			colors.Add(color);
			colors2.Add(tint);
		}
		renderLayer.AddSimpleQuadList(vertexIndex);
		renderLayer.SetLayerMeshDirty();
	}

	public void SetSprite(string elementName)
	{
		SetSprite(elementName, -1f, -1f, -1f, -1f);
	}

	public void SetSprite(string elementName, float borderSize)
	{
		SetSprite(elementName, borderSize, borderSize, borderSize, borderSize);
	}

	public void SetSprite(string elementName, float borderLeft, float borderTop, float borderRight, float borderBottom)
	{
		// The original compares ElementName with elementName here but ignores the result.
		bool unused = elementName == ElementName;
		SetSprite(Futile.AtlasManager.GetElementWithName(elementName), borderLeft, borderTop, borderRight, borderBottom);
	}

	public void SetSprite(FAtlasElement element, float borderLeft, float borderTop, float borderRight, float borderBottom)
	{
		Init(element, 36);
		if (borderLeft < 0f)
		{
			borderLeft = mElement.BorderSize.x;
		}
		mBorderLeft = borderLeft;
		if (borderTop < 0f)
		{
			borderTop = mElement.BorderSize.y;
		}
		mBorderTop = borderTop;
		if (borderRight < 0f)
		{
			borderRight = mElement.BorderSize.z;
		}
		mBorderRight = borderRight;
		if (borderBottom < 0f)
		{
			borderBottom = mElement.BorderSize.w;
		}
		mBorderBottom = borderBottom;
		mShowBorder = borderLeft >= FMath.Epsilon || mBorderRight >= FMath.Epsilon || mBorderTop >= FMath.Epsilon || mBorderBottom >= FMath.Epsilon;
		mAreLocalVerticesDirty = true;
		mShowInterior = mElement.ShowSlicedInterior;
		mAlphaIsDirty = true;
		UpdateLocalVertices();
		MarkDirtyRecursive();
	}

	public void SetSize(float size)
	{
		Width = size;
		Height = size;
	}

	public void SetSize(Vector2 size)
	{
		Width = size.x;
		Height = size.y;
	}

	public void SetSize(float xSize, float ySize)
	{
		Width = xSize;
		Height = ySize;
	}

	public override void SelfResizeExtents()
	{
		if (HasConstraints() && CalculateExtents())
		{
			Rect extents = Extents;
			Position = new Vector2(extents.x + extents.width * 0.5f, extents.y + extents.height * 0.5f);
			if (!SetHack)
			{
				Width = extents.width;
				Height = extents.height;
			}
		}
	}

	static FSlicedSprite()
	{
		cIndexListFull = new int[36]
		{
			0, 1, 5, 4, 1, 2, 6, 5, 2, 3, 7, 6, 4, 5, 9, 8, 5, 6, 10, 9,
			6, 7, 11, 10, 8, 9, 13, 12, 9, 10, 14, 13, 10, 11, 15, 14
		};
		cIndexListBorderOnly = new int[32]
		{
			0, 1, 5, 4, 1, 2, 6, 5, 2, 3, 7, 6, 4, 5, 9, 8, 6, 7, 11, 10,
			8, 9, 13, 12, 9, 10, 14, 13, 10, 11, 15, 14
		};
		cIndexListCenterOnly = new int[4] { 5, 6, 10, 9 };
		DefaultAnchorX = 0.5f;
		DefaultAnchorY = 0.5f;
		mTempTransformedVertices = new Vector3[16];
	}
}
