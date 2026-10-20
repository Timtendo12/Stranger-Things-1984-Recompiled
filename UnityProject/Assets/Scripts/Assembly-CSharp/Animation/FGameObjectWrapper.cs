using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FGameObjectWrapper : IDelayedDispose
{
	[CompilerGenerated]
	private sealed class _003CDoLifespan_003Ed__42 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public float timeInSeconds;

		public FGameObjectWrapper _003C_003E4__this;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return null;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return null;
			}
		}

		[DebuggerHidden]
		public _003CDoLifespan_003Ed__42(int _003C_003E1__state)
		{
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
		}
	}

	protected GameObject mGameObject;

	protected GameObject mBasePrefab;

	protected FAnimDef mAnimDef;

	protected bool mAttachedToWorld;

	protected float mAnimSpeed;

	public FAnimDef AnimDef
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public GameObject GO => null;

	public GameObject BasePrefab => null;

	public bool AttachedToWorld
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public float AnimSpeed => 0f;

	public Vector3 Position
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public FGameObjectWrapper()
	{
	}

	public FGameObjectWrapper(string prefabName)
	{
	}

	public FGameObjectWrapper(GameObject gameObj, bool clone = false, GameObject basePrefab = null, Vector3 pos = default(Vector3))
	{
	}

	~FGameObjectWrapper()
	{
	}

	public void SetGameObject(GameObject gameObj, bool clone = false, Vector3 pos = default(Vector3))
	{
	}

	public void Dispose()
	{
	}

	protected void InternalDispose()
	{
	}

	public virtual void DelayedDispose()
	{
	}

	public float GetAnimLength(string name, bool recursive)
	{
		return 0f;
	}

	public void SetAnimSpeed(float speed, bool recursive)
	{
	}

	public FAnimState PlayAnimType(int animType, int optionalAnimEntryIndex, float animSpeed, int actionID, bool recursive, IAnimTagResponder responder)
	{
		return null;
	}

	public float PlayAnim(string name, bool recursive)
	{
		return 0f;
	}

	public void SetAnimatorBool(string paramName, bool paramValue, bool recursive = true)
	{
	}

	public void SetAnimatorFloat(string paramName, float paramValue, bool recursive = true)
	{
	}

	public void SetAnimatorInt(string paramName, int paramValue, bool recursive = true)
	{
	}

	public void SetPosition(Vector3 p)
	{
	}

	public void SetRotation(Quaternion q)
	{
	}

	public string GetStringDetails()
	{
		return null;
	}

	public int GetCurrentAnimTypeID()
	{
		return 0;
	}

	public int GetCurrentAnimActionID()
	{
		return 0;
	}

	public void SetCurrentAnimActionID(int actionID)
	{
	}

	public void BeginFadeLifespan(float delaySeconds, float fadeSeconds, float lingerSeconds)
	{
	}

	[IteratorStateMachine(typeof(_003CDoLifespan_003Ed__42))]
	protected IEnumerator DoLifespan(float timeInSeconds)
	{
		return null;
	}

	public void SetForward(Vector3 f)
	{
	}

	public void Rotate(float x, float y, float z)
	{
	}

	public void Rotate(Vector3 rotate, float modifier)
	{
	}
}
