using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using HTTP;
using UnityEngine;

public class FDownloadEntry : MonoBehaviour
{
	public delegate bool SuccessHandler(FDownloadEntry downloadEntry);

	public delegate void FailedHandler(FDownloadEntry downloadEntry, string errorMessage);

	[CompilerGenerated]
	private sealed class _003CInternalWait_003Ed__27 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public FDownloadEntry _003C_003E4__this;

		public bool saveToCache;

		private string _003Cerror_003E5__2;

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
		public _003CInternalWait_003Ed__27(int _003C_003E1__state)
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

	protected WWW mDownloader;

	protected Request mHTTPRequest;

	protected string mOriginalURL;

	[CompilerGenerated]
	private SuccessHandler m_Succeeded;

	[CompilerGenerated]
	private FailedHandler m_Failed;

	public string OriginalURL => null;

	public string URL => null;

	public float CacheTTL { get; set; }

	public bool FromCache { get; set; }

	public byte[] Bytes => null;

	public event SuccessHandler Succeeded;

	public event FailedHandler Failed;

	public Texture2D MakeTexture2D()
	{
		return null;
	}

	public static FDownloadEntry Create(string url, string originalURL, bool saveToCache, float cacheTTL, bool fromCache)
	{
		return null;
	}

	[IteratorStateMachine(typeof(_003CInternalWait_003Ed__27))]
	protected IEnumerator InternalWait(bool saveToCache)
	{
		return null;
	}

	public void Cancel()
	{
	}
}
