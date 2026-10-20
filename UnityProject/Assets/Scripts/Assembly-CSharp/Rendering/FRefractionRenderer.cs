using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class FRefractionRenderer : MonoBehaviour
{
	private Shader mBlurShader;

	private Material mMaterial;

	public static HashSet<FRefractHelper> RefractHelpers;

	protected static List<FRefractHelper> mHelpersToRemove;

	protected Dictionary<Camera, FRefractCameraEntry> mCameras;

	private const CameraEvent cCameraEvent = CameraEvent.AfterForwardAlpha;

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

	public static void AddHelper(FRefractHelper helper)
	{
	}

	public static void RemoveHelper(FRefractHelper helper)
	{
	}

	public void SetupCommandBuffers(Camera cam)
	{
	}
}
