#!/usr/bin/env bash
# One-step build for Linux.
#   ./build_linux.sh path/to/StrangerThings.apkm   first time: prepare assets from your APK, then build
#   ./build_linux.sh                               later: rebuild (asks for the APK if setup never ran)
# Extra options (--dev, --clean, --full-clean) are passed on to tools/build.py.
set -euo pipefail
cd "$(dirname "$0")"
TARGET=linux

PY=""
for c in python3 python; do
    if command -v "$c" >/dev/null 2>&1 && "$c" -c 'import sys; sys.exit(sys.version_info < (3, 10))' 2>/dev/null; then
        PY="$c"; break
    fi
done
if [ -z "$PY" ]; then
    echo "ERROR: Python 3.10 or newer is required (e.g. sudo apt install python3) and try again." >&2
    exit 1
fi

APK=""
if [ $# -gt 0 ] && [ "${1#-}" = "$1" ]; then
    APK="$1"; shift
fi
if [ -n "$APK" ]; then
    "$PY" tools/setup.py "$APK"
elif [ ! -f .cache/setup_ok.json ]; then
    "$PY" tools/setup.py
fi

"$PY" tools/build.py --target "$TARGET" "$@"

BIN="dist/linux/StrangerThings1984.x86_64"
chmod +x "$BIN"
echo
echo "Start the game with:  ./$BIN"
