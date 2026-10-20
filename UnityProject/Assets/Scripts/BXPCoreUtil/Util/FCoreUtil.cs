using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

// Rewritten from libil2cpp.so 1.0.605 (FCoreUtil @ 0x185b5f8..0x186a3dc, generics @ 0x19e7a04.., Ghidra vaddr).
public static class FCoreUtil
{
	public static UTF8Encoding StringEncodingUTF8 = new UTF8Encoding();

	public static SHA1 SHA1Provider = new SHA1CryptoServiceProvider();

	public static SHA256 SHA256Provider = new SHA256CryptoServiceProvider();

	public static MD5 MD5Provider = new MD5CryptoServiceProvider();

	public static RNGCryptoServiceProvider CryptoRNG = new RNGCryptoServiceProvider();

	public static Action<string> LogHandler;

	public static Action<string> LogWarningHandler;

	public static Action<string> LogErrorHandler;

	public static Action<string> LogFileHandler;

	public static Action<string> LogFileWarningHandler;

	public static Action<string> LogFileErrorHandler;

	public const int cSecondsPerMinute = 60;

	public const int cSecondsPerHour = 3600;

	public const int cSecondsPerDay = 86400;

	public static Regex Emoji = new Regex("\\p{Cs}");

	public static void Log(string msg)
	{
		LogHandler?.Invoke(msg);
	}

	public static void Log(string format, params object[] args)
	{
		Log(string.Format(format, args));
	}

	public static void Warning(string msg)
	{
		LogWarningHandler?.Invoke(msg);
	}

	public static void Warning(string format, params object[] args)
	{
		Warning(string.Format(format, args));
	}

	public static void Error(string msg)
	{
		LogErrorHandler?.Invoke(msg);
	}

	public static void Error(string format, params object[] args)
	{
		Error(string.Format(format, args));
	}

	public static void LogFile(string msg)
	{
		LogFileHandler?.Invoke(msg);
	}

	public static void LogFile(string format, params object[] args)
	{
		LogFile(string.Format(format, args));
	}

	public static void WarningFile(string msg)
	{
		LogFileWarningHandler?.Invoke(msg);
	}

	public static void WarningFile(string format, params object[] args)
	{
		WarningFile(string.Format(format, args));
	}

	public static void ErrorFile(string msg)
	{
		LogFileErrorHandler?.Invoke(msg);
	}

	public static void ErrorFile(string format, params object[] args)
	{
		ErrorFile(string.Format(format, args));
	}

	public static void Swap<T>(ref T lhs, ref T rhs)
	{
		T temp = lhs;
		lhs = rhs;
		rhs = temp;
	}

	public static bool Valid(this string s)
	{
		return !string.IsNullOrEmpty(s);
	}

	public static byte[] MD5Hash(Stream stream)
	{
		return MD5Provider.ComputeHash(stream);
	}

	public static byte[] SHA1Hash(string str)
	{
		return SHA1Provider.ComputeHash(StringEncodingUTF8.GetBytes(str));
	}

	public static byte[] SHA1Hash(byte[] bytes)
	{
		return SHA1Provider.ComputeHash(bytes);
	}

	public static byte[] SHA1Hash(Stream stream)
	{
		return SHA1Provider.ComputeHash(stream);
	}

	public static byte[] SHA1HashFile(string filename)
	{
		using (FileStream stream = File.OpenRead(filename))
		{
			return SHA1Hash(stream);
		}
	}

	public static string SHA1HashToString(string str)
	{
		return SHA1HashToString(StringEncodingUTF8.GetBytes(str));
	}

	public static string SHA1HashToString(byte[] bytes)
	{
		return HexString(SHA1Hash(bytes));
	}

	public static string HexString(byte[] byteArray, int offset = -1, int length = -1)
	{
		if (byteArray == null)
		{
			return null;
		}
		if (offset < 0)
		{
			offset = 0;
		}
		if (length < 0)
		{
			length = byteArray.Length - offset;
		}
		StringBuilder sb = new StringBuilder();
		for (int i = offset; i < length; i++)
		{
			sb.Append(Convert.ToString(byteArray[i], 16).PadLeft(2, '0'));
		}
		return sb.ToString();
	}

	public static byte[] BytesFromHexString(string hexString)
	{
		if (hexString == null || (hexString.Length & 1) != 0)
		{
			Error("Invalid hex string, must be a multiple of 2 in length");
			return null;
		}
		byte[] bytes = new byte[hexString.Length >> 1];
		for (int i = 0; i < bytes.Length; i++)
		{
			int hi = HexNybbleForChar(hexString[i * 2]);
			if (hi < 0)
			{
				return null;
			}
			int lo = HexNybbleForChar(hexString[i * 2 + 1]);
			if (lo < 0)
			{
				return null;
			}
			bytes[i] = (byte)(hi * 16 + lo);
		}
		return bytes;
	}

	public static int HexNybbleForChar(char hexChar)
	{
		if (hexChar >= '0' && hexChar <= '9')
		{
			return hexChar - '0';
		}
		if (hexChar >= 'A' && hexChar <= 'F')
		{
			return hexChar - 'A' + 10;
		}
		if (hexChar >= 'a' && hexChar <= 'f')
		{
			return hexChar - 'a' + 10;
		}
		return -1;
	}

	public static bool CompareByteArrays(byte[] a, byte[] b)
	{
		if (a == null || b == null)
		{
			return a == b;
		}
		return a.SequenceEqual(b);
	}

	public static string MarshalIntPtrToUTF8(IntPtr stringPtr)
	{
		if (stringPtr == IntPtr.Zero)
		{
			return null;
		}
		int length = 0;
		while (Marshal.ReadByte(stringPtr, length) != 0)
		{
			length++;
		}
		byte[] bytes = new byte[length];
		Marshal.Copy(stringPtr, bytes, 0, length);
		return Encoding.UTF8.GetString(bytes);
	}

	public static string DecodeBase64Utf8(this string str)
	{
		return Encoding.UTF8.GetString(Convert.FromBase64String(str));
	}

	public static string EncodeBase64(this byte[] bytes)
	{
		return Convert.ToBase64String(bytes);
	}

	public static XmlElement LoadXmlString(string xmlString)
	{
		return LoadXmlString(xmlString, out byte[] _, false);
	}

	public static XmlElement LoadXmlString(string xmlString, out byte[] hash)
	{
		return LoadXmlString(xmlString, out hash, true);
	}

	public static XmlElement LoadXmlString(string xmlString, out byte[] hash, bool computeHash)
	{
		hash = null;
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(xmlString);
		if (doc.DocumentElement == null)
		{
			Error("Failed to load/parse xml string");
			return null;
		}
		if (computeHash)
		{
			hash = SHA1Hash(xmlString);
		}
		return doc.DocumentElement;
	}

	public static string TimeIntervalToString(float timeInSeconds)
	{
		string result = "";
		int days = FMath.FloorToInt(timeInSeconds / cSecondsPerDay);
		if (days > 1)
		{
			result = result + days.ToString() + " days ";
		}
		else if (days == 1)
		{
			result = result + days.ToString() + " day ";
		}
		timeInSeconds -= days * cSecondsPerDay;
		int hours = FMath.FloorToInt(timeInSeconds / 3600f);
		if (hours > 1)
		{
			result = result + hours.ToString() + " hours ";
		}
		else if (hours == 1)
		{
			result = result + hours.ToString() + " hour ";
		}
		timeInSeconds -= hours * cSecondsPerHour;
		int minutes = FMath.FloorToInt(timeInSeconds / 60f);
		if (minutes > 1)
		{
			result = result + minutes.ToString() + " minutes ";
		}
		else if (minutes == 1)
		{
			result = result + minutes.ToString() + " minute ";
		}
		int seconds = FMath.RoundToInt(timeInSeconds - minutes * cSecondsPerMinute);
		if (seconds > 1)
		{
			result = result + seconds.ToString() + " seconds";
		}
		else if (seconds == 1)
		{
			result = result + seconds.ToString() + " second";
		}
		return result.TrimEnd(' ');
	}

	// TODO(port): verify separators against the original (0x1866728); structure matches the decompilation. [left: TimeIntervalToShortString has no callers]
	public static string TimeIntervalToShortString(float timeInSeconds, int numIntervals, bool showSubSecondInterval = false, int dayLength = 1, int hourLength = 1, int minuteLength = 1, int secondLength = 1, int subSecondLength = 1)
	{
		string result = "";
		int intervals = 0;
		int days = FMath.FloorToInt(timeInSeconds / cSecondsPerDay);
		if (days > 0)
		{
			result = result + days.ToString("D" + dayLength.ToString()) + "d ";
			intervals++;
		}
		if (intervals < numIntervals)
		{
			float remaining = timeInSeconds - days * cSecondsPerDay;
			int hours = FMath.FloorToInt(remaining / 3600f);
			if (hours > 0 || intervals > 0)
			{
				result = result + hours.ToString("D" + hourLength.ToString()) + "h";
				intervals++;
			}
			if (intervals < numIntervals)
			{
				remaining -= hours * cSecondsPerHour;
				int minutes = FMath.FloorToInt(remaining / 60f);
				if (minutes > 0 || intervals > 0)
				{
					if (intervals > 0)
					{
						result += " ";
					}
					result = result + minutes.ToString("D" + minuteLength.ToString()) + "m";
					intervals++;
				}
				if (intervals < numIntervals)
				{
					remaining -= minutes * cSecondsPerMinute;
					int seconds = FMath.FloorToInt(remaining);
					if (intervals > 0)
					{
						result += " ";
					}
					if (days < 1 && timeInSeconds < 1f && showSubSecondInterval && remaining > float.Epsilon)
					{
						int tenths = FMath.FloorToInt(remaining * 10f);
						result = result + "0." + tenths.ToString("D" + subSecondLength.ToString()) + "s";
					}
					else
					{
						result = result + seconds.ToString("D" + secondLength.ToString()) + "s";
					}
				}
			}
		}
		return result.TrimEnd(' ');
	}

	public static void Execute(string command, string arguments, bool waitForExit = true)
	{
		ProcessStartInfo info = new ProcessStartInfo(command, arguments)
		{
			UseShellExecute = false,
			CreateNoWindow = true
		};
		Process process = Process.Start(info);
		if (waitForExit)
		{
			process.WaitForExit();
		}
	}

	public static int ExecuteWithOutput(string command, string arguments, out string output, out string errorOutput)
	{
		ProcessStartInfo info = new ProcessStartInfo(command, arguments)
		{
			UseShellExecute = false,
			CreateNoWindow = true,
			RedirectStandardOutput = true,
			RedirectStandardError = true
		};
		using (Process process = Process.Start(info))
		{
			output = process.StandardOutput.ReadToEnd();
			errorOutput = process.StandardError.ReadToEnd();
			process.WaitForExit();
			return process.ExitCode;
		}
	}

	public static int ExecuteWithWindow(string command, string arguments)
	{
		using (Process process = Process.Start(new ProcessStartInfo(command, arguments) { UseShellExecute = true }))
		{
			process.WaitForExit();
			return process.ExitCode;
		}
	}

	public static int ExecuteWithCapture(string command, string arguments, Action<string> outputFunc, Action<string> errorFunc)
	{
		return ExecuteWithCapture(command, arguments, null, outputFunc, errorFunc);
	}

	public static int ExecuteWithCapture(string command, string arguments, string extraLogToCapture, Action<string> outputFunc, Action<string> errorFunc)
	{
		ProcessStartInfo info = new ProcessStartInfo(command, arguments)
		{
			UseShellExecute = false,
			CreateNoWindow = true,
			RedirectStandardOutput = true,
			RedirectStandardError = true
		};
		using (Process process = new Process { StartInfo = info })
		{
			process.OutputDataReceived += (sender, e) => { if (e.Data != null) outputFunc?.Invoke(e.Data); };
			process.ErrorDataReceived += (sender, e) => { if (e.Data != null) errorFunc?.Invoke(e.Data); };
			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			process.WaitForExit();
			if (extraLogToCapture != null && File.Exists(extraLogToCapture))
			{
				foreach (string line in File.ReadAllLines(extraLogToCapture))
				{
					outputFunc?.Invoke(line);
				}
			}
			return process.ExitCode;
		}
	}

	public static T[] Concat<T>(this T[] x, T[] y)
	{
		T[] result = new T[x.Length + y.Length];
		x.CopyTo(result, 0);
		y.CopyTo(result, x.Length);
		return result;
	}

	public static string TrimPrefix(string sourceString, string prefix)
	{
		int index = sourceString.IndexOf(prefix) + prefix.Length;
		if (index >= 0 && index < sourceString.Length)
		{
			return sourceString.Substring(index);
		}
		return sourceString;
	}

	public static T First<T>(this IEnumerable<T> e)
	{
		using (IEnumerator<T> enumerator = e.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		throw new InvalidOperationException();
	}

	public static string SafeToString(this object obj)
	{
		return obj != null ? obj.ToString() : string.Empty;
	}

	public static bool AnyNull(params object[] objects)
	{
		return objects.Any(o => o == null);
	}

	public static bool AllNull(params object[] objects)
	{
		return objects.All(o => o == null);
	}

	public static bool AllNonNull(params object[] objects)
	{
		return objects.All(o => o != null);
	}

	public static bool AnyNonNull(params object[] objects)
	{
		return objects.Any(o => o != null);
	}

	public static bool LengthsMatch(params Array[] arrays)
	{
		for (int i = 1; i < arrays.Length; i++)
		{
			if (arrays[i].Length != arrays[0].Length)
			{
				return false;
			}
		}
		return true;
	}

	public static bool AllEqual(params float[] values)
	{
		for (int i = 1; i < values.Length; i++)
		{
			if (values[i] != values[0])
			{
				return false;
			}
		}
		return true;
	}

	public static string EnsureWindowsLineEndings(this string str)
	{
		return str.Replace("\r\n", "\n").Replace("\n", "\r\n");
	}

	public static string GetRandomAlphanumericString(int count)
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		byte[] random = new byte[count];
		CryptoRNG.GetBytes(random);
		StringBuilder sb = new StringBuilder(count);
		for (int i = 0; i < count; i++)
		{
			sb.Append(chars[random[i] % chars.Length]);
		}
		return sb.ToString();
	}

	public static LinkedListNode<T> GetNodeAtIndex<T>(this LinkedList<T> list, int index)
	{
		LinkedListNode<T> node = list.First;
		for (int i = 0; i < index && node != null; i++)
		{
			node = node.Next;
		}
		return node;
	}

	public static XmlElement CreateXmlDoc(string rootName, params object[] rootAttributePairs)
	{
		XmlDocument doc = new XmlDocument();
		XmlElement root = doc.CreateElement(rootName);
		doc.AppendChild(root);
		root.AddAttributes(rootAttributePairs);
		return root;
	}

	public static int EndIndexOf(this string s, string val)
	{
		int index = s.IndexOf(val);
		return index < 0 ? index : index + val.Length;
	}

	public static object SafeInvoke(this MethodInfo methodInfo, object target, object[] parameters)
	{
		return methodInfo.Invoke(target, parameters);
	}

	public static bool MoveFileWithOverwrite(string source, string dest)
	{
		File.Delete(dest);
		File.Move(source, dest);
		return true;
	}

	public static bool GetFieldValue<T>(this Type type, string name, ref T result)
	{
		FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (field != null && typeof(T).IsAssignableFrom(field.FieldType))
		{
			result = (T)field.GetValue(null);
			return true;
		}
		PropertyInfo property = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (property != null && typeof(T).IsAssignableFrom(property.PropertyType))
		{
			result = (T)property.GetValue(null, null);
			return true;
		}
		return false;
	}

	public static T CreateInstance<T>(string typeName) where T : class, new()
	{
		Type type = Type.GetType(typeName);
		if (type == null)
		{
			return null;
		}
		return Activator.CreateInstance(type) as T;
	}

	public static T CreateInstance<T>(Type type) where T : class, new()
	{
		if (type == null)
		{
			return null;
		}
		return Activator.CreateInstance(type) as T;
	}

	public static T CreateInstanceUnchecked<T>(Type type) where T : class
	{
		if (type == null)
		{
			return null;
		}
		return Activator.CreateInstance(type) as T;
	}

	public static void CopyTo(this Stream input, Stream output)
	{
		byte[] buffer = new byte[16384];
		int read;
		while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
		{
			output.Write(buffer, 0, read);
		}
	}

	public static bool IsFileLockedForWrite(string name)
	{
		try
		{
			using (File.Open(name, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
			{
			}
			return false;
		}
		catch (IOException)
		{
			return true;
		}
	}

	public static int ToInt32(byte[] bytes, int startIndex)
	{
		return BitConverter.ToInt32(bytes, startIndex);
	}

	public static Guid GenerateRandomGuid()
	{
		byte[] bytes = new byte[16];
		CryptoRNG.GetBytes(bytes);
		return new Guid(bytes);
	}

	public static void ForEach<T>(this IEnumerable<T> collection, Action<T, int, bool> action)
	{
		using (IEnumerator<T> enumerator = collection.GetEnumerator())
		{
			if (!enumerator.MoveNext())
			{
				return;
			}
			int index = 0;
			bool more;
			do
			{
				T current = enumerator.Current;
				more = enumerator.MoveNext();
				action(current, index, !more);
				index++;
			}
			while (more);
		}
	}

	private static T CreateDelegateFor<T>(Type type, object target, string name, bool silentIfMissing) where T : class
	{
		if (name == null)
		{
			return null;
		}
		Type delegateType = typeof(T);
		if (!typeof(MulticastDelegate).IsAssignableFrom(delegateType))
		{
			FAssert.Fail(delegateType.ToString() + " is not a delegate type!");
			return null;
		}
		ParameterInfo[] parameters = delegateType.GetMethod("Invoke").GetParameters();
		Type[] types = parameters.Select(p => p.ParameterType).ToArray();
		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | (target == null ? BindingFlags.Static : BindingFlags.Instance);
		MethodInfo method = type.GetMethod(name, flags, null, types, null);
		if (method == null)
		{
			if (!silentIfMissing)
			{
				FAssert.Fail("Could not find method " + name + " on " + type);
			}
			return null;
		}
		return (target == null ? Delegate.CreateDelegate(delegateType, method) : Delegate.CreateDelegate(delegateType, target, method)) as T;
	}

	public static T GetStaticDelegate<T>(Type type, string name, bool silentIfMissing = false) where T : class
	{
		return CreateDelegateFor<T>(type, null, name, silentIfMissing);
	}

	public static T GetDelegate<T>(object obj, string name, bool silentIfMissing = false) where T : class
	{
		if (obj == null)
		{
			return null;
		}
		return CreateDelegateFor<T>(obj.GetType(), obj, name, silentIfMissing);
	}

	public static FileInfo[] GetFilesSortedByDate(string path, string pattern)
	{
		return new DirectoryInfo(path).GetFiles(pattern).OrderBy(f => f.LastWriteTimeUtc).ToArray();
	}

	public static FileInfo[] GetFilesSortedByDateDescending(string path, string pattern)
	{
		return new DirectoryInfo(path).GetFiles(pattern).OrderByDescending(f => f.LastWriteTimeUtc).ToArray();
	}

	public static FileInfo[] GetFiles(string path, string pattern, bool recursive)
	{
		return new DirectoryInfo(path).GetFiles(pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
	}

	public static void PurgeFilesOlderThanDays(string path, string pattern, int days)
	{
		if (!Directory.Exists(path))
		{
			return;
		}
		DateTime cutoff = DateTime.UtcNow.AddDays(-days);
		foreach (FileInfo file in GetFiles(path, pattern, false))
		{
			if (file.LastWriteTimeUtc < cutoff)
			{
				try
				{
					Log("Purging file " + file.FullName + " because it is more than " + days + " days old.");
					file.Delete();
				}
				catch (Exception)
				{
				}
			}
		}
	}

	public static bool SetFieldValue<T>(object obj, string name, T newValue)
	{
		FieldInfo field = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (field == null)
		{
			return false;
		}
		field.SetValue(obj, newValue);
		return true;
	}

	public static string RemovePrefix(this string s, string prefix)
	{
		return s.StartsWith(prefix) ? s.Substring(prefix.Length) : s;
	}

	public static string RemoveSuffix(this string s, string suffix)
	{
		return s.EndsWith(suffix) ? s.Substring(0, s.Length - suffix.Length) : s;
	}

	public static long GetFileSize(string filePath)
	{
		return new FileInfo(filePath).Length;
	}

	public static string GetLocalIPAddress()
	{
		foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
		{
			if (address.AddressFamily == AddressFamily.InterNetwork)
			{
				return address.ToString();
			}
		}
		return null;
	}

	public static string EscapeXML(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		return System.Security.SecurityElement.Escape(input);
	}

	public static string UnEscapeXML(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		return input.Replace("&apos;", "'").Replace("&quot;", "\"").Replace("&gt;", ">").Replace("&lt;", "<").Replace("&amp;", "&");
	}

	public static bool IsAlphanumericOrWhitespace(this string text)
	{
		return text.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));
	}

	public static byte[] SHA256Hash(string str)
	{
		return SHA256Provider.ComputeHash(StringEncodingUTF8.GetBytes(str));
	}

	public static byte[] SHA256Hash(byte[] bytes)
	{
		return SHA256Provider.ComputeHash(bytes);
	}

	public static byte[] SHA256Hash(Stream stream)
	{
		return SHA256Provider.ComputeHash(stream);
	}

	public static byte[] SHA256HashFile(string filename)
	{
		using (FileStream stream = File.OpenRead(filename))
		{
			return SHA256Hash(stream);
		}
	}

	public static byte[] HMAC256(byte[] key, byte[] dataToSign)
	{
		using (HMACSHA256 hmac = new HMACSHA256(key))
		{
			return hmac.ComputeHash(dataToSign);
		}
	}

	public static byte[] HMAC256(string keyString, string stringToSign)
	{
		return HMAC256(StringEncodingUTF8.GetBytes(keyString), stringToSign);
	}

	public static byte[] HMAC256(byte[] key, string stringToSign)
	{
		return HMAC256(key, StringEncodingUTF8.GetBytes(stringToSign));
	}

	public static string StripEmoji(this string text)
	{
		if (text == null)
		{
			return null;
		}
		return Emoji.Replace(text, "");
	}
}
