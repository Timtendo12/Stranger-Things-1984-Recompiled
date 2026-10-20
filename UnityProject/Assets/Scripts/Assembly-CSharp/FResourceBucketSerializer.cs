using System;
using Newtonsoft.Json;

public class FResourceBucketSerializer : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		return null;
	}

	public override bool CanConvert(Type objectType)
	{
		return false;
	}
}
