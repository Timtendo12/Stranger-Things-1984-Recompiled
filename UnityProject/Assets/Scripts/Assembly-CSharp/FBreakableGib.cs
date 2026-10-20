using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FBreakableGib @ 0x16f12cc..0x16f1970, Ghidra vaddr).
public class FBreakableGib : MonoBehaviour
{
	public List<GameObject> ChildParts = new List<GameObject>();

	public FGibInfo GibInfo;

	public float SpeedThreshold = 2f;

	public bool DisappearOnHit;

	public string ImpactEffectOnBreak;

	protected GameObject mSource;

	protected bool mBroken;

	public void Awake()
	{
		Rigidbody orCreateComponent = base.gameObject.GetOrCreateComponent<Rigidbody>();
		for (int i = 0; i < ChildParts.Count; i++)
		{
			GameObject gameObject = ChildParts[i];
			if (!(gameObject == null))
			{
				gameObject.GetOrCreateComponent<FixedJoint>().connectedBody = orCreateComponent;
				gameObject.GetOrCreateComponent<FBreakableGibChildHelper>().SetParent(this);
			}
		}
	}

	public void SetSource(GameObject source)
	{
		mSource = source;
	}

	public void Break()
	{
		mBroken = true;
		for (int i = 0; i < ChildParts.Count; i++)
		{
			GameObject gameObject = ChildParts[i];
			if (!(gameObject == null))
			{
				Object.Destroy(gameObject.GetOrCreateComponent<FixedJoint>());
			}
		}
		GibInfo.SpawnGibs(base.gameObject, base.transform, Vector3.zero, null, null);
		FImpactEffect impactEffect = FImpactEffectManager.GetImpactEffect(ImpactEffectOnBreak);
		if (impactEffect != null)
		{
			impactEffect.SpawnAtPosition(base.transform.position, base.transform.rotation);
		}
		if (DisappearOnHit)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (!mBroken && collision.relativeVelocity.magnitude >= SpeedThreshold && !collision.gameObject.HasAncestor(mSource))
		{
			Break();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (GibInfo != null && GibInfo.HasGibs)
		{
			GibInfo.DrawSpawnGizmo(base.transform.position);
		}
	}
}
