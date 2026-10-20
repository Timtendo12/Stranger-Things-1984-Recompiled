using System;
using System.IO;

namespace Unity.IO.Compression
{
	public class GZipStream : Stream
	{
		private DeflateStream deflateStream;

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

		public Stream BaseStream => null;

		public GZipStream(Stream stream, CompressionMode mode)
		{
		}

		public GZipStream(Stream stream, CompressionMode mode, bool leaveOpen)
		{
		}

		private void SetDeflateStreamFileFormatter(CompressionMode mode)
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

		public override IAsyncResult BeginRead(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			return null;
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return 0;
		}

		public override IAsyncResult BeginWrite(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			return null;
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
		}

		public override int Read(byte[] array, int offset, int count)
		{
			return 0;
		}

		public override void Write(byte[] array, int offset, int count)
		{
		}

		protected override void Dispose(bool disposing)
		{
		}
	}
}
