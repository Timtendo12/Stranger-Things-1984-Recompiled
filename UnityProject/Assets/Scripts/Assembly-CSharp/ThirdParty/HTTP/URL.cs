using System.Text;

// Rewritten from libil2cpp.so 1.0.605 (HTTP.URL @ 0x177ec60..0x177ed10, Ghidra vaddr).
namespace HTTP
{
	public class URL
	{
		private static string safeChars = "-_.~abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

		public static string Encode(string value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				if (safeChars.IndexOf(c) == -1)
				{
					stringBuilder.Append("%" + string.Format("{0:X2}", (int)c));
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
