using System;
using System.Collections;
using System.IO;

// Rewritten from libil2cpp.so 1.0.605 (PreviewLabs.PlayerPrefs @ 0x1785078..0x17872a0, Ghidra vaddr).
namespace PreviewLabs
{
	public static class PlayerPrefs
	{
		private static Hashtable playerPrefsHashtable;

		private static bool hashTableChanged;

		private static string serializedOutput;

		private static string serializedInput;

		private const string PARAMETERS_SEPERATOR = ";";

		private const string KEY_VALUE_SEPERATOR = ":";

		private static readonly string fileName;

		static PlayerPrefs()
		{
			playerPrefsHashtable = new Hashtable();
			hashTableChanged = false;
			serializedOutput = string.Empty;
			serializedInput = string.Empty;
			fileName = FUtil.PersistentDataPath + "/PlayerPrefs.txt";
			if (File.Exists(fileName))
			{
				StreamReader streamReader = new StreamReader(fileName);
				serializedInput = streamReader.ReadToEnd();
				streamReader.Close();
			}
			Deserialize();
		}

		public static bool HasKey(string key)
		{
			return playerPrefsHashtable.ContainsKey(key);
		}

		public static string GetFilename()
		{
			return FUtil.PersistentDataPath + "/PlayerPrefs.txt";
		}

		private static void SetValue(string key, object value)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			hashTableChanged = true;
		}

		public static void SetString(string key, string value)
		{
			SetValue(key, value);
		}

		public static void SetInt(string key, int value)
		{
			SetValue(key, value);
		}

		public static void SetInt64(string key, long value)
		{
			SetValue(key, value);
		}

		public static void SetDateTime(string key, DateTime dateTime)
		{
			SetInt64(key, dateTime.Ticks);
		}

		public static void SetFloat(string key, float value)
		{
			SetValue(key, value);
		}

		public static void SetBool(string key, bool value)
		{
			SetValue(key, value);
		}

		public static string GetString(string key)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				return null;
			}
			return playerPrefsHashtable[key].ToString();
		}

		public static string GetString(string key, string defaultValue)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
			return playerPrefsHashtable[key].ToString();
		}

		public static int GetInt(string key)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				return 0;
			}
			return (int)playerPrefsHashtable[key];
		}

		public static long GetInt64(string key)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				return 0L;
			}
			return (long)playerPrefsHashtable[key];
		}

		public static DateTime GetDateTime(string key)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				return DateTime.UtcNow;
			}
			return new DateTime((long)playerPrefsHashtable[key]);
		}

		public static int GetInt(string key, int defaultValue)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
			return (int)playerPrefsHashtable[key];
		}

		public static long GetInt64(string key, long defaultValue)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
			return (long)playerPrefsHashtable[key];
		}

		public static DateTime GetDateTime(string key, DateTime defaultValue)
		{
			return new DateTime(GetInt64(key, defaultValue.Ticks));
		}

		public static float GetFloat(string key)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				return 0f;
			}
			return (float)playerPrefsHashtable[key];
		}

		public static float GetFloat(string key, float defaultValue)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
			return (float)playerPrefsHashtable[key];
		}

		public static bool GetBool(string key)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				return false;
			}
			return (bool)playerPrefsHashtable[key];
		}

		public static bool GetBool(string key, bool defaultValue)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
			return (bool)playerPrefsHashtable[key];
		}

		public static void DeleteKey(string key)
		{
			playerPrefsHashtable.Remove(key);
		}

		public static void DeleteAll()
		{
			playerPrefsHashtable.Clear();
			hashTableChanged = true;
		}

		public static void DeleteFile()
		{
			File.Delete(fileName);
		}

		public static void Flush()
		{
			if (!hashTableChanged)
			{
				return;
			}
			Serialize();
			StreamWriter streamWriter = File.CreateText(fileName);
			if (streamWriter == null)
			{
				FLog.UnityWarning("PlayerPrefs::Flush() opening file for writing failed: " + fileName);
			}
			else
			{
				streamWriter.WriteLine(serializedOutput);
				streamWriter.Close();
				serializedOutput = string.Empty;
			}
		}

		private static void Serialize()
		{
			IDictionaryEnumerator enumerator = playerPrefsHashtable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (serializedOutput != string.Empty)
				{
					serializedOutput += " ; ";
				}
				serializedOutput = string.Concat(serializedOutput, EscapeNonSeperators(enumerator.Key.ToString()), " : ", EscapeNonSeperators(enumerator.Value.ToString()), " : ", enumerator.Value.GetType().ToString());
			}
		}

		private static void Deserialize()
		{
			if (serializedInput == null || serializedInput.Length <= 0)
			{
				return;
			}
			string[] array = serializedInput.Split(new string[1] { " ; " }, StringSplitOptions.None);
			foreach (string text in array)
			{
				string[] array2 = text.Split(new string[1] { " : " }, StringSplitOptions.None);
				if (array2.Length == 3)
				{
					playerPrefsHashtable.Add(DeEscapeNonSeperators(array2[0]), GetTypeValue(array2[2], DeEscapeNonSeperators(array2[1])));
				}
				else
				{
					FLog.UnityWarning("PlayerPrefs::Deserialize() parameterContent has " + array2.Length.ToString() + " elements");
				}
			}
		}

		private static string EscapeNonSeperators(string inputToEscape)
		{
			if (inputToEscape == null)
			{
				return null;
			}
			inputToEscape = inputToEscape.Replace(":", "\\:");
			return inputToEscape.Replace(";", "\\;");
		}

		private static string DeEscapeNonSeperators(string inputToDeEscape)
		{
			if (inputToDeEscape == null)
			{
				return null;
			}
			inputToDeEscape = inputToDeEscape.Replace("\\:", ":");
			return inputToDeEscape.Replace("\\;", ";");
		}

		public static object GetTypeValue(string typeName, string value)
		{
			switch (typeName)
			{
			case "System.String":
				return value;
			case "System.Int32":
				return Convert.ToInt32(value);
			case "System.Int64":
				return Convert.ToInt64(value);
			case "System.Boolean":
				return Convert.ToBoolean(value);
			case "System.Single":
				return Convert.ToSingle(value);
			default:
				FLog.UnityError("Unsupported type: " + typeName);
				return null;
			}
		}
	}
}
