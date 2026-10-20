using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUserOptions @ 0x17df6e8..0x17e0c10, Ghidra vaddr).
public class FUserOptions
{
	protected const string sProfilesFilename = "PlayerOptions.xml";

	protected static FSaveFile sSaveFile;

	protected static Dictionary<string, KeyCode> mUserKeymapping = new Dictionary<string, KeyCode>();

	public static Dictionary<string, KeyCode> UserKeymapping => mUserKeymapping;

	private FUserOptions()
	{
	}

	public static void Init()
	{
		sSaveFile = new FSaveFile("PlayerOptions.xml", true, true, true, true, false);
		sSaveFile.Load();
		string dataAsUTF8String = sSaveFile.DataAsUTF8String;
		if (dataAsUTF8String != null)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(dataAsUTF8String);
			XmlElement documentElement = xmlDocument.DocumentElement;
			if (documentElement == null)
			{
				FLog.UnityError("Failed to load/parse player options");
			}
			else
			{
				Load(documentElement);
			}
		}
	}

	public static void ChangeUserKeymapping(Dictionary<string, KeyCode> mapping)
	{
		mUserKeymapping = mapping;
		Save();
	}

	public static void ClearUserKeymapping()
	{
		mUserKeymapping.Clear();
		Save();
	}

	public static bool IsActionDefinedByUser(string action)
	{
		if (action.Valid() && mUserKeymapping != null)
		{
			if (!mUserKeymapping.ContainsKey(action))
			{
				return false;
			}
			return mUserKeymapping[action] != KeyCode.None;
		}
		return false;
	}

	protected static void Save()
	{
		XmlElement xmlElement = FCoreUtil.CreateXmlDoc("Options");
		xmlElement.AppendChild(SaveKeymap(xmlElement.OwnerDocument));
		sSaveFile.Save(xmlElement);
	}

	protected static void Load(XmlElement root)
	{
		foreach (XmlNode childNode in root.ChildNodes)
		{
			XmlElement xmlElement = childNode as XmlElement;
			if (xmlElement != null && xmlElement.Name == "Keymap")
			{
				LoadKeymap(xmlElement);
			}
		}
	}

	protected static void LoadKeymap(XmlElement root)
	{
		mUserKeymapping.Clear();
		foreach (XmlNode childNode in root.ChildNodes)
		{
			XmlElement xmlElement = childNode as XmlElement;
			if (xmlElement == null || !(xmlElement.Name == "Action"))
			{
				continue;
			}
			string attrib = xmlElement.GetAttrib("Name", string.Empty);
			string attrib2 = xmlElement.GetAttrib("Key", string.Empty);
			if (!attrib.Valid())
			{
				FLog.UnityWarning("User Keymapping has an action defined with no Name");
			}
			else if (!mUserKeymapping.ContainsKey(attrib))
			{
				if (!attrib2.Valid())
				{
					FLog.UnityWarning("User Keymapping Action (" + attrib + ") has an invalid Key");
					continue;
				}
				KeyCode result = KeyCode.None;
				if (!FEnum.TryParse(attrib2, ref result))
				{
					FLog.UnityWarning(string.Concat("User Keymapping Action (", attrib, ") has an invalid Key (", attrib2, ")"));
				}
				else
				{
					mUserKeymapping.Add(attrib, result);
				}
			}
			else
			{
				FLog.UnityWarning("User Keymapping already has Action (" + attrib + ") defined");
			}
		}
	}

	public static XmlElement SaveKeymap(XmlDocument doc)
	{
		XmlElement xmlElement = doc.CreateElement("Keymap");
		foreach (KeyValuePair<string, KeyCode> item in mUserKeymapping)
		{
			xmlElement.AddChild("Action", string.Empty, "Name", item.Key, "Key", item.Value);
		}
		return xmlElement;
	}
}
