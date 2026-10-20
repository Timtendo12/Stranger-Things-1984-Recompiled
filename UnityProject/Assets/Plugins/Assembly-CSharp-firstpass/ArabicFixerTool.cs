using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (ArabicFixerTool @ 0x1601b58..0x1604794, Ghidra vaddr).
internal class ArabicFixerTool
{
	internal static bool showTashkeel = true;

	internal static bool combineTashkeel = true;

	internal static bool useHinduNumbers;

	internal static string RemoveTashkeel(string str, out List<TashkeelLocation> tashkeelLocation)
	{
		tashkeelLocation = new List<TashkeelLocation>();
		char[] letters = str.ToCharArray();
		int index = 0;
		for (int i = 0; i < letters.Length; i++)
		{
			char c = letters[i];
			if (c == 'ً' || c == 'ٌ' || c == 'ٍ' || c == 'ْ' || c == 'ٓ')
			{
				tashkeelLocation.Add(new TashkeelLocation(c, i));
				index++;
			}
			else if (c == 'َ')
			{
				if (index > 0 && combineTashkeel && tashkeelLocation[index - 1].tashkeel == 'ّ')
				{
					tashkeelLocation[index - 1].tashkeel = 'ﱠ';
					continue;
				}
				tashkeelLocation.Add(new TashkeelLocation('َ', i));
				index++;
			}
			else if (c == 'ُ')
			{
				if (index > 0 && combineTashkeel && tashkeelLocation[index - 1].tashkeel == 'ّ')
				{
					tashkeelLocation[index - 1].tashkeel = 'ﱡ';
					continue;
				}
				tashkeelLocation.Add(new TashkeelLocation('ُ', i));
				index++;
			}
			else if (c == 'ِ')
			{
				if (index > 0 && combineTashkeel && tashkeelLocation[index - 1].tashkeel == 'ّ')
				{
					tashkeelLocation[index - 1].tashkeel = 'ﱢ';
					continue;
				}
				tashkeelLocation.Add(new TashkeelLocation('ِ', i));
				index++;
			}
			else if (c == 'ّ')
			{
				if (index > 0 && combineTashkeel)
				{
					if (tashkeelLocation[index - 1].tashkeel == 'َ')
					{
						tashkeelLocation[index - 1].tashkeel = 'ﱠ';
						continue;
					}
					if (tashkeelLocation[index - 1].tashkeel == 'ُ')
					{
						tashkeelLocation[index - 1].tashkeel = 'ﱡ';
						continue;
					}
					if (tashkeelLocation[index - 1].tashkeel == 'ِ')
					{
						tashkeelLocation[index - 1].tashkeel = 'ﱢ';
						continue;
					}
				}
				tashkeelLocation.Add(new TashkeelLocation('ّ', i));
				index++;
			}
		}
		string[] split = str.Split(new char[12] { 'ً', 'ٌ', 'ٍ', 'َ', 'ُ', 'ِ', 'ّ', 'ْ', 'ٓ', 'ﱠ', 'ﱡ', 'ﱢ' });
		str = string.Empty;
		for (int j = 0; j < split.Length; j++)
		{
			str += split[j];
		}
		return str;
	}

	internal static char[] ReturnTashkeel(char[] letters, List<TashkeelLocation> tashkeelLocation)
	{
		char[] lettersWithTashkeel = new char[letters.Length + tashkeelLocation.Count];
		int tracker = 0;
		for (int i = 0; i < letters.Length; i++)
		{
			lettersWithTashkeel[tracker] = letters[i];
			tracker++;
			foreach (TashkeelLocation location in tashkeelLocation)
			{
				if (location.position == tracker)
				{
					lettersWithTashkeel[tracker] = location.tashkeel;
					tracker++;
				}
			}
		}
		return lettersWithTashkeel;
	}

	internal static string FixLineHe(string str)
	{
		List<TashkeelLocation> tashkeelLocation;
		string origin = RemoveTashkeel(str, out tashkeelLocation);
		origin.ToCharArray();
		char[] lettersFinal = origin.ToCharArray();
		if (showTashkeel)
		{
			lettersFinal = ReturnTashkeel(lettersFinal, tashkeelLocation);
		}
		return ReverseLine(lettersFinal, bracketsToNumberList: true);
	}

	internal static string FixLineAr(string str)
	{
		List<TashkeelLocation> tashkeelLocation;
		string origin = RemoveTashkeel(str, out tashkeelLocation);
		char[] lettersOrigin = origin.ToCharArray();
		char[] lettersFinal = origin.ToCharArray();
		for (int i = 0; i < lettersOrigin.Length; i++)
		{
			lettersOrigin[i] = (char)ArabicTable.ArabicMapper.Convert(lettersOrigin[i]);
		}
		for (int j = 0; j < lettersOrigin.Length; j++)
		{
			int skip = 0;
			if (lettersOrigin[j] == 'ﻝ' && j < lettersOrigin.Length - 1)
			{
				// Lam followed by one of the (mapped) alef forms: ligature base glyph (the leading-letter rule below adds the final +2)
				switch (lettersOrigin[j + 1])
				{
				case 'ﺁ':
					lettersOrigin[j] = 'ﻳ';
					lettersFinal[j + 1] = '￿';
					skip = 1;
					break;
				case 'ﺃ':
					lettersOrigin[j] = 'ﻵ';
					lettersFinal[j + 1] = '￿';
					skip = 1;
					break;
				case 'ﺇ':
					lettersOrigin[j] = 'ﻷ';
					lettersFinal[j + 1] = '￿';
					skip = 1;
					break;
				case 'ﺍ':
					lettersOrigin[j] = 'ﻹ';
					lettersFinal[j + 1] = '￿';
					skip = 1;
					break;
				}
			}
			if (!IsIgnoredCharacter(lettersOrigin[j]))
			{
				if (IsMiddleLetter(lettersOrigin, j))
				{
					lettersFinal[j] = (char)(lettersOrigin[j] + 3);
				}
				else if (IsFinishingLetter(lettersOrigin, j))
				{
					lettersFinal[j] = (char)(lettersOrigin[j] + 1);
				}
				else if (IsLeadingLetter(lettersOrigin, j))
				{
					lettersFinal[j] = (char)(lettersOrigin[j] + 2);
				}
			}
			j += skip;
			if (useHinduNumbers && j < lettersOrigin.Length)
			{
				char digit = lettersOrigin[j];
				if (digit >= '0' && digit <= '9')
				{
					lettersFinal[j] = (char)(0x660 + (digit - '0'));
				}
			}
		}
		if (showTashkeel)
		{
			lettersFinal = ReturnTashkeel(lettersFinal, tashkeelLocation);
		}
		return ReverseLine(lettersFinal, bracketsToNumberList: false);
	}

	// Shared second pass of FixLineAr/FixLineHe: walks the shaped line backwards and builds the visual (right-to-left) order.
	// The two shipped versions differ only in where '[' and ']' go (Arabic: straight into the output, Hebrew: into the pending run).
	private static string ReverseLine(char[] lettersFinal, bool bracketsToNumberList)
	{
		List<char> list = new List<char>();
		List<char> numberList = new List<char>();
		for (int i = lettersFinal.Length - 1; i >= 0; i--)
		{
			char c = lettersFinal[i];
			bool neighbourPunct = i != 0 && char.IsPunctuation(c) && i < lettersFinal.Length - 1 && (char.IsPunctuation(lettersFinal[i - 1]) || char.IsPunctuation(lettersFinal[i + 1]));
			if (neighbourPunct)
			{
				switch (c)
				{
				case '(':
					list.Add(')');
					break;
				case ')':
					list.Add('(');
					break;
				case '<':
					list.Add('>');
					break;
				case '>':
					list.Add('<');
					break;
				case '[':
					list.Add(']');
					break;
				case ']':
					list.Add('[');
					break;
				case '￿':
					break;
				default:
					numberList.Add(c);
					break;
				}
				continue;
			}
			if (i > 0 && i < lettersFinal.Length - 1 && c == ' ' && (char.IsLower(lettersFinal[i - 1]) || char.IsUpper(lettersFinal[i - 1]) || char.IsNumber(lettersFinal[i - 1])) && (char.IsLower(lettersFinal[i + 1]) || char.IsUpper(lettersFinal[i + 1]) || char.IsNumber(lettersFinal[i + 1])))
			{
				numberList.Add(c);
				continue;
			}
			bool special = char.IsNumber(c) || char.IsLower(c) || char.IsUpper(c) || char.IsSymbol(c) || char.IsPunctuation(c);
			// port guard: the shipped code indexes lettersFinal[i + 1] here and throws for a trailing space
			if (!special && i > 1 && c == ' ' && i + 1 < lettersFinal.Length)
			{
				if (lettersFinal[i - 1] == '"' && char.IsUpper(lettersFinal[i + 1]))
				{
					special = true;
				}
				else if (lettersFinal[i + 1] == '/')
				{
					special = true;
				}
			}
			if (!special)
			{
				if (c < '\ud800' || c > '\udfff')
				{
					IncludeNumbers(ref numberList, ref list, c);
				}
				else
				{
					numberList.Add(c);
				}
				continue;
			}
			switch (c)
			{
			case '!':
			case '؟':
				list.Add(c);
				break;
			case '(':
				numberList.Add(')');
				break;
			case ')':
				numberList.Add('(');
				break;
			case '-':
				IncludeNumbers(ref numberList, ref list, '-');
				break;
			case '.':
				if (i + 1 < lettersFinal.Length && char.IsNumber(lettersFinal[i + 1]))
				{
					numberList.Add('.');
				}
				else
				{
					list.Add('.');
				}
				break;
			case '[':
				(bracketsToNumberList ? numberList : list).Add(']');
				break;
			case ']':
				(bracketsToNumberList ? numberList : list).Add('[');
				break;
			case '￿':
				break;
			default:
				numberList.Add(c);
				break;
			}
		}
		if (numberList.Count > 0)
		{
			for (int k = 0; k < numberList.Count; k++)
			{
				list.Add(numberList[numberList.Count - 1 - k]);
			}
			numberList.Clear();
		}
		char[] result = new char[list.Count];
		for (int l = 0; l < result.Length; l++)
		{
			result[l] = list[l];
		}
		return new string(result);
	}

	private static void IncludeNumbers(ref List<char> numberList, ref List<char> list, char letter)
	{
		if (numberList.Count > 0)
		{
			for (int i = 0; i < numberList.Count; i++)
			{
				list.Add(numberList[numberList.Count - 1 - i]);
			}
			numberList.Clear();
		}
		if (letter != 0 && letter != '￿')
		{
			list.Add(letter);
		}
	}

	internal static bool IsIgnoredCharacter(char ch)
	{
		bool isPunctuation = char.IsPunctuation(ch);
		bool isNumber = char.IsNumber(ch);
		bool isLower = char.IsLower(ch);
		bool isUpper = char.IsUpper(ch);
		bool isSymbol = char.IsSymbol(ch);
		bool isPersian = ch == 'ﭖ' || ch == 'ﭺ' || ch == 'ﮊ' || ch == 'ﮒ' || ch == 'ﮎ';
		bool isPresentationFormB = ch <= '﻿' && ch >= 'ﹰ';
		bool isAcceptable = isPresentationFormB || isPersian || ch == 'ﯼ';
		if (!(isPunctuation || isNumber || isLower || isUpper || isSymbol) && isAcceptable)
		{
			return ch == '>' || ch == 'a' || ch == '<' || ch == '؛';
		}
		return true;
	}

	// Letters that never connect to the following letter (alef/dal/dhal/ra/zay/waw presentation forms).
	private static bool IsNonJoiningMiddle(char c)
	{
		switch (c)
		{
		case 'ﺀ':
		case 'ﺁ':
		case 'ﺃ':
		case 'ﺅ':
		case 'ﺇ':
		case 'ﺍ':
		case 'ﺩ':
		case 'ﺫ':
		case 'ﺭ':
		case 'ﺯ':
		case 'ﮊ':
		case 'ﻭ':
			return true;
		default:
			return false;
		}
	}

	internal static bool IsMiddleLetter(char[] letters, int index)
	{
		bool previousJoins = false;
		bool currentJoins = false;
		if (index != 0)
		{
			currentJoins = !IsNonJoiningMiddle(letters[index]);
			char prev = letters[index - 1];
			if (!IsNonJoiningMiddle(prev) && !char.IsPunctuation(prev) && prev != ' ' && prev != '<' && prev != '>')
			{
				previousJoins = prev != '*';
			}
		}
		if (index < letters.Length - 1)
		{
			char next = letters[index + 1];
			if (next == '\r' || next == ' ' || next == 'ﺀ')
			{
				return false;
			}
			if (char.IsNumber(next) || char.IsSymbol(next))
			{
				return false;
			}
			return previousJoins && currentJoins && !char.IsPunctuation(next);
		}
		return false;
	}

	internal static bool IsFinishingLetter(char[] letters, int index)
	{
		bool previousNonJoining = true;
		if (index != 0)
		{
			char prev = letters[index - 1];
			if (prev != ' ' && !IsNonJoiningMiddle(prev) && !char.IsPunctuation(prev) && !char.IsSymbol(prev))
			{
				previousNonJoining = (prev & 0xFFFD) == 60;
			}
		}
		char current = letters[index];
		if (current == ' ' || current == 'ﺀ')
		{
			previousNonJoining = true;
		}
		return !previousNonJoining;
	}

	internal static bool IsLeadingLetter(char[] letters, int index)
	{
		bool previousBreaks = true;
		if (index != 0)
		{
			char prev = letters[index - 1];
			if (!(prev == ' ' || prev == '*' || prev == 'A') && !char.IsPunctuation(prev))
			{
				previousBreaks = prev == '<' || prev == '>' || prev == 'ﮊ' || prev == 'ﻭ' || prev == 'ﺅ' || prev == 'ﺀ' || prev == 'ﺁ' || prev == 'ﺃ' || prev == 'ﺇ' || prev == 'ﺍ' || prev == 'ﺩ' || prev == 'ﺫ' || prev == 'ﺭ' || prev == 'ﺯ';
			}
		}
		char current = letters[index];
		bool currentLeads = false;
		if (!(current == 'ﺁ' || current == 'ﺃ' || current == 'ﺅ' || current == 'ﺇ' || current == 'ﺍ' || current == 'ﺩ' || current == 'ﺫ' || current == 'ﺭ' || current == 'ﺯ') && current != ' ' && current != 'ﮊ')
		{
			currentLeads = current != 'ﻭ' && current != 'ﺀ';
		}
		bool nextOk = false;
		if (index < letters.Length - 1)
		{
			char next = letters[index + 1];
			if (!(next == '\n' || next == '\r' || next == ' ') && !char.IsPunctuation(next) && !char.IsNumber(next) && !char.IsSymbol(next) && !char.IsLower(next) && !char.IsUpper(next))
			{
				nextOk = next != 'ﺀ';
			}
		}
		return nextOk && currentLeads && previousBreaks;
	}
}
