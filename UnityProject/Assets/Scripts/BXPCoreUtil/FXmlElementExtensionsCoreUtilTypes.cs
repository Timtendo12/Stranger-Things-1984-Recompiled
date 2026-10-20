using System.Collections.Generic;
using System.Xml;

public static class FXmlElementExtensionsCoreUtilTypes
{
	public static bool GetAttrib(this XmlElement element, string attribName, ref Fix64 result)
	{
		return false;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, Fix64 result, Fix64 defaultResult)
	{
		return false;
	}

	public static Fix64 GetAttrib(this XmlElement element, string attribName, Fix64 defaultResult)
	{
		return default;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref FixVector3 result)
	{
		return false;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref FixVector3 result, FixVector3 defaultResult)
	{
		return false;
	}

	public static FixVector3 GetAttrib(this XmlElement element, string attribName, FixVector3 defaultResult)
	{
		return default;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref FixVector2 result)
	{
		return false;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref FixVector2 result, FixVector2 defaultResult)
	{
		return false;
	}

	public static FixVector2 GetAttrib(this XmlElement element, string attribName, FixVector2 defaultResult)
	{
		return default;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref FPoint2 result)
	{
		return false;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref FPoint2 result, FPoint2 defaultResult)
	{
		return false;
	}

	public static FPoint2 GetAttrib(this XmlElement element, string attribName, FPoint2 defaultResult)
	{
		return default;
	}

	public static XmlElement AddChild(this XmlElement element, string childName, Fix64 childValue, params object[] attributePairs)
	{
		return null;
	}

	public static XmlElement AddChildDict(this XmlElement element, string childName, Dictionary<string, Fix64> dict, string keyName = "Key", string valueName = "Value", string entryName = "Entry")
	{
		return null;
	}

	public static XmlAttribute AddAttribute(this XmlElement element, string attribName, Fix64 attribValue)
	{
		return null;
	}
}
