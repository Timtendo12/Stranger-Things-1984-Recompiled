using System.Reflection;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FRequirementResourceCount @ 0x181ddd4..0x1820ff8, Ghidra vaddr).
public class FRequirementResourceCount : FRequirementTwoValueIntOperator
{
	public string ResourceName { get; protected set; }

	public string Variable { get; protected set; }

	public override void Init()
	{
		base.Init();
		ResourceName = string.Empty;
		Variable = string.Empty;
	}

	public override bool DeepCopy(FRequirement<FPlayerStats> otherRequirement)
	{
		base.DeepCopy(otherRequirement);
		FRequirementResourceCount fRequirementResourceCount = otherRequirement as FRequirementResourceCount;
		if (fRequirementResourceCount != null && base.DeepCopy(otherRequirement))
		{
			ResourceName = fRequirementResourceCount.ResourceName;
			Variable = fRequirementResourceCount.Variable;
			return true;
		}
		return false;
	}

	public override FRequirement<FPlayerStats> CreateDeepCopy()
	{
		FRequirementResourceCount fRequirementResourceCount = new FRequirementResourceCount();
		fRequirementResourceCount.DeepCopy(this);
		return fRequirementResourceCount;
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			ResourceName = root.GetAttrib("ResourceName", ResourceName);
			Variable = root.GetAttribute("Variable", Variable);
		}
		return flag;
	}

	protected override EvaluateReturnCode EvaluateCore(FPlayerStats data)
	{
		if (data == null)
		{
			return EvaluateReturnCode.NA;
		}
		int currentCount = 0;
		PropertyInfo[] properties = typeof(FPlayerStats).GetProperties();
		for (int i = 0; i < properties.Length; i++)
		{
			PropertyInfo propertyInfo = properties[i];
			if (propertyInfo.Name == ResourceName)
			{
				currentCount = (int)propertyInfo.GetValue(FApp.PlayerStats, null);
			}
			else if (propertyInfo.Name == Variable)
			{
				base.Value1 = (int)propertyInfo.GetValue(FApp.PlayerStats, null);
			}
		}
		return EvaluateCount(currentCount, base.Operator, base.Value1);
	}
}
