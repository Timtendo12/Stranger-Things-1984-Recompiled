using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FLetterQuadGroup @ 0x1698dfc..0x1699128, Ghidra vaddr).
public class FLetterQuadGroup
{
	public List<FLetterQuad> Quads = new List<FLetterQuad>();

	public Color Color = Color.white;

	public FAtlas Atlas;

	public FShader Shader;

	public void Init(FAtlas atlas, Color color, FShader shader)
	{
		Atlas = atlas;
		Shader = shader;
		Color = color;
	}

	public void CalculateVectors(float offsetX, float offsetY, float textScale)
	{
		for (int i = 0; i < Quads.Count; i++)
		{
			if (Quads[i] == null)
			{
				FAssert.Fail("Null letter quad in letter quad group!");
			}
			else
			{
				Quads[i].CalculateVectors(offsetX, offsetY, textScale);
			}
		}
	}

	public void ApplyOffset(float offsetX, float offsetY)
	{
		for (int i = 0; i < Quads.Count; i++)
		{
			Quads[i].ApplyOffest(offsetX, offsetY);
		}
	}
}
