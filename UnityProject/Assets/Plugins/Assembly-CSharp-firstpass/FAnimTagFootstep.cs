using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagFootstep @ 0x160ae78..0x160afd8, Ghidra vaddr).
public class FAnimTagFootstep : FAnimTag
{
	protected string mBone;

	protected Fix64 mWarnDistance;

	protected Fix64 mStompDistance;

	public string Bone => mBone;

	public Fix64 WarnDistance => mWarnDistance;

	public Fix64 StompDistance => mStompDistance;

	public FAnimTagFootstep()
		: base(AnimTagTypes.Footstep)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			mBone = root.GetAttrib("Bone");
			mWarnDistance = root.GetAttrib("WarnDistance", Fix64.One);
			mStompDistance = root.GetAttrib("StompDistance", Fix64.Half);
		}
		return flag;
	}
}
