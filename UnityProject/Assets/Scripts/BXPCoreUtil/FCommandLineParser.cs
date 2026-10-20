using System.Text.RegularExpressions;

public class FCommandLineParser
{
	protected static Regex mSplitter;

	protected static Regex mRemover;

	public static FMultiDictionary<string, string> Parse(string[] commandLine, bool lowerParams = false, string defaultValue = "")
	{
		return null;
	}

	public static void Add(FMultiDictionary<string, string> dict, string parameter, string val, bool lowerParams)
	{
	}
}
