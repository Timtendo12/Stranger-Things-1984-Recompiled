using UnityEngine;

public class FGOTreeInstance
{
	public Vector3 mCanonicalSize;

	public GameObject Base { get; set; }

	public GameObject NonHolderBase { get; set; }

	public GameObject BasePrefab { get; set; }

	public Vector3 AnchorOffset { get; set; }

	public FAnimDef AnimDef { get; set; }

	public Vector3 CanonicalSize
	{
		get
		{
			return default;
		}
		set
		{
		}
	}
}
