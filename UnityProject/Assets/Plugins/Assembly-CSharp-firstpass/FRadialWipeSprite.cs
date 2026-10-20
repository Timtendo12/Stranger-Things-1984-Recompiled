using UnityEngine;

public class FRadialWipeSprite : FSprite
{
	protected float _baseAngle;

	protected float _percentage;

	protected bool _isClockwise;

	protected Vector2[] _meshVertices;

	protected Vector2[] _uvVertices;

	public float baseAngle
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public float percentage
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public bool isClockwise
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public FRadialWipeSprite(string elementName, bool isClockwise, float baseAngle, float percentage)
	{
	}

	private void CalculateTheRadialVertices()
	{
	}

	public override void PopulateRenderLayer()
	{
	}
}
