using System.Xml;
using UnityEngine;

public static class FXmlElementExtensionsUnityTypes
{
	public static bool GetAttrib(this XmlElement element, string attribName, ref Vector2 result)
	{
		return false;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref Vector2 result, Vector2 defaultResult)
	{
		return false;
	}

	public static Vector2 GetAttrib(this XmlElement element, string attribName, Vector2 defaultResult)
	{
		return default;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref Vector3 result)
	{
		return false;
	}

	public static bool GetAttrib(this XmlElement element, string attribName, ref Vector3 result, Vector3 defaultResult)
	{
		return false;
	}

	public static Vector3 GetAttrib(this XmlElement element, string attribName, Vector3 defaultResult)
	{
		return default;
	}
}
