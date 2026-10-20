using System;
using System.Collections.Generic;
using UnityEngine;

public class FRaycaster
{
	protected RaycastHit[] mHits;

	protected Ray mRay;

	protected int mLayerMask;

	protected List<FRaycastResult> mRaycastResults;

	protected List<GameObject> mGOResults;

	protected bool mSort;

	public Func<Camera> GetCamera;

	public List<FRaycastResult> RaycastResults => null;

	public List<GameObject> GOResults => null;

	public int LayerMask
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public bool Sort
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public FRaycaster(int layerMask)
	{
	}

	protected virtual void ClearLists()
	{
	}

	protected virtual void AddGameObjectResult(GameObject go)
	{
	}

	public void GetHits(Vector3 screenPoint)
	{
	}
}
