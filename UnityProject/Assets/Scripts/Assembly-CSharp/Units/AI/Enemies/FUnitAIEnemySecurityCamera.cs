using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIEnemySecurityCamera @ 0x1748e0c..0x174a1f0, Ghidra vaddr).
public class FUnitAIEnemySecurityCamera : FUnitAIBase
{
	protected const float cSpotlightDist = 2f;

	protected const float mBaseWaitTime = 3f;

	protected bool mReportedPlayer;

	protected Vector2 mRaycastPos = Vector2.zero;

	protected FLightBeam mBeam;

	protected bool mWatchingForPlayer = true;

	public FUnitAIEnemySecurityCamera(FUnit owner)
		: base(owner)
	{
	}

	protected override void Init()
	{
		base.Init();
		Vector3 pos = mOwner.Position;
		mRaycastPos = new Vector2(pos.x, pos.y) + Vector2.down + mOwner.Forward();
		bool watching;
		if (mOwner.HasSavedUnitInfo("Watching"))
		{
			watching = mOwner.GetSavedUnitInfo<bool>("Watching");
			mWatchingForPlayer = watching;
		}
		else
		{
			watching = mWatchingForPlayer;
		}
		SetWatching(watching);
		if (mBeam == null)
		{
			mBeam = new FLightBeam(mOwner, false, "Start", "Middle", "End", 2f);
		}
	}

	public override void Update(float deltaTime)
	{
		if (mReportedPlayer)
		{
			return;
		}
		base.Update(deltaTime);
		if (!mWatchingForPlayer)
		{
			return;
		}
		RaycastHit2D[] hits = Physics2D.RaycastAll(mRaycastPos, mOwner.Forward(), 2f);
		for (int i = 0; i < hits.Length; i++)
		{
			Collider2D collider = hits[i].collider;
			if (collider == null || collider.isTrigger)
			{
				continue;
			}
			FTile tile = mOwner.World.GetTileFromGO(collider.gameObject);
			if (tile != null && !tile.TerrainPassable)
			{
				break;
			}
			FUnit unit = mOwner.World.GetUnitFromGO(collider.gameObject);
			if (unit != null && unit.IsPlayer)
			{
				ReportPlayer();
				break;
			}
		}
		if (mBeam != null)
		{
			mBeam.Update(deltaTime, mRaycastPos);
		}
	}

	public override void ReleaseStuff()
	{
		if (mBeam != null)
		{
			mBeam.ReleaseStuff();
		}
	}

	protected void ReportPlayer()
	{
		FApp.SoundManager.PlaySoundSet("Sentry_Alarm");
		FWorld.Player.EnableInput(false);
		FWorld.Player.AI.StopActions();
		FWorld.Player.PlayAnim("Hit", true, null, false);
		FWorld.Player.ShowEmote("Anger", true);
		mReportedPlayer = true;
		mOwner.ShowEmote("Surprise", false);
		FGameUtil.DelayExecute(2f, ResetPlayer);
	}

	protected void ResetPlayer()
	{
		mOwner.World.ResetPlayerNoSwitchRooms();
	}

	protected void SetWatching(bool watching)
	{
		string idle;
		if (!watching)
		{
			mWatchingForPlayer = false;
			if (mBeam != null)
			{
				mBeam.SetVisible(false);
			}
			idle = "Idle_Off";
		}
		else
		{
			mWatchingForPlayer = true;
			if (mBeam != null)
			{
				// 0x1749108..0x1749130: the shipped call leaves a stale register in the deltaTime slot; FLightBeam.Update never reads deltaTime, so 0 is equivalent.
				mBeam.Update(0f, mRaycastPos);
				mBeam.SetVisible(true);
			}
			idle = "Idle";
		}
		mOwner.SetIdle(idle, true, true);
		mOwner.SetSavedUnitInfo("Watching", (object)watching);
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (base.ProcessMessage(m))
		{
			return true;
		}
		if (m.Message != "Trigger")
		{
			return false;
		}
		if (!(mOwner.World.CurrentRoom != mOwner.Room) || m.Data.Contains("NoReveal"))
		{
			SetWatching(!mWatchingForPlayer);
			return true;
		}
		mOwner.World.TryTriggerReveal(mOwner, m.Message);
		return true;
	}
}
