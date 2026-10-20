# Supported game versions

| Game version | versionCode | Unity | global-metadata.dat SHA-256 | libil2cpp.so (arm64) SHA-256 |
|---|---|---|---|---|
| 1.0.605 | 261010701 | 6000.2.6f2 | `64aed75a…2a400dd1` | `ce4308ae…9b086abb` |

The full hashes are in [tools/supported.json](../tools/supported.json), and setup checks them.

The port's C# was re-implemented against this exact build. Other versions have different code,
data layouts and asset sets, so they are refused by default (`--allow-unsupported` overrides
that, at your own risk).

## Adding a version

Supporting another version is real porting work, not a configuration change:

1. **Export the new APK.** Run `tools/setup.py --allow-unsupported --keep-cache`, then compare
   `.cache/export/ExportedProject` with the current one: scripts, serialized fields, and the asset
   list.
2. **Port the logic changes.** Carry any changed game logic over into the C# under
   `UnityProject/Assets`.
3. **Register the version.** Add an entry to `versions` in `tools/supported.json`. Update the
   `required` minimum file counts, `guid_pins` paths and `dll_patch` file list if they changed.
4. **Test.** Run setup, build and play through.
