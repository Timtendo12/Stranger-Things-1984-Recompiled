using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagDropLoot @ 0x160a5e0..0x160a618, Ghidra vaddr).
public class FAnimTagDropLoot : FAnimTag
{
	public FAnimTagDropLoot()
		: base(AnimTagTypes.DropLoot)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		return true;
	}
}
