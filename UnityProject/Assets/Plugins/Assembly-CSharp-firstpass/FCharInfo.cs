using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FCharInfo @ 0x168b828..0x168b838, Ghidra vaddr).
public class FCharInfo
{
	public char charID;

	public float x;

	public float y;

	public float width;

	public float height;

	public Vector2 uvTopLeft;

	public Vector2 uvTopRight;

	public Vector2 uvBottomRight;

	public Vector2 uvBottomLeft;

	public float offsetX;

	public float offsetY;

	public float xadvance;

	public float DynamicScaleMult = 1f;

	public FFont mFont;

	public void Init(CharacterInfo ci, FFont font, int dynamicFontSize, int dynamicFontRequestSize)
	{
		float scale = (float)dynamicFontSize / (float)dynamicFontRequestSize;
		int fontSize = font.mUnityFont.fontSize;
		DynamicScaleMult = scale;
		x = 0f;
		y = 0f;
		charID = (char)ci.index;
		width = ci.glyphWidth;
		height = ci.glyphHeight;
		uvTopLeft = ci.uvTopLeft;
		uvTopRight = ci.uvTopRight;
		uvBottomLeft = ci.uvBottomLeft;
		uvBottomRight = ci.uvBottomRight;
		offsetX = ci.minX;
		offsetY = (float)(int)((float)dynamicFontSize / (float)fontSize * (float)font.mUnityFont.ascent) - scale * (float)ci.maxY;
		mFont = font;
		width *= scale;
		height *= scale;
		xadvance = scale * (float)ci.advance;
		offsetX = scale * offsetX;
	}
}
