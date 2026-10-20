// Standard LZMA SDK (public domain), restored to match the shipped signatures.
namespace SevenZip.CommandLineParser
{
	public class CommandForm
	{
		public string IDString = "";

		public bool PostStringMode;

		public CommandForm(string idString, bool postStringMode)
		{
			IDString = idString;
			PostStringMode = postStringMode;
		}
	}
}
