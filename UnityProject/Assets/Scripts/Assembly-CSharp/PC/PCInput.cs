using System;
using System.Collections.Generic;
using UnityEngine;

// PC port (new): keyboard and gamepad layer on top of the touch / click-to-move input.
// In gameplay (the world HUD page is on top) it drives the player and the HUD buttons; everywhere else it moves a focus
// ring over the touchable colliders of the top page and "taps" the focused one. The mouse path is untouched.
// Legacy UnityEngine.Input only (the Input System package is not installed).
[DefaultExecutionOrder(100)]
public class PCInput : MonoBehaviour
{
	private const float Deadzone = 0.5f;

	private const float NavRepeatDelay = 0.3f;

	private const float NavRepeatRate = 0.12f;

	private const int MoveLookahead = 6;

	private static PCInput sInstance;

	private bool mAxesBroken;

	private bool mDpadBroken;

	private Vector2Int mHeldDir;

	private Vector2Int mLastIssuedDir;

	private bool mMoveIssued;

	private Vector2Int mNavDir;

	private float mNextNavTime;

	private bool mFocusVisible;

	private FUICollider mFocus;

	private FNode mFocusPage;

	private Vector3 mLastMouse;

	private FTouch mPendingRelease;

	private float mReleaseTime = -1f;

	private readonly List<FUICollider> mColliders = new List<FUICollider>();

	private readonly List<FNode> mStack = new List<FNode>();

	private readonly List<FUnit> mScratchUnits = new List<FUnit>();

	private Texture2D mWhite;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Bootstrap()
	{
		if (!Application.isPlaying || sInstance != null)
		{
			return;
		}
		GameObject go = new GameObject("PCInput");
		DontDestroyOnLoad(go);
		sInstance = go.AddComponent<PCInput>();
	}

	private static bool Pad(int button)
	{
		return Input.GetKeyDown(KeyCode.JoystickButton0 + button);
	}

	private static bool AnyKeyDown(params KeyCode[] keys)
	{
		for (int i = 0; i < keys.Length; i++)
		{
			if (Input.GetKeyDown(keys[i]))
			{
				return true;
			}
		}
		return false;
	}

	private float Axis(string name)
	{
		try
		{
			return Input.GetAxisRaw(name);
		}
		catch (ArgumentException)
		{
			return 0f;
		}
	}

	// Combined movement / navigation direction from keyboard, left stick and d-pad (4-way, dominant axis wins).
	private Vector2Int ReadDirection()
	{
		float x = 0f;
		float y = 0f;
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			x -= 1f;
		}
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			x += 1f;
		}
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
		{
			y -= 1f;
		}
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
		{
			y += 1f;
		}
		if (!mAxesBroken)
		{
			float sx = Axis("PCStickX");
			float sy = Axis("PCStickY");
			if (Mathf.Abs(sx) > Deadzone || Mathf.Abs(sy) > Deadzone)
			{
				if (Mathf.Abs(sx) >= Mathf.Abs(sy))
				{
					x += Mathf.Sign(sx);
				}
				else
				{
					y += Mathf.Sign(sy);
				}
			}
			float dx = Axis("PCDpadX");
			float dy = Axis("PCDpadY");
			if (Mathf.Abs(dx) > Deadzone)
			{
				x += Mathf.Sign(dx);
			}
			if (Mathf.Abs(dy) > Deadzone)
			{
				y += Mathf.Sign(dy);
			}
		}
		x = Mathf.Clamp(x, -1f, 1f);
		y = Mathf.Clamp(y, -1f, 1f);
		if (x != 0f && y != 0f)
		{
			// favour the most recently pressed axis; keep the previous direction when still held
			if (mHeldDir.x != 0 && mHeldDir.y == 0)
			{
				x = 0f;
			}
			else
			{
				y = 0f;
			}
		}
		return new Vector2Int((int)x, (int)y);
	}

	private void Update()
	{
		FHUD hud = FApp.HUD;
		if (hud == null || Futile.TouchManager == null)
		{
			return;
		}
		if (mReleaseTime >= 0f && Time.unscaledTime >= mReleaseTime)
		{
			mReleaseTime = -1f;
			mPendingRelease.Phase = TouchPhase.Ended;
			Futile.TouchManager.ProcessSingleTouch(mPendingRelease);
		}
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || (Input.mousePosition - mLastMouse).sqrMagnitude > 16f)
		{
			mFocusVisible = false;
		}
		mLastMouse = Input.mousePosition;
		if (PCDebugOverlay.IsOpen)
		{
			// the debug overlay has the keyboard: stop a held move and drop the focus ring
			mHeldDir = Vector2Int.zero;
			mMoveIssued = false;
			mFocusVisible = false;
			return;
		}
		FPageLegendWorld worldPage = hud.PageLegendWorld;
		FUIPage top = hud.TopPage;
		bool gameplay = top != null && top == worldPage && FApp.World != null && FWorld.Player != null && FWorld.Player.AI != null;
		Vector2Int dir = ReadDirection();
		bool confirm = AnyKeyDown(KeyCode.Space, KeyCode.Return, KeyCode.KeypadEnter, KeyCode.Z, KeyCode.J) || Pad(0);
		if (gameplay)
		{
			UpdateGameplay(worldPage, hud, dir, confirm);
		}
		else
		{
			mHeldDir = Vector2Int.zero;
			mMoveIssued = false;
			if (top != null)
			{
				UpdateMenu(hud, top, dir, confirm);
			}
		}
		mHeldDir = dir;
	}

	private void UpdateGameplay(FPageLegendWorld worldPage, FHUD hud, Vector2Int dir, bool confirm)
	{
		mFocus = null;
		mFocusVisible = false;
		if (worldPage.DialogueOnScreen)
		{
			if (confirm || AnyKeyDown(KeyCode.E, KeyCode.Q))
			{
				worldPage.PCAdvanceDialogue();
			}
			mMoveIssued = false;
			return;
		}
		if (AnyKeyDown(KeyCode.P) || Pad(7))
		{
			worldPage.ShowPauseMenu();
			return;
		}
		if (AnyKeyDown(KeyCode.I, KeyCode.B) || Pad(3))
		{
			worldPage.PCOpenInventory();
			return;
		}
		if (AnyKeyDown(KeyCode.M) || Pad(6))
		{
			worldPage.PCOpenMap();
			return;
		}
		if (AnyKeyDown(KeyCode.Q, KeyCode.E, KeyCode.Tab) || Pad(4) || Pad(5))
		{
			worldPage.PCCycleCharacter();
		}
		if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
		{
			worldPage.PCSelectCharacter(0);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
		{
			worldPage.PCSelectCharacter(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
		{
			worldPage.PCSelectCharacter(2);
		}
		if (AnyKeyDown(KeyCode.X, KeyCode.K, KeyCode.LeftControl) || Pad(2))
		{
			worldPage.PCSpecialAbility();
		}
		FUnit player = FWorld.Player;
		if (!FApp.World.InputEnabled || !player.AliveAndWell)
		{
			mMoveIssued = false;
			return;
		}
		if (confirm)
		{
			DoAction(player);
		}
		DoMovement(player, dir);
	}

	// Interact with / attack the nearest unit; with a ranged weapon and nothing near, fire the way the player faces.
	private void DoAction(FUnit player)
	{
		FRoom room = FApp.World.CurrentRoom;
		if (room == null)
		{
			return;
		}
		FUnit best = null;
		float bestScore = float.MaxValue;
		mScratchUnits.Clear();
		mScratchUnits.AddRange(room.Units);
		for (int i = 0; i < mScratchUnits.Count; i++)
		{
			FUnit u = mScratchUnits[i];
			if (u == null || u == player || u.IsPlayer || !u.AliveAndWell || !u.Interactable || !u.IsTag("Interactable"))
			{
				continue;
			}
			float d = Vector2.Distance(player.Position, u.Position);
			bool attackable = u.IsTag("Attackable");
			if (d > (attackable ? 6f : 2.5f))
			{
				continue;
			}
			float score = attackable ? d : 100f + d; // enemies in range win over doors / NPCs
			if (score < bestScore)
			{
				bestScore = score;
				best = u;
			}
		}
		mMoveIssued = false;
		if (best != null)
		{
			FApp.World.PCSilentMove();
			FUnitAIBase.InputResult result = player.AI.HandleUnitTap(best, true);
			bool handled = result == FUnitAIBase.InputResult.Handled;
			if (result == FUnitAIBase.InputResult.NotHandled)
			{
				player.AI.StopActions();
				handled = player.AI.DoWork(best);
			}
			if (handled)
			{
				FApp.World.FlashUnit(best);
				return;
			}
		}
		if (player.Weapon != null && player.Weapon.CanTargetGround && player.CanAttackWithWeapon())
		{
			Vector2Int f = FacingVector(player.Facing);
			FPoint2 target = new FPoint2(player.TilePosition.x + f.x * 4, player.TilePosition.y + f.y * 4);
			player.AI.StopActions();
			player.AI.DoWork(target);
		}
	}

	private static Vector2Int FacingVector(FAnimEntry.AnimFacing facing)
	{
		switch (facing)
		{
		case FAnimEntry.AnimFacing.UP:
			return new Vector2Int(0, 1);
		case FAnimEntry.AnimFacing.LEFT:
			return new Vector2Int(-1, 0);
		case FAnimEntry.AnimFacing.RIGHT:
			return new Vector2Int(1, 0);
		default:
			return new Vector2Int(0, -1);
		}
	}

	private void DoMovement(FUnit player, Vector2Int dir)
	{
		FRoom room = FApp.World.CurrentRoom;
		if (room == null)
		{
			return;
		}
		if (dir == Vector2Int.zero)
		{
			if (mMoveIssued)
			{
				// released: settle on the tile the unit is in instead of coasting to the far target
				mMoveIssued = false;
				FApp.World.PCSilentMove();
				player.AI.StopActions();
				player.AI.DoMoveToPoint(player.TilePosition, 0, false, true, true, string.Empty);
			}
			return;
		}
		if (mMoveIssued && mLastIssuedDir == dir && player.AI.GetHasMoveAction())
		{
			return;
		}
		FPoint2 pos = player.TilePosition;
		FPoint2 target = pos;
		List<int> ignore = new List<int>();
		for (int k = 1; k <= MoveLookahead; k++)
		{
			FPoint2 n = new FPoint2(pos.x + dir.x * k, pos.y + dir.y * k);
			FTile tile = room.GetTile(n);
			if (tile == null || !tile.GetPassable(player, ignore))
			{
				break;
			}
			target = n;
		}
		if (target == pos)
		{
			if (PCDebugOverlay.LogMoves)
			{
				FPoint2 n1 = new FPoint2(pos.x + dir.x, pos.y + dir.y);
				FTile t1 = room.GetTile(n1);
				string info = "PCMove blocked at " + pos.x + "," + pos.y + " dir " + dir + " tile " + (t1 == null ? "null" : "terrainPassable=" + t1.TerrainPassable);
				if (t1 != null)
				{
					for (int u = 0; u < t1.Units.Count; u++)
					{
						info += " [" + t1.Units[u].ProtoUnit.Name + " obstructs=" + t1.Units[u].Obstructs + "]";
					}
				}
				Debug.Log(info);
			}
			return;
		}
		FApp.World.PCSilentMove();
		player.AI.StopActions();
		mMoveIssued = player.AI.DoMoveToPoint(target, 0, false, true, true, string.Empty);
		mLastIssuedDir = dir;
		if (PCDebugOverlay.LogMoves)
		{
			Debug.Log("PCMove issue from " + pos.x + "," + pos.y + " to " + target.x + "," + target.y + " ok=" + mMoveIssued + " input=" + FApp.World.InputEnabled);
		}
	}

	private void UpdateMenu(FHUD hud, FUIPage top, Vector2Int dir, bool confirm)
	{
		bool escape = Input.GetKeyDown(KeyCode.Escape);
		bool pauseOpen = hud.PagePause != null && hud.PagePause.OnStage && hud.PageLegendWorld != null;
		bool cancel = AnyKeyDown(KeyCode.Backspace, KeyCode.X, KeyCode.P) || Pad(1) || Pad(7) || (escape && !pauseOpen);
		if (cancel)
		{
			mFocus = null;
			if (pauseOpen)
			{
				hud.PageLegendWorld.HidePauseMenu();
				return;
			}
			if (top.HandleBackKey())
			{
				return;
			}
			CollectColliders(top);
			FUICollider close = FindCancelCollider();
			if (close != null)
			{
				Tap(close);
			}
			return;
		}
		if (mFocusPage != top)
		{
			mFocusPage = top;
			mFocus = null;
		}
		bool nav = false;
		Vector2Int navDir = Vector2Int.zero;
		if (dir != Vector2Int.zero)
		{
			if (dir != mNavDir)
			{
				nav = true;
				mNextNavTime = Time.unscaledTime + NavRepeatDelay;
			}
			else if (Time.unscaledTime >= mNextNavTime)
			{
				nav = true;
				mNextNavTime = Time.unscaledTime + NavRepeatRate;
			}
			navDir = dir;
		}
		mNavDir = dir;
		bool wantFocus = nav || confirm;
		if (!wantFocus)
		{
			return;
		}
		CollectColliders(top);
		if (mColliders.Count == 0)
		{
			return;
		}
		if (mFocus == null || !mColliders.Contains(mFocus))
		{
			mFocus = mColliders.Contains(top.PCDefaultFocus) ? top.PCDefaultFocus : PickInitialFocus();
			mFocusVisible = true;
			if (nav || mFocus == null)
			{
				return;
			}
		}
		if (nav)
		{
			FUICollider next = PickNeighbor(mFocus, navDir);
			if (next != null)
			{
				mFocus = next;
			}
			mFocusVisible = true;
		}
		else if (confirm && mFocus != null)
		{
			mFocusVisible = true;
			Tap(mFocus);
		}
	}

	private void CollectColliders(FNode page)
	{
		mColliders.Clear();
		mStack.Clear();
		mStack.Add(page);
		float maxArea = Futile.Screen.Width * Futile.Screen.Height * 0.45f;
		while (mStack.Count > 0)
		{
			FNode node = mStack[mStack.Count - 1];
			mStack.RemoveAt(mStack.Count - 1);
			if (node == null || !node.Visible)
			{
				continue;
			}
			FUICollider c = node as FUICollider;
			if (c != null && c.Enabled && !c.PassThrough)
			{
				Rect r = c.ColliderRect;
				if (r.width > 1f && r.height > 1f && r.width * r.height < maxArea && c.ConcatenatedAlpha > 0.05f)
				{
					mColliders.Add(c);
				}
			}
			int count = node.GetChildCount();
			for (int i = 0; i < count; i++)
			{
				mStack.Add(node.GetChildAt(i));
			}
		}
	}

	private FUICollider FindCancelCollider()
	{
		string[] words = { "close", "cancel", "back", "exit", "done" };
		for (int i = 0; i < mColliders.Count; i++)
		{
			FNode n = mColliders[i];
			for (int depth = 0; n != null && depth < 3; depth++, n = n.Parent)
			{
				string name = n.Name;
				if (string.IsNullOrEmpty(name))
				{
					continue;
				}
				name = name.ToLowerInvariant();
				for (int w = 0; w < words.Length; w++)
				{
					if (name.Contains(words[w]))
					{
						return mColliders[i];
					}
				}
			}
		}
		return null;
	}

	private static Vector2 CenterOf(FUICollider c)
	{
		Rect r = c.ColliderRect;
		return c.LocalToGlobal(r.center);
	}

	private static bool IsDestructive(FUICollider c)
	{
		FNode n = c;
		for (int depth = 0; n != null && depth < 3; depth++, n = n.Parent)
		{
			string name = n.Name;
			if (!string.IsNullOrEmpty(name))
			{
				name = name.ToLowerInvariant();
				if (name.Contains("quit") || name.Contains("delete") || name.Contains("erase") || name.Contains("reset"))
				{
					return true;
				}
			}
		}
		return false;
	}

	private FUICollider PickInitialFocus()
	{
		FUICollider best = null;
		float bestScore = float.MaxValue;
		for (int i = 0; i < mColliders.Count; i++)
		{
			Vector2 p = CenterOf(mColliders[i]);
			// top-left first, never a destructive button (Quit Game, Delete, ...) while others exist
			float score = -p.y * 2f + p.x + (IsDestructive(mColliders[i]) ? 100000f : 0f);
			if (score < bestScore)
			{
				bestScore = score;
				best = mColliders[i];
			}
		}
		return best;
	}

	private FUICollider PickNeighbor(FUICollider from, Vector2Int dir)
	{
		Vector2 origin = CenterOf(from);
		FUICollider best = null;
		float bestScore = float.MaxValue;
		for (int i = 0; i < mColliders.Count; i++)
		{
			FUICollider c = mColliders[i];
			if (c == from)
			{
				continue;
			}
			Vector2 d = CenterOf(c) - origin;
			float along = d.x * dir.x + d.y * dir.y;
			float across = Mathf.Abs(d.x * dir.y) + Mathf.Abs(d.y * dir.x);
			if (along <= 1f || across > along * 3f)
			{
				continue;
			}
			float score = along + across * 2f;
			if (score < bestScore)
			{
				bestScore = score;
				best = c;
			}
		}
		return best;
	}

	private void Tap(FUICollider c)
	{
		Vector2 global = CenterOf(c);
		FTouch touch = default(FTouch);
		touch.FingerId = 0;
		touch.ButtonID = 0;
		touch.Position = global;
		touch.RawPosition = Futile.GlobalPosToScreen(global);
		touch.TapCount = 1;
		touch.Phase = TouchPhase.Began;
		Futile.TouchManager.ProcessSingleTouch(touch);
		mPendingRelease = touch;
		mReleaseTime = Time.unscaledTime + 0.08f;
	}

	private void OnGUI()
	{
		if (!mFocusVisible || mFocus == null || Event.current.type != EventType.Repaint || FApp.HUD == null || FApp.HUD.TopPage != mFocusPage)
		{
			return;
		}
		Rect r = mFocus.ColliderRect;
		Vector2 a = Futile.GlobalPosToScreen(mFocus.LocalToGlobal(new Vector2(r.xMin, r.yMin)));
		Vector2 b = Futile.GlobalPosToScreen(mFocus.LocalToGlobal(new Vector2(r.xMax, r.yMax)));
		Vector2 off = PCDisplay.Letterboxed ? (Vector2)PCDisplay.Offset : Vector2.zero;
		float x0 = Mathf.Min(a.x, b.x) + off.x;
		float x1 = Mathf.Max(a.x, b.x) + off.x;
		float y0 = Screen.height - (Mathf.Max(a.y, b.y) + off.y);
		float y1 = Screen.height - (Mathf.Min(a.y, b.y) + off.y);
		if (mWhite == null)
		{
			mWhite = Texture2D.whiteTexture;
		}
		GUI.depth = -100;
		float t = Mathf.Max(2f, Screen.height / 240f);
		Color pulse = Color.Lerp(new Color(1f, 0.85f, 0.2f, 1f), Color.white, 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * 8f));
		GUI.color = pulse;
		GUI.DrawTexture(new Rect(x0 - t, y0 - t, x1 - x0 + 2f * t, t), mWhite);
		GUI.DrawTexture(new Rect(x0 - t, y1, x1 - x0 + 2f * t, t), mWhite);
		GUI.DrawTexture(new Rect(x0 - t, y0, t, y1 - y0), mWhite);
		GUI.DrawTexture(new Rect(x1, y0, t, y1 - y0), mWhite);
		GUI.color = Color.white;
	}
}
