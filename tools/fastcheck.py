"""Fast C# compile check without opening the Unity editor (developer aid, safe to run concurrently).

Reuses the compiler response files Unity generated (UnityProject/Library/Bee/artifacts/*.dag/<Asm>.rsp), but
 - writes outputs to a private temp directory,
 - rebuilds each assembly's source list from disk (new files are picked up),
 - compiles BXPCoreUtil -> Assembly-CSharp-firstpass -> Assembly-CSharp in order,
   with later assemblies referencing the freshly built earlier ones.
Usage: python tools/fastcheck.py [--only Assembly-CSharp-firstpass] ; exit code 0 = no errors.
Requires one successful Unity import/build beforehand (tools/build.py) to create the rsp files.
"""
import glob
import os
import re
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import gen_discord_config  # noqa: E402
from _common import PROJECT as _PROJECT, ToolError, find_unity_editor, host_os, run_main  # noqa: E402

PROJECT = str(_PROJECT)
ORDER = ['BXPCoreUtil', 'Assembly-CSharp-firstpass', 'Assembly-CSharp']
ROOTS = {
    'BXPCoreUtil': ['Assets/Scripts/BXPCoreUtil'],
    'Assembly-CSharp-firstpass': ['Assets/Plugins'],
}


def editor_data():
    exe = find_unity_editor()
    if host_os() == 'macos':
        return exe.parents[1]  # Unity.app/Contents
    return exe.parent / 'Data'


def sources(asm):
    if asm in ROOTS:
        files = []
        for r in ROOTS[asm]:
            files += [os.path.normpath(f) for f in glob.glob(os.path.join(PROJECT, r, '**', '*.cs'), recursive=True)]
        return [f for f in files if os.sep + 'Editor' + os.sep not in f]
    excluded = [os.path.normpath(os.path.join(PROJECT, p)) + os.sep for r in ROOTS.values() for p in r] + [os.path.join(PROJECT, 'Assets', 'Plugins') + os.sep]
    files = [os.path.normpath(f) for f in glob.glob(os.path.join(PROJECT, 'Assets', '**', '*.cs'), recursive=True)]
    return [f for f in files if os.sep + 'Editor' + os.sep not in f and not any(f.startswith(e) for e in excluded)]


def main():
    gen_discord_config.generate(quiet=True)
    data = editor_data()
    dotnet = data / 'NetCoreRuntime' / ('dotnet.exe' if host_os() == 'windows' else 'dotnet')
    csc = data / 'DotNetSdkRoslyn' / 'csc.dll'
    if not dotnet.exists() or not csc.exists():
        raise ToolError(f'Roslyn not found under {data}.')
    only = sys.argv[sys.argv.index('--only') + 1] if '--only' in sys.argv else None
    dag = sorted(glob.glob(os.path.join(PROJECT, 'Library', 'Bee', 'artifacts', '*E.dag')))
    if not dag:
        raise ToolError('No Bee rsp files; run python tools/build.py once.')
    dag = dag[0]
    out = tempfile.mkdtemp(prefix='fastcheck_')
    failed = False
    try:
        for asm in ORDER:
            rsp_path = os.path.join(dag, asm + '.rsp')
            lines = open(rsp_path, encoding='utf-8-sig').read().splitlines()
            new = []
            for l in lines:
                s = l.strip()
                if not s or s.endswith('.cs"') or s.endswith('.cs'):
                    continue
                if s.startswith('-out:'):
                    s = f'-out:"{os.path.join(out, asm + ".dll")}"'
                elif s.startswith('-refout:'):
                    s = f'-refout:"{os.path.join(out, asm + ".ref.dll")}"'
                elif s.startswith('-r:'):
                    m = re.search(r'Library/Bee/artifacts/[^/]+/([\w.-]+)\.ref\.dll', s)
                    if m and m.group(1) in ORDER:
                        s = f'-r:"{os.path.join(out, m.group(1) + ".ref.dll")}"'
                    elif 'Library/' in s:
                        s = s.replace('"Library/', '"' + os.path.join(PROJECT, 'Library').replace('\\', '/') + '/')
                elif s.startswith('-analyzer:') or s.startswith('-additionalfile:') or s.startswith('-generatedfilesout'):
                    continue
                new.append(s)
            new += ['"' + f + '"' for f in sources(asm)]
            rsp = os.path.join(out, asm + '.rsp')
            open(rsp, 'w', encoding='utf-8').write('\n'.join(new))
            r = subprocess.run([str(dotnet), str(csc), '/noconfig', '/nologo', '@' + rsp], cwd=PROJECT,
                               capture_output=True, text=True, encoding='utf-8', errors='replace')
            errs = sorted({l.strip().replace(PROJECT + os.sep, '') for l in (r.stdout + r.stderr).splitlines() if ': error ' in l})
            if errs:
                failed = True
                print(f'== {asm}: {len(errs)} error(s)')
                for e in errs[:80]:
                    print(e)
                break
            print(f'== {asm}: OK')
            if only == asm:
                break
    finally:
        shutil.rmtree(out, ignore_errors=True)
    return 1 if failed else 0


if __name__ == '__main__':
    run_main(main)
