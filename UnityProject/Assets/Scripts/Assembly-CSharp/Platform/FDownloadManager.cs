using System.Collections.Generic;
using System.Xml;

public class FDownloadManager
{
	protected static Dictionary<string, FDownloadCacheEntry> mCacheEntries;

	protected static string mFilename;

	protected static string mCacheDirectory;

	public static string CacheDirectory => null;

	public static void Init()
	{
	}

	public static void ReadCacheDB()
	{
	}

	public static bool LoadCacheDBXml(XmlElement root)
	{
		return false;
	}

	public static void WriteCacheDB()
	{
	}

	public static FDownloadCacheEntry GetCacheEntry(string url)
	{
		return null;
	}

	public static void RemoveCacheEntry(string url)
	{
	}

	public static bool IsInCache(string url)
	{
		return false;
	}

	public static FDownloadEntry DownloadURL(string url, bool saveToCache, bool loadFromCache, float cacheTTL)
	{
		return null;
	}

	public static void SaveToCache(string originalURL, byte[] bytes, float cacheTTL)
	{
	}
}
