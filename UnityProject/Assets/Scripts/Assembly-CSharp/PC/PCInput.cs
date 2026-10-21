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

	private bool mBossBeating;

	private static bool sSingleHit;

	// debug (-debug, F6): hit the boss of the current room every 0.7 s with the player's weapon until it is gone.
	private System.Collections.IEnumerator BeatBoss()
	{
		mBossBeating = true;
		int maxHits = sSingleHit ? 1 : 120;
		for (int i = 0; i < maxHits; i++)
		{
			FUnit boss = null;
			FRoom room = FApp.World.CurrentRoom;
			if (room != null)
			{
				for (int u = 0; u < room.Units.Count; u++)
				{
					if (room.Units[u] != null && room.Units[u].ProtoUnit.IsTag("Boss") && room.Units[u].AliveAndWell)
					{
						boss = room.Units[u];
					}
				}
			}
			if (boss == null || FWorld.Player == null)
			{
				break;
			}
			boss.OnAttacked(FWorld.Player, null, true, null);
			Debug.Log("PCBoss hit hp=" + boss.HP);
			yield return new WaitForSeconds(0.7f);
		}
		mBossBeating = false;
	}

	private float mNextAudioLog;

	// Diagnostic: launch with -debug to print the playing AudioSources every 4 s into the player log.
	private void LogAudio()
	{
		if (Time.unscaledTime < mNextAudioLog)
		{
			return;
		}
		mNextAudioLog = Time.unscaledTime + 4f;
		System.Text.StringBuilder sb = new System.Text.StringBuilder("PCAudio playing:");
		AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		for (int i = 0; i < sources.Length; i++)
		{
			if (sources[i].isPlaying && sources[i].clip != null && sources[i].clip.length > 15f)
			{
				sb.Append(' ').Append(sources[i].clip.name).Append('(').Append(sources[i].volume.ToString("0.00")).Append(sources[i].mute ? ",MUTED" : "").Append(')');
			}
		}
		Debug.Log(sb.ToString());
	}

	private void Update()
	{
		if (PCDebug.Enabled)
		{
			LogAudio();
		}
		FHUD hud = FApp.HUD;
		if (hud == null || Futile.TouchManager == null)
		{
			return;
		}
		if (PCDebug.Enabled && Input.GetKeyDown(KeyCode.F8) && FWorld.Player != null)
		{
			FWorld.Player.SetHP(0); // debug: test the death / retry flow
		}
		if (PCDebug.Enabled && Input.GetKeyDown(KeyCode.Home) && FWorld.Player != null)
		{
			// debug: cast the guard-cone ray from the player in the four directions and report the first non-passable tile hit
			Vector3 pp = FWorld.Player.Position;
			Vector2[] dirs = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
			RaycastHit2D[] hits = new RaycastHit2D[20];
			for (int d = 0; d < 4; d++)
			{
				int n = Physics2D.RaycastNonAlloc(new Vector2(pp.x, pp.y), dirs[d], hits, 6f);
				string info = "PCRay " + dirs[d] + " from " + pp.x.ToString("0.0") + "," + pp.y.ToString("0.0") + " n=" + n;
				for (int h = 0; h < n; h++)
				{
					FTile t = FApp.World.GetTileFromGO(hits[h].collider.gameObject);
					FUnit u = FApp.World.GetUnitFromGO(hits[h].collider.gameObject);
					info += " [" + (t != null ? "tile " + t.Position.x + "," + t.Position.y + " pass=" + t.TerrainPassable : (u != null ? "unit " + u.ProtoUnit.Name : "other " + hits[h].collider.name)) + " d=" + hits[h].distance.ToString("0.00") + (hits[h].collider.isTrigger ? " trig" : "") + "]";
				}
				Debug.Log(info);
			}
		}
		if (PCDebug.Enabled && Input.GetKeyDown(KeyCode.F10) && FWorld.Player != null)
		{
			// debug: teleport the player into the Library boss room (tile 24,3)
			FWorld.Player.AI.StopActions();
			FWorld.Player.GO.transform.position = new Vector3(24f, 3f, FWorld.Player.GO.transform.position.z);
			FWorld.Player.SetTilePosition(new FPoint2(24, 3), true);
			Debug.Log("PCTeleport room=" + FApp.World.CurrentRoom.ProtoRoom.Name);
		}
		if (PCDebug.Enabled && Input.GetKeyDown(KeyCode.F12) && FApp.PlayerData != null)
		{
			// debug: exercise FApp.UpdateVideoMode (resolution keep / revert dialog)
			FApp.PlayerData.SetResolution(1000, 800, 60, false, true);
		}
		if (PCDebug.Enabled && Input.GetKeyDown(KeyCode.F1) && FApp.World != null)
		{
			// debug: classify every tile of every room (+1 margin): . passable, X blocked by a unit, # wall (collider), ~ no tile
			for (int r = 0; r < FApp.World.Rooms.Count; r++)
			{
				FRoom room = FApp.World.Rooms[r];
				Rect b = room.ProtoRoom.Bounds;
				Debug.Log("PCRoom " + room.ProtoRoom.Name + " bounds " + b + " encounter=" + room.ProtoRoom.EnemyEncounterRoom);
				for (int ty = (int)(b.y + b.height); ty >= (int)b.y - 1; ty--)
				{
					string row = "PCRow y=" + ty.ToString("00") + " ";
					for (int tx = (int)b.x - 1; tx <= (int)(b.x + b.width); tx++)
					{
						FTile t = FApp.World.GetTile(new FPoint2(tx, ty));
						row += t == null ? "~" : (t.TerrainPassable ? "." : "#");
					}
					Debug.Log(row);
				}
			}
		}
		if (PCDebug.Enabled && Input.GetKeyDown(KeyCode.F2) && FWorld.Player != null)
		{
			// debug: knock the player one tile to the left like a thrown vial would
			FWorld.Player.OnAttacked(FWorld.Player, 1, Vector2.left, 1, true);
		}
		if (PCDebug.Enabled && Input.GetKeyDown(KeyCode.F3) && FWorld.Player != null)
		{
			// debug: single step left into whatever is there (e.g. a laser beam)
			FPoint2 tp = FWorld.Player.TilePosition;
			FWorld.Player.AI.StopActions();
			FWorld.Player.AI.DoMoveToPoint(new FPoint2(tp.x - 1, tp.y), 0, false, true, true, string.Empty);
		}
		if (PCDebug.Enabled && Input.GetKeyDown(KeyCode.F4) && FWorld.Player != null)
		{
			FWorld.Player.ProtoUnit.SetDamageTakenModifier(0f); // debug: invulnerable
		}
		if (PCDebug.Enabled && Input.GetKeyDown(KeyCode.F5) && !mBossBeating)
		{
			sSingleHit = true; // debug: one boss hit per F5
			StartCoroutine(BeatBoss());
		}
		if (PCDebug.Enabled && Input.GetKeyDown(KeyCode.F6) && FWorld.Player != null && FApp.World.CurrentRoom != null && !mBossBeating)
		{
			sSingleHit = false;
			StartCoroutine(BeatBoss());
		}
		if (PCDebug.Enabled && Input.GetKeyDown(KeyCode.F7) && FWorld.Player != null && FApp.World.CurrentRoom != null)
		{
			// debug: dump the 9 x 11 tiles around the player
			FPoint2 pp = FWorld.Player.TilePosition;
			for (int ty = pp.y + 6; ty >= pp.y - 3; ty--)
			{
				string row = "PCDump y=" + ty + ":";
				for (int tx = pp.x - 6; tx <= pp.x + 6; tx++)
				{
					FTile t = FApp.World.CurrentRoom.GetTile(new FPoint2(tx, ty));
					if (t == null)
					{
						row += " ##";
						continue;
					}
					string c = t.TerrainPassable ? (t.GetPassable(FWorld.Player, new List<int>()) ? " . " : " X ") : " # ";
					row += c;
				}
				Debug.Log(row);
			}
			FTile ft = FApp.World.CurrentRoom.GetTile(pp);
			for (int ty = pp.y + 6; ty >= pp.y - 3; ty--)
			{
				for (int tx = pp.x - 6; tx <= pp.x + 6; tx++)
				{
					FTile t = FApp.World.CurrentRoom.GetTile(new FPoint2(tx, ty));
					if (t == null)
					{
						continue;
					}
					for (int u = 0; u < t.Units.Count; u++)
					{
						Debug.Log("PCDump unit " + t.Units[u].ProtoUnit.Name + " @" + tx + "," + ty + " obstructs=" + t.Units[u].Obstructs);
					}
				}
			}
		}
		if (PCDebug.Enabled && Input.GetKeyDown(KeyCode.F9) && FApp.PlayerStats != null)
		{
			// debug (-debug): give coins and pudding for shop / ability tests
			FApp.PlayerStats.SetCoins(100);
			FApp.PlayerStats.SetPudding(5);
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
			if (PCDebug.Enabled)
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
		if (PCDebug.Enabled)
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
