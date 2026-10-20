using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FKeymapper @ 0x178a4e4..0x178b248, Ghidra vaddr).
public class FKeymapper
{
	protected Dictionary<string, KeyCode> mDefaultKeymapping = new Dictionary<string, KeyCode>();

	public Dictionary<string, KeyCode> DefaultKeymapping => mDefaultKeymapping;

	public int NumMappedKeys => mDefaultKeymapping.Count;

	public static FKeymapper CreateFromXmlFile(string filename)
	{
		FKeymapper fKeymapper = new FKeymapper();
		fKeymapper.LoadXml(filename);
		return fKeymapper;
	}

	public bool LoadXml(string filename)
	{
		XmlElement xmlElement = FUtil.LoadXmlAsset(filename);
		if (xmlElement == null)
		{
			FLog.UnityError("Failed to parse Keymapper file '" + filename + "'.");
		}
		else
		{
			LoadXml(xmlElement);
		}
		return xmlElement != null;
	}

	public bool LoadXml(XmlElement root)
	{
		mDefaultKeymapping.Clear();
		foreach (XmlNode childNode in root.ChildNodes)
		{
			XmlElement xmlElement = childNode as XmlElement;
			if (xmlElement == null || !(xmlElement.Name == "Action"))
			{
				continue;
			}
			string attrib = xmlElement.GetAttrib("Name", string.Empty);
			string attrib2 = xmlElement.GetAttrib("DefaultKey", string.Empty);
			if (!attrib.Valid())
			{
				FLog.UnityWarning("Keymapper has an action defined with no Name");
			}
			else if (!mDefaultKeymapping.ContainsKey(attrib))
			{
				if (!attrib2.Valid())
				{
					FLog.UnityWarning("Action '" + attrib + "' has an invalid DefaultKey");
					continue;
				}
				KeyCode result = KeyCode.None;
				if (!FEnum.TryParse(attrib2, ref result))
				{
					FLog.UnityWarning(string.Concat("Action '", attrib, "' has an invalid DefaultKey '", attrib2, "'"));
				}
				else
				{
					mDefaultKeymapping.Add(attrib, result);
				}
			}
			else
			{
				FLog.UnityWarning("Keymapper already has Action (" + attrib + ") defined");
			}
		}
		return true;
	}

	public KeyCode GetKey(string action)
	{
		if (FUserOptions.UserKeymapping.ContainsKey(action))
		{
			return FUserOptions.UserKeymapping[action];
		}
		if (mDefaultKeymapping.ContainsKey(action))
		{
			return mDefaultKeymapping[action];
		}
		return KeyCode.None;
	}

	public bool IsKeyMapped(KeyCode code)
	{
		if (FUserOptions.UserKeymapping.ContainsValue(code))
		{
			return true;
		}
		if (!mDefaultKeymapping.ContainsValue(code))
		{
			return false;
		}
		// a default binding only counts while the user has not rebound that action
		foreach (KeyValuePair<string, KeyCode> item in mDefaultKeymapping)
		{
			if (item.Value == code && !FUserOptions.IsActionDefinedByUser(item.Key))
			{
				return true;
			}
		}
		return false;
	}
}
