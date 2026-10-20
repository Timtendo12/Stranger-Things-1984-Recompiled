using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (HackMotion @ 0x17e7be0..0x17e7d70, Ghidra vaddr).
public class HackMotion : MonoBehaviour
{
	public float Distance = 1f;

	protected Vector3 StartPos;

	protected float Offset;

	private void Start()
	{
		StartPos = base.gameObject.transform.position;
		Offset = FRandom.Generic.RangeFloat(0f, 2f);
	}

	private void Update()
	{
		Transform transform = base.gameObject.transform;
		Vector3 forward = base.gameObject.transform.forward;
		float num = Distance * 0.5f;
		transform.position = StartPos + forward * num * Mathf.Sin(Offset + Time.realtimeSinceStartup);
	}
}
