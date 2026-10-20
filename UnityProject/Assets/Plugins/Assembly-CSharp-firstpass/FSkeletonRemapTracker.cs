using System.Collections.Generic;
using UnityEngine;

public class FSkeletonRemapTracker : MonoBehaviour
{
	protected Dictionary<Transform, Transform> mRigidAttachments;

	protected List<FRemapSkinEntry> mSkinEntries;

	protected HashSet<string> mTags;

	public void TrackSkin(SkinnedMeshRenderer skin)
	{
	}

	public void TrackRigidAttachment(Transform attachment, Transform originalParent)
	{
	}

	public void Detach(GameObject ignoreCollisionsObject = null, int newPhysicsLayer = -1, Transform newParent = null)
	{
	}

	public void AddTag(string tag)
	{
	}

	public bool HasTag(string tag)
	{
		return false;
	}

	public bool HasAnyTag(List<string> tags)
	{
		return false;
	}
}
