using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FRequirement<> @ 0x1faeb10..0x1faee78, Ghidra vaddr).
[SaveVersion(0)]
public class FRequirement<ContextType> : ISaveRef
{
	public bool Negate { get; protected set; }

	public static FRequirement<ContextType> CreateFromXml(XmlElement element, IRequirementManager<ContextType> manager)
	{
		FRequirement<ContextType> fRequirement = manager.AllocateRequirementForXmlElement(element);
		if (fRequirement != null)
		{
			fRequirement.Init();
			if (!fRequirement.LoadXml(element))
			{
				fRequirement = null;
			}
		}
		return fRequirement;
	}

	public virtual void Init()
	{
		Negate = false;
	}

	public virtual bool DeepCopy(FRequirement<ContextType> other)
	{
		Negate = other.Negate;
		return true;
	}

	public virtual FRequirement<ContextType> CreateDeepCopy()
	{
		return null;
	}

	public virtual bool LoadXml(XmlElement root)
	{
		Negate = root.GetAttrib("Negate", Negate);
		return true;
	}

	public bool GetMathOperator(string operatorString, out MathOperator op)
	{
		MathOperator result = MathOperator.None;
		bool flag = FEnum.TryParse(operatorString, ref result);
		if (!flag)
		{
			result = MathOperator.None;
		}
		op = result;
		return flag;
	}

	public bool Evaluate(ContextType context)
	{
		EvaluateReturnCode evaluateReturnCode = EvaluateCore(context);
		if (evaluateReturnCode != EvaluateReturnCode.NA)
		{
			return (evaluateReturnCode == EvaluateReturnCode.True) != Negate;
		}
		return false;
	}

	protected virtual EvaluateReturnCode EvaluateCore(ContextType context)
	{
		return EvaluateReturnCode.NA;
	}

	protected virtual EvaluateReturnCode EvaluateCount(int currentCount, MathOperator mathOperator, int compareCount1, int compareCount2 = 0)
	{
		switch (mathOperator)
		{
		case MathOperator.LT:
			return (currentCount < compareCount1) ? EvaluateReturnCode.True : EvaluateReturnCode.False;
		case MathOperator.LTE:
			return (currentCount <= compareCount1) ? EvaluateReturnCode.True : EvaluateReturnCode.False;
		case MathOperator.EQ:
			return (currentCount == compareCount1) ? EvaluateReturnCode.True : EvaluateReturnCode.False;
		case MathOperator.NEQ:
			return (currentCount != compareCount1) ? EvaluateReturnCode.True : EvaluateReturnCode.False;
		case MathOperator.GT:
			return (currentCount > compareCount1) ? EvaluateReturnCode.True : EvaluateReturnCode.False;
		case MathOperator.GTE:
			return (currentCount >= compareCount1) ? EvaluateReturnCode.True : EvaluateReturnCode.False;
		case MathOperator.BET:
			return (currentCount >= compareCount1 && currentCount <= compareCount2) ? EvaluateReturnCode.True : EvaluateReturnCode.False;
		case MathOperator.NBT:
			return (currentCount < compareCount1 || currentCount > compareCount2) ? EvaluateReturnCode.True : EvaluateReturnCode.False;
		default:
			return EvaluateReturnCode.NA;
		}
	}

	public virtual void SaveInternal(FSavegameWriter writer)
	{
		writer.Write(Negate);
	}

	public virtual void LoadInternal(FSavegameReader reader)
	{
		Negate = reader.ReadBool();
	}
}
