using System.Collections.Generic;
using UnityEngine;

public class FAttachPositionOnly : MonoBehaviour
{
	public enum Options
	{
		None = 0,
		AlignToCameraInitial = 1,
		AlignToCameraConstant = 2,
		ProjectToGround = 3
	}

	public Transform AttachTo;

	public Vector3 Offset;

	public Vector3 CenterOffset;

	protected float mCameraOffset;

	public Options Option;

	public bool TrackRotation;

	public bool BillBoardTrackY;

	public bool VirtualAttachmentAutoKill;

	private bool mFirstBillboardUpdate;

	private bool mGotGroundPos;

	private Vector3 mGroundPos;

	public static HashSet<FAttachPositionOnly> sCameraBilboardInstances;

	public static HashSet<FAttachPositionOnly> sCameraOffsetInstances;

	public float CameraOffset
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public void SetBillboard(bool on)
	{
	}

	public static void ApplyCameraOffsets(Camera camera)
	{
	}

	public void LateUpdate()
	{
	}

	public void OnDestroy()
	{
	}
}
