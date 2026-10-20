using UnityEngine;

public class FWindFXController : MonoBehaviour
{
	[Range(0f, 1f)]
	public float EnableAboveWindSpeed;

	[Range(0f, 1f)]
	public float DisableBelowWindSpeed;

	public float MinEnabledTime;

	public float MinDisabledTime;

	protected bool mIsActive;

	protected float mStateChangeLockTimer;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Deactivate()
	{
	}

	public void Activate()
	{
	}
}
