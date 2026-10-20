using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FForceInArea : MonoBehaviour
{
	public enum AreaTypes
	{
		Sphere = 0,
		Capsule = 1,
		Box = 2
	}

	public enum ApplicationTypes
	{
		OneTimeImpulse = 0,
		ContinuousForce = 1
	}

	public enum FalloffModes
	{
		None = 0,
		DistanceFromCenter = 1
	}

	public enum CurveType
	{
		Constant = 0,
		Curve = 1
	}

	public enum ForceShapes
	{
		Linear = 0,
		AwayFromCenter = 1
	}

	[CompilerGenerated]
	private sealed class _003CStartupOneShot_003Ed__20 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public FForceInArea _003C_003E4__this;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return null;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return null;
			}
		}

		[DebuggerHidden]
		public _003CStartupOneShot_003Ed__20(int _003C_003E1__state)
		{
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
		}
	}

	public AreaTypes AreaType;

	public float Radius;

	public float Length;

	public float Width;

	public float Height;

	public ApplicationTypes ApplicationType;

	public FalloffModes FalloffMode;

	public CurveType ForceCurveType;

	public float Force;

	public AnimationCurve ForceCurve;

	public ForceShapes ForceShape;

	[Range(0f, 1f)]
	public float UpwardOffset;

	public Transform ForceDirection;

	protected double mElapsedTime;

	public void Start()
	{
	}

	[IteratorStateMachine(typeof(_003CStartupOneShot_003Ed__20))]
	protected IEnumerator StartupOneShot()
	{
		return null;
	}

	public void FireOneShot()
	{
	}

	public void FixedUpdate()
	{
	}

	public void Apply()
	{
	}

	public float GetDistanceToCenter(Vector3 distanceFromPoint, out Vector3 point)
	{
		point = default;
		return 0f;
	}

	public HashSet<Rigidbody> GetAffectedRigidBodies()
	{
		return null;
	}

	protected void AddColliders(Collider[] colliders, HashSet<Rigidbody> rigidBodies)
	{
	}
}
