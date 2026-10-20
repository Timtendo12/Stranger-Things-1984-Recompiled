using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FGibSpawner @ 0x1706e00..0x1707194, Ghidra vaddr).
public class FGibSpawner : MonoBehaviour
{
	public int MaxGibSpawns = -1;

	public float SpawnDelay = 0.1f;

	public FGibInfo GibInfo;

	public GameObject ForceTarget;

	protected float mTimeTilNextEmit;

	protected float mLastTime;

	protected int mGibSpawnCounter;

	private void Start()
	{
		mGibSpawnCounter = 0;
		mLastTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		if (MaxGibSpawns >= 1 && mGibSpawnCounter >= MaxGibSpawns)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (SpawnDelay < 0.01f)
		{
			SpawnDelay = 0.01f;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		mTimeTilNextEmit -= realtimeSinceStartup - mLastTime;
		mLastTime = realtimeSinceStartup;
		if (mTimeTilNextEmit > 0f)
		{
			return;
		}
		Vector3 simpleForceDirection = ((!(ForceTarget != null)) ? base.transform.forward : (ForceTarget.transform.position - base.transform.position).normalized);
		while (mTimeTilNextEmit <= 0f)
		{
			GibInfo.SpawnGibs(base.gameObject, base.transform, simpleForceDirection, null, null);
			mTimeTilNextEmit += SpawnDelay;
			mGibSpawnCounter++;
			if (MaxGibSpawns >= 1 && mGibSpawnCounter >= MaxGibSpawns)
			{
				Object.Destroy(base.gameObject);
				return;
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (GibInfo != null && GibInfo.HasGibs)
		{
			GibInfo.DrawSpawnGizmo(base.transform.position);
		}
	}
}
