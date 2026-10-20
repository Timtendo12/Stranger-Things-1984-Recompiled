using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FLootNodeListEntryOr<,> @ 0x1f7fe54..0x1f7fed8, Ghidra vaddr).
public class FLootNodeListEntryOr<ContextType, ResultType> : FLootNodeListEntry<ContextType, ResultType> where ResultType : class
{
	public int Weight { get; protected set; }

	public override bool LoadXml(XmlElement root)
	{
		Weight = root.GetAttrib("Weight", 1);
		return true;
	}
}
