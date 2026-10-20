using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FDestructible @ 0x16f859c..0x16f95f4, Ghidra vaddr).
public class FDestructible : MonoBehaviour
{
	[Tooltip("ReplacementPrefab is swapped in for this prefab on taking any damage")]
	public GameObject ReplacementPrefab;

	[Tooltip("Parts that are immmediately released when taking any damage")]
	public List<GameObject> PartsOnDestruct = new List<GameObject>();

	[Tooltip("Parts that will be released when this object is swapped in as a ReplacementPrefab for something else.")]
	public List<GameObject> PartsOnReplace = new List<GameObject>();

	[Tooltip("Parts that can be broken off gradually on damage.")]
	public List<GameObject> BreakableParts = new List<GameObject>();

	protected HashSet<GameObject> mBreakableParts = new HashSet<GameObject>();

	[Tooltip("Gibs thrown on first taking damage.")]
	public FGibInfo GibInfoDestruct;

	[Tooltip("Gibs thrown when taking additional hits, optionally only when no parts can be broken off.")]
	public FGibInfo GibInfoHit;

	[Tooltip("Determines whether GibInfoHit gibs are always used or only if there are no parts to break off.")]
	public bool HitGibsOnlyWhenNoBreakables = true;

	protected bool mDestructed;

	public void Awake()
	{
		SetPartsKinematic(PartsOnDestruct, kinematic: true);
		SetPartsKinematic(PartsOnReplace, kinematic: true);
		SetPartsKinematic(BreakableParts, kinematic: true);
		for (int i = 0; i < BreakableParts.Count; i++)
		{
			mBreakableParts.Add(BreakableParts[i]);
		}
	}

	protected void SetPartsKinematic(List<GameObject> parts, bool kinematic)
	{
		for (int i = 0; i < parts.Count; i++)
		{
			SetPartKinematic(parts[i], kinematic);
		}
	}

	protected void SetPartKinematic(GameObject go, bool kinematic)
	{
		go.ApplyFunctionRecursive(delegate(GameObject obj)
		{
			if (!(obj == null))
			{
				Rigidbody component = obj.GetComponent<Rigidbody>();
				if (!(component == null))
				{
					component.isKinematic = kinematic;
				}
			}
		});
	}

	protected void ReleaseParts(List<GameObject> parts)
	{
		for (int i = 0; i < parts.Count; i++)
		{
			ReleasePart(parts[i]);
		}
	}

	protected void ReleasePart(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		FGibFXWhenThrown component = go.GetComponent<FGibFXWhenThrown>();
		if (component != null)
		{
			component.Thrown();
			if (component.PartStaysInPlace)
			{
				return;
			}
		}
		SetPartKinematic(go, kinematic: false);
		go.transform.parent = null;
		FBreakableGib component2 = go.GetComponent<FBreakableGib>();
		if (component2 != null)
		{
			component2.SetSource(base.gameObject);
		}
		if (go.GetComponent<FFadeLifespan>() == null)
		{
			FFadeLifespan fFadeLifespan = go.AddComponent<FFadeLifespan>();
			fFadeLifespan.FadeDelay = 5f;
			fFadeLifespan.FadeTime = 2f;
			fFadeLifespan.ScaleOut = true;
		}
	}

	public void Replace()
	{
		ReleaseParts(PartsOnReplace);
		mDestructed = true;
	}

	public void NearbyExplosion(Vector3 explosionCenter, float radius, float force, bool skipDestruct = false)
	{
		if (!skipDestruct)
		{
			FDestructible fDestructible = Destruct();
			if (fDestructible != null)
			{
				fDestructible.NearbyExplosion(explosionCenter, radius, force, skipDestruct: true);
			}
		}
		bool flag = true;
		if (mBreakableParts.Count > 0)
		{
			Collider[] array = Physics.OverlapSphere(explosionCenter, radius);
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = array[i].gameObject;
				if (!mBreakableParts.Contains(gameObject))
				{
					continue;
				}
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				Vector3 lhs = ((!(component != null)) ? gameObject.transform.position : component.worldCenterOfMass);
				Vector3 normalized = (explosionCenter - base.transform.position).normalized;
				float num2 = Vector3.Dot(lhs, normalized) - Vector3.Dot(base.transform.position, normalized);
				float num3 = Vector3.Dot(explosionCenter, normalized) - Vector3.Dot(base.transform.position, normalized);
				if ((num2 > 0f && num3 > 0f) || (num2 <= 0f && num3 <= 0f))
				{
					mBreakableParts.Remove(gameObject);
					ReleasePart(gameObject);
					num++;
				}
			}
			flag = num < 1;
			if (HitGibsOnlyWhenNoBreakables && !flag)
			{
				return;
			}
		}
		GibInfoHit.SpawnGibs(base.gameObject, base.transform, Vector3.zero, null, null);
	}

	protected FDestructible Destruct()
	{
		FDestructible result = null;
		if (!mDestructed)
		{
			mDestructed = true;
			ReleaseParts(PartsOnDestruct);
			if (ReplacementPrefab != null)
			{
				GameObject gameObject = ReplacementPrefab.Clone(base.transform.position);
				gameObject.transform.rotation = base.transform.rotation;
				result = gameObject.GetComponent<FDestructible>();
				if (result != null)
				{
					result.ReleaseParts(result.PartsOnReplace);
					result.mDestructed = true;
				}
				Object.Destroy(base.gameObject);
			}
			GibInfoDestruct.SpawnGibs(base.gameObject, base.transform, Vector3.zero, null, null);
		}
		return result;
	}

	private void OnDrawGizmos()
	{
		if (GibInfoDestruct != null && GibInfoDestruct.HasGibs)
		{
			GibInfoDestruct.DrawSpawnGizmo(base.transform.position);
		}
	}

	public void TriggerEntered(GameObject go, Vector3 velocity)
	{
		Destruct();
		Collider[] array = Physics.OverlapSphere(go.transform.position, 2.5f);
		for (int i = 0; i < array.Length; i++)
		{
			Rigidbody component = array[i].gameObject.GetComponent<Rigidbody>();
			if (!(component == null) && !component.isKinematic)
			{
				// TODO(port): force/upwards constants read from lost vector registers (0x16f9300); 1000 / 2.5 / 5 assumed [left: 3D Rigidbody explosion; no FDestructible exists in the 2D data]
				component.AddExplosionForce(1000f, go.transform.position, 2.5f, 5f);
			}
		}
	}
}
