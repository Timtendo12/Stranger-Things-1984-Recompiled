using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FLootTableManager @ 0x17eed88..0x17ef7cc, Ghidra vaddr).
public class FLootTableManager : ILootTableManager<FPlayerStats, FLootDesc>, IRequirementManager<FPlayerStats>
{
	protected Dictionary<string, FLootTable> mLootTables = new Dictionary<string, FLootTable>();

	public Dictionary<string, FLootTable> LootTables => mLootTables;

	public bool AddLootTables(string filename)
	{
		XmlElement xmlElement = FUtil.LoadXmlAsset(filename);
		if (xmlElement != null)
		{
			LoadXml(xmlElement);
		}
		return xmlElement != null;
	}

	public bool LoadXml(XmlElement root)
	{
		mLootTables.Clear();
		bool result = true;
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			FLootTable fLootTable = new FLootTable();
			if (!fLootTable.LoadXml(childNode as XmlElement))
			{
				result = false;
			}
			else if (!mLootTables.ContainsKey(fLootTable.Name))
			{
				mLootTables.Add(fLootTable.Name, fLootTable);
			}
			else
			{
				FLog.UnityError("Duplicate loot table name: " + fLootTable.Name);
			}
		}
		return result;
	}

	public FLootTable GetLootTable(string name)
	{
		if (!mLootTables.TryGetValue(name, out var value))
		{
			value = null;
		}
		return value;
	}

	public void Clear()
	{
		mLootTables.Clear();
	}

	public FLootNode<FPlayerStats, FLootDesc> GetRootNode(string name)
	{
		return GetLootTable(name)?.Root;
	}

	public FLootNode<FPlayerStats, FLootDesc> AllocateNodeForXmlElement(XmlElement xmlNode)
	{
		if (xmlNode.Name == "LootDesc" || xmlNode.Name == "Result")
		{
			return new FLootNodeResult<FPlayerStats, FLootDesc>(this);
		}
		if (xmlNode.Name == "Empty")
		{
			return new FLootNodeEmpty<FPlayerStats, FLootDesc>(this);
		}
		if (xmlNode.Name == "And")
		{
			return new FLootNodeAnd<FPlayerStats, FLootDesc>(this);
		}
		if (xmlNode.Name == "Or")
		{
			return new FLootNodeOr<FPlayerStats, FLootDesc>(this);
		}
		if (xmlNode.Name == "Include")
		{
			return new FLootNodeInclude<FPlayerStats, FLootDesc>(this);
		}
		return null;
	}

	public FLootDesc GetResultForXmlElement(XmlElement xmlNode)
	{
		return new FLootDesc(xmlNode);
	}

	public FRequirement<FPlayerStats> AllocateRequirementForXmlElement(XmlElement element)
	{
		return FApp.PlayerStats.AllocateRequirementForXmlElement(element);
	}
}
