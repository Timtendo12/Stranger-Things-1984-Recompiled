"""Build the game with Unity in batch mode.

    python tools/build.py [--target windows|macos|linux|all] [--dev] [--clean | --full-clean]

Output: dist/windows/StrangerThings1984.exe, dist/macos/StrangerThings1984.app,
dist/linux/StrangerThings1984.x86_64. Unity's log goes to dist/logs/build-<target>.log.
Requires tools/setup.py to have completed and Unity (version in ProjectSettings/ProjectVersion.txt)
with the Mono build-support module for each target.
"""
import argparse
import re
import shutil
import subprocess
import sys
import time
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import gen_discord_config  # noqa: E402
from _common import (DIST, PROJECT, SETUP_STAMP, ToolError, find_unity_editor, host_os, info,  # noqa: E402
                     playback_engines_dir, run_main, unity_version, warn)

TARGETS = {
    # name: (Unity -buildTarget, PlaybackEngines module folder, Unity Hub module name, output file)
    'windows': ('Win64', 'windowsstandalonesupport', 'Windows Build Support (Mono)', 'StrangerThings1984.exe'),
    'macos': ('OSXUniversal', 'macstandalonesupport', 'Mac Build Support (Mono)', 'StrangerThings1984.app'),
    'linux': ('Linux64', 'linuxstandalonesupport', 'Linux Build Support (Mono)', 'StrangerThings1984.x86_64'),
}


def check_module(editor, target):
    engines = playback_engines_dir(editor)
    if not engines.is_dir():
        return  # unusual layout; let Unity report it
    have = {p.name.lower() for p in engines.iterdir()}
    _, folder, module, _ = TARGETS[target]
    if folder not in have:
        raise ToolError(f'Unity {unity_version()} has no {module} module, which is needed to build for {target}.\n'
                        f'Add it in Unity Hub: Installs > {unity_version()} > (gear) > Add modules > {module}.')


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


def build(editor, target, dev, out_root):
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
    first = not (PROJECT / 'Library').exists()
    info(f'Building {target}{" (development)" if dev else ""} with Unity {unity_version()}...'
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
    ap.add_argument('--out', type=Path, default=DIST, help='output root folder (default: dist/)')
    args = ap.parse_args()

    if not SETUP_STAMP.exists() or not (PROJECT / 'Assets' / 'Scenes' / 'Default.unity').exists():
        raise ToolError('The game assets are not set up yet. Run: python tools/setup.py <your APK>  (see README).')
    editor = find_unity_editor()
    out_root = args.out.resolve()
    targets = list(TARGETS) if args.target == 'all' else [args.target]
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
    built = [build(editor, t, args.dev, out_root) for t in targets]
    info('\nDone. Run the game with: python tools/run.py' + ('' if len(built) == 1 else ' --target <target>'))
    if 'macos' in targets and host_os() != 'macos':
        warn('The macOS app is unsigned. See README "macOS" for how to open it on a Mac.')


if __name__ == '__main__':
    run_main(main)
