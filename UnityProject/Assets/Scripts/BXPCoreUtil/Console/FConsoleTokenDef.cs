using System.Text.RegularExpressions;

// Rewritten from libil2cpp.so 1.0.605 (FConsoleTokenDef @ 0x1861fc0..0x1862164, Ghidra vaddr).
public class FConsoleTokenDef
{
	public Regex TokenRegex;

	public FConsoleTokenType TokenType;

	public string ContentsGroup;

	public FConsoleTokenDef(string regexStr, FConsoleTokenType tokenType, string contentsGroup = null)
	{
		TokenRegex = new Regex(string.Format("\\G{0}", regexStr));
		ContentsGroup = contentsGroup;
		TokenType = tokenType;
	}

	public int TryMatch(string str, int startPos, out string contents)
	{
		contents = null;
		Match match = TokenRegex.Match(str, startPos);
		if (!match.Success)
		{
			return 0;
		}
		if (ContentsGroup == null)
		{
			contents = str.Substring(startPos, match.Length);
		}
		else
		{
			Group group = match.Groups[ContentsGroup];
			if (group == null)
			{
				return 0;
			}
			contents = group.Value;
		}
		return match.Length;
	}
}
