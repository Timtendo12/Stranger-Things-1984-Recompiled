using System.Collections.Generic;
using UnityEngine;

public class FAnimToCurveHelper : MonoBehaviour, ISerializationCallbackReceiver
{
	public AnimationCurve ColorR;

	public AnimationCurve ColorG;

	public AnimationCurve ColorB;

	public AnimationCurve ColorA;

	public AnimationCurve PositionX;

	public AnimationCurve PositionY;

	public AnimationCurve PositionZ;

	public AnimationCurve ScaleX;

	public AnimationCurve ScaleY;

	public AnimationCurve ScaleZ;

	public float Length;

	protected static Dictionary<string, string> sRemapNames;

	public void OnAfterDeserialize()
	{
	}

	public void OnBeforeSerialize()
	{
	}
}
