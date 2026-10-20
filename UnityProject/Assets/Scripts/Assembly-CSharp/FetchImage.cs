using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using HTTP;
using UnityEngine;

public class FetchImage : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CStart_003Ed__1 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public FetchImage _003C_003E4__this;

		private string[] _003Curls_003E5__2;

		private int _003Ci_003E5__3;

		private List<Request> _003Crequests_003E5__4;

		private List<Request>.Enumerator _003C_003E7__wrap4;

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
		public _003CStart_003Ed__1(int _003C_003E1__state)
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

		private void _003C_003Em__Finally1()
		{
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
		}
	}

	private List<string> msgs;

	[IteratorStateMachine(typeof(_003CStart_003Ed__1))]
	private IEnumerator Start()
	{
		return null;
	}

	private void Logger(string condition, string msg, LogType type)
	{
	}

	private void OnGUI()
	{
	}
}
