using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FAnimTagBlock @ 0x160b970..0x160baa4, Ghidra vaddr).
public class FAnimTagBlock : FAnimTag
{
	public bool Unblock;

	public bool DisableProjectileEarlyExit;

	public bool DisableAttackEarlyExit;

	public float FixedDelay;

	public FAnimTagBlock()
		: base(AnimTagTypes.Block)
	{
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			Unblock = root.GetAttrib("Unblock", Unblock);
			FixedDelay = root.GetAttrib("FixedDelay", FixedDelay);
			DisableProjectileEarlyExit = root.GetAttrib("DisableProjectileEarlyExit", DisableProjectileEarlyExit);
			DisableAttackEarlyExit = root.GetAttrib("DisableAttackEarlyExit", DisableAttackEarlyExit);
		}
		return flag;
	}
}
