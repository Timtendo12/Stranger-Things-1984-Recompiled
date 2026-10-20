using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HTTP
{
	public class Response
	{
		public int status;

		public string message;

		public float progress;

		public bool chunked;

		public bool zipped;

		public bool cacheable;

		private MemoryStream output;

		private byte[] bytes;

		private Dictionary<string, List<string>> headers;

		public Exception exception;

		public string Text => null;

		public byte[] Bytes
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public AssetBundleCreateRequest Asset => null;

		public void AddHeader(string name, string value)
		{
		}

		public void SetHeader(string name, string value)
		{
		}

		public List<string> GetHeaders(string name)
		{
			return null;
		}

		public List<string> AvailableHeaders()
		{
			return null;
		}

		public string GetHeader(string name)
		{
			return null;
		}

		public Response(Request request)
		{
		}

		public Response(Exception exception)
		{
		}

		private string ReadLine(Stream stream)
		{
			return null;
		}

		private string[] ReadKeyValue(Stream stream)
		{
			return null;
		}

		public void ReadFromStream(Stream inputStream)
		{
		}

		private void WriteOutput(byte[] buffer, int count)
		{
		}
	}
}
