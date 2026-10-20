using System.Collections.Generic;
using System.Linq;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FProtoCollectibleManager @ 0x017c3638..0x017c3fbc, Ghidra vaddr).
public class FProtoCollectibleManager
{
	protected string mFilename;

	protected Dictionary<string, FProtoCollectible> mProtoCollectibles = new Dictionary<string, FProtoCollectible>();

	public Dictionary<string, FProtoCollectible> ProtoCollectibles => mProtoCollectibles;

	public static FProtoCollectibleManager CreateFromFile(string filename)
	{
		FProtoCollectibleManager fProtoCollectibleManager = new FProtoCollectibleManager();
		fProtoCollectibleManager.LoadXml(filename);
		return fProtoCollectibleManager;
	}

	public bool LoadXml(string filename)
	{
		mFilename = filename;
		XmlElement xmlElement = FUtil.LoadXmlAsset(filename);
		if (xmlElement != null)
		{
			return LoadXml(xmlElement);
		}
		FLog.UnityError("Failed to parse ProtoCollectibles file.");
		return false;
	}

	public bool LoadXml(XmlElement root)
	{
		bool result = true;
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType == XmlNodeType.Element && ((XmlElement)childNode).Name == "ProtoCollectible")
			{
				FProtoCollectible fProtoCollectible = new FProtoCollectible();
				if (!fProtoCollectible.LoadXml((XmlElement)childNode))
				{
					result = false;
				}
				else
				{
					mProtoCollectibles.Add(fProtoCollectible.Name, fProtoCollectible);
				}
			}
		}
		return result;
	}

	public int GetNumberProtoCollectibleOfType(FProtoCollectible.Types t)
	{
		int num = 0;
		foreach (FProtoCollectible value in mProtoCollectibles.Values)
		{
			if (value.Type == t)
			{
				num++;
			}
		}
		return num;
	}

	public FProtoCollectible GetProtoCollectible(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		if (!mProtoCollectibles.ContainsKey(name))
		{
			return null;
		}
		return mProtoCollectibles[name];
	}

	public FProtoCollectible GetProtoCollectibleByDBID(int dbid)
	{
		if (dbid < 0)
		{
			return null;
		}
		return mProtoCollectibles.Values.FirstOrDefault((FProtoCollectible c) => c.DBID == dbid);
	}

	public string GetProtoCollectibleName(string protoName, int dbid)
	{
		return string.Format("{0}{1:D3}", protoName, dbid);
	}
}
