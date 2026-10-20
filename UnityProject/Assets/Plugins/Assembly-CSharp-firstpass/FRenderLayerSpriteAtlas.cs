using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Rewritten from libil2cpp.so 1.0.605 (FRenderLayerSpriteAtlas @ 0x1691e30..0x1696304, Ghidra vaddr).
public class FRenderLayerSpriteAtlas : FRenderLayer
{
	protected FAtlas mAtlas;

	protected FShader mShader;

	protected MeshFilter mMeshFilter;

	protected MeshRenderer mMeshRenderer;

	protected Mesh mMesh;

	protected List<Vector3> mVertices = new List<Vector3>();

	protected List<int> mTriangles = new List<int>();

	protected List<Vector2> mUVs = new List<Vector2>();

	protected List<Color> mColors = new List<Color>();

	protected List<Vector4> mColors2 = new List<Vector4>();

	protected List<Vector2> mUV2 = new List<Vector2>();

	protected List<Vector2> mUV3 = new List<Vector2>();

	protected List<Vector4> mUV4 = new List<Vector4>();

	public bool mUsingUV2_3_4;

	private Dictionary<string, Vector4> mLastMaterialValues = new Dictionary<string, Vector4>();

	public int mAtlasGeneration;

	private Material mCachedMaterial;

	public bool DisconnectFrom2D;

	public List<Vector3> vertices => mVertices;

	public List<Vector2> uvs => mUVs;

	public List<Color> colors => mColors;

	public List<Vector4> colors2 => mColors2;

	public List<Vector2> uv2 => mUV2;

	public List<Vector2> uv3 => mUV3;

	public List<Vector4> uv4 => mUV4;

	public int indexCount => mTriangles.Count;

	public Material GetSharedMaterial()
	{
		if (mMeshRenderer != null)
		{
			return mMeshRenderer.sharedMaterial;
		}
		return null;
	}

	public void HackSharedTexture()
	{
		if (mMeshRenderer != null)
		{
			Material material = mMeshRenderer.sharedMaterial;
			Texture texture = material.mainTexture;
			material.mainTexture = null;
			material.mainTexture = texture;
		}
	}

	public Material GetUniqueMaterial()
	{
		if (mMeshRenderer != null)
		{
			return mMeshRenderer.material;
		}
		return null;
	}

	public FRenderLayerSpriteAtlas(FStage stage, FAtlas atlas, FShader shader, bool usingUV2_3_4)
		: base(stage)
	{
		mAtlas = atlas;
		mShader = shader;
		mUsingUV2_3_4 = usingUV2_3_4;
		BatchIndex = shader.Index + atlas.Index * 10000;
		mMeshFilter = mGameObject.AddComponent<MeshFilter>();
		mMeshRenderer = mGameObject.AddComponent<MeshRenderer>();
		mMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		mMeshRenderer.receiveShadows = false;
		mMesh = mMeshFilter.mesh;
		mMesh.MarkDynamic();
		bool requiredReload;
		Material material;
		if (mShader.CustomMaterial == null)
		{
			material = new Material(mShader.Shader);
			material.mainTexture = mAtlas.GetOrLoadTexture(out requiredReload);
		}
		else if (mShader.UpdateCustomMaterialWithAtlas)
		{
			material = new Material(mShader.CustomMaterial);
			material.mainTexture = mAtlas.GetOrLoadTexture(out requiredReload);
		}
		else
		{
			material = mShader.CustomMaterial;
		}
		mMeshRenderer.GetComponent<Renderer>().sharedMaterial = material;
		UpdateTransform();
	}

	public bool LogMaterialValue(string name, Vector4 value)
	{
		Vector4 lastValue;
		if (mLastMaterialValues.TryGetValue(name, out lastValue) && lastValue == value)
		{
			return false;
		}
		mLastMaterialValues[name] = value;
		return true;
	}

	public override void Destroy()
	{
		Object.Destroy(mMeshRenderer.material);
		Object.Destroy(mMesh);
		Object.Destroy(mMeshFilter);
		Object.Destroy(mMeshRenderer);
		base.Destroy();
	}

	public void DebugMeTrackingTest()
	{
		// Original walks the tracking list touching each entry's node (debug null-check only).
		for (int i = 0; i < mRenderLayerTracking.Count; i++)
		{
			FNode node = mRenderLayerTracking[i].mNode;
			if (node == null)
			{
				throw new System.NullReferenceException();
			}
		}
	}

	protected void ClearLists()
	{
		mVertices.Clear();
		mTriangles.Clear();
		mUVs.Clear();
		mColors.Clear();
		mColors2.Clear();
		mUV2.Clear();
		mUV3.Clear();
		mUV4.Clear();
	}

	public override void UpdateTextureIfNeeded()
	{
		if (FAtlasManager.AtlasUnloadingEnabled && mAtlas != null)
		{
			if (mAtlas.mGeneration != mAtlasGeneration)
			{
				UpdateTexture();
			}
			mAtlas.mLastFrameUsed = Time.frameCount;
			mAtlas.mLastUseTime = Time.time;
		}
	}

	public void UnlinkAtlases(List<FAtlas> unlink)
	{
		if (unlink.Contains(mAtlas) && !mAtlas.IsLoaded)
		{
			Material material = mMeshRenderer.GetComponent<Renderer>().sharedMaterial;
			if (material != null && material.mainTexture != null)
			{
				material.mainTexture = null;
			}
		}
	}

	public override void UpdateTexture()
	{
		mAtlasGeneration = mAtlas.mGeneration;
		mCachedMaterial = null;
		bool requiredReload;
		if (!mAtlas.IsLoaded && mAtlas.GetOrLoadTexture(out requiredReload) == null)
		{
			return;
		}
		if (!mAtlas.IsLoaded || mShader.CustomMaterial != null)
		{
			return;
		}
		Material material = mMeshRenderer.GetComponent<Renderer>().sharedMaterial;
		if (material.mainTexture == mAtlas.Texture)
		{
			return;
		}
		Material newMaterial = new Material(mShader.Shader);
		newMaterial.mainTexture = mAtlas.GetOrLoadTexture(out requiredReload);
		mMeshRenderer.GetComponent<Renderer>().sharedMaterial = newMaterial;
	}

	public override void Open()
	{
		if (mOpen)
		{
			FLog.UnityError("FRenderLayerSpriteAtlas -- nested open call, which doesn't make sense");
			return;
		}
		mOpen = true;
		ClearLists();
		mGameObject.layer = 31;
		Futile.AddCurrentCameraRenderLayer(this);
	}

	public override void Open3D()
	{
		mOpen = true;
		ClearLists();
	}

	public override void Close()
	{
		if (mOpen)
		{
			mOpen = false;
			UpdateLayerMesh();
			if (!mLayerDirtyHoldOneFrame)
			{
				mLayerDirty = false;
			}
			mLayerDirtyHoldOneFrame = false;
			return;
		}
		FLog.UnityError("FRenderLayerSpriteAtlas -- trying to close when not open");
	}

	public override void Update()
	{
	}

	public void BakeMesh3D()
	{
		UpdateLayerMesh();
		DisconnectFrom2D = true;
	}

	public void HackUpdateMeshProperties()
	{
		if (!FStage.HackOldMode)
		{
			UpdateLayerMesh();
		}
	}

	protected override void UpdateLayerMesh()
	{
		mMesh.Clear();
		mMesh.SetVertices(mVertices);
		mMesh.SetUVs(0, mUVs);
		if (mUsingUV2_3_4)
		{
			mMesh.SetUVs(1, mUV2);
			mMesh.SetUVs(2, mUV3);
			mMesh.SetUVs(3, mUV4);
		}
		mMesh.SetColors(mColors);
		mMesh.SetTangents(mColors2);
		mMesh.SetTriangles(mTriangles, 0);
	}

	protected override void RecalculateBounds()
	{
		mMesh.RecalculateBounds();
	}

	public override string ComputeName()
	{
		if (!mGameObject.activeSelf)
		{
			return string.Format("FRenderLayer X ({0}) ({1} {2})", mStage.Name, mAtlas.Name, mShader.Name);
		}
		return string.Format("FRenderLayer {0} ({1}) [{2}] ({3} {4})", mDepth, mStage.Name, mTriangles.Count / 3, mAtlas.Name, mShader.Name);
	}

	public override void UpdateRenderQueue()
	{
		if (mCachedMaterial == null)
		{
			mCachedMaterial = mMeshRenderer.GetComponent<Renderer>().material;
		}
		mCachedMaterial.renderQueue = mDepth + 3000;
	}

	public override void SetCameraLayer(int layer)
	{
		mGameObject.layer = layer;
	}

	public bool CanFitTriangles(int numberNeeded)
	{
		return CanFitVertices(mVertices.Count + numberNeeded * 3);
	}

	public bool CanFitQuads(int numberNeeded)
	{
		return CanFitVertices(mVertices.Count + numberNeeded * 4);
	}

	public bool CanFitVertices(int verticesNeeded)
	{
		return mVertices.Count + verticesNeeded < 65536;
	}

	public void AddSimpleTriList(int startVertIndex, int count)
	{
		FAssert.That(count % 3 == 0, "Vertex count for adding triangles is not divisible by three");
		for (int i = 0; i < count; i++)
		{
			mTriangles.Add(startVertIndex + i);
		}
	}

	public void AddSimpleTriList(int startVertIndex)
	{
		AddSimpleTriList(startVertIndex, mVertices.Count - startVertIndex);
	}

	public void AddSimpleQuadList(int startVertIndex, int count)
	{
		FAssert.That((count & 3) == 0, "Vertex count for adding quad is not divisible by four");
		int quads = count / 4;
		for (int i = 0; i < quads; i++)
		{
			mTriangles.Add(startVertIndex);
			mTriangles.Add(startVertIndex + 1);
			mTriangles.Add(startVertIndex + 2);
			mTriangles.Add(startVertIndex);
			mTriangles.Add(startVertIndex + 2);
			mTriangles.Add(startVertIndex + 3);
			startVertIndex += 4;
		}
	}

	public void AddSimpleQuadList(int startVertIndex)
	{
		AddSimpleQuadList(startVertIndex, mVertices.Count - startVertIndex);
	}

	public void RemoveByTracking(FNode.RLTracking tracking)
	{
		int startingVert = tracking.mStartingVert;
		int numTris = tracking.mNumTris;
		int numVerts = tracking.mNumVerts;
		int startingIndex = tracking.mStartingIndex;
		if (mRenderLayerTracking.Count == 1 && mRenderLayerTracking[0] == tracking)
		{
			ClearLists();
		}
		else
		{
			RemoveGeometry(startingVert, numVerts, startingIndex, numTris);
		}
		int index = mRenderLayerTracking.IndexOf(tracking);
		if (index < 0)
		{
			FAssert.Fail("invalid tracking index");
			return;
		}
		for (int i = index + 1; i < mRenderLayerTracking.Count; i++)
		{
			FNode.RLTracking later = mRenderLayerTracking[i];
			later.mStartingVert -= numVerts;
			later.mStartingIndex -= numTris * 3;
		}
		mRenderLayerTracking.RemoveAt(index);
		if (!FStage.HackOldMode)
		{
			mLayerMeshDirty = true;
		}
	}

	public void RemoveSimpleQuadList(int startVertIndex, int numQuads)
	{
		if (startVertIndex < 0)
		{
			return;
		}
		int numVerts = numQuads * 4;
		int end = numVerts + startVertIndex;
		if (mVertices.Count < end)
		{
			return;
		}
		mVertices.RemoveRange(startVertIndex, numVerts);
		mUVs.RemoveRange(startVertIndex, numVerts);
		if (mColors != null && end <= mColors.Count)
		{
			mColors.RemoveRange(startVertIndex, numVerts);
		}
		if (mColors2 != null && end <= mColors2.Count)
		{
			mColors2.RemoveRange(startVertIndex, numVerts);
		}
		if (mUsingUV2_3_4)
		{
			mUV2.RemoveRange(startVertIndex, numVerts);
			mUV3.RemoveRange(startVertIndex, numVerts);
			mUV4.RemoveRange(startVertIndex, numVerts);
		}
		int triangleIndex = startVertIndex / 4 * 6;
		mTriangles.RemoveRange(triangleIndex, numQuads * 6);
		for (int i = triangleIndex; i < mTriangles.Count; i++)
		{
			mTriangles[i] = mTriangles[i] - numQuads * 4;
		}
	}

	public void RemoveGeometry(int startVertIndex, int verts, int triangleIndex, int numTriangles)
	{
		if (mVertices == null || startVertIndex < 0)
		{
			return;
		}
		int end = verts + startVertIndex;
		if (mVertices.Count < end)
		{
			return;
		}
		mVertices.RemoveRange(startVertIndex, verts);
		mUVs.RemoveRange(startVertIndex, verts);
		if (mColors != null && end <= mColors.Count)
		{
			mColors.RemoveRange(startVertIndex, verts);
		}
		if (mColors2 != null && end <= mColors2.Count)
		{
			mColors2.RemoveRange(startVertIndex, verts);
		}
		if (mUsingUV2_3_4)
		{
			mUV2.RemoveRange(startVertIndex, verts);
			mUV3.RemoveRange(startVertIndex, verts);
			mUV4.RemoveRange(startVertIndex, verts);
		}
		mTriangles.RemoveRange(triangleIndex, numTriangles * 3);
		for (int i = triangleIndex; i < mTriangles.Count; i++)
		{
			mTriangles[i] = mTriangles[i] - verts;
		}
	}

	public Mesh GetMesh()
	{
		return mMesh;
	}
}
