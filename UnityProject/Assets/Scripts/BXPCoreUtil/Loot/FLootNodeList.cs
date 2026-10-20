using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FLootNodeList<,> @ 0x1f7ffa0..0x1f80000, Ghidra vaddr).
public abstract class FLootNodeList<ContextType, ResultType> : FLootNode<ContextType, ResultType> where ResultType : class
{
	public List<FLootNodeListEntry<ContextType, ResultType>> Entries = new List<FLootNodeListEntry<ContextType, ResultType>>();

	public FLootNodeList(ILootTableManager<ContextType, ResultType> manager)
		: base(manager)
	{
	}

	protected override bool ParseXmlElement(XmlElement element)
	{
		if (base.ParseXmlElement(element))
		{
			return true;
		}
		FLootNode<ContextType, ResultType> fLootNode = mManager.AllocateNodeForXmlElement(element);
		if (fLootNode == null)
		{
			return false;
		}
		if (!fLootNode.LoadXml(element))
		{
			return true;
		}
		FLootNodeListEntry<ContextType, ResultType> fLootNodeListEntry = AllocateListEntry();
		fLootNodeListEntry.Node = fLootNode;
		if (!fLootNodeListEntry.LoadXml(element))
		{
			return true;
		}
		Entries.Add(fLootNodeListEntry);
		return true;
	}

	protected virtual FLootNodeListEntry<ContextType, ResultType> AllocateListEntry()
	{
		return new FLootNodeListEntry<ContextType, ResultType>();
	}

	protected override void EvaluateOnce(ContextType context, List<ResultType> resultItems, FRandom random)
	{
	}
}
