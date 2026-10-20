using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPageCharacterPicker @ 0x17f0888..0x17f3740, Ghidra vaddr).
public class FPageCharacterPicker : FPageLegend
{
	protected FLabel mCharNameLabel;

	protected FLabel mCharClassLabel;

	protected FSprite mCharWeaponSprite;

	protected FLabel mCharWeaponAmmo;

	protected FLabel mCharWeaponLabel;

	protected FSprite mCharUpgradeSprite;

	protected FLabel mCharUpgradeLabel;

	protected FUIButton[] mCharacters;

	protected FNodeStack mCharacterStack;

	protected FUIButton[] mCharacterOutfits;

	protected FNodeStack mCharacterOutfitStack;

	protected int mCurrCharIndex;

	protected Vector2 mCharSize;

	public FPageCharacterPicker(string name)
		: base(name)
	{
		mCharSize = Vector2.zero;
		mTitle.Text = FLoc.GetString("SelectCharacter");
		mCharClassLabel = new FLabel(Name + ".CharClass", "DefaultStyle", string.Empty, 24f, true, true, false);
		mCharClassLabel.Justify(Sides.Left, Sides.CenterY);
		mCharClassLabel.SetParent(mBackground);
		mCharWeaponSprite = new FSprite(Name + ".CharWeaponSprite", "BlackSquare.png");
		mCharWeaponSprite.SetParent(mBackground);
		mCharWeaponAmmo = new FLabel(Name + ".CharacterAmmo", "DefaultStyle", string.Empty, 16f, false, true, false);
		mCharWeaponAmmo.Justify(Sides.Right, Sides.Bottom);
		mCharWeaponAmmo.SetParent(mCharWeaponSprite);
		mCharWeaponLabel = new FLabel(Name + ".CharWeapon", "DefaultStyle", string.Empty, 24f, true, true, false);
		mCharWeaponLabel.Justify(Sides.Left, Sides.CenterY);
		mCharWeaponLabel.SetParent(mBackground);
		mCharUpgradeSprite = new FSprite(Name + ".CharWeaponSprite", "BlackSquare.png");
		mCharUpgradeSprite.SetParent(mBackground);
		mCharUpgradeLabel = new FLabel(Name + ".CharUpgrade", "DefaultStyle", string.Empty, 24f, true, true, false);
		mCharUpgradeLabel.Justify(Sides.Left, Sides.CenterY);
		mCharUpgradeLabel.SetParent(mBackground);
		mCharacterStack = new FNodeStack(Name + ".CharacterStack", FNodeStack.Modes.CenteredX, 8f);
		mCharacterStack.SetParent(mDialog);
		mCharacterOutfitStack = new FNodeStack(Name + ".CharacterOutfitStack", FNodeStack.Modes.CenteredX, 8f);
		mCharacterOutfitStack.SetParent(mDialog);
		mCharNameLabel = new FLabel(Name + ".CharName", "DefaultStyle", string.Empty, 16f, true, true, false);
		mCharNameLabel.Color = new Color(0f, 1f, 1f, 1f);
		mCharNameLabel.SetParent(mCharacterStack);
		mCloseButton.SetText(FLoc.GetString("SelectButton"));
	}

	public override void LayoutPage()
	{
		base.LayoutPage();
		float margin = mBorderMargin;
		float width = (mBackgroundSize.x + margin * -4f - 64f) - mCloseButtonSize.x;
		mCharUpgradeSprite.DockInside(mBackground, InnerDock.BottomLeft, 64f, 64f, margin, margin);
		mCharUpgradeLabel.DockOutside(mCharUpgradeSprite, OuterDock.Right, width, 64f, margin, 0f);
		mCharWeaponSprite.DockOutside(mCharUpgradeSprite, OuterDock.Top, 64f, 64f, 0f, 0f);
		mCharWeaponAmmo.DockInside(mCharWeaponSprite, InnerDock.Center, 64f, 64f, 0f, 0f);
		mCharWeaponLabel.DockOutside(mCharWeaponSprite, OuterDock.Right, width, 64f, margin, 0f);
		mCharClassLabel.DockOutside(mCharWeaponLabel, OuterDock.TopAlignLeft, width, 64f, 0f, 0f);
		LayoutCharacterStack();
	}

	protected void LayoutCharacterStack()
	{
		if (mCharacters == null)
		{
			InitCharacterList();
		}
		mCharSize = new Vector2(128f, 128f);
		float width = mBackgroundSize.x - (mBorderMargin + mBorderMargin);
		if (mCharacters.Length != 0)
		{
			int count = mCharacters.Length;
			int gaps = count * 8 - 8;
			if (width < (float)(gaps + count * 128))
			{
				float size = (width - (float)gaps) / (float)count;
				mCharSize = new Vector2(size, size);
			}
		}
		mCharacterStack.ClearStack();
		mCharacterStack.DockOutside(mTitle, OuterDock.Bottom, width, 128f, 0f, -48f);
		mCharacterOutfitStack.ClearStack();
		mCharacterOutfitStack.DockOutside(mTitle, OuterDock.Bottom, width, 128f, 0f, 0f - (mCharSize.y + 48f));
		for (int i = 0; i < mCharacters.Length; i++)
		{
			mCharacterStack.StackChild(mCharacters[i], mCharSize);
		}
		for (int j = 0; j < mCharacterOutfits.Length; j++)
		{
			mCharacterOutfitStack.StackChild(mCharacterOutfits[j], mCharSize);
		}
		mCharacterOutfitStack.SelfResizeExtents();
		mCharacterStack.SelfResizeExtents();
		SetCharacter(mCurrCharIndex, false);
	}

	public void InitCharacterList()
	{
		// port guard: the shipped code dereferences FApp.ProtoUnitManager unconditionally (HUD pages are laid out at creation time)
		FProtoUnit[] characters = ((FApp.ProtoUnitManager != null) ? FApp.ProtoUnitManager.GetProtoUnitsWithTag("PlayerCharacter") : null) ?? new FProtoUnit[0];
		mCharacters = new FUIButton[characters.Length];
		mCharacterOutfits = new FUIButton[mCharacters.Length];
		for (int i = 0; i < characters.Length; i++)
		{
			FProtoUnit character = characters[i];
			int tempIndex = i;
			mCharacters[i] = new FUIButton(string.Empty);
			mCharacters[i].Name = character.Name;
			mCharacterOutfits[i] = new FUIButton(character.Name);
			if (!character.CheckUnlockRequirements())
			{
				mCharacters[i].SetBackground(character.Name + "_Icon_White.png");
				mCharacters[i].SetBackgroundColor(FColorNames.Get("DarkGray"));
				mCharacters[i].Data = 0;
				mCharacterOutfits[i].Visible = false;
			}
			else
			{
				mCharacters[i].SetBackground("Button002.png");
				mCharacters[i].SetBackgroundColor(new Color(1f, 1f, 1f, 1f));
				mCharacters[i].SetForeground(OutfitHandler.GetUICharacterImageName(character.Name));
				mCharacters[i].Data = 1;
				mCharacterOutfits[i].SetBackground("Button002.png");
				mCharacterOutfits[i].SetForeground(OutfitHandler.GetUIOutfitButtonImageName(character.Name));
				mCharacterOutfits[i].Visible = OutfitHandler.IsOutfitUnlocked(character.Name);
			}
			mCharacters[i].AddHandleRelease(HandleCharacterButton);
			mCharacterOutfits[i].AddHandleRelease(delegate(FUIButton button, FTouch touch)
			{
				// <InitCharacterList>g__HandleOutfitDisplay|0
				OutfitHandler.SetCurrentOutfitForCharacter(character.Name);
				mCharacters[tempIndex].SetForeground(OutfitHandler.GetUICharacterImageName(character.Name));
				FApp.SoundManager.PlaySoundSet("UIButton");
			});
		}
		mCurrCharIndex = 0;
	}

	protected void SetCharacter(int charIndex, bool closeIfSame)
	{
		if (charIndex < 0 || mCharacters.Length <= charIndex)
		{
			return;
		}
		if (closeIfSame && mCurrCharIndex == charIndex)
		{
			ClosePage("UI.CharSelect.Select");
			return;
		}
		mCurrCharIndex = charIndex;
		FUIButton button = mCharacters[charIndex];
		FProtoUnit unit = FApp.ProtoUnitManager.GetProtoUnit(button.Name);
		FProtoWeapon weapon = unit?.GetProtoWeapon();
		if (unit == null || weapon == null)
		{
			mCharWeaponSprite.SetSprite("BlackSquare.png");
		}
		else
		{
			string icon = weapon.IconOverride;
			if (!icon.Valid())
			{
				icon = button.Name + "_Icon_Weapon.png";
			}
			mCharWeaponSprite.SetSprite(icon);
			if (unit.AlternateWeapons != null)
			{
				FProtoWeapon alternate = null;
				foreach (string alternateName in unit.AlternateWeapons)
				{
					FProtoWeapon candidate = FApp.ProtoWeaponManager.GetProtoWeapon(alternateName);
					if (candidate.HasAmmoResource)
					{
						alternate = candidate;
						break;
					}
				}
				if (alternate != null)
				{
					weapon = alternate;
				}
			}
			if (weapon.HasAmmoResource)
			{
				mCharWeaponAmmo.SetParent(mCharWeaponSprite);
				int ammo = FApp.PlayerStats.GetAmmoAmount(weapon.AmmoResource);
				mCharWeaponAmmo.Text = ammo.ToString();
			}
			else
			{
				mCharWeaponAmmo.Unparent();
			}
		}
		mCharUpgradeSprite.SetSprite("BlackSquare.png");
		for (int i = 0; i < mCharacters.Length; i++)
		{
			if (mCharacters[i].Data != 0)
			{
				mCharacters[i].SetBackground((i == mCurrCharIndex) ? "Button003.png" : "Button002.png");
			}
		}
		mCharClassLabel.Text = FLoc.GetString(button.Name + ".Class");
		mCharWeaponLabel.Text = FLoc.GetString(button.Name + ".Weapon");
		mCharUpgradeLabel.Text = FLoc.GetString(button.Name + ".Upgrade");
		mCharNameLabel.Text = FLoc.GetString(button.Name + ".Name");
		mCharNameLabel.DockOutside(button, OuterDock.Top, mCharSize.x, 48f, 0f, 0f);
	}

	protected void SetLockedCharacter(int charIndex)
	{
		FUIButton button = mCharacters[charIndex];
		for (int i = 0; i < mCharacters.Length; i++)
		{
			if (mCharacters[i].Data != 0)
			{
				mCharacters[i].SetBackground((charIndex == i) ? "Button003.png" : "Button002.png");
			}
		}
		mCharWeaponAmmo.Unparent();
		mCharWeaponSprite.SetSprite("BlackSquare.png");
		mCharUpgradeSprite.SetSprite("BlackSquare.png");
		mCharClassLabel.Text = FLoc.GetString("LockedCharacter.Title");
		mCharWeaponLabel.Text = FLoc.GetString(button.Name + ".Locked.Details");
		mCharUpgradeLabel.Text = string.Empty;
		mCharNameLabel.Text = FLoc.GetString("LockedCharacter.Name");
		mCharNameLabel.DockOutside(button, OuterDock.Top, mCharSize.x, 48f, 0f, 0f);
	}

	protected void HandleCharacterButton(FUIButton button, FTouch touch)
	{
		string name = button.Name;
		if (!name.Valid())
		{
			return;
		}
		FProtoUnit unit = FApp.ProtoUnitManager.GetProtoUnit(name);
		if (unit == null)
		{
			return;
		}
		for (int i = 0; i < mCharacters.Length; i++)
		{
			if (mCharacters[i] == button)
			{
				if (button.Data == 0)
				{
					FApp.SoundManager.PlaySoundSet("UIFail");
					SetLockedCharacter(i);
					return;
				}
				if (mCurrCharIndex != i)
				{
					FApp.SoundManager.PlaySoundSet("UISuccess");
				}
				SetCharacter(i, true);
				return;
			}
		}
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		FApp.SoundManager.PlaySoundSet("UI.CharSelect.Open");
		for (int i = 0; i < mCharacters.Length; i++)
		{
			if (mCharacters[i].Name == FApp.PlayerStats.CurrentCharacter)
			{
				SetCharacter(i, false);
				return;
			}
		}
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		FUIButton button = mCharacters[mCurrCharIndex];
		FProtoUnit unit = FApp.ProtoUnitManager.GetProtoUnit(button.Name);
		if (unit != null)
		{
			FApp.World.SetPlayerCharacter(unit, false);
		}
	}
}
