using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FNewsManager @ 0x178fb88..0x1792480, Ghidra vaddr).
// PC port: the news feed is downloaded from the (offline) game server in the shipped build; Init never sets a server file name,
// so the download path is a no-op here and the lists only fill when LoadNewsXML* is called explicitly.
public class FNewsManager
{
	protected static bool mInitialized;

	protected static List<FNewsItem> mNewsItems;

	protected static List<FBuildNotes> mBuildNotes = new List<FBuildNotes>();

	protected static string mServerNewsFileName;

	protected static FDownloadEntry mDownloadEntry;

	protected static float SecondsBetweenDownloads = 3600f;

	public static List<FBuildNotes> BuildNotes => mBuildNotes;

	public static void Init()
	{
		mInitialized = true;
	}

	public static void ServerURLUpdated()
	{
	}

	public static int GetNumNewsItems()
	{
		if (mNewsItems == null)
		{
			return 0;
		}
		return mNewsItems.Count;
	}

	public static FNewsItem GetNewsItem(int itemIndex)
	{
		if (itemIndex >= 0 && mNewsItems != null && itemIndex < mNewsItems.Count)
		{
			return mNewsItems[itemIndex];
		}
		return null;
	}

	public static void GetUnseenNewsItemData(int lastNewsItemIDSeen, out int numUnseenItems, out bool haveUnseenPopupItem)
	{
		numUnseenItems = 0;
		haveUnseenPopupItem = false;
		if (mNewsItems == null)
		{
			return;
		}
		for (int i = 0; i < mNewsItems.Count; i++)
		{
			if (lastNewsItemIDSeen < mNewsItems[i].ID)
			{
				numUnseenItems++;
				if (mNewsItems[i].Popup)
				{
					haveUnseenPopupItem = true;
				}
			}
		}
	}

	public static int GetMaxNewsItemID()
	{
		int num = -1;
		if (mNewsItems == null)
		{
			return -1;
		}
		for (int i = 0; i < mNewsItems.Count; i++)
		{
			if (num < mNewsItems[i].ID)
			{
				num = mNewsItems[i].ID;
			}
		}
		return num;
	}

	protected static void ClearHandlers()
	{
		// TODO(port): the shipped code unhooks the FDownloadEntry Succeeded/Failed handlers; downloads never start on PC [left: no server on PC; BuildNotes_FINAL contains no News items]
		mDownloadEntry = null;
	}

	public static void DidBecomeActive()
	{
		// TODO(port): the shipped code (re)downloads the news xml from mServerNewsFileName; there is no server on PC [left: no server on PC; BuildNotes_FINAL contains no News items]
	}

	public static void WillResignActive()
	{
		ClearHandlers();
	}

	protected static bool DownloadSucceeded(FDownloadEntry entry)
	{
		if (entry != null)
		{
			LoadNewsXMLFromDownloadEntry(entry);
		}
		ClearHandlers();
		return true;
	}

	protected static void DownloadFailed(FDownloadEntry entry, string errorMessage)
	{
		ClearHandlers();
	}

	public static void LoadNewsXMLFromDownloadEntry(FDownloadEntry entry)
	{
		if (entry != null && entry.Bytes != null)
		{
			string xml = System.Text.Encoding.UTF8.GetString(entry.Bytes);
			if (xml != null)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(xml);
				if (xmlDocument.DocumentElement != null)
				{
					LoadNewsXML(xmlDocument.DocumentElement);
				}
			}
		}
	}

	public static void LoadNewsXMLFromFile(string fileName)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(fileName);
		if (xmlDocument.DocumentElement != null)
		{
			LoadNewsXML(xmlDocument.DocumentElement);
		}
	}

	public static void LoadNewsXML(XmlElement root)
	{
		mNewsItems = new List<FNewsItem>();
		if (root == null)
		{
			return;
		}
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			XmlElement xmlElement = childNode as XmlElement;
			if (!ShouldLoad(xmlElement))
			{
				continue;
			}
			if (string.Compare(xmlElement.Name, "News", StringComparison.OrdinalIgnoreCase) == 0)
			{
				foreach (XmlNode childNode2 in xmlElement.ChildNodes)
				{
					if (childNode2.NodeType != XmlNodeType.Element)
					{
						continue;
					}
					XmlElement xmlElement2 = childNode2 as XmlElement;
					if (ShouldLoad(xmlElement2))
					{
						FNewsItem fNewsItem = new FNewsItem();
						fNewsItem.ID = xmlElement2.GetAttrib("ID", -1);
						// TODO(port): title/description are looked up in the loc tables in the shipped code (0x1790c4c); raw attributes used here [left: no server on PC; BuildNotes_FINAL contains no News items]
						fNewsItem.Title = xmlElement2.GetAttrib("Title", string.Empty);
						fNewsItem.Description = xmlElement2.GetAttrib("Description", string.Empty);
						fNewsItem.URL = xmlElement2.GetAttrib("URL", string.Empty);
						fNewsItem.Popup = xmlElement2.GetAttrib("Popup", false);
						mNewsItems.Add(fNewsItem);
					}
				}
			}
			else if (string.Compare(xmlElement.Name, "BuildNotes", StringComparison.OrdinalIgnoreCase) == 0)
			{
				mBuildNotes = new List<FBuildNotes>();
				foreach (XmlNode childNode3 in xmlElement.ChildNodes)
				{
					if (childNode3.NodeType != XmlNodeType.Element)
					{
						continue;
					}
					XmlElement xmlElement3 = childNode3 as XmlElement;
					if (ShouldLoad(xmlElement3))
					{
						FBuildNotes fBuildNotes = new FBuildNotes();
						fBuildNotes.CoreBuildNumber = xmlElement3.GetAttrib("Build", 0);
						fBuildNotes.Text = xmlElement3.GetAttrib("Text", string.Empty);
						fBuildNotes.DateTime = DateTime.MinValue;
						mBuildNotes.Add(fBuildNotes);
					}
				}
			}
		}
	}

	public static bool IsPlatformOk(XmlElement root)
	{
		string attrib = root.GetAttrib("Platform", string.Empty);
		if (string.IsNullOrEmpty(attrib))
		{
			return true;
		}
		// PC build: only entries targeting all platforms or "Windows"/"PC" apply
		return attrib.IndexOf("Windows", StringComparison.OrdinalIgnoreCase) >= 0 || attrib.IndexOf("PC", StringComparison.OrdinalIgnoreCase) >= 0;
	}

	public static bool IsMinVersionOk(XmlElement root)
	{
		int attrib = root.GetAttrib("MinVersion", 0);
		return attrib <= 605;
	}

	public static bool IsMaxVersionOk(XmlElement root)
	{
		int attrib = root.GetAttrib("MaxVersion", int.MaxValue);
		return attrib >= 605;
	}

	public static bool IsLanguageOk(XmlElement root)
	{
		string attrib = root.GetAttrib("Language", string.Empty);
		return string.IsNullOrEmpty(attrib) || attrib.IndexOf(FLoc.UILanguage ?? string.Empty, StringComparison.OrdinalIgnoreCase) >= 0;
	}

	public static bool IsBuildVariantOk(XmlElement root)
	{
		return true;
	}

	public static bool IsBranchOk(XmlElement root)
	{
		return true;
	}

	public static bool ShouldLoad(XmlElement root)
	{
		return root != null && IsPlatformOk(root) && IsMinVersionOk(root) && IsMaxVersionOk(root) && IsLanguageOk(root) && IsBuildVariantOk(root) && IsBranchOk(root);
	}

	public static int GetHighestBuildNumber()
	{
		int num = 0;
		if (mBuildNotes != null)
		{
			for (int i = 0; i < mBuildNotes.Count; i++)
			{
				if (num < mBuildNotes[i].CoreBuildNumber)
				{
					num = mBuildNotes[i].CoreBuildNumber;
				}
			}
		}
		return num;
	}
}
