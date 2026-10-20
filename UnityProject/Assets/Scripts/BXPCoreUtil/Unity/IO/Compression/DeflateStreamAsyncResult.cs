using System;
using System.Threading;

namespace Unity.IO.Compression
{
	internal class DeflateStreamAsyncResult : IAsyncResult
	{
		public byte[] buffer;

		public int offset;

		public int count;

		public bool isWrite;

		private object m_AsyncObject;

		private object m_AsyncState;

		private AsyncCallback m_AsyncCallback;

		private object m_Result;

		internal bool m_CompletedSynchronously;

		private int m_InvokedCallback;

		private int m_Completed;

		private object m_Event;

		public object AsyncState => null;

		public WaitHandle AsyncWaitHandle => null;

		public bool CompletedSynchronously => false;

		public bool IsCompleted => false;

		internal object Result => null;

		public DeflateStreamAsyncResult(object asyncObject, object asyncState, AsyncCallback asyncCallback, byte[] buffer, int offset, int count)
		{
		}

		internal void Close()
		{
		}

		internal void InvokeCallback(bool completedSynchronously, object result)
		{
		}

		internal void InvokeCallback(object result)
		{
		}

		private void Complete(bool completedSynchronously, object result)
		{
		}

		private void Complete(object result)
		{
		}
	}
}
