// Rewritten from libil2cpp.so 1.0.605 (FTextSpanImage @ 0x16a0420..0x16a0530, Ghidra vaddr).
public class FTextSpanImage : FTextSpan
{
	public FAtlasElement AtlasElement;

	public override int Length => 1;

	// 0x16a04d0: returns literal slot 0x32086b0, the " " that FNameGenerator also uses between first and last name
	public override string WrapString => " ";

	public override FAtlas Atlas
	{
		get
		{
			return AtlasElement.Atlas;
		}
	}

	public void SetElementByName(string elementName)
	{
		AtlasElement = Futile.AtlasManager.GetElementWithName(elementName);
	}
}
