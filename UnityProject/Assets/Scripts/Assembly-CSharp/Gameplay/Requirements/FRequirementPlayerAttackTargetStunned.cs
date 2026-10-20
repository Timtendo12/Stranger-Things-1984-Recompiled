using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FRequirementPlayerAttackTargetStunned @ 0x181dddc..0x1821800, Ghidra vaddr).
public class FRequirementPlayerAttackTargetStunned : FPlayerStatsRequirement
{
	public override bool DeepCopy(FRequirement<FPlayerStats> otherRequirement)
	{
		base.DeepCopy(otherRequirement);
		return otherRequirement is FRequirementPlayerAttackTargetStunned;
	}

	public override FRequirement<FPlayerStats> CreateDeepCopy()
	{
		FRequirementPlayerAttackTargetStunned fRequirementPlayerAttackTargetStunned = new FRequirementPlayerAttackTargetStunned();
		fRequirementPlayerAttackTargetStunned.DeepCopy(this);
		return fRequirementPlayerAttackTargetStunned;
	}

	public override bool LoadXml(XmlElement root)
	{
		return base.LoadXml(root);
	}

	protected override EvaluateReturnCode EvaluateCore(FPlayerStats data)
	{
		if (data == null)
		{
			return EvaluateReturnCode.NA;
		}
		if (FApp.World != null && FWorld.Player != null && FWorld.Player.AI != null)
		{
			FUnit unitByID = FApp.World.GetUnitByID(FWorld.Player.AI.LastAttackTargetID);
			if (unitByID != null)
			{
				if (unitByID.AI == null)
				{
					return EvaluateReturnCode.False;
				}
				if (!unitByID.AI.GetHasActionOfType<FUnitActionStunned>())
				{
					return EvaluateReturnCode.False;
				}
			}
		}
		return EvaluateReturnCode.True;
	}
}
