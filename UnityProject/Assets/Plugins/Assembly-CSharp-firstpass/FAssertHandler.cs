using System;
using System.Diagnostics;

// Rewritten from libil2cpp.so 1.0.605 (FAssertHandler @ 0x160bde8..0x160c110, Ghidra vaddr).
public class FAssertHandler
{
	public static bool SuppressDialog;

	public static void Init()
	{
		FAssert.ShowExceptionDialogStackHandler = ShowExceptionDialog;
		FAssert.ShowExceptionDialogHandler = ShowExceptionDialog;
		FAssert.ShowAssertDialogHandler = ShowAssertDialog;
		FAssert.ShowCommentsDialogHandler = ShowCommentsDialog;
	}

	public static void ShowExceptionDialog(string message, StackTrace callstack)
	{
		callstack.ToString();
		// The dialog itself is compiled out in release builds; only the one-shot suppress flag is set.
		if (!SuppressDialog)
		{
			SuppressDialog = true;
		}
	}

	public static void ShowExceptionDialog(string message, string callstackString)
	{
		if (!SuppressDialog)
		{
			SuppressDialog = true;
		}
	}

	public static int ShowAssertDialog(string message, string callstackString)
	{
		return 0;
	}

	public static bool ShowCommentsDialog(string title, string defaultName, bool canCancel, out string name, out string comments)
	{
		name = string.Empty;
		comments = string.Empty;
		comments = string.Empty;
		name = defaultName;
		return false;
	}
}
