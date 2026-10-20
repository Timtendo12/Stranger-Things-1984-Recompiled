using System.Collections.Generic;

namespace TiledSharp
{
	public class TmxTileset : ITmxElement
	{
		public int firstgid { get; set; }

		public string name { get; set; }

		public int tilewidth { get; set; }

		public int tileheight { get; set; }

		public int spacing { get; set; }

		public int margin { get; set; }

		public int columns { get; set; }

		public int tilecount { get; set; }

		public string source { get; set; }

		public Dictionary<string, TmxTilesetTile> tiles { get; set; }

		public Dictionary<string, object> properties { get; set; }

		public Dictionary<string, TmxTileProperties> tileproperties { get; set; }

		public Dictionary<string, TmxTilePropertyTypes> tilepropertytypes { get; set; }

		public TmxImage image { get; set; }
	}
}
