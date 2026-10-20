# Coverage

Snapshot of how much of the original game logic has been re-implemented (generated during development).

Class = one .cs file. rewritten = has "Rewritten from" header; stub-bodied = every method body empty/trivial return, no header; other = data/enum/attribute or hand-written.

| Assembly | classes | rewritten | stub-bodied | other (data/hand) | methods | stub methods |
|---|---|---|---|---|---|---|
| Assembly-CSharp-firstpass | 332 | 227 | 46 | 59 | 1945 | 281 |
| Assembly-CSharp | 412 | 280 | 49 | 83 | 2917 | 269 |
| BXPCoreUtil | 159 | 37 | 25 | 97 | 1364 | 220 |

## Stub-bodied classes

- Assembly-CSharp/ActiveConnection (1 methods) - dead: HTTP / network / platform share code; the PC build is offline and nothing calls it
- Assembly-CSharp/Arc4RandomNumberGenerator (10 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/AudioOffset (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/BRDFLookupTexture (8 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/CRTEffect (8 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/DeferredDecalRenderer (5 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/DeferredDecalSystem (3 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FAppInit (1 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp/FBillboard (1 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FDebugCollisionVolumeRenderer (6 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp/FDebugFileEditor (1 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp/FDebugLineRenderer (18 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp/FDebugLineRendererMeshObject (8 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp/FExplosionForceInArea (1 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FFontMesh (3 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FFontMeshTextLine (1 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FFontMeshTextSetter (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FForceController (5 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FForceInArea (9 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FImpactPoints (6 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FInterpQuaternion (8 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FInterpVector3 (8 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FLineRendererNoBillboard (12 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FLineStackEntry (3 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FLineStackEntryLine (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FLineStackNode (3 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp/FNetflixSdkExtensions (1 methods) - dead: HTTP / network / platform share code; the PC build is offline and nothing calls it
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
- Assembly-CSharp-firstpass/FDynamicAtlas (8 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FEditorTempObject (2 methods) - dead: debug / editor-only tooling, never instantiated at runtime
- Assembly-CSharp-firstpass/FGradientNode (9 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FMaterialPropertyHelper (1 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FMaterialPropertyHelperExtensions (4 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FParseUnityTypes (4 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FParticleCollisionPlane (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FRadialWipeSprite (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FRagdollHelper (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FRepeatSprite (5 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
- Assembly-CSharp-firstpass/FSaveGameWriterExtensions (2 methods) - dead: no references in code, scenes or prefabs (legacy 3D / rendering / UI helper from the original engine)
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
- BXPCoreUtil/CombinationStream (16 methods) - dead: zip / deflate codec internals, only reachable through the unused Zip/GZip classes
- BXPCoreUtil/CopyEncoder (2 methods) - dead: zip / deflate codec internals, only reachable through the unused Zip/GZip classes
- BXPCoreUtil/FBXPCoreUtil (1 methods) - dead: unreferenced BXPCoreUtil helper (fixed-point variants, savegame / command-line / log-file utilities)
- BXPCoreUtil/FCommandLineParser (2 methods) - dead: unreferenced BXPCoreUtil helper (fixed-point variants, savegame / command-line / log-file utilities)
- BXPCoreUtil/FFuncComparer (1 methods) - dead: unreferenced BXPCoreUtil helper (fixed-point variants, savegame / command-line / log-file utilities)
- BXPCoreUtil/FLogFile (6 methods) - dead: unreferenced BXPCoreUtil helper (fixed-point variants, savegame / command-line / log-file utilities)
- BXPCoreUtil/FLoggedRandom (13 methods) - dead: unreferenced BXPCoreUtil helper (fixed-point variants, savegame / command-line / log-file utilities)
- BXPCoreUtil/FMathUtil (2 methods) - dead: unreferenced BXPCoreUtil helper (fixed-point variants, savegame / command-line / log-file utilities)
- BXPCoreUtil/FMultiDictionary (12 methods) - dead: unreferenced BXPCoreUtil helper (fixed-point variants, savegame / command-line / log-file utilities)
- BXPCoreUtil/FParseCoreUtilTypes (16 methods) - dead: unreferenced BXPCoreUtil helper (fixed-point variants, savegame / command-line / log-file utilities)
- BXPCoreUtil/FPoint2Comparer (2 methods) - dead: unreferenced BXPCoreUtil helper (fixed-point variants, savegame / command-line / log-file utilities)
- BXPCoreUtil/FSaveVersionInfo (1 methods) - dead: unreferenced BXPCoreUtil helper (fixed-point variants, savegame / command-line / log-file utilities)
- BXPCoreUtil/FStringBuilderExtensions (1 methods) - dead: unreferenced BXPCoreUtil helper (fixed-point variants, savegame / command-line / log-file utilities)
- BXPCoreUtil/FXmlElementExtensionsCoreUtilTypes (15 methods) - dead: unreferenced BXPCoreUtil helper (fixed-point variants, savegame / command-line / log-file utilities)
- BXPCoreUtil/FXmlJsonValueConverter (3 methods) - dead: unreferenced BXPCoreUtil helper (fixed-point variants, savegame / command-line / log-file utilities)
- BXPCoreUtil/FastEncoder (15 methods) - dead: zip / deflate codec internals, only reachable through the unused Zip/GZip classes
- BXPCoreUtil/FastEncoderWindow (14 methods) - dead: zip / deflate codec internals, only reachable through the unused Zip/GZip classes
- BXPCoreUtil/Fix16 (33 methods) - dead: unreferenced BXPCoreUtil helper (fixed-point variants, savegame / command-line / log-file utilities)
- BXPCoreUtil/Fix8 (17 methods) - dead: unreferenced BXPCoreUtil helper (fixed-point variants, savegame / command-line / log-file utilities)
- BXPCoreUtil/HuffmanTree (7 methods) - dead: zip / deflate codec internals, only reachable through the unused Zip/GZip classes
- BXPCoreUtil/Inflater (11 methods) - dead: zip / deflate codec internals, only reachable through the unused Zip/GZip classes
- BXPCoreUtil/InputBuffer (11 methods) - dead: zip / deflate codec internals, only reachable through the unused Zip/GZip classes
- BXPCoreUtil/OutputBuffer (12 methods) - dead: zip / deflate codec internals, only reachable through the unused Zip/GZip classes
- BXPCoreUtil/OutputWindow (6 methods) - dead: zip / deflate codec internals, only reachable through the unused Zip/GZip classes
- BXPCoreUtil/SR (1 methods) - dead: unreferenced BXPCoreUtil helper (fixed-point variants, savegame / command-line / log-file utilities)

## Intentionally unported (unused)

Dead 3D / network / savegame code with no callers on PC; left as stubs on purpose.

- Assembly-CSharp/Decal (6 methods)
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
- Assembly-CSharp-firstpass/FFixedAABBTreeExtensions (3 methods)
- Assembly-CSharp-firstpass/FFixedPointHelpers (7 methods)
- Assembly-CSharp-firstpass/FGameObjectNode (23 methods)
- BXPCoreUtil/DeflateInput (3 methods)
- BXPCoreUtil/DeflateStream (31 methods)
- BXPCoreUtil/DeflateStreamAsyncResult (10 methods)
- BXPCoreUtil/DeflaterManaged (5 methods)
- BXPCoreUtil/FBinReader (37 methods)
- BXPCoreUtil/FBinWriter (37 methods)
- BXPCoreUtil/FFixAABB (23 methods)
- BXPCoreUtil/FFixAABBNode (8 methods)
- BXPCoreUtil/FFixAABBTreeMesh (16 methods)
- BXPCoreUtil/FFixHermiteCurve (11 methods)
- BXPCoreUtil/FFixHermiteSpline (14 methods)
- BXPCoreUtil/FFixTransform (7 methods)
- BXPCoreUtil/FSavegameReader (4 methods)
- BXPCoreUtil/FSavegameStaticData (4 methods)
- BXPCoreUtil/FSavegameWriter (35 methods)
- BXPCoreUtil/FixMatrix4x4 (28 methods)
- BXPCoreUtil/FixQuaternion (54 methods)
- BXPCoreUtil/FixVector2 (35 methods)
- BXPCoreUtil/FixVector3 (60 methods)
- BXPCoreUtil/FixVector4 (28 methods)
- BXPCoreUtil/GZipDecoder (5 methods)
- BXPCoreUtil/GZipFormatter (4 methods)
- BXPCoreUtil/GZipStream (15 methods)
- BXPCoreUtil/ZipStorer (23 methods)

## Partially stubbed (no header, some trivial members)

- Assembly-CSharp-firstpass/FAssetPreloader (4 of 17 members trivial)
- BXPCoreUtil/Decoder (1 of 9 members trivial)
- BXPCoreUtil/Encoder (1 of 11 members trivial)
- BXPCoreUtil/FastEncoderStatics (1 of 2 members trivial)
- BXPCoreUtil/InBuffer (1 of 6 members trivial)
- BXPCoreUtil/InWindow (2 of 12 members trivial)
- BXPCoreUtil/OutBuffer (1 of 8 members trivial)
