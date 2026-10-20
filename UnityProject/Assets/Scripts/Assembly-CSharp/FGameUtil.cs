using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FGameUtil @ 0x16fe628..0x1705550, Ghidra vaddr).
public class FGameUtil
{
	public static DateTime sUnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

	public static Coroutine StartCoroutine(IEnumerator routine)
	{
		return FPluginRelay.Instance.StartCoroutine(routine);
	}

	public static void StopCoroutine(Coroutine routine)
	{
		FPluginRelay.Instance.StopCoroutine(routine);
	}

	public static void DelayExecute(float delaySeconds, Action function)
	{
		if (delaySeconds < FMath.Epsilon)
		{
			function();
		}
		else
		{
			StartCoroutine(DoDelayExecute(delaySeconds, function));
		}
	}

	public static IEnumerator DoDelayExecute(float delaySeconds, Action function)
	{
		if (FApp.World == null)
		{
			yield return new WaitForSeconds(delaySeconds);
			function();
		}
		else
		{
			FApp.World.DelayExecute(delaySeconds, function);
		}
	}

	public static void ExecuteAtEndOfFrame(Action function)
	{
		StartCoroutine(DoExecuteAtEndOfFrame(function));
	}

	public static IEnumerator DoExecuteAtEndOfFrame(Action function)
	{
		yield return new WaitForEndOfFrame();
		function();
	}

	public static void ExecuteNextFrame(Action function)
	{
		StartCoroutine(DoExecuteNextFrame(function));
	}

	public static IEnumerator DoExecuteNextFrame(Action function)
	{
		yield return null;
		function();
	}

	public static float GetLayerHeight(Vector3 p, int layerMask, float defaultHeight = 0f)
	{
		RaycastHit hitInfo;
		if (FUtil.RaycastWithoutTriggers(p + Vector3.up * 1000f, Vector3.down, out hitInfo, float.PositiveInfinity, layerMask))
		{
			return hitInfo.point.y;
		}
		return defaultHeight;
	}

	public static void WaitForWWW(WWW www, Action<WWW> onFinished)
	{
		StartCoroutine(DoWaitForWWW(www, onFinished));
	}

	protected static IEnumerator DoWaitForWWW(WWW www, Action<WWW> onFinished)
	{
		while (!www.isDone && string.IsNullOrEmpty(www.error))
		{
			yield return null;
		}
		onFinished(www);
	}

	public static IEnumerator TaskAsCoroutine(Task task)
	{
		while (!task.IsCompleted)
		{
			yield return null;
		}
		if (task.IsFaulted)
		{
			throw task.Exception;
		}
	}

	public static DateTime FromUnixTimestampSeconds(int seconds)
	{
		return sUnixEpoch.AddSeconds(seconds);
	}
}
