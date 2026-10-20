using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagSequence @ 0x160b8b4..0x160b970, Ghidra vaddr).
public class FAnimTagSequence : FAnimTag
{
	public string Name { get; protected set; }

	public FAnimTagSequence()
		: base(AnimTagTypes.Sequence)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			Name = root.GetAttrib("Name");
		}
		return flag;
	}
}
