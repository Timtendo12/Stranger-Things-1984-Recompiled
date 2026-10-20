using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class FDownloadRequestManager : MonoBehaviour
{
	protected class FUnityDownloadEntry
	{
		public UnityWebRequest WebRequest;

		public int InstanceID;

		public Coroutine MonitoringCoroutine;
	}

	[CompilerGenerated]
	private sealed class _003CUnityDownloadCoroutine_003Ed__17 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public FUnityDownloadEntry entry;

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
		public _003CUnityDownloadCoroutine_003Ed__17(int _003C_003E1__state)
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

	protected static Dictionary<int, FDownloadRequest> mPendingDownloadRequests;

	protected static int mNextInstanceID;

	protected static Dictionary<int, FUnityDownloadEntry> mUnityDownloadEntries;

	public static FDownloadRequestManager Instance { get; protected set; }

	public static void Init()
	{
	}

	public static int StartRequest(FDownloadRequest request)
	{
		return 0;
	}

	public static void AbortRequest(FDownloadRequest request)
	{
	}

	public static int GetDownloadedBytes(FDownloadRequest request)
	{
		return 0;
	}

	public void DownloadRequestSuccess(string paramData)
	{
	}

	public void DownloadRequestSuccess(int instanceID)
	{
	}

	public void DownloadRequestError(string paramData)
	{
	}

	public void DownloadRequestError(int instanceID, string errorMsg)
	{
	}

	public static int GetNextInstanceID()
	{
		return 0;
	}

	[IteratorStateMachine(typeof(_003CUnityDownloadCoroutine_003Ed__17))]
	protected static IEnumerator UnityDownloadCoroutine(FUnityDownloadEntry entry)
	{
		return null;
	}

	protected static int BXP_StartDownloadRequest(string url, string destFile)
	{
		return 0;
	}

	protected static int BXP_DownloadRequestGetDownloadedBytes(int instanceID)
	{
		return 0;
	}

	protected static void BXP_DownloadRequestAbort(int instanceID)
	{
	}
}
