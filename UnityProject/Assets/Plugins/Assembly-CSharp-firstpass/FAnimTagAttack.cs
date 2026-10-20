using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagAttack @ 0x160acc0..0x160acf8, Ghidra vaddr).
public class FAnimTagAttack : FAnimTag
{
	public FAnimTagAttack()
		: base(AnimTagTypes.Attack)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		return base.LoadXml(root);
	}
}
