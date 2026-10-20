using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUITooltip @ 0x16a3f6c..0x16a9104, Ghidra vaddr).
public class FUITooltip : FNode
{
	private const string cBackgroundImage = "BlackSquare.png";

	private readonly Vector2 cDefaultBackgroundMargin = new Vector2(16f, 16f);

	private Vector2 mTooltipSize;

	private FSlicedSprite mBackground;

	private FSlicedSprite mBorder;

	private Vector2 mMargin;

	public Vector2 TooltipSize => mTooltipSize;

	public FUITooltip(string name)
		: base(name)
	{
		mBackground = new FSlicedSprite("BlackSquare.png");
		mBackground.SetParent(this);
		mUseConcatenatedColor = false;
		mDeferRedraw = true;
		mMargin = cDefaultBackgroundMargin;
	}

	public FUITooltip(string name, string background, string border, Vector2 margin)
		: base(name)
	{
		mBackground = new FSlicedSprite(background);
		mBackground.SetParent(this);
		mBorder = new FSlicedSprite(border);
		mBorder.SetParent(mBackground);
		mMargin = margin;
		mDeferRedraw = true;
		mUseConcatenatedColor = false;
	}

	public void Show(float textWidth, List<FUITooltipLine> textLines)
	{
		DockInside(base.Parent, InnerDock.Center, textWidth, 100f);
		mBackground.SetParent(this);
		mBackground.DockInside(this, InnerDock.Center, mTooltipSize.x, mTooltipSize.y);
		mBackground.RemoveChildren();
		if (mBorder != null)
		{
			mBorder.SetParent(mBackground);
			mBorder.DockInside(mBackground, InnerDock.Center, mTooltipSize.x, mTooltipSize.y);
		}
		float num = mMargin.y;
		float num2 = num;
		if (textWidth <= 0f)
		{
			textWidth = 0f;
		}
		if (textLines != null && textLines.Count > 0)
		{
			for (int i = 0; i < textLines.Count; i++)
			{
				FUITooltipLine fUITooltipLine = textLines[i];
				FLabel fLabel = new FLabel(fUITooltipLine.FontName, fUITooltipLine.Text, fUITooltipLine.PointSize, shrinkToFitContainer: false, fUITooltipLine.UsesMarkup);
				fLabel.SetParent(mBackground);
				fLabel.Color = fUITooltipLine.TextColor;
				fLabel.Wrap = fUITooltipLine.Wrap;
				fLabel.Justify(GetSideFromHorizSide(fUITooltipLine.JustifyX), Sides.CenterY);
				fLabel.DockInside(mBackground, InnerDock.Top, textWidth, 100f);
				float textHeight = fLabel.TextHeight;
				fLabel.DockInside(mBackground, InnerDock.Top, textWidth, textHeight, 0f, 0f - num2);
				num2 += fLabel.TextHeight;
			}
			num = mMargin.y;
		}
		mTooltipSize.y = num2 + num;
		mTooltipSize.x = textWidth + mMargin.x * 2f;
		mBackground.DockInside(this, InnerDock.Center, mTooltipSize.x, mTooltipSize.y);
		if (mBorder != null)
		{
			mBorder.DockInside(mBackground, InnerDock.Center, mTooltipSize.x, mTooltipSize.y);
		}
		MarkDirtyRecursive();
	}
}
