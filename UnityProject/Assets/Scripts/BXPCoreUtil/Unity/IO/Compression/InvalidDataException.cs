using System;
using System.Runtime.Serialization;

namespace Unity.IO.Compression
{
	[Serializable]
	public sealed class InvalidDataException : SystemException
	{
		public InvalidDataException()
		{
		}

		public InvalidDataException(string message)
		{
		}

		public InvalidDataException(string message, Exception innerException)
		{
		}

		internal InvalidDataException(SerializationInfo info, StreamingContext context)
		{
		}
	}
}
