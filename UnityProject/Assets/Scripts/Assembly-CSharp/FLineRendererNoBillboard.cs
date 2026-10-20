using System.Collections.Generic;
using UnityEngine;

public class FLineRendererNoBillboard : MonoBehaviour
{
	public enum SimulationSpaceTypes
	{
		World = 0,
		Local = 1
	}

	public Material Material;

	public Gradient ColorOverTime;

	public Gradient ColorOverLength;

	public AnimationCurve WidthScaleOverTime;

	public float Width;

	public float Lifetime;

	public float EmitDistance;

	public float MaxLength;

	public float TextureScrollSpeed;

	public bool TextureScrollReverse;

	public float TextureScale;

	public Vector3 UpVector;

	public SimulationSpaceTypes SimulationSpace;

	protected List<FLineRendererSegment> mSegments;

	protected Mesh mMesh;

	protected float mStartTime;

	protected int mOldestSegmentOffset;

	protected bool mMeshDirty;

	public void SetSegmentCount(int count)
	{
	}

	public void SetPath(FixVector3 startPosition, FixVector3 endPosition)
	{
	}

	public void SetSegment(int index, Vector3 pos)
	{
	}

	private void Start()
	{
	}

	protected void EmitInitialSegments()
	{
	}

	private void Update()
	{
	}

	public void UpdateMesh()
	{
	}

	protected void UpdateSegments()
	{
	}

	public void OnDestroy()
	{
	}

	public bool IsDone()
	{
		return false;
	}

	protected float GetCurrentTime()
	{
		return 0f;
	}

	public void SetSimpleColor(Color color)
	{
	}
}
