using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FAtlasManager @ 0x1686540..0x168a208, Ghidra vaddr).
public class FAtlasManager
{
	private static int mNextAtlasIndex;

	private List<FAtlas> mAtlases = new List<FAtlas>();

	private List<FAtlasElement> mAllElements = new List<FAtlasElement>();

	private Dictionary<string, FAtlasElement> mAllElementsByName = new Dictionary<string, FAtlasElement>();

	private List<FFont> mFonts = new List<FFont>();

	private Dictionary<string, FFont> mFontsByName = new Dictionary<string, FFont>();

	private Dictionary<string, FFontStyle> mFontStylesByName = new Dictionary<string, FFontStyle>();

	public static bool AtlasUnloadingEnabled;

	public static bool CompressedAtlasesEnabled;

	public List<FAtlas> Atlases => mAtlases;

	public List<FFont> Fonts => mFonts;

	public bool DoesContainAtlas(string name)
	{
		for (int i = 0; i < mAtlases.Count; i++)
		{
			if (mAtlases[i].Name == name)
			{
				return true;
			}
		}
		return false;
	}

	public FAtlas GetAtlas(string name)
	{
		for (int i = 0; i < mAtlases.Count; i++)
		{
			if (mAtlases[i].Name == name)
			{
				return mAtlases[i];
			}
		}
		return null;
	}

	public List<FAtlas> GetAtlasCaseInsensitiveFragment(string name)
	{
		List<FAtlas> result = new List<FAtlas>();
		if (name == null)
		{
			return result;
		}
		string lower = name.ToLower();
		for (int i = 0; i < mAtlases.Count; i++)
		{
			if (mAtlases[i].Name.ToLower().Contains(lower))
			{
				result.Add(mAtlases[i]);
			}
		}
		return result;
	}

	public FAtlas LoadAtlasFromTexture(string name, Texture texture)
	{
		FAtlas atlas = GetAtlas(name);
		if (atlas == null)
		{
			atlas = new FAtlas(name, texture, ++mNextAtlasIndex);
			AddAtlas(atlas);
		}
		else
		{
			atlas.SetTexture(texture);
		}
		return atlas;
	}

	public FAtlas ActuallyLoadAtlasOrImage(string name, string imagePath, string dataPath, string infoPath, FAtlas.eUnloadStrategy unloadStrategy, FilterMode filterMode)
	{
		if (DoesContainAtlas(name))
		{
			return null;
		}
		FMemDebug.LogUsage("before atlas load '" + name + "'");
		bool singleImage = infoPath == "";
		FAtlas atlas = new FAtlas(name, imagePath, dataPath, infoPath, mNextAtlasIndex++, singleImage, unloadStrategy, filterMode);
		AddAtlas(atlas);
		FMemDebug.LogUsage("after atlas load '" + name + "'");
		return atlas;
	}

	private void AddAtlas(FAtlas atlas)
	{
		List<FAtlasElement> elements = atlas.Elements;
		for (int i = 0; i < elements.Count; i++)
		{
			FAtlasElement element = elements[i];
			element.Atlas = atlas;
			element.IndexInManager = mAllElements.Count;
			element.AtlasIndex = atlas.Index;
			mAllElements.Add(element);
			if (mAllElementsByName.ContainsKey(element.Name))
			{
				throw new Exception("Duplicate element name found! All element names must be unique! Element=" + element.Name);
			}
			mAllElementsByName.Add(element.Name, element);
		}
		mAtlases.Add(atlas);
	}

	public FAtlas LoadAtlas(string atlasPath, FAtlas.eUnloadStrategy unloadStrategy = FAtlas.eUnloadStrategy.UnloadAnytime, FilterMode filterMode = FilterMode.Bilinear)
	{
		return ActuallyLoadAtlasOrImage(atlasPath, atlasPath + Futile.ResourceSuffix, atlasPath + Futile.ResourceSuffix, atlasPath + "Info", unloadStrategy, filterMode);
	}

	public FAtlas LoadImage(string imagePath, FilterMode filterMode = FilterMode.Bilinear)
	{
		return ActuallyLoadAtlasOrImage(imagePath, imagePath + Futile.ResourceSuffix, "", "", FAtlas.eUnloadStrategy.NeverUnload, filterMode);
	}

	public FAtlas LoadDynamicAtlas(string dynamicAtlasPath, int cacheWidth, int cacheHeight, FAtlas.eUnloadStrategy unloadStrategy)
	{
		if (DoesContainAtlas(dynamicAtlasPath))
		{
			return null;
		}
		FDynamicAtlas atlas = new FDynamicAtlas(dynamicAtlasPath, mNextAtlasIndex++, cacheWidth, cacheHeight);
		atlas.UnloadStrategy = unloadStrategy;
		AddAtlas(atlas);
		return atlas;
	}

	public void ActuallyUnloadAtlasOrImage(string name)
	{
		for (int i = mAtlases.Count - 1; i >= 0; i--)
		{
			FAtlas atlas = mAtlases[i];
			if (atlas.Name != name)
			{
				continue;
			}
			for (int j = mAllElements.Count - 1; j >= 0; j--)
			{
				FAtlasElement element = mAllElements[j];
				if (element.Atlas == atlas)
				{
					mAllElements.RemoveAt(j);
					mAllElementsByName.Remove(element.Name);
				}
			}
			atlas.UnloadTexture();
			mAtlases.RemoveAt(i);
		}
	}

	public void UnloadAtlas(string atlasPath)
	{
		ActuallyUnloadAtlasOrImage(atlasPath);
	}

	public void UnloadImage(string imagePath)
	{
		ActuallyUnloadAtlasOrImage(imagePath);
	}

	public void LoadUnloadAtlases()
	{
		for (int i = 0; i < mAtlases.Count; i++)
		{
			if (mAtlases[i].LoadUnload())
			{
				Futile.HandleUpdateTextures();
			}
		}
	}

	public FAtlasElement GetElementWithName(string elementName, bool nullOk = false)
	{
		if (mAllElementsByName.ContainsKey(elementName))
		{
			return mAllElementsByName[elementName];
		}
		if (nullOk)
		{
			return null;
		}
		FAssert.Fail("Couldn't find element named '" + elementName + "'");
		return mAllElementsByName.Get("WhiteSquare.png");
	}

	public FFontStyle GetFontStyle(string fontStyleName)
	{
		if (!mFontStylesByName.ContainsKey(fontStyleName))
		{
			return null;
		}
		return mFontStylesByName[fontStyleName];
	}

	public FFont GetFontWithName(string fontName)
	{
		return mFontsByName[fontName];
	}

	public FFont GetOrLoadFontWithNameSize(string fontName, int pointSize)
	{
		mFontsByName.TryGetValue(fontName, out FFont font);
		if (font != null)
		{
			return font;
		}
		FTextParams textParams = new FTextParams
		{
			PointSize = pointSize,
			lineHeightOffset = 0f
		};
		LoadFont(fontName, fontName + Futile.ResourceSuffix + ".png", "Atlases/" + fontName + Futile.ResourceSuffix, textParams, 1f);
		return GetFontWithName(fontName);
	}

	public void LoadFont(string name, string elementName, string configPath, float configScale)
	{
		LoadFont(name, elementName, configPath, new FTextParams(), configScale);
	}

	public void LoadFont(string name, string elementName, string configPath, FTextParams fontTextParams, float configScale)
	{
		FAtlasElement element = FFont.SpriteFontsDisabled ? null : GetElementWithName(elementName);
		FFont font = new FFont(name, element, configPath, fontTextParams, configScale);
		mFonts.Add(font);
		mFontsByName.Add(name, font);
	}

	public FFont GetFontWithStyleAndSize(string fontStyleName, int pointSize)
	{
		if (FConfigCore.DynamicFonts)
		{
			FFont dynamicFont = FFont.DynamicFonts.Get(FFont.ConvertToDynamicFont(fontStyleName));
			if (dynamicFont != null)
			{
				return dynamicFont;
			}
		}
		return GetBaseFontWithStyleAndSize(fontStyleName, pointSize, out float _);
	}

	public FFont GetBaseFontWithStyleAndSize(string fontStyleName, int pointSize, out float fontResourceScaleInverse)
	{
		fontResourceScaleInverse = 1f;
		mFontStylesByName.TryGetValue(fontStyleName, out FFontStyle style);
		if (style == null)
		{
			return null;
		}
		return style.GetFontBySize(pointSize, out fontResourceScaleInverse);
	}

	public int GetNextLargerPointSize(string fontStyleName, int startPointSize)
	{
		mFontStylesByName.TryGetValue(fontStyleName, out FFontStyle style);
		return style == null ? -1 : style.GetNextLargerPointSize(startPointSize);
	}

	public int GetNextSmallerPointSize(string fontStyleName, int startPointSize)
	{
		mFontStylesByName.TryGetValue(fontStyleName, out FFontStyle style);
		return style == null ? -1 : style.GetNextSmallerPointSize(startPointSize);
	}

	public bool LoadFonts()
	{
		mFontStylesByName.Clear();
		XmlElement root = FUtil.LoadXmlAsset(FLoc.LanguageDirectoryStem + "/Fonts");
		if (root == null)
		{
			root = FUtil.LoadXmlAsset("Data/Fonts");
			if (root == null)
			{
				FLog.UnityError("Failed to load/parse Fonts.xml");
				return false;
			}
		}
		foreach (XmlNode node in root.ChildNodes)
		{
			if (node.NodeType != XmlNodeType.Element || node.Name != "Style")
			{
				continue;
			}
			FFontStyle style = new FFontStyle();
			if (style.LoadXML(node as XmlElement))
			{
				mFontStylesByName.Add(style.Name, style);
			}
		}
		return true;
	}

	public void OnDestroy()
	{
		for (int i = 0; i < mAtlases.Count; i++)
		{
			mAtlases[i].OnDestroy();
		}
	}

	public void Flush()
	{
		for (int i = 0; i < mAtlases.Count; i++)
		{
			mAtlases[i].Flush();
		}
	}

	public List<FAtlas> UnloadUnusedAtlases()
	{
		if (!AtlasUnloadingEnabled)
		{
			return null;
		}
		int frame = Time.frameCount;
		float time = Time.time;
		List<FAtlas> unloaded = null;
		for (int i = 0; i < mAtlases.Count; i++)
		{
			FAtlas atlas = mAtlases[i];
			if (atlas.Unloadable && frame - atlas.mLastFrameUsed > 5 && atlas.UseCount < 1 &&
				(atlas.mKeepAliveTime <= 0f || atlas.mKeepAliveTime <= time - atlas.mLastUseTime) && atlas.UnloadTexture())
			{
				if (unloaded == null)
				{
					unloaded = new List<FAtlas>();
				}
				unloaded.Add(atlas);
			}
		}
		return unloaded;
	}

	public void ResetUseCount3d()
	{
		for (int i = 0; i < mAtlases.Count; i++)
		{
			mAtlases[i].mUseCount3d = 0;
		}
	}
}
