using UnityEngine;

public class FReplaceTextureEntry
{
	public GameObject Root;

	public string OldTexture;

	public string NewTexture;

	public string Skin;

	public bool IsSkinDefault => false;

	public FReplaceTextureEntry(GameObject root, string oldTexture, string newTexture, string skin)
	{
	}

	public FReplaceTextureEntry DeepCopy()
	{
		return null;
	}
}
