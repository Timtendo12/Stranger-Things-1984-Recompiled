using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FXmlElementExtensions @ 0x18909a8..0x1895810, Ghidra vaddr).
public static class FXmlElementExtensions
{
	public static bool GetAttrib(this XmlElement element, string attribName, ref float result)
	{
		bool ok = element.GetAttribute(attribName).TryParseFloat(out float value);
		if (ok)
		{
			result = value;
		}
		return ok;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref float result, float defaultResult)
	{
		return element.GetAttribute(attribName).TryParseFloat(out result, defaultResult);
	}

	public static float GetAttrib(this XmlElement element, string attribName, float defaultResult)
	{
		element.GetAttribute(attribName).TryParseFloat(out float result, defaultResult);
		return result;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref DateTime result)
	{
		bool ok = element.GetAttribute(attribName).TryParseDateTime(out DateTime value);
		if (ok)
		{
			result = value;
		}
		return ok;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref DateTime result, DateTime defaultResult)
	{
		return element.GetAttribute(attribName).TryParseDateTime(out result, defaultResult);
	}

	public static DateTime GetAttrib(this XmlElement element, string attribName, DateTime defaultResult)
	{
		element.GetAttribute(attribName).TryParseDateTime(out DateTime result, defaultResult);
		return result;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref int result)
	{
		bool ok = element.GetAttribute(attribName).TryParseInt(out int value);
		if (ok)
		{
			result = value;
		}
		return ok;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref int result, int defaultResult)
	{
		return element.GetAttribute(attribName).TryParseInt(out result, defaultResult);
	}

	public static int GetAttrib(this XmlElement element, string attribName, int defaultResult)
	{
		element.GetAttribute(attribName).TryParseInt(out int result, defaultResult);
		return result;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref long result)
	{
		bool ok = element.GetAttribute(attribName).TryParseLong(out long value);
		if (ok)
		{
			result = value;
		}
		return ok;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref long result, long defaultResult)
	{
		return element.GetAttribute(attribName).TryParseLong(out result, defaultResult);
	}

	public static long GetAttrib(this XmlElement element, string attribName, long defaultResult)
	{
		element.GetAttribute(attribName).TryParseLong(out long result, defaultResult);
		return result;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref bool result)
	{
		bool ok = element.GetAttribute(attribName).TryParseBool(out bool value);
		if (ok)
		{
			result = value;
		}
		return ok;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref bool result, bool defaultResult)
	{
		return element.GetAttribute(attribName).TryParseBool(out result, defaultResult);
	}

	public static bool GetAttrib(this XmlElement element, string attribName, bool defaultResult)
	{
		element.GetAttribute(attribName).TryParseBool(out bool result, defaultResult);
		return result;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref string result)
	{
		string value = element.GetAttrib(attribName);
		if (value != null)
		{
			result = value;
		}
		return value != null;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref string result, string defaultResult)
	{
		result = element.GetAttrib(attribName);
		bool found = result != null;
		if (!found)
		{
			result = defaultResult;
		}
		return found;
	}

	public static string GetAttrib(this XmlElement element, string attribName, string defaultResult)
	{
		return element.GetAttrib(attribName) ?? defaultResult;
	}

	public static string GetAttrib(this XmlElement element, string attribName)
	{
		if (element.HasAttribute(attribName))
		{
			return element.GetAttribute(attribName);
		}
		return null;
	}

	public static bool GetAttribEnum<T>(this XmlElement element, string attribName, ref T result) where T : struct, IConvertible
	{
		return FEnum.TryParse(element.GetAttrib(attribName, (string)null), ref result);
	}

	public static bool GetAttribEnum<T>(this XmlElement element, string attribName, ref T result, T defaultResult) where T : struct, IConvertible
	{
		if (element.GetAttribEnum(attribName, ref result))
		{
			return true;
		}
		result = defaultResult;
		return false;
	}

	public static T GetAttribEnum<T>(this XmlElement element, string attribName, T defaultResult) where T : struct, IConvertible
	{
		T result = defaultResult;
		element.GetAttribEnum(attribName, ref result, defaultResult);
		return result;
	}

	public static XmlElement GetChild(this XmlElement element, string name)
	{
		foreach (XmlNode node in element.ChildNodes)
		{
			if (node.NodeType == XmlNodeType.Element && node.Name == name)
			{
				return node as XmlElement;
			}
		}
		return null;
	}

	public static string GetChildValue(this XmlElement element, string name, string defaultResult)
	{
		XmlElement child = element.GetChild(name);
		return child != null ? child.InnerText : defaultResult;
	}

	public static int GetChildValue(this XmlElement element, string name, int defaultResult)
	{
		XmlElement child = element.GetChild(name);
		if (child != null)
		{
			child.InnerText.TryParseInt(out defaultResult, defaultResult);
		}
		return defaultResult;
	}

	public static ulong GetChildValue(this XmlElement element, string name, ulong defaultResult)
	{
		XmlElement child = element.GetChild(name);
		if (child != null)
		{
			child.InnerText.TryParseUlong(out defaultResult, defaultResult);
		}
		return defaultResult;
	}

	public static float GetChildValue(this XmlElement element, string name, float defaultResult)
	{
		XmlElement child = element.GetChild(name);
		if (child != null)
		{
			child.InnerText.TryParseFloat(out defaultResult, defaultResult);
		}
		return defaultResult;
	}

	public static bool GetChildValue(this XmlElement element, string name, bool defaultResult)
	{
		XmlElement child = element.GetChild(name);
		if (child != null)
		{
			child.InnerText.TryParseBool(out defaultResult, defaultResult);
		}
		return defaultResult;
	}

	public static DateTime GetChildValue(this XmlElement element, string name, DateTime defaultResult, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
	{
		XmlElement child = element.GetChild(name);
		if (child == null)
		{
			return defaultResult;
		}
		child.InnerText.TryParseLong(out long ticks, defaultResult.Ticks);
		return new DateTime(ticks, dateTimeKind);
	}

	private static IEnumerable<XmlElement> ChildEntries(XmlElement container)
	{
		foreach (XmlNode node in container.ChildNodes)
		{
			if (node.NodeType == XmlNodeType.Element)
			{
				yield return node as XmlElement;
			}
		}
	}

	public static Dictionary<string, bool> GetChildDictBool(this XmlElement element, string childName, string keyName = "Key", string valueName = "Value", bool returnNull = false)
	{
		XmlElement child = element.GetChild(childName);
		if (child == null)
		{
			return returnNull ? null : new Dictionary<string, bool>();
		}
		Dictionary<string, bool> dict = new Dictionary<string, bool>();
		foreach (XmlElement entry in ChildEntries(child))
		{
			string key = entry.GetAttrib(keyName);
			bool value = false;
			if (key != null && entry.GetAttrib(valueName, ref value))
			{
				dict[key] = value;
			}
		}
		return dict;
	}

	public static Dictionary<string, int> GetChildDictInt(this XmlElement element, string childName, string keyName = "Key", string valueName = "Value", bool returnNull = false)
	{
		XmlElement child = element.GetChild(childName);
		if (child == null)
		{
			return returnNull ? null : new Dictionary<string, int>();
		}
		Dictionary<string, int> dict = new Dictionary<string, int>();
		foreach (XmlElement entry in ChildEntries(child))
		{
			string key = entry.GetAttrib(keyName);
			int value = 0;
			if (key != null && entry.GetAttrib(valueName, ref value))
			{
				dict[key] = value;
			}
		}
		return dict;
	}

	public static Dictionary<int, string> GetChildDictIntString(this XmlElement element, string childName, int defaultKey, string keyName = "Key", string valueName = "Value", bool returnNull = false)
	{
		XmlElement child = element.GetChild(childName);
		if (child == null)
		{
			return returnNull ? null : new Dictionary<int, string>();
		}
		Dictionary<int, string> dict = new Dictionary<int, string>();
		foreach (XmlElement entry in ChildEntries(child))
		{
			int key = entry.GetAttrib(keyName, defaultKey);
			string value = entry.GetAttrib(valueName);
			if (value != null)
			{
				dict[key] = value;
			}
		}
		return dict;
	}

	public static Dictionary<string, DateTime> GetChildDictDateTimeTicks(this XmlElement element, string childName, string keyName = "Key", string valueName = "Value", bool returnNull = false)
	{
		XmlElement child = element.GetChild(childName);
		if (child == null)
		{
			return returnNull ? null : new Dictionary<string, DateTime>();
		}
		Dictionary<string, DateTime> dict = new Dictionary<string, DateTime>();
		foreach (XmlElement entry in ChildEntries(child))
		{
			string key = entry.GetAttrib(keyName);
			long ticks = 0L;
			if (key != null && entry.GetAttrib(valueName, ref ticks))
			{
				dict[key] = new DateTime(ticks);
			}
		}
		return dict;
	}

	public static Dictionary<string, string> GetChildDictString(this XmlElement element, string childName, string keyName = "Key", string valueName = "Value", bool returnNull = false)
	{
		XmlElement child = element.GetChild(childName);
		if (child == null)
		{
			return returnNull ? null : new Dictionary<string, string>();
		}
		Dictionary<string, string> dict = new Dictionary<string, string>();
		foreach (XmlElement entry in ChildEntries(child))
		{
			string key = entry.GetAttrib(keyName);
			string value = entry.GetAttrib(valueName);
			if (key != null && value != null)
			{
				dict[key] = value;
			}
		}
		return dict;
	}

	public static List<string> GetChildListString(this XmlElement element, string childName, bool returnNull = false)
	{
		XmlElement child = element.GetChild(childName);
		if (child == null)
		{
			return returnNull ? null : new List<string>();
		}
		List<string> list = new List<string>();
		foreach (XmlElement entry in child.Elements())
		{
			string text = entry.InnerText;
			if (text != null)
			{
				list.Add(text);
			}
		}
		return list;
	}

	public static IEnumerable<XmlElement> Elements(this XmlNode root, string elementName = null)
	{
		foreach (XmlNode node in root.ChildNodes)
		{
			if (node.NodeType == XmlNodeType.Element && (elementName == null || node.Name == elementName))
			{
				yield return (XmlElement)node;
			}
		}
	}

	public static XmlElement AddChild(this XmlElement element, string childName, string childValue, params object[] attributePairs)
	{
		XmlElement child = element.OwnerDocument.CreateElement(childName, element.NamespaceURI);
		if (childValue != null)
		{
			child.InnerText = childValue;
		}
		element.AppendChild(child);
		child.AddAttributes(attributePairs);
		return child;
	}

	public static XmlElement AddChild(this XmlElement element, string childName, ulong childValue, params object[] attributePairs)
	{
		return element.AddChild(childName, childValue.ToString("G"), attributePairs);
	}

	public static XmlElement AddChild(this XmlElement element, string childName, float childValue, params object[] attributePairs)
	{
		return element.AddChild(childName, childValue.ToString("R"), attributePairs);
	}

	public static XmlElement AddChild(this XmlElement element, string childName, int childValue, params object[] attributePairs)
	{
		return element.AddChild(childName, childValue.ToString(), attributePairs);
	}

	public static XmlElement AddChild(this XmlElement element, string childName, bool childValue, params object[] attributePairs)
	{
		return element.AddChild(childName, childValue.ToString(), attributePairs);
	}

	public static XmlElement AddChild(this XmlElement element, string childName, DateTime childValue, params object[] attributePairs)
	{
		return element.AddChild(childName, childValue.Ticks.ToString(), attributePairs);
	}

	public static XmlElement AddChildDict(this XmlElement element, string childName, Dictionary<string, float> dict, string keyName = "Key", string valueName = "Value", string entryName = "Entry")
	{
		XmlElement container = element.AddChild(childName, null);
		if (dict != null)
		{
			foreach (KeyValuePair<string, float> kv in dict)
			{
				container.AddChild(entryName, null, keyName, kv.Key, valueName, kv.Value);
			}
		}
		return container;
	}

	public static XmlElement AddChildDict(this XmlElement element, string childName, Dictionary<string, int> dict, string keyName = "Key", string valueName = "Value", string entryName = "Entry")
	{
		XmlElement container = element.AddChild(childName, null);
		if (dict != null)
		{
			foreach (KeyValuePair<string, int> kv in dict)
			{
				container.AddChild(entryName, null, keyName, kv.Key, valueName, kv.Value);
			}
		}
		return container;
	}

	public static XmlElement AddChildDict(this XmlElement element, string childName, Dictionary<int, string> dict, string keyName = "Key", string valueName = "Value", string entryName = "Entry")
	{
		XmlElement container = element.AddChild(childName, null);
		if (dict != null)
		{
			foreach (KeyValuePair<int, string> kv in dict)
			{
				container.AddChild(entryName, null, keyName, kv.Key, valueName, kv.Value);
			}
		}
		return container;
	}

	public static XmlElement AddChildDict(this XmlElement element, string childName, Dictionary<string, string> dict, string keyName = "Key", string valueName = "Value", string entryName = "Entry")
	{
		XmlElement container = element.AddChild(childName, null);
		if (dict != null)
		{
			foreach (KeyValuePair<string, string> kv in dict)
			{
				container.AddChild(entryName, null, keyName, kv.Key, valueName, kv.Value);
			}
		}
		return container;
	}

	public static XmlElement AddChildDict(this XmlElement element, string childName, Dictionary<string, bool> dict, string keyName = "Key", string valueName = "Value", string entryName = "Entry")
	{
		XmlElement container = element.AddChild(childName, null);
		if (dict != null)
		{
			foreach (KeyValuePair<string, bool> kv in dict)
			{
				container.AddChild(entryName, null, keyName, kv.Key, valueName, kv.Value);
			}
		}
		return container;
	}

	public static XmlElement AddChildDict(this XmlElement element, string childName, Dictionary<string, DateTime> dict, string keyName = "Key", string valueName = "Value", string entryName = "Entry")
	{
		XmlElement container = element.AddChild(childName, null);
		if (dict != null)
		{
			foreach (KeyValuePair<string, DateTime> kv in dict)
			{
				container.AddChild(entryName, null, keyName, kv.Key, valueName, kv.Value);
			}
		}
		return container;
	}

	public static XmlElement AddChildDictDateTimeTicks(this XmlElement element, string childName, Dictionary<string, DateTime> dict, string keyName = "Key", string valueName = "Value", string entryName = "Entry")
	{
		XmlElement container = element.AddChild(childName, null);
		if (dict != null)
		{
			foreach (KeyValuePair<string, DateTime> kv in dict)
			{
				container.AddChild(entryName, null, keyName, kv.Key, valueName, kv.Value.Ticks);
			}
		}
		return container;
	}

	public static XmlElement AddChildList(this XmlElement element, string childName, List<string> list, string entryName = "Entry")
	{
		XmlElement container = element.AddChild(childName, null);
		if (list != null)
		{
			foreach (string value in list)
			{
				container.AddChild(entryName, value);
			}
		}
		return container;
	}

	public static void AddAttributes(this XmlElement element, params object[] attributePairs)
	{
		if (attributePairs == null)
		{
			return;
		}
		if ((attributePairs.Length & 1) != 0)
		{
			throw new ArgumentException("Attributes params must be in name, value pairs", "attributePairs");
		}
		for (int i = 0; i < attributePairs.Length; i += 2)
		{
			XmlAttribute attribute = element.OwnerDocument.CreateAttribute(attributePairs[i] as string);
			attribute.Value = attributePairs[i + 1].ToString();
			element.Attributes.Append(attribute);
		}
	}

	public static XmlAttribute AddAttribute(this XmlElement element, string attribName, string attribValue)
	{
		XmlAttribute attribute = element.OwnerDocument.CreateAttribute(attribName);
		attribute.Value = attribValue;
		element.Attributes.Append(attribute);
		return attribute;
	}

	public static XmlAttribute AddAttribute(this XmlElement element, string attribName, int attribValue)
	{
		return element.AddAttribute(attribName, attribValue.ToString());
	}

	public static XmlAttribute AddAttribute(this XmlElement element, string attribName, float attribValue)
	{
		return element.AddAttribute(attribName, attribValue.ToString("R"));
	}

	public static XmlAttribute AddAttribute(this XmlElement element, string attribName, DateTime attribValue)
	{
		return element.AddAttribute(attribName, attribValue.Ticks.ToString());
	}

	public static XmlAttribute AddAttribute(this XmlElement element, string attribName, bool attribValue)
	{
		return element.AddAttribute(attribName, attribValue.ToString());
	}

	public static string ConvertDocToString(this XmlElement element, bool indent = true)
	{
		using (StringWriter stringWriter = new StringWriter())
		using (XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter))
		{
			if (indent)
			{
				xmlWriter.Formatting = Formatting.Indented;
			}
			element.OwnerDocument.Save(xmlWriter);
			return stringWriter.ToString();
		}
	}

	public static void RemoveFromParent(this XmlElement element)
	{
		if (element.ParentNode != null)
		{
			element.ParentNode.RemoveChild(element);
		}
	}

	public static void DoDepthFirst(this XmlNode root, Action<XmlNode> enterFunc, Action<XmlNode> exitFunc)
	{
		enterFunc?.Invoke(root);
		foreach (XmlNode child in root)
		{
			child.DoDepthFirst(enterFunc, exitFunc);
		}
		exitFunc?.Invoke(root);
	}
}
