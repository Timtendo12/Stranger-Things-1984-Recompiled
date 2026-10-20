using System.Collections.Generic;

namespace TiledSharp
{
	public class TmxObjectGroup : ITmxElement
	{
		public string name { get; set; }

		public TmxColor color { get; set; }

		public DrawOrderType draworder { get; set; }

		public double opacity { get; set; }

		public bool visible { get; set; }

		public int offsetx { get; set; }

		public int offsety { get; set; }

		public TmxList<TmxObject> objects { get; set; }

		public Dictionary<string, object> properties { get; set; }

		public Dictionary<string, object> propertytypes { get; set; }
	}
}
