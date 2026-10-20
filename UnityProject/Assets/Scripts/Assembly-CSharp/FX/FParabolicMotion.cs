using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FParabolicMotion @ 0x17b73fc..0x17b7784, Ghidra vaddr).
public class FParabolicMotion : MonoBehaviour
{
	public Vector3 FinalPosition;

	public float MaxHeight;

	public float FlightTime;

	public float CollectableDelay;

	public GameObject AutoMagnetTarget;

	public bool MobMode;

	public bool DestroyWhenDone = true;

	public bool InitOnStart = true;

	protected Vector3 mInitialPosition;

	protected Vector3 mInitialVelocity;

	protected float mTime;

	protected float mGravity;

	protected bool mActive;

	public bool IsCollectable
	{
		get
		{
			if (FMath.Epsilon <= CollectableDelay)
			{
				return CollectableDelay <= mTime;
			}
			return true;
		}
	}

	private void Start()
	{
		if (InitOnStart)
		{
			Init();
		}
	}

	public void Init()
	{
		Vector3 position = base.gameObject.transform.position;
		mInitialPosition = position;
		MaxHeight = Mathf.Max(MaxHeight, Mathf.Max(FinalPosition.y, position.y));
		if (FlightTime <= 0.001f)
		{
			mInitialPosition = Vector3.zero;
			base.gameObject.transform.position = FinalPosition;
		}
		else
		{
			float num = MaxHeight - position.y;
			float num2 = 2f * (MaxHeight + Mathf.Sqrt((MaxHeight - FinalPosition.y) * num) - position.y) / FlightTime;
			mInitialVelocity = new Vector3((FinalPosition.x - position.x) / FlightTime, num2, (FinalPosition.z - position.z) / FlightTime);
			mGravity = ((!(num > 0.001f)) ? (2f * (position.y - FinalPosition.y) / (FlightTime * FlightTime)) : (num2 * num2 * 0.5f / num));
		}
		mTime = 0f;
		mActive = true;
	}

	private void Update()
	{
		if (!mActive)
		{
			return;
		}
		mTime += Time.deltaTime;
		if (FlightTime <= mTime)
		{
			base.gameObject.transform.position = FinalPosition;
			if (!DestroyWhenDone)
			{
				mActive = false;
			}
			else
			{
				Object.Destroy(this);
			}
		}
		else
		{
			base.gameObject.transform.position = new Vector3(mInitialPosition.x + mTime * mInitialVelocity.x, mInitialPosition.y + mTime * mInitialVelocity.y + mTime * mTime * mGravity * -0.5f, mInitialPosition.z + mTime * mInitialVelocity.z);
		}
	}
}
