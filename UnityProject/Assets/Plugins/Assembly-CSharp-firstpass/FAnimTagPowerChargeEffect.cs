using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagPowerChargeEffect @ 0x160a090..0x160a184, Ghidra vaddr).
public class FAnimTagPowerChargeEffect : FAnimTag
{
	protected string mBone;

	protected bool mPrimary;

	public string Bone => mBone;

	public override bool Primary => mPrimary;

	public FAnimTagPowerChargeEffect()
		: base(AnimTagTypes.PowerChargeEffect)
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
