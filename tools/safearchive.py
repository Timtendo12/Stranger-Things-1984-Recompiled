"""Defensive extraction of untrusted zip / tar archives (user-supplied APKs, downloaded tools).

Every member name is validated before anything is written: no absolute paths, drive letters,
'..' components, backslashes, NUL bytes, symlinks or device files. The resolved destination
must stay inside the target directory, and size / compression-ratio limits guard against
archive bombs.
"""
import shutil
import stat
import tarfile
import zipfile
from pathlib import Path, PurePosixPath

from _common import ToolError

MAX_TOTAL_BYTES = 4 * 1024 ** 3      # 4 GiB uncompressed per extraction
MAX_RATIO = 400                        # uncompressed / compressed, per member above 1 MiB


def safe_member_path(name):
    """Validated relative PurePosixPath for an archive member name, or raise ToolError."""
    if not name or '\x00' in name or '\\' in name:
        raise ToolError(f'Archive contains an invalid file name: {name!r}')
    p = PurePosixPath(name)
    if p.is_absolute() or name.startswith('/') or (len(name) > 1 and name[1] == ':'):
        raise ToolError(f'Archive contains an absolute path: {name!r}')
    if any(part in ('..', '') for part in p.parts if part != '.') or not p.parts:
        raise ToolError(f'Archive contains a path that escapes the target folder: {name!r}')
    return p


def _target(dest, rel):
    dest = dest.resolve()
    out = (dest / Path(*rel.parts)).resolve()
    if out != dest and dest not in out.parents:
        raise ToolError(f'Archive member would be written outside {dest}: {rel}')
    return out


def open_zip(path):
    try:
        return zipfile.ZipFile(path)
    except zipfile.BadZipFile:
        raise ToolError(f'{path} is not a valid zip/APK archive (it may be truncated or a different file type).')
    except OSError as e:
        raise ToolError(f'Cannot read {path}: {e}')


def check_zip(zf, select=None):
    """Validate all member names; return the (name, ZipInfo) pairs accepted by select(name)."""
    chosen, total = [], 0
    for zi in zf.infolist():
        rel = safe_member_path(zi.filename.rstrip('/') if zi.is_dir() else zi.filename)
        mode = zi.external_attr >> 16
        if stat.S_ISLNK(mode):
            raise ToolError(f'Archive contains a symbolic link, which is not allowed: {zi.filename}')
        if zi.flag_bits & 0x1:
            raise ToolError(f'Archive member {zi.filename} is encrypted. Supply an unencrypted APK/APKM.')
        if zi.is_dir() or (select and not select(str(rel))):
            continue
        if zi.file_size > 1024 ** 2 and zi.compress_size and zi.file_size / zi.compress_size > MAX_RATIO:
            raise ToolError(f'Archive member {zi.filename} has a suspicious compression ratio; refusing to extract.')
        total += zi.file_size
        if total > MAX_TOTAL_BYTES:
            raise ToolError('Archive expands to more than 4 GiB; refusing to extract.')
        chosen.append((rel, zi))
    return chosen


def extract_zip(zf, dest, select=None, rename=None):
    """Extract the selected members of an open ZipFile into dest. rename(rel) may map member paths."""
    dest = Path(dest)
    dest.mkdir(parents=True, exist_ok=True)
    count = 0
    for rel, zi in check_zip(zf, select):
        out_rel = PurePosixPath(rename(str(rel))) if rename else rel
        out = _target(dest, safe_member_path(str(out_rel)))
        out.parent.mkdir(parents=True, exist_ok=True)
        written = 0
        with zf.open(zi) as src, open(out, 'wb') as dst:
            while True:
                chunk = src.read(1024 * 1024)
                if not chunk:
                    break
                written += len(chunk)
                if written > zi.file_size:
                    raise ToolError(f'Archive member {zi.filename} is larger than its header says; refusing.')
                dst.write(chunk)
        count += 1
    return count


def extract_tar(path, dest):
    """Extract a .tar.xz/.tar.gz (downloaded tool release) with the same validation rules."""
    dest = Path(dest)
    dest.mkdir(parents=True, exist_ok=True)
    total = 0
    try:
        tf = tarfile.open(path)
    except (tarfile.TarError, OSError) as e:
        raise ToolError(f'{path} is not a valid tar archive: {e}')
    with tf:
        for m in tf.getmembers():
            rel = safe_member_path(m.name.rstrip('/'))
            if m.isdir():
                _target(dest, rel).mkdir(parents=True, exist_ok=True)
                continue
            if not m.isfile():
                raise ToolError(f'Archive contains a link or special file, which is not allowed: {m.name}')
            total += m.size
            if total > MAX_TOTAL_BYTES:
                raise ToolError('Archive expands to more than 4 GiB; refusing to extract.')
            out = _target(dest, rel)
            out.parent.mkdir(parents=True, exist_ok=True)
            with tf.extractfile(m) as src, open(out, 'wb') as dst:
                shutil.copyfileobj(src, dst, 1024 * 1024)
            if m.mode & 0o111:
                out.chmod(0o755)
