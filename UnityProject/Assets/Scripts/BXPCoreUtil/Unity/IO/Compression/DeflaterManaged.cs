using System;

namespace Unity.IO.Compression
{
	internal class DeflaterManaged : IDeflater, IDisposable
	{
		private enum DeflaterState
		{
			NotStarted = 0,
			SlowDownForIncompressible1 = 1,
			SlowDownForIncompressible2 = 2,
			StartingSmallData = 3,
			CompressThenCheck = 4,
			CheckingForIncompressible = 5,
			HandlingSmallData = 6
		}

		private const int MinBlockSize = 256;

		private const int MaxHeaderFooterGoo = 120;

		private const int CleanCopySize = 8072;

		private const double BadCompressionThreshold = 1.0;

		private FastEncoder deflateEncoder;

		private CopyEncoder copyEncoder;

		private DeflateInput input;

		private OutputBuffer output;

		private DeflaterState processingState;

		private DeflateInput inputFromHistory;

		internal DeflaterManaged()
		{
		}

		private bool NeedsInput()
		{
			return false;
		}

		bool IDeflater.NeedsInput()
		{
			return false;
		}

		void IDeflater.SetInput(byte[] inputBuffer, int startIndex, int count)
		{
		}

		int IDeflater.GetDeflateOutput(byte[] outputBuffer)
		{
			return 0;
		}

		bool IDeflater.Finish(byte[] outputBuffer, out int bytesRead)
		{
			bytesRead = default;
			return false;
		}

		void IDisposable.Dispose()
		{
		}

		protected void Dispose(bool disposing)
		{
		}

		private bool UseCompressed(double ratio)
		{
			return false;
		}

		private void FlushInputWindows()
		{
		}

		private void WriteFinal()
		{
		}
	}
}
