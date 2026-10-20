using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FWorldCinematic @ 0x1767c18..0x17680d0, Ghidra vaddr).
public class FWorldCinematic : FWorld
{
	protected float mWatchTime = 5f;

	protected bool mStarted;

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
		GameObject gameObject = new GameObject("LegendWorld_Cinematic");
		FWorldCinematic world = gameObject.AddComponent<FWorldCinematic>();
		FApp.World = world;
		world.ProtoWorld = worldToUse;
		world.mPather = new FPather(world);
		world.mPather.SetStopRange(0);
		world.LoadMap(startParams);
		yield return world.StartCoroutine(world.PostSceneLoad());
	}

	protected override IEnumerator PostSceneLoad(bool revealWorld = true)
	{
		FWorld.Player.SetVisible(false);
		FWorld.Player.SetObstructs(false);
		mInputEnabled = false;
		yield return StartCoroutine(base.PostSceneLoad(true));
		FApp.HUD.PageTheaterWorld.ShowCloseButton(false);
		if (ProtoWorld.LoadData.ContainsKey("Time"))
		{
			mWatchTime = int.Parse(ProtoWorld.LoadData["Time"]);
		}
		mInputEnabled = false;
		mStarted = true;
		if (ProtoWorld.LoadData.ContainsKey("Dialogue"))
		{
			FGameUtil.DelayExecute(2f, TryStartDialogue);
		}
	}

	protected void TryStartDialogue()
	{
		TryShowDialogue("", ProtoWorld.LoadData["Dialogue"]);
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		float fixedDeltaTime = Time.fixedDeltaTime;
		if (!mStarted || FApp.HUD.PageLegendWorld.DialogueOnScreen)
		{
			return;
		}
		mWatchTime -= fixedDeltaTime;
		if (!(mWatchTime > 0f))
		{
			FApp.HUD.PageLegendWorld.Color = Color.white;
			mStarted = false;
			if (!ProtoWorld.LoadData.ContainsKey("LeadsTo"))
			{
				FApp.DestroyWorld(false);
				StartCoroutine(Init(FApp.PlayerStats.CurrentWorld, new FPlayerStartParams()));
			}
			else
			{
				TryWorldTransition(ProtoWorld.LoadData["LeadsTo"], null);
			}
		}
	}
}
