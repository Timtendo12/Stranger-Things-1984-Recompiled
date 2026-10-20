using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FGibFXWhenThrown @ 0x1704fe8..0x1705274, Ghidra vaddr).
public class FGibFXWhenThrown : MonoBehaviour
{
	public List<GameObject> FXObjects;

	public GameObject SpawnPrefab;

	[Tooltip("Set this to prevent part from actually being thrown off, like for smoke fx on a building.")]
	public bool PartStaysInPlace;

	public void Awake()
	{
		SetFXActive(active: false);
	}

	public void Thrown()
	{
		SetFXActive(active: true);
		if (SpawnPrefab != null)
		{
			GameObject gameObject = SpawnPrefab.Clone();
			gameObject.transform.SetParent(base.transform, worldPositionStays: true);
			Renderer component = GetComponent<Renderer>();
			if (component != null)
			{
				gameObject.transform.position = component.bounds.center;
			}
			else
			{
				gameObject.transform.localPosition = Vector3.zero;
			}
		}
	}

	protected void SetFXActive(bool active)
	{
		if (FXObjects == null)
		{
			return;
		}
		for (int i = 0; i < FXObjects.Count; i++)
		{
			FXObjects[i].SetActive(active);
		}
	}
}
