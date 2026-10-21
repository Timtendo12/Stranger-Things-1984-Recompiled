"""Launch the built game.

    python tools/run.py [--target windows|macos|linux|web] [--fullscreen] [--seconds N] [-- extra player args]

By default the game runs windowed at 1280x720 until you quit. With --seconds N it is stopped after
N seconds and a summary of exceptions from the Unity player log is printed (useful for testing).
--target web serves dist/web on http://localhost:<--port> and opens it in the browser until Ctrl+C.
"""
import argparse
import functools
import http.server
import subprocess
import sys
import time
import webbrowser
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from _common import DIST, ToolError, host_os, info, run_main  # noqa: E402

PRODUCTS = {
    'windows': 'StrangerThings1984.exe',
    'macos': 'StrangerThings1984.app/Contents/MacOS/StrangerThings1984',
    'linux': 'StrangerThings1984.x86_64',
    'web': 'index.html',
}


def summarize(log):
    if not log.exists():
        info(f'No player log at {log}')
        return
    lines = log.read_text(encoding='utf-8', errors='replace').splitlines()
    seen = {}
    for i, line in enumerate(lines):
        if ('Exception' in line or 'Error' in line) and not line.lstrip().startswith('at ') and 'Filename:' not in line:
            frames = [l.strip() for l in lines[i + 1:i + 5] if l.strip()]
            key = line + '|' + (frames[0] if frames else '')
            if key in seen:
                seen[key] += 1
                continue
            seen[key] = 1
            info('!! ' + line)
            for f in frames[:3]:
                info('     ' + f)
    info(f'{len(seen)} distinct error/exception messages. Full log: {log}')


def serve_web(port):
    root = DIST / 'web'
    if not (root / PRODUCTS['web']).exists():
        raise ToolError(f'{root / PRODUCTS["web"]} does not exist. Build it first: python tools/build.py --target web')
    handler = functools.partial(http.server.SimpleHTTPRequestHandler, directory=str(root))
    with http.server.ThreadingHTTPServer(('127.0.0.1', port), handler) as server:
        url = f'http://localhost:{port}/'
        info(f'Serving {root} at {url} (Ctrl+C to stop)')
        webbrowser.open(url)
        try:
            server.serve_forever()
        except KeyboardInterrupt:
            pass
    return 0


def main():
    ap = argparse.ArgumentParser(description='Run the built game.')
    ap.add_argument('--target', choices=list(PRODUCTS), default=host_os())
    ap.add_argument('--fullscreen', action='store_true')
    ap.add_argument('--seconds', type=int, help='stop after N seconds and summarise the player log')
    ap.add_argument('--port', type=int, default=8000, help='web: local port to serve dist/web on')
    ap.add_argument('extra', nargs='*', help='extra arguments passed to the player (after --)')
    args = ap.parse_args()
    if args.target == 'web':
        return serve_web(args.port)
    if args.target != host_os():
        raise ToolError(f'A {args.target} build cannot be run on {host_os()}; copy dist/{args.target} to that OS.')
    exe = DIST / args.target / PRODUCTS[args.target]
    if not exe.exists():
        raise ToolError(f'{exe} does not exist. Build it first: python tools/build.py --target {args.target}')
    log = DIST / 'logs' / f'player-{args.target}.log'
    log.parent.mkdir(parents=True, exist_ok=True)
    cmd = [str(exe), '-logFile', str(log), '-screen-fullscreen', '1' if args.fullscreen else '0']
    if not args.fullscreen:
        cmd += ['-screen-width', '1280', '-screen-height', '720']
    cmd += args.extra
    info(f'Starting {exe.name} (player log: {log})')
    p = subprocess.Popen(cmd, cwd=str(exe.parent))
    if args.seconds is None:
        return p.wait()
    deadline = time.time() + args.seconds
    while p.poll() is None and time.time() < deadline:
        time.sleep(0.5)
    exited = p.poll() is not None
    if not exited:
        p.terminate()
        try:
            p.wait(10)
        except subprocess.TimeoutExpired:
            p.kill()
    info(f'== game exited by itself: {exited}')
    summarize(log)
    return 0


if __name__ == '__main__':
    run_main(main)
