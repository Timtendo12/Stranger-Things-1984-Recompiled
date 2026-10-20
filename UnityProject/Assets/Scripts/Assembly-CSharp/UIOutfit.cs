// Rewritten from libil2cpp.so 1.0.605 (UIOutfit.UIOutfit, Ghidra vaddr).
public struct UIOutfit
{
	public string DefaultOutfit;

	public string NewOutfit;

	public string LockedOutfitButton;

	public string UnlockedOutfitButton;

	public string SelectedOutfitIcon;

	public string UnselectedOutfitIcon;

	public UIOutfit(string defaultOutfitName, string newOutfitName, string lockedButton, string unlockedButton, string selectedIcon, string unselectedIcon)
	{
		DefaultOutfit = defaultOutfitName;
		NewOutfit = newOutfitName;
		LockedOutfitButton = lockedButton;
		UnlockedOutfitButton = unlockedButton;
		SelectedOutfitIcon = selectedIcon;
		UnselectedOutfitIcon = unselectedIcon;
	}
}
