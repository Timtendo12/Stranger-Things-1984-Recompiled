// Standard LZMA SDK (public domain), restored to match the shipped signatures.
using System;

namespace SevenZip
{
	internal class InvalidParamException : ApplicationException
	{
		public InvalidParamException()
			: base("Invalid Parameter")
		{
		}
	}
}
