using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FRequirementTwoValueIntOperator @ 0x181f370..0x181f720, Ghidra vaddr).
public abstract class FRequirementTwoValueIntOperator : FPlayerStatsRequirement
{
	public int Value1 { get; protected set; }

	public int Value2 { get; protected set; }

	public MathOperator Operator { get; protected set; }

	public override void Init()
	{
		base.Init();
		Operator = MathOperator.None;
		Value1 = -1;
		Value2 = -1;
	}

	public override bool DeepCopy(FRequirement<FPlayerStats> otherRequirement)
	{
		base.DeepCopy(otherRequirement);
		FRequirementTwoValueIntOperator fRequirementTwoValueIntOperator = otherRequirement as FRequirementTwoValueIntOperator;
		if (fRequirementTwoValueIntOperator != null)
		{
			Value1 = fRequirementTwoValueIntOperator.Value1;
			Value2 = fRequirementTwoValueIntOperator.Value2;
			Operator = fRequirementTwoValueIntOperator.Operator;
		}
		return fRequirementTwoValueIntOperator != null;
	}

	public override bool LoadXml(XmlElement root)
	{
		if (!base.LoadXml(root))
		{
			return false;
		}
		Value1 = root.GetAttrib("Value", Value1);
		Value2 = root.GetAttrib("Value2", Value2);
		if (!GetMathOperator(root.GetAttrib("Op", string.Empty), out var op))
		{
			return false;
		}
		Operator = op;
		return true;
	}
}
