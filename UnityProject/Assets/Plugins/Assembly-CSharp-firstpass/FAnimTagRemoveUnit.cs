using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagRemoveUnit @ 0x160a5ac..0x160a5e0, Ghidra vaddr).
public class FAnimTagRemoveUnit : FAnimTag
{
	public FAnimTagRemoveUnit()
		: base(AnimTagTypes.RemoveUnit)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		return base.LoadXml(root);
	}
}
