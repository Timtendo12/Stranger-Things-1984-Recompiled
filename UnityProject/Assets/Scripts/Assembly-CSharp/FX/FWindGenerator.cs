using UnityEngine;

public class FWindGenerator : MonoBehaviour
{
	[Range(0f, 1f)]
	public float MinNormalSpeed;

	[Range(0f, 1f)]
	public float MaxNormalSpeed;

	[Range(0f, 1f)]
	public float MinGustSpeed;

	[Range(0f, 1f)]
	public float MaxGustSpeed;

	public float MinTimeBeforeVariation;

	public float MaxTimeBeforeVariation;

	public float MinTimeBeforeGust;

	public float MaxTimeBeforeGust;

	public float MinGustTime;

	public float MaxGustTime;

	public float TransitionTime;

	protected float mChangeTimer;

	protected float mGustTimer;

	protected float mTransitionTimer;

	protected float mTransitionToSpeed;

	protected float mTransitionFromSpeed;

	protected bool mGust;

	[ReadOnlyInspector]
	public float CurrentSpeed;

	protected static FWindGenerator mInstance;

	public static float CurrentWindSpeed => 0f;

	public static FWindGenerator Instance => null;

	public void Awake()
	{
	}

	public void OnDestroy()
	{
	}

	protected void TransitionToSpeed(float newSpeed)
	{
	}

	protected void PickNormalSpeed()
	{
	}

	protected void PickGustSpeed()
	{
	}

	public void Update()
	{
	}
}
