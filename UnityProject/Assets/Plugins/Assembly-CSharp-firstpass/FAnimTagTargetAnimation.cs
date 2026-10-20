using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagTargetAnimation @ 0x160b6f8..0x160b7ac, Ghidra vaddr).
public class FAnimTagTargetAnimation : FAnimTag
{
	protected string mAnimation;

	public string Animation => mAnimation;

	public FAnimTagTargetAnimation()
		: base(AnimTagTypes.TargetAnimation)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			mAnimation = root.GetAttrib("Animation");
		}
		return flag;
	}
}
