using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class DeferredDecalRenderer : MonoBehaviour
{
	public Mesh m_CubeMesh;

	private Dictionary<Camera, CommandBuffer> m_Cameras;

	public void OnEnable()
	{
	}

	public void OnDisable()
	{
	}

	public void OnEditorUpdate()
	{
	}

	public void OnPreRender()
	{
	}

	public void SetupCommandBuffers(Camera cam)
	{
	}
}
