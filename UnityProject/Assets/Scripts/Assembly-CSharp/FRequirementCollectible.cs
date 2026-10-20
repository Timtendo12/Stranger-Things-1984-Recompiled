using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FRequirementCollectible @ 0x181ddc4..0x1820000, Ghidra vaddr).
public class FRequirementCollectible : FPlayerStatsRequirement
{
	public int DBID { get; protected set; }

	public bool FailIfCollected { get; protected set; }

	public override void Init()
	{
		base.Init();
		FailIfCollected = false;
		DBID = -1;
	}

	public override bool DeepCopy(FRequirement<FPlayerStats> otherRequirement)
	{
		base.DeepCopy(otherRequirement);
		FRequirementCollectible source = otherRequirement as FRequirementCollectible;
		if (source != null)
		{
			DBID = source.DBID;
			FailIfCollected = source.FailIfCollected;
		}
		return source != null;
	}

	public override FRequirement<FPlayerStats> CreateDeepCopy()
	{
		FRequirementCollectible fRequirement = new FRequirementCollectible();
		fRequirement.DeepCopy(this);
		return fRequirement;
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			DBID = root.GetAttrib("DBID", DBID);
			FailIfCollected = root.GetAttrib("FailIfCollected", FailIfCollected);
		}
		return flag;
	}

	protected override EvaluateReturnCode EvaluateCore(FPlayerStats data)
	{
		if (data == null)
		{
			return EvaluateReturnCode.NA;
		}
		bool failIfCollected = FailIfCollected;
		if (DBID != -1)
		{
			bool flag = data.HasCollectible(DBID);
			if (!failIfCollected)
			{
				if (flag)
				{
					return EvaluateReturnCode.True;
				}
			}
			else if (!flag)
			{
				return EvaluateReturnCode.True;
			}
			return EvaluateReturnCode.False;
		}
		if (!failIfCollected)
		{
			if (data.CurrentPlayerState.Collectibles.Count != FApp.ProtoCollectibleManager.ProtoCollectibles.Count)
			{
				return EvaluateReturnCode.False;
			}
			return EvaluateReturnCode.True;
		}
		if (data.CurrentPlayerState.Collectibles.Count == FApp.ProtoCollectibleManager.ProtoCollectibles.Count)
		{
			return EvaluateReturnCode.False;
		}
		return EvaluateReturnCode.True;
	}
}
