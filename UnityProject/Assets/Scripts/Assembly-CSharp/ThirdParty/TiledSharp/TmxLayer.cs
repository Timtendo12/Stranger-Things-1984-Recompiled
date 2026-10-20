namespace TiledSharp
{
	public class TmxLayer : ITmxElement
	{
		public string name { get; set; }

		public double opacity { get; set; }

		public bool visible { get; set; }

		public int offsetx { get; set; }

		public int offsety { get; set; }

		public int x { get; set; }

		public int y { get; set; }

		public int height { get; set; }

		public int width { get; set; }

		public string type { get; set; }

		public int[] data { get; set; }

		public TmxObject[] objects { get; set; }
	}
}
