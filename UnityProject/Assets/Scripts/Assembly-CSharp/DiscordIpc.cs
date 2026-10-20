using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

// PC port (new): minimal Discord Rich Presence client over the local discord-ipc-N endpoint (no native code, no packages):
// a named pipe on Windows, a Unix domain socket on macOS / Linux.
// All I/O runs on one background thread; the game thread only enqueues activity JSON. Everything is wrapped in try/catch.
public class DiscordIpc
{
	public const int OpHandshake = 0;

	public const int OpFrame = 1;

	public const int OpClose = 2;

	private readonly string mClientId;

	private readonly object mLock = new object();

	private string mPending;

	private bool mHasPending;

	private volatile bool mStop;

	private Thread mThread;

	private Stream mPipe;

	public volatile bool Connected;

	public Action<string> Log;

	public DiscordIpc(string clientId)
	{
		mClientId = clientId;
	}

	// activityJson == null clears the presence.
	public void SetActivity(string activityJson)
	{
		lock (mLock)
		{
			mPending = activityJson;
			mHasPending = true;
			Monitor.Pulse(mLock);
		}
	}

	public void Start()
	{
		if (mThread != null)
		{
			return;
		}
		mThread = new Thread(Run);
		mThread.IsBackground = true;
		mThread.Name = "DiscordIpc";
		mThread.Start();
	}

	// Clears the presence (best effort, waits briefly) and stops the thread.
	public void Shutdown()
	{
		try
		{
			if (Connected)
			{
				SetActivity(null);
				Thread.Sleep(300);
			}
			mStop = true;
			lock (mLock)
			{
				Monitor.Pulse(mLock);
			}
			Stream pipe = mPipe;
			if (pipe != null)
			{
				pipe.Dispose();
			}
		}
		catch (Exception)
		{
		}
	}

	public static byte[] EncodeFrame(int opcode, string json)
	{
		byte[] body = Encoding.UTF8.GetBytes(json);
		byte[] frame = new byte[8 + body.Length];
		BitConverter.GetBytes(opcode).CopyTo(frame, 0);
		BitConverter.GetBytes(body.Length).CopyTo(frame, 4);
		body.CopyTo(frame, 8);
		return frame;
	}

	public static bool ReadFrame(Stream s, out int opcode, out string json)
	{
		opcode = -1;
		json = null;
		byte[] header = new byte[8];
		if (!ReadExact(s, header, 8))
		{
			return false;
		}
		opcode = BitConverter.ToInt32(header, 0);
		int length = BitConverter.ToInt32(header, 4);
		if (length < 0 || length > 1 << 20)
		{
			return false;
		}
		byte[] body = new byte[length];
		if (!ReadExact(s, body, length))
		{
			return false;
		}
		json = Encoding.UTF8.GetString(body);
		return true;
	}

	private static bool ReadExact(Stream s, byte[] buffer, int count)
	{
		int read = 0;
		while (read < count)
		{
			int n = s.Read(buffer, read, count - read);
			if (n <= 0)
			{
				return false;
			}
			read += n;
		}
		return true;
	}

	private void Say(string text)
	{
		try
		{
			if (Log != null)
			{
				Log(text);
			}
		}
		catch (Exception)
		{
		}
	}

	private int mSkipMask;

	private int mConnectedIndex;

	private static readonly bool sUnix = Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX;

	private Stream TryConnect()
	{
		for (int i = 0; i < 10 && !mStop; i++)
		{
			if ((mSkipMask & (1 << i)) != 0)
			{
				continue;
			}
			Stream s = sUnix ? TryConnectUnix(i) : TryConnectPipe(i);
			if (s != null)
			{
				mConnectedIndex = i;
				return s;
			}
		}
		return null;
	}

	private static Stream TryConnectPipe(int i)
	{
		NamedPipeClientStream pipe = null;
		try
		{
			pipe = new NamedPipeClientStream(".", "discord-ipc-" + i.ToString(), PipeDirection.InOut);
			pipe.Connect(150);
			return pipe;
		}
		catch (Exception)
		{
			if (pipe != null)
			{
				try
				{
					pipe.Dispose();
				}
				catch (Exception)
				{
				}
			}
			return null;
		}
	}

	// Same lookup as Discord's own SDKs: $XDG_RUNTIME_DIR, $TMPDIR, $TMP, $TEMP, then /tmp, each also with the
	// Flatpak and Snap sub-folders that sandboxed Discord installs on Linux use.
	private static List<string> UnixSocketPaths(int i)
	{
		string name = "discord-ipc-" + i.ToString();
		List<string> roots = new List<string>();
		foreach (string v in new[] { "XDG_RUNTIME_DIR", "TMPDIR", "TMP", "TEMP" })
		{
			string dir = Environment.GetEnvironmentVariable(v);
			if (!string.IsNullOrEmpty(dir) && !roots.Contains(dir))
			{
				roots.Add(dir);
			}
		}
		if (!roots.Contains("/tmp"))
		{
			roots.Add("/tmp");
		}
		List<string> paths = new List<string>();
		foreach (string root in roots)
		{
			paths.Add(Path.Combine(root, name));
			paths.Add(Path.Combine(Path.Combine(root, "app/com.discordapp.Discord"), name));
			paths.Add(Path.Combine(Path.Combine(root, "snap.discord"), name));
		}
		return paths;
	}

	private static Stream TryConnectUnix(int i)
	{
		foreach (string path in UnixSocketPaths(i))
		{
			// no File.Exists pre-check: whether it reports socket files differs between runtimes; a missing path fails fast
			Socket socket = null;
			try
			{
				socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
				socket.Connect(new UnixEndPoint(path));
				return new NetworkStream(socket, true);
			}
			catch (Exception)
			{
				if (socket != null)
				{
					try
					{
						socket.Close();
					}
					catch (Exception)
					{
					}
				}
			}
		}
		return null;
	}

	// AF_UNIX endpoint, equivalent to Mono.Posix's UnixEndPoint (that assembly is not available to Unity players).
	// SocketAddress layout: 2 bytes address family (written by SocketAddress), then the NUL-terminated path.
	private sealed class UnixEndPoint : EndPoint
	{
		private readonly string mPath;

		public UnixEndPoint(string path)
		{
			mPath = path;
		}

		public override AddressFamily AddressFamily
		{
			get { return AddressFamily.Unix; }
		}

		public override SocketAddress Serialize()
		{
			byte[] bytes = Encoding.UTF8.GetBytes(mPath);
			SocketAddress sa = new SocketAddress(AddressFamily.Unix, 2 + bytes.Length + 1);
			for (int i = 0; i < bytes.Length; i++)
			{
				sa[2 + i] = bytes[i];
			}
			sa[2 + bytes.Length] = 0;
			return sa;
		}

		public override EndPoint Create(SocketAddress socketAddress)
		{
			int n = 0;
			while (2 + n < socketAddress.Size && socketAddress[2 + n] != 0)
			{
				n++;
			}
			byte[] bytes = new byte[n];
			for (int i = 0; i < n; i++)
			{
				bytes[i] = socketAddress[2 + i];
			}
			return new UnixEndPoint(Encoding.UTF8.GetString(bytes));
		}

		public override string ToString()
		{
			return mPath;
		}
	}

	private static void Send(Stream s, int op, string json)
	{
		byte[] frame = EncodeFrame(op, json);
		s.Write(frame, 0, frame.Length);
		s.Flush();
	}

	private void Run()
	{
		int nonce = 0;
		int lastSend = Environment.TickCount - 10000;
		while (!mStop)
		{
			try
			{
				Stream pipe = TryConnect();
				if (pipe == null)
				{
					mSkipMask = 0;
					Thread.Sleep(5000);
					continue;
				}
				mPipe = pipe;
				Send(pipe, OpHandshake, "{\"v\":1,\"client_id\":\"" + mClientId + "\"}");
				int op;
				string reply;
				if (!ReadFrame(pipe, out op, out reply) || op != OpFrame || reply.IndexOf("READY", StringComparison.Ordinal) < 0)
				{
					Say("Discord handshake failed: " + reply);
					pipe.Dispose();
					mSkipMask |= 1 << mConnectedIndex; // another Discord instance / client may own a later pipe
					Thread.Sleep(500);
					continue;
				}
				Connected = true;
				Say("Discord presence connected");
				lock (mLock)
				{
					mHasPending = mPending != null || mHasPending;
				}
				while (!mStop)
				{
					string json;
					bool has;
					lock (mLock)
					{
						if (!mHasPending)
						{
							Monitor.Wait(mLock, 1000);
						}
						has = mHasPending;
						json = mPending;
						mHasPending = false;
					}
					if (!has)
					{
						continue;
					}
					int sinceSend = Environment.TickCount - lastSend;
					if (json != null && sinceSend >= 0 && sinceSend < 4000 && !mStop)
					{
						Thread.Sleep(4000 - sinceSend); // Discord allows 5 updates / 20 s (clearing is never delayed)
					}
					nonce++;
					string activity = json == null ? "null" : json;
					string cmd = "{\"cmd\":\"SET_ACTIVITY\",\"args\":{\"pid\":" + System.Diagnostics.Process.GetCurrentProcess().Id.ToString() + ",\"activity\":" + activity + "},\"nonce\":\"" + nonce.ToString() + "\"}";
					Send(pipe, OpFrame, cmd);
					if (!ReadFrame(pipe, out op, out reply))
					{
						throw new IOException("pipe closed");
					}
					if (reply.IndexOf("\"evt\":\"ERROR\"", StringComparison.Ordinal) >= 0)
					{
						Say("Discord error: " + reply);
					}
					lastSend = Environment.TickCount;
				}
			}
			catch (Exception e)
			{
				if (!mStop)
				{
					Say("Discord presence disconnected: " + e.Message);
				}
			}
			Connected = false;
			try
			{
				if (mPipe != null)
				{
					mPipe.Dispose();
				}
			}
			catch (Exception)
			{
			}
			if (!mStop)
			{
				lock (mLock)
				{
					mHasPending = mPending != null; // resend the latest state after reconnecting
				}
				Thread.Sleep(5000);
			}
		}
	}
}
