using System.Collections.Generic;
using UnityEngine;

public class FGradientNode : FFacetNode
{
	protected float mWidth;

	protected float mHeight;

	protected List<FGradientPoint> mPoints;

	protected List<Vector2> mLocalVertices;

	protected List<Color> mLocalColors;

	protected bool mIsVertical;

	protected bool mIsMeshDirty;

	protected bool mAreLocalVerticesDirty;

	public bool IsVertical
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

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

	public FGradientNode(bool isVertical)
	{
	}

	public override void Invalidate()
	{
	}

	public void AddPoint(float location, Color color)
	{
	}

	public void ClearPoints()
	{
	}

	public override void RedrawInternal(bool shouldForceDirty, bool shouldUpdateDepth, bool wasMatrixDirty, bool wasAlphaDirty)
	{
	}

	public override void HandleAddedToStage()
	{
	}

	protected override void HandleUpdate()
	{
	}

	protected virtual void UpdateLocalVertices()
	{
	}

	public override void PopulateRenderLayer()
	{
	}

	public override void SelfResizeExtents()
	{
	}
}
