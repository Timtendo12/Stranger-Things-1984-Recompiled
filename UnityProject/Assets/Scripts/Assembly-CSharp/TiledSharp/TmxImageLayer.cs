using System.Collections.Generic;

namespace TiledSharp
{
	public class TmxImageLayer : ITmxElement
	{
		public string name { get; set; }

		public int? width { get; set; }

		public int? height { get; set; }

		public bool visible { get; set; }

		public double opacity { get; set; }

		public double offsetX { get; set; }

		public double offsetY { get; set; }

		public TmxImage image { get; set; }

		public Dictionary<string, object> properties { get; set; }
	}
}
