using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FUnitAIEnemySentry @ 0x174a1f0..0x174a860, Ghidra vaddr).
public class FUnitAIEnemySentry : FUnitAIEnemy
{
	// Beam start offsets per facing (LEFT, UP, RIGHT, DOWN), read from the binary (0x89a2f0 / 0x89a3b0).
	private static readonly float[] cBeamOffsetX = new float[4] { 0.06f, -0.33f, -0.06f, 0.29f };

	private static readonly float[] cBeamOffsetY = new float[4] { -0.2f, -0.15f, -0.2f, 0f };

	protected bool mReportedPlayer;

	protected FLightBeam mBeam;

	public FUnitAIEnemySentry(FUnit owner)
		: base(owner)
	{
		mShouldAggro = false;
	}

	protected override void Init()
	{
		base.Init();
		mBeam = new FLightBeam(mOwner, true, "NewStart", "NewMiddle", "NewEnd", 3f);
	}

	public override void Update(float deltaTime)
	{
		if (!mReportedPlayer)
		{
			base.Update(deltaTime);
		}
		if (mBeam == null)
		{
			return;
		}
		if (mPayingAttention)
		{
			if (!mBeam.Visible)
			{
				mBeam.SetVisible(true);
			}
			float offX = 0f;
			float offY = 0f;
			uint facingIndex = (uint)((int)mOwner.Facing - 1);
			if (facingIndex < 4)
			{
				offX = cBeamOffsetX[facingIndex];
				offY = cBeamOffsetY[facingIndex];
			}
			Vector3 pos = mOwner.Position;
			Vector2 forward = mOwner.Forward();
			FUnit hit = mBeam.Update(deltaTime, new Vector2(pos.x + forward.x * 0.5f, pos.y + forward.y * 0.5f), new Vector2(offX, offY));
			if (!mReportedPlayer && hit != null && hit.IsPlayer)
			{
				ReportPlayer();
			}
		}
		else if (mBeam.Visible)
		{
			mBeam.SetVisible(false);
		}
	}

	protected override void AddIdleAction(float deltaTime)
	{
		if (State == EnemyState.AGGROD)
		{
			ReportPlayer();
			return;
		}
		AddAction(new FUnitActionEnemyIdleBase(mOwner.World.GetNextActionID(), mOwner));
		mPayingAttention = true;
	}

	protected void ReportPlayer()
	{
		FWorld.Player.EnableInput(false);
		FWorld.Player.AI.StopActions();
		FWorld.Player.PlayAnim("Hit", true, null, false);
		FWorld.Player.ShowEmote("Anger", true);
		mReportedPlayer = true;
		mOwner.PlayAnim("Alert", false);
		mOwner.ShowEmote("Surprise", false);
		FGameUtil.DelayExecute(2f, ResetPlayer);
	}

	protected void ResetPlayer()
	{
		mOwner.World.ResetPlayerNoSwitchRooms();
	}

	public override void ReleaseStuff()
	{
		if (mBeam != null)
		{
			mBeam.ReleaseStuff();
		}
	}

	public override bool ProcessMessage(FUnitMessage m)
	{
		if (base.ProcessMessage(m))
		{
			return true;
		}
		if (m.Message != "Attacked")
		{
			return false;
		}
		if (m.SenderID == FWorld.Player.ID)
		{
			Vector3 playerPos = FWorld.Player.Position;
			mOwner.FacePosition(new Vector2(playerPos.x, playerPos.y));
			ReportPlayer();
		}
		return true;
	}
}
