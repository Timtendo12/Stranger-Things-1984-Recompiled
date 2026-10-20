using System;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FWeaponObj @ 0x1753fc0..0x1755170, Ghidra vaddr).
public class FWeaponObj : MonoBehaviour
{
	public float ExistTime;

	public float CollideRadius;

	public FUnit Target;

	public FUnit Owner;

	public Vector2 MoveVector;

	public FProtoWeapon ProtoWeapon;

	public FAnimationController mAnimController;

	public Vector2 LandPosition;

	public float StunTime;

	protected bool mHitSomething;

	protected float mExistUntilTime = -1f;

	private RaycastHit2D[] mHits = new RaycastHit2D[20];

	private void Start()
	{
		if (ExistTime > -1f)
		{
			mExistUntilTime = ExistTime + Owner.World.Gametime;
		}
		DoCollision();
		mAnimController = TryPlayWeaponAnimation(ProtoWeapon.Visual, "Play");
	}

	private void FixedUpdate()
	{
		FWorld.GameState state = Owner.World.State;
		if (state == FWorld.GameState.PAUSED)
		{
			return;
		}
		if (state != FWorld.GameState.LOGIC_PAUSED)
		{
			if (Owner.World.Gametime >= mExistUntilTime)
			{
				UnityEngine.Object.Destroy(gameObject);
				return;
			}
			DoCollision();
			if (MoveVector.sqrMagnitude >= 1E-10f)
			{
				transform.Translate(MoveVector.x * Time.deltaTime, MoveVector.y * Time.deltaTime, 0f);
				if (LandPosition.sqrMagnitude >= 1E-10f)
				{
					Vector2 moveDir = MoveVector.normalized;
					Vector3 pos = transform.position;
					Vector2 toLand = new Vector2(LandPosition.x - pos.x, LandPosition.y - pos.y).normalized;
					if ((moveDir - toLand).sqrMagnitude >= 1E-10f)
					{
						transform.position = new Vector3(LandPosition.x, LandPosition.y, 0f);
						HitSomething();
						return;
					}
				}
			}
		}
		if (Owner.World.State != FWorld.GameState.ANIM_PAUSED && mAnimController != null)
		{
			mAnimController.Update(Time.deltaTime);
		}
	}

	protected FAnimationController TryPlayWeaponAnimation(string animDefName, string animName)
	{
		FAnimDef animDef = FAnimDefManager.GetAnimDef(animDefName);
		if (animDef == null)
		{
			return null;
		}
		int animTypeID = FAnimDefManager.GetAnimTypeID(animName);
		if (animTypeID < 0)
		{
			return null;
		}
		FAnimationController controller = new FAnimationController(animDefName, animDef, transform.GetChild(0).gameObject);
		controller.PlayAnim(animTypeID, true, null, false, true);
		controller.SetFacing(Owner.Facing);
		return controller;
	}

	protected void DoCollision()
	{
		Vector3 pos = transform.position;
		int count = Physics2D.CircleCastNonAlloc(new Vector2(pos.x, pos.y), CollideRadius, MoveVector.normalized, mHits, MoveVector.magnitude * Time.deltaTime);
		if (count <= 0)
		{
			return;
		}
		bool hitSomething = false;
		for (int i = 0; i < count; i++)
		{
			RaycastHit2D hit = mHits[i];
			Collider2D collider = hit.collider;
			if (collider.isTrigger)
			{
				continue;
			}
			FUnit unit = FApp.World.GetUnitFromGO(collider.gameObject);
			if (ShouldHitUnit(unit))
			{
				DoHitUnit(unit);
			}
			else
			{
				if (hitSomething)
				{
					HitSomething();
					return;
				}
				FTile tile = FApp.World.GetTileFromGO(collider.gameObject);
				if (tile == null || tile.TerrainPassable || tile.Hole)
				{
					hitSomething = false;
					continue;
				}
				Vector2 tilePos = tile.Position.ToVector2();
				Vector3 weaponPos = transform.position;
				// 0x17544f4..0x1754534: fabd |tile.x-pos.x| and |tile.y-pos.y| compared against 0.4 (@0x899d58); skipped when both are >= 0.4 (diagonal graze)
				if (Mathf.Abs(tilePos.x - weaponPos.x) >= 0.4f && Mathf.Abs(tilePos.y - weaponPos.y) >= 0.4f)
				{
					hitSomething = false;
					continue;
				}
				System.Collections.Generic.List<FUnit> tileUnits = tile.Units;
				for (int j = 0; j < tileUnits.Count; j++)
				{
					if (ShouldHitUnit(tileUnits[j]))
					{
						DoHitUnit(tileUnits[j]);
						break;
					}
				}
			}
			transform.position = new Vector3(hit.point.x, hit.point.y, 0f);
			hitSomething = true;
		}
		if (hitSomething)
		{
			HitSomething();
		}
	}

	protected void DoHitUnit(FUnit unit)
	{
		if (unit == null)
		{
			return;
		}
		if (StunTime > 0f && unit.IsTag("EnemyCharacter"))
		{
			unit.OnStunned(Owner, StunTime);
		}
		if (ProtoWeapon.Damage > 0)
		{
			unit.OnAttacked(Owner, null, false, ProtoWeapon);
		}
	}

	protected bool ShouldHitUnit(FUnit unit)
	{
		if (unit == null)
		{
			return false;
		}
		if (ProtoWeapon != null)
		{
			if (ProtoWeapon.Type == "Ranged" && !unit.ProtoUnit.BlocksProjectiles)
			{
				return false;
			}
			if (Target != unit && ProtoWeapon.OnlyDamageTarget)
			{
				return false;
			}
		}
		return (LandPosition.sqrMagnitude < 1E-10f || !unit.IsTag("EnemyCharacter")) && Owner != unit;
	}

	protected void HitSomething()
	{
		if (mHitSomething)
		{
			return;
		}
		if (Owner.World != null && ProtoWeapon != null && ProtoWeapon.ImpactFX.Valid())
		{
			FProtoUnit impactProto = FApp.ProtoUnitManager.GetProtoUnit(ProtoWeapon.ImpactFX, true);
			Vector3 pos = transform.position;
			FPoint2 point = new FPoint2((int)Math.Round((double)pos.x), (int)Math.Round((double)pos.y));
			FTile tile = Owner.Room.GetClosestOpenTile(point, true);
			string effectName;
			if (impactProto == null)
			{
				effectName = ProtoWeapon.ImpactFX;
			}
			else
			{
				if (tile != null && !(tile.Position != point))
				{
					int unitID = -1;
					Owner.World.SpawnUnitAtPosition(ProtoWeapon.ImpactFX, tile.Position, out unitID);
					mHitSomething = true;
					UnityEngine.Object.Destroy(gameObject);
					return;
				}
				effectName = ProtoWeapon.Name + "_Impact";
			}
			Owner.World.SpawnEffect(effectName, Owner.World.transform, new Vector2(pos.x, pos.y), 200);
		}
		mHitSomething = true;
		UnityEngine.Object.Destroy(gameObject);
	}
}
