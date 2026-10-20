using System.IO;
using System.Net.Sockets;

namespace HTTP
{
	public class ActiveConnection
	{
		public string host;

		public int port;

		public TcpClient client;

		public Stream stream;

		public bool Connected => false;
	}
}
