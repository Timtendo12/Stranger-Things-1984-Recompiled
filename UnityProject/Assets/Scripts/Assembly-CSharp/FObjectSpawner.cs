using UnityEngine;

public class FObjectSpawner : MonoBehaviour
{
	public bool SpawnOnTouch;

	public string ImpactEffectName;

	public int MinObjectsToSpawn;

	public int MaxObjectsToSpawn;

	public GameObject[] SpawnLocations;

	public string[] ObjectsToSpawn;

	public float ForceAlongNormal;

	private void Start()
	{
	}

	public void DoSpawn(Vector3 position, Vector3 normal)
	{
	}

	private void Update()
	{
	}
}
