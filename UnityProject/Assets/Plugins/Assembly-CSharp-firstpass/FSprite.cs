using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FSprite @ 0x16bcd90..0x16c512c, Ghidra vaddr).
public class FSprite : FFacetNode
{
	protected Vector2 mLocalScale = Vector2.one;

	protected Vector2 mFlipScale = Vector2.one;

	protected FMatrix mLocalConcatenatedMatrix = new FMatrix();

	protected Vector2[] mLocalVertices = new Vector2[4];

	protected Vector2[] mCustomUVs;

	protected Rect mLocalRect;

	protected Rect mTextureRect;

	protected bool mMeshIsDirty;

	protected bool mLocalVerticesAreDirty;

	public virtual Rect TextureRect => mTextureRect;

	public virtual Rect LocalRect => mLocalRect;

	public Vector2[] CustomUVs => mCustomUVs;

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

	public FSprite()
	{
	}

	public FSprite(string elementName)
	{
		SetSprite(elementName);
	}

	public FSprite(string name, string elementName)
		: base(name)
	{
		SetSprite(elementName);
	}

	public void FlipSprite(bool x, bool y = false)
	{
		mFlipScale.x = x ? -1f : 1f;
		mFlipScale.y = y ? -1f : 1f;
	}

	protected override void UpdateLocalMatrix()
	{
		mLocalConcatenatedMatrix.PreScale(mFlipScale.x * mLocalScale.x, mFlipScale.y * mLocalScale.y, mConcatenatedMatrix);
	}

	public override void HandleElementChanged()
	{
		base.HandleElementChanged();
		mLocalVerticesAreDirty = true;
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
				mMeshIsDirty = true;
			}
		}
		else if (shouldUpdateDepth || wasAlphaDirty || shouldForceDirty)
		{
			mMeshIsDirty = true;
		}
		if (wasMatrixDirty)
		{
			if (!UseRootMotion)
			{
				mMeshIsDirty = true;
			}
			else
			{
				RootPositionDirty = true;
			}
		}
		if (mLocalVerticesAreDirty)
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
				mMeshIsDirty = true;
			}
			layer.LastFrameAccessed = frameCount;
		}
		if (!DetectRenderLayerClosed())
		{
			if (!mMeshIsDirty && !mRenderLayerDirty && (mLastRenderLayer == null || !mLastRenderLayer.NeedsRerender()))
			{
				mStage.Renderer.HackUpdateCameraRenderLayer(mLastRenderLayer);
				ApplyRootPosition(rootPosition);
				return;
			}
		}
		else
		{
			mMeshIsDirty = true;
		}
		mRenderLayerDirty = false;
		if (UseRootMotion && mLastRenderLayer != null)
		{
			mLastRenderLayer.TempClearRootPosition();
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
		ApplyRootPosition(rootPosition);
	}

	// Shared tail of RedrawInternal (the original jumps to one label from both paths).
	private void ApplyRootPosition(Vector3 rootPosition)
	{
		if (RootInitialPosSet && RootPositionDirty && mLastRenderLayer != null)
		{
			RootPositionDirty = false;
			mLastRenderLayer.SetRootPosition(rootPosition);
			mLastRootMotionPosition = rootPosition;
		}
	}

	protected override GameObject Make3DInternal(F3DTracker tracker)
	{
		if (mElement != null && mElement.Redraw())
		{
			HandleElementChanged();
		}
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
		mLocalVerticesAreDirty = false;
		mTextureRect.width = mElement.SourceSize.x;
		mTextureRect.height = mElement.SourceSize.y;
		mTextureRect.x = -0.5f * mTextureRect.width;
		mTextureRect.y = -0.5f * mTextureRect.height;
		float sourceWidth = mElement.SourceRect.width;
		float sourceHeight = mElement.SourceRect.height;
		mLocalRect.width = sourceWidth;
		mLocalRect.height = sourceHeight;
		float left = mTextureRect.x + mElement.SourceRect.x;
		float bottom = mTextureRect.y + (mTextureRect.height - mElement.SourceRect.y - sourceHeight);
		mLocalRect.x = left;
		mLocalRect.y = bottom;
		mLocalVertices[0] = new Vector2(left, bottom + sourceHeight);
		mLocalVertices[1] = new Vector2(left + sourceWidth, bottom + sourceHeight);
		mLocalVertices[2] = new Vector2(left + sourceWidth, bottom);
		mLocalVertices[3] = new Vector2(left, bottom);
		mMeshIsDirty = true;
	}

	public override void PopulateRenderLayer()
	{
		FRenderLayer prevLayer = mLastRenderLayer;
		FRenderLayerSpriteAtlas renderLayer;
		mStage.Renderer.GetRenderLayerSpriteAtlas(out renderLayer, NoRenderLayerSharing, mElement.Atlas, mShader, mNumberOfVerticesNeeded, false, mShouldSortByZ, prevLayer as FRenderLayerSpriteAtlas, UseRootMotion, mLastRootMotionPosition);
		Bookkeeping1(prevLayer, renderLayer);
		PopulateRenderLayerInternal(renderLayer);
		mLastRenderLayer = renderLayer;
		Bookkeeping(prevLayer, renderLayer);
	}

	protected void PopulateRenderLayerInternal(FRenderLayerSpriteAtlas renderLayer)
	{
		if (mOnStage)
		{
			mMeshIsDirty = false;
			int vertexIndex = renderLayer.vertices.Count;
			NewRLTracking(renderLayer, vertexIndex, renderLayer.indexCount, 2, 4);
			var vertices = renderLayer.vertices;
			var uvs = renderLayer.uvs;
			var colors = renderLayer.colors;
			var colors2 = renderLayer.colors2;
			vertices.Add(mLocalConcatenatedMatrix.GetVector3FromLocalVector2(mLocalVertices[0], 0f));
			vertices.Add(mLocalConcatenatedMatrix.GetVector3FromLocalVector2(mLocalVertices[1], 0f));
			vertices.Add(mLocalConcatenatedMatrix.GetVector3FromLocalVector2(mLocalVertices[2], 0f));
			vertices.Add(mLocalConcatenatedMatrix.GetVector3FromLocalVector2(mLocalVertices[3], 0f));
			if (mCustomUVs == null)
			{
				uvs.Add(mElement.uvTopLeft);
				uvs.Add(mElement.uvTopRight);
				uvs.Add(mElement.uvBottomRight);
				uvs.Add(mElement.uvBottomLeft);
			}
			else
			{
				uvs.Add(mCustomUVs[0]);
				uvs.Add(mCustomUVs[1]);
				uvs.Add(mCustomUVs[2]);
				uvs.Add(mCustomUVs[3]);
			}
			Color color = mConcatenatedColor.CloneWithMultipliedAlpha(mConcatenatedAlpha);
			colors.Add(color);
			colors.Add(color);
			colors.Add(color);
			colors.Add(color);
			Color tint = ConcatenatedAdditiveTintColor;
			colors2.Add(tint);
			colors2.Add(tint);
			colors2.Add(tint);
			colors2.Add(tint);
			renderLayer.AddSimpleQuadList(vertexIndex);
			renderLayer.SetLayerMeshDirty();
		}
		base.PopulateRenderLayer();
	}

	public void SetSprite(string elementName)
	{
		FAtlasElement element = Futile.AtlasManager.GetElementWithName(elementName);
		if (element != mElement)
		{
			Init(element, 4);
			mAlphaIsDirty = true;
			UpdateLocalVertices();
		}
	}

	public override void SelfResizeExtents()
	{
		if (HasConstraints() && CalculateExtents())
		{
			Rect extents = Extents;
			Position = new Vector2(extents.x + extents.width * 0.5f, extents.y + extents.height * 0.5f);
			Width = extents.width;
			Height = extents.height;
		}
	}

	public void AllocateCustomUVs()
	{
		if (mCustomUVs == null)
		{
			mCustomUVs = new Vector2[4];
		}
	}

	public void SetCustomUVsByPercent(Vector2 min, Vector2 max)
	{
		if (mCustomUVs == null)
		{
			AllocateCustomUVs();
		}
		Vector2 bottomLeft = mElement.uvBottomLeft;
		if (!mElement.IsRotated)
		{
			float width = mElement.uvBottomRight.x - bottomLeft.x;
			float height = mElement.uvTopLeft.y - bottomLeft.y;
			mCustomUVs[0] = new Vector2(bottomLeft.x + min.x * width, bottomLeft.y + max.y * height);
			mCustomUVs[1] = new Vector2(bottomLeft.x + max.x * width, bottomLeft.y + max.y * height);
			mCustomUVs[2] = new Vector2(bottomLeft.x + max.x * width, bottomLeft.y + min.y * height);
			mCustomUVs[3] = new Vector2(bottomLeft.x + min.x * width, bottomLeft.y + min.y * height);
		}
		else
		{
			float width = mElement.uvTopLeft.x - bottomLeft.x;
			float height = mElement.uvBottomRight.y - bottomLeft.y;
			mCustomUVs[0] = new Vector2(bottomLeft.x + max.y * width, bottomLeft.y + min.x * height);
			mCustomUVs[1] = new Vector2(bottomLeft.x + max.y * width, bottomLeft.y + max.x * height);
			mCustomUVs[2] = new Vector2(bottomLeft.x + min.y * width, bottomLeft.y + max.x * height);
			mCustomUVs[3] = new Vector2(bottomLeft.x + min.y * width, bottomLeft.y + min.x * height);
		}
	}

	public bool ContainsPoint(Vector2 point)
	{
		Vector2 local = GlobalToLocal(point);
		return local.x >= mLocalRect.x && local.x < mLocalRect.x + mLocalRect.width && local.y >= mLocalRect.y && local.y < mLocalRect.y + mLocalRect.height;
	}
}
