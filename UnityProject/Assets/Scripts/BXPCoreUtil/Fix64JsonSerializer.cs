using System;
using Newtonsoft.Json;

// Rewritten from libil2cpp.so 1.0.605 (Fix64JsonSerializer @ 0x1875578..0x1875760, Ghidra vaddr).
public class Fix64JsonSerializer : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		writer.WriteValue(((Fix64)value).RawValue);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		return Fix64.FromRaw((long)reader.Value);
	}

	public override bool CanConvert(Type objectType)
	{
		return typeof(Fix64).IsAssignableFrom(objectType);
	}
}
