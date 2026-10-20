using System.Xml;
using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (FBucketer @ 0x1646440..0x16469e0, Ghidra vaddr).
public class FBucketer
{
	protected static Dictionary<string, FBucketInfo> mBucketInfos = new Dictionary<string, FBucketInfo>();

	public static void Init(string filename)
	{
		XmlElement xmlElement = FUtil.LoadXmlAsset(filename);
		if (xmlElement == null)
		{
			return;
		}
		foreach (XmlNode childNode in xmlElement.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			XmlElement xmlElement2 = childNode as XmlElement;
			if (xmlElement2 != null)
			{
				string attribute = xmlElement2.GetAttribute("Name");
				if (attribute != null)
				{
					FBucketInfo fBucketInfo = new FBucketInfo();
					if (fBucketInfo.LoadXml(xmlElement2))
					{
						mBucketInfos.Add(attribute, fBucketInfo);
					}
				}
			}
		}
	}

	public static string Bucket(string bucketInfoName, float val)
	{
		if (mBucketInfos.TryGetValue(bucketInfoName, out FBucketInfo value))
		{
			return value.Bucket(val);
		}
		return "";
	}
}
