using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagWork @ 0x160a4bc..0x160a5ac, Ghidra vaddr).
public class FAnimTagWork : FAnimTag
{
	protected string mLaunchBone;

	protected bool mPrimary;

	public string LaunchBone => mLaunchBone;

	public override bool Primary => mPrimary;

	public FAnimTagWork()
		: base(AnimTagTypes.Work)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			mLaunchBone = root.GetAttrib("LaunchBone");
			mPrimary = root.GetAttrib("Primary", true);
		}
		return flag;
	}
}
