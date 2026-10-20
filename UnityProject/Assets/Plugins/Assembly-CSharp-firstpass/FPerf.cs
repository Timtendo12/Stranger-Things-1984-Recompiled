using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPerf @ 0x1663098..0x1663e94, Ghidra vaddr).
// All entry points are [Conditional("FALSE")] in the shipped build, so callers are compiled out.
public class FPerf
{
	protected static List<FPerfEvent> mEventStack = new List<FPerfEvent>();

	protected static Dictionary<string, FPerfEntry> mEntries = new Dictionary<string, FPerfEntry>();

	protected static MonoBehaviour mHelperBehaviour;

	public static float cMSPerTick = 1000f / (float)Stopwatch.Frequency;

	protected static Stopwatch FrameTimer { get; set; }

	[Conditional("FALSE")]
	public static void Init(MonoBehaviour helperBehaviour)
	{
		mHelperBehaviour = helperBehaviour;
		mHelperBehaviour.StartCoroutine(FrameMonitor());
		FrameTimer = Stopwatch.StartNew();
		FLog.UnityLog("Stopwatch frequency: " + Stopwatch.Frequency.ToString());
	}

	protected static IEnumerator FrameMonitor()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
		}
	}

	[Conditional("FALSE")]
	public static void Begin(string name)
	{
		if (!mEntries.TryGetValue(name, out var value) || value == null)
		{
			value = new FPerfEntry();
			value.Name = name;
			mEntries[name] = value;
		}
		value.InEventRefCount++;
		mEventStack.Add(new FPerfEvent(name));
	}

	[Conditional("FALSE")]
	public static void End(string name)
	{
		int num = mEventStack.Count - 1;
		if (num < 0)
		{
			FLog.UnityError("Mismatched perf begin/end. Ended " + name + " but never started");
			return;
		}
		FPerfEvent fPerfEvent = mEventStack[num];
		if (fPerfEvent.Name != name)
		{
			FLog.UnityError(string.Concat("Mismatched perf begin/end. Ended ", name, " but expected ", fPerfEvent.Name, "."));
			return;
		}
		long elapsedTicks = fPerfEvent.Stop();
		mEventStack.RemoveAt(num);
		if (!mEntries.TryGetValue(name, out var value) || value == null)
		{
			FLog.UnityError("Tried to end event " + name + " but it was never started.");
			return;
		}
		value.AddElapsed(elapsedTicks);
		value.InEventRefCount--;
		if (value.InEventRefCount < 0)
		{
			FLog.UnityError("Mismatched perf end for event " + name);
			value.InEventRefCount = 0;
		}
	}

	[Conditional("FALSE")]
	public static void FrameEnd()
	{
		FrameTimer.Stop();
		long elapsedTicks = FrameTimer.ElapsedTicks;
		FrameTimer = Stopwatch.StartNew();
		foreach (KeyValuePair<string, FPerfEntry> mEntry in mEntries)
		{
			mEntry.Value.FrameEnd(elapsedTicks);
		}
	}

	[Conditional("FALSE")]
	public static void LogAll()
	{
		foreach (KeyValuePair<string, FPerfEntry> mEntry in mEntries)
		{
			mEntry.Value.Log();
			FLog.Log(string.Empty);
		}
	}
}
