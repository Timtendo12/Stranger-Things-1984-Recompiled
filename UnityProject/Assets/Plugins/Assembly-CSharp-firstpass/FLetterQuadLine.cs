using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FLetterQuadLine @ 0x1699128..0x1699eb0, Ghidra vaddr).
public class FLetterQuadLine
{
	public Rect Bounds;

	public List<FLetterQuadGroup> LetterQuadGroups = new List<FLetterQuadGroup>();

	public bool HasAnyCharacters
	{
		get
		{
			if (LetterQuadGroups.Count > 0)
			{
				return LetterQuadGroups[0].Quads.Count > 0;
			}
			return false;
		}
	}

	public Vector2 Offset { get; protected set; }

	public int GetTotalQuadCount()
	{
		int total = 0;
		for (int i = 0; i < LetterQuadGroups.Count; i++)
		{
			total += LetterQuadGroups[i].Quads.Count;
		}
		return total;
	}

	public bool DoesMatchCurrent(FAtlas atlas, Color color, FShader shader)
	{
		int last = LetterQuadGroups.Count - 1;
		if (last < 0)
		{
			return false;
		}
		FLetterQuadGroup group = LetterQuadGroups[last];
		if (group.Atlas != atlas)
		{
			return false;
		}
		float dr = group.Color.r - color.r;
		float dg = group.Color.g - color.g;
		float db = group.Color.b - color.b;
		float da = group.Color.a - color.a;
		if (1E-10f <= da * da + db * db + dr * dr + dg * dg)
		{
			return false;
		}
		return group.Shader == shader;
	}

	public FLetterQuadGroup GetOrCreateGroup(FAtlas atlas, Color color, FShader shader)
	{
		if (!DoesMatchCurrent(atlas, color, shader))
		{
			FLetterQuadGroup group = FTextBuilderPools.FLetterQuadGroupPool.Get();
			group.Atlas = atlas;
			group.Shader = shader;
			group.Color = color;
			LetterQuadGroups.Add(group);
			return group;
		}
		return LetterQuadGroups[LetterQuadGroups.Count - 1];
	}

	public void AddLetterQuad(FAtlas atlas, Color color, FShader shader, FLetterQuad quad)
	{
		if (quad != null)
		{
			GetOrCreateGroup(atlas, color, shader).Quads.Add(quad);
		}
		else
		{
			FAssert.Fail("adding null letter quad -- this should never happen");
		}
	}

	public float RemoveLast(int count)
	{
		float result = 0f;
		if (count > 0 && LetterQuadGroups.Count > 0)
		{
			int groupCount = LetterQuadGroups.Count;
			float width = 0f;
			float x = 0f;
			do
			{
				FLetterQuadGroup group = LetterQuadGroups[groupCount - 1];
				int quadCount = group.Quads.Count;
				int remove = count;
				if (quadCount <= count)
				{
					remove = quadCount;
				}
				FLetterQuad first = group.Quads[quadCount - remove];
				width = first.rect.width;
				x = first.rect.x;
				group.Quads.RemoveRange(group.Quads.Count - remove, remove);
				if (group.Quads.Count < 1)
				{
					LetterQuadGroups.RemoveAt(LetterQuadGroups.Count - 1);
				}
				count -= remove;
				groupCount = LetterQuadGroups.Count;
			}
			while (count > 0 && groupCount > 0);
			result = width + x;
			if (groupCount > 0 || count < 1)
			{
				return result;
			}
			FAssert.Fail("Tried to remove too many quads");
			return result;
		}
		FAssert.Fail("Invalid FLetterQuadLine.RemoveLast");
		return result;
	}

	public void CalculateVectors(float offsetX, float offsetY, float textScale)
	{
		Offset = new Vector2(offsetX, offsetY);
		for (int i = 0; i < LetterQuadGroups.Count; i++)
		{
			LetterQuadGroups[i].CalculateVectors(offsetX, offsetY, textScale);
		}
	}

	public void ApplyOffset(float offsetX, float offsetY)
	{
		Bounds.x += offsetX;
		Bounds.y += offsetY;
		for (int i = 0; i < LetterQuadGroups.Count; i++)
		{
			LetterQuadGroups[i].ApplyOffset(offsetX, offsetY);
		}
	}

	public void ReleaseData(bool isDynamic)
	{
		if (!FConfigCore.PoolFLetterQuad)
		{
			return;
		}
		if (!isDynamic)
		{
			for (int i = 0; i < LetterQuadGroups.Count; i++)
			{
				List<FLetterQuad> quads = LetterQuadGroups[i].Quads;
				for (int j = 0; j < quads.Count; j++)
				{
					if (quads[j] == null)
					{
						FAssert.Fail("Null letter quad in letter quad group!");
					}
					else
					{
						quads[j].charInfo = null;
					}
				}
			}
			return;
		}
		for (int k = 0; k < LetterQuadGroups.Count; k++)
		{
			FLetterQuadGroup group = LetterQuadGroups[k];
			for (int l = 0; l < group.Quads.Count; l++)
			{
				FLetterQuad quad = group.Quads[l];
				if (quad == null)
				{
					FAssert.Fail("Null letter quad in letter quad group!");
					continue;
				}
				quad.rotation = 0f;
				quad.IsImage = false;
				quad.shader = null;
				FTextBuilderPools.FLetterQuadPool.Free(quad);
			}
			FTextBuilderPools.FLetterQuadGroupPool.Free(group);
			group.Quads.Clear();
		}
		LetterQuadGroups.Clear();
		FTextBuilderPools.FLetterQuadLinePool.Free(this);
	}
}
