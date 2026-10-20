using System.Linq;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FXmasLightsNode @ 0x1805214..0x180cd7c, Ghidra vaddr).
public class FXmasLightsNode : FNode
{
	protected const float cTimeBetweenUpdates = 1f;

	protected const float cMaxSecretDistance = 15f;

	protected static Color[] cLightColors = new Color[3]
	{
		new Color(1f, 0f, 0f, 1f),
		new Color(1f, 0.92156863f, 0.015686275f, 1f),
		new Color(0f, 1f, 0f, 1f)
	};

	protected FSprite[] mLights;

	protected FNodeStack mLightStack;

	protected FSprite mBG;

	protected float mLastUpdate;

	public FXmasLightsNode(string name)
		: base(name)
	{
		mBG = new FSprite(base.Name + ".BG", "BlackSquare.png");
		mBG.ExpandToFill(this);
		mBG.SetParent(this);
		mLightStack = new FNodeStack(base.Name + ".LightStack", Futile.Screen.IsWidescreen ? FNodeStack.Modes.CenteredY : FNodeStack.Modes.CenteredX, 16f);
		mLightStack.SetParent(mBG);
	}

	protected void Layout()
	{
		bool isWidescreen = Futile.Screen.IsWidescreen;
		Rect extents = Extents;
		float lightSize = (isWidescreen ? extents.width : extents.height) - 16f;
		if (mLights == null)
		{
			mLights = new FSprite[3];
			for (int i = 0; i < mLights.Length; i++)
			{
				FSprite fSprite = new FSprite(base.Name + ".Light_" + i, "WhiteSquare.png");
				mLights[i] = fSprite;
				mLightStack.StackChild(fSprite, new Vector2(lightSize, lightSize));
			}
		}
		bool isWidescreen2 = Futile.Screen.IsWidescreen;
		FNodeStack lightStack = mLightStack;
		FSprite bg = mBG;
		Rect extents2 = Extents;
		float width;
		float height;
		if (!isWidescreen2)
		{
			width = extents2.width - 32f;
			height = lightSize;
		}
		else
		{
			height = extents2.height - 32f;
			width = lightSize;
		}
		lightStack.DockInside(bg, InnerDock.Right, width, height);
	}

	public override void SelfResizeExtents()
	{
		base.SelfResizeExtents();
		Layout();
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();
		Layout();
		if (Futile.Instance != null)
		{
			Futile.Instance.AddSignalUpdate(HandleUpdate);
		}
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();
		if (Futile.Instance != null)
		{
			Futile.Instance.RemoveSignalUpdate(HandleUpdate);
		}
	}

	protected override void HandleUpdate()
	{
		base.HandleUpdate();
		if (FApp.World != null && FApp.World.State == FWorld.GameState.LOGIC_PAUSED)
		{
			return;
		}
		if (!(Time.time - mLastUpdate < 1f))
		{
			UpdateLights();
		}
	}

	protected void UpdateLights()
	{
		if (FApp.World == null)
		{
			return;
		}
		bool found = false;
		float minDist = 15f;
		foreach (FRoom room in FApp.World.Rooms)
		{
			foreach (FProtoUnitStartData item in room.ActiveStartDatas.Where((FProtoUnitStartData sd) => sd.IsSecret()))
			{
				FPoint2 position = item.Position;
				float dist = position.DistanceTo(FWorld.Player.TilePosition);
				if (dist < minDist)
				{
					found = true;
					minDist = dist;
				}
			}
		}
		float t = Mathf.Clamp01((minDist - 15f) / -13f);
		if (mLights != null)
		{
			int length = mLights.Length;
			for (int i = 0; i < mLights.Length; i++)
			{
				if (t < 1f / (float)(length - 1) * (float)i || !found)
				{
					mLights[i].Color = Color.gray;
				}
				else
				{
					mLights[i].Color = cLightColors[i];
				}
			}
			mLastUpdate = Time.time;
		}
	}
}
