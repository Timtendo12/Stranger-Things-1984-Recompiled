using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FShader @ 0x1697824..0x1697d54, Ghidra vaddr).
public class FShader
{
	public static int NextShaderIndex;

	public static FShader DefaultShader;

	public static FShader Normal;

	public static FShader Additive;

	public static FShader DynamicFont;

	public static FShader DynamicFontOutline;

	public static FShader DynamicFontShadow;

	public static FShader DynamicFontOutlineShadow;

	public int Index;

	public string Name;

	public Shader Shader;

	public Material CustomMaterial;

	public bool UpdateCustomMaterialWithAtlas;

	public FShader(string name, Shader shader)
	{
		Name = name;
		Index = NextShaderIndex++;
		Shader = shader;
	}

	public FShader(string name, Material customMaterial)
	{
		Name = name;
		Index = NextShaderIndex++;
		CustomMaterial = customMaterial;
	}

	static FShader()
	{
		Normal = new FShader("Normal", Shader.Find("BonusXP/UI/BXPUIDefault"));
		DefaultShader = Normal;
		Additive = new FShader("Additive", Shader.Find("BonusXP/UI/BXPUIAdditive"));
		DynamicFont = new FShader("DynamicFont", Shader.Find("BonusXP/BXPDynamicFont4"));
		DynamicFontShadow = new FShader("DynamicFontShadow", Shader.Find("BonusXP/BXPDynamicFontShadow4"));
		Material outlineMaterial = new Material(Shader.Find("BonusXP/BXPDynamicFontOutlineVertShadow"));
		outlineMaterial.SetVector("_OutlineParams", new Vector4(2f, 2f, 2f, 0.7f));
		DynamicFontOutline = new FShader("DynamicFontOutline", outlineMaterial);
		DynamicFontOutline.UpdateCustomMaterialWithAtlas = true;
		// The shipped code first creates a shader-based instance and immediately replaces it (keeps the index numbering identical).
		DynamicFontOutlineShadow = new FShader("DynamicFontOutlineShadow", Shader.Find("BonusXP/BXPDynamicFontOutlineVertShadow"));
		Material outlineShadowMaterial = new Material(Shader.Find("BonusXP/BXPDynamicFontOutlineVertShadow"));
		outlineShadowMaterial.SetVector("_OutlineParams", new Vector4(1.5f, 3.5f, 6f, 0.7f));
		DynamicFontOutlineShadow = new FShader("DynamicFontOutlineShadow", outlineShadowMaterial);
		DynamicFontOutlineShadow.UpdateCustomMaterialWithAtlas = true;
	}
}
