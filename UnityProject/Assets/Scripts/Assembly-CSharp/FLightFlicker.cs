using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FLightFlicker @ 0x178c0e8..0x178c22c, Ghidra vaddr).
[ExecuteInEditMode]
public class FLightFlicker : MonoBehaviour
{
	public Light LightToControl;

	[Range(0f, 8f)]
	public float MinIntensity;

	[Range(0f, 8f)]
	public float MaxIntensity = 2f;

	[Range(0.01f, 1f)]
	public float MinIntensityTime = 0.05f;

	[Range(0.01f, 1f)]
	public float MaxIntensityTime = 0.3f;

	protected float TimeTilIntensityChange;

	protected float IntensityChangeRate;

	public void Update()
	{
		TimeTilIntensityChange -= Time.deltaTime;
		if (TimeTilIntensityChange >= 0f)
		{
			LightToControl.intensity += IntensityChangeRate * Time.deltaTime;
			return;
		}
		float num = FRandom.Generic.RangeFloat(MinIntensity, MaxIntensity);
		TimeTilIntensityChange = FRandom.Generic.RangeFloat(MinIntensityTime, MaxIntensityTime);
		IntensityChangeRate = (num - LightToControl.intensity) / TimeTilIntensityChange;
	}
}
