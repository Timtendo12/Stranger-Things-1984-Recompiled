using System;
using UnityEngine;

namespace FUI
{
	// Rewritten from libil2cpp.so 1.0.605 (FUI.Buttons @ 0x1781ea8..0x1782934, Ghidra vaddr).
	internal static class Buttons
	{
		private const float cHUDSquareButtonSize = 72f;

		private const float cBackButtonOffset = 32f;

		private const float cHUDSquareButtonOffset = 16f;

		private const string cButtonFont = "DefaultStyle";

		private const float cButtonPointSize = 48f;

		private const float cBackButtonSize = 72f;

		private const string cButtonBackgroundTexture = "UIBox001.png";

		private const string cButtonBackgroundTextureHighlight = "UIBox001.png";

		private static readonly Color cButtonForegroundColor = FUI.Colors.UITitle;

		private const string cBuyPackButtonFont = "DefaultStyle";

		private const float cBuyPackButtonPointSize = 48f;

		public static Vector2 HUDSquareButtonSize => new Vector2(72f, 72f);

		public static Vector2 HUDSquareButtonOffset => new Vector2(16f, 16f);

		public static Vector2 BackButtonOffset => new Vector2(32f, -32f);

		public static Vector2 BackButtonSize => new Vector2(72f, 72f);

		public static Color ButtonForegroundColor => cButtonForegroundColor;

		public static string ButtonFont => "DefaultStyle";

		public static float ButtonPointSize => 48f;

		public static string ButtonBackgroundTexture => "UIBox001.png";

		public static string ButtonBackgroundTextureHighlight => "UIBox001.png";

		public static FUIButton AllocateStandardButton(string stringId, Action<FUIButton, FTouch> onRelease)
		{
			return AllocateStandardButton(stringId, onRelease, null, null, null, false);
		}

		public static FUIButton AllocateStandardButton(string stringId, Action<FUIButton, FTouch> onRelease, Action<FNode, string, bool> onTab, Action<FNode, string> onEscape, Action<FNode, string> onEnter, bool wantsFocus)
		{
			FUIButton button = new FUIButton("Button." + stringId);
			button.SetBackground("UIBox001.png");
			button.FocusBackgroundSprite = "UIBox001.png";
			button.NonFocusBackgroundSprite = "UIBox001.png";
			button.SetForegroundColor(cButtonForegroundColor);
			FAssert.That(onRelease != null, "AllocateStandardButton() called with null onRelease!");
			if (onRelease != null)
			{
				button.AddHandleRelease(onRelease);
			}
			if (onTab != null)
			{
				button.AddHandleTab(onTab);
			}
			if (onEscape != null)
			{
				button.AddHandleEscape(onEscape);
			}
			if (onEnter != null)
			{
				button.AddHandleEnter(onEnter);
			}
			button.WantsFocus = wantsFocus;
			string text = FLoc.GetString(stringId, true);
			if (string.IsNullOrEmpty(text))
			{
				text = stringId;
			}
			button.SetText(text, "DefaultStyle", 48f, 14f);
			return button;
		}

		public static FUIButton AllocateStandardButton2(string stringId, Action<FUIButton, FTouch> onRelease)
		{
			return AllocateStandardButton2(stringId, onRelease, null, null, null, false);
		}

		public static FUIButton AllocateStandardButton2(string stringId, Action<FUIButton, FTouch> onRelease, Action<FNode, string, bool> onTab, Action<FNode, string> onEscape, Action<FNode, string> onEnter, bool wantsFocus)
		{
			FUIButton button = new FUIButton("Button." + stringId);
			button.SetBackground("UIBox001.png");
			button.FocusBackgroundSprite = string.Empty;
			button.NonFocusBackgroundSprite = string.Empty;
			button.SetForegroundColor(FColorNames.Get("UITitle"));
			if (onRelease != null)
			{
				button.AddHandleRelease(onRelease);
			}
			if (onTab != null)
			{
				button.AddHandleTab(onTab);
			}
			if (onEscape != null)
			{
				button.AddHandleEscape(onEscape);
			}
			if (onEnter != null)
			{
				button.AddHandleEnter(onEnter);
			}
			button.WantsFocus = wantsFocus;
			string text = FLoc.GetString(stringId, true);
			if (string.IsNullOrEmpty(text))
			{
				text = stringId;
			}
			button.SetText(text, "DefaultStyle", 48f, 14f);
			return button;
		}

		public static FUIToggleButton AllocateToggleButton(string stringId, bool initialValue, float textPointSize, float textOffset, Action<FUIButton, FTouch> handler)
		{
			FUIToggleButton button = new FUIToggleButton("Button." + stringId);
			button.SetForeground("ButtonOn.png");
			button.SetForegroundUnchecked("ButtonOff.png");
			button.AddHandleRelease(handler);
			string text = FLoc.GetString(stringId, false);
			if (!string.IsNullOrEmpty(text))
			{
				stringId = text;
			}
			button.SetText(stringId, "DefaultStyle", textPointSize, true, textOffset);
			button.Checked = initialValue;
			return button;
		}

		public static void LayoutAndStackButton(FUIButton button, FNodeStack addToStack)
		{
			FAssert.That(button != null, "LayoutButtonAndStack called with null button!");
			FAssert.That(addToStack != null, "LayoutButtonAndStack called with null addToStack!");
			Vector2 size = FPageShell.cStackButtonSize;
			if (size == Vector2.zero)
			{
				// FPageShell's static ctor values (0x17ae284) are not initialised in FPageShell.cs yet; use the shipped constants
				size = new Vector2(420f, 108f);
			}
			addToStack.StackChild(button, size);
		}
	}
}
