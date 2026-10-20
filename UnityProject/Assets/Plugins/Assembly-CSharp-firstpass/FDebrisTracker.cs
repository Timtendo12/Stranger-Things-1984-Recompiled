using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FDebrisTracker @ 0x16486e8..0x16494dc, Ghidra vaddr).
public class FDebrisTracker : MonoBehaviour
{
	protected static FDebrisTracker mInstance;

	protected LinkedList<GameObject> mItems = new LinkedList<GameObject>();

	protected int mItemCap;

	protected const float cFadeTime = 0.2f;

	public static bool IsInitialized => mInstance != null;

	public static void Init(int itemCap)
	{
		GameObject gameObject = new GameObject("zzzDebrisTracker");
		mInstance = gameObject.AddComponent<FDebrisTracker>();
		Object.DontDestroyOnLoad(gameObject);
		mInstance.mItemCap = itemCap;
	}

	public static void AddItem(GameObject item)
	{
		if (mInstance == null)
		{
			FAssert.Fail("FDebrisTracker not initialized");
			return;
		}
		mInstance.mItems.AddLast(item);
		item.AddOnDestroyEvent(RemoveItem);
		mInstance.CheckCap();
	}

	public static void RemoveItem(GameObject item)
	{
		if (mInstance == null)
		{
			FAssert.Fail("FDebrisTracker not initialized");
			return;
		}
		mInstance.mItems.Remove(item);
		item.RemoveOnDestroyEvent(RemoveItem);
	}

	public static void ClearAll(bool fadeOut)
	{
		if (mInstance == null)
		{
			FAssert.Fail("FDebrisTracker not initialized");
			return;
		}
		foreach (GameObject mItem in mInstance.mItems)
		{
			if (!fadeOut)
			{
				Object.Destroy(mItem);
			}
			else
			{
				mItem.FadeLifespan(0f, 0.2f, 0f, false);
			}
		}
	}

	protected void CheckCap()
	{
		while (mItems.Count > mItemCap)
		{
			GameObject gameObject = mItems.First();
			if (gameObject != null)
			{
				gameObject.FadeLifespan(0f, 0.2f, 0f, false);
			}
			mItems.RemoveFirst();
		}
	}

	protected void OnDestroy()
	{
		foreach (GameObject mItem in mItems)
		{
			mItem.RemoveOnDestroyEvent(RemoveItem);
		}
	}

	private void Update()
	{
		if (!(mInstance != null))
		{
			return;
		}
		foreach (GameObject mItem in mItems)
		{
			if (mItem.transform.position.y < -500f)
			{
				mItem.FadeLifespan(0f, 0.2f, 0f, false);
			}
		}
	}
}
