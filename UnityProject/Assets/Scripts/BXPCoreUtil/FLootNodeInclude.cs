using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FLootNodeInclude<,> @ 0x1f7fa78..0x1f7fc0c, Ghidra vaddr).
public class FLootNodeInclude<ContextType, ResultType> : FLootNode<ContextType, ResultType> where ResultType : class
{
	public string IncludeName;

	public FLootNodeInclude(ILootTableManager<ContextType, ResultType> manager)
		: base(manager)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		base.LoadXml(root);
		IncludeName = root.GetAttribute("LootTable");
		return IncludeName != null;
	}

	protected override void EvaluateOnce(ContextType context, List<ResultType> resultItems, FRandom random)
	{
		FLootNode<ContextType, ResultType> rootNode = mManager.GetRootNode(IncludeName);
		if (rootNode != null)
		{
			rootNode.Evaluate(context, resultItems, random);
		}
	}
}
