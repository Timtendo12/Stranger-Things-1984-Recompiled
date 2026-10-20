using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUI.Colors getters, Ghidra vaddr): each returns FColorNames.Get("<Name>").
namespace FUI
{
	public static class Colors
	{
		public static Color Black => FColorNames.Get("Black");

		public static Color White => FColorNames.Get("White");

		public static Color Red => FColorNames.Get("Red");

		public static Color Green => FColorNames.Get("Green");

		public static Color Blue => FColorNames.Get("Blue");

		public static Color SpindleText => FColorNames.Get("SpindleText");

		public static Color UITitle => FColorNames.Get("UITitle");

		public static Color UITitleDark => FColorNames.Get("UITitleDark");

		public static Color UIEmphasis => FColorNames.Get("UIEmphasis");

		public static Color UIEmphasisDark => FColorNames.Get("UIEmphasisDark");

		public static Color UINormal => FColorNames.Get("UINormal");

		public static Color UIError => FColorNames.Get("UIError");

		public static Color UIBetterStat => FColorNames.Get("UIBetterStat");

		public static Color UIWorseStat => FColorNames.Get("UIWorseStat");

		public static Color UIDamage => FColorNames.Get("UIDamage");

		public static Color UIHeal => FColorNames.Get("UIHeal");

		public static Color UIDisabled => FColorNames.Get("UIDisabled");

		public static Color UISelf => FColorNames.Get("UISelf");

		public static Color UIAlly => FColorNames.Get("UIAlly");

		public static Color UIEnemy => FColorNames.Get("UIEnemy");

		public static Color LegendRed => FColorNames.Get("LegendRed");
	}
}
