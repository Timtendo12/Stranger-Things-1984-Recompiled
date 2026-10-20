using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FLoc @ 0x1658e98..0x165d540, Ghidra vaddr).
public class FLoc
{
	protected static Dictionary<string, string> mStrings;

	protected static Dictionary<string, string> mRemaps;

	protected static Dictionary<string, string> mImageNames;

	protected static Dictionary<string, string> mFontColors;

	protected static List<string> mAdditionalAtlases;

	public static string FontXml;

	protected static string mLanguageDirectoryStem;

	protected static string mLanguageDirectory;

	protected static string mImageNamesLanguageDirectoryStem;

	protected static CultureInfo mUICulture;

	[CompilerGenerated]
	private static Func<XmlNode, string> m_mSpecialXMLNodeFunc;

	public static bool IsRightToLeft;

	protected static string mSystemLocale;

	protected static string mSystemLanguage;

	protected static CultureInfo mSystemCulture;

	protected static string mUILanguage;

	protected static string mUILanguageShort;

	protected static string mDesiredLanguage;

	public static List<string> AdditionalAtlases => mAdditionalAtlases;

	public static string LanguageDirectoryStem => mLanguageDirectoryStem;

	public static string SystemLocale
	{
		get
		{
			if (mSystemLocale == null)
			{
				// PC: the original queried java.util.Locale.getDefault().toString() over JNI; use the OS UI culture instead.
				string name = CultureInfo.CurrentUICulture.Name;
				FLog.UnityLog(string.Format("Fetching java.util.Locale returned: {0}", name));
				if (!string.IsNullOrEmpty(name))
				{
					mSystemLocale = name;
					FLog.UnityWarning("Android call to Locale.toString returned " + mSystemLocale);
				}
				else
				{
					FLog.UnityWarning("Could not call getDefault on 'java.util.Locale' class");
				}
				if (!string.IsNullOrEmpty(mSystemLocale))
				{
					mSystemLocale = mSystemLocale.Replace("_", "-");
				}
				else
				{
					mSystemLocale = "en-US";
				}
			}
			return mSystemLocale;
		}
	}

	public static string SystemLanguage
	{
		get
		{
			if (mSystemLanguage == null)
			{
				// PC: the original queried java.util.Locale.getDefault().getLanguage() over JNI; use the OS UI culture instead.
				string language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
				if (!string.IsNullOrEmpty(language) && language != "iv")
				{
					mSystemLanguage = language;
					FLog.UnityWarning("Android call to Locale.getLanguage returned " + mSystemLanguage);
				}
				else
				{
					FLog.UnityWarning("Could not call getLanguage on 'java.util.Locale' class");
				}
				CorrectLocalaleAndLanguageStrings(ref mSystemLocale, ref mSystemLanguage);
			}
			return mSystemLanguage;
		}
	}

	public static CultureInfo SystemCulture
	{
		get
		{
			if (mSystemCulture == null)
			{
				mSystemCulture = CultureInfo.CreateSpecificCulture(SystemLocale);
			}
			return mSystemCulture;
		}
	}

	public static CultureInfo UICulture => mUICulture;

	public static string UILanguage => mUILanguage;

	public static string UILanguageShort => mUILanguageShort;

	public static string DesiredLanguage => mDesiredLanguage;

	protected static event Func<XmlNode, string> mSpecialXMLNodeFunc
	{
		add
		{
			m_mSpecialXMLNodeFunc = (Func<XmlNode, string>)Delegate.Combine(m_mSpecialXMLNodeFunc, value);
		}
		remove
		{
			m_mSpecialXMLNodeFunc = (Func<XmlNode, string>)Delegate.Remove(m_mSpecialXMLNodeFunc, value);
		}
	}

	public static event Func<XmlNode, string> SpecialXMLNodeFunc
	{
		add
		{
			mSpecialXMLNodeFunc -= value;
			mSpecialXMLNodeFunc += value;
		}
		remove
		{
			mSpecialXMLNodeFunc -= value;
		}
	}

	static FLoc()
	{
		mStrings = new Dictionary<string, string>();
		mRemaps = new Dictionary<string, string>();
		mImageNames = new Dictionary<string, string>();
		mFontColors = new Dictionary<string, string>();
		mAdditionalAtlases = new List<string>();
		mUICulture = CultureInfo.InvariantCulture;
		m_mSpecialXMLNodeFunc = null;
		mSystemLocale = null;
		IsRightToLeft = false;
		mSystemLanguage = null;
		mUILanguage = "en";
		mUILanguageShort = "en";
		mDesiredLanguage = "en";
	}

	public static string GetString(string name, bool quiet = false)
	{
		string value = null;
		mStrings.TryGetValue(name, out value);
		if (value == null)
		{
			int length = name.Length;
			int suffixLength = 1;
			for (int i = length - 1; i >= 0; i--)
			{
				string remapped = mRemaps.Get(name.Substring(0, i));
				if (remapped != null)
				{
					string suffix = name.Substring(i, suffixLength);
					value = mStrings.Get(remapped + suffix);
					if (value != null)
					{
						return value;
					}
				}
				suffixLength++;
			}
			if (!quiet)
			{
				FLog.UnityWarning("did not find string with ID: " + name);
			}
			value = "";
		}
		return value;
	}

	public static string Format(string formatName, params object[] arguments)
	{
		return string.Format(mUICulture, GetString(formatName), arguments);
	}

	public static string FormatRaw(string rawFormatString, params object[] arguments)
	{
		return string.Format(mUICulture, rawFormatString, arguments);
	}

	protected static string PseudoLocString(string str)
	{
		return "<<" + str + ">>";
	}

	public static string GetImageName(string name, bool quiet = false)
	{
		string value = null;
		mImageNames.TryGetValue(name, out value);
		if (value == null)
		{
			if (!quiet)
			{
				FLog.UnityWarning("did not find image name with ID: " + name);
			}
			value = "";
		}
		return value;
	}

	public static string GetFontColorAsHexString(string fontColorName)
	{
		string value = null;
		mFontColors.TryGetValue(fontColorName, out value);
		if (value != null)
		{
			return value;
		}
		return "";
	}

	public static Color GetFontColor(string fontColorName, Color defaultColor)
	{
		string hex = GetFontColorAsHexString(fontColorName);
		if (!string.IsNullOrEmpty(hex))
		{
			return FUtil.ColorFromHex(hex);
		}
		return defaultColor;
	}

	public static void SetLocaleAndLanguage(string locale, string language)
	{
		CorrectLocalaleAndLanguageStrings(ref locale, ref language);
		FLog.Log("Setting locale to: " + locale, true);
		FLog.Log("Setting language to: " + language, true);
		mUILanguage = language;
		mDesiredLanguage = language;
		try
		{
			mUICulture = CultureInfo.CreateSpecificCulture(locale);
		}
		catch (System.ArgumentException)
		{
			// PC port: language folders such as "cz" are not .NET culture names; use the closest culture
			mUICulture = CultureInfo.CreateSpecificCulture(locale == "cz" ? "cs-CZ" : "en-US");
		}
		mLanguageDirectoryStem = "Languages/" + locale;
		if (!LoadStrings())
		{
			FLog.UnityError("Could not find string table for locale! Trying language.");
			mLanguageDirectoryStem = "Languages/" + language;
			if (!LoadStrings())
			{
				FLog.UnityError("Could not find string table for language! Trying to split language to remove locale code.");
				mUILanguage = language.Split('-')[0];
				mLanguageDirectoryStem = "Languages/" + mUILanguage;
				if (!LoadStrings())
				{
					FLog.UnityError("Could not find string table for language! Trying to split locale to get language code.");
					mUILanguage = locale.Split('-')[0];
					mLanguageDirectoryStem = "Languages/" + mUILanguage;
					if (!LoadStrings())
					{
						FLog.UnityError("Could not find string table for locale!  Defaulting to English.");
						mLanguageDirectoryStem = "Languages/" + "en";
						mUILanguage = language;
						if (!LoadStrings())
						{
							FLog.UnityError("Tried to fall back to english and still could not load strings.");
						}
					}
				}
			}
		}
		mUILanguageShort = mUILanguage.Split('-')[0];
		if (!LoadImageNames("Languages/" + "en/LocImageNames"))
		{
			FLog.Error("Could not load english LocImageNames!");
		}
		mImageNamesLanguageDirectoryStem = "Languages/" + locale;
		if (LoadImageNames(mImageNamesLanguageDirectoryStem + "/LocImageNames"))
		{
			return;
		}
		FLog.Error("Could not find image name table for locale! Trying language.");
		mImageNamesLanguageDirectoryStem = "Languages/" + language;
		if (LoadImageNames(mImageNamesLanguageDirectoryStem + "/LocImageNames"))
		{
			return;
		}
		FLog.Error("Could not find image name table for language! Trying to split language to remove locale code.");
		mImageNamesLanguageDirectoryStem = "Languages/" + language.Split('-')[0];
		if (!LoadImageNames(mImageNamesLanguageDirectoryStem + "/LocImageNames"))
		{
			FLog.Error("Could not find image name table for language! Trying to split locale to get language code.");
			string localeLanguage = locale.Split('-')[0];
			mImageNamesLanguageDirectoryStem = "Languages/" + localeLanguage;
			if (localeLanguage != "en" && !LoadImageNames(mImageNamesLanguageDirectoryStem + "/LocImageNames"))
			{
				FLog.Error("Could not find image name table for locale!  Defaulting to English.");
				mImageNamesLanguageDirectoryStem = "Languages/" + "en";
			}
		}
		FLog.Log("Using image names language folder: " + mImageNamesLanguageDirectoryStem);
	}

	private static void CorrectLocalaleAndLanguageStrings(ref string locale, ref string language)
	{
		locale = string.IsNullOrEmpty(locale) ? "en-US" : locale.Replace("_", "-");
		if (string.IsNullOrEmpty(language))
		{
			language = "en";
		}
		else if (language.Contains("zh"))
		{
			language = locale.Contains("Hans") ? "zh-CN" : "zh-TW";
		}
		else if (language == "in")
		{
			language = "id";
		}
		else if (language == "cs")
		{
			language = "cz";
		}
		else if (language == "iw")
		{
			language = "he";
		}
	}

	protected static bool LoadStrings()
	{
		mStrings.Clear();
		if (mLanguageDirectoryStem == null)
		{
			FLog.UnityError("No language file found!");
			return false;
		}
		string path = mLanguageDirectoryStem + "/StringTable";
		string text = FUtil.LoadTextAsset(path);
		if (text == null)
		{
			FLog.UnityError("Failed to load string table: " + path);
			return false;
		}
		bool isArabic = mLanguageDirectoryStem.Contains("/ar");
		bool isHebrew = mLanguageDirectoryStem.Contains("/he");
		bool isHindi = mLanguageDirectoryStem.Contains("/hi");
		IsRightToLeft = isArabic | isHebrew;
		XmlReader reader = XmlReader.Create(new StringReader(text));
		reader.MoveToContent();
		while (reader.Read())
		{
			if (reader.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			if (reader.Name == "string")
			{
				string name = reader.GetAttribute("name");
				if (name == null)
				{
					continue;
				}
				string cEscape = reader.GetAttribute("CEscape");
				bool unescape = cEscape == null || cEscape.ParseBool(true);
				string value = reader.ReadInnerXml();
				bool isThai = mLanguageDirectoryStem.Contains("/th");
				string fixedValue = null;
				bool trimFixed = true;
				if (isArabic)
				{
					fixedValue = RightLangSupport.FixAr(value);
					if (name == "TheGame")
					{
						FLog.UnityLog("Fix Ar " + name + ": " + value + " :: " + fixedValue);
					}
				}
				else if (isHebrew)
				{
					fixedValue = RightLangSupport.FixHe(value);
					if (name == "TheGame")
					{
						FLog.UnityLog("Fix He " + name + ": " + value + " :: " + fixedValue);
					}
				}
				else if (isThai && ThaiFontAdjuster.IsThaiString(value))
				{
					fixedValue = ThaiFontAdjuster.Adjust(value);
					if (name == "TheGame")
					{
						FLog.UnityLog("Fix Thai " + name + ": " + value + " :: " + fixedValue);
					}
				}
				else
				{
					trimFixed = false;
					if (isHindi)
					{
						value = HindiCorrector.Correct(value);
					}
				}
				if (trimFixed)
				{
					value = fixedValue.Trim();
				}
				if (unescape)
				{
					value = Regex.Unescape(value);
				}
				mStrings.Add(name, value);
			}
			else if (reader.Name == "remap")
			{
				string from = reader.GetAttribute("from");
				if (from == null)
				{
					FLog.UnityError("remap missing from");
					continue;
				}
				string to = reader.GetAttribute("to");
				if (to == null)
				{
					FLog.UnityError("remap missing to");
					continue;
				}
				mRemaps.Add(from, to);
			}
		}
		FLog.UnityLog(string.Format("LoadStrings: {0} :: {1} \n {2} :: {3}  :: {4}  :: {5}  :: {6}", (mStrings != null) ? new int?(mStrings.Count) : null, mStrings["TheGame"], IsRightToLeft, isArabic, isHebrew, isHindi, mLanguageDirectoryStem.Contains("/th")));
		return true;
	}

	protected static void LoadStringsXml(XmlElement root)
	{
		foreach (XmlNode node in root.ChildNodes)
		{
			if (node.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			XmlElement element = (XmlElement)node;
			string name = element.GetAttribute("name");
			string text = element.InnerText;
			if (name != null && text != null)
			{
				if (element.GetAttrib("CEscape", false))
				{
					text = Regex.Unescape(text);
				}
				mStrings.Add(name, text);
			}
		}
	}

	protected static bool LoadImageNames(string filename)
	{
		XmlElement root = FUtil.LoadXmlAsset(filename);
		if (root != null)
		{
			LoadImageNamesXml(root);
		}
		return root != null;
	}

	protected static void LoadImageNamesXml(XmlElement root)
	{
		FontXml = null;
		mAdditionalAtlases.Clear();
		foreach (XmlNode node in root.ChildNodes)
		{
			if (node.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			XmlElement element = node as XmlElement;
			if (node.Name == "Image")
			{
				string name = element.GetAttribute("Name");
				string text = element.InnerText;
				if (name != null && text != null)
				{
					mImageNames[name] = text;
				}
			}
			else if (node.Name == "AdditionalAtlas")
			{
				string name = element.GetAttribute("Name");
				if (!string.IsNullOrEmpty(name))
				{
					mAdditionalAtlases.Add(name);
				}
			}
			else if (node.Name == "FontColor")
			{
				string name = element.GetAttribute("Name");
				string text = element.InnerText;
				if (name != null && text != null)
				{
					mFontColors[name] = text;
				}
			}
		}
	}

	public static string ParseSpecialTag(XmlNode node)
	{
		if (m_mSpecialXMLNodeFunc != null)
		{
			return m_mSpecialXMLNodeFunc(node);
		}
		return null;
	}

	public static string GetFormattedTime(double totalGameTime)
	{
		string result = "";
		int seconds = (int)totalGameTime;
		if (seconds >= 3600)
		{
			result += string.Format("{0:D2}:", seconds / 3600);
		}
		return result + string.Format("{0:D2}:{1:D2}", seconds / 60 % 60, seconds % 60);
	}
}
