using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FRequirementMapPack @ 0x181ddd8..0x182141c, Ghidra vaddr).
public class FRequirementMapPack : FPlayerStatsRequirement
{
	public string MapPack { get; protected set; }

	public override void Init()
	{
		base.Init();
		MapPack = string.Empty;
	}

	public override bool DeepCopy(FRequirement<FPlayerStats> otherRequirement)
	{
		base.DeepCopy(otherRequirement);
		FRequirementMapPack source = otherRequirement as FRequirementMapPack;
		if (source != null)
		{
			MapPack = source.MapPack;
		}
		return source != null;
	}

	public override FRequirement<FPlayerStats> CreateDeepCopy()
	{
		FRequirementMapPack fRequirement = new FRequirementMapPack();
		fRequirement.DeepCopy(this);
		return fRequirement;
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			MapPack = root.GetAttrib("MapPack", MapPack);
		}
		return flag;
	}

	protected override EvaluateReturnCode EvaluateCore(FPlayerStats data)
	{
		if (data == null)
		{
			return EvaluateReturnCode.NA;
		}
		if (FApp.World == null)
		{
			return EvaluateReturnCode.False;
		}
		return (!(FApp.World.ProtoWorld.MapPack != MapPack)) ? EvaluateReturnCode.True : EvaluateReturnCode.False;
	}

	public override void SaveInternal(FSavegameWriter writer)
	{
		base.SaveInternal(writer);
		writer.Write(MapPack);
	}

	public override void LoadInternal(FSavegameReader reader)
	{
		base.LoadInternal(reader);
		MapPack = reader.ReadString();
	}
}
