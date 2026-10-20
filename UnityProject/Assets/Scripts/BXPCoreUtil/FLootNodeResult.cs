using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FLootNodeResult<,> @ 0x1faa254..0x1faa3bc, Ghidra vaddr).
public class FLootNodeResult<ContextType, ResultType> : FLootNode<ContextType, ResultType> where ResultType : class
{
	public ResultType Result;

	public FLootNodeResult(ILootTableManager<ContextType, ResultType> manager)
		: base(manager)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		base.LoadXml(root);
		Result = mManager.GetResultForXmlElement(root);
		return Result != null;
	}

	protected override void EvaluateOnce(ContextType context, List<ResultType> resultItems, FRandom random)
	{
		if (Result != null)
		{
			resultItems.Add(Result);
		}
	}
}
