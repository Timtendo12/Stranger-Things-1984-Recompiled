using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagDetachSkinnedAttachmentsByTag @ 0x160b13c..0x160b5ec, Ghidra vaddr).
public class FAnimTagDetachSkinnedAttachmentsByTag : FAnimTagDetachSkinnedAttachmentBase
{
	protected List<string> mTagNames = new List<string>();

	public List<string> TagNames => mTagNames;

	public FAnimTagDetachSkinnedAttachmentsByTag()
		: base(AnimTagTypes.DetachSkinnedAttachmentsByTag)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			string attrib = root.GetAttrib("Name");
			if (!string.IsNullOrEmpty(attrib))
			{
				mTagNames.Add(attrib);
			}
			foreach (XmlElement item in root.Elements())
			{
				if (item.Name == "Tag")
				{
					string attrib2 = item.GetAttrib("Name");
					if (!string.IsNullOrEmpty(attrib2))
					{
						mTagNames.Add(attrib2);
					}
				}
			}
		}
		return flag;
	}
}
