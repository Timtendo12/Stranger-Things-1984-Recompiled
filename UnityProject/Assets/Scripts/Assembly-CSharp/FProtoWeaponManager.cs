using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FProtoWeaponManager @ 0x01823afc..0x0182601c, Ghidra vaddr).
public class FProtoWeaponManager
{
	protected Dictionary<string, FProtoWeapon> mProtoWeapons = new Dictionary<string, FProtoWeapon>();

	protected string mFilename;

	public Dictionary<string, FProtoWeapon> ProtoWeapons => mProtoWeapons;

	public static FProtoWeaponManager CreateFromFile(string filename)
	{
		FProtoWeaponManager fProtoWeaponManager = new FProtoWeaponManager();
		fProtoWeaponManager.LoadXml(filename);
		return fProtoWeaponManager;
	}

	public bool LoadXml(string filename)
	{
		mFilename = filename;
		XmlElement xmlElement = FUtil.LoadXmlAsset(filename);
		if (xmlElement != null)
		{
			return LoadXml(xmlElement);
		}
		FLog.UnityError("Failed to parse Resources file.");
		return false;
	}

	public bool LoadXml(XmlElement root)
	{
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType == XmlNodeType.Element && ((XmlElement)childNode).Name == "ProtoWeapon")
			{
				FProtoWeapon fProtoWeapon = new FProtoWeapon();
				if (!fProtoWeapon.LoadXml((XmlElement)childNode))
				{
					return false;
				}
				mProtoWeapons.Add(fProtoWeapon.Name, fProtoWeapon);
			}
		}
		return true;
	}

	public FProtoWeapon GetProtoWeapon(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		if (!mProtoWeapons.ContainsKey(name))
		{
			return null;
		}
		return mProtoWeapons[name];
	}
}
