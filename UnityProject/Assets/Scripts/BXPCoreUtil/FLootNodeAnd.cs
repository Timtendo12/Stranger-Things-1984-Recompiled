using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FLootNodeAnd<,> @ 0x1f7f6b8..0x1f7f790, Ghidra vaddr).
public class FLootNodeAnd<ContextType, ResultType> : FLootNodeList<ContextType, ResultType> where ResultType : class
{
	public FLootNodeAnd(ILootTableManager<ContextType, ResultType> manager)
		: base(manager)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		return base.LoadXml(root);
	}

	protected override void EvaluateOnce(ContextType context, List<ResultType> resultItems, FRandom random)
	{
		for (int i = 0; i < Entries.Count; i++)
		{
			Entries[i].Node.Evaluate(context, resultItems, random);
		}
	}
}
