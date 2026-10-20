using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FLootNodeListEntry<,> @ 0x1f7ff80..0x1f7ff90, Ghidra vaddr).
public class FLootNodeListEntry<ContextType, ResultType> where ResultType : class
{
	public FLootNode<ContextType, ResultType> Node;

	public virtual bool LoadXml(XmlElement root)
	{
		return true;
	}
}
