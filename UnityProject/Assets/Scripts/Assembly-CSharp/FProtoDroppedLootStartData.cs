using System;

// Rewritten from libil2cpp.so 1.0.605 (FProtoDroppedLootStartData @ 0x18269bc..0x1826a5c, Ghidra vaddr).
[Serializable]
public class FProtoDroppedLootStartData : FProtoUnitStartData
{
	public bool IsDroppedLoot { get; set; }

	public FPoint2 PositionSpawnedFrom { get; set; }

	public bool AutoAwardToPlayer { get; set; }

	public FProtoDroppedLootStartData()
	{
		IsDroppedLoot = false;
		AutoAwardToPlayer = false;
		PositionSpawnedFrom = FPoint2.invalid;
	}
}
