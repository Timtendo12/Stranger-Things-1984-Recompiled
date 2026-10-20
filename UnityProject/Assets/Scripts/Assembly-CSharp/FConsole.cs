// Rewritten from libil2cpp.so 1.0.605 (FConsole @ 0x16ee6ec..0x16f51e4, Ghidra vaddr).
public class FConsole : FConsoleCore
{
	public const int cDefaultHistorySize = 1024;

	protected static int ProcessingFromLog;

	protected static int ProcessingOutput;

	public new static void Init()
	{
		mDerivedType = typeof(FConsole);
		mHistory = new CircularBuffer<FConsoleHistoryEntry>(1024, true);
		FLog.WarningEvent += WarningFromLog;
		FLog.ErrorEvent += ErrorFromLog;
		AllConsoleOutputChanged();
		FConsoleCore.OutputEvent += HandleOutput;
		FConsoleCore.WarningEvent += HandleWarning;
		FConsoleCore.ErrorEvent += HandleError;
		FConsoleCore.Init();
		FConsoleCore.Add(typeof(FConsoleCommands));
		FConsoleCore.Add(typeof(FConsoleCommandsConfig));
	}

	public static void HandleOutput(string str)
	{
		if (ProcessingOutput < 1)
		{
			ProcessingOutput++;
			FLog.OutputEvent -= OutputFromLog;
			if (ProcessingFromLog < 1)
			{
				FLog.UnityLog(str);
			}
			if (FConfig.AllConsoleOutput)
			{
				FLog.OutputEvent += OutputFromLog;
			}
			ProcessingOutput--;
		}
	}

	public static void HandleWarning(string str)
	{
		if (ProcessingOutput < 1)
		{
			ProcessingOutput++;
			FLog.WarningEvent -= WarningFromLog;
			if (ProcessingFromLog < 1)
			{
				FLog.UnityWarning(str);
			}
			FLog.WarningEvent += WarningFromLog;
			ProcessingOutput--;
		}
	}

	public static void HandleError(string str)
	{
		if (ProcessingOutput < 1)
		{
			ProcessingOutput++;
			FLog.ErrorEvent -= ErrorFromLog;
			if (ProcessingFromLog < 1)
			{
				FLog.UnityError(str);
			}
			FLog.ErrorEvent += ErrorFromLog;
			ProcessingOutput--;
		}
	}

	public static void OutputFromLog(string str, bool unityLog)
	{
		ProcessingFromLog++;
		if (unityLog)
		{
			FConsoleCore.Output(str);
		}
		ProcessingFromLog--;
	}

	public static void WarningFromLog(string str, bool unityLog)
	{
		ProcessingFromLog++;
		if (unityLog)
		{
			FConsoleCore.Warning(str);
		}
		ProcessingFromLog--;
	}

	public static void ErrorFromLog(string str, bool unityLog)
	{
		ProcessingFromLog++;
		if (unityLog)
		{
			FConsoleCore.Error(str);
		}
		ProcessingFromLog--;
	}

	public static void AllConsoleOutputChanged()
	{
		FLog.OutputEvent -= OutputFromLog;
		if (FConfig.AllConsoleOutput)
		{
			FLog.OutputEvent += OutputFromLog;
		}
	}
}
