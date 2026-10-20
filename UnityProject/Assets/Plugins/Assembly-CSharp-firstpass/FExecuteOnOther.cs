using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FExecuteOnOther @ 0x1650760..0x1650cc0, Ghidra vaddr).
public class FExecuteOnOther : MonoBehaviour
{
	protected event Action<GameObject> mOnDestroyEvent;

	protected event Action<GameObject> mOnEnableEvent;

	protected event Action<GameObject> mOnDisableEvent;

	public void AddOnDestroyEvent(Action<GameObject> func)
	{
		mOnDestroyEvent -= func;
		mOnDestroyEvent += func;
	}

	public void RemoveOnDestroyEvent(Action<GameObject> func)
	{
		mOnDestroyEvent -= func;
	}

	public void OnDestroy()
	{
		if (this.mOnDestroyEvent != null)
		{
			this.mOnDestroyEvent(base.gameObject);
		}
	}

	public void AddOnEnableEvent(Action<GameObject> func)
	{
		mOnEnableEvent -= func;
		mOnEnableEvent += func;
	}

	public void RemoveOnEnableEvent(Action<GameObject> func)
	{
		mOnEnableEvent -= func;
	}

	public void OnEnable()
	{
		if (this.mOnEnableEvent != null)
		{
			this.mOnEnableEvent(base.gameObject);
		}
	}

	public void AddOnDisableEvent(Action<GameObject> func)
	{
		mOnDisableEvent -= func;
		mOnDisableEvent += func;
	}

	public void RemoveOnDisableEvent(Action<GameObject> func)
	{
		mOnDisableEvent -= func;
	}

	public void OnDisable()
	{
		if (this.mOnDisableEvent != null)
		{
			this.mOnDisableEvent(base.gameObject);
		}
	}
}
