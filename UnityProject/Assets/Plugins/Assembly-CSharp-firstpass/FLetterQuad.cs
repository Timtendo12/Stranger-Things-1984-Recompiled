using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FLetterQuad @ 0x1698d08..0x1698e00, Ghidra vaddr).
public class FLetterQuad
{
	public FCharInfo charInfo;

	public Rect rect;

	public Vector2 topLeft;

	public Vector2 topRight;

	public Vector2 bottomRight;

	public Vector2 bottomLeft;

	public Vector2 uvTopLeft;

	public Vector2 uvTopRight;

	public Vector2 uvBottomRight;

	public Vector2 uvBottomLeft;

	public float rotation;

	public FShader shader;

	public bool IsImage;

	public Vector2 Center => (bottomLeft + topRight) * 0.5f;

	public void CalculateVectors()
	{
		float x = rect.x;
		float y = rect.y;
		float right = x + rect.width;
		float top = y + rect.height;
		topLeft = new Vector2(x, top);
		topRight = new Vector2(right, top);
		bottomRight = new Vector2(right, y);
		bottomLeft = new Vector2(x, y);
	}

	public void ApplyOffest(float offsetX, float offsetY)
	{
		rect.x += offsetX;
		rect.y += offsetY;
		float x = rect.x;
		float y = rect.y;
		float right = x + rect.width;
		float top = y + rect.height;
		topLeft = new Vector2(x, top);
		topRight = new Vector2(right, top);
		bottomRight = new Vector2(right, y);
		bottomLeft = new Vector2(x, y);
	}

	public void CalculateVectors(float offsetX, float offsetY, float textScale)
	{
		float x = rect.x + offsetX;
		float y = rect.y + offsetY;
		float right = rect.x + rect.width + offsetX;
		float top = rect.y + rect.height + offsetY;
		topLeft = new Vector2(x, top);
		topRight = new Vector2(right, top);
		bottomRight = new Vector2(right, y);
		bottomLeft = new Vector2(x, y);
	}

	public void ResetDefaults()
	{
		shader = null;
		IsImage = false;
		rotation = 0f;
	}
}
