using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagLaunchProjectile @ 0x160b7ac..0x160b8b4, Ghidra vaddr).
public class FAnimTagLaunchProjectile : FAnimTag
{
	public string LaunchBone { get; protected set; }

	public string OnLandedEvent { get; protected set; }

	public FAnimTagLaunchProjectile()
		: base(AnimTagTypes.LaunchProjectile)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			LaunchBone = root.GetAttrib("LaunchBone");
			OnLandedEvent = root.GetAttrib("OnLandedEvent");
		}
		return flag;
	}
}
