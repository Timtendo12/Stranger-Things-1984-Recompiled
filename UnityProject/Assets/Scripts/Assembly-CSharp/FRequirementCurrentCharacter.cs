using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FRequirementCurrentCharacter @ 0x181ddc0..0x181fa9c, Ghidra vaddr).
public class FRequirementCurrentCharacter : FPlayerStatsRequirement
{
	public string CharacterName { get; protected set; }

	public override string RequiredCharacter => CharacterName;

	public override void Init()
	{
		base.Init();
		CharacterName = string.Empty;
	}

	public override bool DeepCopy(FRequirement<FPlayerStats> otherRequirement)
	{
		base.DeepCopy(otherRequirement);
		FRequirementCurrentCharacter source = otherRequirement as FRequirementCurrentCharacter;
		if (source != null)
		{
			CharacterName = source.CharacterName;
		}
		return source != null;
	}

	public override FRequirement<FPlayerStats> CreateDeepCopy()
	{
		FRequirementCurrentCharacter fRequirement = new FRequirementCurrentCharacter();
		fRequirement.DeepCopy(this);
		return fRequirement;
	}

	public override bool LoadXml(XmlElement root)
	{
		bool flag = base.LoadXml(root);
		if (flag)
		{
			CharacterName = root.GetAttrib("CharacterName", CharacterName);
		}
		return flag;
	}

	protected override EvaluateReturnCode EvaluateCore(FPlayerStats data)
	{
		if (data == null)
		{
			return EvaluateReturnCode.NA;
		}
		return !(data.CurrentPlayerState.CurrentCharacter != CharacterName) ? EvaluateReturnCode.True : EvaluateReturnCode.False;
	}
}
