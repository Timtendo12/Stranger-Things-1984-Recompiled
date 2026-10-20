using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace HTTP
{
	public class Request
	{
		[CompilerGenerated]
		private sealed class _003C_Wait_003Ed__23 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public Request _003C_003E4__this;

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return null;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return null;
				}
			}

			[DebuggerHidden]
			public _003C_Wait_003Ed__23(int _003C_003E1__state)
			{
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			private bool MoveNext()
			{
				return false;
			}

			bool IEnumerator.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				return this.MoveNext();
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
			}
		}

		public string method;

		public string protocol;

		public byte[] bytes;

		public Uri uri;

		public static byte[] EOL;

		public Response response;

		public bool isDone;

		public int maximumRedirects;

		public bool acceptGzip;

		public bool useCache;

		public Exception exception;

		public RequestState state;

		public Action<Uri> OnRedirect;

		private Dictionary<string, List<string>> headers;

		private static Dictionary<string, string> etags;

		public ActiveConnection upgradedConnection;

		public bool enableCookies;

		public static CookieContainer cookies;

		private bool sent;

		private static List<ActiveConnection> connectionPool;

		public float Progress => 0f;

		public string Text
		{
			set
			{
			}
		}

		public Coroutine Wait()
		{
			return null;
		}

		[IteratorStateMachine(typeof(_003C_Wait_003Ed__23))]
		private IEnumerator _Wait()
		{
			return null;
		}

		public Request(string method, string uri)
		{
		}

		public Request(string method, string uri, bool useCache)
		{
		}

		public Request(string uri, WWWForm form)
		{
		}

		private void LoadEtags()
		{
		}

		private void SaveEtags()
		{
		}

		public Request(string method, string uri, byte[] bytes)
		{
		}

		public void AddHeader(string name, string value)
		{
		}

		public string GetHeader(string name)
		{
			return null;
		}

		public List<string> GetHeaders(string name)
		{
			return null;
		}

		public void SetHeader(string name, string value)
		{
		}

		public void PopHeader(string name)
		{
		}

		private ActiveConnection GetClient(string host, int port, bool useSsl)
		{
			return null;
		}

		public string CalculateHash(string input)
		{
			return null;
		}

		public void Send(Action<Response> responseHandler)
		{
		}

		public void Send(Action<Request> requestHandler)
		{
		}

		public void Send()
		{
		}

		public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return false;
		}

		private void WriteToStream(Stream outputStream)
		{
		}
	}
}
