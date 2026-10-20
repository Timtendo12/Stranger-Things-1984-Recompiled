// Standard LZMA SDK (public domain), restored to match the shipped signatures.
using System;

namespace SevenZip
{
	internal class DataErrorException : ApplicationException
	{
		public DataErrorException()
			: base("Data Error")
		{
		}
	}
}
