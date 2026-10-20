using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (ScrollingUVs @ 0x17738ac..0x1773a24, Ghidra vaddr).
public class ScrollingUVs : MonoBehaviour
{
	public int materialIndex;

	public Vector2 uvAnimationRate = new Vector2(1f, 0f);

	public string textureName = "_MainTex";

	private Vector2 uvOffset = Vector2.zero;

	private void LateUpdate()
	{
		uvOffset += uvAnimationRate * Time.deltaTime;
		if (GetComponent<Renderer>().enabled)
		{
			GetComponent<Renderer>().materials[materialIndex].SetTextureOffset(textureName, uvOffset);
		}
	}
}
