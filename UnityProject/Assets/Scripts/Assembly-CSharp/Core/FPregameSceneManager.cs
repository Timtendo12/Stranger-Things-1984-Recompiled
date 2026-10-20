using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FPregameSceneManager @ 0x17c2904..0x17c326c, Ghidra vaddr).
public class FPregameSceneManager : MonoBehaviour
{
	public List<GameObject> AreaRoots = new List<GameObject>();

	public GameObject Scene;

	protected static FPregameSceneManager mInstance;

	protected static string mLastRequestedAreaName;

	protected GameObject mActiveArea;

	public static FPregameSceneManager Instance => mInstance;

	public void Start()
	{
		DeactivateAll();
		if (mInstance != null)
		{
			FAssert.Fail("Two pregame scene managers!");
		}
		mInstance = this;
		if (!string.IsNullOrEmpty(mLastRequestedAreaName))
		{
			ActivateArea(mLastRequestedAreaName);
		}
	}

	public static void DeactivateAll()
	{
		mLastRequestedAreaName = null;
		if (mInstance == null)
		{
			return;
		}
		for (int i = 0; i < mInstance.AreaRoots.Count; i++)
		{
			if (mInstance.AreaRoots[i] != null)
			{
				mInstance.AreaRoots[i].SetActive(false);
			}
		}
		mInstance.mActiveArea = null;
		if (mInstance.Scene != null)
		{
			mInstance.Scene.SetActive(false);
		}
	}

	public static void ActivateArea(string name)
	{
		mLastRequestedAreaName = name;
		if (mInstance != null)
		{
			mInstance.ActivateAreaInternal(name);
		}
	}

	protected void ActivateAreaInternal(string name)
	{
		if (mActiveArea != null && mActiveArea.name == name)
		{
			return;
		}
		if (mActiveArea != null)
		{
			mActiveArea.SetActive(false);
		}
		GameObject area = FindArea(name);
		if (area != null)
		{
			area.SetActive(true);
			mActiveArea = area;
		}
		else
		{
			mActiveArea = null;
		}
		if (mInstance != null && mInstance.Scene != null)
		{
			mInstance.Scene.SetActive(mActiveArea != null);
		}
	}

	public GameObject FindArea(string name)
	{
		for (int i = 0; i < AreaRoots.Count; i++)
		{
			if (AreaRoots[i] != null && AreaRoots[i].name == name)
			{
				return AreaRoots[i];
			}
		}
		return null;
	}

	public void OnDestroy()
	{
		mInstance = null;
	}
}
