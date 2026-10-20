using System.Xml.Linq;

namespace TiledSharp
{
	public class TmxColor
	{
		public int R { get; private set; }

		public int G { get; private set; }

		public int B { get; private set; }

		public TmxColor(XAttribute xColor)
		{
		}
	}
}
