using UnityEngine;

public class FVisObject : FGOTreeItemInterface, IDelayedDispose, IAnimTagResponder
{
	private Vector3 mPositionCache;

	public GameObject GO { get; set; }

	public float AnimSpeed { get; protected set; }

	public FAnimDef AnimDef { get; protected set; }

	public Vector3 Position
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public Vector3 Center => default;

	public Vector3 VisualHead => default;

	public Vector3 Forward
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	public Quaternion Rotation
	{
		get
		{
			return default;
		}
		set
		{
		}
	}

	Color FGOTreeItemInterface.PlayerColor => default;

	Color FGOTreeItemInterface.PlayerColorB => default;

	Relationship FGOTreeItemInterface.Relationship => Relationship.None;

	public virtual bool IsLeftFacing => false;

	public virtual int CrystalVanityID => 0;

	public void OverrideAnimDef(FAnimDef animDef)
	{
	}

	public FVisObject()
	{
	}

	public FVisObject(GameObject go)
	{
	}

	public virtual void Init()
	{
	}

	public FFadeLifespan BeginFadeLifespan(float delaySeconds, float fadeSeconds, float lingerSeconds)
	{
		return null;
	}

	public virtual void DelayedDispose()
	{
	}

	public void SetForward(Vector3 f)
	{
	}

	public void SetRotation(Quaternion q)
	{
	}

	public void TurnOn()
	{
	}

	public void TurnOff()
	{
	}

	public void ProcessSpawnTag(FAnimTagSpawn spawnTag, FVisObject source, FVisObject target)
	{
	}

	public void ProcessImpactEffectTag(FAnimTagImpactEffect tag, FVisObject source, FVisObject target)
	{
	}

	public void SetAnimSpeed(float speed)
	{
	}

	public bool PlayAnim(string animName, float animSpeed, int actionID, bool crossfade = true, float startAnimTimeNormalized = -1f)
	{
		return false;
	}

	public bool PlayAnim(AnimTypes animType, float animSpeed, int actionID)
	{
		return false;
	}

	public bool PlayAnim(int animType, float animSpeed, int actionID, bool crossfade, float startAnimTimeNormalized = -1f)
	{
		return false;
	}

	public virtual void FireTag(FAnimTag tag, int actionID)
	{
	}

	public virtual void FireLoopTag(int actionID)
	{
	}

	public virtual void FireEndTag(int actionID)
	{
	}

	public bool PlaySoundSet(string soundSetName)
	{
		return false;
	}
}
