using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUIDragRotateNode @ 0x16c5ebc..0x16c63a4, Ghidra vaddr).
public class FUIDragRotateNode : FNode
{
	protected FUICollider mCollider;

	protected float mWidth;

	protected float mHeight;

	protected FSingleTouchableInterface mTouchStartChildNode;

	protected Vector2 mLastDragUpdatePos;

	protected bool mTouchStarted;

	protected Vector2 mRotationSpeed = new Vector2(0.25f, 0f);

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

	public Vector2 RotationSpeed
	{
		get
		{
			return mRotationSpeed;
		}
		set
		{
			mRotationSpeed = value;
		}
	}

	public FUICollider Collider => mCollider;

	public Transform ControlledTransform { get; set; }

	public FUIDragRotateNode(string name)
		: base(name)
	{
		mCollider = new FUICollider(Name + ".Collider", true);
		mCollider.SetParent(this);
		mCollider.AddSignalPress(HandleTapStart);
		mCollider.AddSignalMove(HandleTapMove);
		mCollider.AddSignalRelease(HandleTapEnd, 40f);
		mCollider.AddSignalCancel(HandleTapEnd);
	}

	public void SetControlledGameObject(GameObject go)
	{
		if (go != null)
		{
			ControlledTransform = go.transform;
		}
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

	protected virtual void HandleTapStart(FUICollider collider, FTouch touch)
	{
		mTouchStarted = true;
		mLastDragUpdatePos = touch.Position;
	}

	protected virtual void HandleTapMove(FUICollider collider, FTouch touch)
	{
		Vector2 delta = touch.Position - mLastDragUpdatePos;
		if (ControlledTransform != null)
		{
			ControlledTransform.Rotate(Vector3.up, 0f - delta.x * mRotationSpeed.x);
			ControlledTransform.Rotate(Vector3.right, delta.y * mRotationSpeed.y);
		}
		mLastDragUpdatePos = touch.Position;
	}

	protected virtual void HandleTapEnd(FUICollider collider, FTouch touch)
	{
		HandleTapMove(collider, touch);
		mTouchStarted = false;
	}
}
