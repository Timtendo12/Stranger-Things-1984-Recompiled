using UnityEngine;

[ExecuteInEditMode]
public class FTerrainDecal : MonoBehaviour, ISerializationCallbackReceiver
{
	public float Width;

	public float Height;

	public const float cYOffset = 0.1f;

	protected Mesh mMesh;

	protected MeshFilter mMeshFilter;

	protected MeshRenderer mMeshRenderer;

	public void Awake()
	{
	}

	public void Update()
	{
	}

	public void Init()
	{
	}

	public void UpdateMesh()
	{
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
	}
}
