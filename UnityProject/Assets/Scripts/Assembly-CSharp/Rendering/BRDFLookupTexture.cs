using UnityEngine;

[ExecuteInEditMode]
public class BRDFLookupTexture : MonoBehaviour
{
	public float intensity;

	public float diffuseIntensity;

	public Color keyColor;

	public Color fillColor;

	public Color backColor;

	public float wrapAround;

	public float metalic;

	public float specularIntensity;

	public float specularShininess;

	public float translucency;

	public Color translucentColor;

	public int lookupTextureWidth;

	public int lookupTextureHeight;

	public bool fastPreview;

	public Texture2D lookupTexture;

	private void Awake()
	{
	}

	private static Color ColorRGB(int r, int g, int b)
	{
		return default;
	}

	private void CheckConsistency()
	{
	}

	private Color PixelFunc(float ndotl, float ndoth)
	{
		return default;
	}

	private void TextureFunc(Texture2D tex)
	{
	}

	private void GenerateLookupTexture(int width, int height)
	{
	}

	public void Preview()
	{
	}

	public void Bake()
	{
	}
}
