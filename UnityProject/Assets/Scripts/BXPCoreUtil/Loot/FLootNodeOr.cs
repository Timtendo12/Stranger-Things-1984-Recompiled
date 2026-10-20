using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FLootNodeOr<,> @ 0x1f80444..0x1f80768, Ghidra vaddr).
public class FLootNodeOr<ContextType, ResultType> : FLootNodeList<ContextType, ResultType> where ResultType : class
{
	public FLootNodeOr(ILootTableManager<ContextType, ResultType> manager)
		: base(manager)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		return base.LoadXml(root);
	}

	protected override void EvaluateOnce(ContextType context, List<ResultType> resultItems, FRandom random)
	{
		int num = ComputeTotalWeight(context);
		if (num < 1)
		{
			return;
		}
		int num2 = random.RangeInt(0, num - 1, "FSimLootNodeOr string main roll");
		int num3 = 0;
		for (int i = 0; i < Entries.Count; i++)
		{
			FLootNodeListEntryOr<ContextType, ResultType> fLootNodeListEntryOr = (FLootNodeListEntryOr<ContextType, ResultType>)Entries[i];
			if (fLootNodeListEntryOr.Node.CheckRequirements(context))
			{
				num3 += fLootNodeListEntryOr.Weight;
				if (num2 < num3)
				{
					fLootNodeListEntryOr.Node.Evaluate(context, resultItems, random);
					break;
				}
			}
		}
	}

	protected override FLootNodeListEntry<ContextType, ResultType> AllocateListEntry()
	{
		return new FLootNodeListEntryOr<ContextType, ResultType>();
	}

	protected int ComputeTotalWeight(ContextType context)
	{
		int num = 0;
		for (int i = 0; i < Entries.Count; i++)
		{
			FLootNodeListEntryOr<ContextType, ResultType> fLootNodeListEntryOr = (FLootNodeListEntryOr<ContextType, ResultType>)Entries[i];
			if (fLootNodeListEntryOr.Node.CheckRequirements(context))
			{
				num += fLootNodeListEntryOr.Weight;
			}
		}
		return num;
	}
}
