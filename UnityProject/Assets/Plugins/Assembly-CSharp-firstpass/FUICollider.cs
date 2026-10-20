using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUICollider @ 0x16a3c08..0x16a8c40, Ghidra vaddr).
public class FUICollider : FNode, FSingleTouchableInterface
{
	[CompilerGenerated]
	private Action<FUICollider, FTouch> m_mSignalPress;

	[CompilerGenerated]
	private Action<FUICollider, FTouch> m_mSignalMove;

	[CompilerGenerated]
	private Action<FUICollider, FTouch> m_mSignalRelease;

	[CompilerGenerated]
	private Action<FUICollider, FTouch> m_mSignalCancel;

	[CompilerGenerated]
	private Action<FUICollider, FTouch> m_mSignalAdditionalTouch;

	protected Rect mCollider;

	protected Vector2 mAnchor;

	protected bool mWantAdditionalTouches;

	protected FUICollider mParentContainerCollider;

	protected float mReleaseActivationDistance = float.MaxValue;

	protected int mTouchPriority = -1;

	public Rect ColliderRect => mCollider;

	public Vector2 Anchor
	{
		get
		{
			return mAnchor;
		}
		set
		{
			mAnchor = value;
			UpdatePosition();
		}
	}

	public bool WantAdditionalTouches => mWantAdditionalTouches;

	public int Data { get; set; } = -1;

	public bool CircularCollision { get; set; }

	public bool Enabled { get; set; } = true;

	public float ReleaseActivationDistance
	{
		get
		{
			return mReleaseActivationDistance;
		}
		set
		{
			mReleaseActivationDistance = value;
		}
	}

	public override int TouchPriority
	{
		get
		{
			if (mTouchPriority >= 0)
			{
				return mTouchPriority;
			}
			return base.TouchPriority;
		}
		set
		{
			mTouchPriority = value;
		}
	}

	public virtual FNode SingleTouchableNode => this;

	public string LogName => "UICollider" + Name;

	public bool PassThrough { get; set; }

	protected event Action<FUICollider, FTouch> mSignalPress;

	protected event Action<FUICollider, FTouch> mSignalMove;

	protected event Action<FUICollider, FTouch> mSignalRelease;

	protected event Action<FUICollider, FTouch> mSignalCancel;

	protected event Action<FUICollider, FTouch> mSignalAdditionalTouch;

	public FUICollider(string name = "", bool wantAdditionalTouches = false)
	{
		if (!string.IsNullOrEmpty(name))
		{
			Name = "Collider" + name;
		}
		SetSize(0, 0);
		mWantAdditionalTouches = wantAdditionalTouches;
	}

	public FUICollider GetParentContainerCollider()
	{
		return mParentContainerCollider;
	}

	public void SetParentContainerCollider(FUICollider parentContainerCollider)
	{
		for (FUICollider c = parentContainerCollider; c != null; c = c.mParentContainerCollider)
		{
			if (c == this)
			{
				return;
			}
		}
		mParentContainerCollider = parentContainerCollider;
	}

	public virtual void SetSize(float sizeX, float sizeY)
	{
		SetSize((int)Math.Floor(sizeX), (int)Math.Floor(sizeY));
	}

	public virtual void SetSize(Vector2 size)
	{
		SetSize(size.x, size.y);
	}

	public virtual void SetSize(int sizeX, int sizeY)
	{
		mAnchor = new Vector2(0.5f, 0.5f);
		mCollider.width = sizeX;
		mCollider.height = sizeY;
		UpdatePosition();
	}

	public virtual bool ContainsPoint(Vector2 point, FSingleTouchableInterface ignoreTouchable = null)
	{
		if (mParentContainerCollider == null || mParentContainerCollider == ignoreTouchable)
		{
			Vector2 local = GlobalToLocal(point);
			if (CircularCollision)
			{
				float halfW = mCollider.width * 0.5f;
				float halfH = mCollider.height * 0.5f;
				float dx = local.x - (mCollider.x + halfW);
				float dy = local.y - (mCollider.y + halfH);
				float r = Math.Max(halfW, halfH);
				return dx * dx + dy * dy <= r * r;
			}
			if (mCollider.x <= local.x && local.x < mCollider.x + mCollider.width && mCollider.y <= local.y)
			{
				return local.y < mCollider.y + mCollider.height;
			}
		}
		return false;
	}

	protected virtual void UpdatePosition()
	{
		mCollider.x = mCollider.width * (0f - mAnchor.x);
		mCollider.y = mCollider.height * (0f - mAnchor.y);
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		Futile.TouchManager.AddSingleTouchTarget(this);
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		Futile.TouchManager.RemoveSingleTouchTarget(this);
	}

	public virtual bool HandleSingleTouchBegan(FTouch touch, FSingleTouchableInterface ignoreTouchable = null)
	{
		if (Enabled)
		{
			if (!ContainsPoint(touch.Position, ignoreTouchable))
			{
				return false;
			}
			if (this.mSignalPress != null)
			{
				this.mSignalPress(this, touch);
			}
			return true;
		}
		return false;
	}

	public virtual bool CanHandleTouch(FTouch touch, FSingleTouchableInterface ignoreTouchable = null)
	{
		if (Enabled)
		{
			return ContainsPoint(touch.Position, ignoreTouchable);
		}
		return false;
	}

	public virtual void HandleSingleTouchMoved(FTouch touch)
	{
		if (this.mSignalMove != null)
		{
			this.mSignalMove(this, touch);
		}
	}

	public virtual void HandleSingleTouchEnded(FTouch touch)
	{
		Vector2 local = GlobalToLocal(touch.Position);
		float distance = mCollider.DistanceToPoint(local);
		Action<FUICollider, FTouch> signal = ((!(mReleaseActivationDistance <= distance)) ? this.mSignalRelease : this.mSignalCancel);
		if (signal != null)
		{
			signal(this, touch);
		}
	}

	public virtual void HandleSingleTouchCanceled(FTouch touch)
	{
		if (this.mSignalCancel != null)
		{
			this.mSignalCancel(this, touch);
		}
	}

	public override void SelfResizeExtents()
	{
		if (HasConstraints() && CalculateExtents())
		{
			mCollider = Extents;
		}
	}

	public void HandleAdditionalTouch(FTouch touch)
	{
		if (this.mSignalAdditionalTouch != null)
		{
			this.mSignalAdditionalTouch(this, touch);
		}
	}

	public void AddSignalPress(Action<FUICollider, FTouch> d)
	{
		mSignalPress -= d;
		mSignalPress += d;
	}

	public void AddSignalMove(Action<FUICollider, FTouch> d)
	{
		mSignalMove -= d;
		mSignalMove += d;
	}

	public void AddSignalRelease(Action<FUICollider, FTouch> d, float distance = 40f)
	{
		mSignalRelease -= d;
		mSignalRelease += d;
		mReleaseActivationDistance = distance;
	}

	public void AddSignalCancel(Action<FUICollider, FTouch> d)
	{
		mSignalCancel -= d;
		mSignalCancel += d;
	}

	public void AddSignalAdditionalTouch(Action<FUICollider, FTouch> d)
	{
		mSignalAdditionalTouch -= d;
		mSignalAdditionalTouch += d;
	}

	public void RemoveSignalPress(Action<FUICollider, FTouch> d)
	{
		mSignalPress -= d;
	}

	public void RemoveSignalMove(Action<FUICollider, FTouch> d)
	{
		mSignalMove -= d;
	}

	public void RemoveSignalRelease(Action<FUICollider, FTouch> d)
	{
		mSignalRelease -= d;
	}

	public void RemoveSignalCancel(Action<FUICollider, FTouch> d)
	{
		mSignalCancel -= d;
	}

	public void RemoveSignalAdditionalTouch(Action<FUICollider, FTouch> d)
	{
		mSignalAdditionalTouch -= d;
	}
}
