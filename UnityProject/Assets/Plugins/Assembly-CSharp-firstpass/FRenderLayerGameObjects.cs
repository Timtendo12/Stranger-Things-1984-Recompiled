using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FRenderLayerGameObjects @ 0x1693074..0x16947e8, Ghidra vaddr).
public class FRenderLayerGameObjects : FRenderLayer
{
	public HashSet<FGameObjectNode> mGameObjectNodes = new HashSet<FGameObjectNode>();

	public override int NumDepthsUsed => mGameObjectNodes.Count;

	public FRenderLayerGameObjects(FStage stage)
		: base(stage)
	{
		UpdateTransform();
	}

	public override void Clear()
	{
		mGameObjectNodes.Clear();
	}

	public override void Open()
	{
		Clear();
		mGameObject.layer = 31;
		Futile.AddCurrentCameraRenderLayer(this);
	}

	public override void Close()
	{
	}

	public override void Update()
	{
	}

	protected override void RecalculateBounds()
	{
	}

	public override string ComputeName()
	{
		if (!mGameObject.activeSelf)
		{
			return string.Format("FRenderLayer X ({0}) (GameObjects)", mStage.Name);
		}
		return string.Format("FRenderLayer {0} ({1}) (GameObjects)", mDepth, mStage.Name);
	}

	public override void UpdateRenderQueue()
	{
		int index = 0;
		foreach (FGameObjectNode node in mGameObjectNodes)
		{
			UpdateObjectRenderQueue(node, index, 31);
			index++;
		}
	}

	protected void UpdateObjectRenderQueue(FGameObjectNode obj, int index, int newLayer)
	{
		obj.SetMaterialRenderQueue(index + mDepth + 3000, newLayer);
	}

	public void Add(FGameObjectNode gameObjectNode, Color color, Color tintColor)
	{
		mGameObjectNodes.Add(gameObjectNode);
		gameObjectNode.SetMaterialMisc(mGameObjectNodes.Count + mDepth + 3000, color, tintColor, 31);
	}

	public override void SetCameraLayer(int layer)
	{
		foreach (FGameObjectNode node in mGameObjectNodes)
		{
			node.SetCameraLayer(layer);
		}
	}
}
