"""Build the game with Unity in batch mode.

    python tools/build.py [--target windows|macos|linux|web|all] [--dev] [--clean | --full-clean]
                          [--silicon | --intel]

Output: dist/windows/StrangerThings1984.exe, dist/macos/StrangerThings1984.app (universal unless --silicon or --intel),
dist/linux/StrangerThings1984.x86_64, dist/web/index.html. Unity's log goes to
dist/logs/build-<target>.log. Requires tools/setup.py to have completed and Unity (version in
ProjectSettings/ProjectVersion.txt) with the build-support module for each target (Mono for the
desktop targets, Web Build Support for web). The application icon (also the web favicon) is
downloaded once into .cache/GameIcon.png.
"""
import argparse
import re
import shutil
import subprocess
import sys
import time
import urllib.error
import urllib.request
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import gen_discord_config  # noqa: E402
from _common import (CACHE, DIST, PROJECT, SETUP_STAMP, ToolError, find_unity_editor, host_os,  # noqa: E402
                     info, playback_engines_dirs, run_main, unity_version, warn)

ICON_URL = 'https://i.imgur.com/oOIN6Bh.png'
ICON_CACHE = CACHE / 'GameIcon.png'
# Editor/PortBuild.cs sets the first as the application icon; the web template links the second.
ICON_DESTS = (PROJECT / 'Assets' / 'PCIcon' / 'GameIcon.png',
              PROJECT / 'Assets' / 'WebGLTemplates' / 'StrangerThings' / 'favicon.png')
PNG_SIGNATURE = b'\x89PNG\r\n\x1a\n'

TARGETS = {
    # name: (Unity -buildTarget, PlaybackEngines module folder, Unity Hub module name, output file)
    'windows': ('Win64', 'windowsstandalonesupport', 'Windows Build Support (Mono)', 'StrangerThings1984.exe'),
    'macos': ('OSXUniversal', 'macstandalonesupport', 'Mac Build Support (Mono)', 'StrangerThings1984.app'),
    'linux': ('Linux64', 'linuxstandalonesupport', 'Linux Build Support (Mono)', 'StrangerThings1984.x86_64'),
    'web': ('WebGL', 'webglsupport', 'Web Build Support', 'index.html'),
}


def check_module(editor, target):
    dirs = [d for d in playback_engines_dirs(editor) if d.is_dir()]
    if not dirs:
        return  # unusual layout; let Unity report it
    have = {p.name.lower() for d in dirs for p in d.iterdir()}
    _, folder, module, _ = TARGETS[target]
    if target == host_os() and target == 'macos' and folder not in have:
        return  # the host platform's Mono support ships with the editor; let Unity confirm
    if folder not in have:
        raise ToolError(f'Unity {unity_version()} has no {module} module, which is needed to build for {target}.\n'
                        f'Add it in Unity Hub: Installs > {unity_version()} > (gear) > Add modules > {module}.')


def ensure_icon():
    if not ICON_CACHE.exists():
        info(f'Downloading the application icon from {ICON_URL}...')
        part = ICON_CACHE.with_suffix('.png.part')
        try:
            CACHE.mkdir(parents=True, exist_ok=True)
            # imgur answers 429 to urllib's default User-Agent
            req = urllib.request.Request(ICON_URL, headers={'User-Agent': 'StrangerThings1984-build'})
            with urllib.request.urlopen(req, timeout=30) as r:
                data = r.read()
            if not data.startswith(PNG_SIGNATURE):
                raise OSError('the response is not a PNG image')
            part.write_bytes(data)
            part.replace(ICON_CACHE)
        except (urllib.error.URLError, OSError) as e:
            warn(f'Download of {ICON_URL} failed: {e}\nBuilding with Unity\'s default icon. To use the real one, '
                 f'save that PNG as {ICON_CACHE} and build again.')
            return
    data = ICON_CACHE.read_bytes()
    for dest in ICON_DESTS:
        if not dest.exists() or dest.read_bytes() != data:
            dest.parent.mkdir(parents=True, exist_ok=True)
            dest.write_bytes(data)


def summarize_log(log):
    if not log.exists():
        return []
    text = log.read_text(encoding='utf-8', errors='replace')
    lines = []
    if 'another Unity instance is running' in text or 'Multiple Unity instances cannot open the same project' in text:
        lines.append('The project is open in another Unity editor. Close it and build again.')
    for pat in (r'.*error CS\d+.*', r'.*PortBuild: .*', r'.*Error building Player.*', r'.*Build Finished.*',
                r'.*Scripts have compiler errors.*', r'.*executeMethod class .* could not be found.*'):
        for m in re.finditer(pat, text):
            s = m.group(0).strip()
            s = re.sub(r'^.*?(Assets[\\/])', r'\1', s) if 'error CS' in s else s
            if s not in lines:
                lines.append(s)
    return lines[:60]


def build(editor, target, dev, out_root, mac_arch='universal'):
    check_module(editor, target)
    out_dir = out_root / target
    logs = out_root / 'logs'
    logs.mkdir(parents=True, exist_ok=True)
    log = logs / f'build-{target}.log'
    build_target, _, _, product = TARGETS[target]
    cmd = [str(editor), '-batchmode', '-nographics', '-projectPath', str(PROJECT),
           '-buildTarget', build_target, '-executeMethod', 'PortBuild.Build',
           '-portTarget', target, '-portOut', str(out_dir), '-logFile', str(log)]
    if dev:
        cmd.append('-portDev')
    if target == 'macos':
        cmd += ['-portMacArch', mac_arch]
    first = not (PROJECT / 'Library').exists()
    label = f'{target} ({mac_arch})' if target == 'macos' else target
    info(f'Building {label}{" (development)" if dev else ""} with Unity {unity_version()}...'
         + (' First run imports ~13,000 assets; expect 10-30 minutes.' if first else ''))
    t = time.time()
    code = subprocess.call(cmd)
    for line in summarize_log(log):
        info('  ' + line)
    result = out_dir / product
    if code != 0 or not result.exists():
        raise ToolError(f'Build for {target} failed (Unity exit code {code}). Full log: {log}')
    info(f'Built {result} in {time.time() - t:.0f}s')
    return result


def main():
    ap = argparse.ArgumentParser(description='Build the Stranger Things 1984 PC port with Unity.')
    ap.add_argument('--target', choices=list(TARGETS) + ['all'], default=host_os(),
                    help='platform to build (default: this computer\'s OS)')
    ap.add_argument('--dev', action='store_true', help='development build (debugger, on-screen dev console)')
    ap.add_argument('--clean', action='store_true', help='delete the previous output for the target first')
    ap.add_argument('--full-clean', action='store_true',
                    help='also delete UnityProject/Library (forces a full re-import; slow)')
    arch = ap.add_mutually_exclusive_group()
    arch.add_argument('--silicon', dest='mac_arch', action='store_const', const='arm64', default='universal',
                      help='macOS: Apple silicon (arm64) only, instead of a universal app')
    arch.add_argument('--intel', '--rosetta', dest='mac_arch', action='store_const', const='x64',
                      help='macOS: Intel (x86_64) only; runs under Rosetta on Apple silicon')
    ap.add_argument('--out', type=Path, default=DIST, help='output root folder (default: dist/)')
    args = ap.parse_args()

    if not SETUP_STAMP.exists() or not (PROJECT / 'Assets' / 'Scenes' / 'Default.unity').exists():
        raise ToolError('The game assets are not set up yet. Run: python tools/setup.py <your APK>  (see README).')
    editor = find_unity_editor()
    out_root = args.out.resolve()
    targets = list(TARGETS) if args.target == 'all' else [args.target]
    if args.mac_arch != 'universal' and 'macos' not in targets:
        raise ToolError('--silicon and --intel only apply to macOS builds (--target macos).')
    if args.clean or args.full_clean:
        for t in targets:
            if (out_root / t).exists():
                shutil.rmtree(out_root / t)
                info(f'Removed {out_root / t}')
    if args.full_clean:
        for d in ('Library', 'Temp', 'obj'):
            if (PROJECT / d).exists():
                shutil.rmtree(PROJECT / d)
                info(f'Removed UnityProject/{d}')
    gen_discord_config.generate()
    ensure_icon()
    built = [build(editor, t, args.dev, out_root, args.mac_arch) for t in targets]
    if len(built) > 1:
        hint = ' --target <target>'
    else:
        hint = '' if targets[0] == host_os() else f' --target {targets[0]}'
    info('\nDone. Run the game with: python tools/run.py' + hint)
    if 'macos' in targets and host_os() != 'macos':
        warn('The macOS app is unsigned. See README "macOS" for how to open it on a Mac.')


if __name__ == '__main__':
    run_main(main)
