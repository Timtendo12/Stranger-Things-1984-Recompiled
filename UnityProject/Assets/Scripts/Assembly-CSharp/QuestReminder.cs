using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (QuestReminder @ 0x17c49dc..0x17c4dec, Ghidra vaddr).
public class QuestReminder
{
	protected bool mMapMarkerRevealed;

	public int StartWaitTime { get; protected set; } = -1;

	public int WaitTime { get; protected set; } = -1;

	public string Dialogue { get; protected set; } = string.Empty;

	public string MapMarker { get; protected set; } = string.Empty;

	public bool MapMarkerRevealed
	{
		get
		{
			WaitTime--;
			if (WaitTime < 1)
			{
				mMapMarkerRevealed = true;
				WaitTime = StartWaitTime;
				return true;
			}
			return mMapMarkerRevealed;
		}
	}

	public bool LoadXML(XmlElement root)
	{
		int attrib = root.GetAttrib("WaitTime", WaitTime);
		StartWaitTime = attrib;
		WaitTime = attrib;
		Dialogue = root.GetAttrib("Dialogue", Dialogue);
		MapMarker = root.GetAttrib("MapMarker", MapMarker);
		if (WaitTime < 0)
		{
			return false;
		}
		if (Dialogue.Valid())
		{
			return true;
		}
		return MapMarker.Valid();
	}

	public bool CheckWaitTime()
	{
		WaitTime--;
		bool result = WaitTime < 1;
		if (result)
		{
			mMapMarkerRevealed = true;
			WaitTime = StartWaitTime;
		}
		return result;
	}
}
