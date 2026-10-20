using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagTriggerUnit @ 0x160a95c..0x160aa30, Ghidra vaddr).
public class FAnimTagTriggerUnit : FAnimTag
{
	public string Name { get; protected set; }

	public FAnimTagTriggerUnit()
		: base(AnimTagTypes.TriggerUnit)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		base.LoadXml(root);
		Name = root.GetAttrib("Name", string.Empty);
		return true;
	}
}
