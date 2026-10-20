# Coverage

Snapshot of how much of the original game logic has been re-implemented (generated during development).

Class = one .cs file. rewritten = has "Rewritten from" header; stub-bodied = every method body empty/trivial return, no header; other = data/enum/attribute or hand-written.

| Assembly | classes | rewritten | stub-bodied | other (data/hand) | methods | stub methods |
|---|---|---|---|---|---|---|
| Assembly-CSharp-firstpass | 327 | 225 | 45 | 57 | 1915 | 277 |
| Assembly-CSharp | 405 | 280 | 42 | 83 | 2868 | 217 |
| BXPCoreUtil | 86 | 39 | 0 | 47 | 623 | 0 |

## Stub-bodied classes

- Assembly-CSharp/ActiveConnection (1 methods) - dead: HTTP / network / platform share code; the PC build is offline and nothing calls it
- Assembly-CSharp/Arc4RandomNumberGenerator (10 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/AudioOffset (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/BRDFLookupTexture (8 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/CRTEffect (8 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FAppInit (1 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp/FBillboard (1 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FDebugCollisionVolumeRenderer (6 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp/FDebugFileEditor (1 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp/FExplosionForceInArea (1 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FFontMesh (3 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FFontMeshTextLine (1 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FFontMeshTextSetter (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FForceController (5 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FForceInArea (9 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FImpactPoints (6 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FLineRendererNoBillboard (11 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FLineStackEntry (3 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FLineStackEntryLine (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FLineStackNode (3 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FNodeCircleTimer (5 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FNodeDialogBubble (4 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FObjectSpawner (3 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FPageDebugDockInside (5 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp/FPageDebugDockOutside (5 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp/FRaycastResult (1 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FRaycaster (5 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FRefractHelper (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FReplaceMaterialEntry (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FReplaceTextureEntry (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FResourceEntry (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FSelectionHack (3 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp/FUIHorizontalScrollbar (17 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FUIScrollListHorizontal (5 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FVersionLabel (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FetchImage (5 methods) - dead: HTTP / network / platform share code; the PC build is offline and nothing calls it
- Assembly-CSharp/FetchStream (2 methods) - dead: HTTP / network / platform share code; the PC build is offline and nothing calls it
- Assembly-CSharp/JsonSerializer (23 methods) - dead: HTTP / network / platform share code; the PC build is offline and nothing calls it
- Assembly-CSharp/Request (18 methods) - dead: HTTP / network / platform share code; the PC build is offline and nothing calls it
- Assembly-CSharp/Response (11 methods) - dead: HTTP / network / platform share code; the PC build is offline and nothing calls it
- Assembly-CSharp/SimpleWWW (9 methods) - dead: HTTP / network / platform share code; the PC build is offline and nothing calls it
- Assembly-CSharp/WWWFormTest (2 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp-firstpass/CameraExtensions (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/DebugExtension (49 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp-firstpass/Example (2 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp-firstpass/FAnimRunner (8 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FAnimStateAnimRunner (4 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FAnimToCurveHelper (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FAssetBundleManager (38 methods) - dead: already a PC shim that loads from Resources (looks like a stub only because the bodies are one-liners)
- Assembly-CSharp-firstpass/FAttachPositionOnly (4 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FAttachPositionOnlyHelper (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FBeamRenderer (22 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FCameraPreCull (1 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FCircularCutout (4 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FDebugMeHelpers (2 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp-firstpass/FDynamicAtlas (6 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FEditorTempObject (2 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp-firstpass/FGradientNode (9 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FMaterialPropertyHelper (1 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FMaterialPropertyHelperExtensions (4 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FParseUnityTypes (4 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FParticleCollisionPlane (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FRadialWipeSprite (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FRagdollHelper (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FRepeatSprite (5 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FSetRenderQueue (1 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FShaderReplacementExtensions (1 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FShaderReplacementManager (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FSkeletonRemap (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FSkeletonRemapTracker (6 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FSkeletonRemapTrackerExtensions (1 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FSlotBoneNames (4 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FSpriteAnim (5 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FSpriteAnimFrame (1 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FSpriteAnimManager (3 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FTerrainDecal (6 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FTrailScaler (3 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FTrailScalerHelper (1 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FWebRequest (12 methods) - dead: HTTP / network / platform share code; the PC build is offline and nothing calls it
- Assembly-CSharp-firstpass/FWebRequestRunner (4 methods) - dead: HTTP / network / platform share code; the PC build is offline and nothing calls it
- Assembly-CSharp-firstpass/FXmlElementExtensionsUnityTypes (6 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/MiniJSONAmzn (21 methods) - dead: HTTP / network / platform share code; the PC build is offline and nothing calls it
- Assembly-CSharp-firstpass/MiniJSONAmznExtensions (4 methods) - dead: HTTP / network / platform share code; the PC build is offline and nothing calls it
- Assembly-CSharp-firstpass/NativeShare (2 methods) - dead: HTTP / network / platform share code; the PC build is offline and nothing calls it
- Assembly-CSharp-firstpass/RXUtils (7 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/RXWatcher (1 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/TextureScale (7 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)

## Intentionally unported (unused)

Dead 3D / network / savegame code with no callers on PC; left as stubs on purpose.

- Assembly-CSharp/FDownloadCacheEntry (6 methods)
- Assembly-CSharp/FDownloadEntry (8 methods)
- Assembly-CSharp/FDownloadManager (10 methods)
- Assembly-CSharp/FGOTreeBuilder (1 methods)
- Assembly-CSharp/FGOTreeManager (29 methods)
- Assembly-CSharp/FGameObjectWrapper (26 methods)
- Assembly-CSharp/FObjectManager (10 methods)
- Assembly-CSharp/FObjectManagerEventHandler (2 methods)
- Assembly-CSharp/FPanelHistoryItem (3 methods)
- Assembly-CSharp/FPanelHistoryItemData (1 methods)
- Assembly-CSharp/FPanelHistoryLog (25 methods)
- Assembly-CSharp/FPanelRecordGameControl (6 methods)
- Assembly-CSharp/FPanelTextEntry (11 methods)
- Assembly-CSharp/FRefractionRenderer (7 methods)
- Assembly-CSharp/FResourceBucket (38 methods)
- Assembly-CSharp/FResourceBucketSerializer (3 methods)
- Assembly-CSharp/FVisObject (24 methods)
- Assembly-CSharp/FWindClothController (2 methods)
- Assembly-CSharp/FWindFXController (4 methods)
- Assembly-CSharp/FWindGenerator (8 methods)
- Assembly-CSharp/WebSocket (27 methods)
- Assembly-CSharp/WebSocketExample (2 methods)
- Assembly-CSharp-firstpass/FDownloadHandlerFile (7 methods)
- Assembly-CSharp-firstpass/FDownloadRequest (8 methods)
- Assembly-CSharp-firstpass/FDownloadRequestManager (14 methods)
- Assembly-CSharp-firstpass/FGameObjectNode (23 methods)

## Partially stubbed (no header, some trivial members)

- Assembly-CSharp-firstpass/FAssetPreloader (4 of 17 members trivial)
- BXPCoreUtil/Decoder (1 of 9 members trivial)
- BXPCoreUtil/Encoder (1 of 11 members trivial)
- BXPCoreUtil/InBuffer (1 of 6 members trivial)
- BXPCoreUtil/InWindow (2 of 12 members trivial)
- BXPCoreUtil/OutBuffer (1 of 8 members trivial)

## Removed dead code

Signature-only stubs with no runtime callers were deleted rather than kept: the binary savegame layer
(FBinReader/FBinWriter, FSavegameReader/Writer, FSaveRef and the Save/Load(FBin*) members that used it; real saves
go through FSaveFile as JSON + LZMA), fixed-point 3D math (FixVector2/3/4, FixQuaternion, FixMatrix4x4, FFixAABB*,
FFixHermite*, FFixTransform, Fix8, Fix16), the Unity.IO.Compression deflate/gzip port, ZipStorer, CombinationStream,
the LZMA SDK command-line parser, the deferred decal system (Decal, DeferredDecalSystem/Renderer; no scene, prefab or code used it) and unreferenced BXPCoreUtil helpers. Their signatures remain in the original decompile.
