using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FFadeLifespanExtensions @ 0x1649020, Ghidra vaddr).
public static class FFadeLifespanExtensions
{
	public static void FadeLifespan(this GameObject obj, float fadeDelay = 2f, float fadeTime = 0.25f, float linger = 0f, bool scaleOut = true, Action prefadeAction = null, Action<GameObject> completionAction = null)
	{
		FFadeLifespan fade = obj.AddComponent<FFadeLifespan>();
		fade.FadeDelay = fadeDelay;
		fade.FadeTime = fadeTime;
		fade.Linger = linger;
		fade.ScaleOut = scaleOut;
		fade.PrefadeAction = prefadeAction;
		fade.CompletionAction = completionAction;
	}
}
