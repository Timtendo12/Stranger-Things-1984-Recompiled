using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FScrollNode @ 0x16bf130..0x16c1a14, Ghidra vaddr).
public class FScrollNode : FNode
{
	public const float cDefaultDragThreshold = 15f;

	public const float cTensionExponent = 8f;

	public const float cRestoreExponent = 1.5f;

	public const float cMinRestoreFactor = 6f;

	public const float cGlideDrag = 3.5f;

	public const float cMaxGlideSpeed = 5000f;

	public const float cGlideSetWindow = 0.25f;

	public const float cMinZoomDistance = 5f;

	public const float cZoomTensionExponent = 10f;

	public const float cZoomRestoreExponent = 5.5f;

	public const float cMinZoomRestoreFactor = 12f;

	public const float cZoomTensionDistance = 2f;

	public const float cMatchedSizeEpsilon = 0.75f;

	public const float cGlideCutoffSpeed = 30f;

	protected float mWidth;

	protected float mHeight;

	protected FUICollider mCollider;

	protected FSingleTouchableInterface mTouchStartChildNode;

	protected Vector2 mDragStartScrollPos;

	protected bool mTouchStarted;

	protected bool mDragging;

	protected bool mOutOfBounds;

	protected Vector2 mLastDragUpdatePos;

	protected float mLastDragUpdateTime;

	protected float mLastUpdateTime;

	protected FNode mNodeToScroll;

	protected bool mSecondTouch;

	protected Vector2 mSecondTouchPos;

	protected Vector2 mSecondTouchLocalPos;

	protected Vector2 mFirstTouchLocalPos;

	protected float mLastSecondTouchDistance;

	protected bool mZoomOutOfBounds;

	protected Vector2 mVelocity;

	protected bool mGliding;

	protected List<FVelocityEntry> mVelocityEntries = new List<FVelocityEntry>();

	protected bool mWasZooming;

	protected bool mCanScrollX = true;

	protected bool mCanScrollY = true;

	protected float mMinZoomScale = 0.5f;

	protected float mMaxZoomScale = 2f;

	protected float mHardMinZoomScale = -1f;

	protected float mHardMaxZoomScale = -1f;

	public bool mCanZoom = true;

	public FUICollider Collider => mCollider;

	public override float Width
	{
		get
		{
			return mWidth;
		}
		set
		{
			mWidth = value;
		}
	}

	public override float Height
	{
		get
		{
			return mHeight;
		}
		set
		{
			mHeight = value;
		}
	}

	public float DragThreshold { get; set; }

	public FNode TouchStartChildNode => (mTouchStartChildNode == null) ? null : mTouchStartChildNode.SingleTouchableNode;

	public bool CanScrollX
	{
		get
		{
			return mCanScrollX;
		}
		set
		{
			mCanScrollX = value;
		}
	}

	public bool CanScrollY
	{
		get
		{
			return mCanScrollY;
		}
		set
		{
			mCanScrollY = value;
		}
	}

	public bool ScrollableInHeight => Height < mNodeToScroll.Height * mNodeToScroll.Scale;

	public bool ScrollableInWidth => Width < mNodeToScroll.Width * mNodeToScroll.Scale;

	public float ScrollNodeHeight => mNodeToScroll.Height * mNodeToScroll.Scale;

	public float ScrollNodeWidth => mNodeToScroll.Width * mNodeToScroll.Scale;

	public float MinZoomScale
	{
		get
		{
			return mMinZoomScale;
		}
		set
		{
			mMinZoomScale = value;
		}
	}

	public float MaxZoomScale
	{
		get
		{
			return mMaxZoomScale;
		}
		set
		{
			mMaxZoomScale = value;
		}
	}

	public float HardMinZoomScale
	{
		get
		{
			return mHardMinZoomScale;
		}
		set
		{
			mHardMinZoomScale = value;
		}
	}

	public float HardMaxZoomScale
	{
		get
		{
			return mHardMaxZoomScale;
		}
		set
		{
			mHardMaxZoomScale = value;
		}
	}

	public bool CanZoom
	{
		get
		{
			return mCanZoom;
		}
		set
		{
			mCanZoom = value;
		}
	}

	public bool CanScrollWhenWidthFits { get; set; }

	public bool CanScrollWhenHeightFits { get; set; }

	public Vector2 ScrollPosition
	{
		get
		{
			if (mNodeToScroll != null)
			{
				return mNodeToScroll.Position;
			}
			return Vector2.zero;
		}
		set
		{
			SetScrollPosition(value);
		}
	}

	public Vector2 NodeToScrollSize
	{
		get
		{
			if (mNodeToScroll == null)
			{
				return Vector2.one;
			}
			return new Vector2(mNodeToScroll.Width * mNodeToScroll.Scale, mNodeToScroll.Height * mNodeToScroll.Scale);
		}
	}

	public float NodeToScrollScale => (mNodeToScroll != null) ? mNodeToScroll.Scale : 0f;

	public float ZoomScale => (mNodeToScroll != null) ? mNodeToScroll.Scale : 0f;

	public FScrollNode(string name)
		: base(name)
	{
		mNodeToScroll = null;
		DragThreshold = 15f;
		mCollider = new FUICollider(Name + ".Collider", true);
		mCollider.SetParent(this);
		mCollider.ExpandToFill(this, 0f, 0f);
		mCollider.AddSignalPress(HandleTapStart);
		mCollider.AddSignalMove(HandleTapMove);
		mCollider.AddSignalRelease(HandleTapEnd, 40f);
		mCollider.AddSignalCancel(HandleTapEnd);
		mCollider.AddSignalAdditionalTouch(HandleAdditionalTouch);
	}

	public override void SelfResizeExtents()
	{
		if (HasConstraints() && CalculateExtents())
		{
			Rect extents = Extents;
			Position = new Vector2(extents.x + extents.width * 0.5f, extents.y + extents.height * 0.5f);
			Width = extents.width;
			Height = extents.height;
		}
	}

	public void SetScrollPosition(Vector2 scrollPos)
	{
		SetScrollPosition(scrollPos.x, scrollPos.y);
	}

	public void SetScrollPosition(float x, float y)
	{
		mOutOfBounds = x < GetMinScrollX() || x > GetMaxScrollX() || y < GetMinScrollY() || y > GetMaxScrollY();
		mNodeToScroll.Position = new Vector2(x, y);
	}

	public float GetMinScrollX()
	{
		if (!mCanScrollX || mNodeToScroll == null)
		{
			return 0f;
		}
		float nodeWidth = mNodeToScroll.Width * mNodeToScroll.Scale;
		if (nodeWidth <= Width)
		{
			if (!CanScrollWhenWidthFits)
			{
				return 0f;
			}
			return (nodeWidth - Width) * 0.5f;
		}
		return (Width - nodeWidth) * 0.5f;
	}

	public float GetMaxScrollX()
	{
		if (!mCanScrollX || mNodeToScroll == null)
		{
			return 0f;
		}
		float nodeWidth = mNodeToScroll.Width * mNodeToScroll.Scale;
		if (nodeWidth <= Width)
		{
			if (!CanScrollWhenWidthFits)
			{
				return 0f;
			}
			return (Width - nodeWidth) * 0.5f;
		}
		return (nodeWidth - Width) * 0.5f;
	}

	public float GetMinScrollY()
	{
		if (!mCanScrollY || mNodeToScroll == null)
		{
			return 0f;
		}
		float nodeHeight = mNodeToScroll.Height * mNodeToScroll.Scale;
		if (nodeHeight <= Height)
		{
			if (!CanScrollWhenHeightFits)
			{
				return 0f;
			}
			return (nodeHeight - Height) * 0.5f;
		}
		return (Height - nodeHeight) * 0.5f;
	}

	public float GetMaxScrollY()
	{
		if (!mCanScrollY || mNodeToScroll == null)
		{
			return 0f;
		}
		float nodeHeight = mNodeToScroll.Height * mNodeToScroll.Scale;
		if (nodeHeight <= Height)
		{
			if (!CanScrollWhenHeightFits)
			{
				return 0f;
			}
			return (Height - nodeHeight) * 0.5f;
		}
		return (nodeHeight - Height) * 0.5f;
	}

	public bool IsAtTop()
	{
		return ScrollPosition.y - 1f < GetMinScrollY();
	}

	public bool IsAtBottom()
	{
		return GetMaxScrollY() < ScrollPosition.y + 1f;
	}

	public bool IsAtLeft()
	{
		return GetMaxScrollX() < ScrollPosition.x + 1f;
	}

	public bool IsAtRight()
	{
		return ScrollPosition.x - 1f < GetMinScrollX();
	}

	private void StopScrollMotion()
	{
		mGliding = false;
		mDragging = false;
		mVelocityEntries.Clear();
		mVelocity = Vector2.zero;
	}

	public void ScrollToLeft()
	{
		mNodeToScroll.ResizeConstraintsInGraph();
		SetScrollPosition(GetMaxScrollX(), GetMinScrollY());
		StopScrollMotion();
	}

	public void ScrollToRight()
	{
		mNodeToScroll.ResizeConstraintsInGraph();
		SetScrollPosition(GetMinScrollX(), GetMinScrollY());
		StopScrollMotion();
	}

	public void ScrollToTop()
	{
		mNodeToScroll.ResizeConstraintsInGraph();
		SetScrollPosition(GetMinScrollX(), GetMinScrollY());
		StopScrollMotion();
	}

	public void ScrollToBottom()
	{
		mNodeToScroll.ResizeConstraintsInGraph();
		SetScrollPosition(GetMinScrollX(), GetMaxScrollY());
		StopScrollMotion();
	}

	public void ScrollUp(float amount)
	{
		if (mNodeToScroll != null)
		{
			float y = Mathf.Max(mNodeToScroll.Position.y - amount, GetMinScrollY());
			SetScrollPosition(GetMinScrollX(), y);
		}
	}

	public void ScrollDown(float amount)
	{
		if (mNodeToScroll != null)
		{
			float y = Mathf.Min(mNodeToScroll.Position.y + amount, GetMaxScrollY());
			SetScrollPosition(GetMinScrollX(), y);
		}
	}

	public void ScrollLeft(float amount)
	{
		if (mNodeToScroll != null)
		{
			float x = Mathf.Min(mNodeToScroll.Position.x + amount, GetMaxScrollX());
			SetScrollPosition(x, GetMinScrollY());
		}
	}

	public void ScrollRight(float amount)
	{
		if (mNodeToScroll != null)
		{
			float x = Mathf.Max(mNodeToScroll.Position.x - amount, GetMinScrollX());
			SetScrollPosition(x, GetMinScrollY());
		}
	}

	protected virtual void HandleTapStart(FUICollider collider, FTouch touch)
	{
		if (mNodeToScroll == null)
		{
			return;
		}
		mTouchStarted = true;
		mDragStartScrollPos = mNodeToScroll.Position;
		mLastDragUpdatePos = touch.Position;
		mLastDragUpdateTime = Time.realtimeSinceStartup;
		mVelocityEntries.Clear();
		if (mGliding)
		{
			if (mVelocity.x * mVelocity.x + mVelocity.y * mVelocity.y > 900f)
			{
				mDragging = true;
			}
			mGliding = false;
			mVelocity = Vector2.zero;
		}
		if (!mDragging)
		{
			mTouchStartChildNode = Futile.TouchManager.ProcessSingleTouch(touch, mCollider, this, false);
		}
	}

	protected Vector2 ComputeTensionedNewPos(Vector2 delta)
	{
		if (mNodeToScroll == null)
		{
			return Vector2.zero;
		}
		Vector2 pos = mNodeToScroll.Position;
		float newX = delta.x + pos.x;
		float minX = GetMinScrollX();
		float maxX = GetMaxScrollX();
		float tensionX = 0f;
		if (delta.x < 0f && newX < minX)
		{
			tensionX = (minX - newX) / Width;
		}
		else if (delta.x > 0f && newX > maxX)
		{
			tensionX = (newX - maxX) / Width;
		}
		float newY = delta.y + pos.y;
		float minY = GetMinScrollY();
		float maxY = GetMaxScrollY();
		float tensionY = 0f;
		if (delta.y < 0f && newY < minY)
		{
			tensionY = (minY - newY) / Height;
		}
		else if (delta.y > 0f && newY > maxY)
		{
			tensionY = (newY - maxY) / Height;
		}
		float factorY = Mathf.Exp(tensionY * -8f);
		float factorX = Mathf.Exp(tensionX * -8f);
		Vector2 current = mNodeToScroll.Position;
		return new Vector2(delta.x * factorX + current.x, delta.y * factorY + current.y);
	}

	protected virtual void HandleTapMove(FUICollider collider, FTouch touch)
	{
		if (mNodeToScroll == null)
		{
			return;
		}
		float dx = 0f;
		float dy = 0f;
		if (mCanScrollX)
		{
			dx = touch.Position.x - mLastDragUpdatePos.x;
			if (!CanScrollWhenWidthFits && mNodeToScroll.Width * mNodeToScroll.Scale - Width < 0.75f)
			{
				dx = 0f;
			}
		}
		if (mCanScrollY)
		{
			dy = touch.Position.y - mLastDragUpdatePos.y;
			if (!CanScrollWhenHeightFits && mNodeToScroll.Height * mNodeToScroll.Scale - Height < 0.75f)
			{
				dy = 0f;
			}
		}
		if (!mDragging)
		{
			if (Mathf.Sqrt(dx * dx + dy * dy) < DragThreshold)
			{
				return;
			}
			FTouch cancelTouch = touch;
			cancelTouch.Phase = TouchPhase.Canceled;
			Futile.TouchManager.TheSingleTouchable = mTouchStartChildNode;
			Futile.TouchManager.ProcessSingleTouch(cancelTouch, mCollider, this, true);
			Futile.TouchManager.TheSingleTouchable = mCollider;
			mDragging = true;
		}
		float now = Time.realtimeSinceStartup;
		if (!mSecondTouch)
		{
			SetScrollPosition(ComputeTensionedNewPos(new Vector2(dx, dy)));
			float dt = now - mLastDragUpdateTime;
			if (dt > FMath.Epsilon)
			{
				FVelocityEntry entry = new FVelocityEntry();
				entry.Time = now;
				entry.Velocity = new Vector2(dx / dt, dy / dt);
				mVelocityEntries.Add(entry);
			}
			RemoveOldVelocityEntries();
			mLastDragUpdateTime = now;
			mLastDragUpdatePos = touch.Position;
		}
		else
		{
			ProcessZoom(touch.Position, true);
			mLastDragUpdatePos = touch.Position;
			mFirstTouchLocalPos = mNodeToScroll.LocalToLocal(this, touch.Position);
			mLastDragUpdateTime = now;
		}
	}

	protected virtual void HandleTapEnd(FUICollider collider, FTouch touch)
	{
		mSecondTouch = false;
		if (!mTouchStarted)
		{
			mWasZooming = false;
			return;
		}
		if (!mDragging)
		{
			Futile.TouchManager.TheSingleTouchable = mTouchStartChildNode;
			Futile.TouchManager.ProcessSingleTouch(touch, mCollider, this, true);
		}
		else
		{
			mVelocity = Vector2.zero;
			if (!mWasZooming)
			{
				HandleTapMove(collider, touch);
				mGliding = true;
				if (mVelocityEntries.Count > 0)
				{
					float now = Time.realtimeSinceStartup;
					float totalWeight = 0f;
					for (int i = 0; i < mVelocityEntries.Count; i++)
					{
						float weight = (now - mVelocityEntries[i].Time) * -4f + 1f;
						weight = Mathf.Min(1f, weight * weight);
						weight = weight * 0.9f + 0.1f;
						totalWeight += weight;
						mVelocity = new Vector2(mVelocity.x + mVelocityEntries[i].Velocity.x * weight, mVelocity.y + mVelocityEntries[i].Velocity.y * weight);
					}
					mVelocity = new Vector2(mVelocity.x / totalWeight, mVelocity.y / totalWeight);
				}
			}
			else
			{
				mVelocityEntries.Clear();
				mGliding = false;
			}
		}
		mTouchStarted = false;
		mDragging = false;
		mWasZooming = false;
		mVelocityEntries.Clear();
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		Futile.Instance.AddSignalUpdate(HandleUpdate);
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		Futile.Instance.RemoveSignalUpdate(HandleUpdate);
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
		float now = Time.realtimeSinceStartup;
		float dt = now - mLastUpdateTime;
		mLastUpdateTime = now;
		dt = Mathf.Min(dt, 0.1f);
		if (mDragging || mNodeToScroll == null)
		{
			return;
		}
		if (mOutOfBounds)
		{
			Vector2 pos = mNodeToScroll.Position;
			float restoreX = Vector2.zero.x;
			float restoreY = Vector2.zero.y;
			float minX = GetMinScrollX();
			float maxX = GetMaxScrollX();
			if (pos.x > maxX)
			{
				restoreX = maxX - pos.x;
			}
			if (pos.x < minX)
			{
				restoreX = minX - pos.x;
			}
			float minY = GetMinScrollY();
			float maxY = GetMaxScrollY();
			if (pos.y > maxY)
			{
				restoreY = maxY - pos.y;
			}
			if (pos.y < minY)
			{
				restoreY = minY - pos.y;
			}
			float expX = Mathf.Exp((0f - Mathf.Abs(restoreX)) / Width * 1.5f);
			float expY = Mathf.Exp((0f - Mathf.Abs(restoreY)) / Height * 1.5f);
			float factorX = Mathf.Max(6f, 1f - expX);
			float factorY = Mathf.Max(6f, 1f - expY);
			restoreX *= dt * factorX;
			restoreY = restoreY * dt * factorY;
			if (mGliding)
			{
				float accelX = 1f / dt * restoreX;
				if (mVelocity.x < 0f && accelX > 0f)
				{
					mVelocity.x = accelX + mVelocity.x;
					if (mVelocity.x > 0f)
					{
						mVelocity.x = 0f;
					}
				}
				else if (mVelocity.x > 0f && accelX < 0f)
				{
					mVelocity.x = accelX + mVelocity.x;
					if (mVelocity.x < 0f)
					{
						mVelocity.x = 0f;
					}
				}
				else
				{
					float accelY = 1f / dt * restoreY;
					if (mVelocity.y < 0f && accelY > 0f)
					{
						mVelocity.y = accelY + mVelocity.y;
						if (mVelocity.y > 0f)
						{
							mVelocity.y = 0f;
						}
					}
					else if (mVelocity.y > 0f && accelY < 0f)
					{
						mVelocity.y = accelY + mVelocity.y;
						if (mVelocity.y < 0f)
						{
							mVelocity.y = 0f;
						}
					}
				}
			}
			Vector2 current = mNodeToScroll.Position;
			SetScrollPosition(restoreX + current.x, restoreY + current.y);
		}
		if (mZoomOutOfBounds)
		{
			float scale = mNodeToScroll.Scale;
			float zoomDelta;
			if (scale < mMinZoomScale)
			{
				zoomDelta = mMinZoomScale - scale;
			}
			else if (scale > mMaxZoomScale)
			{
				zoomDelta = mMaxZoomScale - scale;
			}
			else
			{
				zoomDelta = 0f;
			}
			float exp = Mathf.Exp(Mathf.Abs(zoomDelta) * -0.5f * 5.5f);
			float factor = Mathf.Max(12f, 1f - exp);
			SetZoomScale(zoomDelta * dt * factor + mNodeToScroll.Scale);
		}
		if (mGliding)
		{
			Vector2 pos = mNodeToScroll.Position;
			Vector2 velocity = mVelocity;
			float dragX = dt * 3.5f * velocity.x;
			float dragY = dt * 3.5f * velocity.y;
			float newX = velocity.x - dragX;
			float newY = velocity.y - dragY;
			mVelocity = new Vector2(newX, newY);
			if ((dragX > 0f && newX < 0f) || (dragX < 0f && newX > 0f))
			{
				newX = 0f;
				mVelocity.x = 0f;
			}
			if ((dragY > 0f && newY < 0f) || (dragY < 0f && newY > 0f))
			{
				newY = 0f;
				mVelocity.y = 0f;
			}
			if (newX > 5000f)
			{
				mVelocity.x = 5000f;
			}
			else if (newX < -5000f)
			{
				mVelocity.x = -5000f;
			}
			if (newY > 5000f)
			{
				mVelocity.y = 5000f;
			}
			else if (newY < -5000f)
			{
				mVelocity.y = -5000f;
			}
			SetScrollPosition(pos.x + dt * velocity.x, pos.y + dt * velocity.y);
			if (Mathf.Sqrt(mVelocity.x * mVelocity.x + mVelocity.y * mVelocity.y) < 0.1f)
			{
				mGliding = false;
			}
		}
		mNodeToScroll.UpdateMatrix();
		mSecondTouchLocalPos = mNodeToScroll.LocalToLocal(this, mSecondTouchPos);
		mFirstTouchLocalPos = mNodeToScroll.LocalToLocal(this, mLastDragUpdatePos);
	}

	protected void RemoveOldVelocityEntries()
	{
		float now = Time.realtimeSinceStartup;
		for (int i = mVelocityEntries.Count - 1; i >= 0; i--)
		{
			if (now - mVelocityEntries[i].Time > 0.25f)
			{
				mVelocityEntries.RemoveAt(i);
			}
		}
	}

	protected void HandleAdditionalTouch(FUICollider collider, FTouch touch)
	{
		if (!mCanZoom || touch.FingerId != 1)
		{
			return;
		}
		if (touch.Phase == TouchPhase.Ended || touch.Phase == TouchPhase.Canceled)
		{
			mSecondTouch = false;
		}
		else if (touch.Phase == TouchPhase.Moved)
		{
			ProcessZoom(touch.Position, false);
			mSecondTouchPos = touch.Position;
		}
		else if (touch.Phase == TouchPhase.Began)
		{
			mSecondTouch = true;
			mDragging = true;
			mSecondTouchPos = touch.Position;
			float distance = Vector2.Distance(mSecondTouchPos, mLastDragUpdatePos);
			mLastSecondTouchDistance = Mathf.Max(5f, distance);
			mNodeToScroll.UpdateMatrix();
			mFirstTouchLocalPos = mNodeToScroll.LocalToLocal(this, mLastDragUpdatePos);
			mSecondTouchLocalPos = mNodeToScroll.LocalToLocal(this, mSecondTouchPos);
		}
	}

	protected void ProcessZoom(Vector2 pos, bool firstFinger)
	{
		if (!mSecondTouch || !mCanZoom || mNodeToScroll == null)
		{
			return;
		}
		Vector2 anchorLocal = firstFinger ? mFirstTouchLocalPos : mSecondTouchLocalPos;
		mWasZooming = true;
		Vector2 otherPos = firstFinger ? mSecondTouchPos : mLastDragUpdatePos;
		float lastDistance = mLastSecondTouchDistance;
		float distance = Mathf.Max(5f, Vector2.Distance(pos, otherPos));
		if (lastDistance > 0.1f)
		{
			float newScale = distance / lastDistance * mNodeToScroll.Scale;
			float scaleDelta = newScale - mNodeToScroll.Scale;
			float tension = 0f;
			if (newScale < mMinZoomScale && scaleDelta < 0f)
			{
				tension = (mMinZoomScale - newScale) * 0.5f;
			}
			else if (scaleDelta > 0f && newScale > mMaxZoomScale)
			{
				tension = (newScale - mMaxZoomScale) * 0.5f;
			}
			float factor = Mathf.Exp(tension * -10f);
			SetZoomScale(scaleDelta * factor + mNodeToScroll.Scale);
			mNodeToScroll.UpdateMatrix();
		}
		Vector2 local = mNodeToScroll.LocalToLocal(this, pos);
		float scale = mNodeToScroll.Scale;
		SetScrollPosition(ComputeTensionedNewPos(new Vector2((local.x - anchorLocal.x) * scale, (local.y - anchorLocal.y) * scale)));
		mNodeToScroll.UpdateMatrix();
		if (!firstFinger)
		{
			mFirstTouchLocalPos = mNodeToScroll.LocalToLocal(this, mLastDragUpdatePos);
			mSecondTouchLocalPos = local;
		}
		else
		{
			mFirstTouchLocalPos = local;
			mSecondTouchLocalPos = mNodeToScroll.LocalToLocal(this, mSecondTouchPos);
		}
		mLastSecondTouchDistance = distance;
	}

	public void SetZoomScale(float scale)
	{
		if (mNodeToScroll == null)
		{
			return;
		}
		float newScale = scale;
		if (mHardMinZoomScale > 0f && scale < mHardMinZoomScale)
		{
			newScale = mHardMinZoomScale;
		}
		if (mHardMaxZoomScale > 0f)
		{
			// Faithful to the binary: the hard max clamps against this node's own Scale, not the requested scale (0x16c179c).
			newScale = Mathf.Min(mHardMaxZoomScale, base.Scale);
		}
		mNodeToScroll.Scale = newScale;
		mZoomOutOfBounds = newScale < mMinZoomScale || newScale > mMaxZoomScale;
		Vector2 pos = mNodeToScroll.Position;
		mOutOfBounds = pos.x < GetMinScrollX() || pos.x > GetMaxScrollX() || pos.y < GetMinScrollY() || pos.y > GetMaxScrollY();
	}

	public Vector2 ClampScrollPos(Vector2 scrollPos)
	{
		return new Vector2(Mathf.Clamp(scrollPos.x, GetMinScrollX(), GetMaxScrollX()), Mathf.Clamp(scrollPos.y, GetMinScrollY(), GetMaxScrollY()));
	}

	public Vector2 GetScrollPositionForCentering(Vector2 pos, bool allowOutOfBounds)
	{
		if (mNodeToScroll == null)
		{
			return Vector2.zero;
		}
		float scale = mNodeToScroll.Scale;
		Vector2 result = new Vector2(0f - scale * pos.x, 0f - scale * pos.y);
		if (!allowOutOfBounds)
		{
			result = ClampScrollPos(result);
		}
		return result;
	}
}
