"""Shared helpers for the setup/build/run tools: repo paths, .env loading, Unity editor discovery."""
import json
import os
import platform
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
PROJECT = ROOT / 'UnityProject'
CACHE = ROOT / '.cache'
DIST = ROOT / 'dist'
SETUP_STAMP = CACHE / 'setup_ok.json'
SUPPORTED = Path(__file__).resolve().parent / 'supported.json'


class ToolError(Exception):
    """An error with an actionable message; printed without a traceback."""


def info(msg):
    print(msg, flush=True)


def warn(msg):
    print('WARNING: ' + msg, file=sys.stderr, flush=True)


def run_main(main):
    try:
        sys.exit(main() or 0)
    except ToolError as e:
        print('\nERROR: ' + str(e), file=sys.stderr)
        sys.exit(1)
    except KeyboardInterrupt:
        print('\nInterrupted.', file=sys.stderr)
        sys.exit(130)


def load_supported():
    return json.loads(SUPPORTED.read_text(encoding='utf-8'))


def load_env():
    """Values from ROOT/.env, overridden by real environment variables of the same name."""
    values = {}
    env = ROOT / '.env'
    if env.exists():
        for line in env.read_text(encoding='utf-8-sig').splitlines():
            line = line.strip()
            if not line or line.startswith('#') or '=' not in line:
                continue
            k, v = line.split('=', 1)
            values[k.strip()] = v.strip().strip('"').strip("'")
    for k in list(values) + ['UNITY_EDITOR', 'DISCORD_CLIENT_ID', 'DISCORD_LARGE_IMAGE_KEY', 'DISCORD_ENABLED']:
        if k in os.environ:
            values[k] = os.environ[k]
    return values


def host_os():
    s = platform.system()
    if s == 'Windows':
        return 'windows'
    if s == 'Darwin':
        return 'macos'
    if s == 'Linux':
        return 'linux'
    raise ToolError(f'Unsupported host OS: {s}')


def host_arch():
    m = platform.machine().lower()
    if m in ('amd64', 'x86_64', 'x64'):
        return 'x64'
    if m in ('arm64', 'aarch64'):
        return 'arm64'
    raise ToolError(f'Unsupported CPU architecture: {m}')


def unity_version():
    text = (PROJECT / 'ProjectSettings' / 'ProjectVersion.txt').read_text(encoding='utf-8')
    m = re.search(r'm_EditorVersion:\s*(\S+)', text)
    if not m:
        raise ToolError('Cannot read m_EditorVersion from UnityProject/ProjectSettings/ProjectVersion.txt')
    return m.group(1)


def _hub_secondary_dir():
    o = host_os()
    if o == 'windows':
        p = Path(os.environ.get('APPDATA', '')) / 'UnityHub' / 'secondaryInstallPath.json'
    elif o == 'macos':
        p = Path.home() / 'Library' / 'Application Support' / 'UnityHub' / 'secondaryInstallPath.json'
    else:
        p = Path.home() / '.config' / 'UnityHub' / 'secondaryInstallPath.json'
    try:
        v = json.loads(p.read_text(encoding='utf-8'))
        return Path(v) if v else None
    except (OSError, ValueError):
        return None


def _editor_exe(version_dir):
    if host_os() == 'windows':
        return version_dir / 'Editor' / 'Unity.exe'
    if host_os() == 'macos':
        return version_dir / 'Unity.app' / 'Contents' / 'MacOS' / 'Unity'
    return version_dir / 'Editor' / 'Unity'


def find_unity_editor():
    """Path to the Unity editor executable matching ProjectVersion.txt (UNITY_EDITOR overrides)."""
    version = unity_version()
    configured = load_env().get('UNITY_EDITOR', '').strip()
    if configured:
        p = Path(configured).expanduser()
        if p.is_dir():
            p = _editor_exe(p) if (_editor_exe(p)).exists() else p / 'Unity'
        if not p.exists():
            raise ToolError(f'UNITY_EDITOR points to "{configured}", which does not exist.')
        if version not in str(p):
            warn(f'UNITY_EDITOR does not look like Unity {version}: {p}. The project expects exactly {version}.')
        return p
    hubs = []
    o = host_os()
    if o == 'windows':
        for var in ('ProgramFiles', 'ProgramW6432'):
            if os.environ.get(var):
                hubs.append(Path(os.environ[var]) / 'Unity' / 'Hub' / 'Editor')
    elif o == 'macos':
        hubs.append(Path('/Applications/Unity/Hub/Editor'))
    else:
        hubs.append(Path.home() / 'Unity' / 'Hub' / 'Editor')
    sec = _hub_secondary_dir()
    if sec:
        hubs.insert(0, sec)
    for hub in hubs:
        exe = _editor_exe(hub / version)
        if exe.exists():
            return exe
    raise ToolError(
        f'Unity {version} was not found (looked in: {", ".join(str(h) for h in hubs)}).\n'
        f'Install it with Unity Hub (Installs > Install Editor > Archive > {version}), '
        'or set UNITY_EDITOR in .env to the editor executable.')


def playback_engines_dirs(editor_exe):
    """Folders that hold the platform build-support modules of an editor install."""
    if host_os() == 'macos':
        # .../<version>/Unity.app/Contents/MacOS/Unity: the built-in Mac (Mono) support lives in
        # Unity.app/Contents/PlaybackEngines, modules added through Unity Hub in <version>/PlaybackEngines
        return [editor_exe.parents[1] / 'PlaybackEngines', editor_exe.parents[3] / 'PlaybackEngines']
    return [editor_exe.parent / 'Data' / 'PlaybackEngines']
