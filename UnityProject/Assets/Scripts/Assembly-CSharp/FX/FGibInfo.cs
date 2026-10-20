using System;
using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FGibInfo @ 0x1705274..0x1706724, Ghidra vaddr).
[Serializable]
public class FGibInfo
{
	public enum Mode
	{
		Random = 0,
		InOrder = 1
	}

	public enum ForceModes
	{
		Explosion = 0,
		Simple = 1
	}

	public List<GameObject> GibPrefabs;

	public float GibScale = 1f;

	public int GibMinCount = 8;

	public int GibMaxCount = 12;

	public string LootTableName;

	public Vector3 GibStartRadius = new Vector3(1f, 1f, 1f);

	public float GibForceMin = 200f;

	public float GibForceMax = 400f;

	public float GibForceUpwardsOffset;

	public float GibExplosionRadius = 2f;

	public float GibCenterOfMassOffsetRadius;

	public Vector3 GibTorqueMin = Vector3.zero;

	public Vector3 GibTorqueMax = Vector3.zero;

	public Vector3 GibCenterOffset = Vector3.zero;

	public Mode GibMode;

	public ForceModes ForceMode;

	public bool ForceNonRigidBodyToCenter = true;

	public bool HasGibs
	{
		get
		{
			if ((LootTableName == null || LootTableName.Length < 1) && (GibPrefabs == null || GibPrefabs.Count < 1))
			{
				return false;
			}
			return true;
		}
	}

	public void SelectGibs(List<GameObject> resultPrefabs)
	{
		if (GibPrefabs == null || GibPrefabs.Count <= 0)
		{
			return;
		}
		int num = FRandom.Generic.RangeInt(GibMinCount, GibMaxCount);
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = null;
			if (GibMode == Mode.InOrder)
			{
				gameObject = GibPrefabs[i % GibPrefabs.Count];
			}
			else if (GibMode == Mode.Random)
			{
				gameObject = GibPrefabs[FRandom.Generic.RangeInt(0, GibPrefabs.Count - 1)];
			}
			if (!(gameObject == null))
			{
				resultPrefabs.Add(gameObject);
			}
		}
	}

	public void SpawnGibs(GameObject source, Transform launchPointTransform, Vector3 simpleForceDirection, Color? xform1, Color? xform2)
	{
		// the byte at PlayerData+0xa9 (user option "gibs") gates spawning
		if (FApp.PlayerData != null && !FApp.PlayerData.EnableGibs)
		{
			return;
		}
		List<GameObject> list = new List<GameObject>();
		SelectGibs(list);
		Vector3 vector = launchPointTransform.position + GibCenterOffset;
		Quaternion rotation = launchPointTransform.rotation;
		foreach (GameObject item in list)
		{
			Vector3 position = vector;
			if (item.GetComponent<Rigidbody>() != null || !ForceNonRigidBodyToCenter)
			{
				position = vector + new Vector3(FRandom.Generic.RangeFloat(0f - GibStartRadius.x, GibStartRadius.x), FRandom.Generic.RangeFloat(0f - GibStartRadius.y, GibStartRadius.y), FRandom.Generic.RangeFloat(0f - GibStartRadius.z, GibStartRadius.z));
			}
			float num = FRandom.Generic.RangeFloat(GibForceMin, GibForceMax);
			Vector3 torque = new Vector3(FRandom.Generic.RangeFloat(GibTorqueMin.x, GibTorqueMax.x), FRandom.Generic.RangeFloat(GibTorqueMin.y, GibTorqueMax.y), FRandom.Generic.RangeFloat(GibTorqueMin.z, GibTorqueMax.z));
			// TODO(port): force/center arguments are lost in the decompile (vector registers, 0x54fc); explosion centre = launch point, force = random in [GibForceMin, GibForceMax] [left: 3D gib prefabs (no FGibInfo in any prefab of this build); tried: SpawnGibs disassembly, argument registers are SIMD-packed]
			if (ForceMode == ForceModes.Explosion)
			{
				FGibManager.SpawnExplosionForce(source, item, position, rotation, GibScale, vector, num, GibExplosionRadius, GibForceUpwardsOffset, torque, xform1, xform2);
			}
			else if (ForceMode == ForceModes.Simple)
			{
				FGibManager.SpawnSimpleForce(source, item, position, rotation, GibScale, simpleForceDirection * num, GibCenterOfMassOffsetRadius, torque, xform1, xform2);
			}
		}
	}

	public void DrawSpawnGizmo(Vector3 position)
	{
		if (!Application.isPlaying)
		{
			Gizmos.DrawWireCube(position + GibCenterOffset, GibStartRadius);
		}
	}
}
