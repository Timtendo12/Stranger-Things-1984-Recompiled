using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagPowerEndEffect @ 0x160a278..0x160a36c, Ghidra vaddr).
public class FAnimTagPowerEndEffect : FAnimTag
{
	protected string mBone;

	protected bool mPrimary;

	public string Bone => mBone;

	public override bool Primary => mPrimary;

	public FAnimTagPowerEndEffect()
		: base(AnimTagTypes.PowerEndEffect)
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
