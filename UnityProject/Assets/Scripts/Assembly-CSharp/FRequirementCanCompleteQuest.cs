using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FRequirementCanCompleteQuest @ 0x181ddd0..0x1820678, Ghidra vaddr).
public class FRequirementCanCompleteQuest : FPlayerStatsRequirement
{
	public int DBID { get; protected set; }

	public bool MustHaveQuest { get; protected set; }

	public override void Init()
	{
		base.Init();
		DBID = -1;
		MustHaveQuest = true;
	}

	public override bool DeepCopy(FRequirement<FPlayerStats> otherRequirement)
	{
		base.DeepCopy(otherRequirement);
		FRequirementCanCompleteQuest source = otherRequirement as FRequirementCanCompleteQuest;
		if (source != null)
		{
			DBID = source.DBID;
			MustHaveQuest = source.MustHaveQuest;
		}
		return source != null;
	}

	public override FRequirement<FPlayerStats> CreateDeepCopy()
	{
		FRequirementCanCompleteQuest fRequirement = new FRequirementCanCompleteQuest();
		fRequirement.DeepCopy(this);
		return fRequirement;
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			DBID = root.GetAttrib("DBID", DBID);
			MustHaveQuest = root.GetAttrib("MustHaveQuest", MustHaveQuest);
		}
		return flag;
	}

	protected override EvaluateReturnCode EvaluateCore(FPlayerStats data)
	{
		if (data == null)
		{
			return EvaluateReturnCode.NA;
		}
		return data.CanCompleteQuest(DBID, MustHaveQuest) ? EvaluateReturnCode.True : EvaluateReturnCode.False;
	}
}
