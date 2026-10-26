# Port notes: intentional deviations and bugfixes

Differences between this PC port and the shipped Android build (1.0.605). Add an entry whenever the port behaves differently on purpose.

## Guard flashlight cones now stop at all walls
- **Observed:** in the Library, a guard's flashlight cone shrank only when facing the bookshelf units on the top wall, not the left, right or bottom walls. The user confirmed the same quirk exists in the original Android game, so it is a bug in the shipped game too.
- **Cause in the port:** `FLightBeam` clips the cone at non-passable tile colliders and at obstructing units. `FTile.SyncBoxCollider` named collider objects `"(x, y)"` while `GetTileFromGO` expects `"x, y"` (the shipped format string is `"{0}, {1}"`). Tile lookups failed, so only shelf units clipped the cone.
- **Fix:** one line in `FTile.SyncBoxCollider`: the name is now `"{0}, {1}"`. Cones stop at walls in every direction. This also fixes other paths that map a collider to a tile through `GetTileFromGO`.
- **Deviation:** the original's cones did not stop at plain walls. If exact original behaviour is wanted, restore the old name format (re-breaks every `GetTileFromGO` caller, so prefer a targeted change in `FLightBeam` instead).

## Player hit retrigger
- `FUnit.OnAttacked` now does nothing (no push, no Hit animation, no sound) when the player's 0.2 s invulnerability window blocks the damage. The decompile pushes unconditionally. Without this, standing in a laser beam retriggered the Hit sound every tile entry and made the sprite vibrate. Marked in the code.

## FConfig.Music
- The stub getter returned false, so no music played. It is now a backing field defaulting to true.

## PC-only additions
- `PCInput.cs` (keyboard/mouse/gamepad, menu focus ring) and `PCDisplay.cs` (F11 / Alt+Enter fullscreen, 16:9 letterbox). See [CONTROLS.md](CONTROLS.md).
- Developer mode with the `-debug` (or `--debug`) launch flag, e.g. `python tools/run.py -- -debug`. See [Debug mode](#debug-mode).
- `UserCommandsDelayed.txt` runs `<seconds>|<command>` lines after a world loads (test aid).

## Display, input, saves, hits
- FApp.PostInit is invoked from HandleUserStateChange (the shipped code reached it through the Netflix auth callbacks, which the offline shim does not run).
- Reset Save / delete save slot persist immediately (FPlayerData.Save()); the shipped code only changed memory and relied on a later save.
- FApp.UpdateVideoMode applies the mode through PCDisplay.SetMode; Resolution.* dialog strings fall back to English because the string tables lack them.
- FUnit.OnAttacked: a hit on the player inside the 0.2 s invulnerability window is ignored completely (no knockback restart, hit animation or sound); the shipped code pushed again on every call.
- FTile collider GameObjects are named "{0}, {1}" (the previous "({0}, {1})" broke FPoint2.Parse, so tile colliders were never mapped back to tiles: guard cones ignored walls).
- Display: PCDisplay replaces Screen.width/height for the game (FScreen, FUtil, FTouch, FCamera, FWorld, FWorldTheater) and letterboxes to 16:9 through a render target; the forced-16:9 resize code in FScreen.Update was removed.
- Input: PCInput adds keyboard / gamepad control and menu focus navigation (see [CONTROLS.md](CONTROLS.md)). FPageLegendWorld.HandleInput opens the pause menu on Esc only when it is the top page.

## Debug mode

Launching with `-debug` (or `--debug`) turns on (`PCDebug.Enabled`):
- **FPS counter** in the top-right corner.
- **Debug overlay** (`PCDebugOverlay.cs`), opened and closed with **F1**. While it is open the keyboard, gamepad and mouse go only to the overlay (the game keeps running; the window has a Pause button). Tabs:

| Tab | Contents |
|---|---|
| Worlds | Search and load any world; open a world (>) to arrive at one of its entry points (once you are in a world). Library boss room button (top entrance; also the `PCTeleportBoss()` console command, e.g. in `UserCommandsDelayed.txt`). |
| Player | Heal, -1 HP, kill (death / retry flow), invulnerable, play as any character. |
| Items | Coins, pudding, key cards and rocks (0 / +1 / +10 / max); give collectibles by type (dungeon items, tools, upgrades, heart pieces, outfits, ...). |
| Characters | Unlock one or all characters (adds their unlock collectible, so it is saved); play as. |
| Combat | Hit the room's boss once / every 0.7 s until it is gone; knockback and step-left tests. |
| Tools | Collider overlay (green = unit, red = wall / solid tile, yellow = trigger), move and music logging, tile / unit / guard-ray dumps to the player log, resolution keep / revert dialog test, Discord presence update. |

- **Logging** to the player log: Discord Rich Presence errors; move and music logging when switched on in Tools.

## Recovered values
Values re-read from the disassembly changed: name generator whole-name chance (1/101), tabbed page transitions linear, shop price label colour (red/black with gray shadow), game-mode info and conflict popup separators "\n", FSoundManager default volumes, FTextSpanImage wrap string " ".

## Netflix SDK removed, local profile
No Netflix SDK code remains. A fixed local profile ("local-player", same save key as before) feeds HandleUserStateChange once at startup. CloudSaveManager is a local no-op shim; the cloud conflict popup is gone. Preferred language is now an Options > Language setting (PlayerPrefs "PC.Language", default OS language if supported else English), applied live by re-running HandleUserStateChange.

Language picker: Options > Language opens the (previously unused) FPageStringPicker; choosing a language rebuilds the HUD and returns to Options.

## Discord Rich Presence (PC-only addition)
PCDiscord/DiscordIpc add optional Discord presence over the local IPC pipe; inert unless DISCORD_CLIENT_ID is set in .env at build time (compiled into DiscordConfig.Generated.cs, gitignored). Windows uses the discord-ipc-N named pipe; macOS and Linux use the discord-ipc-N Unix socket in $XDG_RUNTIME_DIR, $TMPDIR, $TMP, $TEMP or /tmp, including the Flatpak and Snap sub-folders.
