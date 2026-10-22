#!/usr/bin/env bash
# One-step web (WebGL) build, from macOS or Linux.
#   ./build_webgl.sh path/to/StrangerThings.apkm   first time: prepare assets from your APK, then build
#   ./build_webgl.sh                               later: rebuild (asks for the APK if setup never ran)
# Extra options (--dev, --clean, --full-clean) are passed on to tools/build.py.
set -euo pipefail
cd "$(dirname "$0")"
TARGET=web

PY=""
for c in python3 python; do
    if command -v "$c" >/dev/null 2>&1 && "$c" -c 'import sys; sys.exit(sys.version_info < (3, 10))' 2>/dev/null; then
        PY="$c"; break
    fi
done
if [ -z "$PY" ]; then
    echo "ERROR: Python 3.10 or newer is required. Install it from https://www.python.org/downloads/ (or your package manager) and try again." >&2
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

echo
echo "Start the game with:  $PY tools/run.py --target web   (serves dist/web on http://localhost:8000)"
