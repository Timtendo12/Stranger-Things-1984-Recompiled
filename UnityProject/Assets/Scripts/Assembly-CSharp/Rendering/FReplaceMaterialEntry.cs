using UnityEngine;

public class FReplaceMaterialEntry
{
	public GameObject Root;

	public string OldMaterial;

	public string NewMaterial;

	public string Skin;

	public bool IsSkinDefault => false;

	public FReplaceMaterialEntry(GameObject root, string oldMaterial, string newMaterial, string skin)
	{
	}

	public FReplaceMaterialEntry DeepCopy()
	{
		return null;
	}
}
