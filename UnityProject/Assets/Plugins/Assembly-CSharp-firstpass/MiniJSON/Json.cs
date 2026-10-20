using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace MiniJSON
{
	// Standard MiniJSON (Calvin Rien) as used by the shipped build.
	public static class Json
	{
		private sealed class Parser : IDisposable
		{
			private enum TOKEN
			{
				NONE = 0,
				CURLY_OPEN = 1,
				CURLY_CLOSE = 2,
				SQUARED_OPEN = 3,
				SQUARED_CLOSE = 4,
				COLON = 5,
				COMMA = 6,
				STRING = 7,
				NUMBER = 8,
				TRUE = 9,
				FALSE = 10,
				NULL = 11
			}

			private const string WHITE_SPACE = " \t\n\r";

			private const string WORD_BREAK = " \t\n\r{}[],:\"";

			private StringReader json;

			private char PeekChar
			{
				get
				{
					int c = json.Peek();
					return (c == -1) ? '\0' : Convert.ToChar(c);
				}
			}

			private char NextChar => Convert.ToChar(json.Read());

			private string NextWord
			{
				get
				{
					StringBuilder word = new StringBuilder();
					while (WORD_BREAK.IndexOf(PeekChar) == -1)
					{
						word.Append(NextChar);
						if (json.Peek() == -1)
						{
							break;
						}
					}
					return word.ToString();
				}
			}

			private TOKEN NextToken
			{
				get
				{
					EatWhitespace();
					if (json.Peek() == -1)
					{
						return TOKEN.NONE;
					}
					switch (PeekChar)
					{
					case '{':
						return TOKEN.CURLY_OPEN;
					case '}':
						json.Read();
						return TOKEN.CURLY_CLOSE;
					case '[':
						return TOKEN.SQUARED_OPEN;
					case ']':
						json.Read();
						return TOKEN.SQUARED_CLOSE;
					case ',':
						json.Read();
						return TOKEN.COMMA;
					case '"':
						return TOKEN.STRING;
					case ':':
						return TOKEN.COLON;
					case '-':
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						return TOKEN.NUMBER;
					}
					switch (NextWord)
					{
					case "false":
						return TOKEN.FALSE;
					case "true":
						return TOKEN.TRUE;
					case "null":
						return TOKEN.NULL;
					}
					return TOKEN.NONE;
				}
			}

			private Parser(string jsonString)
			{
				json = new StringReader(jsonString);
			}

			public static object Parse(string jsonString)
			{
				using (Parser parser = new Parser(jsonString))
				{
					return parser.ParseValue();
				}
			}

			public void Dispose()
			{
				json.Dispose();
				json = null;
			}

			private Dictionary<string, object> ParseObject()
			{
				Dictionary<string, object> table = new Dictionary<string, object>();
				json.Read();
				while (true)
				{
					switch (NextToken)
					{
					case TOKEN.NONE:
						return null;
					case TOKEN.COMMA:
						continue;
					case TOKEN.CURLY_CLOSE:
						return table;
					}
					string name = ParseString();
					if (name == null)
					{
						return null;
					}
					if (NextToken != TOKEN.COLON)
					{
						return null;
					}
					json.Read();
					table[name] = ParseValue();
				}
			}

			private List<object> ParseArray()
			{
				List<object> array = new List<object>();
				json.Read();
				bool parsing = true;
				while (parsing)
				{
					TOKEN nextToken = NextToken;
					switch (nextToken)
					{
					case TOKEN.NONE:
						return null;
					case TOKEN.COMMA:
						continue;
					case TOKEN.SQUARED_CLOSE:
						parsing = false;
						break;
					default:
						array.Add(ParseByToken(nextToken));
						break;
					}
				}
				return array;
			}

			private object ParseValue()
			{
				return ParseByToken(NextToken);
			}

			private object ParseByToken(TOKEN token)
			{
				switch (token)
				{
				case TOKEN.STRING:
					return ParseString();
				case TOKEN.NUMBER:
					return ParseNumber();
				case TOKEN.CURLY_OPEN:
					return ParseObject();
				case TOKEN.SQUARED_OPEN:
					return ParseArray();
				case TOKEN.TRUE:
					return true;
				case TOKEN.FALSE:
					return false;
				case TOKEN.NULL:
					return null;
				default:
					return null;
				}
			}

			private string ParseString()
			{
				StringBuilder s = new StringBuilder();
				json.Read();
				bool parsing = true;
				while (parsing)
				{
					if (json.Peek() == -1)
					{
						break;
					}
					char c = NextChar;
					switch (c)
					{
					case '"':
						parsing = false;
						break;
					case '\\':
						if (json.Peek() == -1)
						{
							parsing = false;
							break;
						}
						c = NextChar;
						switch (c)
						{
						case '"':
						case '/':
						case '\\':
							s.Append(c);
							break;
						case 'b':
							s.Append('\b');
							break;
						case 'f':
							s.Append('\f');
							break;
						case 'n':
							s.Append('\n');
							break;
						case 'r':
							s.Append('\r');
							break;
						case 't':
							s.Append('\t');
							break;
						case 'u':
						{
							char[] hex = new char[4];
							for (int i = 0; i < 4; i++)
							{
								hex[i] = NextChar;
							}
							s.Append((char)Convert.ToInt32(new string(hex), 16));
							break;
						}
						}
						break;
					default:
						s.Append(c);
						break;
					}
				}
				return s.ToString();
			}

			private object ParseNumber()
			{
				string number = NextWord;
				if (number.IndexOf('.') == -1)
				{
					long.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out long result);
					return result;
				}
				double.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsed);
				return parsed;
			}

			private void EatWhitespace()
			{
				while (WHITE_SPACE.IndexOf(PeekChar) != -1)
				{
					json.Read();
					if (json.Peek() == -1)
					{
						break;
					}
				}
			}
		}

		private sealed class Serializer
		{
			private StringBuilder builder;

			private Serializer()
			{
				builder = new StringBuilder();
			}

			public static string Serialize(object obj)
			{
				Serializer serializer = new Serializer();
				serializer.SerializeValue(obj);
				return serializer.builder.ToString();
			}

			private void SerializeValue(object value)
			{
				if (value == null)
				{
					builder.Append("null");
				}
				else if (value is string str)
				{
					SerializeString(str);
				}
				else if (value is bool b)
				{
					builder.Append(b ? "true" : "false");
				}
				else if (value is IList list)
				{
					SerializeArray(list);
				}
				else if (value is IDictionary dict)
				{
					SerializeObject(dict);
				}
				else if (value is char c)
				{
					SerializeString(new string(c, 1));
				}
				else
				{
					SerializeOther(value);
				}
			}

			private void SerializeObject(IDictionary obj)
			{
				bool first = true;
				builder.Append('{');
				foreach (object key in obj.Keys)
				{
					if (!first)
					{
						builder.Append(',');
					}
					SerializeString(key.ToString());
					builder.Append(':');
					SerializeValue(obj[key]);
					first = false;
				}
				builder.Append('}');
			}

			private void SerializeArray(IList anArray)
			{
				builder.Append('[');
				bool first = true;
				foreach (object obj in anArray)
				{
					if (!first)
					{
						builder.Append(',');
					}
					SerializeValue(obj);
					first = false;
				}
				builder.Append(']');
			}

			private void SerializeString(string str)
			{
				builder.Append('"');
				foreach (char c in str.ToCharArray())
				{
					switch (c)
					{
					case '"':
						builder.Append("\\\"");
						break;
					case '\\':
						builder.Append("\\\\");
						break;
					case '\b':
						builder.Append("\\b");
						break;
					case '\f':
						builder.Append("\\f");
						break;
					case '\n':
						builder.Append("\\n");
						break;
					case '\r':
						builder.Append("\\r");
						break;
					case '\t':
						builder.Append("\\t");
						break;
					default:
						int codepoint = Convert.ToInt32(c);
						if (codepoint >= 32 && codepoint <= 126)
						{
							builder.Append(c);
						}
						else
						{
							builder.Append("\\u");
							builder.Append(codepoint.ToString("x4"));
						}
						break;
					}
				}
				builder.Append('"');
			}

			private void SerializeOther(object value)
			{
				if (value is float f)
				{
					builder.Append(f.ToString("R", CultureInfo.InvariantCulture));
				}
				else if (value is int || value is uint || value is long || value is sbyte || value is byte || value is short || value is ushort || value is ulong)
				{
					builder.Append(value);
				}
				else if (value is double || value is decimal)
				{
					builder.Append(Convert.ToDouble(value).ToString("R", CultureInfo.InvariantCulture));
				}
				else
				{
					SerializeString(value.ToString());
				}
			}
		}

		public static object Deserialize(string json)
		{
			if (json == null)
			{
				return null;
			}
			return Parser.Parse(json);
		}

		public static string Serialize(object obj)
		{
			return Serializer.Serialize(obj);
		}
	}
}
