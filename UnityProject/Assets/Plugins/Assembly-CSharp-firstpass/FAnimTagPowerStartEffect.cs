using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagPowerStartEffect @ 0x160a184..0x160a278, Ghidra vaddr).
public class FAnimTagPowerStartEffect : FAnimTag
{
	protected string mBone;

	protected bool mPrimary;

	public string Bone => mBone;

	public override bool Primary => mPrimary;

	public FAnimTagPowerStartEffect()
		: base(AnimTagTypes.PowerStartEffect)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			mBone = root.GetAttrib("Bone");
			mPrimary = root.GetAttrib("Primary", true);
		}
		return flag;
	}
}
