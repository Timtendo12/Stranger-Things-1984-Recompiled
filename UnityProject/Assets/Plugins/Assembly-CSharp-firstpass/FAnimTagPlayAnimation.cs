using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagPlayAnimation @ 0x160a770..0x160a844, Ghidra vaddr).
public class FAnimTagPlayAnimation : FAnimTag
{
	public string Name { get; protected set; }

	public FAnimTagPlayAnimation()
		: base(AnimTagTypes.PlayAnimation)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		base.LoadXml(root);
		Name = root.GetAttrib("Name", string.Empty);
		return true;
	}
}
