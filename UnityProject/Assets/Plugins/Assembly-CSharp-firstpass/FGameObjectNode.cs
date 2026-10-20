using System.Collections.Generic;
using UnityEngine;

public class FGameObjectNode : FNode
{
	protected Vector3 mAnchor;

	protected Vector3 mCanonicalSize;

	protected float mCanonicalSizeMax;

	protected float mLocalScaleX;

	protected float mLocalScaleY;

	protected GameObject mGameObject;

	protected GameObject mHolderObject;

	protected GameObject mAnchorObject;

	protected GameObject mBasePrefab;

	protected FRenderLayerGameObjects mRenderLayer;

	protected FAnimDef mAnimDef;

	protected List<Material> mMaterials;

	protected List<GameObject> mObjects;

	protected int mLayer;

	protected int mRenderQueue;

	protected Color32 mMainColor;

	protected Color32 mTintColor;

	public virtual Vector3 Anchor
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public virtual Vector3 CanonicalSize
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public override float Width
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public override float Height
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public FAnimDef AnimDef
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public GameObject GameObj => null;

	public GameObject BasePrefab => null;

	public GameObject AnchorObject => null;

	public FGameObjectNode()
	{
	}

	public FGameObjectNode(string prefabName)
	{
	}

	public FGameObjectNode(GameObject gameObj, bool clone, GameObject basePrefab)
	{
	}

	~FGameObjectNode()
	{
	}

	public void SetGameObject(GameObject gameObj, bool clone)
	{
	}

	public override bool CanCull()
	{
		return false;
	}

	public override void RedrawInternal(bool shouldForceDirty, bool shouldUpdateDepth, bool wasMatrixDirty, bool wasAlphaDirty)
	{
	}

	public override void HandleAddedToStage()
	{
	}

	public override void HandleRemovedFromStage()
	{
	}

	public override void SelfResizeExtents()
	{
	}

	public override void Dispose(bool disposing)
	{
	}

	public override float PlayAnim(string name, bool recursive)
	{
		return 0f;
	}

	public override float GetAnimLength(string name, bool recursive)
	{
		return 0f;
	}

	public override void SetAnimSpeed(float speed, bool recursive)
	{
	}

	public override FAnimState PlayAnimType(int animType, float animSpeed, bool recursive, IAnimTagResponder responder)
	{
		return null;
	}

	public override void SetAnimatorBool(string paramName, bool paramValue, bool recursive = true)
	{
	}

	public override void SetAnimatorFloat(string paramName, float paramValue, bool recursive = true)
	{
	}

	public override void SetAnimatorInt(string paramName, int paramValue, bool recursive = true)
	{
	}

	public override string GetStringDetails()
	{
		return null;
	}

	public int GetCurrentAnimTypeID()
	{
		return 0;
	}

	public void SetCameraLayer(int newLayer)
	{
	}

	public void SetMaterialMisc(int renderQueue, Color color, Color tintColor, int newLayer = -1)
	{
	}

	public void SetMaterialRenderQueue(int renderQueue, int newLayer = -1)
	{
	}

	public bool SetAnchorFromBonePosition(string boneName)
	{
		return false;
	}
}
