using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FCameraLerp @ 0x16f1c04..0x16f2160, Ghidra vaddr).
public class FCameraLerp : MonoBehaviour
{
	protected Camera mCamera;

	protected Vector3 mTargetPosition;

	protected Vector3 mLookAtPosition;

	protected Quaternion mTargetRotation;

	protected bool mUseLookAt;

	protected float mDuration = 1f;

	protected float mTimeRemaining;

	protected bool mInterpolating;

	protected Vector3 mStartPosition;

	protected Quaternion mStartRotation;

	protected Func<float, float, float, float, float> mEaseFunction = GoEaseLinear.EaseNone;

	public bool IsInterpolating => mInterpolating;

	public void Awake()
	{
		mCamera = GetComponent<Camera>();
	}

	public void Update()
	{
		if (!mInterpolating)
		{
			return;
		}
		mTimeRemaining -= Time.deltaTime;
		if (mTimeRemaining < 0f)
		{
			mTimeRemaining = 0f;
			mInterpolating = false;
		}
		if (mEaseFunction != null)
		{
			float num = mEaseFunction(mTimeRemaining, 0f, 1f, mDuration);
			float t = Mathf.Clamp01(1f - num);
			Vector3 position = Vector3.Lerp(mStartPosition, mTargetPosition, t);
			mCamera.transform.position = position;
			if (!mUseLookAt)
			{
				mCamera.transform.rotation = Quaternion.Slerp(mStartRotation, mTargetRotation, t);
				return;
			}
			Vector3 normalized = (mLookAtPosition - mCamera.transform.position).normalized;
			Quaternion b = Quaternion.LookRotation(normalized);
			mCamera.transform.rotation = Quaternion.Slerp(mCamera.transform.rotation, b, t);
		}
	}

	public void StartLerp(Vector3 targetPos, Vector3 lookAtPos, float duration, GoEaseType easeType = GoEaseType.QuadInOut)
	{
		mStartPosition = mCamera.transform.position;
		mStartRotation = mCamera.transform.rotation;
		mTargetPosition = targetPos;
		mLookAtPosition = lookAtPos;
		mDuration = duration;
		mTimeRemaining = duration;
		mInterpolating = true;
		mUseLookAt = true;
		mEaseFunction = GoTweenUtils.easeFunctionForType(easeType);
	}

	public void StartLerp(Vector3 targetPos, Quaternion targetRotation, float duration, GoEaseType easeType = GoEaseType.QuadInOut)
	{
		mStartPosition = mCamera.transform.position;
		mStartRotation = mCamera.transform.rotation;
		mTargetPosition = targetPos;
		mTargetRotation = targetRotation;
		mDuration = duration;
		mTimeRemaining = duration;
		mInterpolating = true;
		mUseLookAt = false;
		mEaseFunction = GoTweenUtils.easeFunctionForType(easeType);
	}
}
