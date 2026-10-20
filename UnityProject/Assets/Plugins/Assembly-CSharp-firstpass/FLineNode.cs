using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FLineNode @ 0x16b3ebc..0x16b50a8, Ghidra vaddr).
public class FLineNode : FFacetNode
{
	protected float mLineWidth = 1f;

	protected List<Vector2> mPoints = new List<Vector2>();

	protected List<Color> mColors = new List<Color>();

	protected List<Vector2> mLocalVertices = new List<Vector2>();

	protected bool mIsMeshDirty = true;

	protected bool mAreLocalVerticesDirty = true;

	public float LineWidth
	{
		get
		{
			return mLineWidth;
		}
		set
		{
			mLineWidth = value;
		}
	}

	public List<Vector2> Points
	{
		get
		{
			return mPoints;
		}
		set
		{
			mPoints = value;
			mAreLocalVerticesDirty = true;
		}
	}

	public List<Color> Colors
	{
		get
		{
			return mColors;
		}
		set
		{
			mColors = value;
			mAreLocalVerticesDirty = true;
		}
	}

	public FLineNode(float lineWidth = 1f)
	{
		mLineWidth = lineWidth;
		Init(Futile.AtlasManager.GetElementWithName("WhiteSquare.png"), 0);
	}

	public override void Invalidate()
	{
		mAreLocalVerticesDirty = true;
	}

	public override void RedrawInternal(bool shouldForceDirty, bool shouldUpdateDepth, bool wasMatrixDirty, bool wasAlphaDirty)
	{
		if (!shouldForceDirty && !wasAlphaDirty && !shouldUpdateDepth && !wasMatrixDirty)
		{
			if (!mIsMeshDirty && !mRenderLayerDirty && (mLastRenderLayer == null || !mLastRenderLayer.NeedsRerender()))
			{
				if (mStage != null)
				{
					mStage.Renderer.HackUpdateCameraRenderLayer(mLastRenderLayer);
				}
				return;
			}
		}
		else
		{
			mIsMeshDirty = true;
		}
		mRenderLayerDirty = false;
		PopulateRenderLayer();
	}

	public override void RedrawPrep(bool shouldForceDirty, bool shouldUpdateDepth, bool wasMatrixDirty, bool wasAlphaDirty)
	{
		if (mAreLocalVerticesDirty)
		{
			int num = mNumberOfVerticesNeeded;
			UpdateLocalVertices();
			if (num != mNumberOfVerticesNeeded && mStage != null)
			{
				mStage.HandleFacetsChanged();
			}
		}
	}

	private static Vector2 SafeNormalize(Vector2 v)
	{
		float magnitude = v.magnitude;
		if (magnitude <= 1E-05f)
		{
			return Vector2.zero;
		}
		return v / magnitude;
	}

	protected virtual void UpdateLocalVertices()
	{
		mIsMeshDirty = true;
		mAreLocalVerticesDirty = false;
		if (mPoints.Count < 2)
		{
			mNumberOfVerticesNeeded = 0;
			return;
		}
		mNumberOfVerticesNeeded = (mPoints.Count - 1) * 4;
		mLocalVertices.Clear();
		float num = mLineWidth * 0.5f;
		Vector2 vector = SafeNormalize(mPoints[1] - mPoints[0]);
		// TODO(port): the miter-join arithmetic is partly lost in the decompile (vector registers, 0x16b41a0); standard miter strip used [left: only used by the debug FPS graph]
		Vector2 vector2 = new Vector2(0f - vector.y, vector.x);
		for (int i = 0; i < mPoints.Count - 1; i++)
		{
			Vector2 vector3 = SafeNormalize(mPoints[i + 1] - mPoints[i]);
			Vector2 vector4 = ((i + 1 != mPoints.Count - 1) ? SafeNormalize(mPoints[i + 2] - mPoints[i + 1]) : vector3);
			Vector2 vector5 = vector3 + vector4;
			Vector2 vector6;
			if (vector5.magnitude <= FMath.Epsilon)
			{
				vector6 = new Vector2(0f - vector3.y, vector3.x);
			}
			else
			{
				vector5 /= vector5.magnitude;
				Vector2 vector7 = new Vector2(0f - vector5.y, vector5.x);
				float num2 = vector7.x * (0f - vector3.y) + vector7.y * vector3.x;
				float num3 = ((!(num2 >= 0.05f)) ? 20f : (1f / num2));
				vector6 = vector7 * num3;
			}
			mLocalVertices.Add(mPoints[i] - vector2 * num);
			mLocalVertices.Add(mPoints[i] + vector2 * num);
			mLocalVertices.Add(mPoints[i + 1] + vector6 * num);
			mLocalVertices.Add(mPoints[i + 1] - vector6 * num);
			vector2 = vector6;
		}
	}

	public override void PopulateRenderLayer()
	{
		if (!mOnStage)
		{
			return;
		}
		mIsMeshDirty = false;
		if (mStage == null || mElement == null)
		{
			return;
		}
		FRenderLayer prevLayer = mLastRenderLayer;
		mStage.Renderer.GetRenderLayerSpriteAtlas(out var renderLayer, NoRenderLayerSharing, mElement.Atlas, mShader, mNumberOfVerticesNeeded, false, mShouldSortByZ, prevLayer as FRenderLayerSpriteAtlas, UseRootMotion, mLastRootMotionPosition);
		Bookkeeping1(prevLayer, renderLayer);
		mLastRenderLayer = renderLayer;
		Bookkeeping(prevLayer, renderLayer);
		if (renderLayer == null || renderLayer.vertices == null)
		{
			return;
		}
		int count = renderLayer.vertices.Count;
		int num = (mNumberOfVerticesNeeded + 3) / 4;
		NewRLTracking(renderLayer, count, renderLayer.indexCount, num * 2, num * 4);
		Color tint = ConcatenatedAdditiveTintColor;
		Vector3[] array = new Vector3[mLocalVertices.Count];
		for (int i = 0; i < mLocalVertices.Count; i++)
		{
			array[i] = mConcatenatedMatrix.GetVector3FromLocalVector2(mLocalVertices[i], 0f);
		}
		Vector2 item = (mElement.uvTopLeft + mElement.uvBottomRight) * 0.5f;
		for (int j = 0; j < mNumberOfVerticesNeeded; j++)
		{
			renderLayer.vertices.Add(array[j]);
			renderLayer.uvs.Add(item);
			renderLayer.colors2.Add(tint);
		}
		for (int k = 0; k < num; k++)
		{
			Color color = mColors[k];
			Color color2 = mColors[k + 1];
			Color color3 = new Color(color.r * mConcatenatedColor.r, color.g * mConcatenatedColor.g, color.b * mConcatenatedColor.b, color.a * mConcatenatedAlpha * mConcatenatedColor.a);
			Color color4 = new Color(color2.r * mConcatenatedColor.r, color2.g * mConcatenatedColor.g, color2.b * mConcatenatedColor.b, color2.a * mConcatenatedAlpha * mConcatenatedColor.a);
			renderLayer.colors.Add(color3);
			renderLayer.colors.Add(color3);
			renderLayer.colors.Add(color4);
			renderLayer.colors.Add(color4);
		}
		renderLayer.AddSimpleQuadList(count);
	}

	public void AddPoint(Vector2 p, Color c)
	{
		mPoints.Add(p);
		mColors.Add(c);
	}
}
