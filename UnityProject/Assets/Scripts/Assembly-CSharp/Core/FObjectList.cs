using System.Collections.Generic;
using UnityEngine;

public class FObjectList
{
	public List<GameObject> Objects;

	public GameObject Prefab;

	public bool KeepActive;

	public bool IsKinematic;

	public bool Reparent;

	public FObjectList(GameObject templateObject, bool keepActive, bool kinematic, bool reparent)
	{
	}
}
