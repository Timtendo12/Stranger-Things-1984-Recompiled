using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FColorNames @ 0x1647784..0x1648404, Ghidra vaddr).
public class FColorNames
{
	protected static Dictionary<string, Color> mColors = new Dictionary<string, Color>();

	public static void AddColorsXml(string filename)
	{
		XmlElement root = FUtil.LoadXmlAsset(filename);
		if (root == null)
		{
			FLog.UnityError("Could not load colors: " + filename);
			return;
		}
		foreach (XmlElement element in root.Elements())
		{
			string name = element.GetAttrib("Name");
			string hex = element.GetAttrib("Hex");
			if (!string.IsNullOrEmpty(hex))
			{
				mColors[name] = FUtil.ColorFromHex(hex);
			}
			else
			{
				FLog.UnityError("Did not specify a color for: " + name);
			}
		}
	}

	public static void Set(string name, Color color)
	{
		mColors[name] = color;
	}

	public static void Set(string name, string hex)
	{
		mColors[name] = FUtil.ColorFromHex(hex);
	}

	public static Color Get(string name)
	{
		if (!mColors.TryGetValue(name, out var color))
		{
			FLog.UnityWarning("Did not find color for name: " + name);
			color = Color.white;
		}
		return color;
	}

	public static string CreateCompareString(int currentValue, int defaultValue)
	{
		return currentValue.ToString() + CreateCompareStringDelta(currentValue, defaultValue);
	}

	public static string CreateCompareStringDelta(int currentValue, int defaultValue)
	{
		int delta = currentValue - defaultValue;
		if (delta == 0)
		{
			return string.Empty;
		}
		string text = delta.ToString();
		if (currentValue < defaultValue)
		{
			text = " (" + text + ")";
			return ColorizeWithColorName(text, "UIWorseStat");
		}
		text = " (+" + text + ")";
		return ColorizeWithColorName(text, "UIBetterStat");
	}

	public static string ColorizeWithHex(string r, string colorString)
	{
		return "<color hex='" + colorString + "'>" + r + "</color>";
	}

	public static string ColorizeWithColorName(string r, string colorString)
	{
		return "<color name='" + colorString + "'>" + r + "</color>";
	}
}
