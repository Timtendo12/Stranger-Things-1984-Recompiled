using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FFontStyle @ 0x1688cb0..0x168abb8, Ghidra vaddr).
public class FFontStyle
{
	public class FFontEntry
	{
		public string Name;

		public int PointSize;

		public float FontResourceScaleInverse;

		public FFontEntry(string name, int pointSize, float fontResourceScaleInverse)
		{
			Name = name;
			PointSize = pointSize;
			FontResourceScaleInverse = fontResourceScaleInverse;
		}
	}

	protected float mSmallestPointSize = float.MaxValue;

	protected List<FFontEntry> mFontEntries = new List<FFontEntry>();

	public string Name { get; set; }

	public float SmallestPointSize => mSmallestPointSize;

	public bool LoadXML(XmlElement root)
	{
		if (root == null)
		{
			return false;
		}
		Name = root.GetAttrib("Name", (string)null);
		foreach (XmlNode node in root.ChildNodes)
		{
			if (node.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			XmlElement element = (XmlElement)node;
			if (element.Name == "Font")
			{
				string fontName = element.GetAttrib("Name", (string)null);
				int pointSize = element.GetAttrib("PointSize", -1);
				FFont font = Futile.AtlasManager.GetOrLoadFontWithNameSize(fontName, pointSize);
				if (font != null)
				{
					mFontEntries.Add(new FFontEntry(fontName, pointSize, (float)pointSize / font.PointSize));
				}
			}
			else if (element.Name == "DynamicFont")
			{
				float scaleHack = element.GetAttrib("ScaleHack", 1f);
				float lineHeightOffset = element.GetAttrib("LineHeightOffset", 0f);
				FFont.LoadDynamicFont(Name, FShader.DynamicFont, element.GetAttrib("FontFile", (string)null), lineHeightOffset, new Vector4(0f, 0f, 1f, 1f), 1f, new Vector4(0.33f, 0.28f, 0.29f, 1f), new Vector4(0f, 0f, 0f, 1f), 1f, scaleHack);
			}
		}
		mFontEntries.Sort((FFontEntry a, FFontEntry b) => (b.PointSize < a.PointSize) ? (-1) : ((a.PointSize < b.PointSize) ? 1 : 0));
		mSmallestPointSize = GetSmallestPointSize();
		return true;
	}

	public FFont GetFontBySize(int pointSize, out float fontResourceScaleInverse)
	{
		fontResourceScaleInverse = 1f;
		for (int i = 0; i < mFontEntries.Count; i++)
		{
			if (i == mFontEntries.Count - 1 || mFontEntries[i].PointSize <= pointSize)
			{
				fontResourceScaleInverse = mFontEntries[i].FontResourceScaleInverse;
				return Futile.AtlasManager.GetFontWithName(mFontEntries[i].Name);
			}
		}
		return null;
	}

	protected float GetSmallestPointSize()
	{
		float smallest = float.MaxValue;
		for (int i = 0; i < mFontEntries.Count; i++)
		{
			if ((float)mFontEntries[i].PointSize < smallest)
			{
				smallest = mFontEntries[i].PointSize;
			}
		}
		return smallest;
	}

	public int GetNextLargerPointSize(int startPointSize)
	{
		for (int i = 0; ; i++)
		{
			if (i + 1 >= mFontEntries.Count)
			{
				return mFontEntries[0].PointSize;
			}
			if (mFontEntries[i + 1].PointSize <= startPointSize)
			{
				return mFontEntries[i].PointSize;
			}
		}
	}

	public int GetNextSmallerPointSize(int startPointSize)
	{
		if (mFontEntries.Count < 1)
		{
			return -1;
		}
		for (int i = mFontEntries.Count - 1; i > 0; i--)
		{
			if (startPointSize <= mFontEntries[i - 1].PointSize)
			{
				return mFontEntries[i].PointSize;
			}
		}
		return mFontEntries[mFontEntries.Count - 1].PointSize;
	}

	public List<int> GetAvailableSizes()
	{
		List<int> sizes = new List<int>();
		for (int i = 0; i < mFontEntries.Count; i++)
		{
			sizes.Add(mFontEntries[i].PointSize);
		}
		return sizes;
	}
}
