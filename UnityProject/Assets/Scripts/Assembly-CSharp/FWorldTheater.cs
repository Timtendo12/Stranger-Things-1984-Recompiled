using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FWorldTheater @ 0x176925c..0x1769a00 + iterators to 0x176a4a0, Ghidra vaddr).
public class FWorldTheater : FWorld
{
	protected List<FAnimationController> mCharacters = new List<FAnimationController>();

	public new static IEnumerator Init(string worldName, FPlayerStartParams startParams = null)
	{
		if (FApp.WorldManager.ProtoWorlds == null)
		{
			yield break;
		}
		FProtoWorld worldToUse = null;
		foreach (KeyValuePair<string, FProtoWorld> protoWorld in FApp.WorldManager.ProtoWorlds)
		{
			if (protoWorld.Key.Equals(worldName, StringComparison.OrdinalIgnoreCase))
			{
				worldToUse = protoWorld.Value;
				break;
			}
		}
		if (worldToUse == null)
		{
			yield break;
		}
		GameObject gameObject = new GameObject("LegendWorld_Theater");
		FWorldTheater world = gameObject.AddComponent<FWorldTheater>();
		FApp.World = world;
		world.ProtoWorld = worldToUse;
		world.mPather = new FPather(world);
		world.mPather.SetStopRange(0);
		world.LoadMap(startParams);
		yield return world.StartCoroutine(world.PostSceneLoad());
	}

	protected override void LoadSpecialUnits(FPoint2 playerstart)
	{
	}

	public FAnimationController SpawnDiscreteAnimation(string animDefName, string animName, int sortingOrder = 200)
	{
		FAnimDef animDef = FAnimDefManager.GetAnimDef(animDefName);
		if (animDef == null)
		{
			FLog.UnityWarning("No anim def for visual layer '" + animDefName + "' found.");
			return null;
		}
		int animTypeID = FAnimDefManager.GetAnimTypeID(animName);
		if (animTypeID < 0)
		{
			return null;
		}
		GameObject gameObject = new GameObject(ProtoWorld.Name + "_EffectAnim_" + animDefName);
		SpriteRenderer spriteRenderer = gameObject.GetOrCreateComponent<SpriteRenderer>();
		spriteRenderer.sortingLayerName = "Foreground";
		gameObject.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localEulerAngles = Vector3.zero;
		gameObject.transform.SetAsFirstSibling();
		FAnimationController fAnimationController = new FAnimationController(animDefName, animDef, gameObject);
		fAnimationController.SetFacing(FAnimEntry.AnimFacing.DOWN);
		fAnimationController.PlayAnim(animTypeID, true);
		return fAnimationController;
	}

	protected override IEnumerator PostSceneLoad(bool revealWorld = true)
	{
		InitCamera();
		FApp.HUD.RemoveAllUIPages();
		FApp.HUD.PushPage(FApp.HUD.PageLegendWorld, true, false);
		FApp.HUD.PushPage(FApp.HUD.PageTheaterWorld, true, false);
		float orthoSize = CameraOrthographicSize;
		int screenWidth = PCDisplay.Width;
		int screenHeight = PCDisplay.Height;
		Rect cameraRect = mCamera.rect;
		FProtoUnit[] playerCharacters = FApp.ProtoUnitManager.GetProtoUnitsWithTag("PlayerCharacter");
		for (int i = 0; i < playerCharacters.Length; i++)
		{
			FProtoUnit proto = playerCharacters[i];
			if (proto.CheckUnlockRequirements())
			{
				FAnimationController anim = SpawnDiscreteAnimation(proto.Name, "Idle", 200);
				mCharacters.Add(anim);
				ColorAllSprites(anim.GO, new Color(0.4f, 0.4f, 0.4f, 1f));
				anim.SetFacing(FAnimEntry.AnimFacing.UP);
				anim.GO.transform.localScale = new Vector3(2f, 2f, 2f);
				Vector3 camPos = mCamera.transform.position;
				anim.GO.transform.position = new Vector3((float)i * 1.5f + 1f + (camPos.x - orthoSize * (float)screenWidth / (float)screenHeight * cameraRect.width), camPos.y - orthoSize + 0.5f, camPos.z + 10f);
			}
		}
		mRenderStage = new FStage("World2");
		mRenderStage.SetShouldSortByZ(true);
		Futile.AddStageAtIndex(mRenderStage, 0);
		mRenderStage.MarkDirtyRecursive();
		FVideoPlayer.LoopPointReachedAction += LoopPointReached;
		FVideoPlayer.PlayVideo(ProtoWorld.LoadData["VideoName"]);
		mInputEnabled = false;
		FApp.HUD.PageLegendWorld.DimWorld(0f, 0.5f);
		yield break;
	}

	public void ExitTheater()
	{
		FVideoPlayer.EndVideo();
	}

	protected void LoopPointReached()
	{
		FVideoPlayer.LoopPointReachedAction -= LoopPointReached;
		StartCoroutine(ResetPlayer(0f));
	}

	public override void FixedUpdate()
	{
		mGametime += Time.fixedDeltaTime;
		foreach (FAnimationController mCharacter in mCharacters)
		{
			mCharacter.Update(Time.fixedDeltaTime);
		}
	}

	public override void LateUpdate()
	{
	}
}
