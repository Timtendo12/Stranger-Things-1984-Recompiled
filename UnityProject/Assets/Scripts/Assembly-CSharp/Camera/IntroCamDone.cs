using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (IntroCamDone @ 0x17e8178..0x17e819c, Ghidra vaddr).
public class IntroCamDone : StateMachineBehaviour
{
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.enabled = false;
	}
}
