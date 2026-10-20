using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FRenderer @ 0x1690d54..0x169388c, Ghidra vaddr).
public class FRenderer
{
	private List<FRenderLayer> mAllLayers = new List<FRenderLayer>();

	private List<FRenderLayer> mLiveLayers = new List<FRenderLayer>();

	private List<FRenderLayer> mPreviousLiveLayers = new List<FRenderLayer>();

	private List<FRenderLayer> mCachedLayers = new List<FRenderLayer>();

	private FRenderLayer mTopLayer;

	private FStage mStage;

	public static int NewLayers;

	public static int ReusedLayers;

	private List<FRenderLayer> mOldLayers = new List<FRenderLayer>();

	private const int kMaxOldLayers = 50;

	private static bool mVirtualBatchIndexDirty;

	private static int mVirtualBatchIndex;

	public static int VirtualBatchIndex
	{
		get
		{
			return mVirtualBatchIndex;
		}
		set
		{
			if (mVirtualBatchIndex != value)
			{
				mVirtualBatchIndex = value;
				mVirtualBatchIndexDirty = true;
			}
		}
	}

	public FRenderer(FStage stage)
	{
		mStage = stage;
	}

	public void Clear()
	{
		int count = mAllLayers.Count;
		for (int i = 0; i < count; i++)
		{
			mAllLayers[i].Destroy();
		}
		mAllLayers.Clear();
		mLiveLayers.Clear();
		mPreviousLiveLayers.Clear();
		mCachedLayers.Clear();
	}

	public void UpdateLayerTransforms()
	{
		int count = mAllLayers.Count;
		for (int i = 0; i < count; i++)
		{
			mAllLayers[i].UpdateTransform();
		}
	}

	public void UpdateLayerTextures()
	{
		int count = mAllLayers.Count;
		for (int i = 0; i < count; i++)
		{
			mAllLayers[i].UpdateTexture();
		}
	}

	public void StartRender()
	{
		List<FRenderLayer> swap = mLiveLayers;
		mLiveLayers = mPreviousLiveLayers;
		mPreviousLiveLayers = swap;
		mTopLayer = null;
	}

	public void EndRender()
	{
		int count = mPreviousLiveLayers.Count;
		for (int i = 0; i < count; i++)
		{
			if (!mPreviousLiveLayers[i].Destroyed)
			{
				mPreviousLiveLayers[i].RemoveFromWorld();
				if (mPreviousLiveLayers[i] is FRenderLayerGameObjects)
				{
					mCachedLayers.Add(mPreviousLiveLayers[i]);
				}
			}
		}
		mPreviousLiveLayers.Clear();
		if (mTopLayer != null)
		{
			mTopLayer.Close();
		}
	}

	public void ReleaseLiveLayer(FRenderLayer layer, bool destroy)
	{
		if (layer.Destroyed)
		{
			return;
		}
		layer.SetLayerDirty();
		int index = mLiveLayers.IndexOf(layer);
		if (index >= 0 && index < mLiveLayers.Count)
		{
			mLiveLayers.RemoveAt(index);
		}
		if (destroy)
		{
			mAllLayers.Remove(layer);
			Futile.RemoveCurrentCameraRenderLayer(layer);
			layer.Destroy();
			return;
		}
		if (mOldLayers.Contains(layer))
		{
			// Original evaluates layer.ToString() here (result unused; a stripped log message).
			layer.ToString();
			return;
		}
		layer.IsInOldList = true;
		mOldLayers.Add(layer);
		layer.mGeneration++;
		layer.RemoveFromWorld();
		layer.ClearRLTracking();
		layer.mBoundNodes.Clear();
		Futile.RemoveCurrentCameraRenderLayer(layer);
		if (mOldLayers.Count > kMaxOldLayers)
		{
			FRenderLayer oldest = mOldLayers[0];
			mAllLayers.Remove(oldest);
			oldest.Destroy();
			mOldLayers.RemoveAt(0);
		}
	}

	public void DestroyOldLayers()
	{
		for (int i = 0; i < mOldLayers.Count; i++)
		{
			FRenderLayer layer = mOldLayers[i];
			mAllLayers.Remove(layer);
			layer.Destroy();
		}
		mOldLayers.Clear();
		for (int j = 0; j < mCachedLayers.Count; j++)
		{
			FRenderLayer layer2 = mCachedLayers[j];
			mAllLayers.Remove(layer2);
			layer2.Destroy();
		}
		mCachedLayers.Clear();
	}

	protected FRenderLayerSpriteAtlas CreateRenderLayerSpriteAtlas(int batchIndex, FAtlas atlas, FShader shader, bool usingUV2_3_4)
	{
		for (int i = 0; i < mOldLayers.Count; i++)
		{
			FRenderLayerSpriteAtlas oldLayer = mOldLayers[i] as FRenderLayerSpriteAtlas;
			if (oldLayer != null && oldLayer.BatchIndex == batchIndex && oldLayer.mUsingUV2_3_4 == usingUV2_3_4)
			{
				oldLayer.IsInOldList = false;
				mOldLayers.RemoveAt(i);
				mLiveLayers.Add(oldLayer);
				oldLayer.AddToWorld();
				oldLayer.mGeneration++;
				oldLayer.ClearRLTracking();
				oldLayer.mBoundNodes.Clear();
				if (oldLayer.depth != -1)
				{
					oldLayer.depth = -1;
				}
				return oldLayer;
			}
		}
		FRenderLayerSpriteAtlas layer = new FRenderLayerSpriteAtlas(mStage, atlas, shader, usingUV2_3_4);
		mLiveLayers.Add(layer);
		mAllLayers.Add(layer);
		layer.AddToWorld();
		layer.mGeneration++;
		layer.ClearRLTracking();
		layer.mBoundNodes.Clear();
		if (layer.depth != -1)
		{
			layer.depth = -1;
		}
		return layer;
	}

	protected bool NeedNewFacetTopLayer(FRenderLayer renderLayer, int batchIndex, int numberOfVerticesNeeded, bool supportUV2_3_4, bool shouldSortByZ, bool useRootMotion = false, Vector3 rootMotionPos = default(Vector3))
	{
		FRenderLayerSpriteAtlas layer = renderLayer as FRenderLayerSpriteAtlas;
		if (layer != null && layer.IsOpen && layer.CanRLShare() && layer.BatchIndex == batchIndex && !mVirtualBatchIndexDirty && layer.UseRootMotion == useRootMotion && (!layer.UseRootMotion || layer.RootMotionPosition == rootMotionPos) && layer.CanFitVertices(numberOfVerticesNeeded) && layer.mUsingUV2_3_4 == supportUV2_3_4 && layer.ShouldSortByZ == shouldSortByZ && !layer.Destroyed)
		{
			return layer.IsInOldList;
		}
		return true;
	}

	public bool GetRenderLayerSpriteAtlas(out FRenderLayerSpriteAtlas renderLayer, bool noRenderLayerSharing, FAtlas atlas, FShader shader, int numberOfFacetsNeeded, bool supportUV2_3_4, bool shouldSortByZ, FRenderLayerSpriteAtlas lastLayer = null, bool useRootMotion = false, Vector3 rootMotionPos = default(Vector3))
	{
		int batchIndex = shader.Index + atlas.Index * 10000;
		bool needNew = NeedNewFacetTopLayer(mTopLayer, batchIndex, numberOfFacetsNeeded, supportUV2_3_4, shouldSortByZ, useRootMotion, rootMotionPos);
		if (!needNew && !noRenderLayerSharing)
		{
			MakeSureLayerIsEnabled(mTopLayer);
			Futile.AddCurrentCameraRenderLayer(mTopLayer);
			int frameCount = Time.frameCount;
			if (mTopLayer.LastFrameEdited != frameCount)
			{
				mTopLayer.LastFrameEdited = frameCount;
			}
			if (!mTopLayer.IsOpen)
			{
				mTopLayer.Open();
				if (mTopLayer == null)
				{
					renderLayer = null;
					return false;
				}
			}
		}
		else
		{
			if (mTopLayer != null && mTopLayer.IsOpen)
			{
				mTopLayer.Close();
			}
			mTopLayer = CreateRenderLayerSpriteAtlas(batchIndex, atlas, shader, supportUV2_3_4);
			VirtualBatchIndex = batchIndex;
			mVirtualBatchIndexDirty = false;
			mTopLayer.UseRootMotion = useRootMotion;
			mTopLayer.ShouldSortByZ = shouldSortByZ;
			if (!mTopLayer.IsOpen)
			{
				mTopLayer.Open();
			}
			else
			{
				Futile.AddCurrentCameraRenderLayer(mTopLayer);
			}
			mTopLayer.LastFrameEdited = Time.frameCount;
		}
		renderLayer = mTopLayer as FRenderLayerSpriteAtlas;
		return needNew || noRenderLayerSharing;
	}

	public void HackUpdateCameraRenderLayer(FRenderLayer renderLayer)
	{
		if (renderLayer != null && !FStage.HackOldMode && !renderLayer.Destroyed)
		{
			MakeSureLayerIsEnabled(renderLayer);
			Futile.AddCurrentCameraRenderLayer(renderLayer);
		}
	}

	public static void MakeSureLayerIsEnabled(FRenderLayer renderLayer)
	{
		if (!renderLayer.IsActive())
		{
			renderLayer.AddToWorld();
		}
	}

	public static void ClearStats()
	{
		NewLayers = 0;
		ReusedLayers = 0;
	}

	public static void PrintStats(string text)
	{
		FLog.Log(string.Format("{0}  newlayers:{1}  reusedlayers:{2}", text, NewLayers, ReusedLayers), true);
	}

	public void Get3DFacetRenderLayer(out FRenderLayerSpriteAtlas renderLayer, FAtlas atlas, FShader shader, int numberOfVerticesNeeded, bool usingUV2_3_4, F3DTracker recycleTracker, int recycleCounter)
	{
		if (recycleTracker != null)
		{
			FRenderLayerSpriteAtlas recycled = recycleTracker.GetFacetLayerByIndex(recycleCounter);
			if (recycled != null)
			{
				recycled.AddToWorld();
				recycled.Open3D();
				if (recycled.CanFitVertices(numberOfVerticesNeeded))
				{
					ReusedLayers++;
					renderLayer = recycled;
					return;
				}
			}
		}
		NewLayers++;
		FRenderLayerSpriteAtlas layer = new FRenderLayerSpriteAtlas(mStage, atlas, shader, usingUV2_3_4);
		layer.AddToWorld();
		layer.Open3D();
		renderLayer = layer;
	}

	public FRenderLayerGameObjects CreateRenderLayerGameObjects()
	{
		int count = mPreviousLiveLayers.Count;
		for (int i = 0; i < count; i++)
		{
			FRenderLayer layer = mPreviousLiveLayers[i];
			if (layer is FRenderLayerGameObjects)
			{
				layer.Clear();
				mPreviousLiveLayers.RemoveAt(i);
				mLiveLayers.Add(layer);
				return layer as FRenderLayerGameObjects;
			}
		}
		int cachedCount = mCachedLayers.Count;
		for (int j = 0; j < cachedCount; j++)
		{
			FRenderLayer layer2 = mCachedLayers[j];
			if (layer2 is FRenderLayerGameObjects)
			{
				layer2.Clear();
				mCachedLayers.RemoveAt(j);
				layer2.AddToWorld();
				mLiveLayers.Add(layer2);
				return layer2 as FRenderLayerGameObjects;
			}
		}
		FRenderLayerGameObjects newLayer = new FRenderLayerGameObjects(mStage);
		mLiveLayers.Add(newLayer);
		mAllLayers.Add(newLayer);
		newLayer.AddToWorld();
		return newLayer;
	}

	public void GetRenderLayerGameObjects(out FRenderLayerGameObjects renderLayer)
	{
		if (mTopLayer == null || !(mTopLayer is FRenderLayerGameObjects))
		{
			if (mTopLayer != null)
			{
				mTopLayer.Close();
			}
			mTopLayer = CreateRenderLayerGameObjects();
			mTopLayer.Open();
		}
		renderLayer = mTopLayer as FRenderLayerGameObjects;
	}

	public FShader GetDefaultShader()
	{
		return FShader.Normal;
	}

	public void Update()
	{
		int count = mLiveLayers.Count;
		for (int i = 0; i < count; i++)
		{
			mLiveLayers[i].Update();
		}
	}

	public void CloseCurrentLayer()
	{
		if (mTopLayer != null && mTopLayer.IsOpen)
		{
			mTopLayer.Close();
		}
		mTopLayer = null;
	}

	public void UnlinkAtlases(List<FAtlas> unloaded)
	{
		for (int i = mAllLayers.Count - 1; i >= 0; i--)
		{
			FRenderLayerSpriteAtlas layer = mAllLayers[i] as FRenderLayerSpriteAtlas;
			if (layer != null)
			{
				layer.UnlinkAtlases(unloaded);
			}
		}
	}

	public void CleanUnRenderedLayers()
	{
		mTopLayer = null;
		int frameCount = Time.frameCount;
		for (int i = mAllLayers.Count - 1; i >= 0; i--)
		{
			FRenderLayerSpriteAtlas layer = mAllLayers[i] as FRenderLayerSpriteAtlas;
			if (layer != null && frameCount - layer.LastFrameRendered > 10 && !layer.IsInOldList && layer.mRenderLayerTracking.Count == 0)
			{
				layer.ClearRLTracking();
				ReleaseLiveLayer(layer, false);
			}
		}
	}

	private void DebugListNames(List<FRenderLayer> renderLayers, string debug)
	{
		int frameCount = Time.frameCount;
		for (int i = 0; i < renderLayers.Count; i++)
		{
			FRenderLayer layer = renderLayers[i];
			if (frameCount - layer.LastFrameRendered > 3)
			{
				if (!layer.IsActive())
				{
					layer.GetRenderGO().name = debug + "_inactive_" + layer.mRenderLayerTracking.Count.ToString();
				}
				else
				{
					layer.GetRenderGO().name = debug + "_active_" + layer.mRenderLayerTracking.Count.ToString();
				}
			}
		}
	}
}
