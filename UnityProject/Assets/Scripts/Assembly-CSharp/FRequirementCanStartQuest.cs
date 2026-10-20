using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FRequirementCanStartQuest @ 0x181ddcc..0x1820970, Ghidra vaddr).
public class FRequirementCanStartQuest : FPlayerStatsRequirement
{
	public int DBID { get; protected set; }

	public override void Init()
	{
		base.Init();
		DBID = -1;
	}

	public override bool DeepCopy(FRequirement<FPlayerStats> otherRequirement)
	{
		base.DeepCopy(otherRequirement);
		FRequirementCanStartQuest source = otherRequirement as FRequirementCanStartQuest;
		if (source != null)
		{
			DBID = source.DBID;
		}
		return source != null;
	}

	public override FRequirement<FPlayerStats> CreateDeepCopy()
	{
		FRequirementCanStartQuest fRequirement = new FRequirementCanStartQuest();
		fRequirement.DeepCopy(this);
		return fRequirement;
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			DBID = root.GetAttrib("DBID", DBID);
		}
		return flag;
	}

	protected override EvaluateReturnCode EvaluateCore(FPlayerStats data)
	{
		if (data == null)
		{
			return EvaluateReturnCode.NA;
		}
		return data.CanStartQuest(DBID) ? EvaluateReturnCode.True : EvaluateReturnCode.False;
	}

	public override void SaveInternal(FSavegameWriter writer)
	{
		base.SaveInternal(writer);
		writer.Write(DBID);
	}

	public override void LoadInternal(FSavegameReader reader)
	{
		base.LoadInternal(reader);
		DBID = reader.ReadInt();
	}
}
