using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FSpawnImpactEffect @ 0x17cf66c..0x17cf7c4, Ghidra vaddr).
public class FSpawnImpactEffect : MonoBehaviour
{
	public float Delay;

	public string ImpactEffect;

	protected float mElapsed;

	public void Update()
	{
		mElapsed += Time.deltaTime;
		if (!(mElapsed < Delay))
		{
			FImpactEffect impactEffect = FImpactEffectManager.GetImpactEffect(ImpactEffect);
			impactEffect.SpawnAtPosition(base.transform.position, base.transform.rotation);
			Object.Destroy(this);
		}
	}
}
