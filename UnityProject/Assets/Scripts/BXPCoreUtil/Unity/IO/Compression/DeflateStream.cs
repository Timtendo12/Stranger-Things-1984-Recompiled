using System;
using System.IO;

namespace Unity.IO.Compression
{
	public class DeflateStream : Stream
	{
		internal delegate void AsyncWriteDelegate(byte[] array, int offset, int count, bool isAsync);

		private enum WorkerType : byte
		{
			Managed = 0,
			Unknown = 1
		}

		internal const int DefaultBufferSize = 8192;

		private Stream _stream;

		private CompressionMode _mode;

		private bool _leaveOpen;

		private Inflater inflater;

		private IDeflater deflater;

		private byte[] buffer;

		private int asyncOperations;

		private readonly AsyncCallback m_CallBack;

		private readonly AsyncWriteDelegate m_AsyncWriterDelegate;

		private IFileFormatWriter formatWriter;

		private bool wroteHeader;

		private bool wroteBytes;

		public Stream BaseStream => null;

		public override bool CanRead => false;

		public override bool CanWrite => false;

		public override bool CanSeek => false;

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

		public DeflateStream(Stream stream, CompressionMode mode)
		{
		}

		public DeflateStream(Stream stream, CompressionMode mode, bool leaveOpen)
		{
		}

		private static IDeflater CreateDeflater()
		{
			return null;
		}

		private static WorkerType GetDeflaterType()
		{
			return WorkerType.Managed;
		}

		internal void SetFileFormatReader(IFileFormatReader reader)
		{
		}

		internal void SetFileFormatWriter(IFileFormatWriter writer)
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

		public override int Read(byte[] array, int offset, int count)
		{
			return 0;
		}

		private void ValidateParameters(byte[] array, int offset, int count)
		{
		}

		private void EnsureNotDisposed()
		{
		}

		private void EnsureDecompressionMode()
		{
		}

		private void EnsureCompressionMode()
		{
		}

		public override IAsyncResult BeginRead(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			return null;
		}

		private void ReadCallback(IAsyncResult baseStreamResult)
		{
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return 0;
		}

		public override void Write(byte[] array, int offset, int count)
		{
		}

		internal void InternalWrite(byte[] array, int offset, int count, bool isAsync)
		{
		}

		private void WriteDeflaterOutput(bool isAsync)
		{
		}

		private void DoWrite(byte[] array, int offset, int count, bool isAsync)
		{
		}

		private void DoMaintenance(byte[] array, int offset, int count)
		{
		}

		private void PurgeBuffers(bool disposing)
		{
		}

		protected override void Dispose(bool disposing)
		{
		}

		public override IAsyncResult BeginWrite(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			return null;
		}

		private void WriteCallback(IAsyncResult asyncResult)
		{
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
		}

		private void CheckEndXxxxLegalStateAndParams(IAsyncResult asyncResult)
		{
		}

		private void AwaitAsyncResultCompletion(DeflateStreamAsyncResult asyncResult)
		{
		}
	}
}
