using UnityEngine;

public class Example : MonoBehaviour
{
	public bool debugPoint;

	public Vector3 debugPoint_Position;

	public float debugPoint_Scale;

	public Color debugPoint_Color;

	public bool debugBounds;

	public Vector3 debugBounds_Position;

	public Vector3 debugBounds_Size;

	public Color debugBounds_Color;

	public bool debugCircle;

	public Vector3 debugCircle_Up;

	public float debugCircle_Radius;

	public Color debugCircle_Color;

	public bool debugWireSphere;

	public float debugWireSphere_Radius;

	public Color debugWireSphere_Color;

	public bool debugCylinder;

	public Vector3 debugCylinder_End;

	public float debugCylinder_Radius;

	public Color debugCylinder_Color;

	public bool debugCone;

	public Vector3 debugCone_Direction;

	public float debugCone_Angle;

	public Color debugCone_Color;

	public bool debugArrow;

	public Vector3 debugArrow_Direction;

	public Color debugArrow_Color;

	public bool debugCapsule;

	public Vector3 debugCapsule_End;

	public float debugCapsule_Radius;

	public Color debugCapsule_Color;

	private void OnDrawGizmos()
	{
	}

	private void Update()
	{
	}
}
