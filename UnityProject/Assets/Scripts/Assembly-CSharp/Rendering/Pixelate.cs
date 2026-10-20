// Rewritten from libil2cpp.so 1.0.605 (Pixelate @ 0x016e0a28..0x016e0d3c, Ghidra vaddr).
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Pixelate")]
public class Pixelate : MonoBehaviour
{
	public Shader Shader;

	protected Material mMaterial;

	public int BlockSize { get; set; }

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (mMaterial == null)
		{
			mMaterial = new Material(Shader);
		}
		mMaterial.SetInt("_PixelateX", BlockSize);
		mMaterial.SetInt("_PixelateY", BlockSize);
		Graphics.Blit(source, destination, mMaterial, 0);
	}

	private void OnDisable()
	{
		Object.DestroyImmediate(mMaterial);
	}

	public void TweenToBlockSize(int size)
	{
		Go.killAllAbstractTweensWithTarget(this);
		base.enabled = true;
		GoTweenConfig config = new GoTweenConfig().intProp("BlockSize", size).onComplete(TweenComplete);
		Go.to(this, 0.5f, config);
	}

	protected void TweenComplete(AbstractGoTween tween)
	{
	}
}
