using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (F3DTracker @ 0x1681690..0x1682dd0, Ghidra vaddr).
public class F3DTracker : MonoBehaviour
{
	private class FontUpdateTracker
	{
		public FFont mFont;

		public int Size;

		public int LastVersion;

		public int LastGenericVersion;

		public int LastGlobalCounter;

		public Dictionary<FFont.UnityCharacterFontTracking, int> UpdateCounts = new Dictionary<FFont.UnityCharacterFontTracking, int>();

		public bool CheckDirty(bool init = false)
		{
			if (!mFont.mIsDynamic)
			{
				return false;
			}
			bool dirty = false;
			for (int i = 0; i < mFont.mThisFontsTrackers.Count; i++)
			{
				FFont.UnityCharacterFontTracking tracking = mFont.mThisFontsTrackers[i];
				mFont.PerFrameRequestCharacters(tracking);
				int count = 0;
				if (!UpdateCounts.TryGetValue(tracking, out count))
				{
					UpdateCounts[tracking] = tracking.TextureRebuildCount;
					dirty = true;
				}
				if (tracking.TextureRebuildCount != count)
				{
					UpdateCounts[tracking] = tracking.TextureRebuildCount;
					dirty = true;
				}
			}
			return dirty;
		}

		public void RegisterCallbacksFor3DNodes()
		{
			for (int i = 0; i < mFont.mThisFontsTrackers.Count; i++)
			{
				mFont.mThisFontsTrackers[i].RegisterCallbacksFor3DNodes();
			}
		}
	}

	public bool Recycle;

	public int FutileDepth;

	public F3DTracker TopTracker;

	public int OffsetDepth;

	public int LocalDepth;

	public List<int> RegisteredDepths = new List<int>();

	private bool mDirty;

	public List<F3DTracker> ChildTrackers = new List<F3DTracker>();

	public List<F3DTracker> ToRecycle = new List<F3DTracker>();

	public List<GameObject> RenderObjects = new List<GameObject>();

	public List<FRenderLayerSpriteAtlas> RenderLayers = new List<FRenderLayerSpriteAtlas>();

	public List<int> RecycleAddress = new List<int>();

	public List<int> LastRecycleAddress = new List<int>();

	private List<FontUpdateTracker> FontTrackers = new List<FontUpdateTracker>();

	private bool RenderLayersDestroyed;

	public void PrepForRecycle()
	{
		Recycle = true;
		RegisteredDepths.Clear();
		FontTrackers.Clear();
		ToRecycle.AddRange(ChildTrackers);
	}

	public void Recycling(F3DTracker tracker)
	{
		ToRecycle.Remove(tracker);
	}

	public void CleanUnRecycled()
	{
		for (int i = 0; i < ToRecycle.Count; i++)
		{
			F3DTracker tracker = ToRecycle[i];
			if (tracker == this)
			{
				continue;
			}
			if (tracker != null && tracker.gameObject != null)
			{
				ChildTrackers.Remove(tracker);
				Object.Destroy(tracker.gameObject);
			}
			else
			{
				// Original builds a debug string here (result unused).
				ChildTrackers.Remove(tracker);
			}
		}
		ToRecycle.Clear();
	}

	public void Start()
	{
		if (TopTracker != null)
		{
			UpdateDepth();
		}
	}

	public F3DTracker FindMatchingExistingTracker(FNode toMatch)
	{
		for (int i = 0; i < ChildTrackers.Count; i++)
		{
			F3DTracker tracker = ChildTrackers[i];
			if (toMatch.HasSameRecycleSignature(tracker.RecycleAddress))
			{
				if (tracker.RenderLayers.Count == 0)
				{
					return null;
				}
				return tracker;
			}
		}
		return null;
	}

	public void SetRecycleAddress(List<int> address)
	{
		LastRecycleAddress = RecycleAddress;
		RecycleAddress = new List<int>(address);
	}

	private void OnDestroy()
	{
		RecursiveClearRenderLayers(this);
	}

	private void RecursiveClearRenderLayers(F3DTracker tracker)
	{
		if (tracker == null || tracker.RenderLayersDestroyed)
		{
			return;
		}
		for (int i = 0; i < tracker.RenderLayers.Count; i++)
		{
			FRenderLayerSpriteAtlas layer = tracker.RenderLayers[i];
			if (layer != null)
			{
				layer.Destroy();
			}
		}
		// Original sets the flag on the receiver, not on the visited tracker.
		RenderLayersDestroyed = true;
		for (int j = 0; j < tracker.ChildTrackers.Count; j++)
		{
			F3DTracker child = tracker.ChildTrackers[j];
			if (child != tracker)
			{
				RecursiveClearRenderLayers(child);
			}
		}
	}

	public void UpdateDepths(int baseOffset = 3000)
	{
		UpdateDepth(baseOffset);
		for (int i = 0; i < ChildTrackers.Count; i++)
		{
			if (ChildTrackers[i] != null)
			{
				ChildTrackers[i].UpdateDepth(baseOffset);
			}
		}
	}

	private void UpdateDepth(int baseOffset = 3000)
	{
		LocalDepth = TopTracker.GetLocalDepth(FutileDepth, baseOffset);
		for (int i = 0; i < RenderLayers.Count; i++)
		{
			FRenderLayerSpriteAtlas layer = RenderLayers[i];
			if (layer != null)
			{
				Material material = layer.GetUniqueMaterial();
				if (material != null)
				{
					material.renderQueue = LocalDepth;
				}
			}
		}
	}

	public bool CheckForFontAtlasChanges3()
	{
		bool changed = false;
		for (int i = 0; i < FontTrackers.Count; i++)
		{
			changed = changed || FontTrackers[i].CheckDirty();
		}
		return changed;
	}

	public FRenderLayerSpriteAtlas GetFacetLayerByIndex(int index)
	{
		if (index < RenderLayers.Count)
		{
			return RenderLayers[index];
		}
		return null;
	}

	public void AddRenderLayer(GameObject renderObject, FRenderLayerSpriteAtlas renderLayer)
	{
		if (!RenderObjects.Contains(renderObject))
		{
			RenderObjects.Add(renderObject);
		}
		if (!RenderLayers.Contains(renderLayer))
		{
			RenderLayers.Add(renderLayer);
		}
		renderObject.transform.parent = base.transform;
		renderObject.transform.position = Vector3.zero;
	}

	public void RegisterDepth(int futileDepth)
	{
		if (futileDepth != 0)
		{
			RegisteredDepths.Add(futileDepth);
			mDirty = true;
		}
	}

	public void RegisterTracker(F3DTracker tracker)
	{
		if (!ChildTrackers.Contains(tracker))
		{
			ChildTrackers.Add(tracker);
		}
	}

	public void RegisterLabel(FLabel label)
	{
		FontUpdateTracker tracker = new FontUpdateTracker();
		tracker.mFont = label.Font;
		FFont font = label.Font;
		tracker.LastGenericVersion = font.CrossSizeAtlasVersion;
		tracker.LastVersion = font.GetAlasVersion((int)label.PointSize);
		tracker.Size = (int)label.PointSize;
		tracker.CheckDirty();
		tracker.RegisterCallbacksFor3DNodes();
		FontTrackers.Add(tracker);
	}

	public int GetLocalDepth(int futileDepth, int baseOffset)
	{
		if (mDirty)
		{
			mDirty = false;
			RegisteredDepths.Sort();
		}
		// The original has no bounds check: an unregistered depth throws ArgumentOutOfRangeException.
		for (int i = 0; ; i++)
		{
			if (RegisteredDepths[i] == futileDepth)
			{
				return OffsetDepth + baseOffset + i;
			}
		}
	}

	public void SetShaderValueUnique(string key, float value)
	{
		for (int i = 0; i < RenderLayers.Count; i++)
		{
			FRenderLayerSpriteAtlas layer = RenderLayers[i];
			if (layer != null)
			{
				Material material = layer.GetUniqueMaterial();
				if (material != null)
				{
					material.SetFloat(key, value);
				}
			}
		}
		for (int j = 0; j < ChildTrackers.Count; j++)
		{
			F3DTracker child = ChildTrackers[j];
			if (child != this && child != null)
			{
				child.SetShaderValueUnique(key, value);
			}
		}
	}
}
