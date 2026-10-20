using UnityEngine;

public class FRepeatSprite : FSprite
{
	protected float mWidth;

	protected float mHeight;

	protected float mScrollX;

	protected float mScrollY;

	protected float mTextureWidth;

	protected float mTextureHeight;

	public override Rect TextureRect => default;

	public override Rect LocalRect => default;

	public override float Width
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public override float Height
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public float ScrollX
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public float ScrollY
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public FRepeatSprite(string elementName, float width, float height, float scrollX, float scrollY)
	{
	}

	public override void HandleElementChanged()
	{
	}

	protected override void UpdateLocalVertices()
	{
	}

	public override void PopulateRenderLayer()
	{
	}
}
