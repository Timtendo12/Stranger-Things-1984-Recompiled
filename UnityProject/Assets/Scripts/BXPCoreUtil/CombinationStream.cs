using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public class CombinationStream : Stream
{
	internal class CombinationStreamAsyncResult : IAsyncResult
	{
		private readonly object _asyncState;

		private readonly ManualResetEvent _manualResetEvent;

		public int BytesRead;

		public bool IsCompleted { get; internal set; }

		public WaitHandle AsyncWaitHandle => null;

		public object AsyncState => null;

		public bool CompletedSynchronously { get; internal set; }

		public Exception Exception { get; internal set; }

		public CombinationStreamAsyncResult(object asyncState)
		{
		}

		internal void SetAsyncWaitHandle()
		{
		}
	}

	private readonly IList<Stream> _streams;

	private readonly IList<int> _streamsToDispose;

	private int _currentStreamIndex;

	private Stream _currentStream;

	private long _length;

	private long _postion;

	public IList<Stream> InternalStreams => null;

	public override bool CanRead => false;

	public override bool CanSeek => false;

	public override bool CanWrite => false;

	public override long Length => 0L;

	public override long Position
	{
		get
		{
			return 0L;
		}
		set
		{
		}
	}

	public CombinationStream(IList<Stream> streams)
	{
	}

	public CombinationStream(IList<Stream> streams, IList<int> streamsToDispose)
	{
	}

	public override void Flush()
	{
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return 0L;
	}

	public override void SetLength(long value)
	{
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return 0;
	}

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
	{
		return null;
	}

	public override int EndRead(IAsyncResult asyncResult)
	{
		return 0;
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
	{
		return null;
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
	}

	protected override void Dispose(bool disposing)
	{
	}
}
