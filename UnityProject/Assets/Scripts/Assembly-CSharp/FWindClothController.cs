using UnityEngine;

public class FWindClothController : MonoBehaviour
{
	public float AccelCurveMax;

	public AnimationCurve ExternalAccelerationX;

	public AnimationCurve ExternalAccelerationY;

	public AnimationCurve ExternalAccelerationZ;

	public AnimationCurve RandomAccelerationX;

	public AnimationCurve RandomAccelerationY;

	public AnimationCurve RandomAccelerationZ;

	protected Cloth mCloth;

	public void Awake()
	{
	}

	public void Update()
	{
	}
}
