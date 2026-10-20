namespace Unity.IO.Compression
{
	internal class DeflateInput
	{
		internal struct InputState
		{
			internal int count;

			internal int startIndex;
		}

		private byte[] buffer;

		private int count;

		private int startIndex;

		internal byte[] Buffer
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		internal int Count
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		internal int StartIndex
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		internal void ConsumeBytes(int n)
		{
		}

		internal InputState DumpState()
		{
			return default;
		}

		internal void RestoreState(InputState state)
		{
		}
	}
}
