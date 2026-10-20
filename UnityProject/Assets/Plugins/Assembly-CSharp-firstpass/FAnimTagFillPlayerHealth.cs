using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagFillPlayerHealth @ 0x160a918..0x160a95c, Ghidra vaddr).
public class FAnimTagFillPlayerHealth : FAnimTag
{
	public FAnimTagFillPlayerHealth()
		: base(AnimTagTypes.FillPlayerHealth)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		return base.LoadXml(root);
	}
}
