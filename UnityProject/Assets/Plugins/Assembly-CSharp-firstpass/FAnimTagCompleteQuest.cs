using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagCompleteQuest @ 0x160a6c4..0x160a770, Ghidra vaddr).
public class FAnimTagCompleteQuest : FAnimTag
{
	public int DBID { get; protected set; }

	public FAnimTagCompleteQuest()
		: base(AnimTagTypes.CompleteQuest)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		DBID = root.GetAttrib("DBID", -1);
		return true;
	}
}
