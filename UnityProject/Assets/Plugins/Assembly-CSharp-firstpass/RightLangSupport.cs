using System;

// Rewritten from libil2cpp.so 1.0.605 (RightLangSupport @ 0x15ffccc..0x1600148, Ghidra vaddr).
public static class RightLangSupport
{
	private delegate string Replacer(string str);

	// PC port: the shipped game ran on Android where Environment.NewLine is "\n"; keep that so Windows ("\r\n") does not change the result.
	private const string NewLine = "\n";

	public static string FixAr(string str)
	{
		ArabicFixerTool.showTashkeel = true;
		ArabicFixerTool.combineTashkeel = true;
		ArabicFixerTool.useHinduNumbers = false;
		return Fix(str, ArabicFixerTool.FixLineAr);
	}

	public static string FixHe(string str)
	{
		ArabicFixerTool.showTashkeel = false;
		ArabicFixerTool.combineTashkeel = false;
		ArabicFixerTool.useHinduNumbers = false;
		return Fix(str, ArabicFixerTool.FixLineHe);
	}

	private static string Fix(string str, Replacer replacer)
	{
		if (str.Contains("\\n"))
		{
			str = str.Replace("\\n", NewLine);
		}
		if (str.Contains("\n"))
		{
			str = str.Replace("\n", NewLine);
		}
		if (str.Contains(NewLine))
		{
			string[] lines = str.Split(new string[1] { NewLine }, StringSplitOptions.None);
			if (lines.Length != 1)
			{
				string result = replacer(lines[0]);
				for (int i = 1; i < lines.Length; i++)
				{
					result = result + NewLine + replacer(lines[i]);
				}
				return result;
			}
		}
		return replacer(str);
	}
}
