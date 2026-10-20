using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FCamera @ 0x16f1994..0x16f1c04, Ghidra vaddr).
public class FCamera : MonoBehaviour
{
	public enum Modes
	{
		None = 0,
		World = 1
	}

	public Modes Mode;

	public float Damping = 0.1f;

	public float OrthoDamping = 0.25f;

	public float OrthoSizeHorizontal = 4f;

	public float OrthoSizeVertical = 5f;

	protected Vector3 mDesiredPosition;

	protected Vector3 mSmoothDampVelocity = Vector3.zero;

	protected float mSmoothOrthoVelocity;

	protected bool mTransitionToDesiredPosition;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	public float GetOrthoSize()
	{
		if (PCDisplay.Width > PCDisplay.Height)
		{
			return OrthoSizeHorizontal;
		}
		return OrthoSizeVertical;
	}

	public void Initialize()
	{
		Mode = Modes.World;
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		if (mTransitionToDesiredPosition)
		{
			base.transform.position = Vector3.SmoothDamp(base.transform.position, mDesiredPosition, ref mSmoothDampVelocity, Damping, float.PositiveInfinity, deltaTime);
			if (!(Vector3.Distance(base.transform.position, mDesiredPosition) >= 1f))
			{
				mTransitionToDesiredPosition = false;
			}
		}
	}

	public void TransitionTo(Vector3 point)
	{
		mDesiredPosition = point;
		mTransitionToDesiredPosition = true;
	}

	public void SetPosition(Vector3 point)
	{
		base.transform.position = point;
	}
}
