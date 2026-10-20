using System.Xml;

public interface IXmlSerializable
{
	bool LoadXml(XmlElement root, string findParentName);

	XmlElement SerializeXml(XmlElement parent, string rootName);
}
