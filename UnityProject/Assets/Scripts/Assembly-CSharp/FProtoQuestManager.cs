using System.Collections.Generic;
using System.Linq;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FProtoQuestManager @ 0x17c4dec..0x17c5774, Ghidra vaddr).
public class FProtoQuestManager
{
	protected string mFilename;

	protected Dictionary<string, FProtoQuest> mProtoQuests = new Dictionary<string, FProtoQuest>();

	public Dictionary<string, FProtoQuest> ProtoQuests => mProtoQuests;

	public static FProtoQuestManager CreateFromFile(string filename)
	{
		FProtoQuestManager fProtoQuestManager = new FProtoQuestManager();
		fProtoQuestManager.LoadXml(filename);
		return fProtoQuestManager;
	}

	public bool LoadXml(string filename)
	{
		mFilename = filename;
		XmlElement xmlElement = FUtil.LoadXmlAsset(filename);
		if (xmlElement == null)
		{
			FLog.UnityError("Failed to parse ProtoQuests file.");
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
			if (childNode.NodeType == XmlNodeType.Element && ((XmlElement)childNode).Name == "ProtoQuest")
			{
				FProtoQuest fProtoQuest = new FProtoQuest();
				fProtoQuest.LoadXml((XmlElement)childNode);
				mProtoQuests.Add(fProtoQuest.Name, fProtoQuest);
			}
		}
		return true;
	}

	public FProtoQuest GetProtoQuest(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		if (!mProtoQuests.ContainsKey(name))
		{
			return null;
		}
		return mProtoQuests[name];
	}

	public FProtoQuest GetProtoQuestByDBID(int dbid)
	{
		if (dbid < 0)
		{
			return null;
		}
		return mProtoQuests.Values.FirstOrDefault((FProtoQuest q) => q.DBID == dbid);
	}

	public FProtoQuest GetProtoQuestRequiresItemDBID(int dbid)
	{
		if (dbid < 0)
		{
			return null;
		}
		return mProtoQuests.Values.FirstOrDefault((FProtoQuest q) => q.Requirements != null && q.Requirements.Any((FPlayerStatsRequirement r) => r is FRequirementCollectible fRequirementCollectible && fRequirementCollectible.DBID == dbid));
	}
}
