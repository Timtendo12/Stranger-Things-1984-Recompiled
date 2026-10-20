using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagSpawnEffect @ 0x160a844..0x160a918, Ghidra vaddr).
public class FAnimTagSpawnEffect : FAnimTag
{
	public string Name { get; protected set; }

	public FAnimTagSpawnEffect()
		: base(AnimTagTypes.SpawnEffect)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		base.LoadXml(root);
		Name = root.GetAttrib("Name", string.Empty);
		return true;
	}
}
