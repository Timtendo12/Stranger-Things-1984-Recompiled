using UnityEngine;

[ExecuteInEditMode]
public class Decal : MonoBehaviour
{
	public enum Kind
	{
		DiffuseOnly = 0,
		NormalsOnly = 1,
		DiffuseAndNormals = 2,
		DiffuseSpecOverride = 3,
		EmissiveOnly = 4
	}

	public Kind m_Kind;

	public Material m_Material;

	public void OnEnable()
	{
	}

	public void Start()
	{
	}

	public void OnDisable()
	{
	}

	private void DrawGizmo(bool selected)
	{
	}

	public void OnDrawGizmos()
	{
	}

	public void OnDrawGizmosSelected()
	{
	}
}
