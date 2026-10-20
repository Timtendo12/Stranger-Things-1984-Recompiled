public enum AnimTagTypes
{
	Unknown = -1,
	[Creates(typeof(FAnimTagWork))]
	Work = 0,
	[Creates(typeof(FAnimTagAttack))]
	Attack = 1,
	[Creates(typeof(FAnimTagFakeAttack))]
	FakeAttack = 2,
	[Creates(typeof(FAnimTagHeal))]
	Heal = 3,
	[Creates(typeof(FAnimTagSound))]
	Sound = 4,
	[Creates(typeof(FAnimTagSpawn))]
	Spawn = 5,
	[Creates(typeof(FAnimTagData))]
	Data = 6,
	[Creates(typeof(FAnimTagCallMethod))]
	CallMethod = 7,
	[Creates(typeof(FAnimTagImpactEffect))]
	ImpactEffect = 8,
	[Creates(typeof(FAnimTagPowerChargeEffect))]
	PowerChargeEffect = 9,
	[Creates(typeof(FAnimTagPowerStartEffect))]
	PowerStartEffect = 10,
	[Creates(typeof(FAnimTagPowerEndEffect))]
	PowerEndEffect = 11,
	[Creates(typeof(FAnimTagReact))]
	React = 12,
	[Creates(typeof(FAnimTagFootstep))]
	Footstep = 13,
	[Creates(typeof(FAnimTagDetachSkinnedAttachment))]
	DetachSkinnedAttachment = 14,
	[Creates(typeof(FAnimTagDetachSkinnedAttachmentsByTag))]
	DetachSkinnedAttachmentsByTag = 15,
	[Creates(typeof(FAnimTagDetachSkinnedAttachments))]
	DetachSkinnedAttachments = 16,
	[Creates(typeof(FAnimTagTargetAnimation))]
	TargetAnimation = 17,
	[Creates(typeof(FAnimTagLaunchProjectile))]
	LaunchProjectile = 18,
	[Creates(typeof(FAnimTagSequence))]
	Sequence = 19,
	[Creates(typeof(FAnimTagBlock))]
	Block = 20,
	[Creates(typeof(FAnimTagRemoveUnit))]
	RemoveUnit = 21,
	[Creates(typeof(FAnimTagDropLoot))]
	DropLoot = 22,
	[Creates(typeof(FAnimTagSetAlive))]
	SetAlive = 23,
	[Creates(typeof(FAnimTagMoveToPatrolPoint))]
	MoveToPatrolPoint = 24,
	[Creates(typeof(FAnimTagGrantQuest))]
	GrantQuest = 25,
	[Creates(typeof(FAnimTagProcessMessage))]
	ProcessMessage = 26,
	[Creates(typeof(FAnimTagCompleteQuest))]
	CompleteQuest = 27,
	[Creates(typeof(FAnimTagPlayAnimation))]
	PlayAnimation = 28,
	[Creates(typeof(FAnimTagTriggerUnit))]
	TriggerUnit = 29,
	[Creates(typeof(FAnimTagSpawnEffect))]
	SpawnEffect = 30,
	[Creates(typeof(FAnimTagFillPlayerHealth))]
	FillPlayerHealth = 31
}
