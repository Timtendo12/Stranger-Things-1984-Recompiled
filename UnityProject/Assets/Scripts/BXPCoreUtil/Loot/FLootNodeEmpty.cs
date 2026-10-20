using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FLootNodeEmpty<,> @ 0x1f7f964..0x1f7f9ec, Ghidra vaddr).
public class FLootNodeEmpty<ContextType, ResultType> : FLootNode<ContextType, ResultType> where ResultType : class
{
	public FLootNodeEmpty(ILootTableManager<ContextType, ResultType> manager)
		: base(manager)
	{
	}

	protected override void EvaluateOnce(ContextType context, List<ResultType> resultItems, FRandom random)
	{
		resultItems.Add(null);
	}
}
