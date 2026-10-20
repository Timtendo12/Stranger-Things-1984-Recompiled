using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FKeyboardOptions @ 0x169e658..0x169ea3c, Ghidra vaddr).
public struct FKeyboardOptions
{
	public TouchScreenKeyboardType KeyboardType;

	public bool Secure;

	public bool Multiline;

	public bool Autocorrect;

	public static FKeyboardOptions Default { get; private set; }

	public static FKeyboardOptions DefaultNoAutocorrect { get; private set; }

	public static FKeyboardOptions DefaultEmail { get; private set; }

	public static FKeyboardOptions DefaultPassword { get; private set; }

	static FKeyboardOptions()
	{
		Default = new FKeyboardOptions { Autocorrect = true };
		// Shipped code stores the same value (Autocorrect = true) for DefaultNoAutocorrect.
		DefaultNoAutocorrect = new FKeyboardOptions { Autocorrect = true };
		DefaultEmail = new FKeyboardOptions { KeyboardType = (TouchScreenKeyboardType)7 };
		DefaultPassword = new FKeyboardOptions { Secure = true };
	}
}
