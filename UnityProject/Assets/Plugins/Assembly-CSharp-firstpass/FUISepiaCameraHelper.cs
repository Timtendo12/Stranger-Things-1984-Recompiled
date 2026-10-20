using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUISepiaCameraHelper @ 0x166d804..0x166d9f0, Ghidra vaddr).
public class FUISepiaCameraHelper : MonoBehaviour
{
	[Range(0f, 1f)]
	public float SepiaFactor;

	public event Action ActionOnDestroy;

	public virtual void Start()
	{
	}

	public virtual void OnPreRender()
	{
		Shader.SetGlobalFloat("_SEPIA_GLOBAL", SepiaFactor);
	}

	public virtual void OnPostRender()
	{
		Shader.SetGlobalFloat("_SEPIA_GLOBAL", 0f);
	}

	public virtual void OnDestroy()
	{
		if (this.ActionOnDestroy != null)
		{
			this.ActionOnDestroy();
		}
	}
}
