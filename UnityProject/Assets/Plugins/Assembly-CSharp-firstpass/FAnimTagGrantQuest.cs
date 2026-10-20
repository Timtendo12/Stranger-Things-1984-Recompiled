using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagGrantQuest @ 0x160a618..0x160a6c4, Ghidra vaddr).
public class FAnimTagGrantQuest : FAnimTag
{
	public int DBID { get; protected set; }

	public FAnimTagGrantQuest()
		: base(AnimTagTypes.GrantQuest)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		DBID = root.GetAttrib("DBID", -1);
		return true;
	}
}
