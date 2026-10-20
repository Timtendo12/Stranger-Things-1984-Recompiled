using System.Collections.Generic;

// Rewritten from libil2cpp.so 1.0.605 (HindiCorrector @ 0x1673874..0x1679248, Ghidra vaddr).
// The 155-entry GSUB table (conjunct sequence -> private-use glyph) was read from the static constructor (0x1673a5c).
public class HindiCorrector
{
	public class GSUB
	{
		public string source;

		public string dest;
	}

	private static GSUB[] gsub = new GSUB[155]
	{
		new GSUB { source = "\u0926\u094d\u0926", dest = "\uf45a" },
		new GSUB { source = "\u0915\u094d\u0930", dest = "\uf3d1" },
		new GSUB { source = "\u0916\u094d\u0930", dest = "\uf364" },
		new GSUB { source = "\u0917\u094d\u0930", dest = "\uf365" },
		new GSUB { source = "\u0918\u094d\u0930", dest = "\uf366" },
		new GSUB { source = "\u091a\u094d\u0930", dest = "\uf367" },
		new GSUB { source = "\u091c\u094d\u0930", dest = "\uf368" },
		new GSUB { source = "\u091d\u094d\u0930", dest = "\uf369" },
		new GSUB { source = "\u091e\u094d\u0930", dest = "\uf36b" },
		new GSUB { source = "\u091f\u094d\u0930", dest = "\u091f\uf1b0" },
		new GSUB { source = "\u0920\u094d\u0930", dest = "\u0920\uf1b0" },
		new GSUB { source = "\u0921\u094d\u0930", dest = "\u0921\uf1b0" },
		new GSUB { source = "\u0922\u094d\u0930", dest = "\u0922\uf1b0" },
		new GSUB { source = "\u0923\u094d\u0930", dest = "\uf36c" },
		new GSUB { source = "\u0924\u094d\u0930", dest = "\uf36e" },
		new GSUB { source = "\u0925\u094d\u0930", dest = "\uf36f" },
		new GSUB { source = "\u0926\u094d\u0930", dest = "\uf370" },
		new GSUB { source = "\u0927\u094d\u0930", dest = "\uf371" },
		new GSUB { source = "\u0928\u094d\u0930", dest = "\uf373" },
		new GSUB { source = "\u092a\u094d\u0930", dest = "\uf374" },
		new GSUB { source = "\u092b\u094d\u0930", dest = "\uf375" },
		new GSUB { source = "\u092c\u094d\u0930", dest = "\uf376" },
		new GSUB { source = "\u092d\u094d\u0930", dest = "\uf377" },
		new GSUB { source = "\u092e\u094d\u0930", dest = "\uf379" },
		new GSUB { source = "\u092f\u094d\u0930", dest = "\uf37a" },
		new GSUB { source = "\u0932\u094d\u0930", dest = "\uf37b" },
		new GSUB { source = "\u0935\u094d\u0930", dest = "\uf37c" },
		new GSUB { source = "\u0936\u094d\u0930", dest = "\uf37d" },
		new GSUB { source = "\u0937\u094d\u0930", dest = "\uf37e" },
		new GSUB { source = "\u0938\u094d\u0930", dest = "\uf37f" },
		new GSUB { source = "\u0939\u094d\u0930", dest = "\uf380" },
		new GSUB { source = "\u0930\u094d\u0915", dest = "\u0915\uf005" },
		new GSUB { source = "\u0930\u094d\u0916", dest = "\u0916\uf306" },
		new GSUB { source = "\u0930\u094d\u0917", dest = "\u0917\uf306" },
		new GSUB { source = "\u0930\u094d\u0918", dest = "\u0918\uf306" },
		new GSUB { source = "\u0930\u094d\u0919", dest = "\u0919\uf003" },
		new GSUB { source = "\u0930\u094d\u091a", dest = "\u091a\uf306" },
		new GSUB { source = "\u0930\u094d\u091b", dest = "\u091b\uf001" },
		new GSUB { source = "\u0930\u094d\u091c", dest = "\u091c\uf306" },
		new GSUB { source = "\u0930\u094d\u091d", dest = "\u091d\uf306" },
		new GSUB { source = "\u0930\u094d\u091e", dest = "\u091e\uf306" },
		new GSUB { source = "\u0930\u094d\u091f", dest = "\u091f\uf001" },
		new GSUB { source = "\u0930\u094d\u0920", dest = "\u0920\uf001" },
		new GSUB { source = "\u0930\u094d\u0921", dest = "\u0921\uf001" },
		new GSUB { source = "\u0930\u094d\u0922", dest = "\u0922\uf001" },
		new GSUB { source = "\u0930\u094d\u0923", dest = "\u0923\uf306" },
		new GSUB { source = "\u0930\u094d\u0924", dest = "\u0924\uf306" },
		new GSUB { source = "\u0930\u094d\u0925", dest = "\u0925\uf306" },
		new GSUB { source = "\u0930\u094d\u0926", dest = "\u0926\uf000" },
		new GSUB { source = "\u0930\u094d\u0927", dest = "\u0927\uf306" },
		new GSUB { source = "\u0930\u094d\u0928", dest = "\u0928\uf306" },
		new GSUB { source = "\u0930\u094d\u0929", dest = "\u0929\uf306" },
		new GSUB { source = "\u0930\u094d\u092a", dest = "\u092a\uf306" },
		new GSUB { source = "\u0930\u094d\u092b\u093c", dest = "\u095e\uf306" },
		new GSUB { source = "\u0930\u094d\u092c", dest = "\u092c\uf306" },
		new GSUB { source = "\u0930\u094d\u092d", dest = "\u092d\uf306" },
		new GSUB { source = "\u0930\u094d\u092e", dest = "\u092e\uf306" },
		new GSUB { source = "\u0930\u094d\u092f", dest = "\u092f\uf306" },
		new GSUB { source = "\u0930\u094d\u0930", dest = "\u0930\uf000" },
		new GSUB { source = "\u0930\u094d\u0931", dest = "\u0931\uf000" },
		new GSUB { source = "\u0930\u094d\u0932", dest = "\u0932\uf002" },
		new GSUB { source = "\u0930\u094d\u0933", dest = "\u0933\uf002" },
		new GSUB { source = "\u0930\u094d\u0934", dest = "\u0934\uf002" },
		new GSUB { source = "\u0930\u094d\u0935", dest = "\u0935\uf306" },
		new GSUB { source = "\u0930\u094d\u0936", dest = "\u0936\uf306" },
		new GSUB { source = "\u0930\u094d\u0937", dest = "\u0937\uf306" },
		new GSUB { source = "\u0930\u094d\u0938", dest = "\u0938\uf306" },
		new GSUB { source = "\u0930\u094d\u0939", dest = "\u0939\uf306" },
		new GSUB { source = "\u0915\u094d\u0937", dest = "\uf337" },
		new GSUB { source = "\u091c\u094d\u091e", dest = "\uf339" },
		new GSUB { source = "\u0924\u094d\u0928", dest = "\uf4f5" },
		new GSUB { source = "\u0926\u094d\u0927", dest = "\uf46a" },
		new GSUB { source = "\u0926\u094d\u092d", dest = "\uf01a" },
		new GSUB { source = "\u0926\u094d\u092e", dest = "\uf3e3" },
		new GSUB { source = "\u0926\u094d\u092f", dest = "\uf3e4" },
		new GSUB { source = "\u0926\u094d\u0935", dest = "\uf493" },
		new GSUB { source = "\u0938\u094d\u0924", dest = "\uf35b\u0924" },
		new GSUB { source = "\u0915\u094d", dest = "\uf33a" },
		new GSUB { source = "\u0916\u094d", dest = "\uf33b" },
		new GSUB { source = "\u0917\u094d", dest = "\uf33c" },
		new GSUB { source = "\u0918\u094d", dest = "\uf33d" },
		new GSUB { source = "\u091a\u094d", dest = "\uf33e" },
		new GSUB { source = "\u091b\u094d", dest = "\uf334" },
		new GSUB { source = "\u091c\u094d", dest = "\uf33f" },
		new GSUB { source = "\u091d\u094d", dest = "\uf340" },
		new GSUB { source = "\u091e\u094d", dest = "\uf343" },
		new GSUB { source = "\u0923\u094d", dest = "\uf344" },
		new GSUB { source = "\u0924\u094d", dest = "\uf346" },
		new GSUB { source = "\u0925\u094d", dest = "\uf347" },
		new GSUB { source = "\u0927\u094d", dest = "\uf348" },
		new GSUB { source = "\u0928\u094d", dest = "\uf34a" },
		new GSUB { source = "\u092a\u094d", dest = "\uf34b" },
		new GSUB { source = "\u092b\u094d", dest = "\uf34c" },
		new GSUB { source = "\u092c\u094d", dest = "\uf34d" },
		new GSUB { source = "\u092d\u094d", dest = "\uf34e" },
		new GSUB { source = "\u092e\u094d", dest = "\uf350" },
		new GSUB { source = "\u092f\u094d", dest = "\uf351" },
		new GSUB { source = "\u0931\u094d", dest = "\uf352" },
		new GSUB { source = "\u0932\u094d", dest = "\uf353" },
		new GSUB { source = "\u0933\u094d", dest = "\uf355" },
		new GSUB { source = "\u0935\u094d", dest = "\uf356" },
		new GSUB { source = "\u0936\u094d", dest = "\uf357" },
		new GSUB { source = "\u0937\u094d", dest = "\uf35a" },
		new GSUB { source = "\u0938\u094d", dest = "\uf35b" },
		new GSUB { source = "\u0939\u094d", dest = "\uf35c" },
		new GSUB { source = "\u0915\u0948\u0902", dest = "\u0915\u0948\uf01b" },
		new GSUB { source = "\u0932\u0902", dest = "\u0932\uf01b" },
		new GSUB { source = "\u0922\u093c", dest = "\u095d" },
		new GSUB { source = "\u0915\u0941", dest = "\u0915\uf018" },
		new GSUB { source = "\u0915\u0942", dest = "\u0915\uf019" },
		new GSUB { source = "\u0930\u0942", dest = "\uf555" },
		new GSUB { source = "\u0939\u0941", dest = "\uf564" },
		new GSUB { source = "\u0939\u0942", dest = "\uf565" },
		new GSUB { source = "\u0915\u0947", dest = "\u0915\uf00d" },
		new GSUB { source = "\u091f\u0947", dest = "\u091f\uf008" },
		new GSUB { source = "\u091f\uf1b0\u0947", dest = "\u091f\uf1b0\uf008" },
		new GSUB { source = "\u0930\u0947", dest = "\u0930\uf008" },
		new GSUB { source = "\u0932\u0947", dest = "\u0932\uf00a" },
		new GSUB { source = "\u0933\u0947", dest = "\u0933\uf00a" },
		new GSUB { source = "\uf375\u0947", dest = "\uf375\uf00d" },
		new GSUB { source = "\u0915\u0948", dest = "\u0915\uf015" },
		new GSUB { source = "\uf33a\u0932\u093f", dest = "\uf31e\uf33a\u0932" },
		new GSUB { source = "\uf35b\u091f\u093f", dest = "\uf31e\uf35b\u091f" },
		new GSUB { source = "\uf353\u0915\u093f", dest = "\uf31e\uf353\u0915" },
		new GSUB { source = "\u0915\u093f", dest = "\uf311\u0915" },
		new GSUB { source = "\u0916\u093f", dest = "\uf31e\u0916" },
		new GSUB { source = "\u0917\u093f", dest = "\uf30c\u0917" },
		new GSUB { source = "\u091a\u093f", dest = "\uf314\u091a" },
		new GSUB { source = "\u091c\u093f", dest = "\uf31e\u091c" },
		new GSUB { source = "\u091f\u093f", dest = "\uf310\u091f" },
		new GSUB { source = "\u0920\u093f", dest = "\uf30f\u0920" },
		new GSUB { source = "\u0921\u093f", dest = "\uf30f\u0921" },
		new GSUB { source = "\u0924\u093f", dest = "\uf312\u0924" },
		new GSUB { source = "\u0926\u093f", dest = "\uf30f\u0926" },
		new GSUB { source = "\u0927\u093f", dest = "\uf312\u0927" },
		new GSUB { source = "\u0928\u093f", dest = "\uf312\u0928" },
		new GSUB { source = "\u092b\u093f", dest = "\uf311\u092b" },
		new GSUB { source = "\u092c\u093f", dest = "\uf311\u092c" },
		new GSUB { source = "\u092d\u093f", dest = "\uf314\u092d" },
		new GSUB { source = "\u092e\u093f", dest = "\uf314\u092e" },
		new GSUB { source = "\u0930\u093f", dest = "\uf30d\u0930" },
		new GSUB { source = "\u0932\u093f", dest = "\uf314\u0932" },
		new GSUB { source = "\u0935\u093f", dest = "\uf311\u0935" },
		new GSUB { source = "\u0936\u093f", dest = "\uf317\u0936" },
		new GSUB { source = "\u0938\u093f", dest = "\uf319\u0938" },
		new GSUB { source = "\u0939\u093f", dest = "\uf311\u0939" },
		new GSUB { source = "\uf337\u093f", dest = "\uf319\uf337" },
		new GSUB { source = "\uf376\u093f", dest = "\uf311\uf376" },
		new GSUB { source = "\u0940\u0901", dest = "\u0940\uf300" },
		new GSUB { source = "\u0915\u0940", dest = "\u0915\uf32a" },
		new GSUB { source = "\u091f\u0940", dest = "\u091f\uf323" },
		new GSUB { source = "\u092b\u0940", dest = "\u092b\uf32a" },
		new GSUB { source = "\u0930\u0940", dest = "\u0930\uf322" },
		new GSUB { source = "\u0932\u0940", dest = "\u0932\uf325" },
		new GSUB { source = "\uf363\u0940", dest = "\uf363\uf32a" }
	};

	public static string Correct(string text)
	{
		for (int i = 0; i < gsub.Length; i++)
		{
			text = text.Replace(gsub[i].source, gsub[i].dest);
		}
		string[] words = text.Split(' ');
		for (int j = 0; j < words.Length; j++)
		{
			char[] letters = words[j].ToCharArray();
			for (int k = 1; k < letters.Length; k++)
			{
				// move the "i" matra (U+093F) in front of the consonant it belongs to
				if (letters[k] == (char)0x093f)
				{
					char previous = letters[k - 1];
					letters[k - 1] = (char)0x093f;
					letters[k] = previous;
				}
			}
			words[j] = new string(letters);
		}
		return string.Join(" ", words);
	}
}
