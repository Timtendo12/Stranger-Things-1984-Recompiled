// Standard LZMA SDK (public domain), restored to match the shipped signatures.
using System.Collections;

namespace SevenZip.CommandLineParser
{
	public class SwitchResult
	{
		public bool ThereIs;

		public bool WithMinus;

		public ArrayList PostStrings = new ArrayList();

		public int PostCharIndex;

		public SwitchResult()
		{
			ThereIs = false;
		}
	}
}
