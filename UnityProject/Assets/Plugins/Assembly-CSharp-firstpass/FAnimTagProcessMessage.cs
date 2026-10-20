using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagProcessMessage @ 0x160aa30..0x160ab54, Ghidra vaddr).
public class FAnimTagProcessMessage : FAnimTag
{
	public string Name { get; protected set; }

	public string Data { get; protected set; }

	public FAnimTagProcessMessage()
		: base(AnimTagTypes.ProcessMessage)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			Name = root.GetAttrib("Name", string.Empty);
			Data = root.GetAttrib("Data", string.Empty);
		}
		return flag;
	}
}
