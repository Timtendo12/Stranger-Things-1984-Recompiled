// Rewritten from libil2cpp.so 1.0.605 (FKeychain, Ghidra vaddr). PC port: no platform keychain; callers fall back to local files.
public class FKeychain
{
	public static bool Available => false;

	public static void SetString(string key, string val, bool compressed)
	{
	}

	public static string GetString(string key, bool compressed)
	{
		return null;
	}

	public static void SetBytes(string key, byte[] bytes)
	{
	}

	public static byte[] GetBytes(string key)
	{
		return null;
	}
}
