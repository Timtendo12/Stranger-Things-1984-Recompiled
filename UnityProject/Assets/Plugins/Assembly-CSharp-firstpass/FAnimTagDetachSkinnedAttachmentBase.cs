using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagDetachSkinnedAttachmentBase @ 0x160afd8..0x160b088, Ghidra vaddr).
public class FAnimTagDetachSkinnedAttachmentBase : FAnimTag
{
	public bool OnTarget;

	public FAnimTagDetachSkinnedAttachmentBase(AnimTagTypes tagType)
		: base(tagType)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			OnTarget = root.GetAttrib("OnTarget", OnTarget);
		}
		return flag;
	}
}
