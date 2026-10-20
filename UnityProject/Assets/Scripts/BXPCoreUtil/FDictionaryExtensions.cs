using System.Collections.Generic;

public static class FDictionaryExtensions
{
	public static V Get<K, V>(this IDictionary<K, V> dictionary, K key)
	{
		if (dictionary != null && dictionary.TryGetValue(key, out V value))
		{
			return value;
		}
		return default(V);
	}

	public static V Get<K, V>(this IDictionary<K, V> dictionary, K key, V defaultVal)
	{
		if (dictionary != null && dictionary.TryGetValue(key, out V value))
		{
			return value;
		}
		return defaultVal;
	}

	public static V Get<K, V>(this IDictionary<K, object> dictionary, K key, V defaultVal)
	{
		if (dictionary != null && dictionary.TryGetValue(key, out object value) && value is V typed)
		{
			return typed;
		}
		return defaultVal;
	}

	public static void Deconstruct<K, V>(this KeyValuePair<K, V> k, out K t, out V u)
	{
		t = k.Key;
		u = k.Value;
	}
}
