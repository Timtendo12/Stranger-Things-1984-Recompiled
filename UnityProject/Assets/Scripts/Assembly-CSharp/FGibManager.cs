using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FGibManager @ 0x1705b8c..0x1706e00, Ghidra vaddr).
public class FGibManager : MonoBehaviour
{
	protected static LinkedList<FGibSpawnEntry> SpawnList = new LinkedList<FGibSpawnEntry>();

	protected static bool mInitialized = false;

	public static int MaxSpawnsPerFrame;

	private const float LagTimeTilNuked = 0.25f;

	protected static int SpawnsThisFrame;

	public static void Init()
	{
		if (Application.isPlaying && !mInitialized)
		{
			GameObject gameObject = new GameObject("zzzzzzGibManager");
			gameObject.AddComponent<FGibManager>();
			Object.DontDestroyOnLoad(gameObject);
			mInitialized = true;
		}
	}

	public static void SpawnExplosionForce(GameObject source, GameObject prefab, Vector3 position, Quaternion rotation, float scale, Vector3 explosionCenter, float explosionForce, float explosionRadius, float forceUpwardsOffset, Vector3 torque, Color? xform1, Color? xform2)
	{
		if (SpawnsThisFrame < MaxSpawnsPerFrame)
		{
			GameObject gameObject = prefab.Clone();
			if (gameObject == null)
			{
				return;
			}
			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
			gameObject.transform.localScale = gameObject.transform.localScale * scale;
			if (xform1.HasValue)
			{
				gameObject.SetMaterialColorRecursive("_Xform1", xform1.Value);
			}
			if (xform2.HasValue)
			{
				gameObject.SetMaterialColorRecursive("_Xform2", xform2.Value);
			}
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.AddExplosionForce(explosionForce, explosionCenter, explosionRadius, forceUpwardsOffset);
				component.maxAngularVelocity = 25f;
				component.AddRelativeTorque(torque);
			}
			if (source != null)
			{
				source.IgnoreCollisionRecursive(gameObject);
			}
			FDebrisTracker.AddItem(gameObject);
			SpawnsThisFrame++;
		}
		else
		{
			SpawnList.AddLast(new FGibSpawnEntry(prefab, position, explosionCenter, explosionForce, explosionRadius, forceUpwardsOffset, Time.realtimeSinceStartup, xform1, xform2));
		}
	}

	public static void SpawnSimpleForce(GameObject source, GameObject prefab, Vector3 position, Quaternion rotation, float scale, Vector3 force, float centerOfMassOffsetRadius, Vector3 torque, Color? xform1, Color? xform2)
	{
		if (SpawnsThisFrame >= MaxSpawnsPerFrame)
		{
			// TODO(port): the over-budget branch of the shipped code is not recoverable (0x60d8); the spawn is dropped [left: 3D gibs, over-budget queue branch]
			return;
		}
		GameObject gameObject = prefab.Clone();
		if (gameObject == null)
		{
			return;
		}
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
		gameObject.transform.localScale = gameObject.transform.localScale * scale;
		if (xform1.HasValue)
		{
			gameObject.SetMaterialColorRecursive("_Xform1", xform1.Value);
		}
		if (xform2.HasValue)
		{
			gameObject.SetMaterialColorRecursive("_Xform2", xform2.Value);
		}
		Rigidbody component = gameObject.GetComponent<Rigidbody>();
		if (component != null)
		{
			Vector3 worldCenterOfMass = component.worldCenterOfMass;
			Vector3 vector = new Vector3(FRandom.Generic.RangeFloat(-centerOfMassOffsetRadius, centerOfMassOffsetRadius), FRandom.Generic.RangeFloat(-centerOfMassOffsetRadius, centerOfMassOffsetRadius), FRandom.Generic.RangeFloat(-centerOfMassOffsetRadius, centerOfMassOffsetRadius));
			component.AddForceAtPosition(force, worldCenterOfMass + vector, ForceMode.Impulse);
			component.maxAngularVelocity = 25f;
			component.AddRelativeTorque(torque);
		}
		if (source != null)
		{
			source.IgnoreCollisionRecursive(gameObject);
		}
		FDebrisTracker.AddItem(gameObject);
		SpawnsThisFrame++;
	}

	public void Update()
	{
		int num = Mathf.Min(SpawnList.Count, MaxSpawnsPerFrame - SpawnsThisFrame);
		while (num > 0)
		{
			SpawnList.First.Value.Spawn();
			SpawnList.RemoveFirst();
			num--;
		}
		SpawnsThisFrame = 0;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		while (SpawnList.Count > 0 && realtimeSinceStartup - SpawnList.First.Value.AddTime > 0.25f)
		{
			SpawnList.RemoveFirst();
		}
	}
}
