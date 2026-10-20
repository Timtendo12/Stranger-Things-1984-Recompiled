using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (ChapterTitleAlphaSetter @ 0x16dfc7c..0x16dfe38, Ghidra vaddr).
public class ChapterTitleAlphaSetter : MonoBehaviour
{
	protected float mAlpha = 1f;

	protected float mWhiteAlpha = 1f;

	protected Material mMaterial;

	public float Alpha { get; set; }

	public float WhiteAlpha { get; set; }

	private void Start()
	{
		Alpha = 1f;
		WhiteAlpha = 1f;
		mMaterial = GetComponent<MeshRenderer>().material;
	}

	private void Update()
	{
		if (!(mMaterial == null))
		{
			if (Alpha != mAlpha)
			{
				Alpha = Mathf.Clamp01(Alpha);
				mAlpha = Alpha;
				mMaterial.SetFloat("_Alpha", mAlpha);
			}
			if (WhiteAlpha != mWhiteAlpha)
			{
				WhiteAlpha = Mathf.Clamp01(WhiteAlpha);
				mWhiteAlpha = WhiteAlpha;
				mMaterial.SetFloat("_WhiteAlpha", mWhiteAlpha);
			}
		}
	}
}
