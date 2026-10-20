using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FExecuteOnOtherExtensions @ 0x16489e8..0x1650e88, Ghidra vaddr).
public static class FExecuteOnOtherExtensions
{
	public static void AddOnDestroyEvent(this GameObject go, Action<GameObject> func)
	{
		go.GetOrCreateComponent<FExecuteOnOther>().AddOnDestroyEvent(func);
	}

	public static void RemoveOnDestroyEvent(this GameObject go, Action<GameObject> func)
	{
		FExecuteOnOther component = go.GetComponent<FExecuteOnOther>();
		if (!(component == null))
		{
			component.RemoveOnDestroyEvent(func);
		}
	}

	public static void AddOnEnableEvent(this GameObject go, Action<GameObject> func)
	{
		go.GetOrCreateComponent<FExecuteOnOther>().AddOnEnableEvent(func);
	}

	public static void RemoveOnEnableEvent(this GameObject go, Action<GameObject> func)
	{
		FExecuteOnOther component = go.GetComponent<FExecuteOnOther>();
		if (!(component == null))
		{
			component.RemoveOnEnableEvent(func);
		}
	}

	public static void AddOnDisableEvent(this GameObject go, Action<GameObject> func)
	{
		go.GetOrCreateComponent<FExecuteOnOther>().AddOnDisableEvent(func);
	}

	public static void RemoveOnDisableEvent(this GameObject go, Action<GameObject> func)
	{
		FExecuteOnOther component = go.GetComponent<FExecuteOnOther>();
		if (!(component == null))
		{
			component.RemoveOnDisableEvent(func);
		}
	}
}
