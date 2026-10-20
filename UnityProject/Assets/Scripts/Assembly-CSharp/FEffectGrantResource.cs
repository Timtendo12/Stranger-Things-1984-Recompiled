using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FEffectGrantResource @ 0x16fc678..0x16fc700, Ghidra vaddr).
public class FEffectGrantResource : FEffect
{
	public int ResourceTypeID { get; set; }

	public int Amount { get; set; }

	public override bool Apply()
	{
		// The shipped body is a stub returning true (resource grants were removed from the 2D game).
		return true;
	}

	public override bool LoadXml(XmlElement root)
	{
		return true;
	}
}
