using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FAtlasElement @ 0x16835e0..0x1683adc, Ghidra vaddr).
public class FAtlasElement
{
	public string Name;

	public int IndexInAtlas;

	public int IndexInManager;

	public FAtlas Atlas;

	public int AtlasIndex;

	public Rect uvRect;

	public Vector2 uvTopLeft;

	public Vector2 uvTopRight;

	public Vector2 uvBottomRight;

	public Vector2 uvBottomLeft;

	public Rect SourceRect;

	public Vector2 SourceSize;

	public Vector4 BorderSize;

	public bool IsTrimmed;

	public bool IsRotated;

	public bool ShowSlicedInterior = true;

	public object AtlasData;

	public int UseCount;

	public FAtlasElement Clone()
	{
		FAtlasElement element = new FAtlasElement();
		element.Name = Name;
		element.IndexInAtlas = IndexInAtlas;
		element.IndexInManager = IndexInManager;
		element.Atlas = Atlas;
		element.AtlasIndex = AtlasIndex;
		element.uvRect = uvRect;
		element.uvTopLeft = uvTopLeft;
		element.uvTopRight = uvTopRight;
		element.uvBottomRight = uvBottomRight;
		element.uvBottomLeft = uvBottomLeft;
		element.SourceRect = SourceRect;
		element.SourceSize = SourceSize;
		element.BorderSize = BorderSize;
		element.IsTrimmed = IsTrimmed;
		element.IsRotated = IsRotated;
		element.ShowSlicedInterior = ShowSlicedInterior;
		element.AtlasData = AtlasData;
		return element;
	}

	public override string ToString()
	{
		return string.Format("{0} ({1} {2}x{3})", Name, Atlas.Name, SourceSize.x, SourceSize.y);
	}

	public bool Redraw()
	{
		return Atlas.Redraw(this);
	}

	public bool IncrementUseCount(string nodeName)
	{
		UseCount++;
		return Atlas.IncrementUseCount();
	}

	public bool DecrementUseCount(string nodeName)
	{
		UseCount--;
		if (UseCount < 0)
		{
			FLog.Error("FAtlasElement::DecUC: '" + Name + "' (" + Atlas.Name + ") has UseCount of " + UseCount.ToString() + ", ATLASLOADERROR.");
			UseCount = 0;
		}
		return Atlas.DecrementUseCount();
	}
}
