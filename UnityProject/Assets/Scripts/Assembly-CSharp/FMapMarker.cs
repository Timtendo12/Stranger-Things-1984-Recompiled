using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FMapMarker @ 0x1770810..0x1770a7c, Ghidra vaddr).
public class FMapMarker
{
	public string Name { get; protected set; } = "";

	public string Icon { get; protected set; } = "";

	public string Map { get; protected set; } = "";

	public bool FlashMapButton { get; protected set; } = true;

	public FPoint2 Position { get; protected set; } = FPoint2.invalid;

	public bool LoadXML(XmlElement root)
	{
		Name = root.GetAttrib("Name", Name);
		Icon = root.GetAttrib("Icon", Icon);
		Map = root.GetAttrib("Map", Map);
		Position = root.GetAttrib("Position", Position);
		FlashMapButton = root.GetAttrib("FlashMapButton", FlashMapButton);
		return true;
	}
}
