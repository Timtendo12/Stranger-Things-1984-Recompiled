# Stranger Things: 1984 — Recompiled

A native PC port (Windows, macOS, Linux) of *Stranger Things: 1984* (BonusXP / Netflix, Android
version 1.0.605), plus a web (WebGL) build that runs in the browser. The game logic was
re-implemented in C# from the shipped Android build and runs as a regular Unity project, with
keyboard, mouse and gamepad controls and a resizable or fullscreen window.

**This repository contains no game assets.** You need your own copy of the Android game (an
APK). The setup script turns it into the art, audio, levels and data the port needs, on your own
computer. The application icon is downloaded at build time.

## Status

The game is broadly playable on Windows and macOS. That covers menus, story, ~66 worlds, shops,
bag/map/pause, saves, music and sound, the playable characters, and bosses such as the Evil
Scientist. Known gaps:

- **Unfinished content.** The Library stealth boss and the exit flow back to the world map are
  not finished.
- **Visual effects.** A few 3D/visual-effect shaders are placeholders, so some effects look
  plainer than on Android.
- **Untested gamepads.** Gamepad support is written for the Xbox/XInput layout and is untested on
  real hardware. Button mapping on macOS and Linux may be off; see [docs/CONTROLS.md](docs/CONTROLS.md).
- **Discord Rich Presence** is optional. It works over Discord's local IPC: a named pipe on
  Windows, a Unix socket on macOS and Linux (including Flatpak and Snap installs). Only the
  Windows path has been tried so far.
- **Single instance.** Only one copy of the game can run at a time.

See [docs/PORT_NOTES.md](docs/PORT_NOTES.md) for intentional differences from the Android version and
[docs/coverage.md](docs/coverage.md) for how much of the original code has been re-implemented.

| Platform | Build | Tested |
|---|---|---|
| Windows 10/11 x64 | ✅ | ✅ plays |
| macOS (universal: Intel + Apple silicon) | ✅ | ✅ plays |
| Linux x64 | ✅ (cross-built) | not yet |
| Web (WebGL) | ✅ | early: menus and input work, see [Web](#web) |

## Requirements

- **Your own APK of Stranger Things: 1984, version 1.0.605** (versionCode 261010701). See
  [Getting the APK](#getting-the-apk).
- **Unity 6000.2.6f2**, installed with [Unity Hub](https://unity.com/download). A free Unity
  Personal license is enough.
  - To build for your own OS you need nothing extra. On macOS, Mac (Mono) support comes with the
    editor. On Windows and Linux, Unity Hub installs your OS's build support with the editor.
  - Only to cross-build for another OS, add that OS's module in Unity Hub: *Windows Build
    Support (Mono)*, *Mac Build Support (Mono)* or *Linux Build Support (Mono)*. IL2CPP modules
    and Xcode are not needed.
  - Only for the web build, add *Web Build Support*.
- **Python 3.10 or newer** (only the standard library is used).
- About **3 GB** of free disk space, and an internet connection for the first setup and build.
  Setup downloads AssetRipper once, Unity downloads one package, and the first build downloads
  the application icon.

## Quick start

Install the [requirements](#requirements), clone the repository, then run the build script for
your OS (or the web) with the path to your APK:

```sh
git clone https://github.com/Timtendo12/Stranger-Things-1984-Recompiled.git
cd Stranger-Things-1984-Recompiled
```

| OS | Command | Then start the game |
|---|---|---|
| Windows | `build_windows.bat path\to\StrangerThings.apkm` | `dist\windows\StrangerThings1984.exe` |
| macOS | `./build_macos.sh path/to/StrangerThings.apkm` | `open dist/macos/StrangerThings1984.app` |
| Linux | `./build_linux.sh path/to/StrangerThings.apkm` | `./dist/linux/StrangerThings1984.x86_64` |
| Web (from macOS or Linux) | `./build_webgl.sh path/to/StrangerThings.apkm` | `python3 tools/run.py --target web` |

- **What the script does.** It prepares the game assets from your APK (about a minute), then
  builds the game. The first build imports ~13,000 assets, so allow 2–30 minutes.
- **No APK path?** The script asks for one.
- **Rebuilding.** Run the script again without the APK, e.g. `./build_macos.sh`. Give the APK
  again only if you want to redo the asset setup.
- **Extra options** are passed on to the build: `--dev` for a development build, `--clean` or
  `--full-clean` for a clean rebuild. On macOS the app is universal (Intel + Apple silicon) by
  default; `--silicon` builds for Apple silicon only and `--intel` for Intel only (runs under
  Rosetta on Apple silicon).
- **Web build on Windows.** There is no `.bat` wrapper for it yet; run
  `python tools\build.py --target web` after setup.

The scripts are thin wrappers around `tools/setup.py` and `tools/build.py`, described below. Use
those directly to cross-build for another OS, or for finer control.

## Getting the APK

The port is built against one exact version of the game: **1.0.605 (versionCode 261010701)**, package
`com.netflix.NGP.StrangerThings`. Get it from a device or account that owns the game. For example,
copy the installed APKs off your Android device with `adb`:

```sh
adb shell pm path com.netflix.NGP.StrangerThings     # lists base.apk and split_config.*.apk
adb pull /data/app/.../base.apk
adb pull /data/app/.../split_config.arm64_v8a.apk
```

Setup accepts any of these:

| What you have | Command |
|---|---|
| An APK bundle (`.apkm`, `.xapk`, `.apks`) | `python tools/setup.py game.apkm` |
| `base.apk` plus the arm64 split | `python tools/setup.py base.apk --split split_config.arm64_v8a.apk` |
| A single "fat" APK that contains `lib/arm64-v8a` | `python tools/setup.py game.apk` |
| An already-extracted APK folder | `python tools/setup.py path/to/folder` |

Two parts of the APK are needed: the Unity data in `base.apk` (`assets/bin/Data`) and the 64-bit
ARM code library (`lib/arm64-v8a/libil2cpp.so`, usually in `split_config.arm64_v8a.apk`).

This project does not link to or provide APKs. Do not share APKs or generated assets.

## What setup does

`tools/setup.py` treats the APK as untrusted input:

1. **Validates the input.** It must be a zip-based APK or bundle. Archive paths are checked so
   that nothing can be written outside the target folder (no `..`, absolute paths or symlinks),
   and size and compression limits guard against archive bombs. Encrypted bundles are rejected.
2. **Extracts only what it needs.** That means `assets/bin/Data/**` and `libil2cpp.so`, written to
   `.cache/staging/`.
3. **Checks the game version.** It compares the SHA-256 of `global-metadata.dat` and
   `libil2cpp.so` with [tools/supported.json](tools/supported.json). Other versions are refused
   unless you pass `--allow-unsupported`, and even then they will most likely not work.
4. **Downloads AssetRipper.** It fetches [AssetRipper](https://github.com/AssetRipper/AssetRipper)
   2.0.0 for your OS from GitHub into `.cache/tools/` and verifies its SHA-256. Use
   `--assetripper <path>` to use a copy you already have.
5. **Exports a Unity project.** It runs AssetRipper headless, on a local port only, to export the
   game's assets as a Unity project into `.cache/export/`.
6. **Copies the assets into `UnityProject/`.** It never overwrites files that belong to this
   repository, and it skips the original game's scripts, which the port replaces.
7. **Post-processes the assets.**
   - Retargets the public-key token of five stub DLLs so Unity accepts them.
   - Gives the two assets that the project settings reference (`Default.unity` and the default
     physics material) their expected GUIDs.
8. **Verifies the result.** It checks that every required asset folder and file is present and
   reports what is missing.

Every generated file is gitignored. Re-running setup replaces the previously generated files.
`python tools/setup.py --clean` removes all of them.

## Building

```sh
python tools/build.py                       # this OS, release build
python tools/build.py --target windows      # or macos / linux / web / all
python tools/build.py --dev                 # development build (debugger, Unity dev console)
python tools/build.py --clean               # delete previous output for the target first
python tools/build.py --full-clean          # also delete UnityProject/Library (full re-import)
python tools/build.py --target macos --silicon  # macOS: Apple silicon only (default: universal)
python tools/build.py --target macos --intel    # macOS: Intel only (Rosetta on Apple silicon)
```

| Target | Output |
|---|---|
| windows | `dist/windows/StrangerThings1984.exe` (plus the data folder next to it) |
| macos | `dist/macos/StrangerThings1984.app` |
| linux | `dist/linux/StrangerThings1984.x86_64` (plus the data folder next to it) |
| web | `dist/web/index.html` (plus `Build/`, `StreamingAssets/` and `favicon.png`) |

- The Unity build log is written to `dist/logs/build-<target>.log`.
- Desktop builds use the Mono scripting backend, so you can cross-build from one OS for the
  others as long as the matching *Build Support (Mono)* module is installed. The web build uses
  IL2CPP (WebGL has no Mono) and takes noticeably longer.
- Each build first makes sure the application icon is in place. It is downloaded once from
  imgur into `.cache/GameIcon.png` and used for the exe/app/window icon and the web favicon. If
  the download fails the build continues with Unity's default icon.
- Close the Unity editor before building from the command line. Unity cannot open the same
  project twice.

You can also open `UnityProject/` in the Unity editor after setup. The **Port** menu has
development builds for each platform.

### Configuration (optional)

Copy `.env.example` to `.env`; git ignores `.env`. Environment variables with the same names
override it.

| Setting | Purpose |
|---|---|
| `UNITY_EDITOR` | Path to the Unity editor executable, if it is not in Unity Hub's default location |
| `DISCORD_CLIENT_ID` | Numeric Discord application id to enable Rich Presence. Empty means off |
| `DISCORD_LARGE_IMAGE_KEY` | Rich Presence image key (optional) |
| `DISCORD_ENABLED` | `false` builds without presence |

These settings are only read at build time. The game itself reads no configuration files. Its
settings, such as language, window size and sound, are stored in Unity's PlayerPrefs, and saves
go to Unity's per-user data folder:

- **Windows:** `%USERPROFILE%\AppData\LocalLow\Gear Games, Inc\StrangerThings1984`
- **macOS:** `~/Library/Application Support/Gear Games, Inc/StrangerThings1984`
- **Linux:** `~/.config/unity3d/Gear Games, Inc/StrangerThings1984`
- **Web:** the browser's storage for the page's address (IndexedDB), so saves are per browser
  and per host/port

## Running

```sh
python tools/run.py                 # windowed 1280x720
python tools/run.py --fullscreen
python tools/run.py --seconds 60    # run for 60 s, then print errors from the player log
```

You can also start the built executable directly. Controls are listed in
[docs/CONTROLS.md](docs/CONTROLS.md). F11 or Alt+Enter toggles fullscreen.

Launch with `-debug` for developer mode (collider overlay and more), e.g.
`python tools/run.py -- -debug`. See [docs/PORT_NOTES.md](docs/PORT_NOTES.md#debug-mode).

### macOS

Building on the Mac itself is the simplest option. Xcode is not needed, because the port uses
the Mono backend.

The app is not signed or notarized. If macOS says it is damaged or cannot be opened, run this
once:

```sh
xattr -dr com.apple.quarantine dist/macos/StrangerThings1984.app
open dist/macos/StrangerThings1984.app
```

If you cross-built the app on Windows or Linux and copied it to a Mac, also restore the
executable flag and add an ad-hoc signature. Apple silicon refuses to run unsigned code:

```sh
chmod +x StrangerThings1984.app/Contents/MacOS/StrangerThings1984
xattr -dr com.apple.quarantine StrangerThings1984.app
codesign --force --deep --sign - StrangerThings1984.app
```

### Linux

```sh
chmod +x dist/linux/StrangerThings1984.x86_64
./dist/linux/StrangerThings1984.x86_64
```

The game needs working OpenGL 3.2+ or Vulkan drivers. If it exits with "No supported renderers
found", install your distribution's Mesa/GL packages, e.g. `sudo apt install libgl1 libvulkan1
mesa-vulkan-drivers` on Debian/Ubuntu.

### Web

```sh
python tools/run.py --target web              # serves dist/web on http://localhost:8000 and opens it
python tools/run.py --target web --port 9000  # another port
```

Opening `index.html` straight from disk does not work; browsers block loading the game data from
`file://`. Any static web server can host `dist/web`. The data is gzip-compressed with a
decompression fallback, so no special `Content-Encoding` server setup is needed.

Differences from the desktop builds:

- **No Chinese.** The CJK fonts (~66 MB) are left out, so the zh-cn / zh-tw languages are hidden.
- **No Discord Rich Presence.**
- **The video** (`ExclusiveVideo.mp4`) is streamed from `StreamingAssets/` instead of being
  packed in the build.
- **Developer mode** is enabled with `index.html?debug` instead of a launch flag.

## Clean rebuild from scratch

```sh
python tools/setup.py --clean                      # remove generated assets and caches
python tools/setup.py path/to/your.apkm
python tools/build.py --full-clean
```

## Troubleshooting

| Message | What to do |
|---|---|
| `The 64-bit ARM library lib/arm64-v8a/libil2cpp.so is missing` | You supplied only `base.apk`. Add `--split split_config.arm64_v8a.apk`, or use the whole bundle |
| `This APK is not a supported version` | The port only works with 1.0.605. Get that exact version |
| `is not a valid zip/APK archive` | The file is truncated or not an APK. Download or copy it again |
| `Archive member ... is encrypted` | Some older `.apkm` files are encrypted. Copy the APKs from a device instead |
| `Unity 6000.2.6f2 was not found` | Install that exact version with Unity Hub, or set `UNITY_EDITOR` in `.env` |
| `has no ... Build Support (Mono) module` | Unity Hub → Installs → 6000.2.6f2 → ⚙ → Add modules |
| `The project is open in another Unity editor` | Close the editor, then build again |
| `The game assets are not set up yet` | Run `tools/setup.py` first |
| Black screen on start | Another copy of the game is already running; close it |
| AssetRipper download fails | Download the archive named in `tools/supported.json` yourself and pass `--assetripper <folder>` |
| `Download of https://i.imgur.com/... failed` | The build continues with Unity's default icon. To use the real one, save that PNG as `.cache/GameIcon.png` and build again |
| `has no Web Build Support module` | Unity Hub → Installs → 6000.2.6f2 → ⚙ → Add modules → *Web Build Support* |
| Web page stays blank or fails to load | Serve it over HTTP (`python tools/run.py --target web`); don't open `index.html` from disk |

Build errors are summarized on screen. The full log is in `dist/logs/`. The setup log is
`.cache/assetripper.log`.

## Known limitations

- **One game version.** Only Android version 1.0.605 is supported.
- **Testing coverage.** Windows and macOS are tested end to end. Linux and web builds are
  produced by the same pipeline, but get less testing.
- **Web build.** No Chinese languages and no Discord presence; see [Web](#web).
- **Gamepads.** Mapping on macOS and Linux may be incomplete (legacy Unity input manager).
- **Discord Rich Presence on macOS and Linux** is implemented but untested.
- **Placeholder shaders.** Some original shaders are compiled-only in the APK. AssetRipper exports
  them as placeholders, and the port provides hand-written replacements for the ones the game
  visibly needs.

## Repository layout

```
build_*.sh/.bat   one-step setup + build per OS, and build_webgl.sh for the web (wrappers around tools/)
tools/            setup.py, build.py, run.py, fastcheck.py (dev compile check), supported.json
UnityProject/     Unity project: ported C# (Assets/Scripts, Assets/Plugins/Assembly-CSharp-firstpass),
                  editor build script, replacement shaders, web page template
                  (Assets/WebGLTemplates) and stripping rules (Assets/link.xml), project settings,
                  packages.
                  Everything else under Assets/ is generated by setup (or, for the application
                  icon, downloaded by build.py) and gitignored.
docs/             controls, port notes, coverage, supported versions
```

## Legal

- The port's own code, tools and docs are licensed under the [GNU GPL v3](LICENSE).
- Third-party components keep their own licenses; see [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).
- *Stranger Things* is a trademark of Netflix, Inc. *Stranger Things: 1984* was developed by
  BonusXP.
- This is an unofficial fan project. It is not affiliated with, endorsed by or sponsored by
  Netflix or BonusXP.
- No game assets, APKs or other proprietary files are included or distributed. The application
  icon (also the web favicon) is downloaded by `tools/build.py` at build time. You must own the
  game to use this port.
- Do not commit or share the files generated by `tools/setup.py`.
