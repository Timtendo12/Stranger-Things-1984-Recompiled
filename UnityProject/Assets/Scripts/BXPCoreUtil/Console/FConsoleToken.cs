// Rewritten from libil2cpp.so 1.0.605 (FConsoleToken, Ghidra vaddr).
public class FConsoleToken
{
	public FConsoleTokenType TokenType;

	public string TokenContents;

	public FConsoleToken()
	{
	}

	public FConsoleToken(FConsoleTokenType tokenType, string tokenContents)
	{
		TokenType = tokenType;
		TokenContents = tokenContents;
	}
}
