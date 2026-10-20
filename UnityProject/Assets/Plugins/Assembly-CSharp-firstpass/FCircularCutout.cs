using UnityEngine;

[ExecuteInEditMode]
public class FCircularCutout : MonoBehaviour
{
	public float Width;

	public float Height;

	public float StartAngleDegrees;

	public float EndAngleDegrees;

	protected Mesh mMesh;

	protected MeshFilter mMeshFilter;

	protected MeshRenderer mMeshRenderer;

	protected float mCutoffAngle;

	protected float mStartAngleRadians;

	protected float mEndAngleRadians;

	public void Awake()
	{
	}

	public void Update()
	{
	}

	protected Vector3 CalcPointForAngle(float angleRadians, out int nextCornerIndex)
	{
		nextCornerIndex = default;
		return default;
	}

	public void UpdateMesh()
	{
	}
}
