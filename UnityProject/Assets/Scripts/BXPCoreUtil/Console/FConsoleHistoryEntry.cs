// Rewritten from libil2cpp.so 1.0.605 (FConsoleHistoryEntry, Ghidra vaddr).
public class FConsoleHistoryEntry
{
	public FConsoleHistoryType TextType;

	public string Text;

	public FConsoleHistoryEntry(FConsoleHistoryType textType, string text)
	{
		TextType = textType;
		Text = text;
	}
}
