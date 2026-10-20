using System.Collections.Generic;
using MiniJSON;

public static class MiniJsonExtensions
{
	public static string toJson(this Dictionary<string, object> obj)
	{
		return Json.Serialize(obj);
	}

	public static List<object> listFromJson(this string json)
	{
		return Json.Deserialize(json) as List<object>;
	}

	public static Dictionary<string, object> dictionaryFromJson(this string json)
	{
		return Json.Deserialize(json) as Dictionary<string, object>;
	}
}
