using System;
using System.Xml;

public class FDownloadCacheEntry
{
	public string OriginalURL;

	public string Filename;

	public DateTime LastAccess;

	public DateTime Created;

	public float TTL;

	public string FullFilename => null;

	public bool IsExpired => false;

	public bool IsValid => false;

	public static FDownloadCacheEntry CreateFromXml(XmlElement root)
	{
		return null;
	}

	public bool LoadFromXml(XmlElement root)
	{
		return false;
	}

	public void WriteToXml(XmlElement node)
	{
	}
}
