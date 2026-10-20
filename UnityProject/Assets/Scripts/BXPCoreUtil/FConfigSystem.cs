using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

// Rewritten from libil2cpp.so 1.0.605 (FConfigSystem @ 0x185fbdc..0x1861454, Set<T> @ 0x19e6410, Ghidra vaddr).
// Static fields/properties of registered classes become config entries settable from "Name=Value" lines.
public class FConfigSystem
{
	protected class FConfigEntry
	{
		public Type ConfigType;

		public Action<object> Setter;

		public Func<object> Getter;
	}

	protected static Dictionary<string, FConfigEntry> mConfigEntries = new Dictionary<string, FConfigEntry>();

	protected static Dictionary<string, string> mNonPropertyStrings = new Dictionary<string, string>();

	private const BindingFlags cConfigBindings = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;

	public static void AddClass(Type type)
	{
		foreach (PropertyInfo pi in type.GetProperties(cConfigBindings))
		{
			PropertyInfo property = pi;
			AddConfigEntry(property.Name, new FConfigEntry
			{
				ConfigType = property.PropertyType,
				Setter = x => property.SetValue(null, x, null),
				Getter = () => property.GetValue(null, null)
			});
		}
		foreach (FieldInfo fi in type.GetFields(cConfigBindings))
		{
			FieldInfo field = fi;
			AddConfigEntry(field.Name, new FConfigEntry
			{
				ConfigType = field.FieldType,
				Setter = x => field.SetValue(null, x),
				Getter = () => field.GetValue(null)
			});
		}
	}

	protected static void AddConfigEntry(string name, FConfigEntry entry)
	{
		if (mConfigEntries.Get(name) != null)
		{
			FCoreUtil.Error("Duplicate config field/property name: " + name);
			return;
		}
		mConfigEntries[name] = entry;
	}

	public static void ProcessFile(string path)
	{
		ProcessString(File.ReadAllText(path));
	}

	public static void ProcessString(string str)
	{
		string[] lines = str.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
		for (int i = 0; i < lines.Length; i++)
		{
			string line = lines[i].Trim();
			if (line.Length == 0 || (line.Length >= 2 && line[0] == '/' && line[1] == '/'))
			{
				continue;
			}
			int equals = line.IndexOf('=');
			if (equals < 0)
			{
				// "Name" sets a bool to true, "-Name" sets it to false.
				char first = line[0];
				if (first == '-')
				{
					line = line.Substring(1).Trim();
				}
				if (line.Length < 1)
				{
					FCoreUtil.Error("empty config variable name on line " + i.ToString());
					continue;
				}
				Set(line, first != '-');
				continue;
			}
			string name = line.Substring(0, equals).Trim();
			if (name.Length < 1)
			{
				FCoreUtil.Error("empty config variable name on line " + i.ToString());
				continue;
			}
			string value = line.Substring(equals + 1).Trim();
			if (value.Length < 1)
			{
				FCoreUtil.Error("missing config data on line " + i.ToString());
				continue;
			}
			if (value[0] == '"')
			{
				value = value.Substring(1, value.Length + (value[value.Length - 1] == '"' ? -2 : -1));
			}
			SetFromString(name, value, true, false, false);
		}
	}

	public static bool Set<T>(string name, T val)
	{
		FConfigEntry entry = mConfigEntries.Get(name);
		if (entry == null)
		{
			return false;
		}
		if (typeof(T) == entry.ConfigType)
		{
			entry.Setter(val);
			return true;
		}
		if (entry.ConfigType == typeof(float) && typeof(T) == typeof(int))
		{
			entry.Setter((float)(int)(object)val);
			return true;
		}
		FCoreUtil.Error("Config for '" + name + "' is the wrong type. " + typeof(T).ToString() + ", should be " + entry.ConfigType.ToString());
		return false;
	}

	public static string GetString(string name)
	{
		FConfigEntry entry = mConfigEntries.Get(name);
		if (entry == null)
		{
			return mNonPropertyStrings.Get(name);
		}
		return entry.Getter()?.ToString();
	}

	public static Type GetType(string name)
	{
		FConfigEntry entry = mConfigEntries.Get(name);
		if (entry != null)
		{
			return entry.ConfigType;
		}
		return mNonPropertyStrings.ContainsKey(name) ? typeof(string) : null;
	}

	public static object Get(string name)
	{
		FConfigEntry entry = mConfigEntries.Get(name);
		if (entry != null)
		{
			return entry.Getter();
		}
		return mNonPropertyStrings.Get(name);
	}

	public static bool SetFromString(string name, string valString, bool allowCustom = true, bool quiet = false, bool emptyStringIsTrueForBool = false)
	{
		FConfigEntry entry = mConfigEntries.Get(name);
		if (entry == null)
		{
			if (allowCustom)
			{
				mNonPropertyStrings[name] = valString;
				return true;
			}
			if (!quiet)
			{
				FCoreUtil.Error("Tried to set config '" + name + "' that does not exist, and allowCustom is false");
			}
			return false;
		}
		Type type = entry.ConfigType;
		if (type == typeof(string))
		{
			return Set(name, valString);
		}
		if (type == typeof(int))
		{
			if (valString.TryParseInt(out int intValue))
			{
				return Set(name, intValue);
			}
		}
		else if (type == typeof(float))
		{
			if (valString.TryParseFloat(out float floatValue))
			{
				return Set(name, floatValue);
			}
		}
		else if (type == typeof(Fix64))
		{
			if (valString.TryParseFix64(out Fix64 fixValue))
			{
				return Set(name, fixValue);
			}
		}
		else if (type == typeof(bool))
		{
			if (emptyStringIsTrueForBool && string.IsNullOrEmpty(valString))
			{
				return Set(name, true);
			}
			if (valString.TryParseBool(out bool boolValue))
			{
				return Set(name, boolValue);
			}
		}
		else
		{
			object converted = TypeDescriptor.GetConverter(type).ConvertFromInvariantString(valString);
			entry.Setter(converted);
			return true;
		}
		if (!quiet)
		{
			FCoreUtil.Error("Could not convert '" + valString + "' to " + type.Name + " for config " + name);
		}
		return false;
	}

	public static void Dump(Action<string> log)
	{
		foreach (KeyValuePair<string, FConfigEntry> kv in mConfigEntries)
		{
			object value = kv.Value.Getter();
			log(kv.Key + " = " + (value == null ? "(null)" : value.ToString()));
		}
		foreach (KeyValuePair<string, string> kv in mNonPropertyStrings)
		{
			log(kv.Key + " = " + kv.Value);
		}
	}
}
