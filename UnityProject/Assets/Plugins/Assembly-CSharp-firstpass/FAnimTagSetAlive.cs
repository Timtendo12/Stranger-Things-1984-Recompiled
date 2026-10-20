using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagSetAlive @ 0x160ab54..0x160ac0c, Ghidra vaddr).
public class FAnimTagSetAlive : FAnimTag
{
	public bool Alive { get; protected set; }

	public FAnimTagSetAlive()
		: base(AnimTagTypes.SetAlive)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			// the shipped game reads the "LaunchBone" attribute here (copy-paste quirk of the original)
			Alive = root.GetAttrib("LaunchBone", Alive);
		}
		return flag;
	}
}
