using System.ComponentModel;

// Rewritten from libil2cpp.so 1.0.605 (FEmail, Ghidra vaddr).
// PC port: no platform mail composer and no SMTP credentials; all entry points report failure.
public class FEmail
{
	public static string SenderEmail;

	public static string SmtpUserName;

	public static string SmtpPassword;

	public static bool APISendWithImageAndFiles(string recipient, string bccRecipient, string title, string body, string imageFilename, float jpgCompressionQuality, string[] additionalFiles, string sendMethod = null, string cancelMethod = null, bool allowAsync = true)
	{
		return false;
	}

	private static void SendCompleted(object sender, AsyncCompletedEventArgs e)
	{
	}

	public static bool CanShowComposer()
	{
		return false;
	}

	public static bool ShowComposerWithImageAndFiles(string recipient, string bccRecipient, string title, string body, string imageFilename, float jpgCompressionQuality, string[] additionalFiles, string sendMethod = null, string cancelMethod = null, bool allowAsync = true)
	{
		return false;
	}
}
