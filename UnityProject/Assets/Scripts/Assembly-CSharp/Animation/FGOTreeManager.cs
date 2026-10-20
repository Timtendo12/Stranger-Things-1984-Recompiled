using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FGOTreeManager
{
	[CompilerGenerated]
	private sealed class _003CPlayAnimAtEndOfFrame_003Ed__36 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public GameObject target;

		public string anim;

		public float startAnimTimeNormalized;

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
		public _003CPlayAnimAtEndOfFrame_003Ed__36(int _003C_003E1__state)
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

	public static Dictionary<string, FGOTreeBuilder> mBuilders;

	public static FGOTreeInstance mTempTreeInstance;

	public static string mDefaultMeshName;

	public static string mBaseSlotName;

	public static bool mAttachmentPhase;

	public static Vector3 mPosition;

	public static List<FReplaceTextureEntry> mTempReplaceTextures;

	public static List<FReplaceMaterialEntry> mTempReplaceMaterials;

	public static List<FTransformColorEntry> mTempTransformColors;

	public static Dictionary<GameObject, string> mSlotPerGameObject;

	public static FGOTreeItemInterface mTempItem;

	public static void Init()
	{
	}

	public static void Define(string name, FGOTreeBuilder.BuildMethodDelegate buildMethod)
	{
	}

	public static FGOTreeBuilder GetBuilder(string name)
	{
		return null;
	}

	public static bool IsLeftFacing()
	{
		return false;
	}

	public static GameObject Base(string name, bool mirrorX = false, string attachToMeshName = null)
	{
		return null;
	}

	public static void ReplaceTexture(string oldTexture, string newTexture)
	{
	}

	public static void ReplaceMaterial(string oldMaterial, string newMaterial)
	{
	}

	public static GameObject Attachment(string attachPointName, string attachmentName)
	{
		return null;
	}

	public static GameObject SkinnedAttachmentOrBase(string attachmentName, string attachToMeshName = null)
	{
		return null;
	}

	public static GameObject SkinnedAttachment(string attachmentName, string attachToMeshName, params string[] tags)
	{
		return null;
	}

	public static GameObject SlotBoneNames(string slotName, params string[] boneNameList)
	{
		return null;
	}

	public static void Anchor(float x, float y, float z)
	{
	}

	public static void CanonicalSize(float x, float y, float z)
	{
	}

	public static void TransformColor1(Color color, GameObject root = null)
	{
	}

	public static void TransformColor2(Color color, GameObject root = null)
	{
	}

	public static void TransformColor3(Color color, GameObject root = null)
	{
	}

	public static void TransformColor1(string hexColor, GameObject root = null)
	{
	}

	public static void TransformColor2(string hexColor, GameObject root = null)
	{
	}

	public static void TransformColor3(string hexColor, GameObject root = null)
	{
	}

	public static Color GetColorByNameOrHex(string str)
	{
		return default;
	}

	public static void AnimDef(string animDefName)
	{
	}

	public static void ScaleRelative(float scale)
	{
	}

	public static void ScaleAbsolute(float scale)
	{
	}

	public static FGOTreeInstance Build(string name, FGOTreeItemInterface item, FRandom random, Vector3 position = default(Vector3), int playAnimType = 0, float startAnimTimeNormalized = 0f)
	{
		return null;
	}

	protected static FGOTreeInstance Build(FGOTreeBuilder builder, FGOTreeItemInterface item, FRandom random, Vector3 position = default(Vector3), int playAnimType = 0, float startAnimTimeNormalized = 0f)
	{
		return null;
	}

	[IteratorStateMachine(typeof(_003CPlayAnimAtEndOfFrame_003Ed__36))]
	public static IEnumerator PlayAnimAtEndOfFrame(GameObject target, string anim, float startAnimTimeNormalized)
	{
		return null;
	}

	[GOTreeInit]
	public static void InitParts()
	{
	}

	[GOTreeInit]
	public static void InitUnits()
	{
	}
}
