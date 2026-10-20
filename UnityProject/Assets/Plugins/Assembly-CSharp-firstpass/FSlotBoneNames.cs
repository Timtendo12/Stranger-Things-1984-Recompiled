using System.Collections.Generic;
using UnityEngine;

public class FSlotBoneNames : MonoBehaviour
{
	protected Dictionary<string, List<string>> SlotBoneNames;

	protected Dictionary<string, string> SlotBoneMesheNames;

	public void AddSlotInfo(string slotName, string[] boneNames)
	{
	}

	public List<string> GetBoneNamesForSlot(string slotName)
	{
		return null;
	}

	public void AddSlotMeshName(string slotName, string meshname)
	{
	}

	public string GetSlotMeshName(string slotName)
	{
		return null;
	}
}
