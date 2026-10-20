using System.Collections.Generic;

namespace TiledSharp
{
	public class TmxMap
	{
		public int version { get; set; }

		public int width { get; set; }

		public int height { get; set; }

		public int tilewidth { get; set; }

		public int tileheight { get; set; }

		public string backgroundcolor { get; set; }

		public int? nextobjectid { get; set; }

		public string orientation { get; set; }

		public string renderorder { get; set; }

		public TmxList<TmxTileset> tilesets { get; set; }

		public TmxList<TmxLayer> layers { get; set; }

		public Dictionary<string, object> properties { get; set; }

		public Dictionary<string, object> propertytypes { get; set; }
	}
}
