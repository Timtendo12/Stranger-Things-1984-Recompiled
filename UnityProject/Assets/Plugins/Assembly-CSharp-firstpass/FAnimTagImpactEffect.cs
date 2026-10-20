using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagImpactEffect @ 0x1609f60..0x160a090, Ghidra vaddr).
public class FAnimTagImpactEffect : FAnimTag
{
	protected string mImpactEffectName;

	public bool OnTarget;

	public bool FirstTargetOnly;

	public string ImpactEffectName => mImpactEffectName;

	public FAnimTagImpactEffect()
		: base(AnimTagTypes.ImpactEffect)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		if (base.LoadXml(root))
		{
			mImpactEffectName = root.GetAttrib("ImpactEffectName");
			if (!string.IsNullOrEmpty(mImpactEffectName))
			{
				OnTarget = root.GetAttrib("OnTarget", OnTarget);
				FirstTargetOnly = root.GetAttrib("FirstTargetOnly", FirstTargetOnly);
				return true;
			}
		}
		return false;
	}
}
