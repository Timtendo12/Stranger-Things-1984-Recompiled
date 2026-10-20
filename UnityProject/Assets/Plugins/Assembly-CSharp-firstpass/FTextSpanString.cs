// Rewritten from libil2cpp.so 1.0.605 (FTextSpanString @ 0x16a0530..0x16a0650, Ghidra vaddr).
public class FTextSpanString : FTextSpan
{
	public string Text;

	public FFont Font;

	public float PointSize = 1f;

	public override int Length => Text.Length;

	public override string WrapString => Text;

	public override FAtlas Atlas => Font.element.Atlas;

	public override bool IsWhiteSpaceAtIndex(int index)
	{
		if (index >= 0 && index < Text.Length)
		{
			return char.IsWhiteSpace(Text[index]);
		}
		return false;
	}
}
