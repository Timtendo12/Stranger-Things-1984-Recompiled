using UnityEngine;

public class FAnimStateAnimRunner : FAnimState
{
	public AnimationState AnimState { get; set; }

	public override float Duration => 0f;

	public override float CurrentTime => 0f;

	public override float Speed => 0f;

	public override bool Enabled => false;
}
