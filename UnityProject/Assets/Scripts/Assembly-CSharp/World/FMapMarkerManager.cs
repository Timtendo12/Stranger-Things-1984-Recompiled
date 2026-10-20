using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FMapMarkerManager @ 0x1770a7c..0x17710c0, Ghidra vaddr).
public class FMapMarkerManager
{
	protected Dictionary<string, FMapMarker> mMapMarkers = new Dictionary<string, FMapMarker>();

	protected string mFilename;

	public Dictionary<string, FMapMarker> MapMarkers => mMapMarkers;

	public static FMapMarkerManager CreateFromFile(string filename)
	{
		FMapMarkerManager fMapMarkerManager = new FMapMarkerManager();
		fMapMarkerManager.LoadXml(filename);
		return fMapMarkerManager;
	}

	public bool LoadXml(string filename)
	{
		mFilename = filename;
		XmlElement xmlElement = FUtil.LoadXmlAsset(filename);
		if (xmlElement == null)
		{
			FLog.UnityError("Failed to parse Resources file.");
		}
		else
		{
			LoadXml(xmlElement);
		}
		return xmlElement != null;
	}

	public bool LoadXml(XmlElement root)
	{
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType == XmlNodeType.Element && (childNode as XmlElement).Name == "MapMarker")
			{
				FMapMarker fMapMarker = new FMapMarker();
				fMapMarker.LoadXML(childNode as XmlElement);
				mMapMarkers[fMapMarker.Name] = fMapMarker;
			}
		}
		return true;
	}

	public FMapMarker GetMapMarker(string name)
	{
		if (!mMapMarkers.ContainsKey(name))
		{
			return null;
		}
		return mMapMarkers[name];
	}
}
