using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FFont @ 0x16828cc..0x168fcc0, Ghidra vaddr).
public class FFont
{
	public class UnityCharacterFontTracking
	{
		public Font UnityFont;

		public int Size;

		public FontStyle Style;

		public Dictionary<char, bool> usedCharacters = new Dictionary<char, bool>();

		public string mMinimalRequestString = "";

		public int RequestCount;

		public bool UsedBy3DUI;

		public Dictionary<char, CharacterInfo> mUnityCharInfo = new Dictionary<char, CharacterInfo>();

		public int DirtyEventCallBackCount;

		private bool EventRegistered;

		public int TextureRebuildCount;

		public int LastTrackedFrame;

		private bool mExpectingTextureRebuild;

		public void Init(Font font, int size, FontStyle style, string trackingName)
		{
			UnityFont = font;
			mMinimalRequestString = "?";
			Size = size;
			Style = style;
		}

		public bool Track(string text)
		{
			if (text == null || text.Length < 1)
			{
				return false;
			}
			bool changed = false;
			for (int i = 0; i < text.Length; i++)
			{
				if (!usedCharacters.ContainsKey(text[i]))
				{
					usedCharacters[text[i]] = true;
					mMinimalRequestString += text[i].ToString();
					changed = true;
				}
			}
			if (!changed)
			{
				return false;
			}
			mExpectingTextureRebuild = true;
			return true;
		}

		public void Reset()
		{
			RequestCount++;
			TextureRebuildCount++;
			usedCharacters.Clear();
			mMinimalRequestString = "?";
			mUnityCharInfo.Clear();
		}

		public bool MakeUnityRequest(bool internalChained3DUpdate = false)
		{
			RequestCount++;
			UnityFont.RequestCharactersInTexture(mMinimalRequestString, Size, Style);
			mUnityCharInfo.Clear();
			for (int i = 0; i < mMinimalRequestString.Length; i++)
			{
				char c = mMinimalRequestString[i];
				if (c == '\t' || c == '\n' || c == '\r')
				{
					continue;
				}
				CharacterInfo info;
				if (!UnityFont.GetCharacterInfo(c, out info, Size, Style))
				{
					return false;
				}
				mUnityCharInfo[c] = info;
			}
			if (internalChained3DUpdate)
			{
				return true;
			}
			bool result = true;
			for (int i = 0; i < m3DBoundTrackers.Count; i++)
			{
				UnityCharacterFontTracking tracker = m3DBoundTrackers[i];
				if (tracker != null && tracker.UsedBy3DUI)
				{
					result &= tracker.MakeUnityRequest(true);
				}
			}
			return result;
		}

		public void MakeUnityRequestDirect()
		{
			UnityFont.RequestCharactersInTexture(mMinimalRequestString, Size, Style);
		}

		public bool HasGlyph(char c)
		{
			return mUnityCharInfo.ContainsKey(c);
		}

		public bool TryGetCharInfo(char c, out CharacterInfo info)
		{
			// The shipped code returns the negated TryGetValue result (0x168f8b8).
			return !mUnityCharInfo.TryGetValue(c, out info);
		}

		public void RegisterCallbacksFor3DNodes()
		{
			if (m3DBoundTrackers.Contains(this))
			{
				return;
			}
			UsedBy3DUI = true;
			m3DBoundTrackers.Add(this);
			if (!EventRegistered)
			{
				EventRegistered = true;
				Font.textureRebuilt += OnFontTextureRebuilt;
			}
		}

		private void OnFontTextureRebuilt(Font changedFont)
		{
			if (FConfigCore.TestDisable7330)
			{
				mExpectingTextureRebuild = false;
			}
			if (changedFont != UnityFont && !mExpectingTextureRebuild && changedFont.name != UnityFont.name)
			{
				return;
			}
			mExpectingTextureRebuild = false;
			TextureRebuildCount++;
		}
	}

	public const char ASCII_NEWLINE = '\n';

	public const char ASCII_SPACE = ' ';

	public const char ASCII_NB_SPACE = ' ';

	public const char ASCII_HYPHEN_MINUS = '-';

	public const char ASCII_LINEHEIGHT_REFERENCE = 'M';

	public static readonly bool cConvertBitmapFontsToDynamicFontsWhenPossible;

	public static bool SpriteFontsDisabled;

	public static Dictionary<string, FFont> DynamicFonts;

	public static Dictionary<string, FFont> DynamicFontsHybridList;

	public static string DefaultStyleStroke;

	public static string DefaultStyleStrokeDrop;

	public static string DefaultStyle;

	public static string DefaultStyleLight;

	public static string DefaultStyleLightStroke;

	public static string Plain;

	public static string FixedWidthStroke;

	public static string CardName;

	public static string HUDNumbers;

	public static string Dynamic_DefaultStyleStroke;

	public static string Dynamic_DefaultStyleStrokeDrop;

	public static string Dynamic_DefaultStyle;

	public static string Dynamic_;

	public static string Dynamic_DefaultStyleLightStroke;

	public static string Dynamic_Plain;

	public static string Dynamic_FixedWidthStroke;

	public static string Dynamic_CardName;

	public static string Dynamic_HUDNumbers;

	public static string DynamicMax_DefaultStyleStroke;

	public static string DynamicMax_DefaultStyleStrokeDrop;

	public static string DynamicMax_DefaultStyle;

	public static string DynamicMax_;

	public static string DynamicMax_DefaultStyleLightStroke;

	public static string DynamicMax_Plain;

	public static string DynamicMax_FixedWidthStroke;

	public static string DynamicMax_CardName;

	public static string DynamicMax_HUDNumbers;

	public static string DefaultDynamicMax;

	public List<WeakReference> mDFontLabels = new List<WeakReference>();

	public Font mUnityFont;

	public bool mIsDynamic;

	public FKerningPairs mDyanimcKerningPairs;

	private FShader mShader = FShader.DefaultShader;

	private string _name;

	private FAtlasElement _element;

	private string _configPath;

	private Dictionary<char, FCharInfo> _charInfosByID = new Dictionary<char, FCharInfo>();

	private Dictionary<int, float> _kerningInfos;

	private float _lineHeight;

	private int _configWidth;

	private float mVerticalCenteringOffset;

	private float _configRatio;

	private float _configScale;

	private FTextParams _fontTextParams;

	public Vector4 mDynamicOffsetAdjustment = Vector4.zero;

	public float mDynamicSpacingMult = 1f;

	public float mDynamicPointScaleMult = 1f;

	public Vector4 ShadowColor;

	public Vector4 OutlineColor;

	public float mOutlineMult = 1f;

	public List<UnityCharacterFontTracking> mThisFontsTrackers = new List<UnityCharacterFontTracking>();

	private static Dictionary<int, UnityCharacterFontTracking> mFontTracking2;

	private static Dictionary<Font, int> sFontKeys;

	private static List<UnityCharacterFontTracking> m3DBoundTrackers;

	public int[] mAvailablePointSizes;

	public float mLineHeightMult;

	private string mUnityFontName;

	private static Dictionary<string, Font> sUnityFonts;

	public Dictionary<int, FFont> mSpritFontRefs = new Dictionary<int, FFont>();

	public static int DF_GlobalCounter;

	public int DFHashCode;

	public int DFInstanceID;

	public int CrossSizeAtlasVersion;

	private Dictionary<int, Dictionary<char, bool>> RequestedCharHistory = new Dictionary<int, Dictionary<char, bool>>();

	private Dictionary<int, int> AtlasVersion = new Dictionary<int, int>();

	public FShader FontShader => mShader;

	public string Name => _name;

	public Dictionary<char, FCharInfo> CharInfosByID => _charInfosByID;

	public Dictionary<int, float> KerningInfos => _kerningInfos;

	public float PointSize => _fontTextParams.PointSize;

	public float ScaledLineHeightOffset => _fontTextParams.scaledLineHeightOffset;

	public float ScaledKerningOffset => _fontTextParams.scaledKerningOffset;

	public float LineHeight
	{
		get
		{
			if (mIsDynamic && mUnityFont != null)
			{
				return (float)mUnityFont.lineHeight + _fontTextParams.lineHeightOffset;
			}
			return _lineHeight;
		}
	}

	public float VerticalCenteringOffset => mVerticalCenteringOffset;

	public FAtlasElement element => _element;

	static FFont()
	{
		cConvertBitmapFontsToDynamicFontsWhenPossible = true;
		DynamicFonts = new Dictionary<string, FFont>();
		DynamicFontsHybridList = new Dictionary<string, FFont>();
		DefaultStyleStroke = "DefaultStyleStroke";
		DefaultStyleStrokeDrop = "DefaultStyleStrokeDrop";
		DefaultStyle = "DefaultStyle";
		DefaultStyleLight = "DefaultStyleLight";
		DefaultStyleLightStroke = "DefaultStyleLightStroke";
		Plain = "Plain";
		FixedWidthStroke = "FixedWidthStroke";
		CardName = "CardName";
		HUDNumbers = "HUDNumbers";
		Dynamic_DefaultStyleStroke = "Dynamic_" + DefaultStyleStroke;
		Dynamic_DefaultStyleStrokeDrop = "Dynamic_" + DefaultStyleStrokeDrop;
		Dynamic_DefaultStyle = "Dynamic_" + DefaultStyle;
		Dynamic_ = "Dynamic_" + DefaultStyleLight;
		Dynamic_DefaultStyleLightStroke = "Dynamic_" + DefaultStyleLightStroke;
		Dynamic_Plain = "Dynamic_" + Plain;
		Dynamic_FixedWidthStroke = "Dynamic_" + FixedWidthStroke;
		Dynamic_CardName = "Dynamic_" + CardName;
		Dynamic_HUDNumbers = "Dynamic_" + HUDNumbers;
		DynamicMax_DefaultStyleStroke = "DynamicMax_" + DefaultStyleStroke;
		// Shipped code uses the "Dynamic_" prefix here (0x168ec08).
		DynamicMax_DefaultStyleStrokeDrop = "Dynamic_" + DefaultStyleStrokeDrop;
		DynamicMax_DefaultStyle = "DynamicMax_" + DefaultStyle;
		DynamicMax_ = "DynamicMax_" + DefaultStyleLight;
		DynamicMax_DefaultStyleLightStroke = "DynamicMax_" + DefaultStyleLightStroke;
		DynamicMax_Plain = "DynamicMax_" + Plain;
		DynamicMax_FixedWidthStroke = "DynamicMax_" + FixedWidthStroke;
		DynamicMax_CardName = "DynamicMax_" + CardName;
		DynamicMax_HUDNumbers = "DynamicMax_" + HUDNumbers;
		DefaultDynamicMax = "DynamicMax_" + DefaultStyleStroke;
		mFontTracking2 = new Dictionary<int, UnityCharacterFontTracking>();
		sFontKeys = new Dictionary<Font, int>();
		m3DBoundTrackers = new List<UnityCharacterFontTracking>();
		sUnityFonts = new Dictionary<string, Font>();
		DF_GlobalCounter = 0;
	}

	protected FFont(string name, FAtlasElement element)
	{
		_name = name;
		_element = element;
	}

	public FFont(string name, FAtlasElement element, string configPath, FTextParams fontTextParams, float configScale)
	{
		_name = name;
		_element = element;
		_configPath = configPath;
		_fontTextParams = fontTextParams;
		_configScale = configScale;
		LoadAndParseConfigFile();
	}

	public static string ConvertToDynamicFont(string fontName)
	{
		string dynamicName = "Dynamic_" + fontName.Replace("Dynamic_", "").Replace("DynamicMax_", "");
		if (DynamicFonts.ContainsKey(dynamicName))
		{
			return dynamicName;
		}
		return "Dynamic_" + DefaultStyleStroke;
	}

	public static string ConvertToDynamicFontHybridList(string fontName)
	{
		string baseName = fontName.Replace("Dynamic_", "").Replace("DynamicMax_", "");
		string dynamicName = "Dynamic_" + baseName;
		if (DynamicFontsHybridList.ContainsKey(dynamicName))
		{
			return dynamicName;
		}
		return baseName;
	}

	public static string GetMaxFallBackFont(string fontName)
	{
		if (!FConfigCore.DynamicMaxFonts)
		{
			return fontName;
		}
		string maxName = "DynamicMax_" + fontName.Replace("Dynamic_", "").Replace("DynamicMax_", "");
		if (DynamicFonts.ContainsKey(maxName))
		{
			return maxName;
		}
		// Shipped code prefixes DefaultDynamicMax (already "DynamicMax_...") again (0x168ccbc).
		return "DynamicMax_" + DefaultDynamicMax;
	}

	public static string ConvertToDynamicFontToSpriteFont(string fontName)
	{
		return fontName.Replace("Dynamic_", "");
	}

	public static string GetFallBackFont(FFont fontToConsider, string fontStyleName, string failString, out bool isMaxFallback)
	{
		isMaxFallback = false;
		if (!fontToConsider.mIsDynamic)
		{
			string dynamicName = ConvertToDynamicFont(fontStyleName);
			FFont dynamicFont = null;
			if (DynamicFonts.TryGetValue(dynamicName, out dynamicFont) && !dynamicFont.HasMissingChars(failString))
			{
				return dynamicName;
			}
		}
		isMaxFallback = true;
		return GetMaxFallBackFont(fontStyleName);
	}

	public static void LoadAllDynamicFonts()
	{
		HybridListAlwaysDynamic(FixedWidthStroke);
	}

	public bool TryGetVisualAdjustmentsBySize(float pointSize, out Vector4 offset)
	{
		offset = new Vector4(0f, 0f, 1f, 1f);
		if (mShader != FShader.DynamicFontOutline && mShader != FShader.DynamicFontOutlineShadow)
		{
			return false;
		}
		int size = (int)pointSize;
		if (_name.Contains("DefaultStyleStrokeDrop"))
		{
			if (size <= 24)
			{
				offset = new Vector4(90f, 385f, 2f, 0.6f);
			}
			else if (size <= 36)
			{
				offset = new Vector4(124f, 321f, 2f, 0.8f);
			}
			else
			{
				offset = (size > 48) ? new Vector4(158f, 262f, 2f, 1f) : new Vector4(163f, 282f, 2f, 1f);
			}
		}
		else if (_name.Contains("DefaultStyleStroke"))
		{
			if (size <= 16)
			{
				offset = new Vector4(233f, 47f, 1f, 1f);
			}
			else if (size <= 24)
			{
				offset = new Vector4(85f, 86f, 1f, 1f);
			}
			else if (size <= 36)
			{
				offset = new Vector4(85f, 90f, 1f, 1f);
			}
			else
			{
				offset = (size > 48) ? new Vector4(80f, 106f, 1f, 1f) : new Vector4(93f, 81f, 1f, 1f);
			}
		}
		else if (_name.Contains("DefaultStyleLightStroke"))
		{
			if (size <= 16)
			{
				offset = new Vector4(207f, 59f, 1f, 1f);
			}
			else if (size <= 20)
			{
				offset = new Vector4(50f, 66f, 1f, 1f);
			}
			else if (size <= 24)
			{
				offset = new Vector4(176f, 88f, 1f, 1f);
			}
			else
			{
				offset = (size > 36) ? new Vector4(138f, 108f, 1f, 1f) : new Vector4(163f, 102f, 1f, 1f);
			}
		}
		else if (_name.Contains("CardName"))
		{
			if (size <= 24)
			{
				offset = new Vector4(103f, 88f, 1f, 1f);
			}
			else
			{
				offset = (size > 36) ? new Vector4(134f, 100f, 1f, 1f) : new Vector4(153f, 108f, 1f, 1f);
			}
		}
		else if (_name.Contains("HUDNumbers"))
		{
			if (size <= 36)
			{
				offset = new Vector4(88f, 141f, 1f, 1f);
			}
			else
			{
				offset = (size > 48) ? new Vector4(48f, 56f, 1f, 1f) : new Vector4(86f, 124f, 1f, 1f);
			}
		}
		else if (_name.Contains("FixedWidthStroke"))
		{
			offset = new Vector4(0f, 300f, 1f, 1f);
		}
		return true;
	}

	public bool TryGetTypesettingOffsetsBySize(float pointSize, out Vector3 offset)
	{
		offset = Vector3.zero;
		int size = (int)pointSize;
		if (mShader != FShader.DynamicFontOutline && mShader != FShader.DynamicFontOutlineShadow)
		{
			return false;
		}
		// Note: "DefaultStyle" matches every DefaultStyle* name, so the following DefaultStyle* branches are
		// unreachable in the shipped code as well (0x168d688); kept for fidelity.
		if (_name.Contains("DefaultStyle"))
		{
			offset = new Vector3(0f, 0f, -2f);
			return true;
		}
		if (_name.Contains("DefaultStyleStrokeDrop"))
		{
			if (size <= 24)
			{
				offset = new Vector3(1f, -10f, 4f);
			}
			else if (size <= 36)
			{
				offset = new Vector3(1f, -11f, 7f);
			}
			else if (size <= 48)
			{
				offset = new Vector3(0f, -12f, 9f);
			}
			else if (size <= 72)
			{
				offset = new Vector3(0f, -11f, 11f);
			}
			else
			{
				offset = new Vector3(2f, -1f, 5f);
			}
			return true;
		}
		if (_name.Contains("DefaultStyleStroke"))
		{
			if (size <= 16)
			{
				offset = new Vector3(1f, -2f, 4f);
			}
			else if (size <= 24)
			{
				offset = new Vector3(2f, -2f, 3f);
			}
			else if (size <= 36)
			{
				offset = new Vector3(2f, -2f, 4f);
			}
			else if (size <= 48)
			{
				offset = new Vector3(2f, -2f, 5f);
			}
			else
			{
				offset = new Vector3(2f, -1f, 5f);
			}
			return true;
		}
		if (_name.Contains("DefaultStyleLightStroke"))
		{
			if (size <= 16)
			{
				offset = new Vector3(1f, -1f, 4f);
			}
			else if (size <= 20)
			{
				offset = new Vector3(2f, -2f, 2f);
			}
			else if (size <= 24)
			{
				offset = new Vector3(1f, -2f, 4f);
			}
			else if (size <= 36)
			{
				offset = new Vector3(1f, -2f, 5f);
			}
			else
			{
				offset = new Vector3(1f, -1f, 6f);
			}
			return true;
		}
		if (_name.Contains("HUDNumbers"))
		{
			if (size <= 36)
			{
				offset = new Vector3(2f, 1f, 5f);
				return true;
			}
			if (size <= 48)
			{
				offset = new Vector3(2f, 1f, 4f);
				return true;
			}
			if (size <= 64)
			{
				offset = new Vector3(2f, 2f, 4f);
				return true;
			}
		}
		if (_name.Contains("CardName"))
		{
			if (size <= 24)
			{
				offset = new Vector3(2f, -2f, 3f);
			}
			else
			{
				offset = new Vector3(1f, -2f, (size <= 36) ? 5f : 6f);
			}
			return true;
		}
		if (!_name.Contains("FixedWidthStroke"))
		{
			return false;
		}
		if (size <= 8 || (size >= 11 && size <= 12))
		{
			offset = new Vector3(0f, -4f, 0f);
		}
		else if (size <= 10 || size <= 14)
		{
			offset = new Vector3(0f, -3f, 0f);
		}
		else if (size <= 20)
		{
			offset = new Vector3(0f, -8f, 0f);
		}
		else
		{
			offset = Vector3.zero;
		}
		return true;
	}

	private static void HybridListAlwaysDynamic(string baseFontStyle)
	{
		string dynamicName = "Dynamic_" + baseFontStyle;
		if (DynamicFonts.ContainsKey(dynamicName) && !DynamicFontsHybridList.ContainsKey(dynamicName))
		{
			DynamicFontsHybridList.Add(dynamicName, DynamicFonts[dynamicName]);
		}
	}

	private static int[] SetEquivalentStaticSizes(string name, FFont parent)
	{
		string styleName = name.Replace("Dynamic_", "");
		FFontStyle style = Futile.AtlasManager.GetFontStyle(styleName);
		if (style == null)
		{
			return null;
		}
		List<int> sizes = style.GetAvailableSizes();
		if (sizes == null || sizes.Count < 1)
		{
			return null;
		}
		sizes.Sort();
		for (int i = 0; i < sizes.Count; i++)
		{
			float scaleInverse = 1f;
			FFont spriteFont = Futile.AtlasManager.GetBaseFontWithStyleAndSize(styleName, sizes[i], out scaleInverse);
			if (spriteFont != null)
			{
				parent.mSpritFontRefs[sizes[i]] = spriteFont;
			}
		}
		return sizes.ToArray();
	}

	private void UpdateFromSpriteFont(string name)
	{
		FFont spriteFont = Futile.AtlasManager.GetFontWithName(name.Replace("Dynamic_", ""));
		if (spriteFont != null)
		{
			mVerticalCenteringOffset = spriteFont.mVerticalCenteringOffset;
		}
	}

	public static void ClearDynamicFonts()
	{
		DynamicFonts.Clear();
	}

	public static FFont LoadDynamicFont(string fontName, FShader shader, string fontResource, float lineHeightOffset, Vector4 offsetAdjustment, float spacingMult, Vector4 shadowColor, Vector4 outlineColor, float outlineMult, float scaleHack)
	{
		string name = "Dynamic_" + fontName;
		FFont font = null;
		if (DynamicFonts.TryGetValue(name, out font))
		{
			return font;
		}
		Font unityFont = null;
		if (!sUnityFonts.TryGetValue(fontResource, out unityFont))
		{
			unityFont = FAssetBundleManager.Get<Font>(fontResource);
			sUnityFonts[fontResource] = unityFont;
		}
		if (unityFont == null)
		{
			return null;
		}
		FAtlas atlas = Futile.AtlasManager.LoadAtlasFromTexture(name, unityFont.material.mainTexture);
		FAtlasElement fontElement = Futile.AtlasManager.GetElementWithName(name);
		atlas.UnloadStrategy = FAtlas.eUnloadStrategy.NeverUnload;
		font = new FFont(name, fontElement);
		font.mUnityFont = unityFont;
		font.mShader = shader;
		font.mIsDynamic = true;
		DynamicFonts.Add(name, font);
		unityFont.material.mainTexture.filterMode = FilterMode.Point;
		unityFont.material.mainTexture.anisoLevel = 0;
		Font.textureRebuilt += delegate(Font f)
		{
			font.OnFontRebuilt(f);
		};
		font.mDynamicOffsetAdjustment = offsetAdjustment;
		font.mDynamicSpacingMult = spacingMult;
		font.ShadowColor = shadowColor;
		font.mOutlineMult = outlineMult;
		font.OutlineColor = outlineColor;
		TextAsset kerning = FAssetBundleManager.Get<TextAsset>(fontResource + "Kern");
		if (kerning != null)
		{
			MemoryStream stream = new MemoryStream(kerning.bytes);
			FKerningPairs pairs = new FKerningPairs();
			pairs.Read(stream);
			font.mDyanimcKerningPairs = pairs;
		}
		font.mDynamicPointScaleMult = scaleHack;
		font.mAvailablePointSizes = SetEquivalentStaticSizes(name, font);
		font.mUnityFontName = fontResource.Replace("Fonts/", "");
		font._fontTextParams = new FTextParams();
		font._fontTextParams.lineHeightOffset = lineHeightOffset;
		return font;
	}

	public static void ClearDFLabelTracking()
	{
		foreach (KeyValuePair<string, FFont> pair in DynamicFonts)
		{
			pair.Value.ClearOldLabelRefs();
		}
	}

	private int ClearOldLabelRefs()
	{
		int removed = 0;
		for (int i = mDFontLabels.Count - 1; i >= 0; i--)
		{
			if (!mDFontLabels[i].IsAlive)
			{
				mDFontLabels.RemoveAt(i);
				removed++;
			}
		}
		return removed;
	}

	public bool IsTracking(FLabel node)
	{
		for (int i = mDFontLabels.Count - 1; i >= 0; i--)
		{
			WeakReference reference = mDFontLabels[i];
			if (reference.IsAlive && reference.Target as FLabel == node)
			{
				return true;
			}
		}
		return false;
	}

	private void OnFontRebuilt(Font a)
	{
		for (int i = mDFontLabels.Count - 1; i >= 0; i--)
		{
			WeakReference reference = mDFontLabels[i];
			if (!reference.IsAlive)
			{
				mDFontLabels.RemoveAt(i);
				continue;
			}
			FLabel label = reference.Target as FLabel;
			if (label == null)
			{
				continue;
			}
			if (label.Parent is FTextEntry)
			{
				label.ForceUpdateUVs();
			}
			if (label.DebugMe)
			{
				label.DebugMeConditional();
			}
			label.Invalidate();
			label.mBadTextureFrame = Time.frameCount;
		}
	}

	private void LoadAndParseConfigFile()
	{
		TextAsset asset = FAssetBundleManager.Get<TextAsset>(_configPath);
		if (asset == null)
		{
			FAssert.Fail("Couldn't find font config file!");
			return;
		}
		string[] separators = new string[1] { "\n" };
		string[] lines = asset.text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		if (lines.Length < 2)
		{
			separators[0] = "\r\n";
			lines = asset.text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			if (lines.Length < 2)
			{
				separators[0] = "\r";
				lines = asset.text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			}
		}
		FAssert.That(lines.Length > 1, "Your font file is messed up!");
		Vector2 textureSize = new Vector2(2048f, 2048f);
		if (!SpriteFontsDisabled)
		{
			textureSize = _element.Atlas.TextureSize;
		}
		for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
		{
			string[] words = lines[lineIndex].Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (words[0] == "common")
			{
				_configWidth = words[3].Split('=')[1].ParseInt();
				float ratio = 1f;
				if (!SpriteFontsDisabled)
				{
					ratio = _element.SourceSize.x * Futile.ResourceScale / (float)_configWidth;
				}
				_configRatio = ratio;
				_fontTextParams.PointSize = _fontTextParams.PointSize * (Futile.ResourceScale / _configScale);
				_lineHeight = _configRatio * (float)words[1].Split('=')[1].ParseInt();
				if (SpriteFontsDisabled)
				{
					return;
				}
			}
			else if (words[0] == "chars")
			{
			}
			else if (words[0] == "char")
			{
				FCharInfo charInfo = new FCharInfo();
				for (int w = 1; w < words.Length; w++)
				{
					string[] parts = words[w].Split('=');
					string partName = parts[0];
					if (partName == "letter" || partName == "\r" || parts.Length < 2)
					{
						continue;
					}
					int value = parts[1].ParseInt();
					if (partName == "id")
					{
						charInfo.charID = (char)value;
					}
					else if (partName == "x")
					{
						charInfo.x = _configRatio * (float)value - _element.SourceRect.x;
					}
					else if (partName == "y")
					{
						charInfo.y = _configRatio * (float)value - _element.SourceRect.y;
					}
					else if (partName == "width")
					{
						charInfo.width = _configRatio * (float)value;
					}
					else if (partName == "height")
					{
						charInfo.height = _configRatio * (float)value;
					}
					else if (partName == "xoffset")
					{
						charInfo.offsetX = _configRatio * (float)value;
					}
					else if (partName == "yoffset")
					{
						charInfo.offsetY = _configRatio * (float)value;
					}
					else if (partName == "xadvance")
					{
						charInfo.xadvance = _configRatio * (float)value;
					}
				}
				Rect uvRect = _element.uvRect;
				if (!_element.IsRotated)
				{
					float uvLeft = uvRect.x + charInfo.x / textureSize.x;
					float uvBottom = (textureSize.y - charInfo.y - charInfo.height) / textureSize.y - (1f - (uvRect.y + uvRect.height));
					float uvTop = charInfo.height / textureSize.y + uvBottom;
					float uvRight = uvLeft + charInfo.width / textureSize.x;
					charInfo.uvBottomLeft = new Vector2(uvLeft, uvBottom);
					charInfo.uvTopLeft = new Vector2(uvLeft, uvTop);
					charInfo.uvTopRight = new Vector2(uvRight, uvTop);
					charInfo.uvBottomRight = new Vector2(uvRight, uvBottom);
				}
				else
				{
					float v0 = uvRect.y + uvRect.height - (charInfo.x + charInfo.width) / textureSize.y;
					float u0 = uvRect.width + uvRect.x - (charInfo.y + charInfo.height) / textureSize.x;
					float v1 = charInfo.width / textureSize.y + v0;
					float u1 = charInfo.height / textureSize.x + u0;
					charInfo.uvTopLeft = new Vector2(u1, v1);
					charInfo.uvTopRight = new Vector2(u1, v0);
					charInfo.uvBottomRight = new Vector2(u0, v0);
					charInfo.uvBottomLeft = new Vector2(u0, v1);
				}
				_charInfosByID[charInfo.charID] = charInfo;
			}
			else if (words[0] == "kernings")
			{
				_kerningInfos = new Dictionary<int, float>(words[1].Split('=')[1].ParseInt());
			}
			else if (words[0] == "kerning")
			{
				int first = -1;
				int second = -1;
				float amount = 0f;
				for (int w = 1; w < words.Length; w++)
				{
					string[] parts = words[w].Split('=');
					if (parts.Length < 2)
					{
						continue;
					}
					string partName = parts[0];
					int value = parts[1].ParseInt();
					if (partName == "first")
					{
						first = value;
					}
					else if (partName == "second")
					{
						second = value;
					}
					else if (partName == "amount")
					{
						amount = _configRatio * (float)value;
					}
				}
				if (first >= 0 && second >= 0)
				{
					_kerningInfos[second | (first << 16)] = amount;
				}
			}
			else if (words[0] == "centering")
			{
				mVerticalCenteringOffset = words[1].Split('=')[1].ParseFloat();
			}
		}
		if (_charInfosByID.Get(' ') != null)
		{
			_charInfosByID[' '].offsetX = 0f;
			_charInfosByID[' '].offsetY = 0f;
			_charInfosByID[' '] = _charInfosByID[' '];
		}
	}

	public HashSet<char> GetMissingChars(string text)
	{
		HashSet<char> missing = new HashSet<char>();
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if (_charInfosByID.Get(c) == null && c != '\t' && c != '\n' && c != '\r')
			{
				missing.Add(c);
			}
		}
		return missing;
	}

	public bool HasMissingChars(string text)
	{
		if (text == null)
		{
			return false;
		}
		if (!mIsDynamic)
		{
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (_charInfosByID.Get(c) == null && c != '\t' && c != '\n' && c != '\r')
				{
					return true;
				}
			}
			return false;
		}
		if (mUnityFont == null)
		{
			return true;
		}
		if (text.Length <= 0)
		{
			return false;
		}
		bool hasAll;
		int index = 0;
		do
		{
			hasAll = mUnityFont.HasCharacter(text[index]);
		}
		while (hasAll && ++index < text.Length);
		return !hasAll;
	}

	public bool HasCharacter(char letter)
	{
		if (!mIsDynamic)
		{
			return _charInfosByID.Get(letter) != null;
		}
		if (mUnityFont == null)
		{
			return false;
		}
		return mUnityFont.HasCharacter(letter);
	}

	public int GetAlasVersion(int size)
	{
		int version = 0;
		if (!AtlasVersion.TryGetValue(size, out version))
		{
			version = 0;
		}
		return version;
	}

	public bool RequestCharacters(string text, int size)
	{
		Dictionary<char, bool> history = null;
		if (!RequestedCharHistory.TryGetValue(size, out history))
		{
			history = new Dictionary<char, bool>();
			RequestedCharHistory[size] = history;
			AtlasVersion[size] = 0;
		}
		int version = AtlasVersion[size];
		for (int i = 0; i < text.Length; i++)
		{
			if (!history.ContainsKey(text[i]))
			{
				CrossSizeAtlasVersion++;
				version++;
				history[text[i]] = true;
			}
		}
		AtlasVersion[size] = version;
		if (mUnityFont == null)
		{
			return false;
		}
		Texture texture = mUnityFont.material.mainTexture;
		int instanceID = texture.GetInstanceID();
		DFHashCode = mUnityFont.material.mainTexture.GetHashCode();
		DFInstanceID = instanceID;
		return false;
	}

	public void PerFrameRequestCharacters(UnityCharacterFontTracking tracking)
	{
		int frame = Time.frameCount;
		if (tracking.LastTrackedFrame != frame)
		{
			tracking.MakeUnityRequestDirect();
			tracking.LastTrackedFrame = frame;
		}
	}

	public bool WrappedRequestCharactersInTexture(string text, int size, FontStyle unityFontStyle)
	{
		UnityCharacterFontTracking tracking = GetTracking(mUnityFont, size, unityFontStyle, mUnityFontName);
		int frame = Time.frameCount;
		int lastFrame = tracking.LastTrackedFrame;
		bool changed = tracking.Track(text);
		bool result = true;
		if (lastFrame == frame)
		{
			if (changed)
			{
				Futile.ForceRenderActiveNodes();
				OnFontRebuilt(mUnityFont);
				result = tracking.MakeUnityRequest();
			}
		}
		else
		{
			if (changed)
			{
				Futile.ForceRenderActiveNodes();
				OnFontRebuilt(mUnityFont);
			}
			tracking.MakeUnityRequestDirect();
			tracking.LastTrackedFrame = frame;
		}
		if (!mThisFontsTrackers.Contains(tracking))
		{
			mThisFontsTrackers.Add(tracking);
		}
		return result;
	}

	public bool WrappedGetCharacterInfo(char letter, out CharacterInfo dynCharInfo, int dynamicFontRequestSize, FontStyle dynamicFontStyle)
	{
		return mUnityFont.GetCharacterInfo(letter, out dynCharInfo, dynamicFontRequestSize, dynamicFontStyle);
	}

	private static int UnityFontKey(Font f)
	{
		int key = 0;
		if (!sFontKeys.TryGetValue(f, out key))
		{
			// Shipped code stores Count + 1 but returns 0 on this first lookup (0x168eb04).
			sFontKeys[f] = sFontKeys.Count + 1;
		}
		return key;
	}

	public static UnityCharacterFontTracking GetTracking(Font font, int size, FontStyle unityFontStyle, string trackingName)
	{
		int fontKey = UnityFontKey(font);
		int key = size * 100 + (int)unityFontStyle * 10000 + fontKey;
		UnityCharacterFontTracking tracking = null;
		if (mFontTracking2.TryGetValue(key, out tracking))
		{
			return tracking;
		}
		tracking = new UnityCharacterFontTracking();
		tracking.Init(font, size, unityFontStyle, trackingName);
		mFontTracking2[key] = tracking;
		return tracking;
	}
}
