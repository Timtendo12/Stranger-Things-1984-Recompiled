using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FAnalytics @ 0x17e8a08..0x17e931c, Ghidra vaddr).
// Analytics events are only evaluated locally; SendEvent is empty in the shipped build, nothing is uploaded.
public class FAnalytics
{
	protected static bool mInitialized = false;

	protected static List<FAnalyticEvent> mAnalyticEvents = new List<FAnalyticEvent>();

	public static bool Initialized => mInitialized;

	public static void Init()
	{
		if (mInitialized)
		{
			return;
		}
		XmlElement xmlElement = FUtil.LoadXmlAsset("Data/Analytics");
		if (xmlElement == null)
		{
			Debug.LogError("Failed to load/parse Analytics file");
			return;
		}
		foreach (XmlNode childNode in xmlElement.ChildNodes)
		{
			if (childNode.NodeType == XmlNodeType.Element)
			{
				FAnalyticEvent fAnalyticEvent = new FAnalyticEvent();
				if (fAnalyticEvent.LoadXml(childNode as XmlElement))
				{
					mAnalyticEvents.Add(fAnalyticEvent);
					fAnalyticEvent.HasPosted = fAnalyticEvent.CheckRequirements();
				}
			}
		}
		mInitialized = true;
	}

	public static void Sync()
	{
		foreach (FAnalyticEvent mAnalyticEvent in mAnalyticEvents)
		{
			mAnalyticEvent.HasPosted = mAnalyticEvent.CheckRequirements();
		}
	}

	public static void TrySendAnalytics()
	{
		foreach (FAnalyticEvent mAnalyticEvent in mAnalyticEvents)
		{
			if (mAnalyticEvent.CheckRequirements() && !mAnalyticEvent.HasPosted)
			{
				mAnalyticEvent.SendEvent();
				mAnalyticEvent.HasPosted = true;
			}
		}
	}

	public static void SendCustomDesignEvent(string name)
	{
		// Empty in the shipped build.
	}

	public static void Authenticated()
	{
		// Empty in the shipped build.
	}
}
