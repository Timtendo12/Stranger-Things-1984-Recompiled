using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FLightBeam @ 0x17491e4..0x174a868, Ghidra vaddr).
public class FLightBeam
{
	public const float cSpotlightDist = 3f;

	public const int cFramesBetweenLightUpdate = 5;

	protected float mSpotlightDist;

	protected FUnit mOwner;

	protected FAnimationController mSpotlightStart;

	protected FAnimationController mSpotlightMiddle;

	protected FAnimationController mSpotlightEnd;

	protected bool mVisible = true;

	protected bool mHitUnits = true;

	protected int mLightUpdateTimer = 5;

	private RaycastHit2D[] mHitInfos = new RaycastHit2D[20];

	public bool Visible => mVisible;

	public FLightBeam(FUnit owner, bool hitUnits, string start = "Start", string middle = "Middle", string end = "End", float spotlightDist = 3f)
	{
		mOwner = owner;
		mSpotlightDist = spotlightDist;
		mHitUnits = hitUnits;
		if (mSpotlightStart != null && mSpotlightMiddle != null && mSpotlightEnd != null)
		{
			return;
		}
		mSpotlightStart = SpawnPart(start);
		mSpotlightMiddle = SpawnPart(middle);
		mSpotlightEnd = SpawnPart(end);
		SetVisible(visible: true);
	}

	private FAnimationController SpawnPart(string animName)
	{
		FAnimationController fAnimationController = mOwner.World.SpawnDiscreteAnimation("LightBeam", 100, animName);
		if (fAnimationController != null)
		{
			Transform transform = fAnimationController.GO.transform;
			transform.parent = mOwner.GO.transform;
			transform.SetAsFirstSibling();
		}
		return fAnimationController;
	}

	public FUnit Update(float deltaTime, Vector2? raycastPos = null, Vector2? offset = null)
	{
		if (!raycastPos.HasValue)
		{
			Vector3 position = mOwner.Position;
			raycastPos = new Vector2(position.x, position.y);
		}
		if (!offset.HasValue)
		{
			offset = Vector2.zero;
		}
		if (mSpotlightStart == null || mSpotlightMiddle == null || mSpotlightEnd == null)
		{
			return null;
		}
		float num = mSpotlightDist;
		mLightUpdateTimer--;
		if (mLightUpdateTimer < 1)
		{
			mLightUpdateTimer = 5;
		}
		Vector2 origin = raycastPos.Value;
		Vector2 forward = mOwner.Forward();
		num += 0.25f;
		Vector2 endPoint = origin + forward * num;
		FUnit result = null;
		int num2 = Physics2D.RaycastNonAlloc(origin, forward, mHitInfos, num);
		for (int num3 = num2 - 1; num3 >= 0; num3--)
		{
			RaycastHit2D hit = mHitInfos[num3];
			if (hit.collider.isTrigger)
			{
				continue;
			}
			FTile tileFromGO = mOwner.World.GetTileFromGO(hit.collider.gameObject);
			if (tileFromGO != null)
			{
				if (tileFromGO.TerrainPassable)
				{
					continue;
				}
				endPoint = hit.point;
			}
			if (mHitUnits)
			{
				FUnit unitFromGO = mOwner.World.GetUnitFromGO(hit.collider.gameObject);
				if (unitFromGO != null)
				{
					if (unitFromGO.Obstructs)
					{
						result = unitFromGO;
						endPoint = hit.point;
					}
					else
					{
						result = null;
					}
				}
			}
		}
		mSpotlightStart.SetFacing(mOwner.Facing);
		mSpotlightMiddle.SetFacing(mOwner.Facing);
		mSpotlightEnd.SetFacing(mOwner.Facing);
		Vector2 off = offset.Value;
		mSpotlightStart.GO.transform.position = new Vector3(origin.x + off.x, origin.y + off.y, 0f);
		mSpotlightEnd.GO.transform.position = new Vector3(endPoint.x + off.x, endPoint.y + off.y, 0f);
		Vector3 startPos = mSpotlightStart.GO.transform.position;
		Vector3 endPos = mSpotlightEnd.GO.transform.position;
		Vector3 delta = endPos - startPos;
		mSpotlightMiddle.GO.transform.position = startPos + delta * 0.5f;
		mSpotlightMiddle.GO.transform.localScale = new Vector3(delta.magnitude * Vector3.one.x, Vector3.one.y, Vector3.one.z);
		return result;
	}

	public void SetVisible(bool visible)
	{
		mSpotlightStart.GO.SetActive(visible);
		mSpotlightMiddle.GO.SetActive(visible);
		mSpotlightEnd.GO.SetActive(visible);
		mVisible = visible;
	}

	public void ReleaseStuff()
	{
		Object.Destroy(mSpotlightStart.GO);
		Object.Destroy(mSpotlightMiddle.GO);
		Object.Destroy(mSpotlightEnd.GO);
	}
}
