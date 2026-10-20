using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FXmlElementExtensionsCoreUtilTypes.GetAttrib @ 0x1895a4c..0x1895e10, Ghidra vaddr).
// Only the Fix64 and FPoint2 overloads are kept; the FixVector2/FixVector3 ones and the Fix64 writers were never called.
public static class FXmlElementExtensionsCoreUtilTypes
{
	public static bool GetAttrib(this XmlElement element, string attribName, ref Fix64 result)
	{
		if (element.GetAttribute(attribName).TryParseFix64(out Fix64 val, Fix64.Zero))
		{
			result = val;
			return true;
		}
		return false;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, Fix64 result, Fix64 defaultResult)
	{
		return element.GetAttribute(attribName).TryParseFix64(out result, defaultResult);
	}

	public static Fix64 GetAttrib(this XmlElement element, string attribName, Fix64 defaultResult)
	{
		element.GetAttribute(attribName).TryParseFix64(out Fix64 val, defaultResult);
		return val;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref FPoint2 result)
	{
		if (element.GetAttribute(attribName).TryParseFPoint2(out FPoint2 val, default))
		{
			result = val;
			return true;
		}
		return false;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref FPoint2 result, FPoint2 defaultResult)
	{
		return element.GetAttribute(attribName).TryParseFPoint2(out result, defaultResult);
	}

	public static FPoint2 GetAttrib(this XmlElement element, string attribName, FPoint2 defaultResult)
	{
		element.GetAttribute(attribName).TryParseFPoint2(out FPoint2 val, defaultResult);
		return val;
	}
}
