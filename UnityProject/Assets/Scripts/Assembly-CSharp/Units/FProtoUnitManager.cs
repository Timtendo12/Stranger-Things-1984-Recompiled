using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FProtoUnitManager @ 0x0181e058..0x01824fec, Ghidra vaddr).
public class FProtoUnitManager
{
	protected int mNextID;

	protected Dictionary<string, FProtoUnit> mProtoUnits = new Dictionary<string, FProtoUnit>();

	protected string mFilename;

	public static FProtoUnitManager CreateFromFile(string filename)
	{
		FProtoUnitManager fProtoUnitManager = new FProtoUnitManager();
		fProtoUnitManager.LoadXml(filename);
		return fProtoUnitManager;
	}

	public bool LoadXml(string filename)
	{
		mFilename = filename;
		XmlElement xmlElement = FUtil.LoadXmlAsset(filename);
		if (xmlElement == null)
		{
			FLog.UnityError("Failed to parse ProtoUnits file '" + mFilename + "'.");
		}
		else
		{
			LoadXml(xmlElement);
		}
		return xmlElement != null;
	}

	public bool LoadXml(XmlElement root)
	{
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType == XmlNodeType.Element)
			{
				XmlElement xmlElement = (XmlElement)childNode;
				if (xmlElement.Name == "ProtoUnit")
				{
					FProtoUnit fProtoUnit = new FProtoUnit();
					bool loaded = fProtoUnit.LoadXml(xmlElement, mNextID);
					mNextID++;
					if (loaded)
					{
						mProtoUnits.Add(fProtoUnit.Name, fProtoUnit);
					}
				}
			}
		}
		// The shipped build iterates mProtoUnits.Values here with an empty (compiled-out) body.
		return true;
	}

	public void UpdateProtoUnit(string name, FProtoUnit unit)
	{
		if (mProtoUnits.ContainsKey(name))
		{
			mProtoUnits[name] = unit;
		}
	}

	public FProtoUnit GetProtoUnit(string name, bool caseSensitive = true)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		StringComparison comparisonType = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
		foreach (string key in mProtoUnits.Keys)
		{
			if (string.Equals(name, key, comparisonType))
			{
				return mProtoUnits[key];
			}
		}
		return null;
	}

	public FProtoUnit[] GetProtoUnitsWithTag(string tag)
	{
		return mProtoUnits.Values.Where((FProtoUnit p) => p.IsTag(tag)).ToArray();
	}
}
