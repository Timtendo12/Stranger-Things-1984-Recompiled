using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FAnimateUVOffset @ 0x1604a40..0x1604d1c, Ghidra vaddr).
public class FAnimateUVOffset : MonoBehaviour
{
	public Vector2 UVAnimationRate = new Vector2(0f, 1f);

	public string[] Textures = new string[2] { "_MainTex", "_BumpMap" };

	public int MaterialIndex;

	public bool Animate = true;

	public float AnimSpeed = 1f;

	protected Vector2 mUVOffset = Vector2.zero;

	private void LateUpdate()
	{
		if (!Animate)
		{
			return;
		}
		mUVOffset += UVAnimationRate * AnimSpeed * Time.deltaTime;
		if (mUVOffset.x > 10f)
		{
			mUVOffset.x -= 10f;
		}
		if (mUVOffset.y > 10f)
		{
			mUVOffset.y -= 10f;
		}
		if (mUVOffset.x < -10f)
		{
			mUVOffset.x += 10f;
		}
		if (mUVOffset.y < -10f)
		{
			mUVOffset.y += 10f;
		}
		Renderer component = GetComponent<Renderer>();
		if (!(component != null) || !component.enabled || MaterialIndex < 0 || component.materials.Length <= MaterialIndex || Textures == null)
		{
			return;
		}
		for (int i = 0; i < Textures.Length; i++)
		{
			GetComponent<Renderer>().materials[MaterialIndex].SetTextureOffset(Textures[i], mUVOffset);
		}
	}
}
