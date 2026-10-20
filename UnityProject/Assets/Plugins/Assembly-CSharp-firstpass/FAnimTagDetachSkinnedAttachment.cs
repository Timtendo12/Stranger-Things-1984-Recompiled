using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagDetachSkinnedAttachment @ 0x160b088..0x160b13c, Ghidra vaddr).
public class FAnimTagDetachSkinnedAttachment : FAnimTagDetachSkinnedAttachmentBase
{
	protected string mName;

	public string Name => mName;

	public FAnimTagDetachSkinnedAttachment()
		: base(AnimTagTypes.DetachSkinnedAttachment)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			mName = root.GetAttrib("Name");
		}
		return flag;
	}
}
