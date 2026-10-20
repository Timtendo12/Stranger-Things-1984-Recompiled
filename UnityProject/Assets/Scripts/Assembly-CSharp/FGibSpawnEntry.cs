using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FGibSpawnEntry @ 0x1706724..0x1706a60, Ghidra vaddr).
public class FGibSpawnEntry
{
	public GameObject Prefab;

	public Vector3 Position;

	public Vector3 ExplosionCenter;

	public float ExplosionForce;

	public float ExplosionRadius;

	public float ForceUpwardsOffset;

	public float AddTime;

	public Color? XForm1;

	public Color? XForm2;

	public FGibSpawnEntry(GameObject prefab, Vector3 position, Vector3 explosionCenter, float explosionForce, float explosionRadius, float forceUpwardsOffset, float addTime, Color? xform1, Color? xform2)
	{
		Prefab = prefab;
		Position = position;
		ExplosionCenter = explosionCenter;
		ExplosionForce = explosionForce;
		ExplosionRadius = explosionRadius;
		ForceUpwardsOffset = forceUpwardsOffset;
		AddTime = addTime;
		XForm1 = xform1;
		XForm2 = xform2;
	}

	public void Spawn()
	{
		GameObject gameObject = Prefab.Clone();
		if (gameObject == null)
		{
			return;
		}
		gameObject.transform.position = Position;
		if (XForm1.HasValue)
		{
			gameObject.SetMaterialColorRecursive("_Xform1", XForm1.Value);
		}
		if (XForm2.HasValue)
		{
			gameObject.SetMaterialColorRecursive("_Xform2", XForm2.Value);
		}
		Rigidbody component = gameObject.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.AddExplosionForce(ExplosionForce, ExplosionCenter, ExplosionRadius, ForceUpwardsOffset);
		}
		FDebrisTracker.AddItem(gameObject);
	}
}
