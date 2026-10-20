using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagSpawn @ 0x1608d74..0x1609524, Ghidra vaddr).
public class FAnimTagSpawn : FAnimTag
{
	protected GameObject mPrefab;

	protected string mPrefabName;

	protected string mGOTree;

	protected float mLifespan = -1f;

	protected float mLifespanFadeTime;

	protected float mLifespanLingerTime = 1f;

	protected string mSpawnAt;

	protected float mUIZOffset = 1f;

	protected Vector3 mOffset;

	protected bool mUnique;

	protected bool mOrientAroundY;

	protected bool mOrientFacing;

	protected bool mIgnorePrefabRotation;

	protected bool mParentRotation;

	protected bool mBillboardTrackY;

	protected bool mVirtualAttachmentAutoKill = true;

	protected bool mRandomizeYRotation;

	protected float mMinScale = 1f;

	protected float mMaxScale;

	protected FAttachMode mAttachMode;

	protected float mOffsetTowardCamera;

	protected FColorXformMode mColorXformMode;

	public GameObject Prefab => mPrefab;

	public string PrefabName => mPrefabName;

	public string GOTree => mGOTree;

	public float Lifespan => mLifespan;

	public float LifespanFadeTime => mLifespanFadeTime;

	public float LifespanLingerTime => mLifespanLingerTime;

	public string SpawnAt => mSpawnAt;

	public float UIZOffset => mUIZOffset;

	public Vector3 Offset => mOffset;

	public bool Unique => mUnique;

	public bool OrientAroundY => mOrientAroundY;

	public bool OrientFacing => mOrientFacing;

	public bool IgnorePrefabRotation => mIgnorePrefabRotation;

	public bool ParentRotation => mParentRotation;

	public bool BillboardTrackY => mBillboardTrackY;

	public bool VirtualAttachmentAutoKill => mVirtualAttachmentAutoKill;

	public bool RandomizeYRotation => mRandomizeYRotation;

	public float MinScale => mMinScale;

	public float MaxScale => mMaxScale;

	public FAttachMode AttachMode => mAttachMode;

	public float OffsetTowardCamera => mOffsetTowardCamera;

	public FColorXformMode ColorXformMode => mColorXformMode;

	public FAnimTagSpawn()
		: base(AnimTagTypes.Spawn)
	{
	}

	public override string ToString()
	{
		string text = "SpawnTag: ";
		if (mPrefabName != null)
		{
			text = text + mPrefabName + " ";
		}
		if (mGOTree != null)
		{
			text += mGOTree;
		}
		return text;
	}

	public override bool LoadXml(XmlElement root)
	{
		base.LoadXml(root);
		string attrib = root.GetAttrib("Prefab");
		if (attrib != null)
		{
			mPrefab = FUtil.LoadPrefab(attrib);
			if (mPrefab == null)
			{
				FLog.UnityError("could not load prefab: " + attrib);
			}
			mPrefabName = attrib;
		}
		mGOTree = root.GetAttrib("GOTree");
		if (mPrefab == null && mGOTree == null)
		{
			return false;
		}
		mLifespan = root.GetAttrib("Lifespan", mLifespan);
		mLifespanFadeTime = root.GetAttrib("LifespanFadeTime", mLifespanFadeTime);
		mLifespanLingerTime = root.GetAttrib("LifespanLingerTime", mLifespanLingerTime);
		mSpawnAt = root.GetAttrib("SpawnAt", (string)null);
		mUIZOffset = root.GetAttrib("UIZOffset", mUIZOffset);
		mOffset = root.GetAttrib("Offset", mOffset);
		mUnique = root.GetAttrib("Unique", mUnique);
		mOrientAroundY = root.GetAttrib("OrientAroundY", mOrientAroundY);
		mOrientFacing = root.GetAttrib("OrientFacing", mOrientFacing);
		mIgnorePrefabRotation = root.GetAttrib("IgnorePrefabRotation", mIgnorePrefabRotation);
		mParentRotation = root.GetAttrib("ParentRotation", mParentRotation);
		mBillboardTrackY = root.GetAttrib("BillboardTrackY", mBillboardTrackY);
		mVirtualAttachmentAutoKill = root.GetAttrib("VirtualAttachmentAutoKill", mVirtualAttachmentAutoKill);
		mRandomizeYRotation = root.GetAttrib("RandomizeYRotation", mRandomizeYRotation);
		mMinScale = root.GetAttrib("MinScale", mMinScale);
		mMaxScale = root.GetAttrib("MaxScale", mMaxScale);
		mAttachMode = FAttachMode.None;
		FEnum.TryParse(root.GetAttrib("AttachMode"), ref mAttachMode);
		FEnum.TryParse(root.GetAttrib("ColorXformMode"), ref mColorXformMode);
		mOffsetTowardCamera = root.GetAttrib("OffsetTowardCamera", mOffsetTowardCamera);
		if (!mUnique)
		{
			if (!(mLifespan < float.Epsilon))
			{
				return true;
			}
			FLog.UnityWarning("AnimTagSpawn with unique off requires a lifespan to be defined, spawning: " + attrib);
			mLifespan = 10f;
		}
		return true;
	}
}
