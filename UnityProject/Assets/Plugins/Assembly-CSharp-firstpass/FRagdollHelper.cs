using System.Collections.Generic;
using UnityEngine;

public class FRagdollHelper : MonoBehaviour
{
	public List<Rigidbody> IgnoreRigidBodies;

	public bool StartKinematic;

	protected List<Rigidbody> mRigidBodies;

	public void Awake()
	{
	}

	public void SetKinematic(bool kinematic)
	{
	}
}
