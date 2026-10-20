# Third-party notices

The GNU GPL v3 in [LICENSE](LICENSE) covers the original work in this repository: the PC-specific code
(`PCInput`, `PCDisplay`, `PCDiscord`, `DiscordIpc`, `PortBuild`), the tooling in `tools/`, and the
documentation.

The components below were part of the original game. They are reproduced, or re-implemented
from the game binary, under their own licenses, and those licenses keep applying to them. If a
component's license conflicts with the GPL, the component's own license governs those files.

| Component | Location | License | Upstream |
|---|---|---|---|
| Futile (2D framework) | `UnityProject/Assets/Plugins/Assembly-CSharp-firstpass/` (engine classes such as `Futile.cs`, `FSprite.cs`, `FStage.cs`, `FLabel.cs`, `FAtlasManager.cs`) | MIT, © Matt Rix and contributors | https://github.com/MattRix/Futile |
| GoKit (tweening) | `UnityProject/Assets/Plugins/Assembly-CSharp-firstpass/` (`Go*.cs`, `*Tween*.cs`) | GoKit license: free to use in games, modified or not. The source may not be sold or commercially distributed outside a game | https://github.com/prime31/GoKit |
| Arabic support for Unity (ArabicFixer) | `UnityProject/Assets/Plugins/Assembly-CSharp-firstpass/Arabic*.cs` | MIT, © Abdulla Konash | https://github.com/Konash/arabic-support-unity |
| MiniJSON | `UnityProject/Assets/Plugins/Assembly-CSharp-firstpass/MiniJSON/`, `MiniJSON*.cs` | MIT, © Calvin Rien | https://gist.github.com/darktable/1411710 |
| TiledSharp | `UnityProject/Assets/Scripts/Assembly-CSharp/ThirdParty/TiledSharp/` | Apache License 2.0, © Marshall Ward | https://github.com/marshallward/TiledSharp |
| RetroTVFX | `UnityProject/Assets/Scripts/Assembly-CSharp/ThirdParty/JetFistGames/RetroTVFX/` | MIT, © KillaMaaki (GlaireDaggers) | https://github.com/GlaireDaggers/RetroTVFX |
| PreviewLabs PlayerPrefs | `UnityProject/Assets/Scripts/Assembly-CSharp/ThirdParty/PreviewLabs/` | Free for use in any project, per PreviewLabs | http://www.previewlabs.com |
| LZMA SDK (C#) | `UnityProject/Assets/Scripts/BXPCoreUtil/ThirdParty/SevenZip/` | Public domain, Igor Pavlov | https://www.7-zip.org/sdk.html |
| NPack MersenneTwister (MT19937) | `UnityProject/Assets/Scripts/BXPCoreUtil/ThirdParty/NPack/` | BSD-style MT19937 license, © Makoto Matsumoto and Takuji Nishimura; C# port from NPack | http://www.math.sci.hiroshima-u.ac.jp/m-mat/MT/emt.html |
| Newtonsoft.Json | Unity package `com.unity.nuget.newtonsoft-json` (fetched by Unity, not included) | MIT, © James Newton-King | https://github.com/JamesNK/Newtonsoft.Json |

## Tools used by setup (downloaded, not included)

| Tool | License | Upstream |
|---|---|---|
| AssetRipper 2.0.0 | GPL-3.0 | https://github.com/AssetRipper/AssetRipper |

## Game content

Stranger Things is a trademark of Netflix, Inc. *Stranger Things: 1984* was developed by BonusXP
and published by Netflix. This repository contains none of the game's assets (art, audio, video,
text, levels, scenes or data). `tools/setup.py` produces them on your own computer from an APK
you supply. Do not commit or redistribute those generated files.

The rest of the C# under `UnityProject/Assets/Scripts` and `UnityProject/Assets/Plugins` is a
re-implementation of the game's logic, written to interoperate with your copy of the game's
assets. This project is not affiliated with, endorsed by, or sponsored by Netflix or BonusXP.
