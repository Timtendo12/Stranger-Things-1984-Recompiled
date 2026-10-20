using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagHeal @ 0x160adb8..0x160ae78, Ghidra vaddr).
public class FAnimTagHeal : FAnimTag
{
	public string LaunchBone { get; protected set; }

	public FAnimTagHeal()
		: base(AnimTagTypes.Heal)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			LaunchBone = root.GetAttrib("LaunchBone", LaunchBone);
		}
		return flag;
	}
}
