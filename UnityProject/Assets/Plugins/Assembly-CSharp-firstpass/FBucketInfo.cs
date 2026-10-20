using UnityEngine;
using System.Collections.Generic;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FBucketInfo @ 0x1645b38..0x16463c0, Ghidra vaddr).
public class FBucketInfo
{
	public List<float> Thresholds = new List<float>();

	public bool LoadXml(XmlElement root)
	{
		foreach (XmlNode childNode in root.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			XmlElement xmlElement = childNode as XmlElement;
			if (xmlElement == null)
			{
				continue;
			}
			if (xmlElement.Name == "Threshold")
			{
				if (float.TryParse(xmlElement.GetAttribute("Value"), out float result))
				{
					Thresholds.Add(result);
				}
				else
				{
					Debug.LogError("invalid bucket threshold value");
				}
			}
			else if (xmlElement.Name == "Thresholds"
				&& float.TryParse(xmlElement.GetAttribute("Start"), out float result2)
				&& float.TryParse(xmlElement.GetAttribute("End"), out float result3)
				&& float.TryParse(xmlElement.GetAttribute("Increment"), out float result4))
			{
				for (float num = result2; num <= result3; num += result4)
				{
					Thresholds.Add(num);
				}
			}
		}
		int count = Thresholds.Count;
		if (count > 0)
		{
			Thresholds.Sort();
		}
		return count > 0;
	}

	public int IndexOfFirstAbove(float val)
	{
		int count = Thresholds.Count;
		int num = count - 1;
		for (int i = 0; i < num; i++)
		{
			if (val < Thresholds[i])
			{
				return i;
			}
		}
		if (Thresholds[num] < val)
		{
			num = count;
		}
		return num;
	}

	public string Bucket(float val)
	{
		int num = IndexOfFirstAbove(val);
		if (num == 0)
		{
			return "<" + Thresholds[0].ToString();
		}
		if (num >= Thresholds.Count)
		{
			return ">" + Thresholds[Thresholds.Count - 1].ToString();
		}
		// TODO(port): separator literal Str__ assumed to be "-" (0x16463b8..) [left: analytics bucket label only; the decompile label is one char, "-" is the natural separator]
		return Thresholds[num - 1].ToString() + "-" + Thresholds[num].ToString();
	}
}
