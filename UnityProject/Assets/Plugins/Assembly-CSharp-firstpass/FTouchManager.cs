using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Rewritten from libil2cpp.so 1.0.605 (FTouchManager @ 0x16a0a24..0x16a2ab8, Ghidra vaddr).
public class FTouchManager
{
	public delegate void DoGetTouchPriorityList(List<FSingleTouchableInterface> list);

	public static bool ShouldMouseEmulateTouch;

	private List<FSingleTouchableInterface> mSingleTouchables;

	private List<FSingleTouchableInterface> mSingleTouchablesToAdd;

	private List<FSingleTouchableInterface> mSingleTouchablesToRemove;

	private FSingleTouchableInterface mTheSingleTouchable;

	private List<FSingleTouchableInterface> mSingleTouchablesNew;

	public DoGetTouchPriorityList OnGetPriorityList;

	private bool mUpdating;

	private bool mNeedsPrioritySort;

	private List<FTouch> mPendingTouches;

	public FSingleTouchableInterface TheSingleTouchable
	{
		get
		{
			return mTheSingleTouchable;
		}
		set
		{
			mTheSingleTouchable = value;
		}
	}

	private event Action<FTouch> mSingleTouchEndedAction;

	static FTouchManager()
	{
		ShouldMouseEmulateTouch = true;
	}

	public FTouchManager()
	{
		mSingleTouchables = new List<FSingleTouchableInterface>();
		mSingleTouchablesToAdd = new List<FSingleTouchableInterface>();
		mSingleTouchablesToRemove = new List<FSingleTouchableInterface>();
		mSingleTouchablesNew = new List<FSingleTouchableInterface>();
		mPendingTouches = new List<FTouch>();
		Input.multiTouchEnabled = true;
		// PC port: the shipped (mobile) build cleared this here and relied on Input.touches only. On Windows there are no
		// touches, so the mouse must emulate the single touch.
#if UNITY_STANDALONE || UNITY_EDITOR
		ShouldMouseEmulateTouch = true;
#else
		ShouldMouseEmulateTouch = false;
#endif
	}

	public void AddSingleTouchEndedAction(Action<FTouch> action)
	{
		mSingleTouchEndedAction -= action;
		mSingleTouchEndedAction += action;
	}

	public void RemoveSingleTouchEndedAction(Action<FTouch> action)
	{
		mSingleTouchEndedAction -= action;
	}

	public bool DoesTheSingleTouchableExist()
	{
		return mTheSingleTouchable != null;
	}

	public void ClearSingleTouchable()
	{
		mTheSingleTouchable = null;
	}

	protected FTouch CreateMouseTouch(int buttonID, float offsetX, float offsetY, float touchScale)
	{
		Vector3 mouse = FUtil.mousePosition;
		FTouch touch = default;
		touch.FingerId = 0;
		touch.ButtonID = buttonID;
		touch.Position = new Vector2((mouse.x + offsetX) * touchScale, (mouse.y + offsetY) * touchScale);
		touch.RawPosition = new Vector2(mouse.x, mouse.y);
		touch.TapCount = 1;
		touch.Phase = TouchPhase.Began;
		return touch;
	}

	public bool IsOverUnityGUI(int buttonID)
	{
		if (EventSystem.current == null)
		{
			return false;
		}
		if (EventSystem.current.IsPointerOverGameObject(-1))
		{
			return true;
		}
		return EventSystem.current.IsPointerOverGameObject(buttonID);
	}

	private static int GetPressedMouseButton(Func<int, bool> test)
	{
		if (test(0))
		{
			return 0;
		}
		if (test(1))
		{
			return 1;
		}
		return 2;
	}

	public void Update()
	{
		mUpdating = true;
		if (OnGetPriorityList != null)
		{
			OnGetPriorityList(mSingleTouchablesNew);
		}
		if (mNeedsPrioritySort)
		{
			UpdatePrioritySorting();
		}
		float touchScale = 1f / Futile.DisplayScale;
		float offsetX = -(Futile.Screen.PixelWidth * Futile.Screen.OriginX);
		float offsetY = -(Futile.Screen.PixelHeight * Futile.Screen.OriginY);
		List<FTouch> touches = new List<FTouch>();
		if (ShouldMouseEmulateTouch)
		{
			if (!FConfigCore.EventBasedMouse)
			{
				if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
				{
					int button = GetPressedMouseButton(Input.GetMouseButtonDown);
					if (!IsOverUnityGUI(button) && PCDisplay.MouseInsideView) // PC port: ignore clicks in the letterbox bars
					{
						FTouch touch = CreateMouseTouch(button, offsetX, offsetY, touchScale);
						touch.Phase = TouchPhase.Began;
						touches.Add(touch);
					}
				}
				if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
				{
					int button = GetPressedMouseButton(Input.GetMouseButtonUp);
					if (!IsOverUnityGUI(button))
					{
						FTouch touch = CreateMouseTouch(button, offsetX, offsetY, touchScale);
						touch.Phase = TouchPhase.Ended;
						touches.Add(touch);
					}
				}
				if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
				{
					int button = GetPressedMouseButton(Input.GetMouseButton);
					if (!IsOverUnityGUI(button))
					{
						FTouch touch = CreateMouseTouch(button, offsetX, offsetY, touchScale);
						touch.Phase = TouchPhase.Moved;
						touches.Add(touch);
					}
				}
			}
			else
			{
				foreach (FTouch pending in mPendingTouches)
				{
					touches.Add(pending);
				}
				mPendingTouches.Clear();
			}
		}
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch unityTouch = Input.GetTouch(i);
			FTouch touch = default;
			touch.FingerId = unityTouch.fingerId;
			touch.ButtonID = -1;
			touch.Position = new Vector2((unityTouch.position.x + offsetX) * touchScale, (unityTouch.position.y + offsetY) * touchScale);
			touch.RawPosition = unityTouch.position;
			touch.TapCount = unityTouch.tapCount;
			touch.Phase = unityTouch.phase;
			touches.Add(touch);
		}
		for (int i = 0; i < touches.Count; i++)
		{
			ProcessSingleTouch(touches[i]);
		}
		for (int i = 0; i < mSingleTouchablesToRemove.Count; i++)
		{
			mSingleTouchables.Remove(mSingleTouchablesToRemove[i]);
		}
		for (int i = 0; i < mSingleTouchablesToAdd.Count; i++)
		{
			if (!mSingleTouchables.Contains(mSingleTouchablesToAdd[i]))
			{
				mSingleTouchables.Add(mSingleTouchablesToAdd[i]);
			}
		}
		mSingleTouchablesToRemove.Clear();
		mSingleTouchablesToAdd.Clear();
		mUpdating = false;
	}

	public void HandleDepthChange()
	{
		mNeedsPrioritySort = true;
	}

	private static int PriorityComparison(FSingleTouchableInterface a, FSingleTouchableInterface b)
	{
		return b.TouchPriority - a.TouchPriority;
	}

	private void UpdatePrioritySorting()
	{
		mNeedsPrioritySort = false;
		mSingleTouchables.Sort(PriorityComparison);
	}

	public void AddSingleTouchTarget(FSingleTouchableInterface touchable)
	{
		if (!mUpdating)
		{
			if (!mSingleTouchables.Contains(touchable))
			{
				mSingleTouchables.Add(touchable);
			}
		}
		else if (!mSingleTouchablesToAdd.Contains(touchable))
		{
			int index = mSingleTouchablesToRemove.IndexOf(touchable);
			if (index != -1)
			{
				mSingleTouchablesToRemove.RemoveAt(index);
			}
			mSingleTouchablesToAdd.Add(touchable);
		}
		mNeedsPrioritySort = true;
	}

	public void RemoveSingleTouchTarget(FSingleTouchableInterface touchable)
	{
		if (!mUpdating)
		{
			mSingleTouchables.Remove(touchable);
		}
		else if (!mSingleTouchablesToRemove.Contains(touchable))
		{
			int index = mSingleTouchablesToAdd.IndexOf(touchable);
			if (index != -1)
			{
				mSingleTouchablesToAdd.RemoveAt(index);
			}
			mSingleTouchablesToRemove.Add(touchable);
		}
	}

	public FSingleTouchableInterface ProcessSingleTouch(FTouch touch, FSingleTouchableInterface ignoreTouchable = null, FNode onlyForParent = null, bool setSingleTouchable = true)
	{
		if (touch.FingerId != 0)
		{
			if (mTheSingleTouchable != null && mTheSingleTouchable.WantAdditionalTouches)
			{
				mTheSingleTouchable.HandleAdditionalTouch(touch);
				return mTheSingleTouchable;
			}
			return null;
		}
		if (touch.Phase == TouchPhase.Began)
		{
			if (!FConfigCore.OldTouchManager)
			{
				if (OnGetPriorityList != null)
				{
					OnGetPriorityList(mSingleTouchablesNew);
				}
				for (int i = 0; i < mSingleTouchablesNew.Count; i++)
				{
					FSingleTouchableInterface touchable = mSingleTouchablesNew[i];
					if (touchable == null || touchable == ignoreTouchable)
					{
						continue;
					}
					if (onlyForParent != null && (touchable.SingleTouchableNode == null || !touchable.SingleTouchableNode.HasParentInHierarchy(onlyForParent)))
					{
						continue;
					}
					if (touchable.HandleSingleTouchBegan(touch, ignoreTouchable))
					{
						if (setSingleTouchable)
						{
							mTheSingleTouchable = touchable;
						}
						mSingleTouchablesNew.Clear();
						return touchable;
					}
				}
				mSingleTouchablesNew.Clear();
			}
			else
			{
				for (int i = 0; i < mSingleTouchables.Count; i++)
				{
					FSingleTouchableInterface touchable = mSingleTouchables[i];
					if (ignoreTouchable != null && touchable == ignoreTouchable)
					{
						continue;
					}
					if (onlyForParent != null && (touchable.SingleTouchableNode == null || !touchable.SingleTouchableNode.HasParentInHierarchy(onlyForParent)))
					{
						continue;
					}
					if (touchable.HandleSingleTouchBegan(touch, ignoreTouchable))
					{
						if (setSingleTouchable)
						{
							mTheSingleTouchable = touchable;
						}
						return touchable;
					}
				}
			}
			return mTheSingleTouchable;
		}
		if (touch.Phase == TouchPhase.Ended || touch.Phase == TouchPhase.Canceled)
		{
			if (mTheSingleTouchable != null && mTheSingleTouchable != ignoreTouchable)
			{
				if (touch.Phase == TouchPhase.Ended)
				{
					mTheSingleTouchable.HandleSingleTouchEnded(touch);
				}
				else
				{
					mTheSingleTouchable.HandleSingleTouchCanceled(touch);
				}
				if (this.mSingleTouchEndedAction != null)
				{
					this.mSingleTouchEndedAction(touch);
				}
			}
			mTheSingleTouchable = null;
			return mTheSingleTouchable;
		}
		if (mTheSingleTouchable != null && mTheSingleTouchable != ignoreTouchable)
		{
			mTheSingleTouchable.HandleSingleTouchMoved(touch);
		}
		return mTheSingleTouchable;
	}

	public bool IsColliderTouchable(FUICollider collider, Vector2 mousePos)
	{
		if (collider == null || !collider.Enabled || !collider.ContainsPoint(mousePos))
		{
			return false;
		}
		int count = mSingleTouchablesNew.Count;
		for (int i = 0; i < count; i++)
		{
			FUICollider other = mSingleTouchablesNew[i] as FUICollider;
			if (other == null)
			{
				continue;
			}
			if (other == collider)
			{
				return true;
			}
			if (other.Enabled && !other.PassThrough && other.ContainsPoint(mousePos))
			{
				return false;
			}
		}
		return false;
	}

	public void ProcessEvent(Event e)
	{
		if (!FConfigCore.EventBasedMouse)
		{
			return;
		}
		if (!Event.current.isMouse)
		{
			return;
		}
		if (e.type == EventType.MouseEnterWindow || e.type == EventType.MouseLeaveWindow)
		{
			return;
		}
		mPendingTouches.Add(new FTouch(e));
	}
}
