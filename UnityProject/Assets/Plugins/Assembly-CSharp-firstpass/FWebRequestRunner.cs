using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FWebRequestRunner : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CWaitForRequest_003Ed__4 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public FWebRequest request;

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
		public _003CWaitForRequest_003Ed__4(int _003C_003E1__state)
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

	protected static FWebRequestRunner mInstance;

	public static FWebRequestRunner Instance => null;

	public static void AddRequest(FWebRequest request)
	{
	}

	[IteratorStateMachine(typeof(_003CWaitForRequest_003Ed__4))]
	protected static IEnumerator WaitForRequest(FWebRequest request)
	{
		return null;
	}
}
