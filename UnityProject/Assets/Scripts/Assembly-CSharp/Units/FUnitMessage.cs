// Rewritten from libil2cpp.so 1.0.605 (FUnitMessage @ 0x1752e0c..0x1752ed0, Ghidra vaddr).
public class FUnitMessage
{
	public int SenderID { get; set; } = -1;

	public int RecipientID { get; set; } = -1;

	public string Message { get; set; } = string.Empty;

	public string Data { get; set; } = string.Empty;
}
