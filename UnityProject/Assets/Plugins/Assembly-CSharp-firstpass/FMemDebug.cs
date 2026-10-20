using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FMemDebug @ 0x16602cc..0x16618d0, Ghidra vaddr).
// PC port: the shipped code reads android.os.Debug.MemoryInfo.getTotalPss through JNI; on PC the Unity profiler's reserved memory is used.
// Periodic logging worker and the texture/object dumps are debug helpers (minimal versions).
public class FMemDebug
{
	protected static Thread mThread;

	protected static EventWaitHandle mStopHandle;

	[CompilerGenerated]
	private static Action m_PeriodicLogCallback;

	private static uint mMemUsage;

	public static event Action PeriodicLogCallback
	{
		add
		{
			m_PeriodicLogCallback = (Action)Delegate.Combine(m_PeriodicLogCallback, value);
		}
		remove
		{
			m_PeriodicLogCallback = (Action)Delegate.Remove(m_PeriodicLogCallback, value);
		}
	}

	public static void StartPeriodicLogging()
	{
		// PC port: no background JNI thread; nothing to start
	}

	public static void StopPeriodicLogging()
	{
		if (mStopHandle != null)
		{
			mStopHandle.Set();
		}
	}

	public static void MemLogWorker()
	{
		// PC port: Android-only worker (AttachCurrentThread); never started
	}

	public static void LogUsage(string label)
	{
		uint usage = GetMemUsageInternal();
		mMemUsage = usage;
		string text = ((float)usage / 1048576f).ToString("F2");
		if (label == null)
		{
			FLog.UnityLog("Mem usage: " + text);
		}
		else
		{
			FLog.UnityLog("Mem usage (" + label + "): " + text);
		}
	}

	public static uint GetMemUsage()
	{
		return mMemUsage;
	}

	public static float GetMemUsageMB()
	{
		return (float)mMemUsage / 1048576f;
	}

	private static uint GetMemUsageInternal()
	{
		long reserved = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();
		return (uint)Math.Min(reserved, (long)uint.MaxValue);
	}

	public static void DumpTextures()
	{
		FLog.Log("-------------------- TEXTURES --------------------");
		int pixels = 0;
		Texture2D[] textures = (Texture2D[])Resources.FindObjectsOfTypeAll(typeof(Texture2D));
		foreach (Texture2D texture in textures)
		{
			FLog.Log(texture.width + "x" + texture.height + " " + texture.name);
			pixels += texture.width * texture.height;
		}
		FLog.Log("Total pixel count: " + pixels + " (" + ((float)(pixels << 2) / 1048576f) + " MB if all uncompressed/no mips)");
	}

	public static void DumpObjectCounts()
	{
		FLog.Log("-------------------- OBJECT COUNTS --------------------");
		foreach (System.Collections.Generic.KeyValuePair<string, int> pair in CountObjectsByType())
		{
			FLog.Log(pair.Key + ": " + pair.Value);
		}
	}

	private static System.Collections.Generic.SortedDictionary<string, int> CountObjectsByType()
	{
		System.Collections.Generic.SortedDictionary<string, int> counts = new System.Collections.Generic.SortedDictionary<string, int>();
		foreach (UnityEngine.Object obj in Resources.FindObjectsOfTypeAll<UnityEngine.Object>())
		{
			string key = obj.GetType().Name;
			counts.TryGetValue(key, out var count);
			counts[key] = count + 1;
		}
		return counts;
	}
}
