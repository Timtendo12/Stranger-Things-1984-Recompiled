using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FImpactEffect @ 0x1787800..0x1788a30, Ghidra vaddr).
public class FImpactEffect
{
	protected string mName;

	protected string mSoundSet;

	protected string mItemType;

	protected FAnimTagSpawn mSpawn;

	protected FCameraShakeParams mCameraShake;

	public string Name => mName;

	public string SoundSet => mSoundSet;

	public string ItemType => mItemType;

	public FAnimTagSpawn Spawn => mSpawn;

	public static FVisObject ProcessSpawnTag(FAnimTagSpawn spawn, Vector3 pos, Quaternion rotation)
	{
		// The shipped body is empty (returns null).
		return null;
	}

	public FVisObject SpawnAtPosition(Vector3 pos, bool fakeCameraShakePos = false)
	{
		if (FCoreUtil.Valid(mSoundSet))
		{
			FApp.SoundManager.PlaySoundSetAtPoint(mSoundSet, pos);
		}
		if (mCameraShake != null && fakeCameraShakePos)
		{
			if (FApp.World != null)
			{
				Camera cam = FApp.World.Camera;
				pos = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cam.nearClipPlane));
			}
			mCameraShake.Apply(pos);
		}
		return null;
	}

	public FVisObject SpawnAtPosition(Vector3 pos, Quaternion rotation)
	{
		if (FCoreUtil.Valid(mSoundSet))
		{
			FApp.SoundManager.PlaySoundSetAtPoint(mSoundSet, pos);
		}
		return null;
	}

	public FNode SpawnNode(FNode node, float lifespan = -1f)
	{
		if (mSpawn == null)
		{
			return null;
		}
		FGameObjectNode fGameObjectNode;
		if (mSpawn.GOTree == null)
		{
			fGameObjectNode = null;
			if (mSpawn.Prefab != null)
			{
				fGameObjectNode = new FGameObjectNode(mSpawn.Prefab, clone: true, mSpawn.Prefab);
			}
		}
		else
		{
			FGOTreeInstance fGOTreeInstance = FGOTreeManager.Build(mSpawn.GOTree, null, FRandom.Generic);
			fGameObjectNode = new FGameObjectNode(fGOTreeInstance.Base, clone: false, fGOTreeInstance.BasePrefab);
			// TODO(port): two virtual FNode calls (anchor / size from the GO tree instance: AnchorOffset, CanonicalSize) are not identified (0x1787bd8) [left: FGOTreeManager (3D GO tree) branch, unported dead code]
		}
		Vector3 vector = Vector3.zero;
		bool flag = false;
		if (fGameObjectNode != null)
		{
			if (node != null)
			{
				fGameObjectNode.sortZ = node.sortZ + mSpawn.UIZOffset;
				FGameObjectNode fGameObjectNode2 = (mSpawn.SpawnAt != null) ? node.FindNodeOfType<FGameObjectNode>() : null;
				GameObject gameObject = null;
				if (fGameObjectNode2 != null && fGameObjectNode2.GameObj != null)
				{
					gameObject = ((!(fGameObjectNode2.BasePrefab != null)) ? fGameObjectNode2.GameObj.FindFirstRecursive(mSpawn.SpawnAt) : fGameObjectNode2.GameObj.FindFirstRecursiveMatchingPrefab(mSpawn.SpawnAt, fGameObjectNode2.BasePrefab));
				}
				if (gameObject != null)
				{
					vector = gameObject.transform.position;
					fGameObjectNode.Position = (Vector2)gameObject.transform.position;
					flag = true;
				}
				else
				{
					fGameObjectNode.Position = node.Position;
				}
			}
			if (lifespan < 0f)
			{
				lifespan = mSpawn.Lifespan;
			}
			if (lifespan >= 0f)
			{
				fGameObjectNode.BeginLifespan(lifespan);
			}
		}
		if (FCoreUtil.Valid(mSoundSet))
		{
			if (flag)
			{
				FApp.SoundManager.PlaySoundSetAtPoint(mSoundSet, vector);
			}
			else
			{
				FApp.SoundManager.PlaySoundSet(mSoundSet);
			}
		}
		return fGameObjectNode;
	}

	public bool LoadXml(XmlElement root)
	{
		mName = root.GetAttrib("Name");
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			XmlElement xmlElement = childNode as XmlElement;
			string name = childNode.Name;
			if (name == "Spawn")
			{
				if (mSpawn == null)
				{
					mSpawn = new FAnimTagSpawn();
					mSpawn.LoadXml(xmlElement);
				}
				else
				{
					FLog.UnityError("Impact effect " + mName + " trying to define multiple spawn tags, which is not supported");
				}
			}
			else if (name == "Sound")
			{
				mSoundSet = xmlElement.GetAttrib("Name", string.Empty);
			}
			else if (name == "CameraShake")
			{
				mCameraShake = new FCameraShakeParams();
				mCameraShake.LoadXml(xmlElement);
			}
			else if (name == "Item")
			{
				mItemType = xmlElement.GetAttrib("Name", string.Empty);
			}
		}
		return true;
	}

	public bool GetCenteredOnTarget()
	{
		return mSpawn != null && mSpawn.AttachMode == FAttachMode.CenteredOnTarget;
	}

	public Vector3 GetOffset()
	{
		if (mSpawn == null)
		{
			return Vector3.zero;
		}
		return mSpawn.Offset;
	}

	public static void SaveRef(FBinWriter writer, FImpactEffect impactEffect)
	{
		writer.Write(impactEffect?.mName);
	}

	public static FImpactEffect LoadRef(FBinReader reader)
	{
		string text = reader.ReadString();
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return FImpactEffectManager.GetImpactEffect(text);
	}
}
