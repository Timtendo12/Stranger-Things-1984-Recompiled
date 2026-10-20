using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FPlayerStatsRequirement @ 0x181f200..0x181f340, Ghidra vaddr).
public class FPlayerStatsRequirement : FRequirement<FPlayerStats>
{
	public virtual string RequiredCharacter => string.Empty;

	public static FRequirement<FPlayerStats> AllocateRequirementForXmlElement(XmlElement element)
	{
		if (element.GetAttribute("Type") == "CurrentCharacter")
		{
			return new FRequirementCurrentCharacter();
		}
		return null;
	}

	public override FRequirement<FPlayerStats> CreateDeepCopy()
	{
		return null;
	}
}
