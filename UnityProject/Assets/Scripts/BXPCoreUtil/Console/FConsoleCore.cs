using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

// Rewritten from libil2cpp.so 1.0.605 (FConsoleCore @ 0x18616f8..0x1865004, Ghidra vaddr).
public class FConsoleCore
{
	[CompilerGenerated]
	private static Action<string> m_OutputEvent;

	[CompilerGenerated]
	private static Action<string> m_WarningEvent;

	[CompilerGenerated]
	private static Action<string> m_ErrorEvent;

	[CompilerGenerated]
	private static Action<string> m_CommandExecutedEvent;

	protected static Type mDerivedType;

	protected static Dictionary<string, FConsoleCommandEntry> mMethods;

	protected static FConsoleParserState mParserState;

	protected static FConsoleCommandEntry mMethodEntry;

	protected static List<string> mParamStrings;

	protected static CircularBuffer<FConsoleHistoryEntry> mHistory;

	protected static FConsoleTokenDef[] mTokenDefs;

	public static CircularBuffer<FConsoleHistoryEntry> History => mHistory;

	public static event Action<string> OutputEvent;

	public static event Action<string> WarningEvent;

	public static event Action<string> ErrorEvent;

	public static event Action<string> CommandExecutedEvent;

	static FConsoleCore()
	{
		mDerivedType = typeof(FConsoleCore);
		mMethods = new Dictionary<string, FConsoleCommandEntry>();
		mParserState = FConsoleParserState.Command;
		mParamStrings = new List<string>();
		mTokenDefs = new FConsoleTokenDef[8]
		{
			new FConsoleTokenDef(@"[;\r\n]", FConsoleTokenType.EndOfStatement),
			new FConsoleTokenDef(@"([""])(?<string>(?:\\\\|\\\1|.)*?)\1", FConsoleTokenType.QuotedString, "string"),
			new FConsoleTokenDef(@"(?m)(//.*)$", FConsoleTokenType.Comment),
			new FConsoleTokenDef(",", FConsoleTokenType.Comma),
			new FConsoleTokenDef(@"\(", FConsoleTokenType.OpenParen),
			new FConsoleTokenDef(@"\)", FConsoleTokenType.CloseParen),
			new FConsoleTokenDef(@"[A-Za-z\d\.\-\+]+", FConsoleTokenType.Symbol),
			new FConsoleTokenDef(@"\s*", FConsoleTokenType.Whitespace)
		};
	}

	public static void Init()
	{
		Add(mDerivedType);
	}

	public static void Add(Type type)
	{
		AddMethods(type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy));
	}

	public static void AddMethods(MethodInfo[] methods)
	{
		for (int i = 0; i < methods.Length; i++)
		{
			MethodInfo methodInfo = methods[i];
			object[] customAttributes = methodInfo.GetCustomAttributes(typeof(ConsoleCommand), false);
			if (customAttributes.Length == 0)
			{
				continue;
			}
			string key = methodInfo.Name.ToLower();
			FConsoleCommandEntry fConsoleCommandEntry = mMethods.Get(key);
			if (fConsoleCommandEntry == null)
			{
				fConsoleCommandEntry = new FConsoleCommandEntry();
				fConsoleCommandEntry.CommandAttrib = (ConsoleCommand)customAttributes[0];
				fConsoleCommandEntry.Method = methodInfo;
				mMethods.Add(key, fConsoleCommandEntry);
			}
			else
			{
				FCoreUtil.Error(string.Concat("Console command with duplicate name: ", fConsoleCommandEntry.Method?.ToString(), " in type: ", fConsoleCommandEntry.Method.DeclaringType?.ToString(), ".  Ignoring: ", methodInfo.ToString(), " in type: ", methodInfo.DeclaringType?.ToString()));
			}
		}
	}

	protected static void AddParamString(ref List<string> paramStrings, StringBuilder sb, bool quoted)
	{
		string text = sb.ToString();
		if (!quoted)
		{
			text = text.Trim();
			if (text == "null")
			{
				text = null;
			}
		}
		paramStrings.Add(text);
	}

	protected static bool ParseCommand(FConsoleToken token)
	{
		if (token.TokenType == FConsoleTokenType.Symbol)
		{
			string tokenContents = token.TokenContents;
			mMethodEntry = mMethods.Get(tokenContents.ToLower());
			if (mMethodEntry != null)
			{
				SetParserState(FConsoleParserState.WaitForParams);
				return true;
			}
			Error("Unknown console command: " + tokenContents);
		}
		else
		{
			Error("Error parsing command (unexpected " + token.TokenType.ToString() + ").");
		}
		return false;
	}

	protected static bool ParseWaitForParams(FConsoleToken token)
	{
		FConsoleTokenType tokenType = token.TokenType;
		if (tokenType == FConsoleTokenType.OpenParen)
		{
			SetParserState(FConsoleParserState.Param);
		}
		else
		{
			Error("Error parsing command (unexpected " + tokenType.ToString() + ").");
		}
		return tokenType == FConsoleTokenType.OpenParen;
	}

	protected static bool ParseParam(FConsoleToken token)
	{
		FConsoleTokenType tokenType = token.TokenType;
		if (tokenType == FConsoleTokenType.CloseParen)
		{
			SetParserState(FConsoleParserState.WaitForEnd);
			return true;
		}
		if (tokenType != FConsoleTokenType.Symbol && tokenType != FConsoleTokenType.QuotedString)
		{
			Error("Error parsing command (unexpected " + tokenType.ToString() + ").");
			return false;
		}
		mParamStrings.Add(Regex.Unescape(token.TokenContents));
		SetParserState(FConsoleParserState.WaitForNextParam);
		return true;
	}

	protected static bool ParseWaitForNextParam(FConsoleToken token)
	{
		FConsoleTokenType tokenType = token.TokenType;
		if (tokenType == FConsoleTokenType.CloseParen)
		{
			SetParserState(FConsoleParserState.WaitForEnd);
		}
		else
		{
			if (tokenType != FConsoleTokenType.Comma)
			{
				Error("Error parsing command (unexpected " + tokenType.ToString() + ").");
				return false;
			}
			SetParserState(FConsoleParserState.Param);
		}
		return true;
	}

	protected static bool ParseWaitForEnd(FConsoleToken token)
	{
		Error("Error parsing command (unexpected " + token.TokenType.ToString() + ").");
		return false;
	}

	protected static bool ExecuteParsedCommand()
	{
		if (mMethodEntry == null)
		{
			return true;
		}
		ParameterInfo[] parameters = mMethodEntry.Method.GetParameters();
		if (parameters.Length < mParamStrings.Count)
		{
			Error("Too many parameters! Expecting " + parameters.Length + ", but had " + mParamStrings.Count);
			return false;
		}
		object[] array = (parameters.Length == 0) ? null : new object[parameters.Length];
		for (int i = 0; i < parameters.Length; i++)
		{
			if (i < mParamStrings.Count)
			{
				if (mParamStrings[i] == null)
				{
					array[i] = null;
					continue;
				}
				array[i] = TryParseParam(mParamStrings[i], parameters[i].ParameterType);
				if (array[i] == null)
				{
					return false;
				}
				continue;
			}
			if (parameters[i].RawDefaultValue == DBNull.Value)
			{
				Error("Not enough parameters!");
				Output(mMethodEntry.GetHelpString());
				return false;
			}
			array[i] = parameters[i].RawDefaultValue;
		}
		mMethodEntry.Method.SafeInvoke(null, array);
		SetParserState(FConsoleParserState.Command);
		return true;
	}

	protected static void SetParserState(FConsoleParserState newState)
	{
		if (newState == FConsoleParserState.Command)
		{
			mMethodEntry = null;
			mParamStrings.Clear();
		}
		mParserState = newState;
	}

	public static bool Execute(string str, bool addCommandHistory = false)
	{
		if (addCommandHistory)
		{
			CommandExecuted(str);
		}
		List<FConsoleToken> list = new List<FConsoleToken>();
		int num = 0;
		while (num < str.Length)
		{
			int num2 = 0;
			string contents = null;
			FConsoleTokenDef fConsoleTokenDef = null;
			for (int i = 0; i < mTokenDefs.Length; i++)
			{
				num2 = mTokenDefs[i].TryMatch(str, num, out contents);
				if (num2 > 0)
				{
					fConsoleTokenDef = mTokenDefs[i];
					break;
				}
			}
			if (fConsoleTokenDef == null)
			{
				break;
			}
			list.Add(new FConsoleToken(fConsoleTokenDef.TokenType, contents));
			num += num2;
		}
		SetParserState(FConsoleParserState.Command);
		foreach (FConsoleToken item in list)
		{
			FConsoleTokenType tokenType = item.TokenType;
			if (tokenType == FConsoleTokenType.Whitespace)
			{
				continue;
			}
			if ((tokenType == FConsoleTokenType.EndOfStatement || tokenType == FConsoleTokenType.Comment) && ExecuteParsedCommand())
			{
				continue;
			}
			bool flag;
			switch (mParserState)
			{
			case FConsoleParserState.Command:
				flag = ParseCommand(item);
				break;
			case FConsoleParserState.WaitForParams:
				flag = ParseWaitForParams(item);
				break;
			case FConsoleParserState.Param:
				flag = ParseParam(item);
				break;
			case FConsoleParserState.WaitForNextParam:
				flag = ParseWaitForNextParam(item);
				break;
			case FConsoleParserState.WaitForEnd:
				ParseWaitForEnd(item);
				flag = false;
				break;
			default:
				flag = false;
				break;
			}
			if (!flag)
			{
				return false;
			}
		}
		ExecuteParsedCommand();
		return true;
	}

	protected static object TryParseParam(string str, Type type)
	{
		return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(str);
	}

	[ConsoleCommand("Output a message")]
	public static void Output(string str)
	{
		if (mHistory != null)
		{
			mHistory.Put(new FConsoleHistoryEntry(FConsoleHistoryType.Output, str));
		}
		OutputEvent?.Invoke(str);
	}

	[ConsoleCommand("Output a warning message")]
	public static void Warning(string str)
	{
		if (mHistory != null)
		{
			mHistory.Put(new FConsoleHistoryEntry(FConsoleHistoryType.Warning, str));
		}
		WarningEvent?.Invoke(str);
	}

	[ConsoleCommand("Output an error message")]
	public static void Error(string str)
	{
		if (mHistory != null)
		{
			mHistory.Put(new FConsoleHistoryEntry(FConsoleHistoryType.Error, str));
		}
		ErrorEvent?.Invoke(str);
	}

	public static void CommandExecuted(string str)
	{
		if (mHistory != null)
		{
			mHistory.Put(new FConsoleHistoryEntry(FConsoleHistoryType.Command, str));
		}
		CommandExecutedEvent?.Invoke(str);
	}

	[ConsoleCommand("test parameters")]
	public static void TestParam(int i, float f, string s = "some default string")
	{
		Warning("TestParam, i=" + i + " f=" + f + " s=" + s);
	}

	[ConsoleCommand("Shows help. 'Help' alone shows all commands. 'Help(command)' shows help on that specific command.")]
	public static void Help(string command = null)
	{
		if (command == null)
		{
			foreach (FConsoleCommandEntry value in mMethods.Values)
			{
				Output(value.GetHelpString());
			}
			return;
		}
		string text = command.Trim();
		FConsoleCommandEntry fConsoleCommandEntry = mMethods.Get(text.ToLower());
		if (fConsoleCommandEntry != null)
		{
			Output(fConsoleCommandEntry.GetHelpString());
		}
		else
		{
			Error("Command '" + text + "' not found.  Type 'help' to list all commands'");
		}
	}
}
