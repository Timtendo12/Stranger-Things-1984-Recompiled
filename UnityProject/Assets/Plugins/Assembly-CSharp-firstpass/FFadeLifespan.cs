using System;
using System.Collections;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FFadeLifespan @ 0x1650f38..0x1651500, Ghidra vaddr).
public class FFadeLifespan : MonoBehaviour
{
	public enum FadeState
	{
		Delay = 0,
		Fade = 1,
		Done = 2
	}

	public float FadeDelay;

	public float FadeTime;

	public float Linger;

	public bool ScaleOut;

	public Action PrefadeAction { get; set; }

	public Action<GameObject> CompletionAction { get; set; }

	private void Start()
	{
		StartCoroutine(Process());
	}

	protected IEnumerator Process()
	{
		yield return new WaitForSeconds(FadeDelay);
		if (PrefadeAction != null)
		{
			PrefadeAction();
		}
		base.gameObject.SetAlphaShaderRecursive();
		base.gameObject.StopParticleSystemsAndTrailsRecursive(FadeTime + Linger);
		base.gameObject.StopLightAnimationsRecursive();
		float timeLeft = FadeTime;
		while (timeLeft > FMath.Epsilon)
		{
			float previous = timeLeft;
			timeLeft = Mathf.Max(0f, timeLeft - Time.deltaTime);
			base.gameObject.SetAlphaRecursive(timeLeft / FadeTime);
			base.gameObject.FadeLightRecursive(timeLeft / previous);
			if (ScaleOut)
			{
				Transform t = base.gameObject.transform;
				// 0x1651024 MoveNext: new scale = scale * timeLeft / previous + Vec3Epsilon (per component, as written)
				t.localScale = t.localScale * (timeLeft / previous) + FUtil.Vec3Epsilon;
			}
			yield return null;
		}
		base.gameObject.SetAlphaRecursive(0f);
		if (ScaleOut)
		{
			base.gameObject.transform.localScale = FUtil.Vec3Epsilon;
		}
		base.gameObject.FadeLightRecursive(0f);
		yield return new WaitForSeconds(Linger);
		if (CompletionAction != null)
		{
			CompletionAction(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
