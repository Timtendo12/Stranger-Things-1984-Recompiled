using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagDetachSkinnedAttachments @ 0x160b650..0x160b6f8, Ghidra vaddr).
public class FAnimTagDetachSkinnedAttachments : FAnimTagDetachSkinnedAttachmentBase
{
	public int Count = -1;

	public FAnimTagDetachSkinnedAttachments()
		: base(AnimTagTypes.DetachSkinnedAttachments)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			Count = root.GetAttrib("Count", Count);
		}
		return flag;
	}
}
