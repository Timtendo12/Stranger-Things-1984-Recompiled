using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FRequirementPlayerAttackTargetIsTag @ 0x181dde0..0x1821c80, Ghidra vaddr).
public class FRequirementPlayerAttackTargetIsTag : FPlayerStatsRequirement
{
	public string Tag { get; protected set; }

	public override void Init()
	{
		base.Init();
		Tag = string.Empty;
	}

	public override bool DeepCopy(FRequirement<FPlayerStats> otherRequirement)
	{
		base.DeepCopy(otherRequirement);
		FRequirementPlayerAttackTargetIsTag source = otherRequirement as FRequirementPlayerAttackTargetIsTag;
		if (source != null)
		{
			Tag = source.Tag;
		}
		return source != null;
	}

	public override FRequirement<FPlayerStats> CreateDeepCopy()
	{
		FRequirementPlayerAttackTargetIsTag fRequirement = new FRequirementPlayerAttackTargetIsTag();
		fRequirement.DeepCopy(this);
		return fRequirement;
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			Tag = root.GetAttrib("Tag", Tag);
		}
		return flag;
	}

	protected override EvaluateReturnCode EvaluateCore(FPlayerStats data)
	{
		if (data == null)
		{
			return EvaluateReturnCode.NA;
		}
		if (FApp.World != null && FWorld.Player != null)
		{
			FUnit unitByID = FApp.World.GetUnitByID(FWorld.Player.AI.LastAttackTargetID);
			if (unitByID == null)
			{
				return EvaluateReturnCode.False;
			}
			if (!unitByID.IsTag(Tag))
			{
				return EvaluateReturnCode.False;
			}
		}
		return EvaluateReturnCode.True;
	}
}
