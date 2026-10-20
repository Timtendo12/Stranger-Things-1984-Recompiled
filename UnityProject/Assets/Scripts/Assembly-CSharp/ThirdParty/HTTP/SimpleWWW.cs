using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HTTP
{
	public class SimpleWWW : MonoBehaviour
	{
		[CompilerGenerated]
		private sealed class _003C_Send_003Ed__5 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public Request request;

			public Action<Response> responseDelegate;

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
			public _003C_Send_003Ed__5(int _003C_003E1__state)
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

		[CompilerGenerated]
		private sealed class _003C_Send_003Ed__6 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public Request request;

			public Action<Request> requestDelegate;

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
			public _003C_Send_003Ed__6(int _003C_003E1__state)
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

		private static SimpleWWW _instance;

		private List<Action> onQuit;

		public static SimpleWWW Instance => null;

		public void Send(Request request, Action<Request> requestDelegate)
		{
		}

		public void Send(Request request, Action<Response> responseDelegate)
		{
		}

		[IteratorStateMachine(typeof(_003C_Send_003Ed__5))]
		private IEnumerator _Send(Request request, Action<Response> responseDelegate)
		{
			return null;
		}

		[IteratorStateMachine(typeof(_003C_Send_003Ed__6))]
		private IEnumerator _Send(Request request, Action<Request> requestDelegate)
		{
			return null;
		}

		public void OnQuit(Action fn)
		{
		}

		private void OnApplicationQuit()
		{
		}
	}
}
