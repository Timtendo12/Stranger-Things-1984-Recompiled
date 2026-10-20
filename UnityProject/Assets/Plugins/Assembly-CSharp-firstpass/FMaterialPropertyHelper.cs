using UnityEngine;

public class FMaterialPropertyHelper : MonoBehaviour
{
	public bool AffectTrailsAndParticles;

	public bool AffectProjectors;

	public float Alpha
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public void OnDestroy()
	{
	}
}
