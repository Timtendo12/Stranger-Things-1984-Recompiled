// Standard LZMA SDK (public domain), restored to match the shipped signatures.
using System;
using System.Collections;

namespace SevenZip.CommandLineParser
{
	public class Parser
	{
		public ArrayList NonSwitchStrings = new ArrayList();

		private SwitchResult[] _switches;

		private const char kSwitchID1 = '-';

		private const char kSwitchID2 = '/';

		private const char kSwitchMinus = '-';

		private const string kStopSwitchParsing = "--";

		public SwitchResult this[int index] => _switches[index];

		public Parser(int numSwitches)
		{
			_switches = new SwitchResult[numSwitches];
			for (int i = 0; i < numSwitches; i++)
			{
				_switches[i] = new SwitchResult();
			}
		}

		private bool ParseString(string srcString, SwitchForm[] switchForms)
		{
			int length = srcString.Length;
			if (length == 0)
			{
				return false;
			}
			int num = 0;
			if (!IsItSwitchChar(srcString[num]))
			{
				return false;
			}
			while (num < length)
			{
				if (IsItSwitchChar(srcString[num]))
				{
					num++;
				}
				int num2 = 0;
				int num3 = -1;
				for (int i = 0; i < _switches.Length; i++)
				{
					int length2 = switchForms[i].IDString.Length;
					if (length2 > num3 && num + length2 <= length && string.Compare(switchForms[i].IDString, 0, srcString, num, length2, true) == 0)
					{
						num2 = i;
						num3 = length2;
					}
				}
				if (num3 == -1)
				{
					throw new Exception("maxLen == kNoLen");
				}
				SwitchResult switchResult = _switches[num2];
				SwitchForm switchForm = switchForms[num2];
				if (!switchForm.Multi && switchResult.ThereIs)
				{
					throw new Exception("switch must be single");
				}
				switchResult.ThereIs = true;
				num += num3;
				int num4 = length - num;
				SwitchType type = switchForm.Type;
				switch (type)
				{
				case SwitchType.PostMinus:
					if (num4 == 0)
					{
						switchResult.WithMinus = false;
					}
					else
					{
						switchResult.WithMinus = srcString[num] == kSwitchMinus;
						if (switchResult.WithMinus)
						{
							num++;
						}
					}
					break;
				case SwitchType.PostChar:
				{
					if (num4 < switchForm.MinLen)
					{
						throw new Exception("switch is not full");
					}
					string postCharSet = switchForm.PostCharSet;
					if (num4 == 0)
					{
						switchResult.PostCharIndex = -1;
					}
					else
					{
						int num5 = postCharSet.IndexOf(srcString[num]);
						if (num5 < 0)
						{
							switchResult.PostCharIndex = -1;
						}
						else
						{
							switchResult.PostCharIndex = num5;
							num++;
						}
					}
					break;
				}
				case SwitchType.LimitedPostString:
				case SwitchType.UnLimitedPostString:
				{
					int minLen = switchForm.MinLen;
					if (num4 < minLen)
					{
						throw new Exception("switch is not full");
					}
					if (type == SwitchType.UnLimitedPostString)
					{
						switchResult.PostStrings.Add(srcString.Substring(num));
						return true;
					}
					string text = srcString.Substring(num, minLen);
					num += minLen;
					for (int j = minLen; j < switchForm.MaxLen && num < length; j++)
					{
						char c = srcString[num];
						if (IsItSwitchChar(c))
						{
							break;
						}
						text += c;
						num++;
					}
					switchResult.PostStrings.Add(text);
					break;
				}
				}
			}
			return true;
		}

		public void ParseStrings(SwitchForm[] switchForms, string[] commandStrings)
		{
			int num = commandStrings.Length;
			bool flag = false;
			for (int i = 0; i < num; i++)
			{
				string text = commandStrings[i];
				if (flag)
				{
					NonSwitchStrings.Add(text);
				}
				else if (text == kStopSwitchParsing)
				{
					flag = true;
				}
				else if (!ParseString(text, switchForms))
				{
					NonSwitchStrings.Add(text);
				}
			}
		}

		public static int ParseCommand(CommandForm[] commandForms, string commandString, out string postString)
		{
			for (int i = 0; i < commandForms.Length; i++)
			{
				string iDString = commandForms[i].IDString;
				if (commandForms[i].PostStringMode)
				{
					if (commandString.IndexOf(iDString) == 0)
					{
						postString = commandString.Substring(iDString.Length);
						return i;
					}
				}
				else if (commandString == iDString)
				{
					postString = "";
					return i;
				}
			}
			postString = "";
			return -1;
		}

		private static bool ParseSubCharsCommand(int numForms, CommandSubCharsSet[] forms, string commandString, ArrayList indices)
		{
			indices.Clear();
			int num = 0;
			for (int i = 0; i < numForms; i++)
			{
				CommandSubCharsSet commandSubCharsSet = forms[i];
				int num2 = -1;
				int length = commandSubCharsSet.Chars.Length;
				for (int j = 0; j < length; j++)
				{
					char value = commandSubCharsSet.Chars[j];
					int num3 = commandString.IndexOf(value);
					if (num3 >= 0)
					{
						if (num2 >= 0)
						{
							return false;
						}
						if (commandString.IndexOf(value, num3 + 1) >= 0)
						{
							return false;
						}
						num2 = j;
						num++;
					}
				}
				if (num2 == -1 && !commandSubCharsSet.EmptyAllowed)
				{
					return false;
				}
				indices.Add(num2);
			}
			return num == commandString.Length;
		}

		private static bool IsItSwitchChar(char c)
		{
			if (c != kSwitchID1)
			{
				return c == kSwitchID2;
			}
			return true;
		}
	}
}
