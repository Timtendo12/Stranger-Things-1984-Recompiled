using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FRenderLayer @ 0x169172c..0x16942b8, Ghidra vaddr).
public class FRenderLayer
{
	protected GameObject mGameObject;

	protected Transform mTransform;

	protected FStage mStage;

	protected bool mNeedsRecalculateBoundsIfTransformed;

	protected int mDepth = -1;

	public int BatchIndex;

	protected bool mOpen;

	public bool Destroyed;

	public int LastFrameEdited = -1;

	public int LastFrameRendered = -1;

	public int LastFrameAccessed = -1;

	public bool IsInOldList;

	public List<FNode> mBoundNodes = new List<FNode>();

	public bool mLayerDirty;

	public bool mLayerDirtyHoldOneFrame;

	public bool UseRootMotion;

	public FNode RootMotionParent;

	public Vector3 RootMotionPosition = Vector3.zero;

	public bool SortingDirty;

	public bool ShouldSortByZ;

	public int mGeneration;

	public List<FNode.RLTracking> mRenderLayerTracking = new List<FNode.RLTracking>();

	public bool mLayerMeshDirty;

	public static int DebugStat_RLCreated;

	public static int DebugStat_RLDestoryed;

	public int VirtualCloseFrame;

	public bool IsOpen => mOpen;

	public int depth
	{
		get
		{
			return mDepth;
		}
		set
		{
			if (mDepth != value)
			{
				mDepth = value;
				UpdateRenderQueue();
			}
		}
	}

	public virtual int NumDepthsUsed => 1;

	public string Name => mGameObject.name;

	public void DebugSetObjectName(string name)
	{
		mGameObject.name = name;
	}

	public void TempClearRootPosition()
	{
		if (mGameObject != null)
		{
			mGameObject.transform.position = Vector3.zero;
		}
	}

	public void SetRootPosition(Vector3 pos)
	{
		RootMotionPosition = pos;
		if (mGameObject != null)
		{
			mGameObject.transform.position = pos;
			RootMotionPosition = pos;
		}
	}

	public void SetLayerDirty()
	{
		mLayerDirty = true;
		mLayerDirtyHoldOneFrame = true;
		for (int i = 0; i < mBoundNodes.Count; i++)
		{
			mBoundNodes[i].mRenderLayerDirty = true;
		}
	}

	public bool NeedsRerender()
	{
		return false;
	}

	public FRenderLayer(FStage stage)
	{
		mStage = stage;
		DebugStat_RLCreated++;
		mGameObject = new GameObject("FRenderLayer (" + mStage.Name + ")");
		mTransform = mGameObject.transform;
		mTransform.parent = Futile.Instance.gameObject.transform;
		mGameObject.layer = 31;
		mGameObject.SetActive(false);
	}

	public virtual void UpdateRenderQueue()
	{
	}

	public virtual void Destroy()
	{
		if (!Destroyed)
		{
			DebugStat_RLDestoryed++;
			ClearRLTracking();
			mBoundNodes.Clear();
		}
		Destroyed = true;
		Object.Destroy(mGameObject);
	}

	public virtual void UpdateTransform()
	{
		mTransform.position = mStage.Transform.Position;
		mTransform.rotation = mStage.Transform.Rotation;
		mTransform.localScale = mStage.Transform.LocalScale;
		if (mNeedsRecalculateBoundsIfTransformed)
		{
			mNeedsRecalculateBoundsIfTransformed = false;
			RecalculateBounds();
		}
	}

	public virtual void UpdateTexture()
	{
	}

	public virtual void UpdateTextureIfNeeded()
	{
	}

	protected virtual void RecalculateBounds()
	{
	}

	public virtual void AddToWorld()
	{
		mGameObject.SetActive(true);
	}

	public virtual void RemoveFromWorld()
	{
		mGameObject.SetActive(false);
		Clear();
	}

	public bool IsActive()
	{
		if (mGameObject == null)
		{
			return false;
		}
		return mGameObject.activeSelf;
	}

	public virtual void Clear()
	{
	}

	public virtual string ComputeName()
	{
		if (!mGameObject.activeSelf)
		{
			return string.Format("FRenderLayer X ({0})", mStage.Name);
		}
		return string.Format("FRenderLayer {0} ({1})", mDepth, mStage.Name);
	}

	[Conditional("UPDATE_NAME")]
	public virtual void UpdateName()
	{
		string names = "";
		foreach (FNode node in mBoundNodes)
		{
			names += node.Name;
		}
		names = names.Substring(0, Mathf.Min(names.Length, 60));
		mGameObject.name = ComputeName() + names;
	}

	public virtual void Open()
	{
	}

	public virtual void Open3D()
	{
	}

	public virtual void Close()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void SetCameraLayer(int layer)
	{
	}

	public GameObject GetRenderGO()
	{
		return mGameObject;
	}

	public void SetBoundNodeSortingDirty()
	{
		SortingDirty = true;
		for (int i = 0; i < mBoundNodes.Count; i++)
		{
			FNode node = mBoundNodes[i];
			if (node != null)
			{
				FRenderLayer other = node.GetLastRenderLayer();
				if (other != null && other != this)
				{
					other.SetBoundNodeSortingDirty();
				}
			}
		}
	}

	public void SetLayerMeshDirty()
	{
		if (!FStage.HackOldMode)
		{
			mLayerMeshDirty = true;
		}
	}

	public void UpdateLayerMeshIfNeeded()
	{
		if (mLayerMeshDirty)
		{
			mLayerMeshDirty = false;
			UpdateLayerMesh();
		}
	}

	protected virtual void UpdateLayerMesh()
	{
	}

	public bool CanRLShare()
	{
		if (mRenderLayerTracking.Count == 1 && mRenderLayerTracking[0].mNode.NoRenderLayerSharing)
		{
			return false;
		}
		return true;
	}

	public void ClearRLTracking()
	{
		for (int i = 0; i < mRenderLayerTracking.Count; i++)
		{
			FNode.RLTracking tracking = mRenderLayerTracking[i];
			if (tracking.mNode != null)
			{
				tracking.mNode.RemoveTrackingReference(tracking);
			}
		}
		mRenderLayerTracking.Clear();
	}

	public void DumpUsedImages(FAtlas atlas)
	{
	}
}
