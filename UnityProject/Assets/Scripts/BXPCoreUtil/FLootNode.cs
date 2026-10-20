using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FLootNode<,> @ 0x1faa538..0x1faf100, Ghidra vaddr).
public abstract class FLootNode<ContextType, ResultType> where ResultType : class
{
	public int MinCount = 1;

	public int MaxCount = 1;

	public List<FRequirement<ContextType>> Requirements = new List<FRequirement<ContextType>>();

	protected ILootTableManager<ContextType, ResultType> mManager;

	public int DBID { get; protected set; }

	public FLootNode(ILootTableManager<ContextType, ResultType> manager)
	{
		mManager = manager;
	}

	public static FLootNode<ContextType, ResultType> AllocateNodeForXmlElement(XmlElement xmlNode, ILootTableManager<ContextType, ResultType> manager)
	{
		if (xmlNode.Name == "Result")
		{
			return new FLootNodeResult<ContextType, ResultType>(manager);
		}
		if (xmlNode.Name == "Empty")
		{
			return new FLootNodeEmpty<ContextType, ResultType>(manager);
		}
		if (xmlNode.Name == "And")
		{
			return new FLootNodeAnd<ContextType, ResultType>(manager);
		}
		if (xmlNode.Name == "Or")
		{
			return new FLootNodeOr<ContextType, ResultType>(manager);
		}
		if (xmlNode.Name == "Include")
		{
			return new FLootNodeInclude<ContextType, ResultType>(manager);
		}
		return null;
	}

	public virtual bool CheckRequirements(ContextType context)
	{
		for (int i = 0; i < Requirements.Count; i++)
		{
			if (!Requirements[i].Evaluate(context))
			{
				return false;
			}
		}
		return true;
	}

	public virtual void Evaluate(ContextType context, List<ResultType> resultItems, FRandom random)
	{
		if (CheckRequirements(context))
		{
			int num = random.RangeInt(MinCount, MaxCount, "FLootNode.Evaluate string count");
			while (num > 0)
			{
				EvaluateOnce(context, resultItems, random);
				num--;
			}
		}
	}

	public virtual bool LoadXml(XmlElement root)
	{
		int result = 0;
		if (FParse.TryParseInt(root.GetAttribute("Count"), out result))
		{
			MinCount = result;
			MaxCount = result;
		}
		else
		{
			MinCount = root.GetAttrib("MinCount", -1);
			MaxCount = root.GetAttrib("MaxCount", -1);
		}
		if (MinCount < 0)
		{
			if (MaxCount < 0)
			{
				MaxCount = 1;
			}
			MinCount = MaxCount;
		}
		else if (MaxCount < 0)
		{
			MaxCount = MinCount;
		}
		else if (MaxCount < MinCount)
		{
			FCoreUtil.Swap(ref MinCount, ref MaxCount);
		}
		DBID = root.GetAttrib("DBID", -1);
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType == XmlNodeType.Element)
			{
				ParseXmlElement(childNode as XmlElement);
			}
		}
		return PostLoadXml();
	}

	protected virtual bool ParseXmlElement(XmlElement element)
	{
		if (element.Name != "Require")
		{
			return false;
		}
		FRequirement<ContextType> fRequirement = mManager.AllocateRequirementForXmlElement(element);
		if (fRequirement == null)
		{
			return false;
		}
		fRequirement.Init();
		if (!fRequirement.LoadXml(element))
		{
			return false;
		}
		Requirements.Add(fRequirement);
		return true;
	}

	protected abstract void EvaluateOnce(ContextType context, List<ResultType> resultItems, FRandom random);

	protected virtual bool PostLoadXml()
	{
		return true;
	}
}
