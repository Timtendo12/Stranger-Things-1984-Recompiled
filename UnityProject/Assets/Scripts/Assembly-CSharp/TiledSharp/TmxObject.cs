using System.Collections.Generic;

namespace TiledSharp
{
	public class TmxObject : ITmxElement
	{
		public int id { get; set; }

		public string name { get; set; }

		public string type { get; set; }

		public int x { get; set; }

		public int y { get; set; }

		public int width { get; set; }

		public int height { get; set; }

		public double? rotation { get; set; }

		public bool? visible { get; set; }

		public bool? ellipse { get; set; }

		public Dictionary<string, object> properties { get; set; }

		public Dictionary<string, object> propertytypes { get; set; }
	}
}
