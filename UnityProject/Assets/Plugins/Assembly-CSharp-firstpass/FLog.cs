using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FLog @ 0x165d784..0x165f794, Ghidra vaddr).
public class FLog
{
	public delegate void UpdateLog(ref string text, bool forUnityConsole);

	protected static StreamWriter mLogFile;

	protected static List<UpdateLog> mUpdateLogHandlers = new List<UpdateLog>();

	protected static List<string> mPreInitCache = new List<string>();

	protected static string mFilenameWithoutExtension;

	protected static string mExtension;

	protected static int mKeepOldCount;

	[ThreadStatic]
	protected static bool mPerformingUnityOutput;

	protected static object mLock = new object();

	public static bool PerformingUnityOutput => mPerformingUnityOutput;

	public static string LogFileName => (mLogFile?.BaseStream as FileStream)?.Name;

	public static event Action<string, bool> OutputEvent;

	public static event Action<string, bool> WarningEvent;

	public static event Action<string, bool> ErrorEvent;

	public static string GetOldFilename(int revisionNumber)
	{
		return mFilenameWithoutExtension + "_old" + revisionNumber.ToString() + "." + mExtension;
	}

	public static void AddFilenameList(List<string> files)
	{
		files.Add(LogFileName);
		for (int i = 1; i <= mKeepOldCount; i++)
		{
			files.Add(GetOldFilename(i));
		}
	}

	public static void Init(string name, int keepOldCount = 3)
	{
		mKeepOldCount = keepOldCount;
		string path = FUtil.PersistentDataPath + "/" + name;
		int dot = path.LastIndexOf(".");
		if (dot < 0)
		{
			mFilenameWithoutExtension = path;
			mExtension = "";
		}
		else
		{
			mFilenameWithoutExtension = path.Substring(0, dot);
			mExtension = path.Substring(dot + 1);
		}
		for (int i = keepOldCount; i > 1; i--)
		{
			try
			{
				FCoreUtil.MoveFileWithOverwrite(GetOldFilename(i - 1), GetOldFilename(i));
			}
			catch (Exception)
			{
			}
		}
		if (keepOldCount > 0)
		{
			try
			{
				FCoreUtil.MoveFileWithOverwrite(path, GetOldFilename(1));
			}
			catch (Exception)
			{
			}
		}
		FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
		mLogFile = new StreamWriter(stream);
		Debug.Log("Logfile is located at: " + path);
		mLogFile.WriteLine("File: " + name);
		mLogFile.WriteLine("");
		if (mPreInitCache != null)
		{
			foreach (string line in mPreInitCache)
			{
				mLogFile.WriteLine(line);
			}
		}
		mLogFile.Flush();
		mPreInitCache = null;
	}

	public static void AddUpdateLogHandler(UpdateLog handler, bool endOfList)
	{
		if (endOfList)
		{
			mUpdateLogHandlers.Add(handler);
		}
		else
		{
			mUpdateLogHandlers.Insert(0, handler);
		}
	}

	public static void RemoveUpdateLogHandler(UpdateLog handler)
	{
		mUpdateLogHandlers.Remove(handler);
	}

	protected static void UpdateLogMessage(ref string text, bool forUnityConsole)
	{
		foreach (UpdateLog handler in mUpdateLogHandlers)
		{
			handler(ref text, forUnityConsole);
		}
	}

	public static void Log(string text, bool unityLog = false, UnityEngine.Object obj = null)
	{
		FinalLog(text, unityLog, obj);
	}

	public static void Warning(string text, bool unityLog = false, UnityEngine.Object obj = null)
	{
		FinalWarning(text, unityLog, obj);
	}

	public static void Error(string text, bool unityLog = false, UnityEngine.Object obj = null)
	{
		FinalError(text, unityLog, obj);
	}

	public static void UnityLog(string text, UnityEngine.Object obj = null)
	{
		Log(text, true, obj);
	}

	public static void UnityWarning(string text, UnityEngine.Object obj = null)
	{
		Warning(text, true, obj);
	}

	public static void UnityError(string text, UnityEngine.Object obj = null)
	{
		Error(text, true, obj);
	}

	public static void FinalLog(string text, bool unityLog = false, UnityEngine.Object obj = null)
	{
		bool toUnity = FConfigCore.ForceUnityLog || unityLog;
		if (toUnity)
		{
			string consoleText = text;
			UpdateLogMessage(ref consoleText, true);
			mPerformingUnityOutput = true;
			Debug.Log(consoleText, obj);
			mPerformingUnityOutput = false;
		}
		OutputEvent?.Invoke(text, toUnity);
		Output(text);
	}

	public static void FinalWarning(string text, bool unityLog = false, UnityEngine.Object obj = null)
	{
		bool toUnity = FConfigCore.ForceUnityLog || unityLog;
		if (toUnity)
		{
			string consoleText = text;
			UpdateLogMessage(ref consoleText, true);
			mPerformingUnityOutput = true;
			Debug.LogWarning(consoleText, obj);
			mPerformingUnityOutput = false;
		}
		WarningEvent?.Invoke(text, toUnity);
		Output(text);
	}

	public static void FinalError(string text, bool unityLog = false, UnityEngine.Object obj = null)
	{
		bool toUnity = FConfigCore.ForceUnityLog || unityLog;
		if (toUnity)
		{
			string consoleText = text;
			UpdateLogMessage(ref consoleText, true);
			mPerformingUnityOutput = true;
			Debug.LogError(consoleText, obj);
			mPerformingUnityOutput = false;
		}
		ErrorEvent?.Invoke(text, toUnity);
		Output(text);
	}

	public static void FinalUnityLog(string text, UnityEngine.Object obj = null)
	{
		FinalLog(text, true, obj);
	}

	public static void FinalUnityWarning(string text, UnityEngine.Object obj = null)
	{
		FinalWarning(text, true, obj);
	}

	public static void FinalUnityError(string text, UnityEngine.Object obj = null)
	{
		FinalError(text, true, obj);
	}

	protected static void Output(string text)
	{
		lock (mLock)
		{
			UpdateLogMessage(ref text, false);
			if (mLogFile == null)
			{
				if (mPreInitCache == null)
				{
					mPreInitCache = new List<string>();
				}
				mPreInitCache.Add(text);
			}
			else
			{
				mLogFile.WriteLine(text);
				mLogFile.Flush();
			}
		}
	}

	public static void Close()
	{
		try
		{
			mLogFile.Close();
		}
		catch (Exception)
		{
		}
		mLogFile = null;
	}
}
