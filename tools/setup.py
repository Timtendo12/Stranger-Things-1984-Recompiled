"""Prepare the Unity project from your own copy of the game.

    python tools/setup.py --apk <path to .apkm / .xapk / .apks / base.apk / extracted folder>

Steps: validate the APK, extract only the Unity data and libil2cpp.so into .cache/staging,
export them with AssetRipper (downloaded once into .cache/tools, checksum-verified), copy the
exported assets into UnityProject/ (never overwriting files tracked in git), apply the port's
post-processing, and verify the result. Nothing outside .cache/ and the gitignored asset
folders of UnityProject/ is written.

Run with --help for all options.
"""
import argparse
import hashlib
import json
import os
import shutil
import socket
import subprocess
import sys
import tempfile
import time
import urllib.error
import urllib.parse
import urllib.request
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import safearchive  # noqa: E402
from _common import (CACHE, PROJECT, SETUP_STAMP, ToolError, host_arch, host_os, info,  # noqa: E402
                     load_supported, run_main, warn)

STAGING = CACHE / 'staging'
EXPORT = CACHE / 'export'
TOOLS = CACHE / 'tools'
GENERATED_LIST = CACHE / 'generated_files.txt'

DATA_PREFIX = 'assets/bin/Data/'
IL2CPP = 'lib/arm64-v8a/libil2cpp.so'
METADATA = DATA_PREFIX + 'Managed/Metadata/global-metadata.dat'

# Export paths that the repository provides itself (ported code, settings, packages) or that the
# port removed. Everything else under the export's Assets/ is copied into UnityProject/Assets/.
SKIP_PREFIXES = (
    'Assets/Scripts/',
    'Assets/Plugins/Assembly-CSharp-firstpass/',
)
REMOVED_FILES = {
    'Assets/Plugins/BXPCoreUtil.dll', 'Assets/Plugins/NetflixSdk.dll',
    'Assets/Plugins/NetflixSdk.Internal.Android.dll', 'Assets/Plugins/NetflixSdk.Internal.Common.dll',
    'Assets/Plugins/Newtonsoft.Json.dll',
}


def sha256_file(path):
    h = hashlib.sha256()
    with open(path, 'rb') as f:
        for chunk in iter(lambda: f.read(1024 * 1024), b''):
            h.update(chunk)
    return h.hexdigest()


# ---------------------------------------------------------------- input / validation

def _is_apk(zf):
    names = set(zf.namelist())
    return 'AndroidManifest.xml' in names


def _apk_roles(zf):
    names = set(zf.namelist())
    return {'data': METADATA in names, 'lib': IL2CPP in names}


def open_inputs(apk, splits, tmp, opened):
    """Return (data_zip, lib_zip, manifest_bytes) for the user's input files. Every ZipFile opened is
    appended to `opened` so the caller can close them (required on Windows before temp cleanup)."""
    def _open(path):
        z = safearchive.open_zip(path)
        opened.append(z)
        return z

    apk = Path(apk).expanduser()
    if not apk.exists():
        raise ToolError(f'"{apk}" does not exist.')
    if apk.is_dir():
        raise ToolError('Internal: directories are handled separately.')
    candidates = []
    outer = _open(apk)
    safearchive.check_zip(outer)
    if _is_apk(outer):
        candidates.append((apk.name, outer))
    else:
        nested = [n for n in outer.namelist() if n.lower().endswith('.apk')]
        if not nested:
            raise ToolError(f'{apk.name} is neither an APK nor an APK bundle (.apkm/.xapk/.apks): '
                            'it contains no AndroidManifest.xml and no .apk files.')
        info(f'  {apk.name} is an APK bundle with {len(nested)} APKs; looking for the base and arm64 APKs...')
        for n in nested:
            safearchive.safe_member_path(n)
            # Only the base APK and the arm64 split are relevant; skip the other ABI splits.
            low = n.lower()
            if ('armeabi' in low or 'x86' in low) and 'arm64' not in low:
                continue
            out = Path(tmp) / Path(n).name
            safearchive.extract_zip(outer, tmp, select=lambda s, n=n: s == n, rename=lambda s: Path(s).name)
            inner = _open(out)
            safearchive.check_zip(inner)
            if _is_apk(inner):
                candidates.append((n, inner))
    for s in splits:
        z = _open(Path(s).expanduser())
        safearchive.check_zip(z)
        if not _is_apk(z):
            raise ToolError(f'{s} is not an APK.')
        candidates.append((Path(s).name, z))
    data = next((z for n, z in candidates if _apk_roles(z)['data']), None)
    lib = next((z for n, z in candidates if _apk_roles(z)['lib']), None)
    if data is None:
        raise ToolError('No APK with Unity game data (assets/bin/Data) was found. Is this the right game?')
    if lib is None:
        raise ToolError('The 64-bit ARM library lib/arm64-v8a/libil2cpp.so is missing. If you supplied a single base.apk, '
                        'also pass the arm64 split with --split split_config.arm64_v8a.apk, or supply the whole '
                        '.apkm/.xapk/.apks bundle.')
    manifest = data.read('AndroidManifest.xml')
    return data, lib, manifest


def check_package(manifest, supported, allow):
    pname = supported['package']
    if pname.encode('utf-16-le') not in manifest and pname.encode('utf-8') not in manifest:
        msg = f'The APK does not appear to be {supported["title"]} ({pname}).'
        if not allow:
            raise ToolError(msg + ' Use --allow-unsupported to try anyway.')
        warn(msg)


def stage(args, supported):
    """Fill .cache/staging with assets/bin/Data/** and lib/arm64-v8a/libil2cpp.so; return its path."""
    src = Path(args.apk).expanduser()
    if src.is_dir():
        for rel in (METADATA, IL2CPP):
            if not (src / rel).is_file():
                raise ToolError(f'The folder {src} is not an extracted APK: missing {rel}.')
        info(f'Using already-extracted game folder {src}')
        return src
    if STAGING.exists():
        shutil.rmtree(STAGING)
    STAGING.mkdir(parents=True)
    opened = []
    with tempfile.TemporaryDirectory(dir=CACHE, prefix='apk_') as tmp:
        try:
            info(f'Reading {src.name}...')
            data, lib, manifest = open_inputs(src, args.split, tmp, opened)
            check_package(manifest, supported, args.allow_unsupported)
            info('Extracting Unity data and libil2cpp.so...')
            n = safearchive.extract_zip(data, STAGING, select=lambda s: s.startswith(DATA_PREFIX) or s == 'AndroidManifest.xml')
            n += safearchive.extract_zip(lib, STAGING, select=lambda s: s == IL2CPP)
        finally:
            for z in opened:
                z.close()
    info(f'  {n} files extracted to {STAGING}')
    return STAGING


def fingerprint(staged, supported, allow):
    meta = sha256_file(staged / METADATA)
    lib = sha256_file(staged / IL2CPP)
    for v in supported['versions']:
        if v['global_metadata_sha256'] == meta and v['libil2cpp_sha256'] == lib:
            info(f'Recognised {supported["title"]} {v["version"]} (versionCode {v["version_code"]}).')
            return v['version'], meta, lib
    known = ', '.join(v['version'] for v in supported['versions'])
    msg = (f'This APK is not a supported version of {supported["title"]} (supported: {known}).\n'
           f'  global-metadata.dat sha256 {meta}\n  libil2cpp.so        sha256 {lib}\n'
           'The port is compiled against one exact game version; other versions will most likely fail.')
    if not allow:
        raise ToolError(msg + '\nUse --allow-unsupported to try anyway.')
    warn(msg)
    return 'unsupported', meta, lib


# ---------------------------------------------------------------- AssetRipper

def assetripper_exe(folder):
    names = ['AssetRipper.GUI.Free.exe'] if host_os() == 'windows' else ['AssetRipper.GUI.Free']
    for n in names:
        hits = list(Path(folder).rglob(n))
        if hits:
            return hits[0]
    return None


def ensure_assetripper(args, supported):
    ar = supported['assetripper']
    if args.assetripper:
        p = Path(args.assetripper).expanduser()
        exe = assetripper_exe(p) if p.is_dir() else p
        if not exe or not exe.is_file():
            raise ToolError(f'AssetRipper executable not found at {args.assetripper}.')
        warn(f'Using your own AssetRipper at {exe}; the tested version is {ar["version"]}.')
        return exe
    key = f'{host_os()}_{host_arch()}'
    asset = ar['assets'].get(key)
    if not asset:
        raise ToolError(f'No AssetRipper download is configured for {key}. Download it yourself and pass --assetripper.')
    folder = TOOLS / f'AssetRipper-{ar["version"]}-{key}'
    exe = assetripper_exe(folder) if folder.exists() else None
    if exe:
        return exe
    TOOLS.mkdir(parents=True, exist_ok=True)
    url = ar['url'].format(version=ar['version'], file=asset['file'])
    archive = TOOLS / asset['file']
    if not archive.exists() or sha256_file(archive) != asset['sha256']:
        info(f'Downloading AssetRipper {ar["version"]} ({asset["file"]}) from GitHub...')
        part = archive.with_suffix(archive.suffix + '.part')
        try:
            with urllib.request.urlopen(url, timeout=60) as r, open(part, 'wb') as f:
                shutil.copyfileobj(r, f, 1024 * 1024)
        except (urllib.error.URLError, OSError) as e:
            raise ToolError(f'Download of {url} failed: {e}\nDownload it manually into {TOOLS} or pass --assetripper.')
        part.replace(archive)
    digest = sha256_file(archive)
    if digest != asset['sha256']:
        archive.unlink()
        raise ToolError(f'Checksum mismatch for {asset["file"]} (got {digest}); the download was deleted. Try again.')
    info('  checksum OK, extracting...')
    if folder.exists():
        shutil.rmtree(folder)
    if asset['file'].endswith('.zip'):
        with safearchive.open_zip(archive) as z:
            safearchive.extract_zip(z, folder)
    else:
        safearchive.extract_tar(archive, folder)
    exe = assetripper_exe(folder)
    if not exe:
        raise ToolError(f'AssetRipper executable not found in {archive}.')
    if host_os() != 'windows':
        exe.chmod(0o755)
    return exe


class _NoRedirect(urllib.request.HTTPRedirectHandler):
    def redirect_request(self, *a, **k):
        return None


def _post(opener, base, path, fields, timeout):
    data = urllib.parse.urlencode(fields, doseq=True).encode()
    req = urllib.request.Request(base + path, data=data, method='POST')
    try:
        with opener.open(req, timeout=timeout) as r:
            return r.status
    except urllib.error.HTTPError as e:
        if e.code in (301, 302, 303):
            return e.code
        raise ToolError(f'AssetRipper request {path} failed with HTTP {e.code}. See the AssetRipper log in {CACHE}.')


def run_assetripper(exe, staged, supported):
    if EXPORT.exists():
        shutil.rmtree(EXPORT)
    with socket.socket() as s:
        s.bind(('127.0.0.1', 0))
        port = s.getsockname()[1]
    log = CACHE / 'assetripper.log'
    console = CACHE / 'assetripper_console.log'
    info(f'Running AssetRipper (this takes several minutes; log: {log})...')
    out = open(console, 'w', encoding='utf-8', errors='replace')
    proc = subprocess.Popen([str(exe), '--port', str(port), '--headless', '--log-path', str(log)],
                            cwd=str(exe.parent), stdout=out, stderr=subprocess.STDOUT)
    base = f'http://127.0.0.1:{port}'
    opener = urllib.request.build_opener(_NoRedirect)

    def failure(what):
        # AssetRipper reports exceptions by logging them and redirecting to its home page, so the HTTP
        # status cannot be trusted; show the errors from its console output instead.
        out.flush()
        text = console.read_text(encoding='utf-8', errors='replace') if console.exists() else ''
        lines = text.splitlines()
        hits = [i for i, l in enumerate(lines) if 'Error' in l or 'Exception' in l]
        detail = '\n'.join(lines[hits[0]:hits[0] + 25]) if hits else '\n'.join(lines[-25:])
        return ToolError(f'AssetRipper failed while {what}.\n--- AssetRipper output ---\n{detail}\n---\n'
                         f'Full output: {console}\nLog: {log}')

    def log_has(marker, wait=20):
        end = time.time() + wait
        while time.time() < end:
            for p in (console, log):
                if p.exists() and marker in p.read_text(encoding='utf-8', errors='replace'):
                    return True
            time.sleep(0.5)
        return False

    try:
        deadline = time.time() + 120
        while True:
            if proc.poll() is not None:
                raise ToolError(f'AssetRipper exited immediately (code {proc.returncode}). See {log}.')
            try:
                with urllib.request.urlopen(base + '/', timeout=2):
                    break
            except (urllib.error.URLError, OSError):
                if time.time() > deadline:
                    raise ToolError('AssetRipper did not start within 2 minutes.')
                time.sleep(0.5)
        settings = supported['assetripper']['settings']
        fields = {k: v for k, v in settings.items() if not isinstance(v, bool)}
        fields.update({k: 'true' for k, v in settings.items() if v is True})
        _post(opener, base, '/Settings/Update', fields, 60)
        t = time.time()
        info('  loading game files...')
        _post(opener, base, '/LoadFolder', {'Path': str(Path(staged).resolve())}, 3 * 3600)
        if not log_has('Finished reading files'):
            raise failure('reading the game files')
        info(f'  loaded in {time.time() - t:.0f}s; exporting Unity project...')
        t = time.time()
        _post(opener, base, '/Export/UnityProject', {'Path': str(EXPORT.resolve())}, 3 * 3600)
        if not (EXPORT / 'ExportedProject' / 'Assets').is_dir():
            raise failure('exporting the Unity project')
        info(f'  exported in {time.time() - t:.0f}s')
    finally:
        proc.terminate()
        try:
            proc.wait(15)
        except subprocess.TimeoutExpired:
            proc.kill()
        out.close()
    return EXPORT / 'ExportedProject'


# ---------------------------------------------------------------- assemble into UnityProject

def remove_previous_generated():
    if not GENERATED_LIST.exists():
        return
    n = 0
    for rel in GENERATED_LIST.read_text(encoding='utf-8').splitlines():
        p = PROJECT / rel
        if rel and p.is_file():
            p.unlink()
            n += 1
    GENERATED_LIST.unlink()
    # prune now-empty folders (and their Unity folder metas) below Assets/
    for d in sorted((PROJECT / 'Assets').rglob('*'), key=lambda x: len(x.parts), reverse=True):
        if d.is_dir() and not any(d.iterdir()):
            d.rmdir()
            meta = d.with_name(d.name + '.meta')
            if meta.exists():
                meta.unlink()
    info(f'Removed {n} previously generated files.')


def assemble(export_root):
    remove_previous_generated()
    src_assets = export_root / 'Assets'
    generated, skipped_tracked = [], 0
    for src in sorted(src_assets.rglob('*')):
        if not src.is_file():
            continue
        rel = src.relative_to(export_root).as_posix()
        if rel.startswith(SKIP_PREFIXES) or rel in REMOVED_FILES or rel[:-5] in REMOVED_FILES:
            continue
        dst = PROJECT / rel
        if dst.exists():
            # Present in a fresh checkout => a file the repository owns (e.g. the hand-written
            # shader replacements). Keep ours; the export's .meta next to it still gets copied.
            skipped_tracked += 1
            continue
        dst.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(src, dst)
        generated.append(rel)
    GENERATED_LIST.write_text('\n'.join(generated) + '\n', encoding='utf-8')
    info(f'Copied {len(generated)} asset files into UnityProject/ (kept {skipped_tracked} repository files).')
    return generated


def patch_plugin_dlls(supported):
    """Retarget the AssetRipper stub DLLs from the AOT public key token to the standard one."""
    p = supported['dll_patch']
    old, new = bytes.fromhex(p['from']), bytes.fromhex(p['to'])
    for name in p['files']:
        f = PROJECT / 'Assets' / 'Plugins' / name
        if not f.exists():
            raise ToolError(f'Expected plugin {name} is missing from the export.')
        data = f.read_bytes()
        if old not in data:
            if new in data:
                continue
            raise ToolError(f'{name} does not contain the expected public key token; incompatible export.')
        f.write_bytes(data.replace(old, new))
    info(f'Patched {len(p["files"])} plugin DLLs.')


def pin_guids(generated, supported):
    """Give assets referenced from tracked settings the GUIDs the repository expects."""
    remap = {}
    for rel, want in supported['guid_pins'].items():
        meta = PROJECT / (rel + '.meta')
        if not meta.exists():
            raise ToolError(f'{rel} is missing from the export.')
        for line in meta.read_text(encoding='utf-8').splitlines():
            if line.startswith('guid: '):
                have = line[6:].strip()
                if have != want:
                    remap[have] = want
                break
    if not remap:
        return
    info(f'Re-pinning {len(remap)} asset GUIDs referenced by the project settings...')
    for rel in generated:
        p = PROJECT / rel
        if p.suffix in ('.dll', '.png', '.ogg', '.wav', '.mp4', '.ttf', '.otf', '.bytes', '.jpg'):
            continue
        try:
            text = p.read_text(encoding='utf-8')
        except UnicodeDecodeError:
            continue
        if any(k in text for k in remap):
            for k, v in remap.items():
                text = text.replace(k, v)
            p.write_text(text, encoding='utf-8', newline='')


def verify(supported):
    problems = []
    for rel, minimum in supported['required'].items():
        p = PROJECT / rel
        if minimum == 0:
            if not p.is_file():
                problems.append(f'missing file {rel}')
        else:
            n = sum(1 for f in p.rglob('*') if f.is_file() and f.suffix != '.meta') if p.is_dir() else 0
            if n < minimum:
                problems.append(f'{rel}: {n} files, expected at least {minimum}')
    for shader in list((PROJECT / 'Assets').rglob('*.shader')):
        if not shader.with_name(shader.name + '.meta').exists():
            problems.append(f'{shader.relative_to(PROJECT).as_posix()} has no .meta (asset GUID unknown)')
    if problems:
        raise ToolError('The prepared project is incomplete:\n  ' + '\n  '.join(problems) +
                        '\nRe-run setup; if it persists, your APK is probably not a supported version.')
    info('All required assets are present.')


def clean():
    remove_previous_generated()
    for p in (STAGING, EXPORT, SETUP_STAMP):
        if p.is_dir():
            shutil.rmtree(p)
        elif p.exists():
            p.unlink()
    info('Removed generated assets and setup caches (downloaded tools in .cache/tools are kept).')


def main():
    ap = argparse.ArgumentParser(description='Prepare UnityProject/ from your own copy of Stranger Things: 1984.')
    ap.add_argument('apk', nargs='?', help='.apkm / .xapk / .apks bundle, base.apk, or an extracted APK folder')
    ap.add_argument('--apk', dest='apk_opt', help=argparse.SUPPRESS)
    ap.add_argument('--split', action='append', default=[], help='extra split APK (e.g. split_config.arm64_v8a.apk)')
    ap.add_argument('--assetripper', help='use an existing AssetRipper (folder or executable) instead of downloading')
    ap.add_argument('--allow-unsupported', action='store_true', help='continue when the game version is not recognised')
    ap.add_argument('--keep-cache', action='store_true', help='keep .cache/staging and .cache/export afterwards')
    ap.add_argument('--clean', action='store_true', help='remove everything setup generated, then exit')
    args = ap.parse_args()
    args.apk = args.apk_opt or args.apk

    if args.clean:
        clean()
        return
    supported = load_supported()
    if not args.apk:
        info(f'Supply your own copy of {supported["title"]} {supported["versions"][0]["version"]} '
             '(see README "Getting the APK").')
        args.apk = input('Path to your .apkm / .xapk / .apks / base.apk: ').strip().strip('"').strip("'")
        if not args.apk:
            raise ToolError('No APK given.')
    CACHE.mkdir(exist_ok=True)

    staged = stage(args, supported)
    version, meta_sha, lib_sha = fingerprint(staged, supported, args.allow_unsupported)
    exe = ensure_assetripper(args, supported)
    export_root = run_assetripper(exe, staged, supported)
    generated = assemble(export_root)
    patch_plugin_dlls(supported)
    pin_guids(generated, supported)
    verify(supported)
    SETUP_STAMP.write_text(json.dumps({
        'game_version': version, 'global_metadata_sha256': meta_sha, 'libil2cpp_sha256': lib_sha,
        'assetripper': supported['assetripper']['version'] if not args.assetripper else 'custom',
        'time': time.strftime('%Y-%m-%d %H:%M:%S'),
    }, indent=2), encoding='utf-8')
    if not args.keep_cache:
        for p in (STAGING, EXPORT):
            if p.exists():
                shutil.rmtree(p)
    info('\nSetup complete. Next: python tools/build.py   (see README for targets and options)')


if __name__ == '__main__':
    run_main(main)
