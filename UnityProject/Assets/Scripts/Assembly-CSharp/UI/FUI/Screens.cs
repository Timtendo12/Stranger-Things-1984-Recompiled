using UnityEngine;

namespace FUI
{
	// Rewritten from libil2cpp.so 1.0.605 (FUI.Screens @ 0x17831bc..0x178329c, Ghidra vaddr).
	internal static class Screens
	{
		public const float cLetterboxHeight = 150f;

		public static Vector2 LetterboxSize => new Vector2(Futile.Screen.Width, 150f);

		public static float ScreenHeightMinusLetterboxes => Futile.Screen.Height - 150f - 150f;
	}
}
