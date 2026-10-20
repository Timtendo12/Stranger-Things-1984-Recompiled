using System.Xml;
using UnityEngine;

public class FPanelHistoryItemData
{
	public static float cDefaultDuration;

	public FPanelHistoryItem.Types Type { get; set; }

	public string Text { get; set; }

	public bool Localized { get; set; }

	public string Sender { get; set; }

	public bool SenderLocalized { get; set; }

	public float Duration { get; set; }

	public Color TextColor { get; set; }

	public bool Indented { get; set; }

	public bool Bold { get; set; }

	public bool WaitForInput { get; set; }

	public string ActivateTrigger { get; set; }

	public string ActivateTag { get; set; }

	public string Pic { get; set; }

	public string SoundSet { get; set; }

	public float MinHeight { get; set; }

	public string PingObject { get; set; }

	public FPanelHistoryItemData()
	{
	}

	public FPanelHistoryItemData(string text, FPanelHistoryItem.Types textType, Color color, bool indent, bool bold)
	{
	}

	public static FPanelHistoryItemData CreateFromXml(XmlElement root)
	{
		return null;
	}
}
