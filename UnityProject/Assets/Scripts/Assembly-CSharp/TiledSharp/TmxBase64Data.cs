using System.IO;
using System.Xml.Linq;

namespace TiledSharp
{
	public class TmxBase64Data
	{
		public Stream Data { get; private set; }

		public TmxBase64Data(XElement xData)
		{
		}
	}
}
