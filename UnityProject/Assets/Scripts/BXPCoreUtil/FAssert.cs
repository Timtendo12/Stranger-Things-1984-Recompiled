using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

public class FAssert
{
	public enum DialogResult
	{
		Send = 0,
		Ignore = 1,
		IgnoreForever = 2
	}

	public delegate bool ShowCommentsDialogDelegate(string title, string defaultName, bool canCancel, out string name, out string comments);

	[CompilerGenerated]
	private static Action<string, string, bool> m_SendReportEvent;

	protected static HashSet<string> mIgnoreForever = new HashSet<string>();

	public static Action<string, StackTrace> ShowExceptionDialogStackHandler;

	public static Action<string, string> ShowExceptionDialogHandler;

	public static Func<string, string, int> ShowAssertDialogHandler;

	public static ShowCommentsDialogDelegate ShowCommentsDialogHandler;

	public static event Action<string, string, bool> SendReportEvent
	{
		add
		{
			m_SendReportEvent = (Action<string, string, bool>)Delegate.Combine(m_SendReportEvent, value);
		}
		remove
		{
			m_SendReportEvent = (Action<string, string, bool>)Delegate.Remove(m_SendReportEvent, value);
		}
	}

	public static void That(bool assertion, string message)
	{
		if (!assertion)
		{
			ProcessAssert(message, 1);
		}
	}

	public static void Fail(string message)
	{
		ProcessAssert(message, 1);
	}

	public static void Fail(string message, params object[] parameters)
	{
		ProcessAssert(string.Format(message, parameters), 1);
	}

	public static void Equal<T>(T a, T b, string message) where T : IComparable<T>
	{
		if (CompareSafe(a, b) != 0)
		{
			ProcessAssert(message + " (" + ToStringOrNull(a) + " == " + ToStringOrNull(b) + ")", 1);
		}
	}

	public static void NotEqual<T>(T a, T b, string message) where T : IComparable<T>
	{
		if (CompareSafe(a, b) == 0)
		{
			ProcessAssert(message + " (" + ToStringOrNull(a) + " != " + ToStringOrNull(b) + ")", 1);
		}
	}

	public static void Greater<T>(T a, T b, string message) where T : IComparable<T>
	{
		if (CompareSafe(a, b) <= 0)
		{
			ProcessAssert(message + " (" + ToStringOrNull(a) + " > " + ToStringOrNull(b) + ")", 1);
		}
	}

	public static void GreaterEqual<T>(T a, T b, string message) where T : IComparable<T>
	{
		if (CompareSafe(a, b) < 0)
		{
			ProcessAssert(message + " (" + ToStringOrNull(a) + " >= " + ToStringOrNull(b) + ")", 1);
		}
	}

	public static void Less<T>(T a, T b, string message) where T : IComparable<T>
	{
		if (CompareSafe(a, b) >= 0)
		{
			ProcessAssert(message + " (" + ToStringOrNull(a) + " < " + ToStringOrNull(b) + ")", 1);
		}
	}

	public static void LessEqual<T>(T a, T b, string message) where T : IComparable<T>
	{
		if (CompareSafe(a, b) > 0)
		{
			ProcessAssert(message + " (" + ToStringOrNull(a) + " <= " + ToStringOrNull(b) + ")", 1);
		}
	}

	protected static string ToStringOrNull<T>(T a)
	{
		return a == null ? "null" : a.ToString();
	}

	protected static int CompareSafe<T>(T a, T b) where T : IComparable<T>
	{
		if (a == null)
		{
			return b == null ? 0 : -1;
		}
		if (b == null)
		{
			return 1;
		}
		return a.CompareTo(b);
	}

	protected static void ProcessAssert(string message, int additionalSkip = 1)
	{
		StackTrace stack = new StackTrace(additionalSkip + 1, true);
		StackFrame frame = stack.GetFrame(0);
		if (frame == null)
		{
			FCoreUtil.Warning("Assertion failed: " + message);
			return;
		}
		string fileName = frame.GetFileName();
		int line = frame.GetFileLineNumber();
		string key = fileName + ":" + line.ToString();
		if (mIgnoreForever.Contains(key))
		{
			return;
		}
		MethodBase method = frame.GetMethod();
		FCoreUtil.Warning(string.Format("Assertion failed: {0} -- {1}({2}) -- {3}.{4}", message, fileName, line, method?.DeclaringType?.Name, method?.Name));
		string callstack = stack.ToString();
		switch (ShowAssertDialog(message, callstack))
		{
		case 0:
			m_SendReportEvent?.Invoke(message, callstack, false);
			break;
		case 1:
			FCoreUtil.Warning("Ignored assertion '" + method + "'.");
			break;
		case 2:
			FCoreUtil.Warning("Ignoring assertion '" + method + "' forever.");
			mIgnoreForever.Add(key);
			break;
		}
	}

	public static void ShowExceptionDialog(string message, StackTrace callstack)
	{
		ShowExceptionDialogStackHandler?.Invoke(message, callstack);
	}

	public static void ShowExceptionDialog(string message, string callstackString)
	{
		ShowExceptionDialogHandler?.Invoke(message, callstackString);
	}

	protected static int ShowAssertDialog(string message, string callstackString)
	{
		if (ShowAssertDialogHandler == null)
		{
			return 1;
		}
		return ShowAssertDialogHandler(message, callstackString);
	}

	public static bool ShowCommentsDialog(string title, string defaultName, bool canCancel, out string name, out string comments)
	{
		name = "";
		comments = "";
		if (ShowCommentsDialogHandler == null)
		{
			return false;
		}
		return ShowCommentsDialogHandler(title, defaultName, canCancel, out name, out comments);
	}

	public static void SendReport(string message, string callstackString, bool isException)
	{
		m_SendReportEvent?.Invoke(message, callstackString, isException);
	}
}
