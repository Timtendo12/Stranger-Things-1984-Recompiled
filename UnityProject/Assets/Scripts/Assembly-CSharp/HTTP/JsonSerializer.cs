using System;
using System.Collections;
using System.Text;

namespace HTTP
{
	public class JsonSerializer
	{
		public const int TOKEN_NONE = 0;

		public const int TOKEN_CURLY_OPEN = 1;

		public const int TOKEN_CURLY_CLOSE = 2;

		public const int TOKEN_SQUARED_OPEN = 3;

		public const int TOKEN_SQUARED_CLOSE = 4;

		public const int TOKEN_COLON = 5;

		public const int TOKEN_COMMA = 6;

		public const int TOKEN_STRING = 7;

		public const int TOKEN_NUMBER = 8;

		public const int TOKEN_TRUE = 9;

		public const int TOKEN_FALSE = 10;

		public const int TOKEN_NULL = 11;

		public static object Decode(byte[] json)
		{
			return null;
		}

		public static object Decode(string json)
		{
			return null;
		}

		public static void Decode(object instance, string json)
		{
		}

		public static object Decode(string json, ref bool success)
		{
			return null;
		}

		public static string Encode(object json)
		{
			return null;
		}

		public static T Decode<T>(byte[] json) where T : class, new()
		{
			return null;
		}

		public static T Decode<T>(string json) where T : class, new()
		{
			return null;
		}

		private static object PopulateObject(Type T, object obj)
		{
			return null;
		}

		private static object PopulateObject(Type T, object obj, object instance)
		{
			return null;
		}

		protected static Hashtable ParseObject(char[] json, ref int index, ref bool success)
		{
			return null;
		}

		protected static ArrayList ParseArray(char[] json, ref int index, ref bool success)
		{
			return null;
		}

		protected static object ParseValue(char[] json, ref int index, ref bool success)
		{
			return null;
		}

		protected static string ParseString(char[] json, ref int index, ref bool success)
		{
			return null;
		}

		protected static object ParseNumber(char[] json, ref int index, ref bool success)
		{
			return null;
		}

		protected static int GetLastIndexOfNumber(char[] json, int index)
		{
			return 0;
		}

		protected static void EatWhitespace(char[] json, ref int index)
		{
		}

		protected static int LookAhead(char[] json, int index)
		{
			return 0;
		}

		protected static int NextToken(char[] json, ref int index)
		{
			return 0;
		}

		protected static bool SerializeValue(object value, StringBuilder builder)
		{
			return false;
		}

		protected static bool SerializeObject(Hashtable anObject, StringBuilder builder)
		{
			return false;
		}

		protected static bool SerializeArray(IEnumerable anArray, StringBuilder builder)
		{
			return false;
		}

		protected static bool SerializeString(string aString, StringBuilder builder)
		{
			return false;
		}

		protected static bool SerializeNumber(int number, StringBuilder builder)
		{
			return false;
		}

		protected static bool SerializeNumber(float number, StringBuilder builder)
		{
			return false;
		}

		protected static bool IsNumeric(object o)
		{
			return false;
		}
	}
}
