using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FLootTable @ 0x17eeb14..0x17eeca4, Ghidra vaddr).
public class FLootTable
{
	public string Name;

	public FLootNode<FPlayerStats, FLootDesc> Root = new FLootNodeAnd<FPlayerStats, FLootDesc>(FApp.LootTableManager);

	public void Evaluate(FPlayerStats stats, List<FLootDesc> resultItems, FRandom random)
	{
		Root.Evaluate(stats, resultItems, random);
	}

	public bool LoadXml(XmlElement xmlRoot)
	{
		Name = xmlRoot.GetAttribute("Name");
		return Root.LoadXml(xmlRoot);
	}
}
