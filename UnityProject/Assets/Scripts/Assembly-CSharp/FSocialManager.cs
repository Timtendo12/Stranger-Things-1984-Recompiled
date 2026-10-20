using System;
using System.Collections;
using System.IO;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FSocialManager @ 0x1717a18..0x1718300, Ghidra vaddr).
public static class FSocialManager
{
	public static readonly string cScreenshotPath = Application.persistentDataPath + "/LatestScreenshot.jpg";

	public static readonly string cScreenshotUri = "file://" + cScreenshotPath;

	public static readonly string[] cIncludedHashtags = new string[1] { "#StrangerThings" };

	private static Action<bool> m_mShareComplete;

	private static event Action<bool> mShareComplete
	{
		add
		{
			m_mShareComplete = (Action<bool>)Delegate.Combine(m_mShareComplete, value);
		}
		remove
		{
			m_mShareComplete = (Action<bool>)Delegate.Remove(m_mShareComplete, value);
		}
	}

	public static event Action<bool> ShareCompleteAction
	{
		add
		{
			mShareComplete -= value;
			mShareComplete += value;
		}
		remove
		{
			mShareComplete -= value;
		}
	}

	private static IEnumerator TakeScreenshot()
	{
		yield return FPluginRelay.Instance.StartCoroutine(FPluginRelay.Instance.TakeScreenshotInternal(cScreenshotPath, jpg: true, showUI: true, showPause: true));
	}

	public static void ShareToOS()
	{
		FGameUtil.StartCoroutine(DoShareToOS());
	}

	public static IEnumerator DoShareToOS()
	{
		yield return FGameUtil.StartCoroutine(TakeScreenshot());
		if (!File.Exists(cScreenshotPath))
		{
			FLog.Warning("DoShareToOS: Screenshot does not exist at path: " + cScreenshotPath);
			yield break;
		}
		// NativeShare is a PC no-op (returns false): there is no OS share sheet on Windows
		bool arg = NativeShare.Share(cScreenshotPath, "Stranger Things to Share");
		if (m_mShareComplete != null)
		{
			m_mShareComplete(arg);
		}
	}
}
