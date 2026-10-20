using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FLootDesc @ 0x17eeca4..0x17eed88, Ghidra vaddr).
public class FLootDesc
{
	public string Name { get; set; }

	public int DBID { get; set; }

	public FLootDesc(XmlElement element)
	{
		Name = element.GetAttrib("Name", string.Empty);
		DBID = element.GetAttrib("DBID", -1);
	}
}
