using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class FMultiDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
{
	[CompilerGenerated]
	private sealed class _003CGetEnumerator_003Ed__11 : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private KeyValuePair<TKey, TValue> _003C_003E2__current;

		public FMultiDictionary<TKey, TValue> _003C_003E4__this;

		private Dictionary<TKey, List<TValue>>.Enumerator _003C_003E7__wrap1;

		private TKey _003Ckey_003E5__3;

		private List<TValue> _003Clist_003E5__4;

		private int _003Ci_003E5__5;

		KeyValuePair<TKey, TValue> IEnumerator<KeyValuePair<TKey, TValue>>.Current
		{
			[DebuggerHidden]
			get
			{
				return default;
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
		public _003CGetEnumerator_003Ed__11(int _003C_003E1__state)
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

	protected Dictionary<TKey, List<TValue>> mDictionary;

	public void Add(TKey key, TValue val)
	{
	}

	public void Remove(TKey key)
	{
	}

	public void Remove(TKey key, TValue val)
	{
	}

	public List<TValue> GetList(TKey key)
	{
		return null;
	}

	public int GetCount(TKey key)
	{
		return 0;
	}

	public TValue Get(TKey key, int index)
	{
		return default;
	}

	public TValue GetFirst(TKey key)
	{
		return default;
	}

	public TValue GetFirst(TKey key, TValue defaultValue)
	{
		return default;
	}

	public TValue GetLast(TKey key)
	{
		return default;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return null;
	}

	[IteratorStateMachine(typeof(FMultiDictionary<, >._003CGetEnumerator_003Ed__11))]
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return null;
	}
}
