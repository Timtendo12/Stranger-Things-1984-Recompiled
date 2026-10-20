using System.IO;

namespace TiledSharp
{
	public class TmxImage
	{
		public string Source { get; private set; }

		public string Format { get; private set; }

		public Stream Data { get; private set; }

		public TmxColor Trans { get; private set; }

		public int Width { get; private set; }

		public int Height { get; private set; }
	}
}
